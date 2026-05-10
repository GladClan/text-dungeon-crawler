namespace GameServer.Domain.Items;

public interface IItemsIndex
{
    public Item GetItemById(string Id);
    public Item GetItemByTag(string tag);
    public List<Item> GetShopItems(int intemsCount, string shopType, int rarity, int collection);
}