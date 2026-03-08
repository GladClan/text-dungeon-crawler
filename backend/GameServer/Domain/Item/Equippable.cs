using System.ComponentModel;

namespace GameServer.Domain.Item;

class Equippable(int ArmorTypeLimit, bool Equipped) : Item
{
    public int ArmorTypeLimit { get; private set; }
    public bool Equipped { get; private set; }
    // private onEquip: equipType;
    // private onUnequip: equipType;
}
