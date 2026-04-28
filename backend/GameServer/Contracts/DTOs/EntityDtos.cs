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