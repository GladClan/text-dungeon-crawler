"use client"
import React from "react";
import { gameContext } from "@/context/gameContext";
import { Entity } from "@/lib/obj/entity/entity";
import { getTurnOrder, isParty } from "@/lib/battleFunctions";
import TurnOrderCard from "./components/turnOrderCard";
import PartyMemberCard from "@/components/partyMemberCard";
import PartySection from "./components/partySection";
import ActionButtons from "./components/actionButtons";

const BattlePage: React.FC = () => {
    const contextValue = React.useContext(gameContext);
    if (!contextValue) {
        throw new Error("gameContext is not available, BattlePage must be used within a gameContext provider!")
    }
    const {party, guests, pets, enemies, setGameState, eventTime} = contextValue;

    const [turnOrder] = React.useState(getTurnOrder([...party, ...guests, ...pets, ...enemies]));
    const [currentEntity, setCurrentEntity] = React.useState(turnOrder[0]);

    const isCurrentEntity = (entity: Entity) => {
        return entity.getEntityId() === currentEntity.getEntityId();
    }

    const [turn, setTurn] = React.useState(1);
    const [round, setRound] = React.useState(0);
    function nextTurn() {
        setTurn((turn >= turnOrder.length - 1) ? 0 : (turn + 1));
        setCurrentEntity(turnOrder[turn]);
        setRound(round+1);
        if (round > 10) {
            console.log("what are you doing?");
            setRound(0);
        }
    }

    return(
        <div style={styles.container}>

            {/* Header displaying turn order */}
            <div style={styles.turnOrderContainer}>
                { turnOrder.map((member, index) => (
                    <div key={index}>
                        <TurnOrderCard
                            member={member}
                            isCurrentEntity={isCurrentEntity(member)}
                            isParty={isParty([...party, ...guests, ...pets], member)}
                        />
                    </div>
                ))}
            </div>

            <div style={{display: "flex", flexDirection: "row"}}>
                <div style={styles.section}>
                    {/* party */}
                    <PartySection
                        party={party}
                        guests={guests}
                        pets={pets}
                        isCurrentEntity={isCurrentEntity}
                    />
                    {/* actions */}
                    <ActionButtons
                        member={currentEntity}
                        isActive={isParty([...party, ...pets], currentEntity)}
                    />
                    {/* events log */}
                    {/* enemies section */}
                </div>
                <div style={styles.section}>
                    {}
                </div>
                <div style={styles.section}>
                    {}
                </div>
                <div>
                    <button
                        onClick={() => nextTurn()}
                        >
                        Next turn: {turn + 1}
                    </button>
                </div>
            </div>
        </div>
    )
}

export default BattlePage;

const styles: { [key: string]: React.CSSProperties } = {
    container: {
        display: "flex",
        flexDirection: "column",
        minHeight: "100vh",
        padding: "20px",
        fontFamily: "Garamond, serif",
        background: "linear-gradient(135deg, #2c1810 0%, #1a0f0a 50%, #0f0704 100%)",
        color: "#e6d3a3",
    },
    turnOrderContainer: {
        display: "flex",
        flexDirection: "row",
        marginBottom: "20px",
        padding: "15px",
        border: "3px solid #8b6914",
        borderRadius: "12px",
        background: "linear-gradient(145deg, #1a1611 0%, #0d0b08 100%)",
        boxShadow: "inset 0 0 20px rgba(139,105,20,0.3)",
        overflowX: "auto"
    },
    section: {
        display: "flex",
        flexDirection: "row",
        // minWidth: "100px",
        maxWidth: "30vw",
        // border: "2px solid #fff"
    },
}