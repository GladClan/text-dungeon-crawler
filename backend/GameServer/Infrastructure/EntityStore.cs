using GameServer.Domain.Entities;
using GameServer.Domain.Enums;
using GameServer.Domain.Items.ItemsLibrary.InitialRelease;
using GameServer.Domain.Skills.SkillsLibrary.InitialRelease;
using System.Collections.Concurrent;

namespace GameServer.Infrastructure;

public sealed class EntityStore
{
    private readonly ConcurrentDictionary<string, DamageableEntity> _entities = new();

    public EntityStore()
    {
        var warrior = new DamageableEntity(
            name: "Main Character",
            entityType: "main",
            race: Species.Human.ToString(),
            partyId: "player-party",
            health: 150,
            mana: 80,
            magic: 14,
            strength: 14,
            defense: 14,
            attackType: DamageType.crushing,
            speed: 13,
            resistances: SpeciesDictionary.PlayableSpeciesStats[Species.Human].Resistances,
            proficiencies: new Dictionary<Proficiency, double>{
                {Proficiency.hand, 1.8d},
                {Proficiency.slashing, 1.5d},
                {Proficiency.bludgeoning, 0.85d},
                {Proficiency.potions, 0.85d},
                {Proficiency.healing, 0.6d}
            }
        )
        {
            Inventory = new()
            {
                Items = [
                    new ArmingSword(),
                    new HealingPotion(),
                    new HealingPotion(),
                    new ManaPotion()
                ]
            }
        };
        var mage = new DamageableEntity(
            name: "Mage boy",
            entityType: "mage",
            race: Species.Human.ToString(),
            partyId: "player-party",
            health: 100,
            mana: 100,
            magic: 18,
            strength: 8,
            defense: 8,
            attackType: DamageType.crushing,
            resistances: SpeciesDictionary.PlayableSpeciesStats[Species.Human].Resistances,
            proficiencies: new Dictionary<Proficiency, double>(){
                {Proficiency.spellstrike, 1.7d},
                {Proficiency.healing, 2.9d},
                {Proficiency.bludgeoning, 0.9d},
                {Proficiency.potions, 1d},
                {Proficiency.slashing, 0.5d}
            }
        )
        {
            Inventory = new()
            {
                Items = [
                    new Dagger(),
                    new HealingGem(),
                    new ManaPotion(),
                    new ManaPotion()
                ]
            },
            Skills = [
                new Heal(),
                new Aeroblade(),
                new SpellShield()
            ]
        };
        var tank = new DamageableEntity(
            name: "Buff Guard",
            entityType: "warrior",
            race: Species.Human.ToString(),
            partyId: "player-party",
            health: 200,
            mana: 50,
            magic: 12,
            strength: 14,
            defense: 18,
            attackType: DamageType.crushing,
            speed: 18,
            resistances: SpeciesDictionary.PlayableSpeciesStats[Species.Human].Resistances,
            proficiencies: new Dictionary<Proficiency, double>
            {
                {Proficiency.bludgeoning, 1.5d},
                {Proficiency.hand, 1d},
                {Proficiency.potions, 0.85d},
                {Proficiency.slashing, 0.7d},
                {Proficiency.healing, 0.75d},
                {Proficiency.stealth, 0.7d}
            }
        )
        {
            Inventory = new()
            {
                Items = [
                    new Club(),
                    new HealingPotion(),
                    new HealingPotion(),
                    new RazeChestplate()
                ]
            },
            Skills = [
                new Steal()
            ]
        };

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