using GameServer.Contracts.DTOs;
using GameServer.Domain.Battle;

namespace GameServer.Domain.Entities.EntityAI;

public interface IEntityAI
{
    public string Tag { get; }
    public string SignificantEntityId { get; set; }
    public abstract bool SetSignificantEntityId(string id);
    public abstract EffectDto GetAction(DamageableEntity source, BattleTracker battle);
}