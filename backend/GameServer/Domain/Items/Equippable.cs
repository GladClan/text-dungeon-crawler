using GameServer.Domain.Entities;

namespace GameServer.Domain.Items;

abstract class Equippable(string type, string name, int cost, string description, bool sellable, int armorTypeLimit, bool equipped) : Item(type, name, cost, description, sellable)
{
    public int ArmorTypeLimit { get; protected set; } = armorTypeLimit;
    public bool Equipped { get; protected set; } = equipped;
    
    public abstract void OnEquip(Entity target);
    public abstract void OnUnequip(Entity target);
}
