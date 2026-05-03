using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Api.Controllers;

[ApiController]
[Route("api/inventory/{id}")]
public sealed class InventoryController(InventoryService inventoryService) : ControllerBase
{
    private readonly InventoryService _service = inventoryService;

    private static string IdNotFound(string id)
    {
        return $"Entity not found: {id}";
    }

    [HttpGet("all")]
    public ActionResult<EntityInventoryDto> GetInventory(string id)
    {
        var inventory = _service.GetInventory(id);
        if (inventory is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(inventory);
    }

    [HttpGet("gold")]
    public ActionResult<int> GetGold(string id)
    {
        var gold = _service.GetGold(id);
        if (gold is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(gold);
    }

    [HttpGet("all-items")]
    public ActionResult<List<ItemDto>> GetAllItems(string id)
    {
        var items = _service.GetAllItems(id);
        if (items is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(items);
    }

    [HttpGet("item-id/{itemId}")]
    public ActionResult<ItemDto> GetItemById(string id, string itemId)
    {
        var item = _service.GetItemById(id, itemId);
        if (item is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (item.Name.Length == 0)
        {
            return NotFound($"Item {itemId} could not be found. Please check the item ID and try again.");
        }
        return Ok(item);
    }

    [HttpGet("item-name/{itemName}")]
    public ActionResult<ItemDto> GetItemByName(string id, string itemName)
    {
        var item = _service.GetItemById(id, itemName);
        if (item is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (item.Name.Length == 0)
        {
            return NotFound($"Item {itemName} could not be found. Please check the item name and try again.");
        }
        return Ok(item);
    }

    [HttpGet("item-tag/{itemTag}")]
    public ActionResult<ItemDto> GetItemByTag(string id, string tag)
    {
        var item = _service.GetItemById(id, tag);
        if (item is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (item.Name.Length == 0)
        {
            return NotFound($"Item {tag} could not be found. Please check the item tag and try again.");
        }
        return Ok(item);
    }

    [HttpGet("items-count")]
    public ActionResult<int> GetItemCount(string id)
    {
        var count = _service.GetItemCount(id);
        if (count is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(count);
    }

    [HttpPost("add-item-by-id/{itemId}")]
    public ActionResult<ItemDto> AddItemById(string id, string itemId)
    {
        var item = _service.AddItemById(id, itemId);
        if (item is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (item.Name.Length == 0)
        {
            return NotFound($"Item {itemId} could not be found. Please check the item ID and try again.");
        }
        return CreatedAtAction(nameof(_service.GetItemById), new { id = item.Id }, item);
    }

    [HttpPost("add-item-by-tag/{tag}")]
    public ActionResult<ItemDto> AddItemByTag(string id, string tag)
    {
        var item = _service.AddItemByTag(id, tag);
        if (item is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (item.Name.Length == 0)
        {
            return NotFound($"Item {tag} could not be found. Please check the item tag and try again.");
        }
        return CreatedAtAction(nameof(_service.GetItemByTag), new { id = item.Id }, item);
    }

    [HttpPatch("add-gold")]
    public ActionResult<int> AddGold(string id, [FromBody] int amount)
    {
        var gold = _service.AddGold(id, amount);
        if (gold is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(gold);
    }

    [HttpPatch("set-gold")]
    public ActionResult<int> SetGold(string id, [FromBody] int amount)
    {
        var gold = _service.SetGold(id, amount);
        if (gold is null)
        {
            return NotFound(IdNotFound(id));
        }
        return Ok(gold);
    }

    [HttpPut("absorb-inventory")]
    public ActionResult<EntityInventoryDto> AbsorbInventory(string id, [FromBody] string otherId)
    {
        var inventory = _service.AbsorbInventory(id, otherId);
        if (inventory is null)
        {
            return NotFound(IdNotFound($"{id} or {otherId}"));
        }
        return Ok(inventory);
    }

    [HttpDelete("remove-item-id")]
    public ActionResult<ItemDto> RemoveItemById(string id, [FromBody] string itemId)
    {
        var item = _service.RemoveItemById(id, itemId);
        if (item is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (item.Name.Length == 0)
        {
            return NotFound($"Item {itemId} could not be found. Please check the item ID and try again.");
        }
        return Ok(item);
    }

    [HttpDelete("clear")]
    public ActionResult ClearInventory(string id)
    {
        var success = _service.ClearInventory(id);
        if (success is null)
        {
            return NotFound(IdNotFound(id));
        }
        return NoContent();
    }
}