using GameServer.Application.Common;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Domain.Entities.EntityAI;
using GameServer.Domain.Enums;
using GameServer.Domain.Skills;

namespace GameServer.Domain.Entities;

public class DamageableEntity
{
    private readonly double _levelStackMultiplier = 1.2;
    private readonly double _baseProficiencyMultiplier = 0.00125;
    private readonly int _defenseConstant = 40;
    private static int _entityCounter = 0;
    public string ID { get; }
    public string Name { get; set; }
    public string EntityType { get; set; }
    public string Race { get; set; }
    public string PartyId { get; set; }
    public int MaxHealth { get; set; }
    public double CurrentHealth { get; set; }
    public double HealthBuffer { get; set; }
    public double Magic { get; set; }
    public int MaxMana { get; set; }
    public double CurrentMana { get; set; }
    public double Strength { get; set; }
    public double Defense { get; set; }
    public DamageType AttackDamageType { get; set; } //The the different damage types that the entity can deal in their default attack.
    public bool DealsMagicDamage { get; set; } // Whether the entity's default attack deals magic damae or non-magic damage
    public int Level { get; set; }
    public int Experience { get; set; }
    public bool IsEntityAlive { get; set; }
    public bool DisplayStats { get; set; } // Whether the entity's stats will show details such as max health, strength, etc.
    public bool IsHidden { get; set; } = false; // For whether the entity is able to be seen in the combat menu or not
    public double Speed { get; set; } = 12d;
    public Dictionary<DamageType, double> Resistances { get; set; }
    public Dictionary<Proficiency, double> Proficiencies { get; set; }
    public Dictionary<Proficiency, double> ProficiencyEntries { get; set; }
    public EntityInventory Inventory { get; set; } = new();
    public List<Skill> Skills { get; set; } = [];
    public IEntityAI? AI { get; set; }
    public string? DefaultAttackMessageString; // Valid string parameters are: {SourceName} {TargetName} {AttackDamageType} {AmountSent} {AmountActual}
    public string DeathMessage { get; set; }
    public bool DoNotDeleteOnDeath { get; set; }
    public bool PlayerControlled { get; set; }

    public DamageableEntity()
    {
        ID = GenerateEntityId();
        //     Empty strings
        Name = string.Empty;
        EntityType = string.Empty;
        Race = string.Empty;
        PartyId = string.Empty;
        AttackDamageType = DamageType.damage;
        Resistances = [];
        Proficiencies = new Dictionary<Proficiency, double>(){
            {Proficiency.bludgeoning, 0.85d},
            {Proficiency.potions, 0.85d},
            {Proficiency.slashing, 0.65d},
            {Proficiency.healing, 0.6d}
        };
        ProficiencyEntries = [];
        DefaultAttackMessageString = null;
        DeathMessage = string.Empty;
    }
    public DamageableEntity(
        string name,
        string entityType,
        string race,
        string partyId,
        int health,
        int mana,
        int magic,
        int strength,
        int defense,
        DamageType attackType,
        bool dealsMagicDamage = false,
        int speed = 12,
        int level = 0,
        int experience = 0,
        Dictionary<DamageType, double>? resistances = null,
        Dictionary<Proficiency, double>? proficiencies = null,
        string? defaultAttackMessageString = null,
        string deathMessage = ""
    )
    {
        Name = name;
        EntityType = entityType;
        Race = race;
        PartyId = partyId;
        ID = GenerateEntityId();
        MaxHealth = health;
        CurrentHealth = health;
        Magic = magic;
        MaxMana = mana;
        CurrentMana = mana;
        Strength = strength;
        Defense = defense;
        AttackDamageType = attackType;
        DealsMagicDamage = dealsMagicDamage;
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
        ProficiencyEntries = [];
        DefaultAttackMessageString = defaultAttackMessageString;
        DeathMessage = deathMessage;
    }

