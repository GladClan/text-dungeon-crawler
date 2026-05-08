using Gameserver.Application.Services;
using Gameserver.Contracts.Requests;
using GameServer.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Api.Controllers;

[ApiController]
[Route("api/combat")]
public sealed class CombatController(CombatService combatService) : ControllerBase
{
    private readonly CombatService _service = combatService;

    private static string IdNotFound(string id)
    {
        return $"Entity not found: {id}";
    }

    [HttpGet("{id}/get-proficiency")]
    public ActionResult<ProficiencyDto> GetProficiency(string id, [FromBody] string proficiency)
    {
        var result = _service.GetProficiencyMultiplier(id, proficiency);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Error.Length > 0)
        {
            return ValidationProblem(result.Error);
        }
        return Ok(result);
    }

    [HttpGet("{id}/get-resistance")]
    public ActionResult<ProficiencyDto> GetResistance(string id, [FromBody] string resistance)
    {
        var result = _service.GetResistanceMultiplier(id, resistance);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Error.Length > 0)
        {
            return ValidationProblem(result.Error);
        }
        return Ok(result);
    }

    [HttpPatch("damage")]
    public ActionResult<DamageResultDto> TakeDamage([FromBody] DamageRequest request)
    {
        var result = _service.TakeDamage(request);
        if (result is null)
        {
            return NotFound(IdNotFound(request.SourceId + " or " + request.TargetId));
        }
        if (result.Error.Length > 0)
        {
            return ValidationProblem(result.Error);
        }
        return Ok(result);
    }

    [HttpPatch("heal")]
    public ActionResult<HealResultDto> Heal([FromBody] HealRequest request)
    {
        var result = _service.Heal(request);
        if (result is null)
        {
            return NotFound(IdNotFound(request.SourceId + " or " + request.TargetId));
        }
        if (result.Error.Length > 0)
        {
            return ValidationProblem(result.Error);
        }
        return Ok(result);
    }

    [HttpPatch("change-mana")]
    public ActionResult<HealResultDto> ChangeMana([FromBody] ManaRequest request)
    {
        var result = _service.ChangeMana(request);
        if (result is null)
        {
            return NotFound(IdNotFound(request.TargetId));
        }
        return Ok(result);
    }

    [HttpPatch("add-experience")]
    public ActionResult<LevelUpDto> AddExperience([FromBody] AddExperienceRequest request)
    {
        var result = _service.AddExperience(request);
        if (result is null)
        {
            return NotFound(IdNotFound(request.TargetId));
        }
        return Ok(result);
    }

    [HttpGet("{id}/experience-for-next-level")]
    public ActionResult<int> GetExperienceForNextLevel(string id)
    {
        var result = _service.GetExperienceForNextLevel(id);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("add-proficiency-entry")]
    public ActionResult<StringDoubleDto> AddProficiencyEntry([FromBody] AddProficiencyEntryRequest request)
    {
        var result = _service.AddProficiencyEntry(request);
        if (result is null)
        {
            return NotFound(IdNotFound(request.TargetId));
        }
        if (result.Error.Length > 0)
        {
            return ValidationProblem(result.Error);
        }
        return Ok(result);
    }

    [HttpGet("{id}/death-message")]
    public ActionResult<string> GetDeathMessage(string id)
    {
        var result = _service.GetDeathMessage(id);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }
}