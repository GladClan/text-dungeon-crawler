using GameServer.Application.Services;
using GameServer.Domain.Enums;
using GameServer.Domain.Map.Scene;

namespace GameServer.Domain.Map;

public interface IGameContext
{
    public List<SceneContainer> Scenes { get; init; }
    public SceneContainer CurrentScene { get; protected set; }
    public List<SceneContainer> SceneHistory { get; protected set; }
    public IGameContextState Flags { get; init; }
    public SceneContainer GetNextScene(Biome targetBiome);
    public InventoryService InventoryService { get; init; }

    public void SetScene(SceneContainer scene)
    {
        if (CurrentScene is not null)
        {
            SceneHistory.Add(CurrentScene);
        }

        CurrentScene = scene;
    }

    public SceneContainer? ReturnToPreviousScene()
    {
        if (SceneHistory.Count == 0)
        {
            return null;
        }

        CurrentScene = SceneHistory.Last();
        SceneHistory.RemoveAt(SceneHistory.Count - 1);

        return CurrentScene;
    }

    // Services for use in IEventOptions
    public EntityService EntityService { get; init; }
    public CombatService CombatService { get; init; }
    public BattleService BattleService { get; init; }
    public SkillService SkillService { get; init; }
    public StatisticsService StatisticsService { get; init; }
}

/// <summary>
/// An object to hold bools and other objects that deal with tracking player interactions.
/// </summary>
/// <example>
/// For example, if the player pulls a lever to open a door in a different scene, the lever state can be tracked here using a bool.<br/>
/// <code>public bool LeverPulled { get; set; }</code>
/// </example>
public interface IGameContextState { }