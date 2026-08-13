using GameServer.Application.Common;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Entities;
using GameServer.Domain.Entities.BeastiaryEntity;
using GameServer.Domain.Entities.BeastiaryEntity.BestiaryLibrary;
using GameServer.Domain.Enums;
using GameServer.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace GameServer.Application.Services;

public sealed class EntityService(EntityStore entityStore, BestiaryIndex index, InventoryService inventoryService, SkillService skillService)
{
    private readonly EntityStore _entities = entityStore;
    private readonly BestiaryIndex _index = index;
    private readonly InventoryService _inventoryService = inventoryService;
    private readonly SkillService _skillService = skillService;

    private bool TryGetEntity(string id, [NotNullWhen(true)] out DamageableEntity? target)
    {
        return _entities.TryGet(id, out target) && target is not null;
    }

    private static bool TryParseDamageType(string damageType, out DamageType result)
    {
        return Enum.TryParse(damageType, true, out result);
    }

    public string[] GetAllNames()
    {
        var names = _entities.GetAllNames();
        return names;

    }

    public string[] GetAllIds()
    {
        var ids = _entities.GetAllIds();
        return ids;
    }

    public List<DamageableEntityDto> GetParty(string partyId)
    {
        var result = _entities.GetParty(partyId);
        return result.ToDtos();
    }

    public DamageableEntityDto? GetById(string id)
    {
        if (!TryGetEntity(id, out var entity))
        {
            return null;
        }
        return entity.ToDto();
    }

    public AddEntityResult AddEntityFromRequest(DamageableEntityRequest request)
    {
        AddEntityResult result = new();

        var resistanceResult = request.Resistances.Parse();
        result.AddErrors(resistanceResult.Errors);

        var proficiencyResult = request.Proficiencies.Parse();
        result.AddErrors(proficiencyResult.Errors);

        var proficiencies = (request.Proficiencies is null || request.Proficiencies.Count < 1)
            ? new Dictionary<Proficiency, double>
                {
                    {Proficiency.bludgeoning, 0.85d},
                    {Proficiency.potions, 0.85d},
                    {Proficiency.slashing, 0.65d},
                    {Proficiency.healing, 0.6d}
                }
            : proficiencyResult.Parsed;
        
        if (!Enum.TryParse<DamageType>(request.AttackType, out var dtAttackType))
        {
            result.Errors.Add(new ParseIssue(
                $"{request.AttackType}",
                $"{request.AttackType} is not a valid damage type."
            ));
        }

        string? attackString = null;
        if (request.DefaultAttackMessageString.Length > 0)
        {
            attackString = request.DefaultAttackMessageString;
        }
        
        if (result.IsValid())
        {
            DamageableEntity entity = new(
                name: request.Name,
                entityType: request.EntityType,
                race: request.Race,
                partyId: request.PartyId,
                health: request.Health,
                mana: request.Mana,
                magic: request.Magic,
                strength: request.Strength,
                defense: request.Defense,
                attackType: dtAttackType,
                dealsMagicDamage: request.DealsMagicDamage,
                speed: request.Speed,
                level: request.Level,
                experience: request.Experience,
                resistances: resistanceResult.Parsed,
                proficiencies: proficiencies,
                defaultAttackMessageString: attackString,
                deathMessage: request.DeathMessage
            );

            if (request.ItemTags is not null)
            {
                foreach (string tag in request.ItemTags)
                {
                    var item = _inventoryService.NewItemByTag(tag);
                    if (item is not null)
                    {
                        entity.Inventory.Items.Add(item);
                    }
                }
            }
            if (request.SkilTags is not null)
            {
                foreach (string tag in request.SkilTags)
                {
                    var skill = _skillService.NewSkillByTag(tag);
                    if (skill is not null)
                    {
                        entity.Skills.Add(skill);
                    }
                }
            }
            _entities.Add(entity);
            result.Entity = entity.ToDto();
        }

        return result;
    }

    public AddEntityResult AddBeastiaryEntity(string tag)
    {
        var result = new AddEntityResult();
        var entity = _index.NewBeastiaryEntityByTag(tag);

        // Make sure the result is not the error monster (i.e. is a valid beastiary entity)
        if (entity.Tag.Equals("error", StringComparison.InvariantCultureIgnoreCase))
        {
            return new AddEntityResult
            {
                // If the beast does not exist, do not add it :)
                Errors = [new(
                    $"{nameof(_index.NewBeastiaryEntityByTag)}({tag})",
                    $"The requested tag '{tag}' did not correspond with any valid entity."
                )]
            };
        }

        // Add the items to the entity's inventory from its initial items list
        foreach (string itemTag in entity.ItemTagsForInitialInventory)
        {
            var item = _inventoryService.NewItemByTag(itemTag);
            if (item is not null)
            {
                entity.Inventory.AddItem(item);
            }
            else
            {
                // If the item does not exist, do not add it :)
                result.Errors.Add(new(
                    $"{nameof(_inventoryService.NewItemByTag)}({itemTag})",
                    $"{itemTag} is not a valid item tag"
                ));
            }
        }

        // Add skills to the entity's skills from its initial skills list
        foreach (string skillTag in entity.SkillTagsForInitialSkills)
        {
            var skill = _skillService.NewSkillByTag(skillTag);
            if (skill is not null)
            {
                entity.Skills.Add(skill);
            }
            else
            {
                // If the skill does not exist, do not add it :)
                result.Errors.Add(new(
                    $"{nameof(_skillService.NewSkillByTag)}({skillTag})",
                    $"{skillTag} is not a valid skill tag"
                ));
            }
        }
        result.Entity = entity.ToDto();
        
        // Check if the entity had any problems adding items or skills
        if (result.IsValid())
        {
            _entities.Add(entity);
        }
        return result;
    }

