using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Api.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventController(EventServices services) : ControllerBase
{
    [HttpPatch("choose-option/{optionId}")]
    public ActionResult<EventResultDto> ChooseOption(int optionId)
    {
        var result = services.ChooseOption(optionId);
        if (result.RequiresTarget)
        {
            return StatusCode(StatusCodes.Status300MultipleChoices, result);
        }
        if (result.Error.Length > 0 || !result.Success)
        {
            return BadRequest($"Successful: {result.Success}\n" + result.Error);
        }
        return Ok(result);
    }

    [HttpGet("get-current")]
    public ActionResult<SceneEventDto> GetCurrentEvent()
    {
        var result = services.GetCurrentEvent();
        if (result.Error.Length > 0)
        {
            return BadRequest(result.Error);
        }
        else
        {
            return Ok(result);
        }
    }

    [HttpGet("battle")]
    public ActionResult<BattleDto> GetCurrentBattle()
    {
        var result = services.GetCurrentBattle();
        if (result is null)
        {
            Console.WriteLine($"{nameof(GetCurrentBattle)} returned a null response; no battle is currently active");
            return NotFound("No battle currently active.");
        }
        else
        {
            return Ok(result);
        }
    }

    [HttpPost("select-target")]
    public ActionResult SelectTarget([FromBody] string targetId)
    {
        var result = services.SetPendingResponse(targetId);
        return Ok(result);
    }

    [HttpPatch("use-item")]
    public ActionResult UseItem([FromBody] ActionRequest request)
    {
        Console.WriteLine($"API request: using item {request.ActionId} from {request.SourceId}");
        var result = services.DoAction(
            Domain.Enums.ItemSkillDefault.Item,
            sourceId: request.SourceId,
            actionId: request.ActionId,
            targets: request.TargetIds
        );

        if (result.Error.Length != 0)
        {
            Console.WriteLine(result.Error);
            return NotFound(result.Error);
        }

        return Ok(result);
    }

    [HttpPatch("use-skill")]
    public ActionResult<TurnOverDto> UseSkill([FromBody] ActionRequest request)
    {
        var result = services.DoAction(
            Domain.Enums.ItemSkillDefault.Item,
            sourceId: request.SourceId,
            actionId: request.ActionId,
            targets: request.TargetIds
        );

        if (result.Error.Length != 0)
        {
            return NotFound(result.Error);
        }

        return Ok(result);
    }

    [HttpPatch("default-attack")]
    public ActionResult<TurnOverDto> DefaultAttack([FromBody] ActionRequest request)
    {
        var result = services.DoAction(
            Domain.Enums.ItemSkillDefault.Default,
            sourceId: request.SourceId,
            actionId: "",
            targets: request.TargetIds
        );

        if (result.Error.Length != 0)
        {
            return NotFound(result.Error);
        }

        return Ok(result);
    }

    [HttpPatch("defend")]
    public ActionResult<TurnOverDto> Defend([FromBody] ActionRequest request)
    {
        var result = services.DoAction(
            Domain.Enums.ItemSkillDefault.Default,
            sourceId: request.SourceId,
            actionId: "",
            targets: []
        );

        if (result.Error.Length != 0)
        {
            return NotFound(result.Error);
        }

        return Ok(result);
    }
}
