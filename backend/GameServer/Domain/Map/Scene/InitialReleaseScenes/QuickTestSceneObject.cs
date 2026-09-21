using GameServer.Application.Services;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Requests;
using GameServer.Domain.Enums;
using GameServer.Domain.Map.Scene.EventOptionsLibrary;

namespace GameServer.Domain.Map.Scene.InitialReleaseScenes;

public class QuickTestSceneObjectsState: SceneState
{
    public int GottenGold = 0;
    public bool OgresGateOpen = false;
    public bool HermitGifted = false;
    public string WarriorId = string.Empty;
}

public class QuickTestSceneObject: ISceneContainer
{
    public Dictionary<int, SceneEvent> Events { get; set; }
    public int CurrentEventId { get; set; } = 0;
    public SceneState State { get; init; }
    private readonly string _partyId;
    private readonly string _narrator = "narrator";
    private readonly string _hermit = "Hermit";
    private readonly string _warrior = "Warrior";
    private readonly string _warriorsKey = "warriors-gate-key";
    private readonly DamageableEntityRequest warriorRequest = new()
    {
        Name = "Chosen Warrior",
        EntityType = "warrior",
        Race = "unclear... humanoid",
        PartyId = "none",
        Health = 180,
        Magic = 9,
        Mana = 30,
        Strength = 12,
        Defense = 11,
        AttackType = DamageType.crushing.ToString(),
        DealsMagicDamage = false,
        Speed = 16,
        Level = 2,
        Experience = 12,
        Resistances = [
            new() { Type = DamageType.slashing.ToString(), Value = 0.5 },
            new() { Type = DamageType.piercing.ToString(), Value = 0.4 },
            new() { Type = DamageType.crushing.ToString(), Value = 0.3 },
            new() { Type = DamageType.darkling.ToString(), Value = 0.25 },
            new() { Type = DamageType.burning.ToString(), Value = -0.05 },
            new() { Type = DamageType.freezing.ToString(), Value = -0.1 },
            new() { Type = DamageType.shocking.ToString(), Value = -0.25 },
            new() { Type = DamageType.soaking.ToString(), Value = -0.1 },
        ],
        Proficiencies = [
            new() { Type = Proficiency.bludgeoning.ToString(), Value = 1 },
            new() { Type = Proficiency.bow.ToString(), Value = 0.85 },
            new() { Type = Proficiency.piercing.ToString(), Value = 1.2 },
            new() { Type = Proficiency.slashing.ToString(), Value = 0.95 },
            new() { Type = Proficiency.hand.ToString(), Value = 0.98 },
            new() { Type = Proficiency.healing.ToString(), Value = 0.75 },
            new() { Type = Proficiency.nobility.ToString(), Value = 1.3 },
        ],
        ItemTags = [
            "sword-arming",
            "chestplate-raze",
            "potion-healing",
            "potion-healing"
        ]
        // SkilTags
    };

