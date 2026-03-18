namespace GameServer.Contracts;

public sealed class EntityInventoryDto
{
    public int Gold { get; init; }
    public List<ItemDto> Items { get; init; } = new();
}