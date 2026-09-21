"use client"

import { ChooseOption, GetCurrentEvent, SelectTarget } from "@/lib/api";
import { Option, TargetOption, Dialogue } from "@/lib/types";
import React from "react";
import PartyViewer from "./components/party_view";
import BattleView from "./components/battle_view";

type TargetOptions = {
    message: string,
    optionId: number,
    options: TargetOption[],
}

const GamePage: React.FC = () => {

    async function GetEvent() {
        try {
            setLoading(true);
            // setPreviousDialogue([...previousDialogue, ...(dialogue? dialogue : [])]);
            const newEvent = await GetCurrentEvent();
            if (newEvent.existsActiveBattle){
                setDialogue([{
                    orderId: 0,
                    source: "master program",
                    message: "A battle begins..."
                }]);
                setOptions(null);
                setBattleActive(true);
            } else {
                setDialogue(newEvent.dialogues);
                setOptions(newEvent.options);
            }
        }
        catch (err)
        {
            const message = err instanceof Error ? err.message : "Failed to get next event.";
            console.log(message);
        }
        finally{
            setLoading(false);
        }
    }

    async function SendOption(optionId: number){
        try {
            setLoading(true);
            const result = await ChooseOption(optionId);
            if (result.requiresTarget) {
                setTargetOptions({
                    message: result.message,
                    optionId: optionId,
                    options: result.targets
                });
                setViewParty(false);
            } else {
                setTargetOptions(null);
                await GetEvent();
            }
        }
        catch (err) {
            const message = err instanceof Error ? err.message : "There was a problem processing your choice...";
            console.log(message);
        } finally {
            setLoading(false);
        }
    }

    async function ChooseTarget(targetId: string, optionId: number) {
        try {
            setLoading(true);
            const result = await SelectTarget(targetId);
            setTargetOptions(null);
        } catch (err) {
            const message = err instanceof Error ? err.message : "There was a problem with the target selection...";
            console.log(message);
        } finally {
            await SendOption(optionId);
            setLoading(false);
        }
    }

    const [battleActive, setBattleActive] = React.useState(false);
    const [loading, setLoading] = React.useState(false);
    const [previousDialogue, setPreviousDialogue] = React.useState<Dialogue[]>([]);
    const [dialogue, setDialogue] = React.useState<Dialogue[] | null>();
    const [options, setOptions] = React.useState<Option[] | null>();
    const [targetOptions, setTargetOptions] = React.useState<TargetOptions | null>();
    const [viewParty, setViewParty] = React.useState(false);
    const [partyId, setPartyId] = React.useState("player-party");

    return (
        <div style={styles.page}>
            {viewParty ? 
                <PartyViewer
                    partyId={partyId}
                    setViewParty={setViewParty}
                />
                :
                <button
                    style={styles.button}
                    onClick={() => setViewParty(!viewParty)}
                >
                    View party
                </button>
            }
            {battleActive &&
                <BattleView
                    setBattleActive={setBattleActive}
                />
            }
            <div style={styles.container}>
                <div style={styles.dialogue}>
                    {loading ? 
                        <div style={styles.item}>
                            Loading...
                        </div>
                        :
                        <>
                            {dialogue ? (
                                [...dialogue].map((d, i) =>
                                    <div key={`dialogue-${i}`} style={styles.item}>
                                        <ul>
                                            <span style={{fontSize: "14px"}}>{d.source}</span>
                                            <li style={{marginLeft: "20px", listStyleType: "none"}}>
                                                {d.message}
                                            </li>
                                        </ul>
                                    </div>
                                )
                            ) : (
                                <>
                                    <div style={styles.item}>Press "Begin" to start the adventure :)</div>
                                </>
                            )}
                        </>
                    }
                </div>
                <div style={styles.options}>
                    {options ?                     
                        (
                            <>
                                {options.map((o, i) => (
                                        <button
                                            key={`option-${i}`}
                                            style={styles.button}
                                            onClick={() => SendOption(o.optionId)}
                                        >
                                            {o.optionTitle}
                                        </button>
                                ))}
                                {targetOptions && (
                                    <div style={styles.targetsContainer}>
                                        <p style={{color: "black", fontSize: "18px"}}>
                                            <strong>{targetOptions.message}</strong>
                                        </p>
                                        {targetOptions.options.map((t, n) => 
                                            <div
                                                key={`target-${n}`}
                                                style={styles.targetsButton}
                                                onClick={() => ChooseTarget(
                                                    t.entityId,
                                                    targetOptions.optionId
                                                )}
                                            >
                                                {t.name}
                                            </div>
                                        )}
                                    </div>
                                )}
                            </>
                        ) : (
                            <button style={styles.button} onClick={() => GetEvent()}>
                                Begin
                            </button>
                        )
                    }
                </div>
            </div>
        </div>
    );
}

export default GamePage;
const styles: { [key: string]: React.CSSProperties} = {
    page: {
        // display: "grid",
        // gridTemplateRows: "20px 1fr 20px",
        alignItems: "center",
        justifyItems: "center",
        width: "100%",
        minHeight: "100svh",
        padding: "80px",
        // gap: "20px",
        // fontFamily: "geist sans, sans-serif",
        fontFamily: "Garamond, serif",
        fontSize: "16px",
    },
    container: {
        position: "relative",
        display: "flex",
        flexDirection: "column",
        padding: "10px",
        width: "60vw",
        height: "60vh",
        alignItems: "center",
        border: "1px solid #e6d3a3",
    },
    dialogue: {
        minWidth: "90%",
        boxShadow: "0 0 20px rgba(139,105,20,0.7)",
    },
    item: {
        padding: "10px",
        alignItems: "normal",
    },
    options: {
        position: "absolute",
        bottom: 10,
    },
    button: {
        position: "relative",
        cursor: "pointer",
        fontSize: "18px",
        minWidth: "90px",
        margin: "5px",
        padding: "10px",
        border: "1px solid rgb(139, 105, 20)",
        borderRadius: "5px",
        background: "#e6d3a3",
    },
    targetsContainer: {
        display: "flex",
        flexDirection: "column",
        position: "absolute",
        top: "50px",
        // left: "0px",
        cursor: "pointer",
        fontSize: "16px",
        padding: "5px",
        backgroundColor: "#e6d3a3",
        boxShadow: "0 4px 8px rgba(255, 255, 0, 0.7)",
        borderRadius: "8px",
        zIndex: 10,
    },
    targetsButton: {
        color: "black",
        cursor: "pointer",
        textDecoration: "underline",
        minWidth: "75px",
        margin: "5px",
    }
}