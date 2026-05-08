using GameServer.Domain.Enums;
using GameServer.Domain.Entities;
using GameServer.Application.Common;

namespace GameServer.Domain.Skills;

public abstract class Skill
{
    public string Id { get; }
    public string Name { get; set; } = string.Empty;
    public string Tag { get; init; } = string.Empty;
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
        Id = NewId();
    }

    protected Skill(string name, string tag, int cost, DamageType element, Proficiency proficiency = Proficiency.spellstrike, int level = 0)
    {
        Id = NewId();
        Name = name;
        Tag = tag;
        Cost = cost;
        Element = element;
        Prof = proficiency;
        Level = level;
    }

    private string NewId()
    {
        return $"{Name.Trim().PadLeft(5, '_')[..5]}-{OrdinalDateString.GetOrdinalDate(3)}";
    }
}