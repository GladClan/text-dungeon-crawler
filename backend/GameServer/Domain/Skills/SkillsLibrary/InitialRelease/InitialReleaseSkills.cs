using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
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
        multiTarget: false,
        targetsLimit: 1,
        level: 1
    ) { }

    public override void LevelUpSkill()
    {
        Level++;
        _damage += 10;
    }

    public override EffectDto SkillEffect(DamageableEntity mainTarget, List<DamageableEntity>? subTargets, DamageableEntity source)
    {
        _useage++;
        DamageResultDto first, second, third;
        int damage = 0;
        first = mainTarget.TakeDamage(source, _damage, Element);
        DamageResultDto final = first;
        damage += (int) first.AmountActual;
        if (Level > 3)
        {
            second = mainTarget.TakeDamage(source, _damage / 2, Element);
            damage += (int) second.AmountActual;
            final = first.MergeResults(second);
            if (Level > 7)
            {
                third = mainTarget.TakeDamage(source, _damage / 3, Element);
                damage += (int) third.AmountActual;
                final = final.MergeResults(third);
            }
        }
        if (_useage > Level * 10)
        {
            LevelUpSkill();
            _useage = 0;
        }
        return new EffectDto
        {
            Message = $"{mainTarget.Name} takes {damage} {Element} damage from a mysterious, buzzing cloud summoned by {source.Name}",
            Result = final
        };
    }
}