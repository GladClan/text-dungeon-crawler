using System.ComponentModel.DataAnnotations;
using Gameserver.DataAnnotations;
using GameServer.Domain.Enums;

namespace GameServer.Contracts.Requests;

public sealed class ProficiencyRequest
{
    [Required]
    [EnumDataType(typeof(Proficiency))]
    [MinLength(1)]
    public string Type { get; init; } = string.Empty;

    [Required]
    [Range(typeof(double), "-999999", "999999", ConvertValueInInvariantCulture = true)]
    public double Value { get; init; }
}

public sealed class ResistanceRequest
{
    [Required]
    [EnumDataType(typeof(DamageType))]
    [MinLength(1)]
    public string Type { get; init; } = string.Empty;

    [Required]
    [Range(typeof(double), "-999999", "999999", ConvertValueInInvariantCulture = true)]
    public double Value { get; init; }
}

public sealed class DamageableEntityRequest
{
    [Required]
    [MinLength(1)]
    public string Name { get; init; } = string.Empty;
    
    [Required]
    [MinLength(1)]
    public string EntityType { get; init; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string Race { get; init; } = string.Empty;

    [Required]
    [Range(typeof(int), "0", "999999", ConvertValueInInvariantCulture = true)]
    public int Health { get; init; }

    [Required]
    [Range(typeof(int), "0", "999999", ConvertValueInInvariantCulture = true)]
    public int Magic { get; init; }

    [Required]
    [Range(typeof(int), "0", "999999", ConvertValueInInvariantCulture = true)]
    public int Mana { get; init; }

    [Required]
    [Range(typeof(int), "0", "999", ConvertValueInInvariantCulture = true)]
    public int Strength { get; init; }

    [Required]
    [Range(typeof(int), "0", "999", ConvertValueInInvariantCulture = true)]
    public int Defense { get; init; }

    [MinimumValue(0)]
    public int Speed { get; init; }

    [MinimumValue(0)]
    public int Level { get; init; }

    [MinimumValue(0)]
    public int Experience { get; init; }

    public List<ResistanceRequest>? Resistances { get; init; }

    public List<ProficiencyRequest>? Proficiencies { get; init; }
}

public sealed class SetSpeedRequest
{
    [Range(0, 100)]
    public double Speed { get; init; }
}

public sealed class ChangeHealthRequest
{
    /// <summary>
    /// Source entity id is optional so this can support environmental damage/healing.
    /// </summary>
    public Guid? SourceEntityId { get; init; }

    [Range(typeof(double), "-999999", "999999", ConvertValueInInvariantCulture = true)]
    public double Amount { get; init; }
}

public sealed class ChangeManaRequest
{
    [Range(typeof(double), "-999999", "999999", ConvertValueInInvariantCulture = true)]
    public double Amount { get; init; }
}