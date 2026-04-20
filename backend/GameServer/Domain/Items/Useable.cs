using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Items;

abstract class Useable : Item
{
    public DamageType Element { get; protected set; }
    public Proficiency Prof { get; protected set; }
    public abstract string ItemEffect(DamageableEntity target, DamageableEntity source);
    public bool CanUse(DamageableEntity target)
    {
        return true;
    }

    public Useable()
    {
    }

    public Useable(string type, string name, int cost, string description, bool consumable, bool sellable, DamageType element, Proficiency proficiency)
     : base(type, name, cost, description, consumable, sellable)
    {
        Element = element;
        Prof = proficiency;
    }
}