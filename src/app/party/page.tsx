'use client'
import React, { useState } from 'react';

import PartyMemberCard from '@/components/partyMemberCard';
import CharacterCreationModal from './characterModal';
import { gameContext } from '@/context/gameContext';
import { Entity } from '@/lib/obj/entity/entity';
import { useRouter } from 'next/navigation';
import { quickLevel } from '@/lib/errorMonster';

const PartyPage: React.FC = () => {
    const router = useRouter();
    const contextValue = React.useContext(gameContext);
    if (!contextValue) { throw new Error('PartyPage must be used within a GameProvider'); }
    const { party, setParty, setEnemies } = contextValue;

    const [showCharacterCreation, setShowCharacterCreation] = useState(false);

    const handleCreateCharacter = () => {
        setShowCharacterCreation(true);
    };

    const handleCharacterCreated = (newCharacter: Entity) => {
        setParty(prev => [...prev, newCharacter]);
        setShowCharacterCreation(false);
    };

    const handleRemoveCharacter = (index: number) => {
        setParty(prev => prev.filter((_, i) => i !== index));
    };

    return (
        <div style={styles.container}>
            <h1 style={styles.title}>Party Management</h1>
            
            {party.length === 0 && !showCharacterCreation ? (
                <div style={styles.emptyParty}>
                    <p style={styles.emptyMessage}>Your party is empty. Create your first character!</p>
                    <button style={styles.createButton} onClick={handleCreateCharacter}>
                        Create Character
                    </button>
                </div>
            ) : (
                <div style={styles.partyContainer}>
                    {party.map((member, index) => (
                        <PartyMemberCard
                            key={member.getEntityId()} 
                            member={member} 
                            index={index}
                            onRemove={handleRemoveCharacter}
                        />
                    ))}
                    
                    {party.length < 4 && (
                        <div style={styles.addMemberCard}>
                            <button style={styles.addMemberButton} onClick={handleCreateCharacter}>
                                + Add Party Member
                            </button>
                        </div>
                    )}
                </div>
            )}

            <div
                style={{...styles.createButton, marginTop: "30px"}}
                onClick={() => {
                    setEnemies(quickLevel),
                    router.push('/battle')
                }}
            >
                Go to fight
            </div>

            {showCharacterCreation && (
                <CharacterCreationModal 
                    onCharacterCreated={handleCharacterCreated}
                    onCancel={() => setShowCharacterCreation(false)}
                />
            )}
        </div>
    );
};

export default PartyPage;

const styles: { [key: string]: React.CSSProperties } = {
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
        fontSize: "36px",
        marginBottom: "30px",
        textShadow: "2px 2px 4px rgba(0,0,0,0.8)",
    },
    emptyParty: {
        textAlign: "center",
        padding: "40px",
        border: "3px solid #8b6914",
        borderRadius: "12px",
        background: "linear-gradient(145deg, #1a1611 0%, #0d0b08 100%)",
        boxShadow: "inset 0 0 20px rgba(139,105,20,0.3), 0 0 30px rgba(139,105,20,0.2)",
    },
    emptyMessage: {
        fontSize: "18px",
        marginBottom: "20px",
    },
    createButton: {
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
    partyContainer: {
        display: "flex",
        flexWrap: "wrap",
        gap: "20px",
        justifyContent: "center",
        maxWidth: "1200px",
    },
    addMemberCard: {
        width: "300px",
        height: "200px",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        border: "3px dashed #8b6914",
        borderRadius: "12px",
        background: "rgba(26, 22, 17, 0.5)",
    },
    addMemberButton: {
        padding: "12px 24px",
        fontSize: "16px",
        fontFamily: "Garamond, serif",
        border: "2px solid #d4af37",
        borderRadius: "8px",
        backgroundColor: "#8b4513",
        color: "#ffd700",
        cursor: "pointer",
        transition: "all 0.3s ease",
    },
};