    public enum EventDesignations
    {
        TestEntry = 0,
        WarriorsGate = 1,
        OgresGate = 2,
        HermitsHovel = 3,
        WarriorsAbode = 4,
        GetTheKey = 5,
        GetTheGold = 6,
        OgresGateway = 7,
        HermitsGift = 8,
        HermitGiftNotGiven = 9,
        YouGotAThing = 10,
        OgreFight = 11,
        BattleWon = 12,
        GemPile = 13,
        PrincessTower = 14,
        TestEnd = 15,
    }
    public QuickTestSceneObject(string partyId)
    {
        _partyId = partyId;
        State = new QuickTestSceneObjectsState();

        EventOption GoLeft = new(
            eventTitle: "Go left",
            eventConditions: [],
            eventEffects: [],
            eventNavigation: new SceneEventNavigation((int)EventDesignations.WarriorsGate)
        );
        EventOption GoRight = new(
            eventTitle: "Go right",
            eventConditions: [],
            eventEffects: [],
            eventNavigation: new SceneEventNavigation((int)EventDesignations.HermitsHovel)
        );
        EventOption GoBackToCrossroads = new(
            eventTitle: "Go back to the crossroads",
            eventConditions: [],
            eventEffects: [],
            eventNavigation: new SceneEventNavigation((int)EventDesignations.TestEntry)
        );
        EventOption Stayawhile = new(
            eventTitle: "Stay awhile",
            eventConditions: [],
            eventEffects: [],
            eventNavigation: new SceneEventNavigation((int)EventDesignations.HermitsHovel)
        );

        Events = new Dictionary<int, SceneEvent>
        {
            {(int)EventDesignations.TestEntry,
                new(
                    eventId: (int)EventDesignations.TestEntry,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "There are three signs:")},
                        {1, new(_narrator, "Left to the gate")},
                        {2, new(_narrator, "Forward to the ogre")},
                        {3, new(_narrator, "Right to the hermit")}
                    },
                    eventOptions: [
                        GoLeft,
                        new(
                            eventTitle: "Go straight",
                            eventConditions: [],
                            eventEffects: [],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.OgresGate)
                        ),
                        GoRight
                    ]
                )
            },
            {(int)EventDesignations.WarriorsGate,
                new(
                    eventId: (int)EventDesignations.WarriorsGate,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "The gate needs a key to open.")}
                    },
                    eventOptions: [
                        new(
                            eventTitle: "Use the hermit's key on the gate",
                            eventConditions: [new HasItem(_partyId, _warriorsKey)],
                            eventEffects: [],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.WarriorsAbode)
                        ),
                        GoBackToCrossroads
                    ]
                )
            },
            {(int)EventDesignations.OgresGate,
                new(
                    eventId: (int)EventDesignations.OgresGate,
                    conditions: new(
                        conditions: [new ConditionTestSceneOgresGateClosed()],
                        requirementsNotMetNavigation: new SceneEventNavigation((int)EventDesignations.OgresGateway)
                    ),
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "The gate is shut.")},
                        {1, new(_narrator, "There is a sign chained to the entry.")},
                        {2, new(_narrator, "It says you must have a champion to open the door and a sufficient fund for entry.")},
                        {3, new(_narrator, "Fifty gold.")},
                        {4, new(_narrator, "The price is steep, but the prize is dear.")}
                    },
                    eventOptions: [
                        new(
                            eventTitle: "Open the gate",
                            eventConditions: [
                                new ConditionTestSceneWarriorInParty(_partyId),
                                new HasGold(_partyId, 50)
                            ],
                            eventEffects: [
                                new AddOrRemoveGold(
                                    targetSelector: new PartyMemberSelector(_partyId),
                                    _goldToAdd: -50,
                                    prompt: "Who is paying?"
                                ),
                                new EffectTestSceneSetOgresGateOpen()
                            ],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.OgresGateway)
                        ),
                        GoBackToCrossroads
                    ]
                )
            },
            {(int)EventDesignations.HermitsHovel,
                new(
                    eventId: (int)EventDesignations.HermitsHovel,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "A quaint place, this one, but cozy.")},
                        {1, new(_narrator, "The hermit is home, cooking a lovely stew.")},
                        {2, new(_narrator, "He addresses you.")},
                        {3, new(_hermit, "Hellew, chapper, what can I dew for yew?")}

                    },
                    eventOptions: [
                        new(
                            eventTitle: "Ask for the key",
                            eventConditions: [
                                new NotHasItem(_partyId, _warriorsKey)
                            ],
                            eventEffects: [
                                new ChangeOptionTitle(GoRight, "To the hermit's hovel"),
                                new GiveItem(new PartyMemberSelector(_partyId), _warriorsKey, "Who should receive the key?")
                            ],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.GetTheKey)
                        ),
                        new(
                            eventTitle: "Has he any spare change",
                            eventConditions: [
                                new ConditionTestSceneWarriorInParty(_partyId),
                                new ConditionTestSceneGottenGoldFromHermitLessThan(2)
                            ],
                            eventEffects: [
                                new EffectTestSceneAddGold(
                                    targetSelector: new PartyMemberSelector(_partyId),
                                    prompt: "Who should get the gold?"
                                ),
                                new EffectTestSceneIncrementGottenGold()
                            ],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.GetTheGold)
                        ),
                        new(
                            eventTitle: "Is there anything more he has to offer?",
                            eventConditions: [
                                new ConditionTestSceneOgresGateOpen(),
                                new ConditionTestSceneNotHermitHasGivenGift()
                            ],
                            eventEffects: [
                                new EffectSkillCheck(
                                    targetSelector: new PartyMemberSelector(_partyId),
                                    proficiency: Proficiency.persuasion,
                                    difficulty: (int)Difficulty.medium,
                                    prompt: "Who shoud ask for it? (best choose someone who is proficient in persuasion)"
                                )
                            ],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.HermitsGift)
                        ),
                        GoBackToCrossroads
                    ]
                )
            },
            {(int)EventDesignations.WarriorsAbode,
                new(
                    eventId: (int)EventDesignations.WarriorsAbode,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "A stark place, definitely home to a man.")},
                        {1, new(_narrator, "Sparsely furbished, I mean.")},
                        {2, new(_warrior, "The ogre awaits.")},
                        {3, new(_warrior, "You have the coin?")},
                        {4, new(_warrior, "I don't.")},
                        {5, new(_warrior, "I believe the hermit may be hiding some. He owes me.")}
                    },
                    eventOptions: [
                        new(
                            eventTitle: "Bring the warrior along",
                            eventConditions: [
                                new ConditionTestSceneNotWarriorInParty(_partyId)
                            ],
                            eventEffects: [
                                new ChangeOptionTitle(GoLeft, "Go to the warrior's abode"),
                                new EffectTestSceneAddMemberToPartyRequest(warriorRequest, _partyId)
                            ],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.TestEntry)
                        ),
                        GoBackToCrossroads
                    ]
                )
            },
            {(int)EventDesignations.GetTheKey,
                new(
                    eventId: (int)EventDesignations.GetTheKey,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "The hermit looks rather uncomfortable.")},
                        {1, new(_hermit, "Okay, you got me...")},
                        {2, new(_hermit, "I owe him a couple coins...")},
                        {3, new(_hermit, "Okay, a lot of money.")},
                        {4, new(_hermit, "I have it hereabouts, I'm just sad to part with it.")}

                    },
                    eventOptions: [
                        Stayawhile,
                        GoBackToCrossroads
                    ]
                )
            },
            {(int)EventDesignations.GetTheGold,
                new(
                    eventId: (int)EventDesignations.GetTheGold,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_hermit, "Okay, alright.")},
                        {1, new(_hermit, "Here ya go.")},
                        {2, new(_narrator, "He reaches in a spot you would never think to look.")},
                        {3, new(_narrator, "It's quite a stack of coins.")},
                        {4, new(_narrator, "But you wonder if that's all he has to give.")}
                    },
                    eventOptions: [
                        Stayawhile,
                        GoBackToCrossroads
                    ]
                )
            },
            {(int)EventDesignations.OgresGateway,
                new(
                    eventId: (int)EventDesignations.OgresGateway,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "The gate swings open at your approach, and your money pouch feels the lost weight of missing coins.")},
                        {1, new(_narrator, "That was indeed a steep price.")},
                        {2, new(_narrator, "You hear a bellow from beyond.")},
                        {3, new(_narrator, "It seems the challenger awaits you.")}
                    },
                    eventOptions: [
                        new(
                            eventTitle: "Accept the ogre's challenge",
                            eventConditions: [],
                            eventEffects: [],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.OgreFight)
                        ),
                        GoBackToCrossroads
                    ]
                )
            },
            {(int)EventDesignations.HermitsGift,
                new(
                    eventId: (int)EventDesignations.HermitsGift,
                    conditions: new(
                        conditions: [
                            new ConditionSkillCheckSuccess()
                        ],
                        requirementsNotMetNavigation: new SceneEventNavigation((int)EventDesignations.HermitGiftNotGiven)
                    ),
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_hermit, "Well... I do have one final prize for ye.")},
                        {1, new(_hermit, "I was holdin it in case the time came that I found a worthy champion of me own.")},
                        {2, new(_hermit, "But you might profit from it more than I, I see.")},
                        {3, new(_narrator, "He offers you a choice of three things:")},
                        {4, new(_narrator, "A weapon.")},
                        {5, new(_narrator, "A helmet.")},
                        {6, new(_narrator, "Or a scroll.")}

                    },
                    eventOptions: [
                        new(
                            eventTitle: "Take the weapon",
                            eventConditions: [],
                            eventEffects: [
                                new GiveItem(
                                    targetSelector: new PartyMemberSelector(_partyId),
                                    itemTag: "polearm-ogre-slayer",
                                    prompt: "Who should recieve the weapon?"
                                ),
                                new EffectTestSceneSetHermitGiftedTrue()
                            ],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.YouGotAThing)
                        ),
                        new(
                            eventTitle: "Take the helmet",
                            eventConditions: [],
                            eventEffects: [
                                new GiveItem(
                                    targetSelector: new PartyMemberSelector(_partyId),
                                    itemTag: "helmet-stone",
                                    prompt: "Who should recieve the helmet?"
                                ),
                            new EffectTestSceneSetHermitGiftedTrue()
                            ],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.YouGotAThing)
                        ),
                        new(
                            eventTitle: "Take the scroll",
                            eventConditions: [],
                            eventEffects: [
                                new GiveItem(
                                    targetSelector: new PartyMemberSelector(_partyId),
                                    itemTag: "spell-scroll-shatter",
                                    prompt: "Who should recieve the scroll?"
                                ),
                                new EffectTestSceneSetHermitGiftedTrue()
                            ],
                        eventNavigation: new SceneEventNavigation((int)EventDesignations.YouGotAThing)
                        )
                    ]
                )
            },
            {(int)EventDesignations.HermitGiftNotGiven,
                new(
                    eventId: (int)EventDesignations.HermitGiftNotGiven,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_hermit, "Well...")},
                        {1, new(_hermit, "I'd rather not part with it.")},
                        {2, new(_hermit, "No, I think not.")},
                        {3, new(_narrator, "The hermit turns away.")},
                        {4, new(_narrator, "It seems he wants to help, but he's unconvinced.")},
                        {5, new(_narrator, "Perhaps you can try again later.")},

                    },
                    eventOptions: [
                        Stayawhile,
                        GoBackToCrossroads
                    ]
                )
            },
            {(int)EventDesignations.YouGotAThing,
                new(
                    eventId: (int)EventDesignations.YouGotAThing,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_hermit, "Well there ya go. Best off be goin.")}
                    },
                    eventOptions: [
                        Stayawhile,
                        GoBackToCrossroads
                    ]
                )
            },
            {(int)EventDesignations.OgreFight,
                new(
                    eventId: (int)EventDesignations.OgreFight,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "The gate swings swiftly shut behind you")},
                        {1, new(_narrator, "Ahead of you is the beast, scarred and mindless, with a swarth of minions about him.")},
                        {2, new(_narrator, "He approaches, prepare yourself!")}
                    },
                    eventOptions: [
                        new(
                            eventTitle: "The ogre attacks. Defend yourself!",
                            eventConditions: [],
                            eventEffects: [
                                new StartBattleEffect
                                (
                                    battleStartRequest: new(
                                        partyId: _partyId,
                                        opponentPartyId: "ogre-fight",
                                        bestiaryEntityTags: [
                                            "ogre-brute",
                                            "goblin-vermin",
                                            "goblin-vermin",
                                            "goblin-fireslinger"
                                        ]
                                    )
                                )
                            ],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.BattleWon)
                        )
                    ]
                )
            },
            {(int)EventDesignations.BattleWon,
                new(
                    eventId: (int)EventDesignations.BattleWon,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "That was quite that battle.")},
                        {1, new(_narrator, "Or perhaps it wasn't, I was in fact not paying attention... sorry.")},
                        {2, new(_narrator, "Well, there's one last path to take.")},
                        {3, new(_narrator, "The orge had some treasure.")},

                    },
                    eventOptions: [
                        new(
                            eventTitle: "Investigate the stacks of gems",
                            eventConditions: [],
                            eventEffects: [],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.GemPile)
                        )
                    ]
                )
            },
            {(int)EventDesignations.GemPile,
                new(
                    eventId: (int)EventDesignations.GemPile,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "There is a stack of gems.")},
                        {1, new(_narrator, "Some of them are enchanted, you can see.")},
                        {2, new(_narrator, "On further investigation, the rest are glass, though the enchanted gems look more plain.")},
                        {3, new(_narrator, "Take them?")},

                    },
                    eventOptions: [
                        new(
                            eventTitle: "What a prize!",
                            eventConditions: [],
                            eventEffects: [
                                new GiveItemsArray(
                                    targetSelector: new PartyMemberSelector(_partyId),
                                    _itemTags: ["gem-life", "gem-healing", "gem-fortify", "gem-mana"],
                                    prompt: "Who will take the gems?"
                                )
                            ],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.PrincessTower)
                        )
                    ]
                )
            },
            {(int)EventDesignations.PrincessTower,
                new(
                    eventId: (int)EventDesignations.PrincessTower,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "But it doesn't end there.")},
                        {1, new(_narrator, "The warrior keeps going past, as if he sees a far better prize.")},
                        {2, new(_narrator, "He is moving towards a tower ahead, and at the top you can just see a window.")},
                        {3, new(_narrator, "There is a lady poking her head out of the window, waving at the knight.")},
                        {4, new(_narrator, "It seems he's found his prize, too.")},
                    },
                    eventOptions: [
                        new(
                            eventTitle: "fin. ",
                            eventConditions: [],
                            eventEffects: [],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.TestEnd)
                        )
                    ]
                )
            },
            {(int)EventDesignations.TestEnd,
                new(
                    eventId: (int)EventDesignations.TestEnd,
                    conditions: null,
                    dialogues: new Dictionary<int, Dialogue>()
                    {
                        {0, new(_narrator, "That marks the end.")},
                        {1, new(_narrator, "Thank you for participating in this test scene. I hope everything worked out.")},
                        {2, new(_narrator, "Anyway, you can go now.")}
                    },
                    eventOptions: [
                        new(
                            eventTitle: "fin.",
                            eventConditions: [],
                            eventEffects: [],
                            eventNavigation: new SceneEventNavigation((int)EventDesignations.TestEnd)
                        )
                    ]
                )
            }
        };
    }
}


