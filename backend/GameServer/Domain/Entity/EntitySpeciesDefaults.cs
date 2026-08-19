using GameServer.Domain.Enums;

namespace GameServer.Domain.Entities;

public class SpeciesDefaultObject(Dictionary<DamageType, double> resistances, string description)
{
    public Dictionary<DamageType, double> Resistances { get; init; } = resistances;
    public string SpeciesDescription { get; } = description;
}

public static class SpeciesDictionary
{
    public static readonly Dictionary<Species, SpeciesDefaultObject> PlayableSpeciesStats = new()
    {
        {
            Species.Alf, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, 0},
                    {DamageType.piercing, 0},
                    {DamageType.crushing, 0},
                    {DamageType.spellstrike, 0.1},
                    {DamageType.physical, 0},
                    {DamageType.poisoning, 0.1},
                    {DamageType.healing, 0},
                    {DamageType.radiant, 0.1},
                    {DamageType.necro, 0},
                    {DamageType.shining, 0.1},
                    {DamageType.darkling, 0.1},
                    {DamageType.burning, -0.2},
                    {DamageType.freezing, -0.2},
                    {DamageType.shocking, 0.1},
                    {DamageType.aerial, 0.1},
                    {DamageType.terra, 0.1},
                    {DamageType.soaking, 0.1}
                },
                description: "Alfs are an ancient race. They are sensitive to the various climes of magic and are often skilled at harnessing it. " +
                    "Unlike other races, their allegiences are to harmony in nature, preserving the natural order of life and death."
            )
        },
        {
            Species.Angel, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, 0.1},
                    {DamageType.piercing, 0.05},
                    {DamageType.crushing, -0.1},
                    {DamageType.spellstrike, 0},
                    {DamageType.physical, 0.1},
                    {DamageType.poisoning, 0.1},
                    {DamageType.healing, 0},
                    {DamageType.radiant, 1.1},
                    {DamageType.necro, 1},
                    {DamageType.shining, 0.9},
                    {DamageType.darkling, -1},
                    {DamageType.burning, 0.05},
                    {DamageType.freezing, 0.05},
                    {DamageType.shocking, -0.05},
                    {DamageType.aerial, 0.25},
                    {DamageType.terra, 0.0},
                    {DamageType.soaking, -0.05}
                },
                description: "These winged people have often in history been seen as messengers from the gods. They are a proud people, but benevolent. Natural enemies to darkness."
            )
        },
        {
            Species.Cherufe, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, 0},
                    {DamageType.piercing, 0},
                    {DamageType.crushing, 0},
                    {DamageType.spellstrike, 0},
                    {DamageType.physical, 0},
                    {DamageType.poisoning, 0.1},
                    {DamageType.healing, 0},
                    {DamageType.radiant, 0},
                    {DamageType.necro, 0},
                    {DamageType.shining, 0},
                    {DamageType.darkling, 0},
                    {DamageType.burning, 1.25},
                    {DamageType.freezing, -0.25},
                    {DamageType.shocking, 0},
                    {DamageType.aerial, 0},
                    {DamageType.terra, 0.75},
                    {DamageType.soaking, -1}
                },
                description: "Cherufe are creatures at home in molten rock, often said to be born in the depths of volcanoes. They are dark, fearsome warriors, terrible to have on the opposite side of a fight."
            )
        },
        {
            Species.Daimon, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, 0.05},
                    {DamageType.piercing, -0.1},
                    {DamageType.crushing, 0.01},
                    {DamageType.spellstrike, 0},
                    {DamageType.physical, 0.05},
                    {DamageType.poisoning, 0.1},
                    {DamageType.healing, 0},
                    {DamageType.radiant, -1},
                    {DamageType.necro, 0.5},
                    {DamageType.shining, 0.25},
                    {DamageType.darkling, 1},
                    {DamageType.burning, 0.05},
                    {DamageType.freezing, -0.05},
                    {DamageType.shocking, 0.05},
                    {DamageType.aerial, 0},
                    {DamageType.terra, 0},
                    {DamageType.soaking, -0.05}
                },
                description: "Some say that Daimon are the offspring of demons themselves. They are certainly prone to act like it."
            )
        },
        {
            Species.Djinn, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, 0.3},
                    {DamageType.piercing, 0.5},
                    {DamageType.crushing, 0.25},
                    {DamageType.spellstrike, 1.5},
                    {DamageType.physical, 0.25},
                    {DamageType.poisoning, 0},
                    {DamageType.healing, 0},
                    {DamageType.radiant, 0},
                    {DamageType.necro, 0},
                    {DamageType.shining, 0},
                    {DamageType.darkling, 0},
                    {DamageType.burning, 0},
                    {DamageType.freezing, 0},
                    {DamageType.shocking, 0},
                    {DamageType.aerial, 0.25},
                    {DamageType.terra, 0},
                    {DamageType.soaking, 0}
                },
                description: "Not to be mistaken for a genie. Djinn are intimately entwined with magic, and physically, they aren't much more than flowing sand. That being said, no person in their right mind would challenge a djinn to fight."
            )
        },
        {
            Species.Goblin, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, 0.05},
                    {DamageType.piercing, 0.05},
                    {DamageType.crushing, 0.05},
                    {DamageType.spellstrike, 0},
                    {DamageType.physical, 0.05},
                    {DamageType.poisoning, 0.05},
                    {DamageType.healing, 0},
                    {DamageType.radiant, 0},
                    {DamageType.necro, 0},
                    {DamageType.shining, -0.05},
                    {DamageType.darkling, -0.05},
                    {DamageType.burning, 0},
                    {DamageType.freezing, 0},
                    {DamageType.shocking, 0},
                    {DamageType.aerial, 0},
                    {DamageType.terra, 0},
                    {DamageType.soaking, 0}
                },
                description: "Some see them as obnoxious, but the truth is goblins are cruel. They thrive in the shadows and are expert at scavenging and ambush. Their society is chaotic, often turning on each other as easily as outsiders."
            )
        },
        {
            Species.Gremlin, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, 0},
                    {DamageType.piercing, 0},
                    {DamageType.crushing, 0.05},
                    {DamageType.spellstrike, 0.05},
                    {DamageType.physical, 0},
                    {DamageType.poisoning, 0},
                    {DamageType.healing, 0},
                    {DamageType.radiant, 0},
                    {DamageType.necro, 0},
                    {DamageType.shining, 0},
                    {DamageType.darkling, 0},
                    {DamageType.burning, 0.05},
                    {DamageType.freezing, 0.05},
                    {DamageType.shocking, 0.05},
                    {DamageType.aerial, 0},
                    {DamageType.terra, 0.05},
                    {DamageType.soaking, 0}
                },
                description: "Gremlins are obnoxious pests. They are short creatures, shorter than goblins, and they often go around on all fours, crouched. " +
                    "Their face has an elongated bottom, giving them an anteater-like appearance. Their one benefit (if you can tame one) is that they're prodigious with technology. " +
                    "The only trouble is that they usually only want to stir up trouble and cause mischief."
            )
        },
        {
            Species.Human, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, -0.05},
                    {DamageType.piercing, -0.05},
                    {DamageType.crushing, -0.05},
                    {DamageType.spellstrike, 0},
                    {DamageType.physical, -0.05},
                    {DamageType.poisoning, 0},
                    {DamageType.healing, 0},
                    {DamageType.radiant, 0},
                    {DamageType.necro, 0},
                    {DamageType.shining, 0},
                    {DamageType.darkling, 0.05},
                    {DamageType.burning, 0},
                    {DamageType.freezing, 0},
                    {DamageType.shocking, 0},
                    {DamageType.aerial, 0},
                    {DamageType.terra, 0},
                    {DamageType.soaking, 0}
                },
                description: "Humans are versatile and ambitious, capable of great creativity and terrible destruction. " +
                    "Despite their lack of natural powers, their ingenuity has allowed them to thrive in nearly every environment. " +
                    "The widely-held opinion of humans is that they're unpredictable: sometimes noble, sometimes cruel."
            )
        },
        {
            Species.Merfolk, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, 0.05},
                    {DamageType.piercing, -0.05},
                    {DamageType.crushing, 0},
                    {DamageType.spellstrike, 0},
                    {DamageType.physical, 0.05},
                    {DamageType.poisoning, 0},
                    {DamageType.healing, 0},
                    {DamageType.radiant, 0},
                    {DamageType.necro, 0},
                    {DamageType.shining, 0},
                    {DamageType.darkling, 0},
                    {DamageType.burning, -0.5},
                    {DamageType.freezing, -0.2},
                    {DamageType.shocking, -0.3},
                    {DamageType.aerial, 0},
                    {DamageType.terra, 0},
                    {DamageType.soaking, 1.25}
                },
                description: "Merfolk are graceul and elusive aquatic beings whose society is hidden beneath the waves. Their moods can be as changeable as the tides. " +
                    "Their knowledge of the water and its creatures makes them formidable in their element."
            )
        },
        {
            Species.Ulmloth, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, 0},
                    {DamageType.piercing, 0},
                    {DamageType.crushing, 0},
                    {DamageType.spellstrike, 0},
                    {DamageType.physical, 0},
                    {DamageType.poisoning, 0.5},
                    {DamageType.healing, 0},
                    {DamageType.radiant, 0},
                    {DamageType.necro, 0},
                    {DamageType.shining, -0.5},
                    {DamageType.darkling, 0.5},
                    {DamageType.burning, -0.1},
                    {DamageType.freezing, -0.1},
                    {DamageType.shocking, -0.1},
                    {DamageType.aerial, 0},
                    {DamageType.terra, 0},
                    {DamageType.soaking, 0}
                },
                description: "A mysterious lot, the Ulmloth. Their societies are unknown and their pale complexions give little away. " +
                    "All that is know of them really is their affinity for magical brews and their propensity for shadowed places."
            )
        },
        {
            Species.Wildcap, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, -0.05},
                    {DamageType.piercing, 0.05},
                    {DamageType.crushing, 0},
                    {DamageType.spellstrike, 0},
                    {DamageType.physical, 0},
                    {DamageType.poisoning, 1.1},
                    {DamageType.healing, -0.25},
                    {DamageType.radiant, 0},
                    {DamageType.necro, 0},
                    {DamageType.shining, -0.05},
                    {DamageType.darkling, 0.05},
                    {DamageType.burning, -0.5},
                    {DamageType.freezing, -0.5},
                    {DamageType.shocking, -0.5},
                    {DamageType.aerial, 0},
                    {DamageType.terra, 0.05},
                    {DamageType.soaking, 0.5}
                },
                description: "Often referred to as mushroom people. The Wildcap's body is mostly normal, the only noticeable difference being in their mushrooms heads. " +
                    "Likewise, the color of their skin matches the color of the mushroom. The most common Wildcap variant is red, but they can be seen in a variety of purple, brown, green, and other fungal colors."
            )
        },
        {
            Species._undead, new(
                resistances: new Dictionary<DamageType, double>{
                    {DamageType.slashing, 0},
                    {DamageType.piercing, 0},
                    {DamageType.crushing, 0},
                    {DamageType.spellstrike, 0},
                    {DamageType.physical, 0},
                    {DamageType.poisoning, 1},
                    {DamageType.healing, -1},
                    {DamageType.radiant, -0.5},
                    {DamageType.necro, 2},
                    {DamageType.shining, -0.25},
                    {DamageType.darkling, 0.5},
                    {DamageType.burning, -0.5},
                    {DamageType.freezing, 0.2},
                    {DamageType.shocking, 0},
                    {DamageType.aerial, 0},
                    {DamageType.terra, 0},
                    {DamageType.soaking, 0}
                },
                description: "An abomination of nature. Any creature can be made undead by a necromancer of sufficient power, and to witness such an abomination will leave no soul untainted."
            )
        },
    };
}