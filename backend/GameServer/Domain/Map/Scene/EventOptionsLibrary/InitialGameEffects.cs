using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Requests;
using GameServer.Domain.Enums;

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
                if (result.Value == -1)
                {
                    return new(
                        success: false
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


/// <summary>
/// 
/// </summary>
/// <param name="targetSelector">Selector used to find the target entity performing the skill check</param>
/// <param name="proficiency">Proficiency being checked</param>
/// <param name="difficulty">
/// 0 is trivial      ~90% success at 0.8 proficiency<br/>
/// 1 is easy         ~75% success at 1.0 proficiency<br/>
/// 2 is medium       ~50% success at 1.0 proficiency<br/>
/// 3 is hard         ~25% success at 1.0 proficiency<br/>
/// 4 is impossible   ~10% success at 1.2 proficiency<br/>
/// And so forth
/// </param>
public sealed class EffectSkillCheck(
    ITargetSelector targetSelector,
    Proficiency proficiency,
    int difficulty,
    string? prompt
) : IGameEffect
{
    private static readonly Random r = new();
    public EventResultDto Apply(EventServices services)
    {
        
        var targets = targetSelector.GetTargets(services);
        string? targetId = services.GetPendingResponse();

        if (targets.Count > 0)
        {
            if (targetId is null)
            {
                Console.WriteLine($"Skillcheck: Finding target...");
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
                Console.WriteLine($"Skill check: target found!");
                _ = services.CombatService.AddProficiencyEntry(new()
                {
                    TargetId = targetId,
                    Proficiency = proficiency.ToString()
                });

                var targetProficiency = services.CombatService.GetProficiencyMultiplier(targetId, proficiency.ToString());

                if (targetProficiency is null || targetProficiency.Error.Length > 0)
                {
                    return new(
                        false
                    );
                }

                double difficulty_value = difficulty switch
                {
                    0 => 90d/0.8d,
                    1 => 75d,
                    2 => 50d,
                    3 => 25d,
                    4 => 10d/1.2d,
                    _ => difficulty > 4 ?
                        10 / (difficulty - 4) / (1 + (10 / (difficulty - 3))) :
                        difficulty < -7 ?
                        1001 :
                        90d / (1 + ((-2 + difficulty) / 10))                    
                };

                Console.WriteLine($"Skill check: difficulty set to {difficulty_value}, proficiency multiplier is {targetProficiency.Value}");
                double check = r.Next((int)(100 / targetProficiency.Value));
                bool successful = check <= difficulty_value;

                Console.WriteLine($"Skill check success: {check} < {difficulty_value}");

                Console.WriteLine($"Skill check {(successful ? "succeeded" : "failed")}");

                services.GetCurrentSceneState().SkillCheckSuccess = successful;

                return new(
                    success: true
                );
            }
        }
        return new(
            error: $"No targets found"
        );
    }
}
