using GameServer.Contracts.Parsing;

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
    public List<string> Messages { get; init; } = [];
    public List<InitiativeDto> InitiativeOrder { get; init; } = [];
    public string Error { get; init; } = string.Empty;
}