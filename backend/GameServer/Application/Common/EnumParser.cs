using GameServer.Contracts.Parsing;
using GameServer.Contracts.Requests;
using GameServer.Domain.Enums;

namespace GameServer.Application.Common;

public static class EnumParser
{
    // Parse a list of damage type requests
    public static EnumListParseResult<DamageType> Parse(this List<string> entries)
    {
        if (entries is null)
        {
            return new EnumListParseResult<DamageType>
            {
                Parsed = [],
                Errors = []
            };
        }

        List<DamageType> parsed = [];
        List<ParseIssue> errors = [];
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];

            if (!Enum.TryParse<DamageType>(entry, true, out var dtEnum))
            {
                errors.Add(new ParseIssue(
                    $"AttackType[i]",
                    $"{entry} is not a valid {nameof(DamageType)} value."
                ));
                continue;
            }

            parsed.Add(dtEnum);
        }

        return new EnumListParseResult<DamageType>
        {
            Parsed = parsed,
            Errors = errors
        };
    }

    // Parse list of proficiency requests
    public static EnumDictionaryParseResult<Proficiency> Parse(this List<ProficiencyRequest>? entries)
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

    // Parse a list of resistance requests
    public static EnumDictionaryParseResult<DamageType> Parse(this List<ResistanceRequest>? entries)
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