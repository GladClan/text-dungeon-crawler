namespace Gameserver.Contracts.DTOs;

public sealed class EntityStatsDto(
)
{
    public string Name { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string Race { get; init; } = string.Empty;
    public int MaxHealth { get; init; }
    public int CurrentHealth { get; init; }
    public int Magic { get; init; }
    public int MaxMana { get; init; }
    public int CurrentMana { get; init; }
    public int Strength { get; init; }
    public int Defense { get; init; }
    public int Level { get; init; }
    public int Experience { get; init; }
    public bool IsEntityAlive { get; init; }
    public bool DisplayStats { get; init; }
    public bool IsHidden { get; init; }
    public int Speed { get; init; }
}

public sealed class IntPairDto
{
    public IntPairDto() { }
    public IntPairDto(string error)
    {
        Error = error;
    }
    public IntPairDto(int old, int newVal = 0)
    {
        OldValue = old;
        NewValue = newVal;
        Error = string.Empty;
    }

    public int OldValue { get; set; }
    public int NewValue { get; set; }
    public string Error { get; set; } = string.Empty;
}