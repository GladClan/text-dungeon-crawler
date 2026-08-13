using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Entities;
using GameServer.Domain.Statistics;

namespace GameServer.Domain.Battle;

public class BattleTracker(string partyId, string opponentPartyId, EntityService entityService)
{
    private readonly EntityService _service = entityService;
    public string PartyId { get; set; } = partyId;
    public string OpponentPartyId { get; set; } = opponentPartyId;
    public readonly BattleLog Log = new();
    private readonly List<IBattleEffect> _battleEffects = [];
    public List<InitiativeDto> InitiativeOrder = [];
    public int Turn = 0;
    public int Round = 0;

    public IEnumerable<IGrouping<string, IBattleEffect>> GetBattleEffectsGroupedById()
    {
        return _battleEffects.GroupBy(e => e.EntityId);
    }

    public bool RemoveBattleEffect(IBattleEffect b)
    {
        return _battleEffects.Remove(b);
    }

    public bool EntityHasBattleEffect(string targetId, string tag)
    {
        return _battleEffects.Any(
                e => e.Tag.Equals(tag, StringComparison.InvariantCultureIgnoreCase) &&
                e.EntityId.Equals(targetId, StringComparison.InvariantCultureIgnoreCase)
        );
    }

    public IBattleEffect? GetBattleEffect(string targetId, string tag)
    {
        return _battleEffects.FirstOrDefault(e =>
            e.Tag.Equals(tag, StringComparison.InvariantCultureIgnoreCase) &&
            e.EntityId.Equals(targetId, StringComparison.InvariantCultureIgnoreCase));
    }

    public bool AddContinuousEffect(IBattleEffect effect)
    {
        if (!EntityHasBattleEffect(effect.EntityId, effect.Tag))
        {
            _battleEffects.Add(effect);
            return true;
        }
        return false;
    }

    public List<IBattleEffect> GetAllEfectsForTarget(string id)
    {
        return [.. _battleEffects.Where(e => e.EntityId.Equals(id, StringComparison.InvariantCultureIgnoreCase))];
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

    public List<string> GetPartyIds(string partyId)
    {
        return [.._service.GetParty(partyId).Select(e => e.PartyId)];
    }

    public AddEntityResult AddEntityToBattle(DamageableEntityRequest request)
    {
        return _service.AddEntityFromRequest(request);
    }

    public bool AddLogEntry(EffectDto request)
    {
        return Log.AddNewEntries(request);
    }
}