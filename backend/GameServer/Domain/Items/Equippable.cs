using GameServer.Contracts.DTOs;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Items;

public abstract class Equippable : Item
{
    public ArmorTypes EquippableArmorType { get; set; }
    public int ArmorTypeLimit { get; init; }
    public bool Equipped { get; protected set; }
    
    public virtual bool CanEquip(DamageableEntity target)
    {
        return (
            !Equipped &&
            target.Inventory.EquippedArmorTypes.TryGetValue(EquippableArmorType, out int amount) &&
            amount >= ArmorTypeLimit
        );
    }

    public abstract EffectDto OnEquip(DamageableEntity target);
    public abstract EffectDto OnUnequip(DamageableEntity target);

    public Equippable(string name, string tag, int cost, string description, bool consumable, bool sellable, ArmorTypes armorType, int armorTypeLimit, bool equipped, int shopType, int rarity, int collection)
     : base(name, tag, cost, description, consumable, sellable, shopType, rarity, collection)
    {
        EquippableArmorType = armorType;
        ArmorTypeLimit = armorTypeLimit;
        Equipped = equipped;
    }
}