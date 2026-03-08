namespace GameServer.Domain.Entity;

public abstract class EntityMetadata
{
    private static int _entityCounter = 0;
    public string Name { get; protected set; }
    public string Type { get; protected set; }
    public Guid ID { get; }
    public string SimpleID { get; }

    protected EntityMetadata( string name, string type )
    {
        Name = name;
        Type = type;
        ID = Guid.NewGuid();
        SimpleID = GenerateEntityId();
    }

    private string GenerateEntityId()
    {
        string counter = Interlocked.Increment(ref _entityCounter).ToString("D3");
        string timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
        string prefix = Type.PadRight(3, '_')[..3].ToLowerInvariant();

        return $"{prefix}_{timestamp}_{counter}";
    }
}