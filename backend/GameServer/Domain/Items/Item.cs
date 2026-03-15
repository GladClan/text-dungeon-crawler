namespace GameServer.Domain.Items;

abstract class Item(string type, string name, int value, string description, bool consumable)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Type { get; protected set; } = type;
    public string Name { get; protected set; } = name;
    public int Value { get; protected set; } = value;
    public string Description { get; protected set; } = description;
    public bool Consumable { get; protected set; } = consumable;
}