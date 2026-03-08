namespace GameServer.Domain.Item;

public class Item(string type, string name, int value, string description, bool consumable)
{
    public Guid Id { get; } = id;
    public string Type { get; private set; } = type;
    public string Name { get; private set; } = name;
    public int Value { get; private set; } = value;
    public string Description { get; private set; } = description;
    public bool Consumable { get; private set; } = consumable;
}