    // Because of polymorphism, this can clone BestiaryEntities or base DamageableEntities
    public AddEntityResult? CloneEntity(string id)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        var entity = target.Clone();
        _entities.Add(entity);
        return new AddEntityResult
        {
            Entity = entity.ToDto(),
        };
    }

    public DamageableEntityDto? FixStats(string id, FixStatsRequest request)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        target.MaxHealth = request.Health;
        target.Magic = request.Magic;
        target.MaxMana = request.Mana;
        target.Strength = request.Strength;
        target.Defense = request.Defense;
        target.Speed = request.Speed;

        return target.ToDto();
    }

    public List<ResistanceDto>? GetAllResistances(string id)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        return target.Resistances.ToResistanceDtos();
    }

    public List<ProficiencyDto>? GetAllProficiencies(string id)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        return target.Proficiencies.ToProficiencyDtos();
    }

    public EnumDictionaryParseResult<DamageType>? SetAllResistances(string id, List<ResistanceRequest> resistances)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        var resistanceResult = resistances.Parse();
        if (!(resistanceResult.Errors.Count > 0))
        {
            if (resistanceResult.Parsed is null)
            {
                target.Resistances.Clear();
            }
            else
            {
                target.Resistances = resistanceResult.Parsed;
            }
        }
        return resistanceResult;
    }

    public EnumDictionaryParseResult<Proficiency>? SetAllProficiencies(string id, List<ProficiencyRequest> requestList)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        var proficiencyResult = requestList.Parse();
        if (!(proficiencyResult.Errors.Count > 0))
        {
            if (proficiencyResult.Parsed is null)
            {
                target.Proficiencies.Clear();
            }
            else
            {
                target.Proficiencies = proficiencyResult.Parsed;
            }
        }
        return proficiencyResult;
    }

    public ResistanceDto? SetResistanceMultiplier(string id, ResistanceRequest request){
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        if (!TryParseDamageType(request.Type, out var damageType))
        {
            return new(string.Empty, 0);
        }
        target.Resistances[damageType] = request.Value;
        return new(damageType.ToString(), request.Value);
    }

    public ProficiencyDto? SetProficiency(string id, ProficiencyRequest request)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        if (!Enum.TryParse(request.Type, out Proficiency profEnum))
        {
            return new($"{request.Type} is not a valid proficiency");
        }
        target.Proficiencies[profEnum] = request.Value;
        return new(
            profEnum.ToString(),
            target.Proficiencies[profEnum]
        );
    }

    public ResistanceDto? IncreaseResistance(string id, ResistanceRequest request)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        if (!TryParseDamageType(request.Type, out var dtEnum))
        {
            return new ResistanceDto
            {
                Error = $"{request.Type} is not a valid resistance"
            };
        }
        return target.IncreaseResistance(dtEnum, request.Value);
    }

    public ProficiencyDto? IncreaseProficiency(string id, ProficiencyRequest request)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        if (!Enum.TryParse(request.Type, out Proficiency profEnum))
        {
            return new($"{request.Type} is not a valid proficiency");
        }
        return target.IncreaseProficiency(profEnum, request.Value);
    }

    public bool? IsHidden(string id)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        return target.IsHidden;
    }

    public bool? ToggleIsHidden(string id)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        target.IsHidden = !target.IsHidden;
        return target.IsHidden;
    }

    public double? GetSpeed(string id)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        return target.Speed;
    }

    public DamageableEntity? GetDamageableEntityObject(string id)
    {
        TryGetEntity(id, out var target);
        return target;
    }

    public List<DamageableEntityDto> RemoveEntitiesNotAliveInParty(string partyId)
    {
        List<DamageableEntityDto> result = [];
        if (partyId.Length > 0)
        {
            var targets = _entities.GetParty(partyId);
            foreach (var e in targets)
            {
                if (!e.IsEntityAlive && !e.DoNotDeleteOnDeath)
                {
                    if (e.DoNotDeleteOnDeath)
                    {
                        e.PartyId = $"map-{OrdinalDateString.GetOrdinalDate(3)}";
                        result.Add(e.ToDto());
                    }
                    result.Add(e.ToDto());
                    _entities.Remove(e);
                }
            }
        }
        return result;
    }

    public DamageableEntityDto? RemoveEntityNotAlive(string id)
    {
        if (TryGetEntity(id, out var target) && !target.IsEntityAlive)
        {
            if (target.IsEntityAlive)
            {
                return new DamageableEntityDto
                {
                    Error = $"{target.Name} is still alive! Don't throw them away just like that!"
                };
            }
            if (target.DoNotDeleteOnDeath)
            {
                target.PartyId = $"map-{OrdinalDateString.GetOrdinalDate(3)}";
                return target.ToDto();
            }
            else
            {
                DamageableEntityDto result = target.ToDto();
                _entities.Remove(target);
                return result;
            }
        }
        return null;
    }
}

// hasAI
// getAI
// setAI
// shadow