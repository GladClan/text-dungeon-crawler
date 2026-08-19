using GameServer.Contracts.DTOs;
using GameServer.Contracts.Requests;
using GameServer.Domain.Battle;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Skills.SkillsLibrary.InitialRelease;

public sealed class ErrorSkill : Skill
{
    private int _damage = 30;
    private int _useage = 0;
    public ErrorSkill(): base(
        name: "Mysterious Cloud",
        tag: "error",
        description: "Summons a dark, dangerous cloud. Deals 30 damage to a group of enemies.",
        cost: 0,
        element: DamageType.damage,
        proficiency: Proficiency.spellstrike,
        multiTarget: true,
        targetsLimit: 100,
        skillType: ActionType.Attack,
        level: 1
    ) { }

    public override Skill Clone()
    {
        return new ErrorSkill
        {
            Name = "Mysterious Cloud",
            Tag = "error",
            Description = Description,
            Cost = Cost,
            Element = DamageType.damage,
            SkillProficiency = Proficiency.spellstrike,
            MultiTarget = true,
            TargetsLimit = 100,
            SkillType = ActionType.Attack,
            Level = Level
        };
    }

    public override bool CanLevelUpSkill()
    {
        return false;
    }

    public override void LevelUpSkill()
    {
        Level++;
        _damage += 10;
        Description = $"Summons a dark, dangerous cloud. Deals {_damage} damage to a group of enemies";
    }

    public override EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(SkillProficiency);
        _useage++;
        List<DamageResultDto> results = [];
        DoEffect(results, mainTarget, source);
        string subMessage = "";
        if (subTargets is not null)
        {
            foreach (DamageableEntity target in subTargets)
            {
                DoEffect(results, target, source);
                subMessage += $"\n{target.Name} is enveloped.";
            }
        }
        if (_useage > Level * 10)
        {
            LevelUpSkill();
            _useage = 0;
        }
        double damage = 0;
        foreach (var dto in results)
        {
            damage += dto.AmountActual;
        }
        string message = (subTargets is null) ? $"{mainTarget.Name} takes {damage} {Element} damage from a mysterious, buzzing cloud summoned by {source.Name}"
            : $"{source.Name} summons a mysterious, buzzing cloud which envelops {mainTarget.Name} and others, dealing {damage} {Element} damage.{subMessage}";
        
        var effect = new EffectDto
        {
            Message = message,
            Results = [.. results]
        };
        return effect;
    }

    private void DoEffect(List<DamageResultDto> resultDtos, DamageableEntity target, DamageableEntity source)
    {
        double damage = (_damage + (source.Magic - 10)) * source.GetProficiencyMultiplier(SkillProficiency).Value;
        resultDtos.Add(target.TakeDamage(source, damage, Element));
        if (Level > 3)
        {
            resultDtos.Add(target.TakeDamage(source, damage / 2, Element));
            if (Level > 7)
            {
                resultDtos.Add(target.TakeDamage(source, damage / 3, Element));
            }
        }
    }
}

/// <summary>
/// Charge a target and attack with a vicious bite, dealing piercing damage, potentially poisoning the enemy (based on potions proficiency)
/// </summary>
public sealed class PoisonBite : Skill
{
    private static readonly Random _random = new();
    private int _useage = 0;
    private int _poisonCount = 0;
    private double _damage = 12;
    private int _duration = 5;
    private double _bitingProficiency = 0.5;
    private double _poisonProficiency = 0.5;
    public PoisonBite(): base(
        name: "Poison Bite",
        tag: "poison-bite",
        description: "Charge target and attack with a vicious bite, dealing 12 piercing damage, potentially poisoning enemy.",
        cost: 0,
        element: DamageType.poisoning,
        proficiency: Proficiency.piercing,
        multiTarget: false,
        targetsLimit: 1,
        skillType: ActionType.Attack,
        level: 1
    ) { }

