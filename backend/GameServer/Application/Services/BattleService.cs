using GameServer.Application.Common;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Battle;
using GameServer.Infrastructure;

namespace GameServer.Application.Services;

public sealed class BattleService(StatisticsService statisticsService, EntityService entityService, EntityStore entityStore)
{
    private readonly EntityStore _entities = entityStore;
    private readonly EntityService _service = entityService;
    private readonly StatisticsService _statistics = statisticsService;
    private BattleTracker? CurrentBattle;

    public BattleDto CommenceBattle(BattleStartRequest request)
    {
        if (request.OpponentPartyId is null && request.entityRequests is null)
        {
            return new BattleDto
            {
                Error = "The party wins! No opponents found."
            };
        }
        List<DamageableEntityDto> opponentParty = [];
        if (request.OpponentPartyId is not null)
        {
            opponentParty.AddRange( _service.GetParty(request.OpponentPartyId));
        }
        List<AddEntityResult>? result = null;
        if (request.entityRequests is not null)
        {
            result = [];
            foreach (var eRequest in request.entityRequests)
            {
                var target = _service.AddEntityFromRequest(eRequest);
                result.Add(target);
                if (target.Entity is not null)
                {
                    opponentParty.Add(target.Entity);
                }
            }
        }
        var party = _service.GetParty(request.PartyId);
        CurrentBattle = new(request.PartyId, request.OpponentPartyId ?? $"temp-{OrdinalDateString.GetOrdinalDate(4)}", _service);
        var initiative = GetInitiativeOrder()!;
        return new BattleDto
        {
            EntityDtos = [..opponentParty, ..party],
            EntityResult = result,
            InitiativeOrder = initiative
        };
    }

    public BattleEndDto? EndBattle()
    {
        if (CurrentBattle is null)
        {
            return null;
        }
        var errors = OnBattleEnd();

        // Add experience to entities and record level ups
        var enemyParty = _entities.GetParty(CurrentBattle.OpponentPartyId);
        var party = _entities.GetParty(CurrentBattle.PartyId);
        var livingParty = party.Where(m => m.IsEntityAlive);
        List<LevelUpDto> expGained = [];
        foreach (var e in enemyParty)
        {
            if (!e.IsEntityAlive)
            {
                foreach (var m in livingParty)
                {
                    var gain = m.AddExperience(e.Experience);
                    expGained.Add(gain);
                }
            }
        }
        var enemiesAlive = enemyParty.Where(m => m.IsEntityAlive);
        foreach (var e in enemiesAlive)
        {
            foreach (var m in party)
            {
                if (!m.IsEntityAlive)
                {
                    _ = e.AddExperience(m.Experience * m.Level);
                }
            }
        }
        var result = new BattleEndDto
        {
            Victory = livingParty.Any(),
            Error = errors is null ? string.Empty : string.Join("\n", errors),
            Party = party.ToDtos(),
            Opponents = enemyParty.ToDtos(),
            LevelUps = expGained
        };
        _service.RemoveEntitiesNotAliveInParty(CurrentBattle.OpponentPartyId);
        CurrentBattle = null;
        return result;
    }

    public List<InitiativeDto>? GetInitiativeOrder()
    {
        if (CurrentBattle == null)
        {
            return null;
        }
        List<InitiativeDto> result = [];
        var party = _service.GetParty(CurrentBattle.PartyId);
        var enemies = _service.GetParty(CurrentBattle.OpponentPartyId);
        
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
        CurrentBattle.InitiativeOrder = result;
        return result;
    }

    public TurnoverDto? NextTurn()
    {
        if (CurrentBattle == null)
        {
            return null;
        }

        List<string> results = [];
        string error = "";
        var groups = CurrentBattle.GetBattleEffectsGroupedById();
        foreach (var g in groups)
        {
            var target = _service.GetDamageableEntityObject(g.Key);
            if (target is not null)
            {
                foreach(IBattleEffect b in g)
                {
                    if (!b.Apply(target))
                    {
                        CurrentBattle.RemoveBattleEffect(b);
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
        CurrentBattle.Turn++;
        if (CurrentBattle.Turn > CurrentBattle.InitiativeOrder.Count)
        {
            CurrentBattle.Turn = 0;
            CurrentBattle.Round++;
            CurrentBattle.InitiativeOrder = GetInitiativeOrder()!;
        }
        if (!_service.GetParty(CurrentBattle.PartyId).Any(e => e.IsEntityAlive) || !_service.GetParty(CurrentBattle.OpponentPartyId).Any(e => e.IsEntityAlive))
        {
            error += string.Join("\n", OnBattleEnd()!);
        }
        return new TurnoverDto
        {
            CurrentTurn = CurrentBattle.Turn,
            Messages = results,
            InitiativeOrder = CurrentBattle.InitiativeOrder,
            Error = error
        };
    }

    public List<string>? OnBattleEnd()
    {
        if (CurrentBattle == null)
        {
            return null;
        }
        List<string> errors = [];
        var groups = CurrentBattle.GetBattleEffectsGroupedById();
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
        _statistics.AddEntriesToStats(CurrentBattle.Log.GetAllEntries());
        return errors;
    }

    public bool? AddContinuousEffect(IBattleEffect battleEffect)
    {
        if (CurrentBattle == null)
        {
            return null;
        }
        return CurrentBattle.AddContinuousEffect(battleEffect);
    }

    public bool? RemoveAllContinuousEffects(string targetId)
    {
        if (CurrentBattle == null)
        {
            return null;
        }
        bool? result = null;
        if (_entities.TryGet(targetId, out var target) && target is not null)
        {
            var effects = CurrentBattle.GetAllEfectsForTarget(targetId);
            foreach (var effect in effects)
            {
                if (effect.EntityId.Equals(targetId, StringComparison.InvariantCultureIgnoreCase))
                {
                    effect.Revert(target);
                    CurrentBattle.RemoveBattleEffect(effect);
                    result = true;
                }
            }
        }
        return result;
    }

    public bool? RemoveContinuousEffect(string targetId, string effectTag)
    {
        if (CurrentBattle == null)
        {
            return null;
        }
        var effect = CurrentBattle.GetBattleEffect(targetId, effectTag);
        if (effect is null)
        {
            return false;
        }
        if (_entities.TryGet(targetId, out var targetEntity) && targetEntity is not null)
        {
            effect.Revert(targetEntity);
            CurrentBattle.RemoveBattleEffect(effect);
            return true;
        }
        return null;
    }
}