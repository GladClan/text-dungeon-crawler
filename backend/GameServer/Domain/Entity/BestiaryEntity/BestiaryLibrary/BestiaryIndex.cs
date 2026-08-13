using GameServer.Domain.Entities.BeastiaryEntity.BestiaryLibrary.BestiaryLibraryRepositoryReleases;

namespace GameServer.Domain.Entities.BeastiaryEntity.BestiaryLibrary;

public class BestiaryIndex: IBestiaryIndex
{
    private static readonly IReadOnlyList<IReadOnlyDictionary<string, BeastiaryEntity>> _repositories = [
        InitialReleaseRepository.Repository,
        // FutureReleaseRepository.Repository,
        // LatestReleaseRepository.Repository,
    ];

    private static readonly Dictionary<string, BeastiaryEntity> _catalog = BuildCatalog();
    
    private static Dictionary<string, BeastiaryEntity> BuildCatalog()
    {
        var catalog = new Dictionary<string, BeastiaryEntity>(StringComparer.InvariantCultureIgnoreCase);

        foreach (var repository in _repositories)
        {
            foreach (var (tag, entity) in repository)
            {
                catalog[tag] = entity;
            }
        }
        return catalog;
    }
    public BeastiaryEntity NewBeastiaryEntityByTag(string tag)
    {
        var reference = _catalog.TryGetValue(tag, out var entity)
            ? entity
            : _catalog["error"];
        
        BeastiaryEntity result = new(
            name: reference.Name,
            entityType: reference.EntityType,
            race: reference.Race,
            tag: reference.Tag,
            partyId: reference.PartyId,
            health: reference.MaxHealth,
            mana: reference.MaxMana,
            magic: (int)reference.Magic,
            strength: (int)reference.Strength,
            defense: (int)reference.Defense,
            attackType: reference.AttackDamageType,
            dealsMagicDamage: reference.DealsMagicDamage,
            speed: (int)reference.Speed,
            level: reference.Level,
            experience: reference.Experience,
            resistances: reference.Resistances,
            proficiencies: reference.Proficiencies,
            defaultAttackMessageString: reference.DefaultAttackMessageString,
            deathMessage: reference.DeathMessage,
            bestiaryEntry: reference.BestiaryEntry,
            journalEntry: reference.JournalEntry,
            loreEntry1: reference.LoreEntry1,
            loreEntry2: reference.LoreEntry2,
            defaultAi: reference.DefaultAi,
            initialInventoryTags: reference.ItemTagsForInitialInventory,
            initialSkillTags: reference.SkillTagsForInitialSkills
        );

        return result;
    }
}