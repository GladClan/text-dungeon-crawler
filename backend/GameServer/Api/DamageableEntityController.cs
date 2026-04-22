using GameServer.Contracts;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Api;

[ApiController]
[Route("api/entities")]
public sealed class DamageableEntityController(EntityStore store) : ControllerBase
{
    private readonly EntityStore _entities = store;

    [HttpGet("names-list")]
    public ActionResult<string[]> GetAllNames()
    {
        var names = _entities.GetAllNames();
        return Ok(names);
    }

    [HttpGet("ids-list")]
    public ActionResult<string[]> GetAllIds()
    {
        var ids = _entities.GetAllIds();
        return Ok(ids);
    }

    [HttpGet("{id}")]
    public ActionResult<DamageableEntityDto> GetById(string id)
    {
        if (!_entities.TryGet(id, out var entity) || entity is null) return NotFound();
        return Ok(entity.ToDto());
    }

    [HttpPut("new")]
    public ActionResult<DamageableEntityDto> AddEntity([FromBody] DamageableEntityRequest request)
    {
        var resistances = ParseResistances(request.Resistances);
        var proficiencies = request.Proficiencies is null
            ? ParseProficiencies(request.Proficiencies)
            : new Dictionary<Proficiency, double>
                {
                    {Proficiency.bludgeoning, 0.85d},
                    {Proficiency.potions, 0.85d},
                    {Proficiency.slashing, 0.65d},
                    {Proficiency.healing, 0.6d}
                };

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

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
            resistances,
            proficiencies
        );

        _entities.Add(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.ID }, entity.ToDto());
    }

    private Dictionary<DamageType, double>? ParseResistances(List<ResistanceRequest>? entries)
    {
        if (entries is null)
        {
            return null;
        }

        Dictionary<DamageType, double> parsed = [];

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];

            if (!Enum.TryParse<DamageType>(entry.Type, true, out var damageType))
            {
                ModelState.AddModelError(
                    $"Resistances[{i}].Type",
                    $"'{entry.Type}' is not a valid {nameof(DamageType)} value.");
                continue;
            }

            parsed[damageType] = entry.Value;
        }

        return parsed;
    }

    private Dictionary<Proficiency, double>? ParseProficiencies(List<ProficiencyRequest>? entries)
    {
        if (entries is null)
        {
            return null;
        }

        Dictionary<Proficiency, double> parsed = [];

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];

            if (!Enum.TryParse<Proficiency>(entry.Type, true, out var proficiency))
            {
                ModelState.AddModelError(
                    $"Proficiencies[{i}].Type",
                    $"'{entry.Type}' is not a valid {nameof(Proficiency)} value.");
                continue;
            }

            parsed[proficiency] = entry.Value;
        }

        return parsed;
    }
}