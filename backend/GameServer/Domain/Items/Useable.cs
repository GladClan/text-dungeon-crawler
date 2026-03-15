using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Items;

abstract class Useable(string type, string name, int cost, string description, bool sellable, DamageType element, Proficiency proficiency) : Item(type, name, cost, description, sellable)
{
    public DamageType Element { get; protected set; } = element;
    public Proficiency Prof { get; protected set; } = proficiency;
    public abstract string ItemEffect(Entity target, Entity source);
    public bool CanUse(Entity target)
    {
        return true;
    }
}