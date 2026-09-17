using GameServer.Domain.Entities.EntityAI;

namespace GameServer.Domain.Entities;

public interface IEntityAIIndex
{
    public IEntityAI GetByTag(string tag);
}