using GameServer.Domain.Entities.EntityAI;

namespace GameServer.Domain.Entities.EntityAI.AILibrary;

public class EntitAIIndex : IEntityAIIndex
{
    private readonly List<IEntityAI> AICatalogue = InitializeAIs();

    private static List<IEntityAI> InitializeAIs()
    {
        var AIs = new List<IEntityAI>();
        var AIType = typeof(IEntityAI);
        var assembly = typeof(IEntityAIIndex).Assembly;

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
            catch (Exception ex)
            {
                // Skip AI's that can't be instantiated
                Console.WriteLine(
                    $"Failed to initialize AI type {type.FullName}\n{ex}");
            }
        }
        return AIs;
    }
    public IEntityAI GetByTag(string tag)
    {
        return AICatalogue.FirstOrDefault(s => s.Tag.Equals(tag, StringComparison.InvariantCultureIgnoreCase)) ?? new DefaultAI();
    }
}