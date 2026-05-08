using GameServer.Contracts.DTOs;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Skills.SkillsLibrary.InitialRelease;

public sealed class ErrorSkill : Skill
{
    private int _damage = 30;
    private int _useage = 0;
    public ErrorSkill(): base(
        name: "Mysterious Cloud",
        tag: "error",
        cost: 0,
        element: DamageType.damage,
        proficiency: Proficiency.spellstrike,
        level: 1
    ) { }

    public override void LevelUpSkill()
    {
        Level++;
        _damage += 10;
    }

    public override string SkillEffect(DamageableEntity target, DamageableEntity source)
    {
        _useage++;
        DamageResultDto first, second, third;
        int damage = 0;
        first = target.TakeDamage(source, _damage, Element);
        damage += (int) first.DamageActual;
        if (Level > 3)
        {
            second = target.TakeDamage(source, _damage / 2, Element);
            damage += (int) second.DamageActual;
            if (Level > 7)
            {
                third = target.TakeDamage(source, _damage / 3, Element);
                damage += (int) third.DamageActual;
            }
        }
        if (_useage > Level * 10)
        {
            LevelUpSkill();
            _useage = 0;
        }
        return $"{target.Name} takes {damage} {Element} damage from a mysterious, buzzing cloud summoned by {source.Name}";
    }
}