public sealed class ConditionTestSceneOgresGateClosed : ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        var state = services.GetCurrentSceneState();

        if (state is QuickTestSceneObjectsState q)
        {
            return new(
                success: q.OgresGateOpen == false
            );
        }
        
        return new(
            error: $"Current scene does not have state {nameof(QuickTestSceneObjectsState)}, cannot verify that current scene is {nameof(QuickTestSceneObject)}"
        );
    }
}

public sealed class ConditionTestSceneOgresGateOpen : ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        var state = services.GetCurrentSceneState();

        if (state is QuickTestSceneObjectsState q)
        {
            return new(
                success: q.OgresGateOpen == true
            );
        }
        
        return new(
            error: $"Current scene does not have state {nameof(QuickTestSceneObjectsState)}, cannot verify that current scene is {nameof(QuickTestSceneObject)}"
        );
    }
}

public sealed class ConditionTestSceneNotWarriorInParty(string party_id) : ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        var party = services.EntityService.GetParty(party_id);
        var state = services.GetCurrentSceneState();
        bool result = false;
        if (party is not null && state is QuickTestSceneObjectsState q)
        {
            result = !party.Any(m => m.Id.Equals(q.WarriorId, StringComparison.InvariantCultureIgnoreCase));
            return new(
                success: result
            );
        }
        return new(
            error: $"Party not found with id = {party_id}"
        );
    }
}

