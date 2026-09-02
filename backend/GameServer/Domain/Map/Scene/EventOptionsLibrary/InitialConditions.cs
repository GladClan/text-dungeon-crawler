using GameServer.Domain.Enums;

namespace GameServer.Domain.Map.Scene.EventOptionsLibrary;

public class HasItem(
    string _partyId,
    string _itemTag
) : ICondition
{
    public bool IsMet(IGameContext context)
    {
        var party = context.EntityService.GetParty(_partyId);
        bool result = false;
        if (party is not null)
        {
            foreach (var member in party)
            {
                result = member.Inventory.Items.Any(i => i.Tag.Equals(_itemTag, StringComparison.InvariantCultureIgnoreCase)) || result;
            }
        }
        return result;
    }
}

public class NotHasItem(
    string _partyId,
    string _itemTag
) : ICondition
{
    public bool IsMet(IGameContext context)
    {
        var party = context.EntityService.GetParty(_partyId);
        bool result = false;
        if (party is not null)
        {
            foreach (var member in party)
            {
                result = !member.Inventory.Items.Any(i => i.Tag.Equals(_itemTag, StringComparison.InvariantCultureIgnoreCase)) || result;
            }
        }
        return result;
    }
}

public class MemberInParty(
    string _partyId,
    string _targetPartyMemberId
): ICondition
{
    public bool IsMet(IGameContext context)
    {
        var party = context.EntityService.GetParty(_partyId);
        if (party is not null)
        {
            return party.Any(m => m.Id.Equals(_targetPartyMemberId, StringComparison.InvariantCultureIgnoreCase));
        }
        return false;
    }
}

public class NotMemberInParty(
    string _partyId,
    string _targetPartyMemberId
): ICondition
{
    public bool IsMet(IGameContext context)
    {
        var party = context.EntityService.GetParty(_partyId);
        if (party is not null)
        {
            return !party.Any(m => m.Id.Equals(_targetPartyMemberId, StringComparison.InvariantCultureIgnoreCase));
        }
        return false;
    }
}

public class HasGold(
    string _partyId,
    int _targetGoldAmount
): ICondition
{
    public bool IsMet(IGameContext context)
    {
        var party = context.EntityService.GetParty(_partyId);
        int partyGold = 0;
        if (party is not null)
        {
            foreach (var m in party)
            {
                partyGold += m.Inventory.Gold;
            }
        }
        return partyGold > _targetGoldAmount;
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="_targetId">ID of the target entity performing the skill check</param>
/// <param name="proficiency">Proficiency being checked</param>
/// <param name="_difficulty">
/// 0 is trivial      ~90% success at 0.8 proficiency<br/>
/// 1 is easy         ~75% success at 1.0 proficiency<br/>
/// 2 is medium       ~50% success at 1.0 proficiency<br/>
/// 3 is hard         ~25% success at 1.0 proficiency<br/>
/// 4 is impossible   ~10% success at 1.2 proficiency<br/>
/// And so forth
/// </param>
public class SkillCheck(
    string _targetId,
    Proficiency proficiency,
    int _difficulty
): ICondition
{
    private readonly Random r = new();
    public bool IsMet(IGameContext context)
    {
        _ = context.CombatService.AddProficiencyEntry(new()
        {
            TargetId = _targetId,
            Proficiency = proficiency.ToString(),
            Amount = 1
        });

        var targetProficiency = context.CombatService.GetProficiencyMultiplier(_targetId, proficiency.ToString());

        if (targetProficiency is null || targetProficiency.Error.Length > 0)
        {
            return false;
        }
        double difficulty_value = _difficulty switch
        {
            0 => 90d/0.8d,
            1 => 75d,
            2 => 50d,
            3 => 25d,
            4 => 10d/1.2d,
            _ => _difficulty > 4 ?
                10 / (_difficulty - 4) / (1 + (10 / (_difficulty - 3))) :
                _difficulty < -7 ?
                1001 :
                90d / (1 + ((-2 + _difficulty) / 10))
        };

        return r.Next((int)(100 / targetProficiency.Value)) <= difficulty_value;
    }
}