    public override Skill Clone()
    {
        return new PoisonBite
        {
            Name = Name,
            Tag = "poison-bite",
            Description = Description,
            Cost = Cost,
            Element = DamageType.poisoning,
            SkillProficiency = Proficiency.piercing,
            MultiTarget = false,
            TargetsLimit = 1,
            SkillType = ActionType.Attack,
            Level = Level
        };
    }

    public override bool CanLevelUpSkill()
    {
        return (
            Level <= 6 &&
            _poisonCount > Level &&
            _bitingProficiency > 0.5 &&
            _useage > Level * 10 / _poisonProficiency
        );
    }

    public override void LevelUpSkill()
    {
        if (!CanLevelUpSkill())
        {
            return;
        }
        Level++;
        if (_poisonCount > Level)
        {
            _duration += 2;
        }
        _damage += 2.25d;
    }

    public void FixName()
    {
        double poisonChance = _poisonProficiency < 0 ?
            1 :
            (_poisonProficiency >= 0 && _poisonProficiency <= 2) ?
            1 - (_poisonProficiency / 2) :
            0;

        Description = $"Charge target and attack with a vicious bite, dealing {_damage} piercing damage, potentially poisoning enemy ({poisonChance:P1}% chance).";
    }

    public override EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(SkillProficiency);

        _bitingProficiency = source.GetProficiencyMultiplier(SkillProficiency).Value;
        if (source.GetProficiencyMultiplier(Proficiency.poison).Value != _poisonProficiency)
        {
            _poisonProficiency = source.GetProficiencyMultiplier(Proficiency.poison).Value;
            FixName();
        }

        double damage = (_damage + (source.Strength - 10)) * _bitingProficiency;
        List<DamageResultDto> results = [];
        results.Add(mainTarget.TakeDamage(source, damage, Element));
        string message = $"{source.Name} bites {mainTarget.Name}, dealing {results[0].AmountActual} damage.";
        _useage++;
        if (source.GetProficiencyMultiplier(Proficiency.poison).Value > (_random.Next(200) / 100d))
        {
            source.AddProficiencyEntry(Proficiency.poison);
            bool success = battle.AddContinuousEffect(
                new StatusEffect(
                    tag: Tag,
                    targetId: mainTarget.ID,
                    message: $"{mainTarget.Name} suffered poison damage from {source.Name}'s bite!",
                    stat: StatType.CurrentHealth,
                    delta: _damage / 2 * source.GetProficiencyMultiplier(Proficiency.poison).Value,
                    duration: _duration,
                    damageType: Element
                )
            );
            if (success)
            {
                message += $"\n{mainTarget.Name} was poisoned!";
                _poisonCount++;
            }
        }
        return new(
            message: message,
            results: results,
            wasMagic: false
        );
    }
}

/// <summary>
/// Summons a swarm of giant spiders that obey the target, lasting for several rounds before disappearing.
/// </summary>
public sealed class SummonSpiders : Skill
{
    private int _useage = 0;
    private int _spiderCount = 3;
    private int _duration = 3;
    private int _healthMultiplier = 6;
    private double _proficiency = 0.5;
    public SummonSpiders(): base(
        name: "Summon Spiders",
        tag: "summon-spiders",
        cost: 50,
        element: DamageType.poisoning,
        description: "Summons 3 deadly spiders that obey the target, attacking who their master attacks. The spiders last for 3 rounds before disappearing.",
        proficiency: Proficiency.spellcasting,
        multiTarget: false,
        targetsLimit: 1,
        skillType: ActionType.Summon,
        level: 1
    ) { }

    private SummonSpiders(int cost, int level, int spiderCount, int duration, int healthMultiplier): base(
        name: "Summon Spiders",
        tag: "summon-spiders",
        cost: cost,
        element: DamageType.poisoning,
        description: $"Summons {spiderCount} deadly spiders that obey the target, attacking who their master attacks. The spiders last for {duration} rounds before disappearing.",
        proficiency: Proficiency.spellcasting,
        multiTarget: false,
        targetsLimit: 1,
        skillType: ActionType.Summon,
        level: level
    )
    {
        _spiderCount = spiderCount;
        _duration = duration;
        _healthMultiplier = healthMultiplier;
    }

