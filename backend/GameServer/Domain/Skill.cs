namespace GameServer.Domain;

public class Skill
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int Cost { get; private set; }
    public int Level { get; private set; }
    // proficieny // i.e. gen (spellstrike), ice, fire, lightning, holy, necro, etc.
    // element // i.e. fire, lightning, water, holy, etc.
    // effect
    // isLearnable (entity)
}