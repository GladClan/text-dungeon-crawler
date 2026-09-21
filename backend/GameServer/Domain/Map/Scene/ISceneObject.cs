using GameServer.Application.Services;
using GameServer.Contracts.DTOs;

namespace GameServer.Domain.Map.Scene;

public interface ISceneContainer
{
    public Dictionary<int, SceneEvent> Events { get; set; }
    public int CurrentEventId { get; set; }
    public SceneEvent CurrentEvent => Events[CurrentEventId];
    public SceneState State { get; init; }
}

/// <summary>
/// Each event in the scene.
/// </summary>
/// <param name="eventId">(integer) Event id within the scene. Must be unique for the scene.</param>
/// <param name="conditions" cref="EventConditions">Conditions that must be met to enter the event, and a navigation to fallback event.</param>
/// <param name="dialogues" cref="Dialogue">Dictionary of Dialogues for the event, with integer keys to failsafe dialogue order.</param>
/// <param name="eventOptions" cref="EventOption">List of EventOptions associated with the event.</param>
public class SceneEvent(
    int eventId,
    EventConditions? conditions,
    Dictionary<int, Dialogue> dialogues,
    List<EventOption> eventOptions
)
{
    public int EventId { get; init; } = eventId;
    public EventConditions? Conditions { get; init; } = conditions;
    public Dictionary<int, Dialogue> Dialogues { get; init; } = dialogues;
    public List<EventOption> EventOptions { get; set; } = eventOptions;
}

/// <summary>
/// An object containing the conditions necessary to enter an event, as well as the navigation object for where to go if those conditions aren't met.
/// </summary>
/// <param name="conditions">List of conditions for the event, such as HasItem, SkillCheck, etc.</param>
/// <param name="requirementsNotMetNavigation">Navigation object to specify where to go if the requirements aren't met.</param>
public class EventConditions(
    IReadOnlyCollection<ICondition> conditions,
    IEventNavigation requirementsNotMetNavigation
)
{
    public IReadOnlyCollection<ICondition> Conditions { get; init; } = conditions;
    public IEventNavigation IfNotMetRequirements {get; init; } = requirementsNotMetNavigation;    
}

/// <summary>
/// An object to hold bools and other objects that deal with tracking player interactions.
/// </summary>
/// <example>
/// For example, if the player pulls a lever to open a door in a different event, the lever state can be tracked here using a bool.
/// public bool LeverPulled { get; set; }
/// </example>
public class SceneState
{
    public bool SkillCheckSuccess = false;
}

/// <summary>
/// String pairs that hold the source of a dialogue and the message
/// </summary>
/// <param name="source">String: the source name</param>
/// <param name="message">String: what did they say.</param>
public class Dialogue(string source, string message)
{
    public string Source { get; init; } = source;
    public string Message { get; init; } = message;
}

/// <summary>
/// An option that goes with an event
/// </summary>
public class EventOption(
    string eventTitle,
    IReadOnlyCollection<ICondition> eventConditions,
    IReadOnlyCollection<IGameEffect> eventEffects,
    IEventNavigation eventNavigation
)
{
    public string Title { get; set; } = eventTitle;
    public IReadOnlyCollection<ICondition> Conditions { get; } = eventConditions;
    public IReadOnlyCollection<IGameEffect> Effects { get; } = eventEffects;
    public IEventNavigation Navigation { get; } = eventNavigation;
}

/// <summary>
/// Conditions for IEvent or IEventOption to be available
/// </summary>
/// <Remarks>
/// Such as HasItem, StatRequirements, FlagSet, EnemyDefeated, ObjectInteractedWith and so forth
/// </Remarks>
public interface ICondition
{
    EventResultDto IsMet(EventServices services);
}

/// <summary>
/// Effects for choices made, such as giving items, healing, damaging, starting fights, or setting scene state objects.
/// </summary>
/// <example>
/// public class AddItemEffect(
///     string _targetDamageableEntityId,
///     string _itemTag,
/// ) : IGameEffect
/// {
///     public void Apply(EventServices services)
///     {
///         services.InventoryService.AddItemByTag(_targetDamageableEntityId, _itemTag);
///     }
/// }
/// </example>
public interface IGameEffect
{
    EventResultDto Apply(EventServices services);
}

public record TargetOption(
    string EntityId,
    string Name
);

public interface ITargetSelector
{
    IReadOnlyList<TargetOption> GetTargets(EventServices services);
}

/// <summary>
/// Used by IEventOption to navigate to a new event, new scene, or to return to the previous scene.<br/>
/// Also can be used to navigate to a battle.
/// </summary>
public interface IEventNavigation
{
    /// <summary>
    /// Used to navigate to scenes or events
    /// </summary>
    /// <param name="services"></param>
    /// <returns>True if navigation was successful, else false</returns>
    bool Navigate(EventServices services);
}