using GameServer.Application.Common;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace GameServer.Application.Services;

public sealed class EntityService(EntityStore entityStore, ResistanceParser resistanceParser, ProficiencyParser proficiencyParser)
{
    private readonly EntityStore _entities = entityStore;
    private readonly ResistanceParser _resistanceParser = resistanceParser;
    private readonly ProficiencyParser _proficiencyParser = proficiencyParser;

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

        var resistanceResult = _resistanceParser.Parse(request.Resistances);
        result.AddErrors(resistanceResult.Errors);

        var proficiencyResult = _proficiencyParser.Parse(request.Proficiencies);
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
                request.Speed,
                request.Level,
                request.Experience,
                resistanceResult.Parsed,
                proficiencies
            );

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

    public ResistanceDto? GetResistance(string id, string damageType)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        if (!TryParseDamageType(damageType, out var result))
        {
            return new(string.Empty, 0);
        }
        double value = target.GetResistance(result);
        return new(damageType.ToString(), value);
    }

    public List<ResistanceDto>? GetAllResistances(string id)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        return target.Resistances.ToResistanceDtos();
    }

    public EnumDictionaryParseResult<DamageType>? SetAllResistances(string id, List<ResistanceRequest> resistances)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        var resistanceResult = _resistanceParser.Parse(resistances);
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

    public ResistanceDto? SetResistance(string id, ResistanceRequest request){
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

    public ResistanceDto? AddResistance(string id, ResistanceRequest request)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        if (!TryParseDamageType(request.Type, out var damageType))
        {
            return new(string.Empty, 0);
        }
        target.Resistances[damageType] += request.Value;
        return new(damageType.ToString(), target.Resistances[damageType]);
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
        if (target.IsHidden)
        {
            target.IsHidden = false;
        }
        else
        {
            target.IsHidden = true;
        }
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

    public double? SetSpeed(string id, double speed)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        target.Speed = speed;
        return target.Speed;
    }
}

// hasAI
// getAI
// setAI
// shadow