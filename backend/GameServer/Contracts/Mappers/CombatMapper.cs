using GameServer.Contracts.DTOs;

namespace GameServer.Contracts.Mappers;

public static class CombatMapper
{
    public static DamageResultDto MergeResults(this DamageResultDto first, DamageResultDto second)
    {
        int damage_healing_mana = 0;
        if (first.AmountActual + second.AmountActual > 0)
        {
            damage_healing_mana = 2;
        }
        else
        {
            damage_healing_mana = 1;
        }
        return new DamageResultDto
        {
            Damage_Healing_Mana = damage_healing_mana,
            AmountSent = first.AmountSent + second.AmountSent,
            AmountActual = first.AmountActual + second.AmountActual,
            NewValue = second.NewValue,
            Fatal = first.Fatal || second.Fatal
        };
    }
}