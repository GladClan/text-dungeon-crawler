using GameServer.Contracts.DTOs;
using GameServer.Domain.Battle;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Items.ItemsLibrary.InitialRelease;

public sealed class ErrorItem : Useable
{
    private int _uses = 0;
    private double _damage = 150d;

    public ErrorItem(double damage = 150d, int uses = 0): base(
        name: "Mysterious Cloud",
        tag: "error",
        cost: 0,
        description: "Description",
        consumable: true,
        sellable: false,
        element: DamageType.damage,
        multiTarget: true,
        targetsLimit: 100,
        proficiency: Proficiency.hand,
        itemType: ActionType.Attack,
        shopType: 1000,
        rarity: 1000,
        collection: 99
    )
    {
        _damage = damage;
        _uses = uses;
    }

    public override bool CanUse(DamageableEntity target)
    {
        return true;
    }

    public override Item Clone()
    {
        return new ErrorItem(_damage, _uses);
    }

    private void OnUse(double proficiency)
    {
        _damage *= proficiency / (_uses + 1);
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(Proficiency.hand);
        double damage = (_damage * source.GetProficiencyMultiplier(ItemProficiency).Value) + source.Magic - 10;
        _uses++;
        List<DamageResultDto> results = [];
        results.Add(DoEffect(source, mainTarget, damage));
        string subMessage = "";
        if (subTargets is not null)
        {
            foreach (var target in subTargets)
            {
                results.Add(DoEffect(source, target, damage));
                subMessage += $"\n{target.Name} is enveloped.";
            }
        }
        double totalDamage = 0;
        foreach (var dto in results)
        {
            totalDamage += dto.AmountActual;
        }
        string message = (subTargets is null) ? $"{mainTarget.Name} takes {results[0].AmountActual} {Element} damage from a mysterious, buzzing cloud summoned by {source.Name}"
            : $"{source.Name} summons a mysterious, buzzing cloud which envelops {mainTarget.Name} and others, dealing {totalDamage} {Element} damage.{subMessage}";
        
        var effect = new EffectDto
        {
            Message = message,
            Results = results,
            WasMagic = true
        };
        OnUse(source.GetProficiencyMultiplier(Proficiency.destiny).Value);
        return effect;
    }

    private DamageResultDto DoEffect(DamageableEntity source, DamageableEntity target, double damage)
    {
        if (_uses < 10)
        {
            return target.TakeDamage(source, damage * 3, Element);
        }
        if (_uses < 30)
        {
            return target.TakeDamage(source, damage * 2, Element);
        }
        if (_uses < 45)
        {
            return target.TakeDamage(source, damage, Element);
        }
        return target.TakeDamage(source, _damage / 2, Element);
    }
}

/// <summary cref="Useable">
/// A catapult which can be fired for massive damage<br>
/// Extends Useable
/// </summary>
/// <remarks>
/// Takes three turns to use. One turn to load, one turn to draw, and one turn to fire.<br>
/// Loading and drawing each add a bonus of the source entity's raw strength to the damage total.<br>
/// Can only be used by an entity with strength 15 or greater.
/// 70 base damage + at least 15 * 2 = 100 damage at normal ranged_weapons proficiency and normal crushing resistance
/// </remarks>
public sealed class Catapult : Useable
{
    public enum PrepStage
    {
        empty = 0,
        loaded = 1,
        ready = 2
    }
    private PrepStage _prepStage = PrepStage.empty;
    private double _strengthModifier = 0;
    private static readonly double _damage = 70;
    public Catapult(PrepStage prepStage = PrepStage.empty, double strengthModifier = 0): base(
        name: "Catapult",
        tag: "catapult",
        cost: 0,
        description: "Takes one turn to load, one turn to draw, and one turn to fire. Deals 70 damage to up to three targets, the damage increased by the strength of the one who loads and draws the weapon.",
        consumable: true,
        sellable: true,
        element: DamageType.crushing,
        multiTarget: true,
        targetsLimit: 3,
        proficiency: Proficiency.ranged_weapons,
        itemType: ActionType.Attack,
        shopType: (int)ShopTypes.Siege_Weapons,
        rarity: (int)Rarities.impossible,
        collection: (int)ShopCollections.Weapon
    )
    {
        _prepStage = prepStage;
        _strengthModifier = strengthModifier;
    }

