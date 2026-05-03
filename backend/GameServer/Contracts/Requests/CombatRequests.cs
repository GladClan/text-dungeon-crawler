using System.ComponentModel.DataAnnotations;
using Gameserver.DataAnnotations;
using GameServer.Domain.Enums;

namespace Gameserver.Contracts.Requests;

public sealed class DamageRequest
{
    [Required]
    [MinLength(1)]
    public string TargetId { get; init; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string SourceId { get; init; } = string.Empty;

    [Required]
    [Range(typeof(double), "0", "999999", ConvertValueInInvariantCulture = true)]
    public double DamageSent { get; init; }

    [Required]
    [EnumDataType(typeof(Proficiency))]
    [MinLength(1)]
    public string DamageType { get; init; } = string.Empty;

    public bool PhysicalAttack { get; init; }
}

public sealed class HealRequest
{
    [Required]
    [MinLength(1)]
    public string TargetId { get; init; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string SourceId { get; init; } = string.Empty;

    [Required]
    [MinimumValue(0)]
    public double AmountToHeal { get; init; }
}

public sealed class ManaRequest
{
    [Required]
    [MinLength(1)]
    public string TargetId { get; init; } = string.Empty;

    [Required]
    public double Amount { get; init; }
}

public sealed class AddExperienceRequest
{
    [Required]
    [MinLength(1)]
    public string TargetId { get; init; } = string.Empty;

    [Required]
    [MinimumValue(0)]
    public int Amount { get; set; }
}

public sealed class AddProficiencyEntryRequest
{
    
    [Required]
    [MinLength(1)]
    public string TargetId { get; init; } = string.Empty;
    
    [Required]
    [EnumDataType(typeof(Proficiency))]
    [MinLength(1)]
    public string Proficiency { get; init; } = string.Empty;
}