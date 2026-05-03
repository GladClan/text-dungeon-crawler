using GameServer.Contracts.DTOs;
using GameServer.Domain.Entities;
using GameServer.Domain.Items;

namespace GameServer.Contracts.Mappers;

public static class InventoryMapper
{
    public static EntityInventoryDto ToDto(this EntityInventory inventory)
    {
        return new EntityInventoryDto
        {
            Gold = inventory.Gold,
            Items = [.. inventory.Items.Select(i => i.ToDto())]
        };
    }

    public static ItemDto ToDto(this Item item)
    {
        var useable = item as Useable;
        var equippable = item as Equippable;

        return new ItemDto
        {
            Id = item.Id,
            Type = item.Type,
            Tag = item.Tag,
            Name = item.Name,
            Value = item.Value,
            Description = item.Description,
            Consumable = item.Consumable,
            Element = useable?.Element.ToString(),
            Proficiency = useable?.Prof.ToString(),
            ArmorTypeLimit = equippable?.ArmorTypeLimit,
            Equipped = equippable?.Equipped
        };
    }
}