public sealed class ConditionTestSceneWarriorInParty(string party_id) : ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        var party = services.EntityService.GetParty(party_id);
        var state = services.GetCurrentSceneState();
        bool result = true;
        if (party is not null && state is QuickTestSceneObjectsState q)
        {
            result = party.Any(m => m.Id.Equals(q.WarriorId, StringComparison.InvariantCultureIgnoreCase));
            return new(
                success: result
            );
        }
        return new(
            error: $"Party not found with id = {party_id}"
        );
    }
}

public sealed class ConditionTestSceneGottenGoldFromHermitLessThan(int timesGottenGold): ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        var state = services.GetCurrentSceneState();

        if (state is QuickTestSceneObjectsState q)
        {
            return new(
                success: q.GottenGold < timesGottenGold
            );
        }
        
        return new(
            error: $"Current scene does not have state {nameof(QuickTestSceneObjectsState)}, cannot verify that current scene is {nameof(QuickTestSceneObject)}"
        );
    }
}

public sealed class ConditionTestSceneNotHermitHasGivenGift: ICondition
{
    public EventResultDto IsMet(EventServices services)
    {
        var state = services.GetCurrentSceneState();

        if (state is QuickTestSceneObjectsState q)
        {
            return new(
                success: q.HermitGifted == false
            );
        }
        return new(
            error: $"Current scene does not have state {nameof(QuickTestSceneObjectsState)}, cannot verify that current scene is {nameof(QuickTestSceneObject)}"
        );
    }
}

