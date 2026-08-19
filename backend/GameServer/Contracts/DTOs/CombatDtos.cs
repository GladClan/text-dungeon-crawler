
namespace GameServer.Contracts.DTOs;

public sealed class ProficiencyDto
{
    public ProficiencyDto()
    {
        Proficiency = string.Empty;
        Error = string.Empty;
    }
    public ProficiencyDto(string proficiency, double value)
    {
        Proficiency = proficiency;
        Value = value;
        Error = string.Empty;
    }
    public ProficiencyDto(string error)
    {
        Error = error;
        Proficiency = string.Empty;
    }
    public string Proficiency { get; init; } = string.Empty;
    public double Value { get; init; }
    public string Error { get; init; }
}

public sealed class ResistanceDto
{
    public ResistanceDto()
    {
        Resistance = string.Empty;
        Error = string.Empty;
    }
    public ResistanceDto(string resistance, double value)
    {
        Resistance = resistance;
        Value = value;
        Error = string.Empty;
    }
    public ResistanceDto(string error)
    {
        Resistance = string.Empty;
        Error = error;
    }
    public string Resistance { get; init; } = string.Empty;
    public double Value { get; init; }
    public string Error { get; init; }
}

public sealed class DamageResultDto
{
    public DamageResultDto()
    {
        SourceId = string.Empty;
        TargetId = string.Empty;
        Error = string.Empty;
    }
    public DamageResultDto(string sourceId, string targetId, int actionType, double sent, double actual, double result, bool fatal)
    {
        SourceId = sourceId;
        TargetId = targetId;
        ActionType = actionType;
        AmountSent = sent;
        AmountActual = actual;
        NewValue = result;
        Fatal = fatal;
        Error = string.Empty;
    }
    public DamageResultDto(double sent, string error)
    {
        SourceId = string.Empty;
        TargetId = string.Empty;
        AmountSent = sent;
        Error = error;
    }
    public string SourceId { get; init; }
    public string TargetId { get; init; }
    public int ActionType { get; init; }
    public double AmountSent { get; init; }
    public double AmountActual { get; init; }
    public double NewValue { get; init; }
    public bool Fatal { get; init; }
    public string Error { get; init; }
}

public sealed class LevelUpDto()
{
    public int ExpAtStart { get; init; }
    public int ExpAfter { get; init; }
    public int LevelAtStart { get; set; }
    public int LevelAfter { get; set; }
    public Dictionary<string, double> ProficienciesAtStart { get; set; } = [];
    public Dictionary<string, double> ProficienciesAfter { get; init; } = [];
}

public sealed class StringDoubleDto(string key, double value, string error = "")
{
    public string Key { get; set; } = key;
    public double Value { get; set; } = value;
    public string Error = error;
}

public sealed class EffectDto
{
    public EffectDto()
    {
        Message = string.Empty;
        Error = string.Empty;
    }

    public EffectDto(string message, List<DamageResultDto> results, bool wasMagic)
    {
        Message = message;
        Results = results;
        Error = string.Empty;
        WasMagic = wasMagic;
    }

    public EffectDto(string error)
    {
        Message = string.Empty;
        Error = error;
    }
    public string Message { get; init; }
    public List<DamageResultDto>? Results { get; init; }
    public bool WasMagic { get; init; }
    public string Error { get; set; }
}