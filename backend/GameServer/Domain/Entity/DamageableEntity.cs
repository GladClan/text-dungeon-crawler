using GameServer.Contracts.DTOs;
using GameServer.Domain.Enums;
using GameServer.Domain.Skills;

namespace GameServer.Domain.Entities;

public class DamageableEntity
{
    private readonly int _defenseConstant = 40;
    private static int _entityCounter = 0;
    public string ID { get; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public int MaxHealth { get; set; }
    public double CurrentHealth { get; set; }
    public double Magic { get; set; }
    public int MaxMana { get; set; }
    public double CurrentMana { get; set; }
    public double Strength { get; set; }
    public double Defense { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public bool IsEntityAlive { get; set; }
    public bool DisplayStats { get; set; } // Whether the entity's stats will show details such as max health, strength, etc.
    public bool IsHidden { get; set; } = false; // For whether the entity is able to be seen in the combat menu or not
    public double Speed { get; set; } = 12d;
    public Dictionary<DamageType, double> Resistances { get; set; } = [];
    public Dictionary<Proficiency, double> Proficiencies { get; set; } = [];
    public Dictionary<Proficiency, double> ProficiencyEntries { get; set; } = [];
    public EntityInventory Inventory { get; set; } = new();
    public List<Skill> Skills { get; set; } = [];
    // public EntityAI AI { get; set; }
    public string DeathMessage { get; set; } = string.Empty;

    public DamageableEntity() {}
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
        Dictionary<Proficiency, double>? proficiencies = null,
        string deathMessage = ""
    )
    {
        Name = name;
        EntityType = entityType;
        Race = race;
        ID = GenerateEntityId();
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
        DisplayStats = true;
        Resistances = resistances ?? [];
        Proficiencies = proficiencies ?? new Dictionary<Proficiency, double>(){
            {Proficiency.bludgeoning, 0.85d},
            {Proficiency.potions, 0.85d},
            {Proficiency.slashing, 0.65d},
            {Proficiency.healing, 0.6d}
        };
        DeathMessage = deathMessage;
    }

    public HealResultDto Heal(DamageableEntity source, double amount)
    {
        if (!IsEntityAlive)
        {
            return new HealResultDto(
                    sent: amount,
                    error: $"{Name} is not alive and cannot be healed."
            );
        }
        if (!source.IsEntityAlive)
        {
            return new HealResultDto(
                    sent: amount,
                    error: $"{source.Name} is not alive and cannot heal {Name}."
            );
        }
        // apply healing resistance if exists
        var healingResistance = GetResistanceMultiplier(DamageType.healing);
        double actual = amount * healingResistance.Value;
        CurrentHealth += Math.Min(actual, MaxHealth - CurrentHealth);
        bool wasFatal = DidEntityDie();
        return new HealResultDto(
                sent: amount,
                actual: actual,
                result: CurrentHealth,
                fatal: wasFatal
        );
    }

    public DamageResultDto TakeDamage(DamageableEntity source, double amount, DamageType damageType)
    {
        if (!IsEntityAlive)
        {
            return new DamageResultDto(
                sent: amount,
                error: $"{Name} is not alive and cannot take damage."
            );
        }
        if (!source.IsEntityAlive)
        {
            return new(
                sent: amount,
                error: $"{source.Name} is not alive and cannot deal damage."
            );
        }
        var resistanceDto = GetResistanceMultiplier(damageType);
        double actual = amount * resistanceDto.Value;
        CurrentHealth -= actual;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        bool wasFatal = DidEntityDie();
        return new(
            sent: amount,
            actual: actual,
            result: CurrentHealth,
            blocked: amount - actual,
            fatal: wasFatal
        );
    }

    public bool DidEntityDie()
    {
        if (CurrentHealth <= 0)
        {
                OnDeath();
                return true;
        }
        return false;
    }

    private void OnDeath()
    {
        IsEntityAlive = false;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }
        foreach (Proficiency key in ProficiencyEntries.Keys)
        {
            ProficiencyEntries[key] = 0;
        }
        Experience = 0;
        // level decreases?
        // Proficiencies decrease?
        // Resistance to necro or radiant increases?
    }

    public ManaChangeDto ChangeMana(double amount)
    {
        if (!IsEntityAlive)
        {
            return new(
                amountSent: amount,
                error: $"{Name} is not alive and caoont gain mana"
            );
        }
        double actual = amount;
        if (actual > MaxMana - CurrentMana)
        {
            actual = MaxMana - CurrentMana;
        }
        else if (amount < -CurrentMana)
        {
            actual = -CurrentMana;
        }
        CurrentMana += amount;
        return new(
            amountSent: amount,
            amountActual: actual,
            newMana: CurrentMana
        );
    }

    public ProficiencyDto GetProficiencyMultiplier(Proficiency p)
    {
        var result = Proficiencies.TryGetValue(p, out var value) ? value : 0.5d;
        return new(
            proficiency: p.ToString(),
            value: result
        );
    }

    public ResistanceDto GetResistanceMultiplier(DamageType dtEnum)
    {
        var result = Resistances.TryGetValue(dtEnum, out var value) ? value : 1d;
        if (dtEnum == DamageType.crushing || dtEnum == DamageType.slashing || dtEnum == DamageType.piercing)
        {
                result += Math.Abs(result) * (Defense / _defenseConstant);
        } else 
        if (dtEnum != DamageType.damage && Resistances.TryGetValue(DamageType.spellstrike, out double magicRes))
            {
                result += magicRes;
            }
        return new(dtEnum.ToString(), result);
    }

    public ResistanceDto IncreaseResistance(DamageType dtEnum, double amount)
    {
        if (Resistances.TryGetValue(dtEnum, out _))
        {
            Resistances[dtEnum] += amount;
        }
        else
        {
            Resistances[dtEnum] = 1 + amount;
        }
        return new ResistanceDto(dtEnum.ToString(), Resistances[dtEnum]);
    }

    public ProficiencyDto IncreaseProficiency(Proficiency profEnum, double amount)
    {
        if (Proficiencies.TryGetValue(profEnum, out _))
        {
            Proficiencies[profEnum] += amount;
        }
        else
        {
            Proficiencies[profEnum] = amount;
        }
        return new(profEnum.ToString(), Proficiencies[profEnum]);
    }

    public StringDoubleDto AddProficiencyEntry(Proficiency proficiency, int amount = 1)
    {
        if (ProficiencyEntries.TryGetValue(proficiency, out _))
        {
            ProficiencyEntries[proficiency] += amount;
        }
        else
        {
            ProficiencyEntries[proficiency] = amount;
        }
        return new(proficiency.ToString(), ProficiencyEntries[proficiency]);
    }

    private string GenerateEntityId()
    {
        string counter = Interlocked.Increment(ref _entityCounter).ToString("D3");
        string timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
        string prefix = EntityType.PadRight(3, '_')[..3].ToLowerInvariant();

        return $"{prefix}_{timestamp}_{counter}";
    }
}