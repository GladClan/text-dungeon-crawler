namespace GameServer.Domain.Items;

public abstract class Item
{
    public int Id { get; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Consumable { get; set; }
    public bool Sellable { get; set; }

    public Item()
    {
    }
    public Item(string type, string name, int value, string description, bool consumable, bool sellable)
    {
        Type = type;
        Name = name;
        Value = value;
        Description = description;
        Consumable = consumable;
        Sellable = sellable;
    }
}