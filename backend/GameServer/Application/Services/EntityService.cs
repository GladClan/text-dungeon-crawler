using GameServer.Application.Common;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;
using GameServer.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace GameServer.Application.Services;

public sealed class EntityService(EntityStore entityStore, InventoryService inventoryService, SkillService skillService)
{
    private readonly EntityStore _entities = entityStore;
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

    public AddEntityResult AddEntity(DamageableEntityRequest request)
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
        
        if (result.IsValid())
        {
            DamageableEntity entity = new(
                request.Name,
                request.EntityType,
                request.Race,
                request.Health,
                request.Mana,
                request.Magic,
                request.Strength,
                request.Defense,
                dtAttackType,
                request.DealsMagicDamage,
                request.Speed,
                request.Level,
                request.Experience,
                resistanceResult.Parsed,
                proficiencies
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

    public AddEntityResult CloneEntity(DamageableEntityDto dto)
    {
        AddEntityResult result = new();
        List<ParseIssue> errors = [];
        Dictionary<DamageType, double> resistances = [];
        Dictionary<Proficiency, double> proficiencies = [];

        foreach (var resistance in dto.Resistances)
        {
            if (!Enum.TryParse(resistance.Key, true, out DamageType damageType))
            {
                result.Errors.Add(new ParseIssue(
                    $"{resistance}",
                    $"{resistance.Key} is not a valid damage type."
                ));
            }
            resistances[damageType] = resistance.Value;
        }
        foreach (var proficiency in dto.Proficiencies)
        {
            if (!Enum.TryParse(proficiency.Key, true, out Proficiency entityProficiency))
            {
                result.Errors.Add(new ParseIssue(
                    $"{proficiency}",
                    $"{proficiency.Key} is not a valid proficiency."
                ));
            }
            proficiencies[entityProficiency] = proficiency.Value;
        }

        if (!Enum.TryParse(dto.AttackDamageType, out DamageType dtAttackType))
        {
            result.Errors.Add(new ParseIssue(
                $"{dto.AttackDamageType}",
                $"{dto.AttackDamageType} is not a valid damage type."
            ));
        }

        if (errors.Count == 0)
        {
            DamageableEntity entity = new(
                dto.Name,
                dto.EntityType,
                dto.Race,
                dto.MaxHealth,
                dto.MaxMana,
                (int)dto.Magic,
                (int)dto.Strength,
                (int)dto.Defense,
                dtAttackType,
                dto.DealsMagicDamage,
                (int)dto.Speed,
                dto.Level,
                dto.Experience,
                resistances,
                proficiencies
            );

            _entities.Add(entity);
            result.Entity = entity.ToDto();
        }

        return result;
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