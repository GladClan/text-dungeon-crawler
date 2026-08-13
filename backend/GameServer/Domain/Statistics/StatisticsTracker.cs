using GameServer.Domain.Battle;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Statistics;

public class StatisticsTracker
{
    public int BattlesWon { get; set; }
    public int RoomsOpened { get; set; }
    public int OpponentsConverted { get; set; }
    public int AlliesFound { get; set; }
    public int ItemsAcquired { get; set; }
    public int TimesSentDamage { get; set; }
    public double TotalDamageSent { get; set; }
    public double TotalDamageDealt { get; set; }
    public int TimesSentHealing { get; set; }
    public double TotalHealingSent { get; set; }
    public double TotalHealingDealt { get; set; }
    public int TimesPartyMemberDied { get; set; }
    public int TotalEnemiesDefeated { get; set; }
    public int PartyMemebersLost { get; set; }
    public int SuperdamageFrequency { get; set; } // the number of positive differences between damage sent and damage actual
    public int SubdamageFrequency { get; set; } // the number of negative differences between damage sent and damage actual
    public double PartySuperdamage { get; set; } // the positive difference between damage sent and damage actual
    public double PartySubdamage { get; set; } // the negative difference between damage sent and damage actual
    public double StrategyScore { get; set; } // subdamage divided by superdamage (%subdamage / %superdamage)
    public List<DamageableEntityStatistics> PartyMemberStats { get; set; } = [];
}

public class DamageableEntityStatistics()
{
    public string EntityId { get; init; } = string.Empty;
    public int TimesDied { get; set; }
    public int TimesSentDamage { get; set; }
    public double DamageSent { get; set; }
    public double AverageDamageSent { get; set; }
    public double DamageDealt { get; set; }
    public double AverageDamageDealt { get; set; }
    public double AverageActualDamage { get; set; }
    public int TimesReceivedDamage { get; set; }
    public double DamageRecieved { get; set; }
    public double AverageDamageReceived { get; set; }
    public int TimesHealed { get; set; }
    public double HealingSent { get; set; }
    public double AverageHealingSent { get; set; }
    public double HealingDealt { get; set; }
    public double AverageHealingDealt { get; set; }
    public int TimesReceivedHealing { get; set; }
    public double HealingReceived { get; set; }
    public double AverageHealingReceived { get; set; }
    public int TimesDealtFatalDamage { get; set; }
    public int EntitiesDefeated { get; set; }
    public int TimesUsedMagic { get; set; }
}