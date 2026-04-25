using GameServer.Application.Common;
using GameServer.Contracts;
using GameServer.Contracts.Mappers;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Application.Services;

public sealed class EntityService(EntityStore entityStore, ResistanceParser resistanceParser, ProficiencyParser proficiencyParser)
{
    private readonly EntityStore _entities = entityStore;
    private readonly ResistanceParser _resistanceParser = resistanceParser;
    private readonly ProficiencyParser _proficiencyParser = proficiencyParser;

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
        if (!_entities.TryGet(id, out var entity) || entity is null)
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
}