    public override Skill Clone()
    {
        return new SummonSpiders(Cost, Level, _spiderCount, _duration, _healthMultiplier);
    }

    public override bool CanLevelUpSkill()
    {
        return (
            _proficiency >= 0.75 &&
            _useage > Level * 9 / _proficiency &&
            Level < 5
        );
    }

    public override void LevelUpSkill()
    {
        if (CanLevelUpSkill())
        {
            Level++;
            _duration += 2;
            _healthMultiplier += 2;
            if (Level % 2 == 1)
            {
                _spiderCount++;
                if (Level == 5)
                {
                    _spiderCount++;
                }
            }
            Cost += _duration / _spiderCount;
            Description = $"Summons {_spiderCount} deadly spiders that obey the target, attacking who their master attacks. The spiders last for {_duration} rounds before disappearing.";
        }
    }

    public override EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(SkillProficiency);
        _proficiency = source.GetProficiencyMultiplier(SkillProficiency).Value;
        List<ProficiencyRequest> summonProficiencies = [
            new ProficiencyRequest
            {
                Type = Proficiency.poison.ToString(),
                Value = Math.Max((source.Magic + 2) / 10, 0.2)
                // Normal value: 1.2d
            },
            new ProficiencyRequest
            {
                Type = Proficiency.piercing.ToString(),
                Value = Math.Max((source.Magic - 2) / 10, 0.1)
                // Normal value: 0.8d
            }
        ];

        double healthMultiplier = source.Magic + _healthMultiplier;
        int spiderStrength = (int)(mainTarget.GetProficiencyMultiplier(SkillProficiency).Value * 6);
        int spiderDefense = (int)(mainTarget.GetProficiencyMultiplier(SkillProficiency).Value * 3);

        for (int i = 0; i < _spiderCount; i++)
        {
            var result = battle.AddEntityToBattle(
                new DamageableEntityRequest
                {
                    Name = $"Summoned Spider {i + 1}",
                    EntityType = "summon",
                    Race = "Giant Spider",
                    PartyId = mainTarget.PartyId,
                    Health = (int)(mainTarget.GetProficiencyMultiplier(SkillProficiency).Value * healthMultiplier),
                    Magic = 0,
                    Mana = 0,
                    Strength = spiderStrength,
                    Defense = spiderDefense,
                    Speed = 21,
                    Level = Level,
                    Proficiencies = summonProficiencies,
                    SkilTags = ["poison-bite"]
                }
            );
            if (result.Entity is not null)
            {
                _ = battle.AddContinuousEffect(
                    new StatBuffEffect(
                        tag: Tag,
                        targetId: result.Entity.Id,
                        message: $"The spell of {source.Name} runs out. {result.Entity.Name} withers away ({_spiderCount - (i + 1)} spiders left).",
                        stat: StatType.CurrentHealth,
                        delta: result.Entity.MaxHealth * 2,
                        duration: 1,
                        appliesOverTime: false,
                        turnDelayBeforeActive: battle.InitiativeOrder.Count * _duration
                    )
                );
            }
        }
        _useage++;
        return new(
            message: $"{source.Name} summoned {_spiderCount} spiders to attack with {mainTarget.Name}. The spiders ready themselves to attack.",
            results: [new(
                sourceId: source.ID,
                targetId: source.ID,
                actionType: (int)SkillType,
                sent: _spiderCount,
                actual: _spiderCount,
                result: _spiderCount,
                fatal: false
            )],
            wasMagic: true
        );
    }
}

/// <summary>
/// Creates a shield of magic that blocks damage for a single target before shattering.
/// </summary>
public sealed class SpellShield: Skill
{
    private int _useage = 0;
    private int _block = 30;
    private double _proficiency = 0.5;
    private double _magic = 0;
    private double _actual_cost = 9;
    public SpellShield(): base(
        name: "Spellshield",
        tag: "spell-shield",
        description: "Creates a shield of magic that blocks 30 damage before shattering.",
        cost: 9,
        element: DamageType.enchanting,
        proficiency: Proficiency.enchanting,
        multiTarget: false,
        targetsLimit: 1,
        skillType: ActionType.Defense,
        level: 1
    ) { }

