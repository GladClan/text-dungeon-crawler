using GameServer.Domain.Enums;
using GameServer.Domain.Skills;

namespace GameServer.Domain.Entities.BeastiaryEntity;

public class BeastiaryEntity(
    string name,
    string entityType,
    string race,
    string tag,
    string partyId,
    int health,
    int mana,
    int magic,
    int strength,
    int defense,
    DamageType attackType,
    bool dealsMagicDamage = false,
    int speed = 12,
    int level = 0,
    int experience = 0,
    Dictionary<DamageType, double>? resistances = null,
    Dictionary<Proficiency, double>? proficiencies = null,
    string? defaultAttackMessageString = null,
    string deathMessage = "",
    string description = "",
    string bestiaryEntry = "",
    string journalEntry = "",
    string loreEntry1 = "",
    string loreEntry2 = "",
    string defaultAi = "",
    List<string>? initialInventoryTags = null,
    List<string>? initialSkillTags = null
    ) : DamageableEntity(
    name: name,
    entityType: entityType,
    race: race,
    partyId: partyId,
    health: health,
    mana: mana,
    magic: magic,
    strength: strength,
    defense: defense,
    attackType: attackType,
    dealsMagicDamage: dealsMagicDamage,
    speed: speed,
    level: level,
    experience: experience,
    resistances: resistances,
    proficiencies: proficiencies,
    defaultAttackMessageString: defaultAttackMessageString,
    deathMessage: deathMessage
    )
{
    public string Tag = tag;
    public string Description = description;
    public string BestiaryEntry = bestiaryEntry;
    public string JournalEntry = journalEntry;
    public string LoreEntry1 = loreEntry1;
    public string LoreEntry2 = loreEntry2;
    public string DefaultAi = defaultAi;
    public List<string> ItemTagsForInitialInventory = initialInventoryTags ?? [];
    public List<string> SkillTagsForInitialSkills = initialSkillTags ?? [];

    public override BeastiaryEntity Clone()
    {
        var cloneSkills = new List<Skill>();
        foreach (var skill in Skills)
        {
            cloneSkills.Add(skill.Clone());
        }

        var result = new BeastiaryEntity(
            name: Name,
            entityType: EntityType,
            race: Race,
            tag: Tag,
            partyId: PartyId,
            health: MaxHealth,
            mana: MaxMana,
            magic: (int)Magic,
            strength: (int)Strength,
            defense: (int)Defense,
            attackType: AttackDamageType,
            dealsMagicDamage: DealsMagicDamage,
            speed: (int)Speed,
            level: Level,
            experience: Experience,
            resistances: Resistances,
            proficiencies: Proficiencies,
            defaultAttackMessageString: DefaultAttackMessageString,
            deathMessage: DeathMessage,
            bestiaryEntry: BestiaryEntry,
            journalEntry: JournalEntry,
            loreEntry1: LoreEntry1,
            loreEntry2: LoreEntry2,
            defaultAi: DefaultAi,
            initialInventoryTags: ItemTagsForInitialInventory,
            initialSkillTags: SkillTagsForInitialSkills
        )
        {
            CurrentHealth = CurrentHealth,
            CurrentMana = CurrentMana,
            Inventory = Inventory.Clone(),
            Skills = cloneSkills
        };

        return result;
    }
}