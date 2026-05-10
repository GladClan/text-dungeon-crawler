using GameServer.Contracts.DTOs;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Items;

public abstract class Useable : Item
{
    public DamageType Element { get; protected set; }
    public Proficiency Prof { get; protected set; }
    public abstract EffectDto ItemEffect(DamageableEntity target, DamageableEntity source);
    public virtual bool CanUse(DamageableEntity target)
    {
        return true;
    }

    public Useable()
    {
    }

    public Useable(string type, string name, int cost, string description, bool consumable, bool sellable, DamageType element, Proficiency proficiency, string shopType, int rarity, int collection)
     : base(type, name, cost, description, consumable, sellable, shopType, rarity, collection)
    {
        Element = element;
        Prof = proficiency;
    }
}