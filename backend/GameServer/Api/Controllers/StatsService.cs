using Gameserver.Contracts.DTOs;
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

    [HttpPatch("set-experience")]
    public ActionResult<IntPairDto> SetExperience(string id, [FromBody] int newValue)
    {
        var result = _service.SetEntityExperience(id, newValue);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("set-level")]
    public ActionResult<IntPairDto> SetLevel(string id, [FromBody] int newValue)
    {
        var result = _service.SetEntityLevel(id, newValue);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("set-max-health")]
    public ActionResult<IntPairDto> SetMaxHealth(string id, [FromBody] int newValue)
    {
        var result = _service.SetMaxHealth(id, newValue);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("set-current-health")]
    public ActionResult<IntPairDto> SetCurrentHealth(string id, [FromBody] int newValue)
    {
        var result = _service.SetCurrentHealth(id, newValue);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("set-max-mana")]
    public ActionResult<IntPairDto> SetMaxMana(string id, [FromBody] int newValue)
    {
        var result = _service.SetMaxMana(id, newValue);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("set-current-mana")]
    public ActionResult<IntPairDto> SetCurrentMana(string id, [FromBody] int newValue)
    {
        var result = _service.SetCurrentMana(id, newValue);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("set-magic")]
    public ActionResult<IntPairDto> SetMagic(string id, [FromBody] int newValue)
    {
        var result = _service.SetMagic(id, newValue);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("set-strength")]
    public ActionResult<IntPairDto> SetStrength(string id, [FromBody] int newValue)
    {
        var result = _service.SetStrength(id, newValue);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("set-defense")]
    public ActionResult<IntPairDto> SetDefense(string id, [FromBody] int newValue)
    {
        var result = _service.SetDefense(id, newValue);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("set-speed")]
    public ActionResult<IntPairDto> SetSpeed(string id, [FromBody] int newValue)
    {
        var result = _service.SetSpeed(id, newValue);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpGet("stats-are-displayed")]
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