namespace GameServer.Contracts.DTOs;

public sealed class ItemDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Tag { get; init; } = string.Empty;
    public int Value { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool Consumable { get; init; }
    public bool Sellable { get; init; }
    public string? Element { get; init; }
    public string? Proficiency { get; init; }
    public string? ArmorType { get; init; }
    public int? ArmorTypeLimit { get; init; }
    public bool? Equipped { get; init; }
    public string Error { get; init; } = string.Empty;
    
    public ItemDto(
        string id,
        string name,
        string tag,
        int value,
        string description,
        bool consummable,
        bool sellable,
        string error,
        string? element = null,
        string? proficiency =null,
        string? armorType = null,
        int? armorTypeLimit = null,
        bool? equipped = null
    )
    {
        Id = id;
        Name = name;
        Tag = tag;
        Value = value;
        Description = description;
        Consumable = consummable;
        Sellable = sellable;
        Element = element;
        Proficiency = proficiency;
        ArmorType = armorType;
        ArmorTypeLimit = armorTypeLimit;
        Equipped = equipped;
        Error = error;
    }
}