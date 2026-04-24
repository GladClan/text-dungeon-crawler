using System.ComponentModel.DataAnnotations;

namespace GameServer.Contracts.Requests;


public sealed class AddItemByIdRequest
{
    /// <summary>
    /// Use a catalog id from your item registry; resolve it to a domain Item in your service layer.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string ItemId { get; init; } = string.Empty;
}

public sealed class RemoveItemByNameRequest
{
    [Required]
    [MinLength(1)]
    public string ItemName { get; init; } = string.Empty;
}

public sealed class RemoveItemByIndexRequest
{
    [Range(0, int.MaxValue)]
    public int Index { get; init; }
}