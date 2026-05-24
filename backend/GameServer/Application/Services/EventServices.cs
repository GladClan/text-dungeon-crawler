using GameServer.Application.Common;
using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Battle;
using GameServer.Domain.Enums;
using GameServer.Domain.Statistics;
using GameServer.Infrastructure;

namespace Gameserver.Application.Services;

public sealed class EventServices(StatisticsTracker statisticsTracker, EntityService entityService, EntityStore entityStore)
{
    private readonly EntityStore _entities = entityStore;
    private readonly EntityService _service = entityService;
    private readonly StatisticsTracker _statistics = statisticsTracker;
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
                var target = _service.AddEntity(eRequest);
                result.Add(target);
                if (target.Entity is not null)
                {
                    opponentParty.Add(target.Entity);
                }
            }
        }
        var party = _service.GetParty(request.PartyId);
        CurrentBattle = new(_statistics, _service, request.PartyId, request.OpponentPartyId ?? $"temp-{OrdinalDateString.GetOrdinalDate(4)}");
        var initiative = CurrentBattle.GetInitiativeOrder();
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
        var errors = CurrentBattle.OnBattleEnd();

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

    public ChallengeResultDto? ChallengeEntitySkill(string id, double value, Proficiency proficiency)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            Random random = new();
            double chance = random.NextDouble();
            double result = target.GetProficiencyMultiplier(proficiency).Value * chance;
            return new ChallengeResultDto
            {
                Success = result > value,
                Margin = value - result
            };
        }
        return null;
    }
}