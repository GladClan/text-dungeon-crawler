namespace GameServer.Domain.Enums;

public enum DamageType
{
    // Physical DamageTypes
    damage,
    slashing,
    piercing,
    crushing,

    // Magical DamageTypes
    spellstrike,
    physical,       // For spells that deal non-elemental (physical) magical damage
    poisoning,      // Can be magical damage... or not
    healing,        // Most likely magical DamageType
    radiant,
    necro,
    shining,
    darkling,
    burning,
    freezing,
    shocking,
    aerial,
    terra,          // Technically you could count throwing a rock at an enemy as terra damage, but I don't. That's bludgeoning, my guy :)
    soaking,        // Again, technically you could hold someone under a bucket of water and count that as soaking, but not I. That's lame.
    enchanting,     // For skills like Spellshield
}

public static class DamageTypeHierarchies
{
    public static bool IsPhysicalDamage(DamageType d)
    {
        return (
            d == DamageType.crushing ||
            d == DamageType.slashing ||
            d == DamageType.piercing
        );
    }

    public static bool IsMagicDamage(DamageType d)
    {
        return (
            d != DamageType.damage &&
            !(
                IsPhysicalDamage(d)
            )
        );
    }
}