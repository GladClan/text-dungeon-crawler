using GameServer.Application.Common;

namespace GameServer.Domain.Items;

public abstract class Item
{
    public string Id { get; init; }         // A unique identifier for the item. Created upon item creation.
    public string Tag { get; init; }        // A tag that identifies the item for creation. All items created from the model have the same tag.
    public string Name { get; set; }        // User-facing name, can have multiple of the same item (i.e. "Health Potion")
    public int Value { get; set; }          // The 'sales price', if you will
    public string Description { get; set; } // A description of the item
    public bool Consumable { get; set; }    // True for equippables that break or items that have only so many uses
    public bool Sellable { get; set; }      // Can I sell it or is it priceless / worthless?
    public int ShopType { get; set; }       // Used to identify what kind of shop this would be sold in. May affect the selling price.
    public int Rarity { get; set; }         // How often will I see this item in shops or in the wild?
    public int Collection { get; set; }     // Used to determine when the item appears in shops.

    public Item(string name, string tag, int value, string description, bool consumable, bool sellable, int shopType, int rarity, int collection)
    {
        Id = NewId();
        Name = name;
        Tag = tag;
        Value = value;
        Description = description;
        Consumable = consumable;
        Sellable = sellable;
        ShopType = shopType;
        Rarity = rarity;
        Collection = collection;
    }

    public abstract Item Clone();

    private string NewId()
    {
        return $"{Tag}-{OrdinalDateString.GetOrdinalDate(3)}";
    }
}