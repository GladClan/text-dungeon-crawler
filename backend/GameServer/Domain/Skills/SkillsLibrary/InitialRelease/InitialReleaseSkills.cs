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
        return new ErrorSkill();
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
        resultDtos.Add(target.TakeDamage(source, _damage, Element));
        if (Level > 3)
        {
            resultDtos.Add(target.TakeDamage(source, _damage / 2, Element));
            if (Level > 7)
            {
                resultDtos.Add(target.TakeDamage(source, _damage / 3, Element));
            }
        }
    }
}

public sealed class PoisonBite : Skill
{
    private static readonly Random _random = new();
    private int _useage = 0;
    private int _poisonCount = 0;
    private double _damage = 5;
    private int _duration = 5;
    private double _bitingProficiency = 0.5;
    private double _potionsProficiency = 0.5;
    public PoisonBite(): base(
        name: "Poison Bite",
        tag: "poison-bite",
        description: "Charge target and attack with a vicious bite, dealing 5 piercing damage, potentially poisoning enemy.",
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
        return new PoisonBite();
    }

    public override void LevelUpSkill()
    {
        Level++;
        if (_poisonCount > Level)
        {
            _duration++;
        }
        _damage += _bitingProficiency;
    }

    public void FixName()
    {
        double poisonChance = _potionsProficiency < 0 ?
            1 :
            (_potionsProficiency >= 0 && _potionsProficiency <= 2) ?
            1 - (_potionsProficiency / 2) :
            0;

        Description = $"Charge target and attack with a vicious bite, dealing {_damage} piercing damage, potentially poisoning enemy ({poisonChance:P1}% chance).";
    }

    public override EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(SkillProficiency);

        _bitingProficiency = source.GetProficiencyMultiplier(SkillProficiency).Value;
        if (source.GetProficiencyMultiplier(Proficiency.potions).Value != _potionsProficiency)
        {
            _potionsProficiency = source.GetProficiencyMultiplier(Proficiency.potions).Value;
            FixName();
        }

        double damage = _damage * _bitingProficiency * source.Strength / 2;
        List<DamageResultDto> results = [];
        results.Add(mainTarget.TakeDamage(source, damage, Element));
        string message = $"{source.Name} bites {mainTarget.Name}, dealing {results[0].AmountActual} damage.";
        _useage++;
        if (source.GetProficiencyMultiplier(Proficiency.potions).Value > (_random.Next(200) / 100d))
        {
            bool success = battle.AddContinuousEffect(
                new StatusEffect(
                    tag: Tag,
                    targetId: mainTarget.ID,
                    message: $"{mainTarget.Name} suffered poison damage from {source.Name}'s bite!",
                    stat: StatType.CurrentHealth,
                    delta: _damage / 2 * source.GetProficiencyMultiplier(Proficiency.potions).Value,
                    duration: _duration,
                    damageType: DamageType.poisoning
                )
            );
            if (success)
            {
                message += $"\n{mainTarget.Name} was poisoned!";
                _poisonCount++;
            }
        }
        if (_useage > Level * 9 / source.GetProficiencyMultiplier(Proficiency.piercing).Value)
        {
            LevelUpSkill();
        }
        return new(
            message: message,
            results: results,
            wasMagic: false
        );
    }
}

public sealed class SummonSpiders : Skill
{
    private int _useage = 0;
    private int _spiderCount = 3;
    private int _duration = 3;
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

    public override Skill Clone()
    {
        return new SummonSpiders();
    }

    public override void LevelUpSkill()
    {
        Level++;
        _duration++;
        if (Level % 2 == 1)
            _spiderCount++;
        Description = $"Summons {_spiderCount} deadly spiders that obey the target, attacking who their master attacks. The spiders last for {_duration} rounds before disappearing.";
    }

    private static string OrdinalNumber(int num) => num switch
    {
        1 => "first",
        2 => "second",
        3 => "third",
        4 => "fourth",
        5 => "fifth",
        6 => "sixth",
        7 => "seventh",
        8 => "eighth",
        9 => "ninth",
        10 => "tenth",
        11 => "eleventh",
        12 => "twelfth",
        13 => "thirteenth",
        14 => "fourteenth",
        15 => "fifteenth",
        16 => "sixteenth",
        _ => "umpteenth (that was a lot of spiders)"
    };

    public override EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(SkillProficiency);
        List<ProficiencyRequest> summonProficiencies = [
            new ProficiencyRequest
            {
                Type = Proficiency.potions.ToString(),
                Value = 1.5d
            },
            new ProficiencyRequest
            {
                Type = Proficiency.piercing.ToString(),
                Value = 1d
            }
        ];
        var request = new DamageableEntityRequest
        {
            Name = "Summoned Spider",
            EntityType = "summon",
            Race = "Giant Spider",
            PartyId = mainTarget.PartyId,
            Health = (int)(mainTarget.GetProficiencyMultiplier(Proficiency.spellcasting).Value * 10),
            Magic = 0,
            Mana = 0,
            Strength = (int)(mainTarget.GetProficiencyMultiplier(Proficiency.spellcasting).Value * 10),
            Defense = (int)(mainTarget.GetProficiencyMultiplier(Proficiency.spellcasting).Value * 3),
            Speed = 21,
            Level = Level,
            Proficiencies = summonProficiencies,
            SkilTags = ["poison-bite"]
        };
        for (int i = 0; i < _spiderCount; i++)
        {
            var result = battle.AddEntityToBattle(request);
            if (result.Entity is not null)
            {
                bool success = battle.AddContinuousEffect(
                    new StatBuffEffect(
                        tag: Tag,
                        targetId: result.Entity.Id,
                        message: $"The spell of {source.Name} runs out. The {OrdinalNumber(i+1)} spider they summoned withers away.",
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
        if (_useage > Level * 12 / source.GetProficiencyMultiplier(Proficiency.spellcasting).Value)
        {
            LevelUpSkill();
        }
        return new(
            message: $"{source.Name} summoned {_spiderCount} spiders to attack with {mainTarget.Name}. The spiders ready themselves to attack.",
            results: [],
            wasMagic: true
        );
    }
}

public sealed class SpellShield: Skill
{
    private int _useage = 0;
    private double _block = 12;
    public SpellShield(): base(
        name: "Spellshield",
        tag: "spell-shield",
        description: "Creates a shield of magic that blocks 12 damage before shattering.",
        cost: 9,
        element: DamageType.enchanting,
        proficiency: Proficiency.spellcasting,
        multiTarget: false,
        targetsLimit: 1,
        skillType: ActionType.Defense,
        level: 1
    ) { }

    public override Skill Clone()
    {
        return new SpellShield();
    }

    public override void LevelUpSkill()
    {
        Level++;
        _block += Math.Sqrt(_useage);
        Description = $"Creates a shield of magic that blocks {_block} damage before shattering";
    }

    public override EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(SkillProficiency);
        _useage++;

        double buffed = _block * source.GetProficiencyMultiplier(SkillProficiency).Value;
        var result = Cast(mainTarget, source, buffed);

        if (result.AmountSent > buffed && _useage > Level * 9 / source.GetProficiencyMultiplier(Proficiency.spellcasting).Value)
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
            damage_healing_mana: (int)Damage_Healing_Mana.Other,
            sent: amount,
            actual: result,
            result: target.HealthBuffer,
            fatal: false
        );
    }
}