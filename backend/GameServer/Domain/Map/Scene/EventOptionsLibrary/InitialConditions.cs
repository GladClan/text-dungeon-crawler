using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Map.Scene.EventOptionsLibrary;

public class HasItem(
    string _partyId,
    string _itemTag
) : ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        var party = services.EntityService.GetParty(_partyId);
        if (party is not null)
        {
            bool result = false;
            foreach (var member in party)
            {
                result = member.Inventory.Items.Any(i => i.Tag.Equals(_itemTag, StringComparison.InvariantCultureIgnoreCase)) || result;
            }
            return new(
                success: result
            );
        }
        return new(
            error: $"Party not found with id = {_partyId}"
        );
    }
}

public class NotHasItem(
    string _partyId,
    string _itemTag
) : ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        var party = services.EntityService.GetParty(_partyId);
        if (party is not null)
        {
            bool result = true;
            foreach (var member in party)
            {
                result = !member.Inventory.Items.Any(i => i.Tag.Equals(_itemTag, StringComparison.InvariantCultureIgnoreCase)) && result;
            }
            return new(
                success: result
            );
        }
        return new(
            error: $"Party not found with id = {_partyId}"
        );
    }
}

public class MemberInParty(
    string _partyId,
    string _targetPartyMemberId
): ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        var party = services.EntityService.GetParty(_partyId);
        if (party is not null)
        {
            bool result = party.Any(m => m.Id.Equals(_targetPartyMemberId, StringComparison.InvariantCultureIgnoreCase));
            return new(
                success: result
            );
        }
    return new(
        error: $"Party not found with id = {_partyId}"
    );
    }
}

public class NotMemberInParty(
    string _partyId,
    string _targetPartyMemberId
): ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        var party = services.EntityService.GetParty(_partyId);
        bool result = false;
        if (party is not null)
        {
            result = !party.Any(m => m.Id.Equals(_targetPartyMemberId, StringComparison.InvariantCultureIgnoreCase));
            return new(
                success: result
            );
        }
        return new(
            error: $"Party not found with id = {_partyId}"
        );
    }
}

public class HasGold(
    string _partyId,
    int _targetGoldAmount
): ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        var party = services.EntityService.GetParty(_partyId);
        int partyGold = 0;
        if (party is not null)
        {
            foreach (var m in party)
            {
                partyGold += m.Inventory.Gold;
            }
            return new(
                success: partyGold > _targetGoldAmount
            );
        }
        return new(
            error: $"Party not found with id = {_partyId}"
        );
    }
}

public class ConditionSkillCheckSuccess : ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        return new(
            success: services.GetCurrentSceneState().SkillCheckSuccess
        );
    }
}