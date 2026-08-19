namespace GameServer.Domain.Enums;

public enum ShopCollections
{
    Artifact = 1,
    Jewelers = 3,
    Potion = 8,
    Armor = 15,
    Weapon = 17
}

public enum ShopTypes
{
    error = 1000,
    Equipment = 10,
    Siege_Weapons = 15
}

public enum Rarities
{
    super_common = 1,
    common = 5,
    uncommon = 15,
    rare = 50,
    legendary = 100,
    impossible = 1000
}