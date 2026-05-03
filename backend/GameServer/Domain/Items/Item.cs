using GameServer.Application.Common;

namespace GameServer.Domain.Items;

public abstract class Item
{
    public string Id { get; init; } = string.Empty; // A unique identifier for the item. Created upon item creation.
    public string Type { get; init; } = string.Empty;
    public string Tag { get; init; } = string.Empty; // A tag that identifies the item for creation. All items created from the model have the same tag.
    public string Name { get; set; } = string.Empty; // User-facing name, can have multiple of the same item (i.e. "Health Potion")
    public int Value { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Consumable { get; set; }
    public bool Sellable { get; set; }

    public Item()
    {
        Id = NewId();
    }
    public Item(string type, string name, int value, string description, bool consumable, bool sellable)
    {
        Id = NewId();
        Type = type;
        Name = name;
        Value = value;
        Description = description;
        Consumable = consumable;
        Sellable = sellable;
    }

    private string NewId()
    {
        return $"{Tag}-{OrdinalDateString.GetOrdinalDate(3)}";
    }
}