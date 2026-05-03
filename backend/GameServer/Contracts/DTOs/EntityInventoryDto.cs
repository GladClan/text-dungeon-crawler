namespace GameServer.Contracts.DTOs;

public sealed class EntityInventoryDto
{
    public int Gold { get; init; }
    public List<ItemDto> Items { get; init; } = [];
}