using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Battle;


public sealed class StatusEffect(
    string targetId,
    string message,
    StatType stat,
    double delta,
    int duration,
    DamageType damageType,
    int turnDelayBeforeActive = 0,
    double deltaChange = 0
) : IBattleEffect
{
    public string EntityId { get; init; } = targetId;
    public string Message { get; set; } = message;
    public int RemainingTurns { get; set; } = duration;
    private double _delta = delta;
    private readonly double _deltaChange = deltaChange;
    private int _turnDelayBeforeActive = turnDelayBeforeActive;
    private readonly DamageType _damageType = damageType;

    public bool Apply(DamageableEntity target)
    {
        if (_turnDelayBeforeActive <= 0 && RemainingTurns > 0)
        {
            if (RemainingTurns > 0)
            {
                RemainingTurns--;
                bool result = ModifyStat(target, stat, _delta, _damageType);
                _delta += _deltaChange;
                return result;
            }
            else
            {
                _turnDelayBeforeActive--;
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public bool Revert(DamageableEntity target)
    {
        return false;
    }

    private static bool ModifyStat(DamageableEntity entity, StatType stat, double amount, DamageType damageType)
    {
        switch (stat)
        {
            case StatType.MaxHealth: 
                entity.MaxHealth -= (int) amount;
                entity.CurrentHealth -= amount;
                return true;
            case StatType.CurrentHealth:
                entity.TakeDamage(new DamageableEntity(), amount, damageType);
                return true;
            case StatType.Magic: 
                entity.Magic -= amount;
                return true;
            case StatType.MaxMana: 
                entity.MaxMana -= (int) amount;
                entity.CurrentMana -= amount;
                return true;
            case StatType.CurrentMana: 
                entity.CurrentMana -= amount;
                return true;
            case StatType.Strength: 
                entity.Strength -= amount;
                return true;
            case StatType.Defense: 
                entity.Defense -= amount;
                return true;
            case StatType.Speed: 
                entity.Speed -= amount;
                return true;
            default: 
                return false;
        }
    }
}