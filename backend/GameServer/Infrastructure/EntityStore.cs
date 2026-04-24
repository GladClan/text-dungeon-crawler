using GameServer.Domain.Entities;
using GameServer.Domain.Enums;
using System.Collections.Concurrent;

public sealed class EntityStore
{
    private readonly ConcurrentDictionary<string, DamageableEntity> _entities = new();

    public EntityStore()
    {
        var warrior = new DamageableEntity(
            name: "Main Character",
            entityType: "player",
            race: "human",
            health: 150,
            mana: 80,
            magic: 14,
            strength: 14,
            defense: 14,
            speed: 13,
            level: 0,
            experience: 0,
            resistances: null,
            proficiencies: new Dictionary<Proficiency, double>{
                {Proficiency.hand, 1.8d},
                {Proficiency.slashing, 1.5d},
                {Proficiency.bludgeoning, 0.85d},
                {Proficiency.potions, 0.85d},
                {Proficiency.healing, 0.6d}
            }
        );
        var mage = new DamageableEntity(
            name: "Mage boy",
            entityType: "player",
            race: "human",
            health: 100,
            mana: 100,
            magic: 18,
            strength: 8,
            defense: 8,
            proficiencies: new Dictionary<Proficiency, double>{
                {Proficiency.spellstrike, 1.7d},
                {Proficiency.healing, 2.9d},
                {Proficiency.bludgeoning, 0.9d},
                {Proficiency.potions, 1d},
                {Proficiency.slashing, 0.5d}
        });
        var tank = new DamageableEntity(
            name: "Buff Guard",
            entityType: "player",
            race: "human",
            health: 200,
            mana: 50,
            magic: 12,
            strength: 14,
            defense: 18,
            speed: 18,
            proficiencies: new Dictionary<Proficiency, double>
            {
                {Proficiency.bludgeoning, 1.5d},
                {Proficiency.hand, 1d},
                {Proficiency.potions, 0.85d},
                {Proficiency.slashing, 0.7d},
                {Proficiency.healing, 0.75d}
            });
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
}