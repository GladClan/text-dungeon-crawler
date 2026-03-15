namespace GameServer.Domain.Entities;

public abstract class EntityMetadata
{
    private static int _entityCounter = 0;
    public string Name { get; protected set; }
    public string EntityType { get; protected set; }
    public Guid ID { get; }
    public string SimpleID { get; }

    protected EntityMetadata( string name, string type )
    {
        Name = name;
        EntityType = type;
        ID = Guid.NewGuid();
        SimpleID = GenerateEntityId();
    }

    private string GenerateEntityId()
    {
        string counter = Interlocked.Increment(ref _entityCounter).ToString("D3");
        string timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
        string prefix = EntityType.PadRight(3, '_')[..3].ToLowerInvariant();

        return $"{prefix}_{timestamp}_{counter}";
    }
}