    public SpellShield(int cost, int skillLevel, int block): base(
        name: "Spellshield",
        tag: "spell-shield",
        description: $"Creates a shield of magic that blocks {block} damage before shattering.",
        cost: cost,
        element: DamageType.enchanting,
        proficiency: Proficiency.enchanting,
        multiTarget: false,
        targetsLimit: 1,
        skillType: ActionType.Defense,
        level: skillLevel
    )
    {
        _block = block;
    }

    public override Skill Clone()
    {
        return new SpellShield(Cost, Level, _block);
    }

    public override bool CanLevelUpSkill()
    {
        return (
            Level < 6 &&
            _proficiency > 0.75d &&
            _magic > 0 &&
            _useage > Level * 8 / _proficiency
        );
    }

    public override void LevelUpSkill()
    {
        if (!CanLevelUpSkill())
        {
            return;
        }
        Level++;
        _block += 6;
        if (Level == 6)
        {
            _block += 10;
        }
        _actual_cost += 1.5;
        Cost = (int)_actual_cost;
        Description = $"Creates a shield of magic that blocks {_block} damage before shattering";
    }

    public override EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(SkillProficiency);
        _proficiency = source.GetProficiencyMultiplier(SkillProficiency).Value;
        _magic = source.Magic;
        _useage++;

        double buffed = (_block * source.GetProficiencyMultiplier(SkillProficiency).Value) + _magic;
        var result = Cast(mainTarget, source, buffed);

        if ( CanLevelUpSkill() && result.AmountSent > buffed)
        {
            LevelUpSkill();
        }

        return new EffectDto
        {
            Message = $"{source.Name} casts Spellshield on {mainTarget.Name}.",
            Results = [result],
            WasMagic = true
        };
    }

    private DamageResultDto Cast(DamageableEntity target, DamageableEntity source, double amount)
    {
        double result = amount / target.GetResistanceMultiplier(Element).Value;
        target.AddHealthBuffer(source, amount);

        return new(
            sourceId: source.ID,
            targetId: target.ID,
            actionType: (int)SkillType,
            sent: amount,
            actual: result,
            result: target.HealthBuffer,
            fatal: false
        );
    }
}

/// <summary>
/// Absorbs health from the target and adds it to the caster
/// </summary>
public sealed class Absorb: Skill
{
    double _damage = 10;
    double _proficiency = 0.5;
    double _useage = 0;
    public Absorb(): base(
        name: "Absorb",
        tag: "absorb",
        description: "Absorbs 10 health from the enemy and adds it to the caster.",
        cost: 7,
        element: DamageType.necro,
        proficiency: Proficiency.spellstrike,
        multiTarget: false,
        targetsLimit: 1,
        skillType: ActionType.Attack,
        level: 1
    ) { }
    
    public Absorb(int cost, int skillLevel, int damage): base(
        name: "Absorb",
        tag: "absorb",
        description: $"Absorbs {damage} health from the enemy and adds it to the caster.",
        cost: cost,
        element: DamageType.necro,
        proficiency: Proficiency.spellstrike,
        multiTarget: false,
        targetsLimit: 1,
        skillType: ActionType.Attack,
        level: skillLevel
    )
    {
        _damage = damage;
    }

    public override Skill Clone()
    {
        return new Absorb(Cost, Level, (int)_damage);
    }

    public override bool CanLevelUpSkill()
    {
        return (
            Level < 4 &&
            _proficiency > 0.7d &&
            _useage > Level * 8 / _proficiency
        );
    }
    
    public override void LevelUpSkill()
    {
        Level++;

        // Increase damage
        double gain = (int)Math.Pow(_damage, Math.Clamp(_proficiency, 1.1, 1.25)) - _damage;
        gain = Math.Clamp(gain, 2, 9);
        _damage += gain;

        Cost += (int)(_proficiency + 1);
        Description = $"Absorbs {_damage} health from the enemy and adds it to the caster.";
    }

