using GameServer.Domain.Enums;

namespace GameServer.Domain.Entities.BeastiaryEntity.BestiaryLibrary.BestiaryLibraryRepositoryReleases;

public static class InitialReleaseRepository
{
    public static readonly Dictionary<string, BeastiaryEntity> Repository = new()
    {
        {
            "error",
            new BeastiaryEntity(
                name: "m̴̪̟̒͆y̸̑͜s̸͚̜̘̲̋̋̀̕͜͜͝t̴̲̞͕̤͂͝e̵̺̤͂̑͋̀́̅r̷̟̳̼̪͛ỉ̵͙͎̟̮͎̐̊̒̈́̂ͅo̵̪͕͑̿̈̇̄ṳ̴̗̰̱̣̣͑s̸̩̠̩͐̔͝ ̴̢̒ç̴̠̏̂̇̇̾o̶̖̙͔͕͍͕̓r̶̗͆̽̍̂̓͝r̶̦̭̗̄̆̆͛̔͠u̶̬̗̮̺̪̫͂̀̋̚p̵̗̆̉t̷̤͉͍̞̫̪̂i̸̦͎̭̬̊̈́̐̅͝ͅõ̴̡̫̥͊n̶̡̡͖͈̍͂̾̔͠",
                entityType: "monster",
                race: "None",
                tag: "error",
                partyId: "error",
                health: 10000,
                mana: 10000,
                magic: 20,
                strength: 30,
                defense: 12,
                attackType: DamageType.damage,
                dealsMagicDamage: true,
                speed: 20,
                level: 1000,
                experience: 6000,
                resistances: new Dictionary<DamageType, double>
                {
                    {DamageType.slashing, 0.95d},
                    {DamageType.crushing, 0.95d},
                    {DamageType.piercing, 0.95d},
                    {DamageType.spellstrike, 0.25d},
                    {DamageType.radiant, -0.5d},
                    {DamageType.shining, -0.95d},
                    {DamageType.shocking, -2d}
                },
                proficiencies: new Dictionary<Proficiency, double>
                {
                    {Proficiency.hand, 1.5},
                    {Proficiency.spellstrike, 1.5},
                    {Proficiency.spellcasting, 1.75},
                    {Proficiency.destiny, 0.01}
                },
                defaultAttackMessageString: "{SourceName} seems to roil and bubble, blue streaks flashing across the cloud of black and grey\n" 
                    + "{TargetName} takes {AmountActual} damage from the mysterious entity.",
                deathMessage: "The mysterious entity seems to fade away, flashes of blue and yellow and white rippling across reality as it passes.\n"
                    + "After a couple of moments, the party's wounds are all that's left to testify of the entity's existence.",
                description: "A mysterious cloud of roiling, bubbling fuzz. Occasional yellow and white streaks are visible flashing through the cloud of black and grey. Something about it says that it really shouldn't be here.",
                bestiaryEntry: "An anomalous entity of roiling black fuzz and flashing static. It exhibits unstable patterns and shifts unpredictably.",
                journalEntry: "Went fishing the other day... I seem to have fished up a glitching cloud instead of a local beast. My instincts insist this thing simply does not belong in this world.",
                loreEntry1: "Scholars call it a tear in the fabric of reality. It manifests only when a requested soul fails to materialize. A physical placeholder filling a sudden void in nature",
                loreEntry2: "Looking directly at the flashing static causes severe vertigo. It does not eat, sleep, or breathe like a normal creature. It feels less like a living animal and more like a cosmic mistake",
                defaultAi: "",
                initialInventoryTags: ["error", "error"],
                initialSkillTags: ["error"]
            )
        },
        {
            "ogre-brute",
            new BeastiaryEntity(
                name: "Ogre Brute",
                entityType: "monster",
                race: "Ogre",
                tag: "ogre-brute",
                partyId: "",
                health: 260,
                mana: 0,
                magic: 0,
                strength: 20,
                defense: 16,
                attackType: DamageType.crushing,
                dealsMagicDamage: false,
                speed: 12,
                level: 1,
                experience: 50,
                resistances: new Dictionary<DamageType, double>(SpeciesDictionary.PlayableSpeciesStats[Species.Goblin].Resistances),
                proficiencies: new Dictionary<Proficiency, double>()
                {
                    {Proficiency.bludgeoning, 1.7},
                    {Proficiency.melee_weapons, 1.2},
                    {Proficiency.combat, 1},
                },
                defaultAttackMessageString: "{SourceName} strikes {TargetName} with a giant fist, dealing {AmountActual} damage",
                deathMessage: "",
                description: "A rather unintelligent beast known for its quick temper and massive stature. The brute strength alone of this beast is enough to send any normal fighter packing.",
                bestiaryEntry: "This should be further description of this ogre, talkign about the physical or tempermental attributes of the beast.",
                journalEntry: "Here is written a little blurb. Perhaps informational, perhaps more jollitous.",
                loreEntry1: "Lore!",
                loreEntry2: "LORE!!",
                defaultAi: "custom-ogre",
                initialInventoryTags: ["club-giant"],
                initialSkillTags: [""]
            )
        },
        {
            "goblin-vermin",
            new(
                name: "Goblin Vermin",
                entityType: "monster",
                race: Species.Goblin.ToString(),
                tag: "goblin-vermin",
                partyId: "",
                health: 40,
                mana: 40,
                magic: 10,
                strength: 10,
                defense: 10,
                attackType: DamageType.piercing,
                dealsMagicDamage: false,
                speed: 10,
                level: 1,
                experience: 10,
                resistances: new Dictionary<DamageType, double>(SpeciesDictionary.PlayableSpeciesStats[Species.Goblin].Resistances),
                proficiencies: new Dictionary<Proficiency, double>()
                {
                    {Proficiency.bludgeoning, 0.75},
                    {Proficiency.piercing, 0.75},
                    {Proficiency.stealth, 0.66},
                    {Proficiency.melee_weapons, 0.6},
                },
                defaultAttackMessageString: "{SourceName} leaps upon {TargetName} and bites, dealing {AmountActual} damage",
                deathMessage: "",
                description: SpeciesDictionary.PlayableSpeciesStats[Species.Goblin].SpeciesDescription,
                bestiaryEntry: "",      // Further description about the physical or tempermental attributes of this specific creature.
                journalEntry: "",       // A little blurb--perhaps informational, perhaps jollitous
                loreEntry1: "",         // Lore!
                loreEntry2: "",         // LORE!!
                defaultAi: "goblin",
                initialInventoryTags: ["club", "potion-healing"],
                initialSkillTags: ["steal"]
            )
        },
        {
            "goblin-fireslinger",
            new(
                name: "Goblin Fireslinger",
                entityType: "monster",
                race: Species.Goblin.ToString(),
                tag: "goblin-fireslinger",
                partyId: "",
                health: 40,
                mana: 100,
                magic: 12,
                strength: 8,
                defense: 8,
                attackType: DamageType.crushing,
                dealsMagicDamage: false,
                speed: 12,
                level: 1,
                experience: 14,
                resistances: new Dictionary<DamageType, double>(SpeciesDictionary.PlayableSpeciesStats[Species.Goblin].Resistances),
                proficiencies: new Dictionary<Proficiency, double>()
                {
                    {Proficiency.bludgeoning, 0.65},
                    {Proficiency.firecasting, 0.9},
                    {Proficiency.spellstrike, 0.7},
                    {Proficiency.spellcasting, 0.6},
                },
                defaultAttackMessageString: "{SourceName} leaps at {TargetName} and strikes, dealing {AmountActual} damage",
                deathMessage: "",
                description: SpeciesDictionary.PlayableSpeciesStats[Species.Goblin].SpeciesDescription,
                bestiaryEntry: "",
                journalEntry: "",
                loreEntry1: "",
                loreEntry2: "",
                defaultAi: "goblin",
                initialInventoryTags: ["dagger", "potion-mana", "potion-mana", "potion-healing"],
                initialSkillTags: ["firecast", "firebolt"]
            )
        }
    };
}

// {
//     "",
//     new BeastiaryEntity(
//         name: "",
//         entityType: "",
//         race: "",
//         tag: "",
//         partyId: "",
//         health: 0,
//         mana: 0,
//         magic: 0,
//         strength: 10,
//         defense: 10,
//         attackType: DamageType.damage,
//         dealsMagicDamage: false,
//         speed: 10,
//         level: 1,
//         experience: 10,
//         resistances: new Dictionary<DamageType, double>()
//         {
//             // 
//         },
//         proficiencies: new Dictionary<Proficiency, double>()
//         {
//             // 
//         },
//         defaultAttackMessageString: "",                      // Valid string parameters are: {SourceName} {TargetName} {AttackDamageType} {AmountSent} {AmountActual}
//         deathMessage: "",
//         description: "",
//         bestiaryEntry: "",
//         journalEntry: "",
//         loreEntry1: "",
//         loreEntry2: "",
//         defaultAi: "",
//         initialInventoryTags: [""],
//         initialSkillTags: [""]
//     )
// }