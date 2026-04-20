using System.ComponentModel.DataAnnotations;
using GameServer.Domain.Entities;
using GameServer.Domain.Items;
using GameServer.Domain.Skills;

namespace GameServer.Contracts;

/*
Quick start
1) Use DamageableEntityDto (and nested DTOs) as your API response shape.
2) Use request DTOs (SetSpeedRequest, ChangeHealthRequest, etc.) as [FromBody] inputs.
3) In controllers/services, map domain entities with: entity.ToDto().
4) Keep game logic in domain classes; keep API contracts and validation in this Contracts layer.

Why this exists
- DTOs prevent frontend code from depending on domain internals.
- Request DTOs centralize validation rules for API commands.
- Mapper methods keep controllers thin and consistent.
*/

/// <summary>
/// A transport-safe item shape for API responses.
/// Optional fields allow one DTO to represent useable and equippable item variants.
/// </summary>


// Request DTOs are command payloads sent by the frontend to mutate game state.

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

public sealed class RemoveItemByNameRequest
{
    [Required]
    [MinLength(1)]
    public string ItemName { get; init; } = string.Empty;
}

public sealed class RemoveItemByIndexRequest
{
    [Range(0, int.MaxValue)]
    public int Index { get; init; }
}

public sealed class AddItemByIdRequest
{
    /// <summary>
    /// Use a catalog id from your item registry; resolve it to a domain Item in your service layer.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string ItemId { get; init; } = string.Empty;
}

public sealed class LearnSkillByIdRequest
{
    /// <summary>
    /// Use a catalog id from your skill registry; resolve it to a domain Skill in your service layer.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string SkillId { get; init; } = string.Empty;
}

/// <summary>
/// Mapper methods convert domain models to DTOs.
/// Keep this translation in one place so controllers stay thin and frontend contracts stay consistent.
/// </summary>
public static class EntityContractMapper
{
    /// <summary>
    /// Main mapper used by query endpoints and mutation responses.
    /// Example controller usage: return Ok(entity.ToDto());
    /// </summary>
    public static DamageableEntityDto ToDto(this DamageableEntity entity)
    {
        return new DamageableEntityDto
        {
            Id = entity.ID,
            Name = entity.Name,
            EntityType = entity.EntityType,
            MaxHealth = entity.MaxHealth,
            CurrentHealth = entity.CurrentHealth,
            MaxMana = entity.MaxMana,
            CurrentMana = entity.CurrentMana,
            Magic = entity.Magic,
            Strength = entity.Strength,
            Defense = entity.Defense,
            Level = entity.Level,
            Experience = entity.Experience,
            IsEntityAlive = entity.IsEntityAlive,
            Visible = entity.Visible,
            IsHidden = entity.IsHidden,
            Speed = entity.Speed,
            Resistances = ToStringKeyDictionary(entity.Resistances),
            Proficiencies = ToStringKeyDictionary(entity.Proficiencies),
            Inventory = new EntityInventoryDto
            {
                Gold = entity.Inventory.Gold,
                Items = entity.Inventory.Items.Select(MapItem).ToList()
            },
            Skills = new EntitySkillsDto
            {
                Skills = entity.Skills.Skills.Select(MapSkill).ToList()
            }
        };
    }

    /// <summary>
    /// Utility mapper for list endpoints.
    /// </summary>
    public static List<DamageableEntityDto> ToDtos(this IEnumerable<DamageableEntity> entities)
    {
        return [.. entities.Select(ToDto)];
    }

    private static Dictionary<string, double> ToStringKeyDictionary<TEnum>(Dictionary<TEnum, double> source)
        where TEnum : struct, Enum
    {
        return source.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => kvp.Value,
            StringComparer.OrdinalIgnoreCase);
    }

    private static ItemDto MapItem(Item item)
    {
        var useable = item as Useable;
        var equippable = item as Equippable;

        return new ItemDto
        {
            Id = item.Id,
            Type = item.Type,
            Name = item.Name,
            Value = item.Value,
            Description = item.Description,
            Consumable = item.Consumable,
            Element = useable?.Element.ToString(),
            Proficiency = useable?.Prof.ToString(),
            ArmorTypeLimit = equippable?.ArmorTypeLimit,
            Equipped = equippable?.Equipped
        };
    }

    private static SkillDto MapSkill(Skill skill)
    {
        return new SkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            Cost = skill.Cost,
            Element = skill.Element.ToString(),
            Proficiency = skill.Prof.ToString(),
            Level = skill.Level
        };
    }
}
