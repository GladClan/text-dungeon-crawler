using GameServer.Domain.Enums;
using GameServer.Domain.Entities;
using GameServer.Application.Common;
using GameServer.Contracts.DTOs;
using GameServer.Domain.Battle;

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
    public bool MultiTarget { get; set; }
    public int TargetsLimit { get; set; }
    public ActionType SkillType { get; init; }
    public int Level { get; set; }
    public abstract EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle);
    public abstract void LevelUpSkill();
    public bool IsLearnable (DamageableEntity target)
    {
        return true;
    }

    public Skill()
    {
        Id = NewId();
    }

    protected Skill(string name, string tag, string description, int cost, DamageType element, bool multiTarget, int targetsLimit, ActionType skillType, Proficiency proficiency = Proficiency.spellstrike, int level = 0)
    {
        Id = NewId();
        Name = name;
        Tag = tag;
        Description = description;
        Cost = cost;
        Element = element;
        Prof = proficiency;
        MultiTarget = multiTarget;
        TargetsLimit = targetsLimit;
        SkillType = skillType;
        Level = level;
    }

    private string NewId()
    {
        return $"{Name.Trim().PadLeft(5, '_')[..5]}-{OrdinalDateString.GetOrdinalDate(3)}";
    }
}