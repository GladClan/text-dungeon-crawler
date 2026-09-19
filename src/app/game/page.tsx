"use client"

import { ChooseOption, GetCurrentEvent, SelectTarget } from "@/lib/api";
import { Option, TargetOption, Dialogue } from "@/lib/types";
import React from "react";

const GamePage: React.FC = () => {
    const _filler1 = "Here is what a paragraph will look like, all long ant strong, full of words.";
    const _filler_2 = "I didn't want to fill this with some \"lorem ispum\" filler, so I decided I would just type up some stuff and see how it looks on the page. This is a bit longer to see how a longer phrase will look as it wraps around";
    const _filler_3 = "And here is a repeat of the ifrst two sentences to see what a really long one will look like: " + _filler1 + " " + _filler_2;

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
            if (result.RequiresTarget) {
                setTargetOptions(result.Targets);
            } else {
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
            await SelectTarget(targetId);
        } catch (err) {
            const message = err instanceof Error ? err.message : "There was a problem with the target selection...";
            console.log(message);
        } finally {
            await SendOption(optionId);
            setLoading(false);
        }
    }

    const [loading, setLoading] = React.useState(false);
    const [previousDialogue, setPreviousDialogue] = React.useState<Dialogue[]>([]);
    const [dialogue, setDialogue] = React.useState<Dialogue[] | null>();
    const [options, setOptions] = React.useState<Option[] | null>();
    const [targetOptions, setTargetOptions] = React.useState<TargetOption[] | null>();
    return (
        <div style={styles.page}>
            <div style={styles.container}>
                <div style={styles.dialogue}>
                    {loading ? 
                        <div style={styles.item}>
                            Loading...
                        </div>
                        :
                        <>
                            {dialogue ? (
                                [...dialogue, ...previousDialogue].map((d, i) =>
                                    <div key={`dialogue-${i}`} style={styles.item}>{d.message}</div>
                                )
                            ) : (
                                <>
                                    <div style={styles.item}>{_filler1}</div>
                                    <div style={styles.item}>{_filler_2}</div>
                                    <div style={styles.item}>{_filler_3}</div>
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
                                    <button key={`option-${i}`} style={styles.button} onClick={() => SendOption(o.optionId)}>
                                        {o.optionTitle}
                                        {targetOptions && (
                                            <div style={styles.targetsContainer}>
                                                {targetOptions.map((t, n) => 
                                                    <button
                                                        key={`target-${n}`}
                                                        style={styles.targetsButton}
                                                        onClick={() => ChooseTarget(
                                                            t.EntityId,
                                                            o.optionId
                                                        )}
                                                    >
                                                        {t.Name}
                                                    </button>
                                                )}
                                            </div>
                                        )}
                                    </button>
                                ))}
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
        top: "10px",
        left: "10px",
        cursor: "pointer",
        fontSize: "16px",
        padding: "5px",
        boxShadow: "0 4px 8px rgba(0, 0, 0, 0.2)",
        borderRadius: "8px",
        zIndex: 10,
    },
    targetsButton: {
        cursor: "pointer",
        minWidth: "75px",
        margin: "5px",
        // 
    }
}