namespace GameServer.Contracts.DTOs;

public sealed class SkillDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Tag { get; init; } = string.Empty;
    public int Cost { get; init; }
    public string Element { get; init; } = string.Empty;
    public string Proficiency { get; init; } = string.Empty;
    public bool MultiTarget { get; init; }
    public int TargetsLimit { get; init; }
    public int Level { get; init; }
    public string Error { get; init; } = string.Empty;
}