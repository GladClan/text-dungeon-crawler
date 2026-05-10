using GameServer.Contracts.DTOs;
using GameServer.Domain.Entities;

namespace GameServer.Domain.Items;

public abstract class Equippable : Item
{
    public int ArmorTypeLimit { get; init; }
    public bool Equipped { get; protected set; }
    
    public abstract EffectDto OnEquip(DamageableEntity target);
    public abstract EffectDto OnUnequip(DamageableEntity target);

    public Equippable()
    {
    }

    public Equippable(string type, string name, int cost, string description, bool consumable, bool sellable, int armorTypeLimit, bool equipped)
     : base(type, name, cost, description, consumable, sellable)
    {
        ArmorTypeLimit = armorTypeLimit;
        Equipped = equipped;
    }
}