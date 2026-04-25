using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Enums;

namespace GameServer.Application.Common;

public sealed class ResistanceParser
{
    public EnumDictionaryParseResult<DamageType> Parse(List<ResistanceRequest>? entries)
    {
        if (entries is null)
        {
            return new EnumDictionaryParseResult<DamageType>
            {
                Parsed = null,
                Errors = []
            };
        }

        Dictionary<DamageType, double> parsed = [];
        List<ParseIssue> errors = [];

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];

            if (!Enum.TryParse<DamageType>(entry.Type, true, out var damageType))
            {
                errors.Add(new ParseIssue(
                    $"Resistances[{i}].Type",
                    $"'{entry.Type}' is not a valid {nameof(DamageType)} value."));
                continue;
            }

            parsed[damageType] = entry.Value;
        }

        return new EnumDictionaryParseResult<DamageType>
        {
            Parsed = parsed,
            Errors = errors
        };
    }
}
