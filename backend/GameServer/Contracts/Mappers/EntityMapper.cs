using GameServer.Contracts.DTOs;
using GameServer.Domain.Entities;
using GameServer.Domain.Items;
using GameServer.Domain.Enums;
using GameServer.Domain.Skills;

namespace GameServer.Contracts.Mappers;

/// <summary>
/// Mapper methods convert domain models to DTOs.
/// Keep this translation in one place so controllers stay thin and frontend contracts stay consistent.
/// </summary>
public static class EntityMapper
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
            Resistances = entity.Resistances.ToStringKeyDictionary(),
            Proficiencies = entity.Proficiencies.ToStringKeyDictionary(),
            Inventory = entity.Inventory.ToDto(),
            Skills = new EntitySkillsDto
            {
                Skills = [.. entity.Skills.Skills.Select(MapSkill)]
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

    /// <summary>
    /// Converts resistance dictionaries into the public DTO shape used by the API.
    /// </summary>
    public static List<ResistanceDto> ToResistanceDtos(this IDictionary<DamageType, double>? source)
    {
        return source is null
            ? []
            : [.. source.Select(kvp => new ResistanceDto(kvp.Key.ToString(), kvp.Value))];
    }

    public static Dictionary<string, double> ToStringKeyDictionary<TEnum>(this Dictionary<TEnum, double> source)
        where TEnum : struct, Enum
    {
        return source.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => kvp.Value,
            StringComparer.OrdinalIgnoreCase);
    }

    private static SkillDto MapSkill(Skill skill)
    {
        return new SkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            Description = skill.Description,
            Cost = skill.Cost,
            Element = skill.Element.ToString(),
            Proficiency = skill.Prof.ToString(),
            Level = skill.Level
        };
    }
}