"use client"
import React, { useState } from "react";
import { useRouter } from "next/navigation";

import { gameContext } from "@/context/gameContext";
import { gamestate } from "@/lib/gamestateLib";
import { toTitleCase } from "@/lib/QOLFunctions";
import WigglyButton from "@/components/wigglyButton";
import PartyMemberCard from "@/components/partyMemberCard";

const BattleResultPage: React.FC = () => {
    const router = useRouter();
    const contextValue = React.useContext(gameContext);
    if (!contextValue) {
        throw new Error("Game context is not available!");
    }
    const { party, enemies, setEnemies, gameState } = contextValue;

    const [expGain, setExpGain] = useState<number>(0);
    // const [expAtStart] = useState<number[]>(party.map(member => member.getStats().getExperience()));
    // const [expAfter, setExpAfter] = useState<number[]>([])
    // const [levelAtStart] = useState<number[]>(party.map(member => member.getStats().getLevel()));
    // const [levelAfter, setLevelAfter] = useState<number[]>([])
    // const [proficiencyAtStart] = useState<{[key: string]: number}[]>(party.map(member => member.getStats().getAllProficiencies()))
    // const [proficiencyAfter, setProficiencyAfter] = useState<{[key: string]: number}[]>([])

    React.useEffect(() => {
        const expFromEnemies = enemies.reduce((total, enemy) => total + enemy.getStats().getExperience(), 0);
        if (gameState === gamestate.BattleVictory) {
            setExpGain(expFromEnemies);
            // party.forEach(member => {
            //         member.getStats().addExperience(expGain);
            //         setExpAfter(prev => [...prev, member.getStats().getExperience()]);
            //         setLevelAfter(prev => [...prev, member.getStats().getLevel()]);
            //         setProficiencyAfter(prev => [...prev, member.getStats().getAllProficiencies()]);
            //     });
            setEnemies([]);
        }
    }, [])

    return (
        <div style={styles.container}>
            {gameState === gamestate.BattleVictory ? (
                <>
                    <div style={styles.title}>
                        Victory!
                    </div>
                    <div style={styles.options}>
                        The party gained {expGain} experience for defeating the enemies!
                    </div>
                    <div style={styles.partyContainer}>
                        {party.map((member, index) => {
                            const statsClone = member.getStats().clone();
                            statsClone.addExperience(expGain);
                            return (
                                <div test-id={`${index}`} key={index} style={{display: 'flex', flexDirection: 'column'}}>
                                    <PartyMemberCard
                                        index={index}
                                        member={member}
                                        isActive={false}
                                    />
                                    <div> {/* This div for the exp gain animation?? */}
                                        {member.getStats().getLevel() < statsClone.getLevel() ? (
                                            <div style={{...styles.box, ...styles.newStatsBox}}>
                                                <div style={{fontSize: 16}}>Level: {member.getStats().getLevel()} ➣ {statsClone.getLevel()}</div>
                                                <div style={{fontSize: 14}}>Experience: {statsClone.getExperience()}</div>
                                                <div style={{fontSize: 12}}>Experience to next level: {statsClone.getExperienceForNextLevel() - statsClone.getExperience()}</div>
                                                <ul style={{marginTop: 10, fontSize: 16, textDecoration: 'italicized'}}>Proficiencies increased:
                                                    {Object.entries(statsClone.getAllProficiencies()).map(([key, value]) => 
                                                        <li key={key} style={{marginLeft: 10, fontSize: 14, textDecoration: 'none', listStyleType: 'none'}}>
                                                            {`${toTitleCase(key)}: `}{member.getStats().getProficiency(key) < value ? `${member.getStats().getProficiency(key).toFixed(2)} ➣ ${value.toFixed(2)}` : `${value.toFixed(2)}`}
                                                            {/* {toTitleCase(key)}:{' '}
                                                            {member.getStats().getProficiency(key).toFixed(2)}
                                                            {' '}➣ {value.toFixed(2)} */}
                                                        </li>
                                                    )}
                                                </ul>
                                            </div>
                                        ) : (
                                            <div style={styles.box}>
                                                <div style={{fontSize: 16}}>Level: {statsClone.getLevel()}</div>
                                                <div style={{fontSize: 14}}>Experience: {member.getStats().getExperience()} ➣ {statsClone.getExperience()}</div>
                                                <div style={{fontSize: 12}}>Experience to next level: {statsClone.getExperienceForNextLevel() - statsClone.getExperience()}</div>
                                            </div>
                                        )}
                                    </div>
                                    {/* another div for proficiency gains? to play once finished leveling up? Or perhaps I'll do that in a tooltip pop-up stlye sort of thing? Don't forget to sort them... somehow. */}
                                </div>
                            )
                        })}
                    </div>
                    <div style={{marginTop: 20}}>
                        <WigglyButton func={() => (
                                party.forEach(member => member.getStats().addExperience(expGain)),
                                router.push('/')
                            )}>
                            continue
                        </WigglyButton>
                    </div>
                </>
            ) : gameState == gamestate.PartyDefeat ? (
                <>
                    <div style={styles.title}>
                        Your party has been defeated...
                    </div>
                    <div style={styles.options}>
                        <WigglyButton
                            func={() => router.push('/story')}
                        >
                            Reload last save point
                        </WigglyButton>
                        <WigglyButton func={() => (router.push('/'))}>
                            Exit to main menu
                        </WigglyButton>
                    </div>
                </>
            ) : (
                <>
                    <div style={styles.title}>
                        ...What?
                    </div>
                    <WigglyButton func={() => router.push('/')}>
                        Go home
                    </WigglyButton>
                </>
            )}
    </div>
    )
}

export default BattleResultPage;

const styles: ({ [key: string]: React.CSSProperties}) = {
    container: {
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        minHeight: "100vh",
        padding: "20px",
        fontFamily: "Garamond, serif",
        background: "linear-gradient(135deg, #2c1810 0%, #1a0f0a 50%, #0f0704 100%)",
        color: "#e6d3a3",
    },
    title: {
        fontSize: "26px",
        textShadow: "2px 2px 4px rgba(255, 126, 126, 1.5)",
        marginBottom: "30px"
    },
    partyContainer: {
        paddingTop: "12px",
        display: "flex",
        flexDirection: "row",
        gap: "25px",
    },
    options: {
        display: "flex",
        flexDirection: "column",
        height: "100%",
        // justifyContent: "center",
        alignItems: "center",
        gap: "15px",
    },
    button: {
        padding: "12px 24px",
        fontSize: "18px",
        fontFamily: "Garamond, serif",
        border: "2px solid #d4af37",
        borderRadius: "8px",
        backgroundColor: "#8b4513",
        color: "#ffd700",
        cursor: "pointer",
        transition: "all 0.3s ease",
        textShadow: "2px 2px 4px rgba(0,0,0,0.8)",
    },
    box: {
        margin: 10,
        padding: 10,
        border: "2px solid #d4af37",
        borderRadius: 12,
        background: "linear-gradient(145deg, #2a1f15 0%, #1a1611 100%)",
    },
    newStatsBox: {
        background: "linear-gradient(145deg, #452a17ff, #24180cff)",
        boxShadow: "0 0 15px rgba(255, 215, 0, 0.5)",
    }
}