namespace GameServer.Contracts.DTOs;

public sealed class ResistanceDto
{
    public ResistanceDto(){}
    public ResistanceDto(string resistance, double value)
    {
        Resistance = resistance;
        Value = value;
    }
    public string Resistance { get; init; } = string.Empty;
    public double Value { get; init; }
}

public sealed class DamageResultDto
{
    public DamageResultDto()
    {
        Error = string.Empty;
    }
    public DamageResultDto(double sent, double actual, double result, double blocked, bool fatal)
    {
        DamageSent = sent;
        DamageActual = actual;
        ResultingHealth = result;
        DamageDifference = blocked;
        Fatal = fatal;
        Error = string.Empty;
    }
    public DamageResultDto(double sent, double actual, string error)
    {
        DamageSent = sent;
        DamageActual = actual;
        Error = error;
    }
    public double DamageSent { get; init; }
    public double DamageActual { get; init; }
    public double ResultingHealth { get; init; }
    public double DamageDifference { get; init; }
    public bool Fatal { get; init; }
    public string Error { get; init; }
}

public sealed class HealResultDto
{
    public HealResultDto()
    {
        Error = string.Empty;
    }
    public HealResultDto(double sent, double actual, double result, bool fatal = false)
    {
        AmountSent = sent;
        AmountActual = actual;
        NewHealth = result;
        Fatality = fatal;
        Error = string.Empty;
    }
    public HealResultDto(double sent, string error)
    {
        AmountSent = sent;
        AmountActual = 0;
        Error = error;
    }
    public double AmountSent { get; init; }
    public double AmountActual { get; init; }
    public double NewHealth { get; init; }
    public bool Fatality { get; init; }
    public string Error { get; init; }
}

public  sealed class ManaChangeDto
{
    public ManaChangeDto()
    {
        Error = string.Empty;
    }
    public ManaChangeDto(double amountSent, string error)
    {
        AmountSent = amountSent;
        Error = error;
    }
    public ManaChangeDto(double amountSent, double amountActual, double newMana)
    {
        AmountSent = amountSent;
        AmountActual = amountActual;
        NewMana = newMana;
        Error = string.Empty;
    }
        public double AmountSent { get; init; }
        public double AmountActual { get; init; }
        public double NewMana { get; init; }
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