    public override bool CanUse(DamageableEntity target)
    {
        return target.Strength >= 15 || _prepStage == PrepStage.ready;
    }

    public override Item Clone()
    {
        return new Catapult(_prepStage, _strengthModifier);
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        string message = "The catapult must be broken";
        List<DamageResultDto> results = [];
        // Stage 1: Load the catapult
        if (_prepStage == PrepStage.empty)
        {
            _strengthModifier += source.Strength;
            message = $"{source.Name} loads the catapult. It still needs to be drawn and locked before it can be fired.";
            _prepStage = PrepStage.loaded;
        }
        // Satge 2: Draw the catapult
        else if (_prepStage == PrepStage.loaded)
        {
            _strengthModifier += source.Strength;
            message = $"{source.Name} draws the catapult. It is ready to be fired.";
            _prepStage = PrepStage.ready;
        }
        // Satge 3: Fire the catapult
        else if (_prepStage == PrepStage.ready)
        {
            double damage = (_damage * source.GetProficiencyMultiplier(ItemProficiency).Value) + _strengthModifier;
            results.Add(mainTarget.TakeDamage(source, damage, Element));
            message = $"{source.Name} fires the catapult at {mainTarget.Name}! It hits, dealing {results[0].AmountActual} {Element} damage.";
            if (subTargets is not null)
            {
                foreach (DamageableEntity target in subTargets)
                {
                    var result = target.TakeDamage(source, damage, Element);
                    message += $"\nIt also hits {target.Name}, dealing {result.AmountActual} damage.";
                    results.Add(result);
                }
            }
            _strengthModifier = 0;
            _prepStage = PrepStage.empty;
        }
        return new(
            message: message,
            results: results,
            wasMagic: false
        );
    }
}

/// <summary>
/// The most common type of sword
/// </summary>
/// <remarks>
/// 16 base damage, augmented by slashing proficiency and strength
/// </remarks>
public sealed class ArmingSword : Useable
{
    private static readonly double _damage = 16;
    
    public ArmingSword(): base(
        name: "Arming Sword",
        tag: "arming-sword",
        cost: 10,
        description: "A standard, military-grade armament. Deals 16 damage.",
        consumable: false,
        sellable: true,
        element: DamageType.slashing,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.slashing,
        itemType: ActionType.Attack,
        shopType: (int)ShopTypes.Equipment,
        rarity: (int)Rarities.common,
        collection: (int)ShopCollections.Weapon
    ) { }

    public override Item Clone()
    {
        return new ArmingSword();
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(ItemProficiency);
        double damage = (_damage * source.GetProficiencyMultiplier(ItemProficiency).Value) + source.Strength - 10;
        var result = mainTarget.TakeDamage(source, damage, Element);
        return new(
            message: $"{source.Name} slashes {mainTarget.Name} with their {Name}, dealing {result.AmountActual} {Element} damage.",
            results: [result],
            wasMagic: false
        );
    }
}

/*
// Constructor template
public useable_name(): base(
    name: "",
    tag: "",
    cost: 0,
    description: "Description",
    consumable: true,
    sellable: true,
    element: DamageType.damage,
    multiTarget: false,
    targetsLimit: 1,
    proficiency: Proficiency.hand,
    itemType: ActionType.Attack,
    shopType: (int)ShopTypes.Equipment,
    rarity: (int)Rarities.common,
    collection: (int)ShopCollections.Weapon
) { }

// Things involved in every ItemEffect :)
    source.AddProficiencyEntry(ItemProficiency);                // Add proficiency entries for each item used
    source.GetProficiencyMultiplier(ItemProficiency).Value;     // Multiply damage or effect by proficiencies
    + source.Strength - 10 || source.Magic - 10;                // Add in strength or magic bonus
    return new EffectDto                                        // All items return EffectDto, which includes an array of DamageResultDtos
*/