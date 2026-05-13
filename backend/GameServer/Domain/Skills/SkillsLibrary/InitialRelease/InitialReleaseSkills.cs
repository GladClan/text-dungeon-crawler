using System.Xml;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Domain.Battle;
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
        multiTarget: true,
        targetsLimit: 100,
        level: 1
    ) { }

    public override void LevelUpSkill()
    {
        Level++;
        _damage += 10;
    }

    public override EffectDto SkillEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        _useage++;
        List<DamageResultDto> results = [];
        DoEffect(results, mainTarget, source);
        string subMessage = "";
        if (subTargets is not null)
        {
            foreach (DamageableEntity target in subTargets)
            {
                DoEffect(results, target, source);
                subMessage += $"\n{target.Name} is enveloped.";
            }
        }
        if (_useage > Level * 10)
        {
            LevelUpSkill();
            _useage = 0;
        }
        double damage = 0;
        foreach (var dto in results)
        {
            damage += dto.AmountActual;
        }
        string message = (subTargets is null) ? $"{mainTarget.Name} takes {damage} {Element} damage from a mysterious, buzzing cloud summoned by {source.Name}"
            : $"{source.Name} summons a mysterious, buzzing cloud which envelops {mainTarget.Name} and others, dealing {damage} {Element} damage.{subMessage}";
        
        return new EffectDto
        {
            Message = message,
            Results = [.. results]
        };
    }

    private void DoEffect(List<DamageResultDto> resultDtos, DamageableEntity target, DamageableEntity source)
    {
        resultDtos.Add(target.TakeDamage(source, _damage, Element));
        if (Level > 3)
        {
            resultDtos.Add(target.TakeDamage(source, _damage / 2, Element));
            if (Level > 7)
            {
                resultDtos.Add(target.TakeDamage(source, _damage / 3, Element));
            }
        }
    }
}