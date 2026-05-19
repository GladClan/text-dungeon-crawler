using GameServer.Domain.Entities.EntityAI;

namespace GameServer.Domain.Entities.EntityAI.AILibrary;

public class EntitAIIndex : IEntitAIIndex
{
    private readonly List<IEntityAI> AICatalogue = InitializeAIs();

    private static List<IEntityAI> InitializeAIs()
    {
        var AIs = new List<IEntityAI>();
        var AIType = typeof(IEntityAI);
        var assembly = typeof(IEntitAIIndex).Assembly;

        // Find all concrete types that inherit from IEntityAI in the InitialRelease namespace
        var concreteAITypes = assembly.GetTypes()
            .Where(t =>
                t.Namespace == "GameServer.Domain.Entities.EntityAI.AILibrary" &&
                !t.IsInterface &&
                AIType.IsAssignableFrom(t)
            );
        foreach (var type in concreteAITypes)
        {
            try
            {
                if (Activator.CreateInstance(type) is IEntityAI ai)
                {
                    AIs.Add(ai);
                }
            }
            catch
            {
                // Skip AI's that can't be instantiated
            }
        }
        return AIs;
    }
    public IEntityAI GetByTag(string tag)
    {
        return AICatalogue.FirstOrDefault(s => s.Tag.Equals(tag, StringComparison.InvariantCultureIgnoreCase)) ?? new DefaultAI();
    }
}