    public override EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(SkillProficiency);
        _useage++;
        _proficiency = source.GetProficiencyMultiplier(SkillProficiency).Value;
        double buffed = (_damage *= source.GetProficiencyMultiplier(SkillProficiency).Value) + source.Magic - 10;
        var result = mainTarget.TakeDamage(source, buffed, Element);

        var healing = source.Heal(source, result.AmountActual);

        return new(
            message: $"{source.Name} deals {result.AmountActual} damage to {mainTarget.Name} and gains {healing.AmountActual} health",
            results: [
                new DamageResultDto(
                    sourceId: source.ID,
                    targetId: mainTarget.ID,
                    actionType: (int)ActionType.Attack,
                    sent: buffed,
                    actual: result.AmountActual,
                    result: mainTarget.CurrentHealth,
                    fatal: result.Fatal
                ),
                new DamageResultDto(
                    sourceId: source.ID,
                    targetId: source.ID,
                    actionType: (int)ActionType.Healing,
                    sent: result.AmountActual,
                    actual: healing.AmountActual,
                    result: source.CurrentHealth,
                    fatal: healing.Fatal
                )
            ],
            wasMagic: true
        );
    }
}

/// <summary>
/// Creates a blade of wind travelling in an expanding arc. Deals wind damage to any creature in its path (max targets set by TargetsLimit).
/// </summary>
public sealed class Aeroblade: Skill
{
    double _useage = 0;
    double _damage = 20;
    double _spellProficiency = 0.5d;
    public Aeroblade(): base(
        name: "Aeroblade",
        tag: "aeroblade",
        description: "Creates a blade of wind travelling in an expanding arc. Deals 20 wind damage to any creature in its path (3 targets).",
        cost: 9,
        element: DamageType.aerial,
        proficiency: Proficiency.spellstrike,
        multiTarget: true,
        targetsLimit: 3,
        skillType: ActionType.Attack,
        level: 1
    ) { }

    public Aeroblade(int cost, int skillLevel, int damage, int targetsLimit): base(
        name: "Aeroblade",
        tag: "aeroblade",
        description: $"Creates a blade of wind travelling in an expanding arc. Deals {damage} wind damage to any creature in its path ({targetsLimit} targets).",
        cost: cost,
        element: DamageType.aerial,
        proficiency: Proficiency.spellstrike,
        multiTarget: true,
        targetsLimit: targetsLimit,
        skillType: ActionType.Attack,
        level: skillLevel
    ) { }

    public override Skill Clone()
    {
        return new Aeroblade(Cost, Level, (int)_damage, TargetsLimit);
    }

    public override bool CanLevelUpSkill()
    {
        return (
            Level < 6 &&
            _spellProficiency > 0.75d &&
            _useage > Level * 8 / _spellProficiency
        );
    }

    public override void LevelUpSkill()
    {
        Level++;
        TargetsLimit++;
        Cost += 1;
        _damage += 3;
        Description = $"Creates a blade of wind travelling in an expanding arc. Deals {_damage} wind damage to any creature in its path ({TargetsLimit} targets).";
    }

    public override EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(SkillProficiency);
        _useage++;
        _spellProficiency = source.GetProficiencyMultiplier(SkillProficiency).Value;
        double buffed = (_damage * source.GetProficiencyMultiplier(SkillProficiency).Value) + source.Magic - 10;

        List<DamageResultDto> result = [];
        string resultMessage = $"{source.Name} casts {Name}.";
        result.Add(mainTarget.TakeDamage(source, buffed, Element));
        resultMessage += $"\n{mainTarget.Name} is struck by the blade of wind, taking {result[0].AmountActual} damage";
        if (subTargets is not null)
        {
            foreach (var target in subTargets)
            {
                var damageResult = target.TakeDamage(source, buffed, Element);
                result.Add(damageResult);
                resultMessage += $"\n{target.Name} is struck by the blade of wind, taking {damageResult.AmountActual} damage.";
            }
        }

