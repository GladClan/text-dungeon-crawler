using GameServer.Domain.Items.ItemsLibrary.InitialRelease;

namespace GameServer.Domain.Items.ItemsLibrary;

public class ItemsIndexInitialRelease : IItemsIndex
{
    public Item GetItemById(string Id)
    {
        return Id switch
        {
            _ => new ErrorItem(),
        };
    }

    public Item GetItemByTag(string tag)
    {
        return tag switch
        {
            _ => new ErrorItem(),
        };
    }
}