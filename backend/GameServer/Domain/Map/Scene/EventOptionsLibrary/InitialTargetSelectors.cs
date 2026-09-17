using GameServer.Application.Services;

namespace GameServer.Domain.Map.Scene.EventOptionsLibrary;

// PartyMemberSelector
// LivingPartyMemberSelector
// DeadPartyMemberSelector
// EnemySelector
// AllPartyMembersSelector
// RandomPartyMemberSelector
// SpecificEntitySelector

public class SpecificEntitySelector(string entityId) : ITargetSelector
{
    public IReadOnlyList<TargetOption> GetTargets(EventServices services)
    {
        var entity = services.EntityService.GetById(entityId);

        if (entity == null)
            return [];

        return
        [
            new TargetOption(entity.Id, entity.Name)
        ];
    }
}

public class PartyMemberSelector(string partyId) : ITargetSelector
{
    public IReadOnlyList<TargetOption> GetTargets(EventServices services)
    {
        return 
        [..
            services.EntityService.GetParty(partyId)
                .Select(entity => new TargetOption(
                    entity.Id,
                    entity.Name))
        ];
    }
}

public class LivingPartyMemberSelector(string partyId) : ITargetSelector
{
    public IReadOnlyList<TargetOption> GetTargets(EventServices services)
    {
        return [..
            services.EntityService.GetParty(partyId)
                .Where(entity => entity.IsEntityAlive)
                .Select(entity => new TargetOption(
                    entity.Id,
                    entity.Name))
        ];
    }
}

public class PartyMemberWithoutItemSelector(string partyId, string itemTag) : ITargetSelector
{

    public IReadOnlyList<TargetOption> GetTargets(EventServices services)
    {
        return [..    
            services.EntityService.GetParty(partyId)
                .Where(entity => 
                    !entity.Inventory.Items.Any(i => i.Tag.Equals(itemTag, StringComparison.InvariantCultureIgnoreCase)))
                .Select(entity => new TargetOption(
                    entity.Id,
                    entity.Name))
        ];
    }
}