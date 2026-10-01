import { DefaultAttack, GetActiveBattle, GetOpponentTurn, GetParty, UseItem, UseSkill } from "@/lib/api";
import { Battle, Entity, Item, Skill, TurnOver } from "@/lib/types";
import React from "react";

interface props {
    setBattleActive: (value: boolean) => void  //React.Dispatch<React.SetStateAction<boolean>>
}

type Action = {
    name: string;
    id: string;
}

enum op { // short of "options"
    closed = 0,
    item = 1,
    skill = 2,
    default_attack = 3
}

const BattleView: React.FC<props> = ({ setBattleActive }) => {
    const [battle, setBattle] = React.useState<Battle | null>(null);
    const [party, setParty] = React.useState<Entity[] | null>(null);
    const [opponents, setOpponents] = React.useState<Entity[] | null>(null);
    const [messages, setMessages] = React.useState<string[]>([]);
    const [itemOrSkillsOpen, setItemOrSkillsOpen] = React.useState<op>(0);
    const [itemOrSkillUsing, setItemOrSkillUsing] = React.useState<op>(0);
    const [action, setAction] = React.useState<Action | null>(null);
    const [selectedTargets, setSelectedTargets] = React.useState<string[]>([]);
    const [targetsLimit, setTargetsLimit] = React.useState(0);
    const opponentTurnRunning = React.useRef(false);

    React.useEffect(() => {
        // Get the party using the partyId parameter from the constructor
        async function LoadResources() {
            try {
                const battleResult = await GetActiveBattle();
                setBattle(battleResult);
                const partyId = battleResult.partyId;
                const opponentPartyId = battleResult.opponentPartyId;
                const partyResult = await GetParty(partyId);
                const opponentsResult = await GetParty(opponentPartyId);
                setParty(partyResult);
                setOpponents(opponentsResult);
            } catch (err) {
                const message = err instanceof Error ? err.message : "Failed to load battle";
                console.log(message);
            } finally {
                // 
            }
        }

        LoadResources();
    }, []);

    const currentEntity = 
        battle?.entityDtos.find(
            m => m.id === battle.initiativeOrder[battle.currentRound % battle.initiativeOrder.length].entityId
        ) ?? null

    const isPartyTurn = currentEntity?.partyId === battle?.partyId;

    function SetItemOrSkillsOpen(itemOrSkill: op){
        setItemOrSkillsOpen(
            itemOrSkill === 0 || itemOrSkill === itemOrSkillsOpen ? 0 :
            itemOrSkill
        );
    }

    function SetAction(
        request: Pick<Skill | Item, "id" | "name" | "targetsLimit">,
        actionType: op
    ) {
        setItemOrSkillsOpen(op.closed);

        // if selected action is the same as the selected action, deselect
        if (action?.id === request.id) {
            setAction(null);
            setTargetsLimit(0);
            setItemOrSkillUsing(op.closed);
            setSelectedTargets([]);
        }
        else {
            setAction({name: request.name, id: request.id});
            setItemOrSkillUsing(actionType);
            setTargetsLimit(request.targetsLimit);
            setTargetsLimit(request.targetsLimit);
        }
        // if selected targets exceeds targets limit, cut the excess
        if (selectedTargets.length > targetsLimit) {
            setSelectedTargets((prev) => prev.slice(0, targetsLimit));
        }
    }

    async function UseItemOrSkill() {
        setItemOrSkillsOpen(op.closed);
        if (selectedTargets.length == 0 || !action || !currentEntity) {
            return;
        }

        let result: TurnOver | null = null;
        switch (itemOrSkillUsing) {
            case op.item:
                result = await UseItem(currentEntity.id, action.id, selectedTargets);
                break;
            case op.skill:
                result = await UseSkill(currentEntity.id, action.id, selectedTargets);
                break;
            case op.default_attack:
                result = await DefaultAttack(currentEntity.id, selectedTargets[0]);
                // use default attack id = selectedTarget[0]
                break;
            default:
                console.log(`Chosen action is type ${itemOrSkillUsing}... does not compute!`);
        }

        if (result !== null){
            ReloadParties(result);
        }
    }

    function SelectTarget(target: string) {
        setItemOrSkillsOpen(op.closed);
        if (selectedTargets.includes(target)) {
            setSelectedTargets(selectedTargets.filter((i) => i !== target));
        }
        else if (selectedTargets.length >= targetsLimit && action) {
            setSelectedTargets((prev) => [...prev.slice(1), target]);
        }
        else if (action) {
            setSelectedTargets((prev) => [...prev, target]);
        } else {
            return;
        }
    }

    // Get opponent turn
    React.useEffect(() => {
        if (isPartyTurn) {
            return;
        }

        opponentTurnRunning.current = true;

        async function RunOpponentTurn() {
            if (!battle || !currentEntity) {
                return;
            }

            try {
                // Give it a pause for presentation
                await new Promise(resolve => setTimeout(resolve, 750));

                const result = await GetOpponentTurn(currentEntity.id);
                ReloadParties(result);
            } catch (err) {
                console.log("Opponent turn failed.");
                console.log(err);
            } finally {
                opponentTurnRunning.current = false;
            }
        }

        RunOpponentTurn();
    }, [battle, currentEntity, isPartyTurn]);

    function ReloadParties(newTurn: TurnOver) {
        setItemOrSkillsOpen(op.closed);
        setSelectedTargets([]);
        setAction(null);

        const updatedParty = party;
        if (party && opponents) {
            if (newTurn.affectedEntities.some(m => m.partyId === battle?.partyId)) {
                newTurn.affectedEntities.forEach(e => {
                    if (e.partyId === battle?.partyId) {
                        const target = party.find(m => m.id === e.id);
                        if (target) {
                            party[
                                party.indexOf(target)
                            ] = e;
                        }
                    }
                });
                setParty(updatedParty);
            }

            const updatedOpponents = opponents;
            if (newTurn.affectedEntities.some(m => m.partyId === battle?.opponentPartyId)) {
                newTurn.affectedEntities.forEach(e => {
                    if (e.partyId === battle?.opponentPartyId) {
                        const target = opponents.find(m => m.id === e.id);
                        if (target) {
                            opponents[
                                opponents.indexOf(target)
                            ] = e;
                        }
                    }
                });
                setOpponents(updatedOpponents);
            }
        }

        setBattle(
            battle ?
                {
                    partyId: battle.partyId,
                    opponentPartyId: battle.opponentPartyId,
                    currentRound: newTurn.currentTurn,
                    initiativeOrder: newTurn.initiativeOrder,
                    entityDtos: (party && opponents ? [...party, ...opponents] : battle.entityDtos)
                }
                : battle
        );
        setMessages(prev => [...prev, ...newTurn.messages]);
    }

    return (
        <div style={styles.page}>
            {/* Initiative */}
            <div style={styles.container_initiative}>
                {battle?.initiativeOrder.map((member) => 
                    <div
                        key={`initiative-${member.initiative}`}
                        style={{...(member.entityId === currentEntity?.id ? {boxShadow: "0 0 20px #ddd000", padding: "5px"} : {})}}
                    >
                        <p>{member.entityName}</p>
                        <p>Initiative: {member.initiative}</p>
                        {/* member speed */}
                    </div>
                )}
            </div>

            {/* Action section */}
            <div style={styles.container_actions}>
                <div style={{...styles.buttonsContainer, ...(isPartyTurn ? {display: "none"} : styles.buttonsContainerDisactivated)}} />
                <div style={styles.buttonsContainer}>
                    <div
                        style={styles.action}
                        onClick={() => SetAction(
                            {
                                id: "default",
                                name: "unarmed strike",
                                targetsLimit: 1
                            },
                            op.default_attack
                        )}
                    >
                        Attack
                    </div>
                    <div
                        style={styles.action}
                    >
                        Defend
                    </div>
                    <div
                        style={styles.action}
                        onClick={() => SetItemOrSkillsOpen(op.item)}
                    >
                        Use Item
                        {itemOrSkillsOpen === op.item &&
                            <div style={styles.action_dropdown}>
                                {isPartyTurn && currentEntity && (
                                    currentEntity.inventory.items.length > 0 ? 
                                    currentEntity.inventory.items.map(item =>
                                        // (item.canUse &&
                                            <p
                                                key={item.id}
                                                title={item.description}
                                                onClick={() => SetAction(item, op.item)}
                                            >
                                                {item.name}
                                            </p>
                                        // )
                                    ) :
                                    <p>No items</p>
                                )
                                }
                            </div>
                        }
                    </div>
                    <div
                        style={styles.action}
                        onClick={() => SetItemOrSkillsOpen(op.skill)}
                    >
                        Use Skill
                        {itemOrSkillsOpen === op.skill &&
                            <div style={styles.action_dropdown}>
                                {isPartyTurn && currentEntity && (
                                    currentEntity.skills.length > 0 ?
                                    currentEntity.skills.map((skill) =>
                                        <p
                                            key={skill.id}
                                            title={skill.description}
                                            onClick={() => SetAction(skill, op.skill)}
                                        >
                                            {skill.name}
                                        </p>
                                    ) :
                                    <p>No skills</p>
                                )}
                            </div>
                        }
                    </div>
                </div>

                {/* Confirm button (checks that an item/attack has been selected and the targets have also been selected) */}
                <div
                    style={{
                        ...styles.confirmButton,
                        ...(action && selectedTargets.length > 0 ?
                            {} :
                            styles.confirmButtonDisactivated
                        )
                    }}
                    onClick={() => UseItemOrSkill()}
                >
                    {action ? `Use ${action.name}` : ""}
                </div>
                
            </div>

            {/* Heads-up display */}
            <div style={styles.hud}>

                {/* Party */}
                <div style={styles.hud_item}>
                    {party && party.map((member) => 
                        <div
                            key={member.id}
                            style={{
                                ...styles.party_member,
                                ...(action ?
                                    {cursor: "pointer"} :
                                    {}
                                ),
                                ...(selectedTargets.includes(member.id) ?
                                    styles.party_member_selected :
                                    {}
                                )
                            }}
                            onClick={() => SelectTarget(member.id)}
                        >
                            <h2>{member.name}</h2>
                            <p>Level: {member.level}</p>
                            <p>Speed: {member.speed}</p>
                            <p>Health: {parseFloat(member.currentHealth.toFixed(3))} / {member.maxHealth}
                                {member.healthBuffer > 0 && 
                                    <span style={{marginLeft: "20px"}}>Buffer: {member.healthBuffer}</span>
                                }
                            </p>
                            <p>Mana: {member.currentMana} / {member.maxMana}</p>
                        </div>
                    )}
                </div>

                {/* Events */}
                <div style={styles.hud_item}>
                    {messages.map((message, i) =>
                        <p
                            style={{marginBottom: "20px"}}
                            key={i}
                        >
                            {message}
                        </p>
                    )}
                </div>

                {/* Enemies */}
                <div style={styles.hud_item}>
                    {opponents && opponents.map((member) =>
                        <div
                            key={member.id}
                            style={{
                                ...styles.party_member,
                                ...(action ?
                                    {cursor: "pointer"} :
                                    {}
                                ),
                                ...(selectedTargets.includes(member.id) ?
                                    styles.party_member_selected :
                                    {}
                                )
                            }}
                            onClick={() => SelectTarget(member.id)}
                        >
                            <h2>{member.name}</h2>
                            {member.displayStats &&
                                <>
                                    <p>Level: {member.level}</p>
                                    <p>Speed: {member.speed}</p>
                                    <p>Health: {parseFloat(member.currentHealth.toFixed(3))} / {member.maxHealth}</p>
                                    <p>Mana: {member.currentMana} / {member.maxMana}</p>
                                </>
                            }
                        </div>
                    )}
                </div>
            </div>
        </div>
    )
}