        return new(
            message: resultMessage,
            results: result,
            wasMagic: true
        );
    }
}

/// <summary>
/// Attempt to steal from a target.<br>
/// Can be leveled up to increase chance of stealing.<br>
/// Can also level up into Mug or Magnet - introducing damage and multiple targets, respectively.
/// </summary>
public sealed class Steal: Skill
{
    private static readonly Random random = new();
    private double _stealth = 0.5;
    private double _nobility = 0.5;
    private int _stealChance = 50;
    private double _damage = 0;
    private double _cost_actual = 0;
    private bool _secondSteal = false;
    public Steal(): base(
        name: "Steal",
        tag: "steal",
        description: "Attempt to steal an item and gold from target",
        cost: 0,
        element: DamageType.damage,
        proficiency: Proficiency.stealth,
        multiTarget: false,
        targetsLimit: 1,
        skillType: ActionType.Other,
        level: 1
    ) { }
    
    public Steal(int cost, int skillLevel, int stealChance, int damage, int targetsLimit, string skillName, string description, bool secondSteal): base(
        name: skillName,
        tag: "steal",
        description: description,
        cost: cost,
        element: DamageType.damage,
        proficiency: Proficiency.stealth,
        multiTarget: targetsLimit > 1,
        targetsLimit: targetsLimit,
        skillType: damage > 0 ? ActionType.Attack : ActionType.Other,
        level: skillLevel
    )
    {
        _stealChance = stealChance;
        _damage = damage;
        _secondSteal = secondSteal;
    }

    public override Skill Clone()
    {
        return new Steal(Cost, Level, _stealChance, (int)_damage, TargetsLimit, Name, Description, _secondSteal);
    }

    public override bool CanLevelUpSkill()
    {
        return (
            _stealth > 0.5d &&
            Level < 7
        );
    }

    public override void LevelUpSkill()
    {
        Level++;
        if (Level < 5)
        {
            // Increase stealing chance
            _stealChance += Level * 2;
        }
        else
        {
            _stealChance += Level;
        }
        if (Level >= 3)
        {
            _secondSteal = true;
            // At level 3, this skill evolves
            // Depending on the nobility stat of the source (compared to the stealth stat) this skill can either be Mug OR Magnet
            // Mug damages target with a chance of stealing stuff
            if (_stealth <= _nobility)
            {
                SkillType = ActionType.Attack;
                _damage += 3;
                _stealChance -= Level;
                // Change name to "Mug" and update description
                Name = "Mug";
                string appendage = MultiTarget ? $" from {TargetsLimit} targets" : "";
                Description = $"Deal {_damage} damage to target and attempt to steal an item and gold{appendage} (%{_stealChance} chance).";
            }
            // Magnet steals items and gold from a group of targets
            else
            {
                Name = "Magnet";
                MultiTarget = true;
                TargetsLimit++;
                string s = _damage > 0 ? $"Deal {_damage} damage s" : "S";
                Description = $"{s}teal an item and gold from {TargetsLimit} targets (%{_stealChance} chance).";
            }
            // Increase skill cost slightly
            _cost_actual += 1.5;
            Cost = (int) _cost_actual;
        }
    }

    public override EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(SkillProficiency);
        _stealth = source.GetProficiencyMultiplier(SkillProficiency).Value;
        _nobility = source.GetProficiencyMultiplier(Proficiency.nobility).Value;

        List<string> resultMessages = [];

        List<DamageResultDto> results = [];

        results.Add(DoSteal(source, mainTarget, resultMessages));
        if (MultiTarget && subTargets is not null && TargetsLimit > 0)
        {
            for (int i = 0; i < TargetsLimit && i < subTargets.Count; i++)
            {
                results.Add(DoSteal(source, subTargets[i], resultMessages));
            }
        }

