using GameServer.Domain.Items;

namespace GameServer.Domain.Entities;

public sealed class EntityInventory(List<Item> items, int gold)
{
    public EntityInventory() : this([], 0) { }

    public int Gold { get; set; } = gold;
    public List<Item> Items { get; set; } = [.. items ?? []];

    public EntityInventory AddItem(Item item)
    {
        Items.Add(item);
        return this;
    }

    public bool HasItem(string itemName)
    {
        return Items.Any(i => i.Name == itemName);
    }
}