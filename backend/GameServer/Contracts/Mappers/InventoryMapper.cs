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

        return new ItemDto(
            id: item.Id,
            name: item.Name,
            tag: item.Tag,
            value: item.Value,
            description: item.Description,
            consummable: item.Consumable,
            sellable: item.Sellable,
            element: useable?.Element.ToString(),
            proficiency: useable?.ItemProficiency.ToString(),
            armorType: equippable?.EquippableArmorType.ToString(),
            armorTypeLimit: equippable?.ArmorTypeLimit,
            equipped: equippable?.Equipped,
            error: string.Empty
        );
    }
}