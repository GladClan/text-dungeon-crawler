using GameServer.Contracts.DTOs;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;
using GameServer.Domain.Skills.SkillsLibrary.InitialRelease;

namespace GameServer.Domain.Items.ItemsLibrary.InitialRelease;

/// <summary>
/// A chestplate made from many small plates of steel. The smallness of the many plates it is made from makes it very flexible and versatile.
/// </summary>
/// <remarks>
/// + 7 defense
/// </remarks>
public sealed class RazeChestplate : Equippable
{
    private static readonly int _defense = 7;
    public RazeChestplate(): base(
        name: "Raze Chestplate",
        tag: "raze-chestplate",
        cost: 50,
        description: "A chestplate made from many small plates of steel. The smallness of the many plates it is made from makes it very flexible and versatile.\n +7 defense",
        consumable: false,
        sellable: true,
        armorType: ArmorTypes.Chestplates,
        armorTypeLimit: 1,
        equipped: false,
        shopType: (int)ShopTypes.Equipment,
        rarity: (int)Rarities.common,
        collection: (int)ShopCollections.Armor
    ) { }

    public override Item Clone()
    {
        return new RazeChestplate();
    }

    public override EffectDto OnEquip(DamageableEntity target)
    {
        if (!CanEquip(target))
        {
            return new(
                error: $"{target.Name} cannot equip {Name}."
            );
        }
        target.Inventory.EquippedArmorTypes[EquippableArmorType] = target.Inventory.EquippedArmorTypes.TryGetValue(EquippableArmorType, out var amount) ? amount + 1 : 1;
        Equipped = true;
        target.Defense += _defense;
        return new(
            message: $"{target.Name} equips {Name}, increasing their defense from {target.Defense - _defense} to {target.Defense}.",
            results: [new(
                sourceId: target.ID,
                targetId: target.ID,
                actionType: (int)ActionType.Other,
                sent: _defense,
                actual: _defense,
                result: target.Defense,
                fatal: false
            )],
            wasMagic: false
        );
    }

    public override EffectDto OnUnequip(DamageableEntity target)
    {
        if (!Equipped)
        {
            return new(
                error: $"{target.Name} does not have {Name} equipped!"
            );
        }
        target.Inventory.EquippedArmorTypes[EquippableArmorType]--;
        Equipped = false;
        target.Defense -= _defense;
        return new(
            message: $"{target.Name} unequips {Name}, decreasing their defense from {target.Defense + _defense} to {target.Defense}.",
            results: [new(
                sourceId: target.ID,
                targetId: target.ID,
                actionType: (int)ActionType.Other,
                sent: -_defense,
                actual: -_defense,
                result: target.Defense,
                fatal: false
            )],
            wasMagic: false
        );
    }
}

/// <summary cref="SummonSpiders">
/// Increases spellcasting proficiency and learns target the SummonSpiders skill
/// </summary>
/// <remarks>
/// + 0.3 to Proficiency.spellcasting
/// CanEquip override to add a minimum Magic requirement
/// </remarks>
public sealed class SpiderRing : Equippable
{
    private static readonly double _proficiencyIncrease = 0.3d;
    private string _skillId = "";
    private static readonly Proficiency _targetProficiency = Proficiency.spellcasting;
    public SpiderRing(): base(
        name: "Ring of Spiders",
        tag: "fire-ring",
        cost: 120,
        description: $"Increases spellcasting proficiency and gives knowledge of the {nameof(SummonSpiders)} spell",
        consumable: false,
        sellable: true,
        armorType: ArmorTypes.Rings,
        armorTypeLimit: 10,
        equipped: false,
        shopType: (int)ShopTypes.Equipment,
        rarity: (int)Rarities.rare,
        collection: (int)ShopCollections.Jewelers
    ) { }

    public override bool CanEquip(DamageableEntity target)
    {
        return base.CanEquip(target) && target.Magic > 10;
    }
    public override Item Clone()
    {
        return new SpiderRing();
    }

    public override EffectDto OnEquip(DamageableEntity target)
    {
        if (!CanEquip(target))
        {
            return new(
                error: $"{target.Name} cannot equip {Name}."
            );
        }
        target.Inventory.EquippedArmorTypes[EquippableArmorType] = target.Inventory.EquippedArmorTypes.TryGetValue(EquippableArmorType, out var amount) ? amount + 1 : 1;
        Equipped = true;
        target.Proficiencies.TryGetValue(_targetProficiency, out double value);
        target.Proficiencies[_targetProficiency] = value + _proficiencyIncrease;
        var newSkill = new SummonSpiders();
        _skillId = newSkill.Id;
        target.Skills.Add(newSkill);
        return new(
            message: $"{target.Name}'s spellcasting proficiency increased to {target.GetStoredProficiency(_targetProficiency)}, and learned the spell {nameof(SummonSpiders)}",
            results: [new(
                sourceId: target.ID,
                targetId: target.ID,
                actionType: (int)ActionType.Buff,
                sent: _proficiencyIncrease,
                actual: _proficiencyIncrease,
                result: target.GetStoredProficiency(_targetProficiency).Value,
                fatal: false
            )],
            wasMagic: false
        );
    }

    public override EffectDto OnUnequip(DamageableEntity target)
    {
        if (!Equipped)
        {
            return new(
                error: $"{target.Name} does not have {Name} equipped!"
            );
        }
        target.Inventory.EquippedArmorTypes[EquippableArmorType]--;
        Equipped = false;

        target.Proficiencies[_targetProficiency] -= _proficiencyIncrease;
        int index = target.Skills.FindIndex(s => s.Id.Equals(_skillId, StringComparison.InvariantCultureIgnoreCase));
        if (index != -1)
        {
            target.Skills.RemoveAt(index);
        }
        return new(
            message: $"{target.Name}'s spellcasting proficiency decreased to {target.GetStoredProficiency(_targetProficiency)} and lost the spell {nameof(SummonSpiders)}",
            results: [new(
                sourceId: target.ID,
                targetId: target.ID,
                actionType: (int)ActionType.Debuff,
                sent: -_proficiencyIncrease,
                actual: -_proficiencyIncrease,
                result: target.GetStoredProficiency(_targetProficiency).Value,
                fatal: false
            )],
            wasMagic: false
        );
    }
}

/*
// Equippable constructor template
    public equippable_name(): base(
        name: "",
        tag: "",
        cost: 0,
        description: "Description",
        consumable: false,
        sellable: true,
        armorType: ArmorTypes.Chestplate,
        armorTypeLimit: 1,
        equipped: false,
        shopType: (int)ShopTypes.Equipment,
        rarity: (int)Rarities.common,
        collection: (int)ShopCollections.Armor
    ) { }

// Things involved in every OnEquip :)
    if (!CanEquip(target))
    {
        return new(
            error: $"{target.Name} cannot equip {Name}."
        );
    }
    target.Inventory.EquippedArmorTypes[EquippableArmorType] = 
        target.Inventory.EquippedArmorTypes.TryGetValue(EquippableArmorType, out var amount) ?
            amount + 1 : 1;
    Equipped = true;

// And on Unequip:
    if (!Equipped)
    {
        return new(
            error: $"{target.Name} does not have {Name} equipped!"
        );
    }
    target.Inventory.EquippedArmorTypes[EquippableArmorType]--;
    Equipped = false;
*/