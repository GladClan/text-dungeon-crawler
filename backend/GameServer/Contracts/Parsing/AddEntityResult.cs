using GameServer.Contracts.DTOs;

namespace GameServer.Contracts.Parsing;

public sealed class AddEntityResult
{
    public DamageableEntityDto? Entity { get; set; } = null;

    public List<ParseIssue> Errors { get; init; } = [];

    public void AddErrors(IEnumerable<ParseIssue> errors)
    {
        foreach (ParseIssue err in errors)
        {
            Errors.Add(err);
        }
    }

    public bool IsValid()
    {
        return Errors.Count == 0;
    }
}
