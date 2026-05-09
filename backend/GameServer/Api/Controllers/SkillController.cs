using Gameserver.Contracts.DTOs;
using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Api.Controllers;

[ApiController]
[Route("api/{id}/skills")]
public sealed class SkillController(SkillService skillService) : ControllerBase
{
    private readonly SkillService _service = skillService;

    private static string IdNotFound(string id)
    {
        return $"Entity not found: {id}";
    }

    [HttpGet("get-all-skills")]
    public ActionResult<List<SkillDto>> GetAllSkills(string id)
    {
        var result = _service.GetAllSkills(id);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpGet("skill-id/{skillId}")]
    public ActionResult<SkillDto> GetSkillById(string id, string skillId)
    {
        var result = _service.GetSkillById(id, skillId);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Error.Length > 0)
        {
            return NotFound(result.Error);
        }
        return Ok(result);
    }

    [HttpGet("skill-name/{skillName}")]
    public ActionResult<SkillDto> GetSkillByName(string id, string skillName)
    {
        var result = _service.GetSkillByName(id, skillName);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Error.Length > 0)
        {
            return NotFound(result.Error);
        }
        return Ok(result);
    }

    [HttpPost("add-skill-id/{skilId}")]
    public ActionResult<SkillDto> AddSkillById(string id, string skilId)
    {
        var result = _service.AddSkillById(id, skilId);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Error.Length > 0)
        {
            return NotFound(result.Error);
        }
        return Ok(result);
    }

    [HttpPost("add-skill-tag/{skillTag}")]
    public ActionResult<SkillDto> AddSkillByTag(string id, string skillTag)
    {
        var result = _service.AddSkillByTag(id, skillTag);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Error.Length > 0)
        {
            return NotFound(result.Error);
        }
        return Ok(result);
    }

    [HttpDelete("remove-id/{skillId}")]
    public ActionResult<SkillDto> RemoveById(string id, string skillId)
    {
        var result = _service.RemoveSkillById(id, skillId);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Error.Length > 0)
        {
            return NotFound(result.Error);
        }
        return Ok(result);
    }

    [HttpDelete("remove-name/{skillName}")]
    public ActionResult<SkillDto> RemoveByName(string id, string skillName)
    {
        var result = _service.RemoveSkillByName(id, skillName);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Error.Length > 0)
        {
            return NotFound(result.Error);
        }
        return Ok(result);
    }

    [HttpPut("learn-from/{sourceId}")]
    public ActionResult<List<SkillDto>> LearnAllSkillsFromSource(string id, string sourceId)
    {
        var result = _service.LearnAllSkillsFromSource(id, sourceId);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }

    [HttpPut("forget-all")]
    public ActionResult<List<SkillDto>> ForgetAllSkills(string id)
    {
        var result = _service.ForgetAllSkills(id);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(result);
    }
}