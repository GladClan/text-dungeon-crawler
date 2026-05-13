using GameServer.Contracts.DTOs;
using GameServer.Domain.Battle;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Items.ItemsLibrary.InitialRelease;

public sealed class ErrorItem : Useable
{
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
        targetsLimit: 1,
        proficiency: Proficiency.hand,
        shopType: "error",
        rarity: 1000,
        collection: 99
    ) { }

    public override EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle)
    {
        var result = mainTarget.TakeDamage(new DamageableEntity(), 1000d, Element);
        return new EffectDto
        {
            Message = $"{mainTarget.Name} takes {result.AmountActual} damage from a mysterious, buzzing cloud, summoned by {source.Name}",
            Results = [result]
        };
    }
}