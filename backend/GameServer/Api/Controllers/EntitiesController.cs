using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Api.Controllers;

[ApiController]
[Route("api/entities")]
public sealed class EntitiesController(EntityService entityService) : ControllerBase
{
    private readonly EntityService _service = entityService;

    private static string IdNotFound(string id)
    {
        return $"Entity not found: {id}";
    }

    [HttpGet("names-list")]
    public ActionResult<string[]> GetAllNames()
    {
        var names = _service.GetAllNames();
        return Ok(names);
    }

    [HttpGet("ids-list")]
    public ActionResult<string[]> GetAllIds()
    {
        var ids = _service.GetAllIds();
        return Ok(ids);
    }

    [HttpGet("{id}")]
    public ActionResult<DamageableEntityDto> GetById(string id)
    {
        var result = _service.GetById(id);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        } else
        {
            return Ok(result);
        }
    }

    [HttpGet("{id}/get-resistance")]
    public ActionResult<ResistanceDto> GetResistance(string id, [FromBody] string damageType)
    {
        var result = _service.GetResistance(id, damageType);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Resistance.Length == 0 && result.Value == 0)
        {
            return ValidationProblem($"{damageType} is not a valid Damage Type");
        }
        return Ok(result);
    }

    [HttpGet("{id}/get-all-resistances")]
    public ActionResult<List<ResistanceDto>> GetAllResistances(string id)
    {
        var result = _service.GetAllResistances(id);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpGet("{id}/is-hidden")]
    public ActionResult<bool> GetIsHidden(string id)
    {
        var result = _service.IsHidden(id);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpGet("{id}/get-speed")]
    public ActionResult<double> GetSpeed(string id)
    {
        var result = _service.GetSpeed(id);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPut("new")]
    public ActionResult<DamageableEntityDto> AddEntity([FromBody] DamageableEntityRequest request)
    {
        var result = _service.AddEntity(request);
        AddParseErrors(result.Errors);

        if (!ModelState.IsValid || result.Entity is null)
        {
            return ValidationProblem(ModelState);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Entity.Id }, result.Entity);
    }

    [HttpPut("clone/{id}")]
    public ActionResult<DamageableEntityDto> CloneEntity(string id)
    {
        var target = _service.GetById(id);
        if (target is null)
        {
            return NotFound(IdNotFound(id));
        }
        var result = _service.CloneEntity(target);
        AddParseErrors(result.Errors);

        if (!ModelState.IsValid || result.Entity is null)
        {
            return ValidationProblem(ModelState);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Entity.Id }, result.Entity);
    }

    [HttpPatch("{id}/set-stats")]
    public ActionResult<DamageableEntityDto> FixStats(string id, [FromBody] FixStatsRequest fixStatsRequest)
    {
        var result = _service.FixStats(id, fixStatsRequest);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPatch("{id}/set-all-resistances")]
    public ActionResult<List<ResistanceDto>> SetAllResistances(string id, [FromBody] List<ResistanceRequest> requests)
    {
        var result = _service.SetAllResistances(id, requests);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        
        AddParseErrors(result.Errors);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        return result.Parsed.ToResistanceDtos();
    }

    [HttpPatch("{id}/set-resistance")]
    public ActionResult<ResistanceDto> SetResistance(string id, [FromBody] ResistanceRequest request)
    {
        var result = _service.SetResistance(id, request);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Resistance.Length == 0 && result.Value == 0)
        {
            return ValidationProblem($"{request.Type} is not a valid Damage Type");
        }
        return Ok(result);
    }

    [HttpPatch("{id}/change-resistance")]
    public ActionResult<ResistanceDto> ChangeResistance(string id, [FromBody] ResistanceRequest request)
    {
        var result = _service.AddResistance(id, request);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Resistance.Length == 0 && result.Value == 0)
        {
            return ValidationProblem($"{request.Type} is not a valid Damage Type");
        }
        return Ok(result);
    }

    [HttpPatch("{id}/hide")]
    public ActionResult<bool> Hide(string id)
    {
        var isHidden = _service.IsHidden(id);
        if (isHidden is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (isHidden == false)
        {
            var result = _service.ToggleIsHidden(id);
            if (result is null)
            {
                return NotFound($"We lost entity {id} somewhere in transit. Sorry :[");
            }
            return Ok(result);
        }
        else
        {
            return BadRequest($"DamageableEntity is already hidden: {id}");
        }
    }

    [HttpPatch("{id}/reveal")]
    public ActionResult<bool> Reveal(string id)
    {
        var isHidden = _service.IsHidden(id);
        if (isHidden is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (isHidden == true)
        {
            var result = _service.ToggleIsHidden(id);
            if (result is null)
            {
                return NotFound($"We lost entity {id} somewhere in transit. Sorry :[");
            }
            return Ok(result);
        }
        else
        {
            return BadRequest($"DamageableEntity is not hidden: {id}");
        }
    }

    [HttpPatch("{id}/set-speed")]
    public ActionResult<double> SetSpeed(string id, [FromBody] double speed)
    {
        var result = _service.SetSpeed(id, speed);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    private void AddParseErrors(IEnumerable<ParseIssue> errors)
    {
        foreach (ParseIssue err in errors)
        {
            ModelState.AddModelError(err.Field, err.Message);
        }
    }
}