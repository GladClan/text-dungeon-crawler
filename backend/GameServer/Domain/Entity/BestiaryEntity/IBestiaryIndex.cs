namespace GameServer.Domain.Entities.BeastiaryEntity;

public interface IBestiaryIndex
{
    public BeastiaryEntity NewBeastiaryEntityByTag(string tag);
}