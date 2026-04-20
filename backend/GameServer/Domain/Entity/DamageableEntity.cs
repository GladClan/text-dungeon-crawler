using GameServer.Domain.Enums;
using GameServer.Domain.Exceptions;

namespace GameServer.Domain.Entities;

public class DamageableEntity
{
    
    private static int _entityCounter = 0;
    public string Name { get; protected set; } = string.Empty;
    public string EntityType { get; protected set; } = string.Empty;
    public string Race { get; protected set; } = string.Empty;
    public Guid ID { get; } = Guid.NewGuid();
    public string SimpleID { get; } = string.Empty;
    public int MaxHealth { get; protected set; }
    public double CurrentHealth { get; protected set; }
    public int MaxMana { get; protected set; }
    public double Magic { get; protected set; }
    public double CurrentMana { get; protected set; }
    public double Strength { get; protected set; }
    public double Defense { get; protected set; }
    public int Level { get; protected set; }
    public int Experience { get; protected set; }
    public bool IsEntityAlive { get; protected set; }
    public bool Visible { get; protected set; }
    public Dictionary<DamageType, double> Resistances { get; private set; } = [];
    public Dictionary<Proficiency, double> Proficiencies { get; private set; } = [];
    public Dictionary<Proficiency, double> ProficiencyEntries { get; private set; } = [];
    public EntityInventory Inventory { get; private set; } = new();
    public EntitySkills Skills { get; private set; } = new();
    public double Speed { get; private set; } = 12d;
    // public EntityAI AI { get; private set; }
    public bool IsHidden { get; private set; } = false;

    DamageableEntity() {}
    public DamageableEntity(
        string name,
        string entityType,
        string race,
        int health,
        int mana,
        int magic,
        int strength,
        int defense,
        int speed = 12,
        int level = 0,
        int experience = 0,
        Dictionary<DamageType, double>? resistances = null,
        Dictionary<Proficiency, double>? proficiencies = null
    )
    {
        Name = name;
        EntityType = entityType;
        Race = race;
        ID = Guid.NewGuid();
        SimpleID = GenerateEntityId();
        MaxHealth = health;
        CurrentHealth = health;
        Magic = magic;
        MaxMana = mana;
        CurrentMana = mana;
        Strength = strength;
        Defense = defense;
        Speed = speed;
        Level = level;
        Experience = experience;
        IsEntityAlive = true;
        Visible = true;
        Resistances = resistances ?? [];
        Proficiencies = proficiencies ?? new Dictionary<Proficiency, double>(){
            {Proficiency.bludgeoning, 0.85d},
            {Proficiency.potions, 0.85d},
            {Proficiency.slashing, 0.65d},
            {Proficiency.healing, 0.6d}
        };
    }

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
        double actual = Math.Min(amount * multiplier, MaxHealth - CurrentHealth);
        CurrentHealth += actual;
        return actual;
    }

    public double TakeDamage(DamageableEntity source, double amount)
    {
        if (!IsEntityAlive)
        {
            throw new EntityNotAliveException($"Entity {Name} is dead and cannot gain or lose health");
        }
        double actual = Math.Min(amount, CurrentHealth);
        CurrentHealth += actual;
        if (CurrentHealth <= 0)
        {
            OnDeath();
        }
        return actual;
    }

    public double ExpendMana(double amount)
    {
        if (!IsEntityAlive || amount > CurrentMana)
        {
            throw new EntityNotAliveException($"Entity {Name} is dead and cannot gain or lose mana");
        }
        CurrentMana -= amount;
        return CurrentMana;
    }

    public double GainMana(double amount)
    {
        if (!IsEntityAlive)
        {
            throw new EntityNotAliveException($"Entity {Name} is dead and cannot gain or lose mana");
        }
        double actual = Math.Min(amount, MaxMana - CurrentMana);
        CurrentMana += actual;
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

    public bool Hide()
    {
        if (IsHidden)
        {
            return false;
        }
        IsHidden = true;
        return true;
    }

    public bool Reveal()
    {
        if (!IsHidden)
        {
            return false;
        }
        IsHidden = false;
        return true;
    }

    public DamageableEntity Clone()
    {
        return new DamageableEntity(
            Name,
            EntityType,
            Race,
            MaxHealth,
            MaxMana,
            (int)Magic,
            (int)Strength,
            (int)Defense,
            (int)Speed,
            Level,
            Experience,
            new Dictionary<DamageType, double>(Resistances),
            new Dictionary<Proficiency, double>(Proficiencies)
        )
        {
            Inventory = Inventory.Clone(),
            Skills = Skills.Clone(),
            Speed = Speed,
            IsHidden = IsHidden,
        };
    }

    private string GenerateEntityId()
    {
        string counter = Interlocked.Increment(ref _entityCounter).ToString("D3");
        string timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
        string prefix = EntityType.PadRight(3, '_')[..3].ToLowerInvariant();

        return $"{prefix}_{timestamp}_{counter}";
    }
}

/**

*/