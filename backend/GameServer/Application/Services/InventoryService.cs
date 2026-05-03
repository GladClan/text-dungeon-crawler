using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Contracts.Requests;

namespace GameServer.Application.Services;

public sealed class InventoryService(EntityStore entityStore)
{
    private readonly EntityStore _entities = entityStore;

    public EntityInventoryDto? GetInventory(string id)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            return target.Inventory.ToDto();
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
            return [..target.Inventory.Items.Select(i => i.ToDto())];
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
                return item.ToDto();
            }
            return new ItemDto();
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
                return item.ToDto();
            }
            return new ItemDto();
        }
        return null;
    }

    public ItemDto? GetItemByTag(string id, string tag)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            var item = target.Inventory.Items.FirstOrDefault(i => i.Tag.Equals(tag, StringComparison.InvariantCultureIgnoreCase));
            if (item is not null)
            {
                return item.ToDto();
            }
            return new ItemDto();
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

    public ItemDto? AddItemById(string id, string itemId)
    {
        throw new NotImplementedException();
    }

    public ItemDto? AddItemByTag(string id, string tag)
    {
        throw new NotImplementedException();
    }

    public int? AddGold(string id, int amount)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
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
                return target.Inventory.ToDto();
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
                return new ItemDto();
            }
            target.Inventory.Items.Remove(result);
            return result.ToDto();
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
// clear
// setInventory
// toString
// clone
// isEmpty
// size