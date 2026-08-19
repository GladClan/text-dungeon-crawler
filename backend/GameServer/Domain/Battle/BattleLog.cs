using GameServer.Contracts.DTOs;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Battle;

public class BattleLog()
{
    private readonly List<BattleLogEntry> _entries = [];
    private readonly List<string> _damageableEntityIds = [];

    public List<BattleLogEntry> GetAllEntries()
    {
        return [.. _entries];
    }

    public List<BattleLogEntry> GetEntriesBySource(string id)
    {
        return [.. _entries.Where(e => e.SourceId.Equals(id, StringComparison.InvariantCultureIgnoreCase))];
    }

    public bool AddNewEntries(EffectDto request)
    {
        if (request.Results is null)
        {
            return false;
        }
        foreach (var result in request.Results)
        {
            _entries.Add(new BattleLogEntry
            {
                ActionType = (ActionType)result.ActionType,
                AmountSent = result.AmountSent,
                AmountActual = result.AmountActual,
                NewValue = result.NewValue,
                Fatal = result.Fatal,
                WasMagic = request.WasMagic,
                SourceId = result.SourceId,
                TargetId = result.TargetId,
            });
        }
        return true;
    }

    public string? GetHighestSingleDamage(int nthEntry = 0)
    {
        string? result = null;
        double max = double.MinValue;
        if (nthEntry == 0)
        {
            foreach (BattleLogEntry e in _entries)
            {
                if (e.ActionType == ActionType.Attack && e.AmountActual > max)
                {
                    max = e.AmountActual;
                    result = e.SourceId;
                }
            }
            return result;
        }
        else
        {
            var sorted = _entries.OrderByDescending(e => e.AmountActual);
            return nthEntry < sorted.Count() ? sorted.ElementAt(nthEntry).SourceId : null;
        }
    }

    public string? GetHasAttackedSource(string sourceId, int nthEntry = 0)
    {
        string? result = null;
        foreach (BattleLogEntry e in _entries)
        {
            if (e.ActionType == ActionType.Attack && e.TargetId.Equals(sourceId, StringComparison.InvariantCultureIgnoreCase) && e.AmountActual > 0)
            {
                if (nthEntry <= 0)
                {
                    return e.SourceId;
                }
                else
                {
                    result = "";
                    nthEntry--;
                }
            }
        }
        return result;
    }

    public string? GetHighestDamageSent()
    {
        string? result = null;
        double max = double.MinValue;
        foreach (BattleLogEntry e in _entries)
        {
            if (e.ActionType == ActionType.Attack && e.AmountSent > max)
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
        foreach (BattleLogEntry e in _entries)
        {
            if (e.ActionType == ActionType.Attack)
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

    public string? GetMostHealer(int nthEntry = 0)
    {
        Dictionary<string, int> frequencyMap = [];
        int max = int.MinValue;
        string? result = null;
        foreach (BattleLogEntry e in _entries)
        {
            if (e.ActionType == ActionType.Healing)
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
        if (nthEntry == 0)
        {
            return result;
        }
        else
        {
            if (nthEntry < frequencyMap.Count)
            {
                return frequencyMap.OrderByDescending(e => e.Value).ElementAt(nthEntry).Key;
            }
            else
            {
                return null;
            }
        }
    }

    public string? GetFirstFatalDamage()
    {
        var result = _entries.FirstOrDefault(e => e.Fatal);
        return result?.SourceId;
    }

    public string? GetMagicUser()
    {
        var result = _entries.FirstOrDefault(e => e.WasMagic);
        return result?.SourceId;
    }

    public string? GetUsedMagicMost()
    {
        Dictionary<string, int> frequencyMap = [];
        int max = int.MinValue;
        string? result = null;
        foreach (BattleLogEntry e in _entries)
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
    public ActionType ActionType { get; init; }
    public double AmountSent { get; init; }
    public double AmountActual { get; init; }
    public double NewValue { get; init; }
    public bool Fatal { get; init; }
    public bool WasMagic { get; init; }
    public string SourceId { get; init; } = string.Empty;
    public string TargetId { get; init; } = string.Empty;
}