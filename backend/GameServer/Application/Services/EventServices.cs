using GameServer.Application.Common;
using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Battle;
using GameServer.Domain.Statistics;
using GameServer.Infrastructure;

namespace Gameserver.Application.Services;

public sealed class EventServices(StatisticsTracker statisticsTracker, EntityService entityService, EntityStore entityStore)
{
    private readonly EntityStore _entities = entityStore;
    private readonly EntityService _service = entityService;
    private readonly StatisticsTracker _statistics = statisticsTracker;
    private BattleTracker CurrentBattle;

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
        CurrentBattle = new(_statistics, _entities, request.PartyId, request.OpponentPartyId ?? $"temp-{OrdinalDateString.GetOrdinalDate(4)}");
        var initiative = CurrentBattle.GetInitiativeOrder();
        return new BattleDto
        {
            EntityDtos = [..opponentParty, ..party],
            EntityResult = result,
            InitiativeOrder = initiative
        };
    }

    public BattleDto? EndBattle()
    {
        if (CurrentBattle is null)
        {
            return null;
        }
        var errors = CurrentBattle.OnBattleEnd();
        var result = new BattleDto
        {
            Error = errors is null ? string.Empty : string.Join("\n", errors),
            EntityDtos = _entities.GetParty(CurrentBattle.OpponentPartyId).ToDtos()
        };
    }
}