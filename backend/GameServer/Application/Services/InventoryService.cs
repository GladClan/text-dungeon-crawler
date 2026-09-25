using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Domain.Items;
using GameServer.Infrastructure;

namespace GameServer.Application.Services;

public sealed class InventoryService(EntityStore entityStore, IItemsIndex itemIndex)
{
    private readonly EntityStore _entities = entityStore;
    private readonly IItemsIndex _itemsIndexer = itemIndex;

    public EntityInventoryDto? GetInventory(string id)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            return target.Inventory.ToDto(target);
        }
        return null;
    }

    public int? GetGold(string id)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            return target.Inventory.Gold;
        }
        return null;
    }

    public List<ItemDto>? GetAllItems(string id)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            return [..target.Inventory.Items.Select(i => i.ToDto(target))];
        }
        return null;
    }

    public ItemDto? GetItemById(string id, string itemId)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            var item = target.Inventory.Items.FirstOrDefault(i => i.Id.Equals(itemId, StringComparison.InvariantCultureIgnoreCase));
            if (item is not null)
            {
                return item.ToDto(target);
            }
            return new ItemDto(
                error: $"Item id {itemId} could not be found"
            );
        }
        return null;
    }

    public ItemDto? GetItemByName(string id, string name)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            var item = target.Inventory.Items.FirstOrDefault(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (item is not null)
            {
                return item.ToDto(target);
            }
            return new ItemDto(
                error: $"Item with name {name} could not be found"
            );
        }
        return null;
    }

    public ItemDto? GetItemByTag(string taretEntityId, string itemTag)
    {
        if (_entities.TryGet(taretEntityId, out var target) && target is not null)
        {
            var item = target.Inventory.Items.FirstOrDefault(i => i.Tag.Equals(itemTag, StringComparison.InvariantCultureIgnoreCase));
            if (item is not null)
            {
                return item.ToDto(target);
            }
            return new ItemDto(
                error: $"Item with tag {itemTag} could not be found"
            );
        }
        return null;
    }

    public int? GetItemCount(string id)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            return target.Inventory.Items.Count;
        }
        return null;
    }

    public ItemDto? AddItemByTag(string id, string tag)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            var item = _itemsIndexer.GetItemByTag(tag);
            if (item.Tag.Equals("error"))
            {
                return new ItemDto(
                error: $"Item with name {tag} could not be found"
            );
            }
            target.Inventory.Items.Add(item);
            return item.ToDto(target);
        }
        return null;
    }

    public Item? NewItemByTag(string tag)
    {
        var item = _itemsIndexer.GetItemByTag(tag.Trim());
        if (item.Tag == "error")
        {
            return null;
        }
        return item;
    }


    /// <summary>
    /// Adds an amount of gold to the target's inventory
    /// </summary>
    /// <param name="id">The id for the DamageableEntity to add the gold to</param>
    /// <param name="amount">The amount of gold to add</param>
    /// <returns>The resulting amount of gold in the target's inventory.<br/>
    /// Null if the target entity cannot be found.<br/>
    /// Negative one if the amount would make the target's gold sum negative.
    /// </returns>
    public int? AddGold(string id, int amount)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            if (amount < 0 - target.Inventory.Gold)
            {
                return -1;
            }
            target.Inventory.Gold += amount;
            return target.Inventory.Gold;
        }
        return null;
    }

    public int? SetGold(string id, int amount)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            target.Inventory.Gold = amount;
            return target.Inventory.Gold;
        }
        return null;
    }

    public EntityInventoryDto? AbsorbInventory(string id, string otherId)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            if (_entities.TryGet(otherId, out var other) && other is not null)
            {
                target.Inventory.Items = [..target.Inventory.Items, ..other.Inventory.Items];
                other.Inventory.Items = [];
                target.Inventory.Gold += other.Inventory.Gold;
                other.Inventory.Gold = 0;
                return target.Inventory.ToDto(target);
            }
        }
        return null;
    }
    
    public ItemDto? RemoveItemById(string id, string itemId)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            var result = target.Inventory.Items.Find(i => i.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase));
            if (result == null)
            {
                return new ItemDto(
                    error: $"Item id \"{id}\" could not be found in the inventory of {target.Name} ({target.ID})"
                );
            }
            target.Inventory.Items.Remove(result);
            return result.ToDto(target);
        }
        return null;
        // Also an option, but a bad idea:
        // RemoveItemByTag
        // RemoveItemByName
    }

    public bool? ClearInventory(string id)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            target.Inventory.Items = [];
            target.Inventory.Gold = 0;
            return true;
        }
        return null;
    }
}

// hasItem
// setInventory
// clone
// isEmpty
// size