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
        proficiency: Proficiency.hand) { }

    public override string ItemEffect(DamageableEntity target, DamageableEntity source)
    {
        var result = target.TakeDamage(new DamageableEntity(), 1000d, Element);
        return $"{target.Name} takes {result.DamageActual} damage from a mysterious, buzzing cloud, summoned by {source.Name}";
    }
}