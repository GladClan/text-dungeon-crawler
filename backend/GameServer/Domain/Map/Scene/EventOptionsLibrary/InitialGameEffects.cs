using GameServer.Contracts.DTOs;
using GameServer.Contracts.Requests;

namespace GameServer.Domain.Map.Scene.EventOptionsLibrary;

public class ChangeOptionTitle(EventOption o, string _newTitle): IGameEffect
{
    public EffectDto Apply(IGameContext context)
    {
        o.Title = _newTitle;
        return new();
    }
}

public class GiveItem(
    string _targetDamageableEntityId,
    string _itemTag
): IGameEffect
{
    public EffectDto Apply(IGameContext context)
    {
        var result = context.InventoryService.AddItemByTag(_targetDamageableEntityId, _itemTag);
        if (result is null)
        {
            return new(
                error: $"Could not find entity {_targetDamageableEntityId}"
            );
        }
        if (result.Error.Length > 0)
        {
            return new(
                error: result.Error
            );
        }
        return new(
            message: $"You received a {result.Name}",
            results: [],
            wasMagic: false
        );
    }
}

public class GiveItemsArray(
    string _targetDamageableEntityId,
    List<string> _itemTags
): IGameEffect
{
    public EffectDto Apply(IGameContext context)
    {
        List<ItemDto> results = [];
        foreach (string tag in _itemTags)
        {
            var result = context.InventoryService.AddItemByTag(_targetDamageableEntityId, tag);
            if (result is null)
            {
                return new(
                    error: $"Could not find entity {_targetDamageableEntityId}"
                );
            }
            if (result.Error.Length > 0)
            {
                return new(
                    error: result.Error
                );
            }
            results.Add(result);
        }
        return new(
            message: $"You received {string.Join(",", results.Select(r => r.Name))}",
            results: [],
            wasMagic: false
        );
    }
}

public class AddMemberToPartyById(
    string _newMemberId,
    string _partyId
) : IGameEffect
{
    public EffectDto Apply(IGameContext context)
    {
        var result = context.EntityService.ChangeParty(_newMemberId, _partyId);
        if (result is null)
        {
            return new(
                error: $"Could not find entity {_newMemberId}"
            );
        }
        return new(
            message: $"{result.Name} joins the party!",
            results: [],
            wasMagic: false
        );
    }
}

public class AddMemberToPartyRequest(
    DamageableEntityRequest entityRequest,
    string _partyId
): IGameEffect
{
    public EffectDto Apply(IGameContext context)
    {
        var addEntityResult = context.EntityService.AddEntityFromRequest(entityRequest);
        if (addEntityResult.Entity is null)
        {
            return new(
                error: $"There was an issue adding the new entity: {string.Join("\n", addEntityResult.Errors)}"
            );
        }
        var result = context.EntityService.ChangeParty(addEntityResult.Entity.Id, _partyId);
        if (result is null)
        {
            return new(
                error: "We lost the new entity somewhere in transit..."
            );
        }
        return new(
            message: result.Id,
            results: [],
            wasMagic: false
        );
    }
}

public class RemoveMemberFromParty(
    string _partyMemberId,
    string _nullPartyId
) : IGameEffect
{
    public EffectDto Apply(IGameContext context)
    {
        var result = context.EntityService.ChangeParty(_partyMemberId, _nullPartyId);
        if (result is null)
        {
            return new(
                error: $"Could not find entity {_partyMemberId}"
            );
        }
        return new(
            message: $"{result.Name} leaves the party.",
            results: [],
            wasMagic: false
        );
    }
}

public class AddOrRemoveGold(
    string _targetDamageableEntityId,
    int _goldToAdd
): IGameEffect
{
    public EffectDto Apply(IGameContext context)
    {
        var result = context.InventoryService.AddGold(_targetDamageableEntityId, _goldToAdd);
        if (result is null)
        {
            return new(
                error: $"Could not find entity {_targetDamageableEntityId}"
            );
        }
        return new(
            message: $"You gained {result} gold!",
            results: [],
            wasMagic: false
        );
    }
}

public class StartBattleEffect(
    string battleStartMessage,
    BattleStartRequest battleStartRequest
) : IGameEffect
{
    public EffectDto Apply(IGameContext context)
    {
        var result = context.BattleService.CommenceBattle(battleStartRequest);
        if (result.Error.Length > 0)
        {
            return new(
                error: result.Error
            );
        }
        return new(
            message: battleStartMessage,
            results: [],
            wasMagic: false
        );
    }
}


/*
DoorIsOpen = true
GuardsAlerted = true
GiveGold(random(10, 20) * random(9, 20))
*/