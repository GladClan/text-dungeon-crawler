namespace GameServer.Contracts.DTOs;

public sealed class SkillDto
{
    public SkillDto(
        string id,
        string name,
        string description,
        bool canUse,
        string tag,
        int cost,
        string element,
        string proficiency,
        int targetsLimit,
        int level
    )
    {
        Id = id;
        Name = name;
        Description = description;
        CanUse = canUse;
        Tag = tag;
        Cost = cost;
        Element = element;
        Proficiency = proficiency;
        MultiTarget = targetsLimit > 1;
        TargetsLimit = targetsLimit;
        Level = level;
    }

    public SkillDto(
        string error
    )
    {
        Error = error;

        Id = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        Tag = string.Empty;
        Element = string.Empty;
        Proficiency = string.Empty;
    }

    public string Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public bool? CanUse { get; init; }
    public string Tag { get; init; }
    public int Cost { get; init; }
    public string Element { get; init; }
    public string Proficiency { get; init; }
    public bool MultiTarget { get; init; }
    public int TargetsLimit { get; init; }
    public int Level { get; init; }
    public string Error { get; init; } = string.Empty;
}