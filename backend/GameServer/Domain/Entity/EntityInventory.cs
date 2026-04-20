using GameServer.Domain.Items;

namespace GameServer.Domain.Entities;

public sealed class EntityInventory(Item[] items, int gold)
{
    public EntityInventory() : this([], 0) { }

    public int Gold { get; set; } = gold;
    public List<Item> Items { get; set; } = [.. items ?? []];

    public EntityInventory AddItem(Item item)
    {
        Items.Add(item);
        return this;
    }

    public Item RemoveItemByName(string name)
    {
        Item? item = Items.Find(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? throw new KeyNotFoundException($"Item '{name}' was not found.");
        Items.Remove(item);
        return item;
    }

    public Item RemoveItemByIndex(int index)
    {
        if (index >= Items.Count)
        {
            throw new IndexOutOfRangeException($"Index {index} is out of bounds for length {Items.Count}.");
        }
        Item item = Items[index];
        Items.RemoveAt(index);
        return item;
    }

    public Item GetItemAtIndex(int index)
    {
        if (index >= Items.Count)
        {
            throw new IndexOutOfRangeException($"Index {index} is out of bounds for length {Items.Count}.");
        }
        return Items[index];
    }

    public bool HasItem(string itemName)
    {
        return Items.Any(i => i.Name == itemName);
    }

    public void Clear()
    {
        Items.Clear();
    }

    public int GetItemCount()
    {
        return Items.Count;
    }

    public void Merge(EntityInventory other)
    {
        Items.AddRange(other.Items);
    }

    public EntityInventory Clone()
    {
        return new EntityInventory([.. Items], Gold);
    }
}