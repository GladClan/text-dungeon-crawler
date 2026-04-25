namespace GameServer.Contracts.Parsing;

public sealed class EnumDictionaryParseResult<TEnum>
    where TEnum : struct, Enum
{
    public Dictionary<TEnum, double>? Parsed { get; init; }

    public IReadOnlyList<ParseIssue> Errors { get; init; } = [];
}
