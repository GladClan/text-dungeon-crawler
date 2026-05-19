using GameServer.Domain.Entities.EntityAI;

namespace GameServer.Domain.Entities;

public interface IEntitAIIndex
{
    public IEntityAI GetByTag(string tag);
}