public sealed class EffectTestSceneSetOgresGateOpen : IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        var state = services.GetCurrentSceneState();

        if (state is QuickTestSceneObjectsState q)
        {
            if (q.OgresGateOpen)
            {
                return new(
                    error: "Effect is already applied; Ogres gate is open."
                );
            }
            q.OgresGateOpen = true;
            return new(
                success: true
            );
        }
        return new(
            error: $"Current scene does not have the proper state object type: {nameof(QuickTestSceneObjectsState)}"
        );
    }
}

public class EffectTestSceneAddGold(
    ITargetSelector targetSelector,
    string? prompt
): IGameEffect
{
    private static readonly Random r = new();
    public EventResultDto Apply(EventServices services)
    {
        var targets = targetSelector.GetTargets(services);

        string? targetId = services.GetPendingResponse();

        if (targets.Count > 0)
        {
            if (targetId is null)
            {
                if (targets.Count == 1)
                {
                    targetId = targets[0].EntityId;
                }
                else
                {
                    return new(
                        success: false,
                        requiresTarget: true,
                        prompt: prompt ?? "",
                        targets: [..targets]
                    );
                }
            }
            if (targetId is not null)
            {
                var result = services.InventoryService.AddGold(targetId, r.Next(5, 9) * r.Next(5, 9));// between 25 and 81 gold each time.
                if (result is null)
                {
                    return new(
                        error: $"Could not find entity {targetId}"
                    );
                }
                return new(
                    success: true
                );
            }
        }

        return new(
            success: false
        );
    }
}

