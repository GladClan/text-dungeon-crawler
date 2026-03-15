using GameServer.Domain.Enums;
using GameServer.Domain.Exceptions;

namespace GameServer.Domain.Entities;

public class DamageableEntity(
    string name,
    string entityType,
    int health,
    int mana,
    int magic,
    int strength,
    int defense,
    int level = 0,
    int experience = 0,
    Dictionary<DamageType, double>? resistances = null,
    Dictionary<Proficiency, double>? proficiencies = null
) : EntityMetadata(name, entityType)
{
    public int MaxHealth { get; protected set; } = health;
    public double Health { get; protected set; } = health;
    public int MaxMana { get; protected set; } = mana;
    public double Magic { get; protected set; } = magic;
    public double Mana { get; protected set; } = mana;
    public double Strength { get; protected set; } = strength;
    public double Defense { get; protected set; } = defense;
    public int Level { get; protected set; } = level;
    public int Experience { get; protected set; } = experience;
    public bool IsEntityAlive { get; protected set; } = true;
    public bool Visible { get; protected set; } = true;
    public Dictionary<DamageType, double> Resistances { get; private set; } = resistances ?? [];
    public Dictionary<Proficiency, double> Proficiencies { get; private set; } = proficiencies ?? new Dictionary<Proficiency, double>(){
        {Proficiency.bludgeoning, 0.85d},
        {Proficiency.potions, 0.85d},
        {Proficiency.slashing, 0.65d},
        {Proficiency.healing, 0.6d}
    };
    public Dictionary<Proficiency, double> ProficiencyEntries { get; private set; } = [];

    public void FixResistances(Dictionary<DamageType, double> resDict) {
        Resistances = [];
        foreach (DamageType key in resDict.Keys)
        {
            Resistances[key] = resDict[key];
        }
    }

    public void FixProficiencies(Dictionary<Proficiency, double> profDict)
    {
        Proficiencies = [];
        foreach (Proficiency key in profDict.Keys)
        {
            Proficiencies[key] = profDict[key];
        }
    }

    public double IncreaseResistance(DamageType key, double val)
    {
        Resistances[key] += val;
        return Resistances[key];
    }

    public double DecreaseResistance(DamageType key, double val)
    {
        Resistances[key] -= val;
        return Resistances[key];
    }

    public double Heal(DamageableEntity source, double amount)
    {
        if (!IsEntityAlive)
        {
            throw new EntityNotAliveException($"Entity {Name} is dead and cannot gain or lose health");
        }
        // apply healing resistance if exists
        double multiplier = Resistances.TryGetValue(DamageType.healing, out var value) ? value : 1;
        double actual = Math.Min(amount * multiplier, MaxHealth - Health);
        Health += actual;
        return actual;
    }

    public double TakeDamage(DamageableEntity source, double amount)
    {
        if (!IsEntityAlive)
        {
            throw new EntityNotAliveException($"Entity {Name} is dead and cannot gain or lose health");
        }
        double actual = Math.Min(amount, Health);
        Health += actual;
        if (Health <= 0)
        {
            OnDeath();
        }
        return actual;
    }

    public double ExpendMana(double amount)
    {
        if (!IsEntityAlive || amount > Mana)
        {
            throw new EntityNotAliveException($"Entity {Name} is dead and cannot gain or lose mana");
        }
        Mana -= amount;
        return Mana;
    }

    public double GainMana(double amount)
    {
        if (!IsEntityAlive)
        {
            throw new EntityNotAliveException($"Entity {Name} is dead and cannot gain or lose mana");
        }
        double actual = Math.Min(amount, MaxMana - Mana);
        Mana += actual;
        return actual;
    }

    private void OnDeath()
    {
        IsEntityAlive =false;
        foreach (Proficiency key in ProficiencyEntries.Keys)
        {
            ProficiencyEntries[key] = 0;
        }
        Experience =0;
        // level decreases?
        // Proficiencies decrease?
    }

    public double GetProficiency(Proficiency p)
    {
        return Proficiencies.TryGetValue(p, out var value) ? value : 0.5d;
    }

    public double GetResistance(DamageType d)
    {
        return Resistances.TryGetValue(d, out var value) ? value : 0d;
    }
}