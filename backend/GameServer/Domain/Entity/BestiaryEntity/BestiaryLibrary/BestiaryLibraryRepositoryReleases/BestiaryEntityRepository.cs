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
        }
    };
}