namespace GameServer.Domain.Entity;

public sealed class EntityInventory(Item[] items, int gold)
{
    public int Gold { get; set; } = gold;
    public List<Item> Items { get; private set; } = [.. items];

    public EntityInventory AddItem(Item item)
    {
        Items.Add(item);
        return this;
    }

    public Item RemoveItemByName(string name)
    {
        Item item = Items.Find(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        Items.Remove(item);
        return item is null ? throw new Exception(KeyNotFoundException($"Item '{name}' was not found.")) : item;
    }

    public Item RemoveItemByIndex(int index)
    {
        Item item;
        index > items.Length ? throw new IndexOutOfRangeException($"Index {index} is out of bounds for length {items.Length}") : item = Items[index];
        Items.RemoveAt(index);
        return item;
    }

    public Item getItemAtIndex(int index)
    {
        Items item;
        index > items.Length ? throw new IndexOutOfRangeException($"Index {index} is out of bounds for length {items.Length}") : item = Items[index];
        return item;
    }

    public bool HasItem(string itemName)
    {
        return Items.All(i => i.Name == itemName);
    }

    public void Clear()
    {
        Items.Clear();
    }

    public int GetItemCount()
    {
        return items.Length;
    }

    public void Merge(EntityInventory other)
    {
        other.Items.CopyTo(Items);
    }

    public EntityInventory Clone()
    {
        return new EntityInventory
        {
            Gold = Gold,
            Items = [.. Items]
        };
    }
}