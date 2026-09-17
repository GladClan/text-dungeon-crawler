using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Api.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EntitiesController(EventServices services) : ControllerBase
{
    // ChooseOption
    // GetCurrentEvent
    [HttpPatch("choose-option")]
    public ActionResult<EventResultDto> ChooseOption(int optionId)
    {
        var result = services.ChooseOption(optionId);
        if (result.RequiresTarget)
        {
            return StatusCode(StatusCodes.Status300MultipleChoices, result.Targets);
        }
        if (result.Error.Length > 0 || !result.Success)
        {
            return BadRequest(result.Error);
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
}
