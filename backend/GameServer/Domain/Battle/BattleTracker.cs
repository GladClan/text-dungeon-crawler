using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Entities;
using GameServer.Domain.Statistics;

namespace GameServer.Domain.Battle;

/*
Add function to enable continuous effects and battle end effects (if an effect is temporary but the battle ends before it runs out)
Add function to add the statistics for each character to the StatisticsTracker
*/

public class BattleTracker(StatisticsTracker statisticsTracker, EntityService entityService, string partyId, string opponentPartyId)
{
    public string PartyId { get; set; } = partyId;
    public string OpponentPartyId { get; set; } = opponentPartyId;
    private readonly EntityService _service = entityService;
    private readonly StatisticsTracker _statistics = statisticsTracker;
    public readonly BattleLog Log = new();
    private readonly List<IBattleEffect> _battleEffects = [];
    public List<InitiativeDto> InitiativeOrder = [];
    private int _turns = 0;
    private int _rounds = 0;
    
    public bool AddLogEntry(EffectDto request)
    {
        return Log.AddEntry(request);
    }

    public List<string> GetPartyIds(string partyId)
    {
        return [.._service.GetParty(partyId).Select(e => e.PartyId)];
    }

    public List<InitiativeDto> GetInitiativeOrder()
    {
        List<InitiativeDto> result = [];
        var party = _service.GetParty(PartyId);
        var enemies = _service.GetParty(OpponentPartyId);
        
        List<DamageableEntityDto> members = [.. party, .. enemies];
        members.Sort((a, b) => b.Speed.CompareTo(a.Speed));
        
        int turnCounts = members.Sum(m => 1 + (int)Math.Floor(m.Speed / 20));
        for (int i = 0; i < turnCounts; i++)
        {
            result.Add(new InitiativeDto
            {
                Initiative = i,
                EntityName = members[i % members.Count].Name,
                EntityId = members[i].Id
            });
        }
        InitiativeOrder = result;
        return result;
    }

    public TurnoverDto NextTurn()
    {
        List<string> results = [];
        string error = "";
        var groups = _battleEffects.GroupBy(e => e.EntityId);
        foreach (var g in groups)
        {
            var target = _service.GetDamageableEntityObject(g.Key);
            if (target is not null)
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
                error += $"Could not find entity id: {g.Key}\n";
            }
        }
        _turns++;
        if (_turns > InitiativeOrder.Count)
        {
            _turns = 0;
            _rounds++;
            InitiativeOrder = GetInitiativeOrder();
        }
        if (!_service.GetParty(PartyId).Any(e => e.IsEntityAlive) || !_service.GetParty(OpponentPartyId).Any(e => e.IsEntityAlive))
        {
            error += string.Join("\n", OnBattleEnd());
        }
        return new TurnoverDto
        {
            Messages = results,
            InitiativeOrder = InitiativeOrder,
            Error = error
        };
    }

    public List<string> OnBattleEnd()
    {
        List<string> errors = [];
        var groups = _battleEffects.GroupBy(b => b.EntityId);
        foreach (var g in groups)
        {
            var target = _service.GetDamageableEntityObject(g.Key);
            if (target is not null)
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
        _statistics.AddEntriesToStats(Log.Entries);
        return errors;
    }

    public AddEntityResult AddEntityToBattle(DamageableEntityRequest request)
    {
        return _service.AddEntity(request);
    }

    public bool AddContinuousEffect(IBattleEffect battleEffect)
    {
        _battleEffects.Add(battleEffect);
        return true;
    }

    public DamageableEntity? GetEntity(string id)
    {
        return _service.GetDamageableEntityObject(id);
    }

    public bool ExistsPartyMemberAtCriticalHealth(string partyId, int criticalPercentage = 20)
    {
        var party = _service.GetParty(partyId);
        return party.Any(e => (e.CurrentHealth / e.MaxHealth * 100) <= criticalPercentage);
    }

    public string? GetPartyMemberAtCriticalHealth(string partyId, int criticalPercentage = 20)
    {
        var party = _service.GetParty(partyId);
        var result = party.FirstOrDefault(m => (m.CurrentHealth / m.MaxHealth * 100) <= criticalPercentage);
        return result?.Id;
    }
}