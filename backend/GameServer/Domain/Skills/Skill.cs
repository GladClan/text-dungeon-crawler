using GameServer.Domain.Enums;
using GameServer.Domain.Entities;

namespace GameServer.Domain.Skills;

public abstract class Skill
{
    public int Id { get; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Cost { get; set; }
    public DamageType Element { get; set; }
    public Proficiency Prof { get; set; }
    public int Level { get; set; }
    public abstract string SkillEffect(DamageableEntity target, DamageableEntity source);
    public abstract void LevelUpSkill();
    public bool IsLearnable (DamageableEntity target)
    {
        return true;
    }

    public Skill()
    {
    }

    public Skill(string name, int cost, DamageType element, Proficiency proficiency = Proficiency.spellstrike, int level = 0)
    {
        Name = name;
        Cost = cost;
        Element = element;
        Prof = proficiency;
        Level = level;
    }
}