using GameServer.Domain.Entities;

namespace GameServer.Domain.Battle;

public interface IBattleEffect
{
    string EntityId { get; init; }
    bool Apply(DamageableEntity target);
    bool Revert(DamageableEntity target);
    string Message { get; set; }
    int RemainingTurns { get; set; } // for multi-turn effects
}