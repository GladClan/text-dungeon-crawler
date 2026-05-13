using GameServer.Domain.Battle;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Statistics;

public class StatisticsTracker
{
    public int BattlesWon { get; set; }
    public int RoomsOpened { get; set; }
    public List<DamageableEntityStatistics> PartyStats { get; set; } = [];

    public bool AddEntriesToStats(List<BattleLogEntry> entries)
    {
        var bySource = entries.GroupBy(e => e.SourceId);
        var byTarget = entries.GroupBy(e => e.TargetId);
        
        foreach (var g in bySource)
        {
            string id = g.Key;
            var stats = PartyStats.FirstOrDefault(s => s.EntityId.Equals(id, StringComparison.InvariantCultureIgnoreCase));
            if (stats == null)
            {
                stats = new DamageableEntityStatistics { EntityId = id };
                PartyStats.Add(stats);
            }

            var damageEntries = g.Where(e => e.Damage_Healing_Mana == Damage_Healing_Mana.Damage).ToList();
            var healEntries = g.Where(e => e.Damage_Healing_Mana == Damage_Healing_Mana.Healing).ToList();

            double damageSum = damageEntries.Sum(e => e.AmountActual);
            double healSum = healEntries.Sum(e => e.AmountActual);

            stats.DamageSent += damageEntries.Sum(e => e.AmountSent);
            stats.AverageDamageSent = damageEntries.Count != 0 ? damageEntries.Average(e => e.AmountSent) : stats.AverageDamageSent;

            stats.DamageDealt += damageSum;
            stats.AverageDamage = damageEntries.Count != 0 ? damageEntries.Average(e => e.AmountActual) : stats.AverageDamage;

            stats.TimesHealed += healEntries.Count;
            stats.AverageHealing = healEntries.Count != 0 ? healEntries.Average(e => e.AmountActual) : stats.AverageHealing;
            
            stats.TimesHealed += healEntries.Count;
            stats.AverageHealing = healEntries.Count != 0 ? healEntries.Average(e => e.AmountActual) : stats.AverageHealing;

            stats.TimesUsedMagic += g.Count(e => e.WasMagic);
            stats.TimesDealtFatalDamage += g.Count(e => e.Fatal && e.Damage_Healing_Mana == Damage_Healing_Mana.Damage);

            stats.EntitiesDefeated += g.Where(e => e.Fatal).Select(e => e.TargetId).Distinct().Count();
        }

        foreach (var g in byTarget)
        {
            var id = g.Key;
            var stats = PartyStats.FirstOrDefault(s => s.EntityId.Equals(id, StringComparison.InvariantCultureIgnoreCase));
            if (stats == null)
            {
                stats = new DamageableEntityStatistics { EntityId = id };
                PartyStats.Add(stats);
            }

            var damageEntries = g.Where(e => e.Damage_Healing_Mana == Damage_Healing_Mana.Damage).ToList();
            var healEntries = g.Where(e => e.Damage_Healing_Mana == Damage_Healing_Mana.Healing).ToList();

            var damageSum = damageEntries.Sum(e => e.AmountActual);
            var healSum = healEntries.Sum(e => e.AmountActual);

            stats.DamageRecieved += damageSum;
            stats.AverageDamageReceived = damageEntries.Count != 0 ? damageEntries.Average(e => e.AmountActual) : stats.AverageDamageReceived;

            stats.HealingReceived += healSum;
            stats.AverageHealingReceived = healEntries.Count != 0 ? healEntries.Average(e => e.AmountActual) : stats.AverageHealingReceived;

            stats.TimesDied = g.Count(e => e.Fatal);
        }

        return true;
    }
}

public class DamageableEntityStatistics()
{
    public string EntityId { get; init; } = string.Empty;
    public int TimesDied { get; set; }
    public double DamageSent { get; set; }
    public double AverageDamageSent { get; set; }
    public double DamageDealt { get; set; }
    public double AverageDamage { get; set; }
    public double DamageRecieved { get; set; }
    public double AverageDamageReceived { get; set; }
    public int TimesHealed { get; set; }
    public double AverageHealing { get; set; }
    public double HealingReceived { get; set; }
    public double AverageHealingReceived { get; set; }
    public int TimesDealtFatalDamage { get; set; }
    public int EntitiesDefeated { get; set; }
    public int TimesUsedMagic { get; set; }
}