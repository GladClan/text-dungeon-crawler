using System.Collections.Concurrent;
using GameServer.Domain.Map;
using GameServer.Domain.Map.Scene.InitialReleaseScenes;

namespace GameServer.Infrastructure;

public sealed class ContextStore
{
    private readonly ConcurrentDictionary<string, GameContext> _saves = new();

    public ContextStore()
    {
        var context = new GameContext(
            scenes: [new QuickTestSceneObject(partyId: "player-party")],
            gameContextState: new TestGameContextState()
        );
    }
}

/*
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;
using System.Collections.Concurrent;

namespace GameServer.Infrastructure;

public sealed class EntityStore
{
    private readonly ConcurrentDictionary<string, DamageableEntity> _entities = new();

    public EntityStore()
    {
        var warrior = new DamageableEntity(
            ...
        );
        var mage = new DamageableEntity(
            ...
        );
        var tank = new DamageableEntity(
            ...
        );

        _entities[warrior.ID] = warrior;
        _entities[mage.ID] = mage;
        _entities[tank.ID] = tank;
    }

    public bool TryGet(string id, out DamageableEntity? entity) => _entities.TryGetValue(id, out entity);

    public void Add(DamageableEntity entity)
    {
        _entities[entity.ID] = entity;
    }

    public DamageableEntity? Remove(DamageableEntity entity)
    {
        _entities.TryRemove(entity.ID, out var removed);
        return removed;
    }

    public string[] GetAllNames() => [.. _entities.Values.Select(entity => entity.Name)];

    public string[] GetAllIds() => [.. _entities.Values.Select(entity => entity.ID)];

    public List<DamageableEntity> GetParty(string partyId)
    {
        return [.. _entities.Values.Where(e => e.PartyId.Equals(partyId, StringComparison.InvariantCultureIgnoreCase))];
    }
}
*/