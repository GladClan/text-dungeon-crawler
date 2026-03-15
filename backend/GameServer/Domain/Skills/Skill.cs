using GameServer.Domain.Enums;
using GameServer.Domain.Entities;

namespace GameServer.Domain.Skills;

abstract class Skill(string name, int cost, DamageType element, Proficiency proficiency = Proficiency.spellstrike, int level = 0)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; protected set; } = name;
    public int Cost { get; protected set; } = cost;
    public DamageType Element { get; protected set; } = element; // i.e. fire, lightning, water, holy, etc.
    public Proficiency Prof { get; protected set; } = proficiency; // i.e. gen (spellstrike), ice, fire, lightning, holy, necro, etc.
    public int Level { get; protected set; } = level;
    public abstract string SkillEffect(Entity target, Entity source);
    public abstract void LevelUpSkill();
    public bool IsLearnable (Entity target)
    {
        return true;
    }
}