    public DamageResultDto Heal(DamageableEntity source, double amount)
    {
        if (!IsEntityAlive)
        {
            return new DamageResultDto(
                    sent: amount,
                    error: $"{Name} is not alive and cannot be healed."
            );
        }
        if (!source.IsEntityAlive)
        {
            return new DamageResultDto(
                    sent: amount,
                    error: $"{source.Name} is not alive and cannot heal {Name}."
            );
        }
        // apply healing resistance if exists
        var healingResistance = GetResistanceMultiplier(DamageType.healing);
        double actual = amount * healingResistance.Value;
        CurrentHealth += Math.Min(actual, MaxHealth - CurrentHealth);
        bool wasFatal = DidEntityDie();
        return new(
            source.ID,
            ID,
            damage_healing_mana: 2,
            sent: amount,
            actual: actual,
            result: CurrentHealth,
            fatal: wasFatal
        );
    }

    public DamageResultDto AddHealthBuffer(DamageableEntity source, double amount)
    {
        if (!IsEntityAlive)
        {
            return new(
                sent: amount,
                error: $"{Name} is not alive and cannot be healed."
            );
        }
        if (!source.IsEntityAlive)
        {
            return new DamageResultDto(
                    sent: amount,
                    error: $"{source.Name} is not alive and cannot add health buffer to {Name}."
            );
        }
        HealthBuffer += amount;
        return new(
            source.ID,
            ID,
            damage_healing_mana: 4,
            sent: amount,
            actual: amount,
            result: HealthBuffer,
            fatal: !IsEntityAlive
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
        HealthBuffer -= actual;
        if (HealthBuffer < 0)
        {
            CurrentHealth += HealthBuffer;
            HealthBuffer = 0;
        }
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        bool wasFatal = DidEntityDie();
        return new(
            source.ID,
            ID,
            damage_healing_mana: 1,
            sent: amount,
            actual: actual,
            result: CurrentHealth,
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

    public DamageResultDto ChangeMana(double amount)
    {
        if (!IsEntityAlive)
        {
            return new(
                sent: amount,
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
            "",
            ID,
            damage_healing_mana: 3,
            sent: amount,
            actual: actual,
            result: CurrentMana,
            fatal: false
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

    /// <summary>
    /// Increases one of this entity's proficiency by the specified amount
    /// </summary>
    /// <param name="profEnum">The target proficiency to increase</param>
    /// <param name="amount">The amount to increase the proficiency by. If no proficiency exists in the entity, the base amount is 0.5</param>
    /// <returns>A string value for the proficiency and the resulting value of the proficiency</returns>
    public ProficiencyDto IncreaseProficiency(Proficiency profEnum, double amount)
    {
        if (Proficiencies.TryGetValue(profEnum, out _))
        {
            Proficiencies[profEnum] += amount;
        }
        else
        {
            Proficiencies[profEnum] = 0.5 + amount;
        }
        return new(profEnum.ToString(), Proficiencies[profEnum]);
    }

    /// <summary>
    /// Add a proficiency entry to this entity
    /// </summary>
    /// <param name="proficiency">The corresponding proficiency to add to</param>
    /// <param name="amount">The number of entries to add. Default: 1</param>
    /// <returns>The proficiency and the number of entries already present</returns>
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

    public int GetExperienceForNextLevel(int? level = null)
    {
        return (int)Math.Floor(100 * Math.Pow(1.2, level ?? Level));
    }

    /// <summary>
    /// Adds experience and levels up the entity if applicable
    /// When the entity levels up, its proficiency entries are loaded and used to increase actual proficiencies, using the formula: Pg = 0.00125 * Pe / Pa^2
    /// Pg is the proficiency gain
    /// Pe is the value of the proficiency entry, or the proficiency entry count
    /// Pa is the current proficiency, or proficiency actual value
    /// </summary>
    /// <param name="experience">The amount of experience to add</param>
    /// <returns cref="LevelUpDto">A LevelUpDto, even if the entity did not level up</returns>
    public LevelUpDto AddExperience(int experience)
    {
        // Save beginning values for the returned LevelUpDto later
        int expAtStart = Experience;
        int levelAtStart = Level;
        var proficienciesAtStart = Proficiencies.ToStringKeyDictionary();

        // What this function name actually says it should do
        Experience += experience;

        // If the experience is enough to level up:
        if (Experience >= GetExperienceForNextLevel())
        {
            Experience -= GetExperienceForNextLevel();
            Level++;

            // Discover if the entity increases by more than one level
            double stackValue = 0;
            if (Experience > GetExperienceForNextLevel())
            {
                stackValue = Math.Pow(_levelStackMultiplier, LevelUpStack(0));
            }

            // Increase proficiencies according to the entries in proficiencyEntries
            foreach (var entry in ProficiencyEntries)
            {
                double currentProficiencyMultiplier = Math.Pow(GetProficiencyMultiplier(entry.Key).Value, 2);
                double increase = stackValue * _baseProficiencyMultiplier * entry.Value / currentProficiencyMultiplier;
                IncreaseProficiency(entry.Key, increase);
            }

            // Empty proficiencyEntries and reset health and mana
            ProficiencyEntries = [];
            CurrentHealth = MaxHealth;
            CurrentMana = MaxMana;

            return new LevelUpDto
            {
                ExpAtStart = expAtStart,
                ExpAfter = Experience,
                LevelAtStart = levelAtStart,
                LevelAfter = Level,
                ProficienciesAtStart = proficienciesAtStart,
                ProficienciesAfter = Proficiencies.ToStringKeyDictionary()
            };
        }
        else return new LevelUpDto
        {
            ExpAtStart = expAtStart,
            ExpAfter = Experience,
            LevelAtStart = Level,
            LevelAfter = Level,
            ProficienciesAtStart = proficienciesAtStart,
            ProficienciesAfter = proficienciesAtStart
        };
    }

    /// <summary>
    /// A recursive funtion which determins the number of levels the entity gains from its experience, if greater than zero.
    /// </summary>
    /// <param name="stack">The number of times the entity has already leveled up, after the first time (defaultly set this to zero when calling this function)</param>
    /// <returns>The number levels the entity gained from its experience points</returns>
    private int LevelUpStack(int stack = 0)
    {
        if (Experience < GetExperienceForNextLevel(Level))
        {
            return stack;
        }
        Experience -= GetExperienceForNextLevel(Level);
        Level++;
        return LevelUpStack(stack + 1);
    }

    /// <summary>
    /// Creates a clone of this entity. Should be self-explanatory.
    /// </summary>
    /// <returns>A clone of this entity, including clones of the inventory and skills. The skills and items will be reset to their base stats.</returns>
    public virtual DamageableEntity Clone()
    {
        var cloneSkills = new List<Skill>();
        foreach (var skill in Skills)
        {
            cloneSkills.Add(skill.Clone());
        }

        var result = new DamageableEntity(
            name: Name,
            entityType: EntityType,
            race: Race,
            partyId: PartyId,
            health: MaxHealth,
            mana: MaxMana,
            magic: (int)Magic,
            strength: (int)Strength,
            defense: (int)Defense,
            attackType: AttackDamageType,
            dealsMagicDamage: DealsMagicDamage,
            speed: (int)Speed,
            level: Level,
            experience: Experience,
            resistances: Resistances,
            proficiencies: Proficiencies,
            defaultAttackMessageString: DefaultAttackMessageString,
            deathMessage: DeathMessage
        )
        {
            CurrentHealth = CurrentHealth,
            CurrentMana = CurrentMana,
            Inventory = Inventory.Clone(),
            Skills = cloneSkills
        };

        return result;
    }

    /// <summary>
    /// Generate a unique ID for a new entity. The ID is based of the number of entities created in this session, the ordinal date, and the entity type.
    /// </summary>
    /// <returns>A unique id for the entity, formatted {entity type}_{timestamp}_{entity counter}</returns>
    private string GenerateEntityId()
    {
        string counter = Interlocked.Increment(ref _entityCounter).ToString("D3");
        string timestamp = OrdinalDateString.GetOrdinalDate(3);
        string prefix = EntityType.PadRight(3, '_')[..3].ToLowerInvariant();

        return $"{prefix}_{timestamp}_{counter}";
    }
}