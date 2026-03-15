namespace GameServer.Domain.Exceptions;

public class EntityNotAliveException : Exception
{
    public EntityNotAliveException()
    {
    }

    public EntityNotAliveException(string message)
        : base(message)
    {
    }

    public EntityNotAliveException(string message, Exception inner)
        : base(message, inner)
    {
    }
}