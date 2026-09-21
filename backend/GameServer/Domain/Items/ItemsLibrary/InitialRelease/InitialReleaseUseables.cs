using GameServer.Contracts.DTOs;
using GameServer.Domain.Battle;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;
using GameServer.Domain.Skills.SkillsLibrary.InitialRelease;

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
/// A catapult which can be fired for massive damage<br/>
/// Extends Useable
/// </summary>
/// <param cref="tag">Tag: "catapult"</param>
/// <remarks>
/// Takes three turns to use. One turn to load, one turn to draw, and one turn to fire.<br/>
/// Loading and drawing each add a bonus of the source entity's raw strength to the damage total.<br/>
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
/// <param cref="tag">Tag: "sword-arming"</param>
/// <remarks>
/// 16 base damage, augmented by slashing proficiency and strength
/// </remarks>
public sealed class ArmingSword : Useable
{
    private static readonly double _damage = 16;
    
    public ArmingSword(): base(
        name: "Arming Sword",
        tag: "sword-arming",
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

/// <summary>
/// A healing potion, used to heal. Default 25 health.
/// </summary>
/// <param cref="tag">Tag: "potion-healing"</param>
public sealed class HealingPotion : Useable
{
    private readonly int _health;
    public HealingPotion(int health = 25): base(
        name: "Healing potion",
        tag: "potion-healing",
        cost: 5,
        description: "A potion that can be applied to wounds or drunk to heal 25 health.",
        consumable: true,
        sellable: true,
        element: DamageType.healing,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.potions,
        itemType: ActionType.Healing,
        shopType: (int)ShopTypes.Potions,
        rarity: (int)Rarities.common,
        collection: (int)ShopCollections.Potion
    )
    {
        _health = health;
        Name = health < 15 ? "Minor Health Potion" :
            health < 30 ? "Health Potion" :
            health < 59 ? "Potent Health Potion" :
            "Greater Health Potion";
        Description = $"A potion that can be applied to wounds or drunk to heal {_health} health.";
    }
    public override Item Clone()
    {
        return new HealingPotion(_health);
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(ItemProficiency);
        double healingValue = _health * source.GetProficiencyMultiplier(ItemProficiency).Value;
        var result = mainTarget.Heal(source, healingValue);
        source.Inventory.Items.Remove(this);
        return new(
            message: $"{source.Name} used {Name} on {mainTarget.Name}, healing {result.AmountActual} health.",
            results: [result],
            wasMagic: false
        );
    }
}

/// <summary>
/// A potion used to restore mana. Default 15 mana.
/// </summary>
/// <param cref="tag">Tag: "potion-mana"</param>
public sealed class ManaPotion : Useable
{
    private readonly int _mana;
    public ManaPotion(int mana = 15): base(
        name: "Mana Potion",
        tag: "potion-mana",
        cost: mana / 5,
        description: "Description",
        consumable: true,
        sellable: true,
        element: DamageType.healing,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.potions,
        itemType: ActionType.Buff,
        shopType: (int)ShopTypes.Potions,
        rarity: (int)Rarities.common,
        collection: (int)ShopCollections.Potion
    )
    {
        _mana = mana;
        Name = mana < 10 ? "Minor Mana Potion" :
            mana < 20 ? "Mana Potion" :
            mana < 30 ? "Greater Mana Potion" :
            "Potent Mana Potion";
        Description = $"A potion that regenerates {_mana} mana when drunk.";
    }

    public override Item Clone()
    {
        return new ManaPotion(_mana);
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(ItemProficiency);
        double buffed = _mana * source.GetProficiencyMultiplier(ItemProficiency).Value;
        
        var result = mainTarget.ChangeMana(buffed);

        source.Inventory.Items.Remove(this);
        return new(
            message: $"{mainTarget.Name} drank a {Name} from {source.Name} and gained {result.AmountActual} mana.",
            results: [result],
            wasMagic: false
        );
    }
}

/// <summary>
/// A naginata that deals 21 base damage, with a 1 in 5 chance of dealing 30 magical physical damage
/// </summary>
/// <param cref="tag">Tag: "polearm-ogre-slayer"</param>
public sealed class OgreSlayerPolearm: Useable
{
    private static readonly Random r = new();
    private readonly double _damage = 21;
    public OgreSlayerPolearm(): base(
        name: "Ogre Slayer",
        tag: "polearm-ogre-slayer",
        cost: 180,
        description: "A naginata built to fight ogres. Sometimes striking a target will create a magical explosion that devastates the target.",
        consumable: false,
        sellable: true,
        element: DamageType.slashing,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.slashing,
        itemType: ActionType.Attack,
        shopType: (int)ShopTypes.Equipment,
        rarity: (int)Rarities.uncommon,
        collection: (int)ShopCollections.Weapon
    ) { }

    public override Item Clone()
    {
        return new OgreSlayerPolearm();
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {    
        source.AddProficiencyEntry(ItemProficiency);
        double buffed = (_damage * source.GetProficiencyMultiplier(ItemProficiency).Value) + source.Strength - 10;
        List<DamageResultDto> results = [];
        results.Add(mainTarget.TakeDamage(source, buffed, Element));

        string resultMessage = $"{source.Name} slices {mainTarget.Name} with their {Name}, dealing {results[0]} slicing damage.";

        if (r.Next(100) < source.GetProficiencyMultiplier(Proficiency.spellstrike).Value * 20)
        {
            double extraDmg = (_damage * 1.4 * source.GetProficiencyMultiplier(Proficiency.spellstrike).Value) + source.Magic - 10;
            results.Add(mainTarget.TakeDamage(source, extraDmg, DamageType.physical));
            resultMessage += $"\nThe {Name} seems to vibrate and flex, and the air trembles around {mainTarget.Name}. Suddenly, the force explodes, dealing {results[1].AmountActual} damage to {mainTarget.Name}.";
        }

        return new(
            message: resultMessage,
            results: results,
            wasMagic: false
        );
    }
}

/// <summary cref="Shatter">
/// A spell scroll that has similar effect to the Skill Shatter. Deals 25 physical magic damage to a single target and learns the shatter spell to the source.<br/>
/// Using this item removes it from the source's inventory. If the item cannot be found in the source's inventory, the item gets grumpy and removes the source's inventory.
/// </summary>
/// <param cref="tag">Tag: "spell-scroll-shatter"</param>
public sealed class ShatterScroll: Useable
{
    private readonly double _damage = 25;
    public ShatterScroll(): base(
        name: "Shatter Spell Scroll",
        tag: "spell-scroll-shatter",
        cost: 300,
        description: "A scroll written in intricate script, almost seemin to flicker and glow. It is tied with a ribbon that instructs: reading this scroll will imbue you with the knowledge of SHATTER, casting the spell upon a target.",
        consumable: true,
        sellable: true,
        element: DamageType.physical,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.spellstrike,
        itemType: ActionType.Attack,
        shopType: (int)ShopTypes.Magic,
        rarity: (int)Rarities.rare,
        collection: (int)ShopCollections.Artifact
    ) { }

    public override Item Clone()
    {
        return new ShatterScroll();
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(ItemProficiency);
        double damage = (_damage * source.GetProficiencyMultiplier(ItemProficiency).Value) + source.Magic - 10;

        var result = mainTarget.TakeDamage(source, damage, Element);
        string resultMessage = $"{source.Name} reads from the {Name}. The air itself seems to flex around {mainTarget.Name} and they take {result.AmountActual} damage.";

        source.Skills.Add(new Shatter());
        resultMessage += $"\n{source.Name} learns a new spell: Shatter";

        var item = source.Inventory.Items.FirstOrDefault(i => i.Id.Equals(Id, StringComparison.InvariantCultureIgnoreCase));
        if (item is null)
        {
            source.Inventory.Items.Clear();
            resultMessage += $"\nThe scroll seems to have not existed in the first place; {source.Name} feels their inventory shrivel and fade into nothingness...";
        }
        else
        {
            source.Inventory.Items.Remove(item);
            resultMessage += $"\nThe scroll flares up in a burst of fire and disappears.";
        }

        return new(
            message: resultMessage,
            results: [result],
            wasMagic: true
        );
    }
}

/// <summary>
/// A gem that increases the MaxHealth and CurrentHealth of a target by 7, then removes itself from the source's inventory.
/// </summary>
/// <param cref="tag">Tag: "gem-life"</param>
public sealed class LifeGem : Useable
{
    private readonly int _increase = 7;
    public LifeGem(): base(
        name: "Gem of Vitality",
        tag: "gem-life",
        cost: 0,
        description: "",
        consumable: true,
        sellable: false,
        element: DamageType.healing,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.potions,
        itemType: ActionType.Buff,
        shopType: (int)ShopTypes.Magic,
        rarity: (int)Rarities.rare,
        collection: (int)ShopCollections.Artifact
    )
    {
        Description = "A gem that glitters and seems to shine with a curious light. Its shine seems to fill you with vitality." +
            $"\nConsuming this gem increases your max health by {_increase}";
    }

    public override Item Clone()
    {
        return new LifeGem();
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        mainTarget.MaxHealth += _increase;
        mainTarget.CurrentHealth += _increase;

        source.Inventory.Items.Remove(this);

        return new(
            message: $"{source.Name} uses the {Name} on {mainTarget.Name}, increasing their max health by {_increase}",
            results: [
                new(
                    sourceId: source.ID,
                    targetId: mainTarget.ID,
                    actionType: (int)ItemType,
                    sent: _increase,
                    actual: _increase,
                    result: mainTarget.MaxHealth,
                    fatal: false
                )
            ],
            wasMagic: false
        );
    }
}

/// <summary>
/// A gem that restores a target to full health, then removes itself from the source's inventory.
/// </summary>
/// <param cref="tag">Tag: "gem-healing"</param>
public sealed class HealingGem : Useable
{
    public HealingGem(): base(
        name: "Gem of Health",
        tag: "gem-healing",
        cost: 0,
        description: "A gem that glitters and seems to shine with a curious light. Its shine seems to fill you with vigor.\n" + 
            "Consuming this gem heals you to full health.",
        consumable: true,
        sellable: false,
        element: DamageType.healing,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.potions,
        itemType: ActionType.Buff,
        shopType: (int)ShopTypes.Magic,
        rarity: (int)Rarities.rare,
        collection: (int)ShopCollections.Artifact
    ) { }
    public override Item Clone()
    {
        return new HealingGem();
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        double healingSent = mainTarget.MaxHealth - mainTarget.CurrentHealth;
        mainTarget.CurrentHealth = mainTarget.MaxHealth;

        source.Inventory.Items.Remove(this);

        return new(
            message: $"{source.Name} uses the {Name} on {mainTarget.Name}, restoring them to full health.",
            results: [
                new(
                    sourceId: source.ID,
                    targetId: mainTarget.ID,
                    actionType: (int)ItemType,
                    sent: healingSent,
                    actual: healingSent,
                    result: mainTarget.CurrentHealth,
                    fatal: false
                )
            ],
            wasMagic: false
        );
    }
}

/// <summary>
/// A gem that increases a target's strength and defense by 3, then removes itself from the source's inventory.
/// </summary>
/// <param cref="tag">Tag: "gem-fortify"</param>
public sealed class FortifyGem : Useable
{
    public FortifyGem(): base(
        name: "Gem of Fortification",
        tag: "gem-fortify",
        cost: 0,
        description: "A gem that glitters and seems to shine with a curious light. Its shine seems to fill you with strength.\n" + 
            "Consuming this gem increases your strength and defense by 3",
        consumable: true,
        sellable: false,
        element: DamageType.enchanting,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.potions,
        itemType: ActionType.Buff,
        shopType: (int)ShopTypes.Magic,
        rarity: (int)Rarities.rare,
        collection: (int)ShopCollections.Artifact
    ) { }
    public override Item Clone()
    {
        return new FortifyGem();
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        mainTarget.Strength += 3;
        mainTarget.Defense += 3;

        source.Inventory.Items.Remove(this);

        return new(
            message: $"{source.Name} uses the {Name} on {mainTarget.Name}, restoring them to full health.",
            results: [
                new(
                    sourceId: source.ID,
                    targetId: mainTarget.ID,
                    actionType: (int)ItemType,
                    sent: 3,
                    actual: 3,
                    result: mainTarget.Strength,
                    fatal: false
                ),
                new(
                    sourceId: source.ID,
                    targetId: mainTarget.ID,
                    actionType: (int)ItemType,
                    sent: 3,
                    actual: 3,
                    result: mainTarget.Defense,
                    fatal: false
                )
            ],
            wasMagic: false
        );
    }
}

/// <summary>
/// A gem that increases the target's magic by 3 and the target's MaxMana by 15
/// </summary>
/// <param cref="tag">Tag: "gem-mana"</param>
public sealed class ManaGem : Useable
{
    public ManaGem(): base(
        name: "Gem of Magic",
        tag: "gem-mana",
        cost: 0,
        description: "A gem that glitters and seems to shine with a curious light. Its shine seems to fill you with strength.\n" + 
            "Consuming this gem increases your Magic by 3 and max mana by 15",
        consumable: true,
        sellable: false,
        element: DamageType.enchanting,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.potions,
        itemType: ActionType.Buff,
        shopType: (int)ShopTypes.Magic,
        rarity: (int)Rarities.rare,
        collection: (int)ShopCollections.Artifact
    ) { }
    public override Item Clone()
    {
        return new ManaGem();
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        mainTarget.Magic += 3;
        mainTarget.MaxMana += 15;

        source.Inventory.Items.Remove(this);

        return new(
            message: $"{source.Name} uses the {Name} on {mainTarget.Name}, increasing their Magic power by 3 and raising their mana capacity by 15.",
            results: [
                new(
                    sourceId: source.ID,
                    targetId: mainTarget.ID,
                    actionType: (int)ItemType,
                    sent: 3,
                    actual: 3,
                    result: mainTarget.Magic,
                    fatal: false
                ),
                new(
                    sourceId: source.ID,
                    targetId: mainTarget.ID,
                    actionType: (int)ItemType,
                    sent: 15,
                    actual: 15,
                    result: mainTarget.MaxMana,
                    fatal: false
                )
            ],
            wasMagic: false
        );
    }
}

/// <summary>
/// A giant club that deals 30 base damage. Requires 15 strength to use.
/// </summary>
/// <param cref="tag">Tag: "club-giant"</param>
public sealed class GiantClub: Useable
{
    private readonly double _damage = 30;
    public GiantClub(): base(
        name: "Giant's Club",
        tag: "club-giant",
        cost: 12,
        description: "A truly giant club, dealing strikes of giant damage.",
        consumable: false,
        sellable: true,
        element: DamageType.crushing,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.bludgeoning,
        itemType: ActionType.Attack,
        shopType: (int)ShopTypes.Equipment,
        rarity: (int)Rarities.uncommon,
        collection: (int)ShopCollections.Weapon
    ) { }

    public override Item Clone()
    {
        return new GiantClub();
    }

    public override bool CanUse(DamageableEntity target)
    {
        return target.Strength > 15;
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        if (!CanUse(source))
        {
            return new(
                message: $"{source.Name} is not strong enough to wield the {Name}!",
                results: [],
                wasMagic: false
            );
        }
        source.AddProficiencyEntry(ItemProficiency);
        double buffed = (_damage * source.GetProficiencyMultiplier(ItemProficiency).Value) + source.Strength - 10;

        var effect = mainTarget.TakeDamage(source, buffed, Element);

        return new(
            message: $"{source.Name} strikes {mainTarget.Name} with the {Name}, dealing {effect.AmountActual} damage.",
            results: [effect],
            wasMagic: false
        );
    }
}

/// <summary>
/// A simple dagger. Deals 18 base damage
/// </summary>
/// <param cref="tag">Tag: "dagger"</param>
public sealed class Dagger : Useable
{
    private readonly double _damage = 18;
    public Dagger(): base(
        name: "Dagger",
        tag: "dagger",
        cost: 20,
        description: "A wickedly long knife" +
            "\nDeals 18 damage.",
        consumable: false,
        sellable: true,
        element: DamageType.slashing,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.slashing,
        itemType: ActionType.Attack,
        shopType: (int)ShopTypes.Equipment,
        rarity: (int)Rarities.super_common,
        collection: (int)ShopCollections.Weapon
    ) { }

    public override Item Clone()
    {
        return new Dagger();
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(ItemProficiency);
        double buffed = (_damage * source.GetProficiencyMultiplier(ItemProficiency).Value) + source.Strength - 10;

        var result = mainTarget.TakeDamage(source, buffed, Element);

        return new(
            message: $"{source.Name} slashes {mainTarget.Name} with their {Name}, dealing {result.AmountActual} damage.",
            results: [result],
            wasMagic: false
        );
    }
}

/// <summary>
/// A simple club, deals 18 base damage
/// </summary>
/// <param cref="tag">Tag: "club"</param>
public sealed class Club: Useable
{
    private readonly double _damage = 18;
    public Club(): base(
        name: "Club",
        tag: "club",
        cost: 18,
        description: "A simple club, almost just a stick of wood." +
            "\nDeals 18 damage.",
        consumable: false,
        sellable: true,
        element: DamageType.crushing,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.hand,
        itemType: ActionType.Attack,
        shopType: (int)ShopTypes.Equipment,
        rarity: (int)Rarities.common,
        collection: (int)ShopCollections.Weapon
    ) { }

    public override Item Clone()
    {
        return new Club();
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(ItemProficiency);

        double buffed = (_damage * source.GetProficiencyMultiplier(ItemProficiency).Value) + source.Strength - 10;
        var result = mainTarget.TakeDamage(source, buffed, Element);
        
        return new(
            message: $"{source.Name} strikes {mainTarget.Name} with their {Name}, dealing {result.AmountActual} damage.",
            results: [result],
            wasMagic: false
        );
    }
}

/// <summary>
/// A key item, unlocks the warrior's gate in the test scene.
/// </summary>
/// <param cref="tag">Tag: "warriors-gate-key"</param>
public sealed class WarriorsGateKey: Useable
{
    public WarriorsGateKey(): base(
        name: "Key to the Warrior's Gate",
        tag: "warriors-gate-key",
        cost: 0,
        description: "A key, once held by a hermit as a prank t oa great warrior. Opens the gate to the warrior's abode.",
        consumable: false,
        sellable: false,
        element: DamageType.damage,
        multiTarget: false,
        targetsLimit: 1,
        proficiency: Proficiency.stealth,
        itemType: ActionType.Other,
        shopType: (int)ShopTypes.error,
        rarity: (int)Rarities.impossible,
        collection: (int)ShopCollections.Artifact
    ) { }

    public override bool CanUse(DamageableEntity target)
    {
        return false;
    }

    public override Item Clone()
    {
        return new WarriorsGateKey();
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(ItemProficiency, -1);
        double buffed = (10 * source.GetProficiencyMultiplier(ItemProficiency).Value) + source.Strength - 10;
        var result = mainTarget.TakeDamage(source, buffed, Element);
        return new(
            message: $"{source.Name} strikes {mainTarget.Name} with {Name}, dealing {result.AmountActual} damage... Why they chose to attack with a key is beyond me.\n" +
            "HOW they attacked with the key is even farther beyond me!",
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