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
        bool result = false;
        if (party is not null)
        {
            foreach (var member in party)
            {
                result = !member.Inventory.Items.Any(i => i.Tag.Equals(_itemTag, StringComparison.InvariantCultureIgnoreCase)) || result;
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
public class SkillCheck(
    ITargetSelector targetSelector,
    Proficiency proficiency,
    int difficulty,
    string? prompt
): ICondition
{
    private readonly Random r = new();
    public EventResultDto IsMet(EventServices services)
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
                
                bool successful = r.Next((int)(100 / targetProficiency.Value)) <= difficulty_value;

                return new(
                    success: successful
                );
            }
        }
        return new(
            error: $"No targets found"
        );
    }
}