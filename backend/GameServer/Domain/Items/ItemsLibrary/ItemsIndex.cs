using GameServer.Domain.Items.ItemsLibrary.InitialRelease;
using System.Reflection;

namespace GameServer.Domain.Items.ItemsLibrary;

public class ItemsIndexInitialRelease : IItemsIndex
{
    private static readonly List<Item> ItemCatalog = InitializeItems();

    private static List<Item> InitializeItems()
    {
        var items = new List<Item>();
        var itemType = typeof(Item);
        var assembly = typeof(ItemsIndexInitialRelease).Assembly;

        // Find all concrete types that inherit from Item in the InitialRelease namespace
        var concreteItemTypes = assembly.GetTypes()
            .Where(t => 
                t.Namespace == "GameServer.Domain.Items.ItemsLibrary" &&
                !t.IsAbstract &&
                itemType.IsAssignableFrom(t));

        foreach (var type in concreteItemTypes)
        {
            try
            {
                if (Activator.CreateInstance(type) is Item item)
                {
                    items.Add(item);
                }
            }
            catch
            {
                // Skip items that can't be instantiated
            }
        }

        return items;
    }

    public Item GetItemById(string id)
    {
        return ItemCatalog.FirstOrDefault(i => i.Id == id) ?? new ErrorItem();
    }

    public Item GetItemByTag(string tag)
    {
        return ItemCatalog.FirstOrDefault(i => i.Tag == tag) ?? new ErrorItem();
    }

    public List<Item> GetShopItems(int itemsCount, string shopType, int rarity, int collection)
    {
        return [..
            ItemCatalog
                .Where(i => i.ShopType == shopType && i.Rarity <= rarity && i.Collection <= collection)
                .OrderBy(x => Random.Shared.Next())
                .Take(itemsCount)
            ];
    }
}