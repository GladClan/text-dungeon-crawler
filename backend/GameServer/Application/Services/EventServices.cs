using GameServer.Contracts.DTOs;
using GameServer.Domain.Map;
using GameServer.Domain.Map.Scene;

namespace GameServer.Application.Services;

public sealed class EventServices(
    GameContext context,
    EntityService entityService,
    InventoryService inventoryService,
    CombatService combatService,
    BattleService battleService,
    SkillService skillService,
    StatisticsService statisticsService
)
{
    // Services for use in IEventOptions
    public EntityService EntityService { get; init; } = entityService;
    public InventoryService InventoryService { get; init; } = inventoryService;
    public CombatService CombatService { get; init; } = combatService;
    public BattleService BattleService { get; init; } = battleService;
    public SkillService SkillService { get; init; } = skillService;
    public StatisticsService StatisticsService { get; init; } = statisticsService;

    public string? GetPendingResponse()
    {
        return context.GetPendingResponse();
    }

    public bool SetCurrentEventId(int targetEventId)
    {
        if (!context.CurrentScene.Events.TryGetValue(targetEventId, out _))
        {
            return false;
        }

        context.CurrentScene.CurrentEventId = targetEventId;
        return true;
    }

    public bool NavigateToPreviousScene()
    {
        if (context.SceneHistory.Count == 0)
        {
            return false;
        }

        context.CurrentScene = context.SceneHistory.Last();
        context.SceneHistory.RemoveAt(context.SceneHistory.Count - 1);

        return true;
    }

    public SceneState GetCurrentSceneState()
    {
        return context.CurrentScene.State;
    }

    /// <summary>
    /// Used to choose an option in an event. Verifies if the option is viable, applies the effects, and navigates to the next event.
    /// </summary>
    /// <param name="optionId">
    /// The index value of the option to be chosen
    /// </param>
    /// <returns cref="EventResultDto">
    /// Whether the option was successful. Will flag if the option requires targets, and include the potential targets for the effects. <br/>
    /// </returns>
    public EventResultDto ChooseOption(int optionId)
    {
        // Make sure option is within the scope of the current event's options
        if ( optionId < 0 || optionId > context.CurrentScene.CurrentEvent.EventOptions.Count)
        {
            string optionscount = context.CurrentScene.CurrentEvent.EventOptions.Count == 1 ? "0" : $"0 - {context.CurrentScene.CurrentEvent.EventOptions.Count - 1}";
            return new(
                error: $"ID {optionId} does not exist within the current option value range of {optionscount}"
            );
        }
        // Assign option
        var chosenOption = context.CurrentScene.CurrentEvent.EventOptions[optionId];
        // Check conditions
        foreach (var condition in chosenOption.Conditions)
        {
            var conditionIsMet = condition.IsMet(this);
            if (conditionIsMet.Success == false)
            {
                return conditionIsMet;
            }
        }
        // Apply effects
        foreach (var effect in chosenOption.Effects)
        {
            var result = effect.Apply(this);
            if (result.RequiresTarget || !result.Success || result.Error.Length > 0)
            {
                return result;
            }
        }
        // Navigate
        // Return resulting event information
        if (chosenOption.Navigation.Navigate(this))
        {
            return new(
                success: true
            );
        }
        else
        {
            return new(
                error: "Event navigation failed"
            );
        }
    }

    /// <summary>
    /// Used to get the current event of the current scene.<br/>
    /// </summary>
    /// <remarks>
    /// First checks <c>GameContext</c> if there is an active battle. If not: <br/>
    /// Automatically checks the scene's conditions. If thsoe conditions are not met, triggers the event's <c>IfNotMetRequirements.Navigate</c> method. Repeats this process until the conditions are met.<br/>
    /// </remarks>
    /// <returns cref="SceneEventDto">
    /// If active battle in <see cref="GameContext">, <c>ExistsActiveBattle</c> flag is set to true; </br>
    /// Otherwise returns a dto of the current event with verified viable options. </br>
    /// Potentially retruns dto with just the error populated in case the scene or event is null
    /// </returns>
    public SceneEventDto GetCurrentEvent()
    {
        // Check to see if there is a battle: return battle active flag
        if (context.CurrentBattle is not null)
        {
            return new(
                isBattleActive: true
            );
        }
        var currentEvent = context.CurrentScene.CurrentEvent;
        if (currentEvent is not null)
        {
            // Check to see if curent scene is viable based on the conditions
            while (currentEvent.Conditions is not null && currentEvent.Conditions.Conditions.Any(c => c.IsMet(this).Success == false))
            {
                // If conditions not met, activate the given function
                currentEvent.Conditions.IfNotMetRequirements.Navigate(this);
                // And reset the assigned current event
                currentEvent = context.CurrentScene.CurrentEvent;
            }
            // Return the current event with all viable options
            List<OptionDto> viableOptions = [];
            for (int i = 0; i < currentEvent.EventOptions.Count; i++)
            {
                if (currentEvent.EventOptions[i].Conditions.All(c => c.IsMet(this).Success == true))
                {
                    viableOptions.Add(new(
                        optionId: i,
                        optionTitle: currentEvent.EventOptions[i].Title
                    ));
                }
            }
            return new(
                id: currentEvent.EventId,
                dialogues: [..
                    currentEvent.Dialogues.Select(
                        d => new DialogueDto(
                            orderId: d.Key,
                            source: d.Value.Source,
                            message: d.Value.Message
                        )
                    )
                ],
                options: viableOptions
            );
        }
        else
        {
            return new(
                error: $"Could not find the current event in scene {nameof(context.CurrentScene)}."
            );
        }
    }

    public bool SetPendingResponse(string responseString)
    {
        context.SetPendingResponse(responseString);
        return true;
    }

    public BattleDto? GetCurrentBattle()
    {
        if (context.CurrentBattle is null)
        {
            return null;
        }
        var party = EntityService.GetParty(context.CurrentBattle.PartyId);
        var opponentParty = EntityService.GetParty(context.CurrentBattle.OpponentPartyId);
        return new()
        {
            InitiativeOrder = context.CurrentBattle.InitiativeOrder,
            EntityDtos = [..party, ..opponentParty]
        };
    }
}