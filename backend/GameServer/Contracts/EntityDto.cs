namespace GameServer.Contracts;

public sealed class EntityDto
{
    public Guid Id { get; init; }
    public string SimpleId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;

    public int MaxHealth { get; init; }
    public double Health { get; init; }
    public int MaxMana { get; init; }
    public double Mana { get; init; }
    public double Magic { get; init; }
    public double Strength { get; init; }
    public double Defense { get; init; }
    public int Level { get; init; }
    public int Experience { get; init; }

    public bool IsEntityAlive { get; init; }
    public bool Visible { get; init; }
    public bool IsHidden { get; init; }
    public double Speed { get; init; }

    public Dictionary<string, double> Resistances { get; init; } = new();
    public Dictionary<string, double> Proficiencies { get; init; } = new();

    public EntityInventoryDto Inventory { get; init; } = new();
    public EntitySkillsDto Skills { get; init; } = new();
}