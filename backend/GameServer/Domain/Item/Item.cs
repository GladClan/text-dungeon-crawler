namespace GameServer.Domain;

public class Item
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Name { get; private set; }
    public int Value { get; private set; }
    public string Description { get; private set; }
    public bool Consumable { get; private set; }
}