using GameServer.Domain.Entities;
using System.Collections.Concurrent;

public sealed class EntityStore
{
    private readonly ConcurrentDictionary<string, DamageableEntity> _entities = new();

    public EntityStore()
    {
        var starter = new DamageableEntity("Mage boy", "party", "human", 100, 100, 18, 8, 8);
        _entities[starter.ID] = starter;
    }

    public bool TryGet(string id, out DamageableEntity? entity) => _entities.TryGetValue(id, out entity);

    public void Add(DamageableEntity entity)
    {
        _entities[entity.ID] = entity;
    }

    public string[] GetAllNames() => [.. _entities.Values.Select(entity => entity.Name)];

    public string[] GetAllIds() => [.. _entities.Values.Select(entity => entity.ID)];
}