using GameServer.Domain.Battle;
using GameServer.Domain.Enums;
using GameServer.Domain.Statistics;

namespace GameServer.Application.Services;

public sealed class StatisticsService(StatisticsTracker statistics)
{
    public double GetPartyStatistic(PartyStatistic stat)
    {
        return stat switch
        {
            PartyStatistic.BattlesWon => statistics.BattlesWon,
            PartyStatistic.RoomsOpened => statistics.RoomsOpened,
            PartyStatistic.OpponentsConverted => statistics.OpponentsConverted,
            PartyStatistic.AlliesFound => statistics.AlliesFound,
            PartyStatistic.ItemsAcquired => statistics.ItemsAcquired,
            PartyStatistic.TimesSentDamage => statistics.TimesSentDamage,
            PartyStatistic.TotalDamageSent => statistics.TotalDamageSent,
            PartyStatistic.TotalDamageDealt => statistics.TotalDamageDealt,
            PartyStatistic.TimesSentHealing => statistics.TimesSentHealing,
            PartyStatistic.TotalHealingSent => statistics.TotalHealingSent,
            PartyStatistic.TotalHealingDealt => statistics.TotalHealingDealt,
            PartyStatistic.TimesPartyMemberDied => statistics.TimesPartyMemberDied,
            PartyStatistic.TotalEnemiesDefeated => statistics.TotalEnemiesDefeated,
            PartyStatistic.PartyMemebersLost => statistics.PartyMemebersLost,
            PartyStatistic.SuperdamageFrequency => statistics.SuperdamageFrequency,
            PartyStatistic.SubdamageFrequency => statistics.SubdamageFrequency,
            PartyStatistic.PartySuperdamage => statistics.PartySubdamage,
            PartyStatistic.PartySubdamage => statistics.PartySuperdamage,
            PartyStatistic.StrategyScore => statistics.StrategyScore,
            _ => throw new ArgumentException($"{stat} is not a valid statistic", nameof(stat))
        };
    }

