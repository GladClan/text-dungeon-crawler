using System.ComponentModel.DataAnnotations;

namespace GameServer.Contracts.Requests;

public sealed class LearnSkillByIdRequest
{
    /// <summary>
    /// Use a catalog id from your skill registry; resolve it to a domain Skill in your service layer.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string SkillId { get; init; } = string.Empty;
}