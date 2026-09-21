using System.ComponentModel.DataAnnotations;

namespace GameServer.Contracts.Requests;

public sealed class BattleStartRequest(
    string partyId,
    string opponentPartyId,
    List<string>? bestiaryEntityTags = null,
    List<DamageableEntityRequest>? damageableEntityRequests = null
    )
{
    [Required]
    [MinLength(1)]
    public string PartyId { get; init; } = partyId;
    public string OpponentPartyId { get; init; } = opponentPartyId;
    public List<string> BestiaryEntityTags { get; init; } = bestiaryEntityTags ?? [];
    public List<DamageableEntityRequest> entityRequests { get; init; } = damageableEntityRequests ?? [];
}