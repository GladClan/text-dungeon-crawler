namespace GameServer.Domain.Items;

public interface IItemsIndex
{
    public Item GetItemById(string Id);
    public Item GetItemByTag(string tag);
}