export default BattleView;

const styles: { [key: string]: React.CSSProperties} = {
    page: {
        position: "fixed",
        inset: 0,
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        padding: "25px",
        background: "linear-gradient(135deg, #3e0d00, #340e00)",
        overflow: "auto",
        zIndex: 99,
    },
    container_initiative: {
        display: "flex",
        flexDirection: "row",
        gap: "20px",
        height: "10vh",
        maxWidth: "85vw",
        margin: "25px",
        padding: "10px",
        background: "linear-gradient(135deg, #160d00, #342000)",
        border: "2px solid #d7a82f"
    },
    container_actions: {
        position: "relative",
        display: "grid",
        gridTemplateColumns: "2fr 1fr",
        gap: "20px",
        height: "7vh",
        width: "65vw",
        margin: "25px",
        background: "linear-gradient(135deg, #4d4400, #5d5305)",
        border: "2px solid #d7a82f"
    },
    buttonsContainer: {
        display: "flex",
        flexDirection: "row",
        justifyContent: "space-evenly",
        alignItems: "center"
    },
    confirmButton: {
        width: "50%",
        height: "65%",
        alignSelf: "center",
        justifySelf: "center",
        alignContent: "center",
        textAlign: "center",
        color: "black",
        background: "radial-gradient(#ffff60, #ffdd00)",
        border: "2px solid #d7a82f",
        boxShadow: "0 0 20px #ddd000",
        cursor: "pointer"
    },
    confirmButtonDisactivated: {
        background: "none",
        boxShadow: "none",
        color: "yellow",
        cursor: "initial"
    },
    buttonsContainerDisactivated: {
        position: "absolute",
        inset: 0,
        background: "linear-gradient(325deg, rgba(0,0,0,0.5), rgba(20,0,0,0.9))",
        zIndex: 50,
    },
    action: {
        position: "relative",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        minWidth: "20%",
        height: "50%",
        color: "black",
        background: "radial-gradient(#fff23f, #ffbb00)",
        cursor: "pointer",
        userSelect: "none"
    },
    action_dropdown: {
        position: "absolute",
        top: "100%",
        left: 0,
        display: "flex",
        flexDirection: "column",
        minWidth: "200px",
        minHeight: "20px",
        background: "linear-gradient(#ffbb00, #c18e01)"
    },
    hud: {
        display: "grid",
        gridTemplateColumns: "1fr 1fr 1fr",
        gap: "5vw",
        height: "100%",
        maxHeight: "70vh",
    },
    hud_item: {
        height: "100%",
        width: "25vw",
        background: "linear-gradient(135deg, rgba(50, 20, 20, 0.2), rgba(70, 30, 30, 0.5))",
        overflow: "auto"
    },
    party_member: {
        margin: "2%",
        padding: "3%",
        border: "1px solid #446",
        userSelect: "none"
    },
    party_member_selected: {
        backgroundColor: "rgba(255,255,130,0.2)"
    }
}