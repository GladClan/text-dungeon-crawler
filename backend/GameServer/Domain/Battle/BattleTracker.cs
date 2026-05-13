using GameServer.Contracts.DTOs;
using GameServer.Domain.Statistics;

namespace GameServer.Domain.Battle;

/*
Add function to enable continuous effects and battle end effects (if an effect is temporary but the battle ends before it runs out)
Add function to add the statistics for each character to the StatisticsTracker
*/

public class BattleTracker(StatisticsTracker statisticsTracker, EntityStore entityStore)
{
    private readonly EntityStore _entities = entityStore;
    private readonly StatisticsTracker _statistics = statisticsTracker;
    private readonly BattleLog _log = new();
    private readonly List<IBattleEffect> _battleEffects = [];
    private int _turns = 0;
    private int _rounds = 0;
    
    public bool AddLogEntry(EffectDto request)
    {
        return _log.AddEntry(request);
    }

    public List<string> NextTurn()
    {
        List<string> results = [];
        var groups = _battleEffects.GroupBy(e => e.EntityId);
        foreach (var g in groups)
        {
            if (_entities.TryGet(g.Key, out var target) && target is not null)
            {
                foreach(IBattleEffect b in g)
                {
                    if (!b.Apply(target))
                    {
                        _battleEffects.Remove(b);
                    }
                    else
                    {
                        results.Add(b.Message);
                    }
                }
            }
            else
            {
                results.Add($"Could not find entity id: {g.Key}");
            }
        }
        _turns++;
        throw new NotImplementedException();
    }

    public List<string> OnBattleEnd()
    {
        List<string> errors = [];
        var groups = _battleEffects.GroupBy(b => b.EntityId);
        foreach (var g in groups)
        {
            if (_entities.TryGet(g.Key, out var target)  && target is not null)
            {
                foreach (IBattleEffect b in g)
                {
                    b.Revert(target);
                }
            }
            else
            {
                errors.Add($"Could not find damageable entity {g.Key}");
            }
        }
        _statistics.AddEntriesToStats(_log.Entries);
        return errors;
    }

    public bool AddContinuousEffect(IBattleEffect battleEffect)
    {
        _battleEffects.Add(battleEffect);
        return true;
    }
}


/*
public override EffectDto SkillEffect(DamageableEntity mainTarget, List<DamageableEntity>? subTargets, DamageableEntity source, BattleContext ctx)
{
    var buff = new StatBuffEffect(mainTarget.ID, new Dictionary<string,double>{{"Strength", 5}});
    buff.Apply(ctx.EntityStore);                 // make the buff active immediately
    ctx.BattleLog.AddOnBattleEndEffect(buff);    // will call Revert() when battle ends
    // ... produce effect DTO for immediate result
}
*/