    public double? GetEntityStatistic(string id, EntityStatistic stat)
    {
        var entityStats = statistics.PartyMemberStats.FirstOrDefault(s => s.EntityId.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        if (entityStats == null)
        {
            return null;
        }
        return stat switch
        {
            EntityStatistic.TimesDied => entityStats.TimesDied,
            EntityStatistic.TimesSentDamage => entityStats.TimesSentDamage,
            EntityStatistic.DamageSent => entityStats.DamageSent,
            EntityStatistic.AverageDamageSent => entityStats.AverageDamageSent,
            EntityStatistic.DamageDealt => entityStats.DamageDealt,
            EntityStatistic.AverageDamageDealt => entityStats.AverageDamageDealt,
            EntityStatistic.AverageActualDamage => entityStats.AverageActualDamage,
            EntityStatistic.TimesReceivedDamage => entityStats.TimesReceivedDamage,
            EntityStatistic.DamageRecieved => entityStats.DamageRecieved,
            EntityStatistic.AverageDamageReceived => entityStats.AverageDamageReceived,
            EntityStatistic.TimesHealed => entityStats.TimesHealed,
            EntityStatistic.HealingSent => entityStats.HealingSent,
            EntityStatistic.AverageHealingSent => entityStats.AverageHealingSent,
            EntityStatistic.HealingDealt => entityStats.HealingDealt,
            EntityStatistic.AverageHealingDealt => entityStats.AverageHealingDealt,
            EntityStatistic.TimesReceivedHealing => entityStats.TimesReceivedHealing,
            EntityStatistic.HealingReceived => entityStats.HealingReceived,
            EntityStatistic.AverageHealingReceived => entityStats.AverageHealingReceived,
            EntityStatistic.TimesDealtFatalDamage => entityStats.TimesDealtFatalDamage,
            EntityStatistic.EntitiesDefeated => entityStats.EntitiesDefeated,
            EntityStatistic.TimesUsedMagic => entityStats.TimesUsedMagic,
            _ => throw new ArgumentException($"{stat} is not a valid statistic", nameof(stat))
        };
    }

    public bool AddEntriesToStats(List<BattleLogEntry> entries)
    {
        var bySource = entries.GroupBy(e => e.SourceId);
        var byTarget = entries.GroupBy(e => e.TargetId);
        
        // Add statistic party members created
        foreach (var g in bySource)
        {
            string id = g.Key;
            var stats = statistics.PartyMemberStats.FirstOrDefault(s => s.EntityId.Equals(id, StringComparison.InvariantCultureIgnoreCase));
            if (stats == null)
            {
                stats = new DamageableEntityStatistics { EntityId = id };
                statistics.PartyMemberStats.Add(stats);
            }

            var damageEntries = g.Where(e => e.ActionType == ActionType.Attack).ToList();
            var healEntries = g.Where(e => e.ActionType == ActionType.Healing).ToList();

            double damageSentSum = damageEntries.Sum(e => e.AmountSent);
            double damageActualSum = damageEntries.Sum(e => e.AmountActual);
            double healSentSum = healEntries.Sum(e => e.AmountSent);
            double healActualSum = healEntries.Sum(e => e.AmountActual);

            statistics.SuperdamageFrequency += damageEntries.Count(e => e.AmountActual > e.AmountSent);
            statistics.SubdamageFrequency += damageEntries.Count(e => e.AmountSent > e.AmountActual);
            statistics.PartySuperdamage += damageEntries.Sum(e => e.AmountActual > e.AmountSent ? e.AmountActual - e.AmountSent : 0);
            statistics.PartySubdamage += damageEntries.Sum(e => e.AmountActual < e.AmountSent ? e.AmountSent - e.AmountActual : 0);
            statistics.TimesSentDamage += damageEntries.Count;
            statistics.TotalDamageSent += damageSentSum;
            statistics.TotalDamageDealt += damageActualSum;
            statistics.TimesSentHealing += healEntries.Count;
            statistics.TotalHealingSent += healSentSum;
            statistics.TotalHealingDealt += healActualSum;

            stats.TimesSentDamage += damageEntries.Count;

            stats.DamageSent += damageEntries.Sum(e => e.AmountSent);
            stats.AverageDamageSent = stats.DamageSent / stats.TimesSentDamage;

            stats.DamageDealt += damageActualSum;
            stats.AverageActualDamage = stats.DamageDealt / stats.TimesSentDamage;

            stats.TimesHealed += healEntries.Count;

            stats.HealingSent += healSentSum;
            stats.AverageHealingSent = stats.HealingSent / stats.TimesHealed;

            stats.HealingDealt += healActualSum;
            stats.AverageHealingDealt = stats.HealingDealt / stats.TimesHealed;
            
            stats.TimesUsedMagic += g.Count(e => e.WasMagic);

            stats.TimesDealtFatalDamage += g.Count(e => e.Fatal && e.ActionType == ActionType.Attack);

            int enemiesDefeated = g.Where(e => e.Fatal).Select(e => e.TargetId).Distinct().Count();
            stats.EntitiesDefeated += enemiesDefeated;
            statistics.TotalEnemiesDefeated += enemiesDefeated;
        }

        foreach (var g in byTarget)
        {
            var id = g.Key;
            var stats = statistics.PartyMemberStats.FirstOrDefault(s => s.EntityId.Equals(id, StringComparison.InvariantCultureIgnoreCase));
            if (stats == null)
            {
                stats = new DamageableEntityStatistics { EntityId = id };
                statistics.PartyMemberStats.Add(stats);
            }

            var damageEntries = g.Where(e => e.ActionType == ActionType.Attack).ToList();
            var healEntries = g.Where(e => e.ActionType == ActionType.Healing).ToList();

            var damageSum = damageEntries.Sum(e => e.AmountActual);
            var healSum = healEntries.Sum(e => e.AmountActual);

            stats.DamageRecieved += damageSum;
            stats.AverageDamageReceived = damageEntries.Count != 0 ? damageEntries.Average(e => e.AmountActual) : stats.AverageDamageReceived;

            stats.HealingReceived += healSum;
            stats.AverageHealingReceived = healEntries.Count != 0 ? healEntries.Average(e => e.AmountActual) : stats.AverageHealingReceived;

            int timesDied = g.Count(e => e.Fatal);

            stats.TimesDied += timesDied;

            statistics.TimesPartyMemberDied += timesDied;
        }

        // double offensiveEfficiency = (statistics.TotalDamageDealt - statistics.TotalDamageSent) / statistics.TimesSentDamage;
        double offensiveEfficiency = statistics.PartySubdamage != 0 ?
            statistics.PartySuperdamage / statistics.PartySubdamage / statistics.TimesSentDamage : 
            statistics.PartySuperdamage / statistics.TimesSentDamage;
        double piercingRatio = statistics.SuperdamageFrequency / (statistics.SubdamageFrequency + 1);

        // unsure what to do with this in the case of undead enemies
        // double healingEfficiency = (statistics.TotalHealingDealt - statistics.TotalHealingSent) / statistics.TimesSentHealing; 
        double positivePlayScore = (
            (offensiveEfficiency * 0.4)
            + (piercingRatio * 0.3)
            // + (healingEfficiency * 0.2)
            + (statistics.TotalEnemiesDefeated * 0.1)
        ) / 4;
        double deathPenaltyMultiplier = 1.0 / (statistics.TimesPartyMemberDied + 1);

        statistics.StrategyScore = (int)(positivePlayScore * deathPenaltyMultiplier);

        return true;
    }
}