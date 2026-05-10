
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
        Error = string.Empty;
    }
    public DamageResultDto(int damage_healing_mana, double sent, double actual, double result, bool fatal)
    {
        Damage_Healing_Mana = damage_healing_mana;
        AmountSent = sent;
        AmountActual = actual;
        NewValue = result;
        Fatal = fatal;
        Error = string.Empty;
    }
    public DamageResultDto(double sent, string error)
    {
        AmountSent = sent;
        Error = error;
    }
    public int Damage_Healing_Mana { get; init; }
    public double AmountSent { get; init; }
    public double AmountActual { get; init; }
    public double NewValue { get; init; }
    public bool Fatal { get; init; }
    public string Error { get; init; }
}

public sealed class LevelUpDto(int levelAtStart = 0, int levelAfter = 0, Dictionary<string, double>? proficienciesAtStart = null)
{
    public int LevelAtStart { get; set; } = levelAtStart;
    public int LevelAfter { get; set; } = levelAfter;
    public Dictionary<string, double> ProficienciesAtStart { get; set; } = proficienciesAtStart ?? [];
    
    public Dictionary<string, double> ProficienciesAfter { get; set; } = [];
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

    public EffectDto(string message, DamageResultDto result)
    {
        Message = message;
        Result = result;
        Error = string.Empty;
    }

    public EffectDto(string error)
    {
        Message = string.Empty;
        Error = error;
    }
    public string Message { get; init; }
    public DamageResultDto? Result { get; init; }
    public string Error { get; init; }
}