using GameServer.Contracts.Parsing;
using GameServer.Domain.Map.Scene;

namespace GameServer.Contracts.DTOs;

public sealed class BattleDto
{
    public List<string> Messages { get; set; } = [];
    public List<InitiativeDto> InitiativeOrder { get; set; } = [];
    public List<DamageableEntityDto> EntityDtos { get; set;} = [];
    public List<AddEntityResult>? EntityResult { get; set; }
    public string Error { get; set; } = string.Empty;
}

public sealed class NextTurnDto
{
    public List<string> Messages { get; set; } = [];
    public string Error { get; set; } = string.Empty;
}

public sealed class InitiativeDto
{
    public int Initiative { get; init; }
    public string EntityName { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
}

public sealed class TurnoverDto
{
    public int CurrentTurn { get; init; }
    public List<string> Messages { get; init; } = [];
    public List<InitiativeDto> InitiativeOrder { get; init; } = [];
    public string Error { get; init; } = string.Empty;
}

public sealed class BattleEndDto
{
    public bool Victory { get; set; }
    public List<DamageableEntityDto> Party { get; init; } = [];
    public List<DamageableEntityDto> Opponents { get; init; } = [];
    public List<LevelUpDto> LevelUps { get; init; } = [];
    public string Error { get; init; } = string.Empty;
}

public class EventResultDto
{
    public bool Success { get; set; }
    public bool RequiresTarget { get; set; }
    public string Message { get; set; }
    public List<TargetOption> Targets { get; set; }
    public string Error { get; set; }
    public EventResultDto(string error)
    {
        Success = false;
        Message = string.Empty;
        Targets = [];
        Error = error;
    }
    public EventResultDto(bool success)
    {
        Success = success;
        Message = string.Empty;
        Targets = [];
        Error = string.Empty;
    }
    public EventResultDto(
        bool success,
        bool requiresTarget,
        string prompt,
        List<TargetOption> targets
    )
    {
        Success = success;
        RequiresTarget = requiresTarget;
        Message = prompt;
        Targets = targets;
        Error = string.Empty;
    }
}

public class SceneEventDto
{
    public int ID { get; init; }
    public List<DialogueDto> Dialogues { get; init; }
    public List<OptionDto> Options { get; init; }
    public bool ExistsActivaBattle { get; init; }
    public string Error { get; init; }

    public SceneEventDto(
        int id,
        List<DialogueDto> dialogues,
        List<OptionDto> options
    )
    {
        ID = id;
        Dialogues = dialogues;
        Options = options;
        Error = string.Empty;
    }
    public SceneEventDto(
        bool isBattleActive
    )
    {
        Dialogues = [];
        Options = [];
        ExistsActivaBattle = isBattleActive;
        Error = string.Empty;
    }
    public SceneEventDto(
        string error
    )
    {
        Error = error;
        Dialogues = [];
        Options = [];
    }
}

public class DialogueDto(
    int orderId,
    string source,
    string message
)
{
    public int OrderId { get; init; } = orderId;
    public string Source { get; init; } = source;
    public string Message { get; init; } = message;
}

public class OptionDto(
    int optionId,
    string optionTitle
)
{
    public int OptionId { get; init; } = optionId;
    public string OptionTitle { get; init; } = optionTitle;
}

