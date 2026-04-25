using GameServer.Application.Services;
using GameServer.Contracts;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Api.Controllers;

[ApiController]
[Route("api/entities")]
public sealed class EntitiesController(EntityService entityService) : ControllerBase
{
    private readonly EntityService _service = entityService;

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
            return NotFound();
        } else
        {
            return Ok(result);
        }
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

    private void AddParseErrors(IEnumerable<ParseIssue> errors)
    {
        foreach (ParseIssue err in errors)
        {
            ModelState.AddModelError(err.Field, err.Message);
        }
    }
}