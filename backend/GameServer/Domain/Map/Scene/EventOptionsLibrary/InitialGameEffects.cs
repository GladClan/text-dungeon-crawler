using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Requests;

namespace GameServer.Domain.Map.Scene.EventOptionsLibrary;

public class ChangeOptionTitle(EventOption o, string _newTitle): IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        o.Title = _newTitle;
        return new(
            success: true
        );
    }
}

public class GiveItem(
    ITargetSelector targetSelector,
    string itemTag,
    string? prompt
): IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        var targets = targetSelector.GetTargets(services);

        string? targetId = services.GetPendingResponse();

        if (targets.Count > 0)
        {
            if (targetId is null)
            {
                if (targets.Count == 1)
                {
                    targetId = targets[0].EntityId;
                }
                else
                {
                    return new(
                        success: true,
                        requiresTarget: true,
                        prompt: prompt ?? "",
                        targets: [..targets]
                    );
                }
            }
            else
            {
                if (targets.Any(t => t.EntityId.Equals(targetId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var giveItemResult = services.InventoryService.AddItemByTag(targetId, itemTag);
                    if (giveItemResult is null)
                    {
                        return new(
                            error: $"Could not find the target entity, ID: {targetId}"
                        );
                    }
                    if (giveItemResult.Error.Length > 0)
                    {
                        return new(
                            error: giveItemResult.Error
                        );
                    }
                    return new(
                        success: true
                    );
                }
            }
        }

        return new(
            success: false
        );
    }
}

public class GiveItemsArray(
    ITargetSelector targetSelector,
    List<string> _itemTags,
    string? prompt
): IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        var targets = targetSelector.GetTargets(services);

        string? targetId = services.GetPendingResponse();

        if (targets.Count > 0)
        {
            if (targetId is null)
            {
                if (targets.Count == 1)
                {
                    targetId = targets[0].EntityId;
                }
                else
                {
                    return new(
                        success: false,
                        requiresTarget: true,
                        prompt: prompt ?? "",
                        targets: [..targets]
                    );
                }
            }
            if (targetId is not null)
            {
                if (targets.Any(t => t.EntityId.Equals(targetId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    string errors = "";
                    foreach (string tag in _itemTags)
                    {
                        var giveItemResult = services.InventoryService.AddItemByTag(targetId, tag);
                        if (giveItemResult is null)
                        {
                            return new(
                                error: $"Could not find the target entity, ID: {targetId}"
                            );
                        }
                        _itemTags.RemoveAt(0);
                        if (giveItemResult.Error.Length > 0)
                        {
                            return new(
                                error: giveItemResult.Error
                            );
                        }
                    }

                    if (errors.Length == 0)
                    {
                        return new(
                            success: true
                        );
                    }
                    else
                    {
                        return new(
                            error: errors
                        );
                    }
                }
            }
        }

        return new(
            success: false
        );
    }
}

public class AddMemberToPartyById(
    string _newMemberId,
    string _partyId
) : IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        var result = services.EntityService.ChangeParty(_newMemberId, _partyId);
        if (result is null)
        {
            return new(
                error: $"Could not find entity {_newMemberId}"
            );
        }
        return new(
            success: true
        );
    }
}

public class AddMemberToPartyRequest(
    DamageableEntityRequest entityRequest,
    string _partyId
): IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        var addEntityResult = services.EntityService.AddEntityFromRequest(entityRequest);
        if (addEntityResult.Entity is null)
        {
            return new(
                error: $"There was an issue adding the new entity: {string.Join("\n", addEntityResult.Errors)}"
            );
        }
        var result = services.EntityService.ChangeParty(addEntityResult.Entity.Id, _partyId);
        if (result is null)
        {
            return new(
                error: "We lost the new entity somewhere in transit..."
            );
        }
        return new(
            success: true
        );
    }
}

public class RemoveMemberFromParty(
    string _partyMemberId,
    string _nullPartyId
) : IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        var result = services.EntityService.ChangeParty(_partyMemberId, _nullPartyId);
        if (result is null)
        {
            return new(
                error: $"Could not find entity {_partyMemberId}"
            );
        }
        return new(
            success: true
        );
    }
}

public class AddOrRemoveGold(
    ITargetSelector targetSelector,
    int _goldToAdd,
    string? prompt
): IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        var targets = targetSelector.GetTargets(services);

        string? targetId = services.GetPendingResponse();

        if (targets.Count > 0)
        {
            if (targetId is null)
            {
                if (targets.Count == 1)
                {
                    targetId = targets[0].EntityId;
                }
                else
                {
                    return new(
                        success: false,
                        requiresTarget: true,
                        prompt: prompt ?? "",
                        targets: [..targets]
                    );
                }
            }
            if (targetId is not null)
            {
                var result = services.InventoryService.AddGold(targetId, _goldToAdd);
                if (result is null)
                {
                    return new(
                        error: $"Could not find entity {targetId}"
                    );
                }
                return new(
                    success: true
                );
            }
        }

        return new(
            success: false
        );
    }
}

public class StartBattleEffect(
    BattleStartRequest battleStartRequest
) : IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        var result = services.BattleService.CommenceBattle(battleStartRequest);
        if (result.Error.Length > 0)
        {
            return new(
                error: result.Error
            );
        }
        return new(
            success: true
        );
    }
}


/*
GiveGold(random(10, 20) * random(9, 20))
*/