public sealed class EffectTestSceneIncrementGottenGold: IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        var state = services.GetCurrentSceneState();

        if (state is QuickTestSceneObjectsState q)
        {
            q.GottenGold++;
            return new(
                success: true
            );
        }
        return new(
            error: $"Current scene does not have the proper state object type: {nameof(QuickTestSceneObjectsState)}"
        );
    }
}

public sealed class EffectTestSceneSetHermitGiftedTrue: IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        var state = services.GetCurrentSceneState();

        if (state is QuickTestSceneObjectsState q)
        {
            if (q.HermitGifted)
            {
                return new(
                    error: "Effect already applied; Hermit has already gifted party."
                );
            }
            q.HermitGifted = true;
            return new(
                success: true
            );
        }
        return new(
            error: $"Current scene does not have the proper state object type: {nameof(QuickTestSceneObjectsState)}"
        );
    }
}

public class EffectTestSceneAddMemberToPartyRequest(DamageableEntityRequest entityRequest, string _partyId) : IGameEffect
{
    public EventResultDto Apply(EventServices services)
    {
        var addEntityResult = services.EntityService.AddEntityFromRequest(entityRequest);
        if (addEntityResult.Entity is null)
        {
            return new(
                error: $"There was an issue adding the new entity: {string.Join("\n", addEntityResult.Errors)}"
            );
        }
        var result = services.EntityService.ChangeParty(addEntityResult.Entity.Id, _partyId);
        if (result is null)
        {
            return new(
                error: "We lost the new entity somewhere in transit..."
            );
        }
        var state = services.GetCurrentSceneState();
        if (state is QuickTestSceneObjectsState q)
        {
            q.WarriorId = result.Id;
        }
        else
        {
            return new(
                error: $"Current scene does not have the proper state object type: {nameof(QuickTestSceneObjectsState)}"
            );
        }
        return new(
            success: true
        );
    }
}
