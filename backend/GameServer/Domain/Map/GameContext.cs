using GameServer.Domain.Battle;
using GameServer.Domain.Enums;
using GameServer.Domain.Map.Scene;

namespace GameServer.Domain.Map;

public class GameContext
{
    public List<ISceneContainer> Scenes { get; init; }
    public ISceneContainer CurrentScene { get; set; }
    public List<ISceneContainer> SceneHistory { get; protected set; }
    public IGameContextState Flags { get; init; }
    public BattleTracker? CurrentBattle { get; set; }
    private string? PendingResponse { get; set; }

    public GameContext(    
        List<ISceneContainer> scenes,
        IGameContextState gameContextState
    )
    {
        if (scenes.Count < 1)
        {
            throw new ArgumentException("Game must have at least one scene, you ninny!", nameof(scenes));
        }
        Scenes = scenes;
        CurrentScene = scenes[0];
        Flags = gameContextState;
        SceneHistory = [];
    }
    
    public void SetPendingResponse(string response)
    {
        PendingResponse = response;
    }

    public string? GetPendingResponse()
    {
        string? response = PendingResponse;
        PendingResponse = null;
        return response;
    }

    public ISceneContainer GetNextScene(Biome targetBiome)
    {
        throw new NotImplementedException();
    }

    public void SetScene(ISceneContainer scene)
    {
        if (CurrentScene is not null)
        {
            SceneHistory.Add(CurrentScene);
        }

        CurrentScene = scene;
    }
}

/// <summary>
/// An object to hold bools and other objects that deal with tracking player interactions.
/// </summary>
/// <example>
/// For example, if the player pulls a lever to open a door in a different scene, the lever state can be tracked here using a bool.<br/>
/// <code>public bool LeverPulled { get; set; }</code>
/// </example>
public interface IGameContextState { }

public class TestGameContextState: IGameContextState { }