        return new(
            message: string.Join("\n", resultMessages),
            results: results,
            wasMagic: true
        );
    }

    private DamageResultDto DoSteal(DamageableEntity source, DamageableEntity target, List<string> messages)
    {
        double sent = 0, actual = 0, result = 0;
        bool fatal = false;
        if (SkillType == ActionType.Attack)
        {
            var damageResult = DoDamage(source,target);
            sent = damageResult.AmountSent;
            actual = damageResult.AmountActual;
            result = damageResult.NewValue;
            fatal = damageResult.Fatal;
        }
        // If the target notices the attempt to steal
        bool suspicion = false;

        //  Check that target has any gold
        if (target.Inventory.Gold > 0)
        {
            if (random.Next((int)(80 / _stealth)) < _stealChance)
            {
                    // Assign how much gold to steal
                    int stolen = random.Next(target.Inventory.Gold);
                    // steal gold
                    target.Inventory.Gold -= stolen;
                    source.Inventory.Gold += stolen;
                    // add result message to messages
                    messages.Add($"{source.Name} stole {stolen} gold from {target.Name}");
            }
            if (random.Next(100) < _stealChance)
            {
                suspicion = true;
            }
        }
        else
        {
            messages.Add($"{target.Name} doesn't have any gold to steal!");
        }

        // Check that target has any items
        if (target.Inventory.Items.Count > 0)
        {
            if (random.Next((int)(100 / _stealth)) < _stealChance + (suspicion ? 0 : _stealChance / 2))
            {
                // Assign item
                int index = random.Next(target.Inventory.Items.Count);
                var itemStolen = target.Inventory.Items[index];
                // steal item
                target.Inventory.Items.RemoveAt(index);
                source.Inventory.AddItem(itemStolen);
                messages.Add($"{source.Name} stole {itemStolen.Name} from {target.Name}");

                // Very low chance to steal a second item
                if (
                    _secondSteal &&
                    target.Inventory.Items.Count > 0 &&
                    random.Next((int)(100 / _stealth)) < _stealChance - (suspicion ? _stealChance / 2 : _stealChance / 1.5)
                )
                {
                    // Assign item
                    int secondIndex = random.Next(target.Inventory.Items.Count);
                    var secondItemStolen = target.Inventory.Items[index];
                    // steal item
                    target.Inventory.Items.RemoveAt(secondIndex);
                    source.Inventory.AddItem(secondItemStolen);
                    messages.Add($"{source.Name} also managed to steal {secondItemStolen.Name} from {target.Name}");
                }
            }
        }
        else
        {
            messages.Add($"{target.Name} does not have any items to steal!");
        }

        return new(
            sourceId: source.ID,
            targetId: target.ID,
            actionType: (int)SkillType,
            sent: sent,
            actual: actual,
            result: result,
            fatal: fatal
        );
    }

    private DamageResultDto DoDamage(DamageableEntity source, DamageableEntity target)
    {
        double buffed = _damage * source.GetProficiencyMultiplier(source.DealsMagicDamage ? Proficiency.spellstrike : Proficiency.combat).Value;
        buffed += source.Strength - 10;
        var result = target.TakeDamage(source, buffed, Element);
        return new(
            sourceId: source.ID,
            targetId: target.ID,
            actionType: (int)ActionType.Attack,
            sent: buffed,
            actual: result.AmountActual,
            result: result.NewValue,
            fatal: result.Fatal
        );
    }
}

/*
// Constructor template
public skill_name(): base(
    name: "",
    tag: "",
    description: "",
    cost: 0,
    element: DamageType.,
    proficiency: Proficiency.,
    multiTarget: false,
    targetsLimit: 1,
    skillType: ActionType.Defense,
    level: 1
) { }

// Things involved in every skill :)
SkillEffect
    source.AddProficiencyEntry(SkillProficiency);               // Add proficiency entries for each skill used
    source.GetProficiencyMultiplier(SkillProficiency).Value;    // Multiply damage or effect by proficiencies
    + source.Magic - 10 || source.Strength - 10;                // Add in magic or strength bonus
    return new EffectDto                                        // All skills return EffectDto, which includes an array of DamageResultDtos
*/