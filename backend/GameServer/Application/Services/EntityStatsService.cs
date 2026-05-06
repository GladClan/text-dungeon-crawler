using System.Diagnostics.CodeAnalysis;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Requests;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Application.Services;

public sealed class EntityStatsService(EntityStore entityStore)
{
    private readonly EntityStore _entities = entityStore;

    private bool TryGetEntity(string id, [NotNullWhen(true)] out DamageableEntity? target)
    {
            return _entities.TryGet(id, out target) && target is not null;
    }
}

// setVisible
// isVisible
// isHidden
// setIsHidden

// getExperience
// setExperience
// getLevel
// setLevel

// getStats
// getMaxHealth
// setMaxHealth
// getHealth
// setHealth
// getMaxMana
// setMaxMana
// getMana
// setMana
// getMagic
// setMagic
// getStrength
// setStrength
// getDefense
// setDefense
// getAllProficiencies