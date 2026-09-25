import { GetActiveBattle, GetParty, UseItem } from "@/lib/api";
import { Battle, Entity, Item, Skill } from "@/lib/types";
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
    const [itemOrSkillsOpen, setItemOrSkillsOpen] = React.useState<op>(0);
    const [itemOrSkillUsing, setItemOrSkillUsing] = React.useState<op>(0);
    const [action, setAction] = React.useState<Action | null>(null);
    const [selectedTargets, setSelectedTargets] = React.useState<string[]>([]);
    const [targetsLimit, setTargetsLimit] = React.useState(0);

    function SetAction(request: Skill | Item, actionType: op) {
        // if selected action is the same as the selected action, deselect
        if (action?.id === request.id) {
            setAction(null);
            setTargetsLimit(0);
            setItemOrSkillUsing(op.closed);
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

    function UseItemOrSkill() {
        if (selectedTargets.length == 0 || !action || !currentEntity) {
            return;
        }
        switch (itemOrSkillUsing) {
            case op.item:
                const result = UseItem(currentEntity.id, action.id, selectedTargets);
                break;
            case op.skill:
                // use skill id = id && targets = selectedTargets
                break;
            case op.default_attack:
                // use default attack id = selectedTarget[0]
                break;
            default:
                console.log(`Chosen action is type ${itemOrSkillUsing}... does not compute!`);
        }
    }

    function SelectTarget(target: string) {
        if (selectedTargets.includes(target)) {
            setSelectedTargets(selectedTargets.filter((i) => i !== target));
        }
        else if (selectedTargets.length >= targetsLimit) {
            return;
        }
        else {
            setSelectedTargets((prev) => [...prev, target]);
        }
    }

    function GetOpponentTurn(target: string) {
        // 
    }

    function ReloadParties(affected: Entity[]) {
        // 
    }

    const currentEntity = 
        battle?.entityDtos.find(
            m => m.id === battle.initiativeOrder[battle.currentRound].entityId
        ) ?? null

    const isPartyTurn = currentEntity?.partyId === battle?.partyId;

    function SetItemOrSkillsOpen(itemOrSkill: op){
        setItemOrSkillsOpen(
            itemOrSkill === 0 || itemOrSkill === itemOrSkillsOpen ? 0 :
            itemOrSkill
        );
    }

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

    return (
        <div style={styles.page}>
            {/* Initiative */}
            <div style={styles.container_initiative}>
                {/*
                    Active member / member:
                        speed
                */}
                {battle?.initiativeOrder.map((member) => 
                    <div key={`initiative-${member.initiative}`}>
                        <p>{member.entityName}</p>
                        <p>Initiative: {member.initiative}</p>
                    </div>
                )}
            </div>

            {/* Action section */}
            <div style={styles.container_actions}>
                <div style={{...styles.buttonsContainer, ...(isPartyTurn ? {display: "none"} : styles.buttonsContainerDisactivated)}} />
                <div style={styles.buttonsContainer}>
                    <div
                        style={styles.action}
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
                <div style={{...styles.confirmButton, ...(action && selectedTargets.length > 0 ? {} : styles.confirmButtonDisactivated)}}>{action ? `Use ${action.name}` : ""}</div>
                
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
                            <p>Health: {member.currentHealth} / {member.maxHealth}</p>
                            <p>Mana: {member.currentMana} / {member.maxMana}</p>
                        </div>
                    )}
                </div>

                {/* Events */}
                <div style={styles.hud_item}>
                    <p>
                        Action selected: {" "}
                        {action? action.name : "None"}
                    </p>
                    {action &&
                        <>
                            <p>Maximium targets: {targetsLimit}</p>
                            {selectedTargets.length > 0 ? (
                                <>
                                    <ul>Targets selected:</ul>
                                    {selectedTargets.map((t, i) =>
                                        <li key={`${i}-${t}`}>{t}</li>
                                    )}
                                </>
                            ) :
                            <p>No targets selected.</p>
                            }
                        </>
                    }
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
                                    <p>Health: {member.currentHealth} / {member.maxHealth}</p>
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
        height: "100%"
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