namespace GameServer.Domain.Items;

public interface IItemsIndex
{
    public Item GetItemByTag(string tag);
    public List<Item> GetShopItems(int intemsCount, int shopType, int rarity, int collection);
}