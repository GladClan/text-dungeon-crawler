using GameServer.Contracts.DTOs;
using GameServer.Domain.Enums;
using GameServer.Domain.Statistics;

namespace GameServer.Domain.Battle;

public class BattleLog()
{
    public readonly List<BattleLogEntry> Entries = [];
    private readonly List<string> _damageableEntityIds = [];

    public bool AddEntry(EffectDto request)
    {
        if (request.Result is null)
        {
            return false;
        }
        if (!_damageableEntityIds.Contains(request.SourceId))
        {
            _damageableEntityIds.Add(request.SourceId);
        }
        if (!_damageableEntityIds.Contains(request.TargetId))
        {
            _damageableEntityIds.Add(request.TargetId);
        }
        Entries.Add(new BattleLogEntry
        {
            Damage_Healing_Mana = (Damage_Healing_Mana)request.Result.Damage_Healing_Mana,
            AmountSent = request.Result.AmountSent,
            AmountActual = request.Result.AmountActual,
            NewValue = request.Result.NewValue,
            Fatal = request.Result.Fatal,
            WasMagic = request.WasMagic,
            SourceId = request.SourceId,
            TargetId = request.TargetId,
        });
        return true;
    }

    public List<BattleLogEntry> GetEntriesBySource(string id)
    {
        return [.. Entries.Where(e => e.SourceId.Equals(id, StringComparison.InvariantCultureIgnoreCase))];
    }

    public string? GetHighestSingleDamage()
    {
        string? result = null;
        double max = double.MinValue;
        foreach (BattleLogEntry e in Entries)
        {
            if (e.Damage_Healing_Mana == Damage_Healing_Mana.Damage && e.AmountActual > max)
            {
                max = e.AmountActual;
                result = e.SourceId;
            }
        }
        return result;
    }

    public string? GetHighestDamageSent()
    {
        string? result = null;
        double max = double.MinValue;
        foreach (BattleLogEntry e in Entries)
        {
            if (e.Damage_Healing_Mana == Damage_Healing_Mana.Damage && e.AmountSent > max)
            {
                max = e.AmountSent;
                result = e.SourceId;
            }
        }
        return result;
    }

    public string? GetMostDamage()
    {
        Dictionary<string, double> frequencyMap = [];
        foreach (BattleLogEntry e in Entries)
        {
            if (e.Damage_Healing_Mana == Damage_Healing_Mana.Damage)
            {
                if (frequencyMap.TryGetValue(e.SourceId, out _))
                {
                    frequencyMap[e.SourceId] += e.AmountActual;
                }
                else
                {
                    frequencyMap[e.SourceId] = e.AmountActual;
                }
            }
        }
        double max = double.MinValue;
        string? result = null;
        foreach (var f in frequencyMap)
        {
            if (f.Value > max)
            {
                max = f.Value;
                result = f.Key;
            }
        }
        return result;
    }

    public string? GetMostHealer()
    {
        Dictionary<string, int> frequencyMap = [];
        int max = int.MinValue;
        string? result = null;
        foreach (BattleLogEntry e in Entries)
        {
            if (e.Damage_Healing_Mana == Damage_Healing_Mana.Healing)
            {
                if (frequencyMap.TryGetValue(e.SourceId, out int val))
                {
                    frequencyMap[e.SourceId]++;
                }
                else
                {
                    frequencyMap[e.SourceId] = 1;
                }
                if (val + 1 > max)
                {
                    max = val + 1;
                    result = e.SourceId;
                }
            }
        }
        return result;
    }

    public string? GetFirstFatalDamage()
    {
        var result = Entries.FirstOrDefault(e => e.Fatal);
        return result?.SourceId;
    }

    public string? GetMagicUser()
    {
        var result = Entries.FirstOrDefault(e => e.WasMagic);
        return result?.SourceId;
    }

    public string? GetUsedMagicMost()
    {
        Dictionary<string, int> frequencyMap = [];
        int max = int.MinValue;
        string? result = null;
        foreach (BattleLogEntry e in Entries)
        {
            if (e.WasMagic)
            {
                if (frequencyMap.TryGetValue(e.SourceId, out int val))
                {
                    frequencyMap[e.SourceId]++;
                }
                else
                {
                    frequencyMap[e.SourceId] = 1;
                }
                if (val + 1 > max)
                {
                    max = val + 1;
                    result = e.SourceId;
                }
            }
        }
        return result;
    }

    public List<string> GetRecentActionsFromSource(string id)
    {
        throw new NotImplementedException();
    }
}

public class BattleLogEntry()
{
    public Damage_Healing_Mana Damage_Healing_Mana { get; init; }
    public double AmountSent { get; init; }
    public double AmountActual { get; init; }
    public double NewValue { get; init; }
    public bool Fatal { get; init; }
    public bool WasMagic { get; init; }
    public string SourceId { get; init; } = string.Empty;
    public string TargetId { get; init; } = string.Empty;
}