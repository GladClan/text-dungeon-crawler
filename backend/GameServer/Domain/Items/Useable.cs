using GameServer.Contracts.DTOs;
using GameServer.Domain.Battle;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace GameServer.Domain.Items;

public abstract class Useable : Item
{
    public DamageType Element { get; protected set; }
    public Proficiency Prof { get; protected set; }
    public abstract EffectDto ItemEffect(DamageableEntity source, DamageableEntity mainTarget, List<DamageableEntity>? subTargets, BattleTracker battle);
    public ActionType ItemType { get; set; }
    public bool MultiTarget { get; set; }
    public int TargetsLimit { get; set; }
    public virtual bool CanUse(DamageableEntity target)
    {
        return true;
    }

    public Useable()
    {
    }

    public Useable(string type, string name, int cost, string description, bool consumable, bool sellable, DamageType element, bool multiTarget, int targetsLimit, Proficiency proficiency, ActionType itemType, string shopType, int rarity, int collection)
     : base(type, name, cost, description, consumable, sellable, shopType, rarity, collection)
    {
        Element = element;
        Prof = proficiency;
        ItemType = itemType;
        MultiTarget = multiTarget;
        TargetsLimit = targetsLimit;
    }
}