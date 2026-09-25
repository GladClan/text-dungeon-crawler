namespace GameServer.Contracts.DTOs;

public sealed class ItemDto
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string Tag { get; init; }
    public int Value { get; init; }
    public string Description { get; init; }
    public bool Consumable { get; init; }
    public bool Sellable { get; init; }
    public bool CanUse { get; init; }
    public string? Element { get; init; }
    public string? Proficiency { get; init; }
    public string? ArmorType { get; init; }
    public int? ArmorTypeLimit { get; init; }
    public int? TargetsLimit { get; init; }
    public bool? Equipped { get; init; }
    public string Error { get; init; }
    
    public ItemDto(
        string id,
        string name,
        string tag,
        int value,
        string description,
        bool consummable,
        bool sellable,
        bool canUse,
        string? element = null,
        string? proficiency =null,
        string? armorType = null,
        int? armorTypeLimit = null,
        int? targetsLimit = null,
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
        CanUse = canUse;
        Element = element;
        Proficiency = proficiency;
        ArmorType = armorType;
        ArmorTypeLimit = armorTypeLimit;
        TargetsLimit = targetsLimit;
        Equipped = equipped;
        Error = string.Empty;
    }
    public ItemDto(
        string error
    )
    {
        Error = error;

        Id = string.Empty;
        Name = string.Empty;
        Tag = string.Empty;
        Description = string.Empty;
        Consumable = false;
        Sellable = false;
    }
}