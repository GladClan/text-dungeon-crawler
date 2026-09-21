using GameServer.Application.Common;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Battle;
using GameServer.Domain.Map;

namespace GameServer.Application.Services;

public sealed class BattleService(
    GameContext context,
    StatisticsService statisticsService,
    EntityService entityService
)
{
    private readonly EntityService _service = entityService;
    private readonly StatisticsService _statistics = statisticsService;

    public BattleStartDto CommenceBattle(BattleStartRequest request)
    {
        if (
            request.OpponentPartyId.Length == 0
            && request.entityRequests.Count == 0
            && request.BestiaryEntityTags.Count == 0
        )
        {
            return new BattleStartDto
            {
                Error = "The party wins! No opponents found."
            };
        }

        List<DamageableEntityDto> opponentParty = _service.GetParty(request.OpponentPartyId);

        List<AddEntityResult> result = [];
        foreach (string tag in request.BestiaryEntityTags)
        {
            var target = _service.AddBeastiaryEntity(tag, request.OpponentPartyId);
            result.Add(target);
            if (target is not null && target.Errors.Count == 0 && target.Entity is not null)
            {
                opponentParty.Add(target.Entity);
            }
            else
            {
                Console.WriteLine($"Failed to add bestiary entity tag: [{tag}]");
                return new()
                {
                    Error = $"Entity tag could not be found: {tag}"
                };
            }
        }
        if (request.entityRequests is not null)
        {
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
        context.CurrentBattle = new(request.PartyId, request.OpponentPartyId ?? $"temp-{OrdinalDateString.GetOrdinalDate(4)}", _service);
        var initiative = GetInitiativeOrder()!;
        return new BattleStartDto
        {
            EntityDtos = [..opponentParty, ..party],
            EntityResult = result.Count == 0 ? null : result,
            InitiativeOrder = initiative
        };
    }

    public BattleEndDto? EndBattle()
    {
        if (context.CurrentBattle is null)
        {
            return null;
        }
        List<string> errors = OnBattleEnd() ?? [];

        // Add experience to entities and record level ups
        var enemyParty = _service.GetParty(context.CurrentBattle.OpponentPartyId);
        var party = _service.GetParty(context.CurrentBattle.PartyId);
        List<LevelUpDto> expGained = [];
        int partyExpGain = enemyParty.Sum(e => e.IsEntityAlive? 0 : e.Experience);
        foreach (var m in party)
        {
            if (m.IsEntityAlive)
            {
                var expGainResult = _service.AddExperience(m.Id, partyExpGain);
                if (expGainResult is null)
                {
                    errors.Add($"Party member {m.Id} seems to have disappeared after the battle... sorry about that.");
                }
                else
                {
                    expGained.Add(expGainResult);
                }
            }
        }

        int enemyExpGain = party.Sum(m => m.IsEntityAlive? 0 : m.Experience * m.Level);
        foreach (var e in enemyParty)
        {
            if (e.IsEntityAlive)
            {
                _ = _service.AddExperience(e.Id, enemyExpGain);
            }
        }
        var result = new BattleEndDto
        {
            Victory = party.Any(m => m.IsEntityAlive),
            Error = errors is null ? string.Empty : string.Join("\n", errors),
            Party = party,
            Opponents = enemyParty,
            LevelUps = expGained
        };

        _service.RemoveEntitiesNotAliveInParty(context.CurrentBattle.OpponentPartyId);
        context.CurrentBattle = null;
        return result;
    }

    public List<InitiativeDto>? GetInitiativeOrder()
    {
        if (context.CurrentBattle == null)
        {
            return null;
        }
        List<InitiativeDto> result = [];
        var party = _service.GetParty(context.CurrentBattle.PartyId);
        var enemies = _service.GetParty(context.CurrentBattle.OpponentPartyId);
        
        List<DamageableEntityDto> members = [.. party, .. enemies];
        members.Sort((a, b) => b.Speed.CompareTo(a.Speed));
        
        int maxTurns = members.Max(m => 1 + (int)(m.Speed / 20));

        for (int round = 0; round < maxTurns; round++)
        {
            foreach (var m in members)
            {
                if (round < 1 + (int)(m.Speed / 20))
                {
                    result.Add(
                        new()
                        {
                            Initiative = result.Count + 1,
                            EntityName = m.Name,
                            EntityId = m.Id
                        }
                    );
                }
            }
        }
        context.CurrentBattle.InitiativeOrder = result;
        return result;
    }

    public TurnoverDto? NextTurn()
    {
        if (context.CurrentBattle == null)
        {
            return null;
        }

        List<string> results = [];
        string error = "";
        var groups = context.CurrentBattle.GetBattleEffectsGroupedById();
        foreach (var g in groups)
        {
            var target = _service.GetDamageableEntityObject(g.Key);
            if (target is not null)
            {
                foreach(IBattleEffect b in g)
                {
                    if (!b.Apply(target))
                    {
                        context.CurrentBattle.RemoveBattleEffect(b);
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
        context.CurrentBattle.Turn++;
        if (context.CurrentBattle.Turn > context.CurrentBattle.InitiativeOrder.Count)
        {
            context.CurrentBattle.Turn = 0;
            context.CurrentBattle.Round++;
            context.CurrentBattle.InitiativeOrder = GetInitiativeOrder()!;
        }
        if (!_service.GetParty(context.CurrentBattle.PartyId).Any(e => e.IsEntityAlive) || !_service.GetParty(context.CurrentBattle.OpponentPartyId).Any(e => e.IsEntityAlive))
        {
            error += string.Join("\n", OnBattleEnd()!);
        }
        return new TurnoverDto
        {
            CurrentTurn = context.CurrentBattle.Turn,
            Messages = results,
            InitiativeOrder = context.CurrentBattle.InitiativeOrder,
            Error = error
        };
    }

    public List<string>? OnBattleEnd()
    {
        if (context.CurrentBattle == null)
        {
            return null;
        }
        List<string> errors = [];
        var groups = context.CurrentBattle.GetBattleEffectsGroupedById();
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
        _statistics.AddEntriesToStats(context.CurrentBattle.Log.GetAllEntries());
        return errors;
    }

    public bool? AddContinuousEffect(IBattleEffect battleEffect)
    {
        if (context.CurrentBattle == null)
        {
            return null;
        }
        return context.CurrentBattle.AddContinuousEffect(battleEffect);
    }

    public bool? RemoveAllContinuousEffects(string targetId)
    {
        if (context.CurrentBattle == null)
        {
            return null;
        }
        bool? result = null;
        var target = _service.GetDamageableEntityObject(targetId);
        if (target is not null)
        {
            var effects = context.CurrentBattle.GetAllEfectsForTarget(targetId);
            foreach (var effect in effects)
            {
                if (effect.EntityId.Equals(targetId, StringComparison.InvariantCultureIgnoreCase))
                {
                    effect.Revert(target);
                    context.CurrentBattle.RemoveBattleEffect(effect);
                    result = true;
                }
            }
        }
        return result;
    }

    public bool? RemoveContinuousEffect(string targetId, string effectTag)
    {
        if (context.CurrentBattle == null)
        {
            return null;
        }
        var effect = context.CurrentBattle.GetBattleEffect(targetId, effectTag);
        if (effect is null)
        {
            return false;
        }
        var target = _service.GetDamageableEntityObject(targetId);
        if (target is not null)
        {
            effect.Revert(target);
            context.CurrentBattle.RemoveBattleEffect(effect);
            return true;
        }
        return null;
    }
}