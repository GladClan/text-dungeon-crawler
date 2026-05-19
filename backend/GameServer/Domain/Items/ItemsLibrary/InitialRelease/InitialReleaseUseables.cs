using GameServer.Contracts.DTOs;
using GameServer.Domain.Battle;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Items.ItemsLibrary.InitialRelease;

public sealed class ErrorItem : Useable
{
    private int _uses = 0;
    private double _damage = 150d;
    public override bool CanUse(DamageableEntity target)
    {
        return true;
    }

    public ErrorItem(): base(
        type: "error",
        name: "Mysterious Cloud",
        cost: 0,
        description: "Description",
        consumable: true,
        sellable: false,
        element: DamageType.damage,
        multiTarget: false,
        targetsLimit: 100,
        proficiency: Proficiency.hand,
        itemType: ActionType.Attack,
        shopType: "error",
        rarity: 1000,
        collection: 99
    ) { }

    private void OnUse(double proficiency)
    {
        _damage *= proficiency / _uses;
    }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        source.AddProficiencyEntry(Proficiency.hand);
        _uses++;
        List<DamageResultDto> results = [];
        results.Add(DoEffect(source, mainTarget));
        string subMessage = "";
        if (subTargets is not null)
        {
            foreach (var target in subTargets)
            {
                results.Add(DoEffect(source, target));
                subMessage += $"\n{target.Name} is enveloped.";
            }
        }
        double damage = 0;
        foreach (var dto in results)
        {
            damage += dto.AmountActual;
        }
        string message = (subTargets is null) ? $"{mainTarget.Name} takes {damage} {Element} damage from a mysterious, buzzing cloud summoned by {source.Name}"
            : $"{source.Name} summons a mysterious, buzzing cloud which envelops {mainTarget.Name} and others, dealing {damage} {Element} damage.{subMessage}";
        
        var effect = new EffectDto
        {
            Message = message,
            Results = results
        };
        OnUse(source.GetProficiencyMultiplier(Proficiency.destiny).Value);
        battle.AddLogEntry(effect);
        return effect;
    }

    private DamageResultDto DoEffect(DamageableEntity target, DamageableEntity source)
    {
        if (_uses < 10)
        {
            return target.TakeDamage(source, _damage * 3, Element);
        }
        if (_uses < 30)
        {
            return target.TakeDamage(source, _damage * 2, Element);
        }
        if (_uses < 45)
        {
            return target.TakeDamage(source, _damage, Element);
        }
        return target.TakeDamage(source, _damage / 2, Element);
    }
}