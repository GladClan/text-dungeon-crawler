namespace GameServer.Domain.Enums;

public enum Proficiency
{
    // Parent categories
    combat,
    melee_weapons,
    ranged_weapons,
    spellcasting,
    spellstrike,

    // Specific
    hand,
    slashing,
    bludgeoning,
    piercing,
    potions,
    poison,
    bow,
    healing,
    enchanting,
    stealth,
    nobility,
    destiny
}

/*
spellcasting
├── spellstrike
└── healing

combat
├── melee_weapons
│   ├── slashing
│   ├── bludgeoning
│   └── piercing
├── hand
└── ranged_weapons
    └── bow
*/

public static class ProficienciesHierarchies
{
    public static Proficiency? GetParentProficiency(Proficiency p)
    {
        return p switch
        {
            Proficiency.spellstrike => Proficiency.spellcasting,
            Proficiency.healing => Proficiency.spellcasting,
            Proficiency.enchanting => Proficiency.spellcasting,

            Proficiency.melee_weapons => Proficiency.combat,
            Proficiency.ranged_weapons => Proficiency.combat,
            Proficiency.hand => Proficiency.combat,

            Proficiency.slashing => Proficiency.melee_weapons,
            Proficiency.bludgeoning => Proficiency.melee_weapons,
            Proficiency.piercing => Proficiency.melee_weapons,

            Proficiency.bow => Proficiency.ranged_weapons,

            Proficiency.poison => Proficiency.potions,

            _ => null
        };
    }
}