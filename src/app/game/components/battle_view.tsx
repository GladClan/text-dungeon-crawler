import { GetActiveBattle } from "@/lib/api";
import { Battle } from "@/lib/types";
import React from "react";

interface props {
    setBattleActive: (value: boolean) => void  //React.Dispatch<React.SetStateAction<boolean>>
}

const BattleView: React.FC<props> = ({ setBattleActive }) => {
    const [battle, setBattle] = React.useState<Battle | null>();
    const [refresh, setRefresh] = React.useState(false);


    React.useEffect(() => {
        // Get the party using the partyId parameter from the constructor
        async function LoadParty() {
            try {
                const result = await GetActiveBattle();
                setBattle(result);
            } catch (err) {
                const message = err instanceof Error ? err.message : "Failed to load battle";
                console.log(message);
            } finally {
                // 
            }
        }

        LoadParty();
    }, [refresh]);

    return (
        <div style={styles.page}>
            {/* Initiative */}
            <div style={styles.container_initiative}>
                {/*
                    Active member / member:
                        speed
                */}
                {battle?.initiativeOrder.map((member, index) => 
                    <div key={`initiative-${member.initiative}`}>
                        <p>{member.entityName}</p>
                        <p>{member.initiative}</p>
                    </div>
                )}
            </div>
            {/* Action section */}
            <div>
                {/*
                    Actions:
                        attack,
                        defend,
                        items,
                        skills
                */}
                {/* Confirm button (checks that an item/attack has been selected and the targets have also been selected) */}
            </div>
            {/* Heads-up display */}
            <div>
                {/* 
                    Party:
                        name,
                        level,
                        health,
                        mana 
                        key = id
                */}
                {/* Events */}
                {/*
                    Enemies:
                        name,
                        (if visible)
                            level,
                            health,
                            mana
                        key = id
                */}
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
        // justifyContent: "center",
        alignItems: "center",
        padding: "50px",
        background: "linear-gradient(135deg, #3e0d00, #340e00)",
        overflow: "auto",
        zIndex: 99,
    },
    container_initiative: {
        display: "flex",
        flexDirection: "row",
        gap: "20px",
        height: "15vh",
        maxWidth: "85vw",
        background: "linear-gradient(135deg, #160d00, #342000)",
        border: "2px solid #d7a82f"
    }
}
/*

const styles: { [key: string]: React.CSSProperties} = {
    modal: {
        position: "fixed",
        inset: 0,
        display: "flex",
        backgroundColor: "rgba(0, 0, 0, 0.5)",
        backdropFilter: "blur(10px)",
        zIndex: 99,
    },
    container: {
        alignSelf: "center",
        width: "90%",
        height: "90%",
        background: "linear-gradient(135deg, #160d00, #342000)",
        margin: "10%",
        overflow: "auto",
    },
    button: {
        cursor: "pointer",
        backgroundColor: "#e6d3a3",
        margin: "20px",
        padding: "5px",
        borderRadius: "50px",
    },
    memberslist: {
        marginLeft: "20px",
        display: "flex",
        flexDirection: "row",
        // flex: "1 1 200px",
        flexWrap: "wrap",
        gap: "10px",
    },
    card: {
        width: "250px",
        boxShadow: "0 0 20px rgba(139,105,20,0.7)",
        padding: "10px",
        // alignItems: "normal",
    },
    selector: {
        cursor: "pointer",
        textDecoration: "underline"
    }
}
*/