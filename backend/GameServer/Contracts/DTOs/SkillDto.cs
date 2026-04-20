namespace GameServer.Contracts;

public sealed class SkillDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Cost { get; init; }
    public string Element { get; init; } = string.Empty;
    public string Proficiency { get; init; } = string.Empty;
    public int Level { get; init; }
}