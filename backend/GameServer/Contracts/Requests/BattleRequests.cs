using System.ComponentModel.DataAnnotations;

namespace GameServer.Contracts.Requests;

public sealed class BattleStartRequest
{
    [Required]
    [MinLength(1)]
    public string PartyId { get; init; } = string.Empty;

    public string OpponentPartyId { get; init; } = string.Empty;

    public List<DamageableEntityRequest> entityRequests = [];
}