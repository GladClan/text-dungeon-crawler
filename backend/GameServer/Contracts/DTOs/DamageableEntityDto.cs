namespace GameServer.Contracts;

public sealed class DamageableEntityDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string Race { get; init; } = string.Empty;
    public int MaxHealth { get; init; }
    public double CurrentHealth { get; init; }
    public double Magic { get; init; }
    public int MaxMana { get; init; }
    public double CurrentMana { get; init; }
    public double Strength { get; init; }
    public double Defense { get; init; }
    public int Level { get; init; }
    public int Experience { get; init; }
    public bool IsEntityAlive { get; init; }
    public bool Visible { get; init; }
    public bool IsHidden { get; init; }
    public double Speed { get; init; }

    public Dictionary<string, double> Resistances { get; init; } = [];
    public Dictionary<string, double> Proficiencies { get; init; } = [];

    public EntityInventoryDto Inventory { get; init; } = new();
    public EntitySkillsDto Skills { get; init; } = new();
}