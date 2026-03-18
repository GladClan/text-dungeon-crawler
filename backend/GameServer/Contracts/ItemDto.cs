namespace GameServer.Contracts;

public sealed class ItemDto
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Value { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool Consumable { get; init; }

    public string? Element { get; init; }
    public string? Proficiency { get; init; }
    public int? ArmorTypeLimit { get; init; }
    public bool? Equipped { get; init; }
}