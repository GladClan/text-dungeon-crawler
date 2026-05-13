using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Battle;

public sealed class StatBuffEffect(
    string targetId,
    string message,
    StatType stat,
    double delta,
    int duration,
    bool appliesOverTime,
    int turnDelayBeforeActive = 0,
    DamageType? resistance = null,
    Proficiency? proficiency = null
) : IBattleEffect
{
    public string EntityId { get; init; } = targetId;
    public string Message { get; set; } = message;
    public int RemainingTurns { get; set; } = duration;
    private int _turnDelayBeforeActive = turnDelayBeforeActive;
    private readonly double _delta = delta;
    private readonly bool _appliesOverTime = appliesOverTime;
    private bool _activated = false;

    public bool Apply(DamageableEntity target)
    {
        if (_turnDelayBeforeActive <= 0 && RemainingTurns > 0)
        {
            RemainingTurns--;
            if (!_activated)
            {
                _activated = true;
                return ModifyStat(target, stat, _delta, resistance, proficiency);
            }
            return true;
        }
        else if (_appliesOverTime)
        {
            _turnDelayBeforeActive--;
            return true;
        }
        else if (_activated)
        {
            return !Revert(target);
        }
        else
        {
            return false;
        }
    }

    public bool Revert(DamageableEntity target)
    {
        return ModifyStat(target, stat, -_delta, resistance, proficiency);
    }

    private static bool ModifyStat(DamageableEntity entity, StatType stat, double amount, DamageType? resistance, Proficiency? proficiency)
    {
        switch (stat)
        {
            case StatType.MaxHealth: 
                entity.MaxHealth += (int) amount;
                entity.CurrentHealth += amount;
                return true;
            case StatType.CurrentHealth:
                entity.CurrentHealth += amount;
                return true;
            case StatType.Magic: 
                entity.Magic += amount;
                return true;
            case StatType.MaxMana: 
                entity.MaxMana += (int) amount;
                entity.CurrentMana += amount;
                return true;
            case StatType.CurrentMana: 
                entity.CurrentMana += amount;
                return true;
            case StatType.Strength: 
                entity.Strength += amount;
                return true;
            case StatType.Defense: 
                entity.Defense += amount;
                return true;
            case StatType.Speed: 
                entity.Speed += amount;
                return true;
            case StatType.Resistances: 
                if (resistance is not null)
                {
                    if (entity.Resistances.TryGetValue((DamageType)resistance, out double val))
                    {
                        entity.Resistances[(DamageType)resistance] = val + amount;
                    }
                    else
                    {
                        entity.Resistances[(DamageType)resistance] = amount;
                    }
                    return true;
                }
                return false;
            case StatType.Proficiencies: 
                if (proficiency is not null)
                {
                    if (entity.Proficiencies.TryGetValue((Proficiency)proficiency, out double val))
                    {
                        entity.Proficiencies[(Proficiency)proficiency] = val + amount;
                    }
                    else
                    {
                        entity.Proficiencies[(Proficiency)proficiency] = amount;
                    }
                    return true;
                }
                return false;
            default: 
                return false;
        }
    }
}