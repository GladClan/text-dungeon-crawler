using System.Diagnostics.CodeAnalysis;
using Gameserver.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Domain.Entities;

namespace GameServer.Application.Services;

public sealed class EntityStatsService(EntityStore entityStore)
{
    private readonly EntityStore _entities = entityStore;

    private bool TryGetEntity(string id, [NotNullWhen(true)] out DamageableEntity? target)
    {
            return _entities.TryGet(id, out target) && target is not null;
    }

    public EntityStatsDto? GetStats(string id)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        return target.StatsToDto();
    }

    public IntPairDto? SetEntityExperience(string id, int newValue)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        int old = target.Experience;
        target.Experience = newValue;
        return new IntPairDto(old, target.Experience);
    }
    public IntPairDto? SetEntityLevel(string id, int newValue)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        int old = target.Level;
        target.Level = newValue;
        return new(old, target.Level);
    }
    public IntPairDto? SetMaxHealth(string id, int newValue)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        int old = target.MaxHealth;
        target.MaxHealth = newValue;
        return new(old, target.Level);
    }
    public IntPairDto? SetCurrentHealth(string id, int newValue)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        double old = target.CurrentHealth;
        target.CurrentHealth = newValue;
        return new((int) old, target.Level);
    }
    public IntPairDto? SetMaxMana(string id, int newValue)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        int old = target.MaxMana;
        target.MaxMana = newValue;
        return new(old, target.Level);
    }
    public IntPairDto? SetCurrentMana(string id, int newValue)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        double old = target.CurrentMana;
        target.CurrentMana = newValue;
        return new((int) old, target.Level);
    }
    public IntPairDto? SetMagic(string id, int newValue)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        double old = target.Magic;
        target.Magic = newValue;
        return new((int) old, target.Level);
    }
    public IntPairDto? SetStrength(string id, int newValue)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        double old = target.Strength;
        target.Strength = newValue;
        return new((int) old, target.Level);
    }
    public IntPairDto? SetDefense(string id, int newValue)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        double old = target.Defense;
        target.Defense = newValue;
        return new((int) old, target.Level);
    }
    public IntPairDto? SetSpeed(string id, int newValue)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        double old = target.Speed;
        target.Speed = newValue;
        return new((int) old, target.Level);
    }

    public bool? StatsDisplayed(string id)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        return target.DisplayStats;
    }
    public bool? ToggleDisplayStats(string id)
    {
        if (!TryGetEntity(id, out var target))
        {
            return null;
        }
        target.DisplayStats = !target.DisplayStats;
        return target.DisplayStats;
    }
}