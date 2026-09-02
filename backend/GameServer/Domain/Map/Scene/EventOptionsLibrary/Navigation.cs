namespace GameServer.Domain.Map.Scene.EventOptionsLibrary;

public class SceneEventNavigation(int targetEventId) : IEventNavigation
{
    private readonly int _targetEventId = targetEventId;

    public bool Navigate(IGameContext context)
    {
        if (!context.CurrentScene.Events.TryGetValue(_targetEventId, out _))
        {
            return false;
        }

        context.CurrentScene.CurrentEventId = _targetEventId;
        return true;
    }
}

public class NavigateToPreviousScene: IEventNavigation
{
    public bool Navigate(IGameContext context)
    {
        var result = context.ReturnToPreviousScene();
        if (result is null)
        {
            return false;
        }
        return true;
    }
}