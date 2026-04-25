using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Enums;

namespace GameServer.Application.Common;

public sealed class ProficiencyParser
{
    public EnumDictionaryParseResult<Proficiency> Parse(List<ProficiencyRequest>? entries)
    {
        if (entries is null)
        {
            return new EnumDictionaryParseResult<Proficiency>
            {
                Parsed = null,
                Errors = []
            };
        }

        Dictionary<Proficiency, double> parsed = [];
        List<ParseIssue> errors = [];

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];

            if (!Enum.TryParse<Proficiency>(entry.Type, true, out var proficiency))
            {
                errors.Add(new ParseIssue(
                    $"Proficiencies[{i}].Type",
                    $"'{entry.Type}' is not a valid {nameof(Proficiency)} value."));
                continue;
            }

            parsed[proficiency] = entry.Value;
        }

        return new EnumDictionaryParseResult<Proficiency>
        {
            Parsed = parsed,
            Errors = errors
        };
    }
}
