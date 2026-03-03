namespace GameServer.Domain.Entity;

public sealed class EntityInventory
{
    public int Gold { get; private set; }
    // public List<Item> Items { get; private set; } = new();

    public EntityInventory Clone()
    {
        return new EntityInventory
        {
            Gold = Gold,
            // Items = new List<Item>(Items)
        };
    }
}