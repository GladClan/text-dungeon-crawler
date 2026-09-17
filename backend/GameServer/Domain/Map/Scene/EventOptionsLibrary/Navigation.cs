using GameServer.Application.Services;

namespace GameServer.Domain.Map.Scene.EventOptionsLibrary;

public class SceneEventNavigation(int targetEventId) : IEventNavigation
{
    private readonly int _targetEventId = targetEventId;

    public bool Navigate(EventServices services)
    {
        return services.SetCurrentEventId(_targetEventId);
    }
}

public class NavigateToPreviousScene: IEventNavigation
{
    public bool Navigate(EventServices services)
    {
        return services.NavigateToPreviousScene();
    }
}