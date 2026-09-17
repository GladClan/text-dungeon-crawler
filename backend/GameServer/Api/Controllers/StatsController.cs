using GameServer.Contracts.DTOs;
using GameServer.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Api.Controllers;

[ApiController]
[Route("api/{id}/stats")]
public sealed class StatsController(EntityStatsService entityStatsService) : ControllerBase
{
    private readonly EntityStatsService _service = entityStatsService;

    private static string IdNotFound(string id)
    {
        return $"Entity not found: {id}";
    }

    [HttpGet]
    public ActionResult<EntityStatsDto> GetStats(string id)
    {
        var result = _service.GetStats(id);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpGet("get-proficiency-hierarchy")]
    public ActionResult<List<ProficiencyDto>> GetProficiencyHierarchy(string id, [FromBody] string proficiency)
    {
        var result = _service.GetProficiencyHierarchy(id, proficiency);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Any(r => r.Error.Length > 0))
        {
            return ValidationProblem(result.First(r => r.Error.Length > 0).Error);
        }
        return Ok(result);
    }

    [HttpGet("get-proficiency")]
    public ActionResult<ProficiencyDto> GetStoredProficiency(string id, [FromBody] string proficiency)
    {
        var result = _service.GetStoredProficiency(id, proficiency);
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

    [HttpGet("are-stats-displayed")]
    public ActionResult<bool> StatsAreHidden(string id)
    {
        var result = _service.StatsDisplayed(id);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("hide-stats")]
    public ActionResult<IntPairDto> HideStats(string id)
    {
        var statsAreHidden = _service.StatsDisplayed(id);
        if (statsAreHidden is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (statsAreHidden == true)
        {
            return BadRequest($"Stats for DamageableEntity {id} are already hidden");
        }
        var result = _service.ToggleDisplayStats(id);
        if (result is null)
        {
            return NotFound($"We lost entity {id} somewhere in transit. Sorry :[");
        }
        return Ok(result);
    }

    [HttpPatch("show-stats")]
    public ActionResult<IntPairDto> ShowStats(string id)
    {
        var statsAreHidden = _service.StatsDisplayed(id);
        if (statsAreHidden is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (statsAreHidden == false)
        {
            return BadRequest($"Stats for DamageableEntity {id} are already visible");
        }
        var result = _service.ToggleDisplayStats(id);
        if (result is null)
        {
            return NotFound($"We lost entity {id} somewhere in transit. Sorry :[");
        }
        return Ok(result);
    }
}