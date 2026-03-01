import React, { useState } from 'react';
import { Entity } from '@/lib/obj/entity/entity';

const CharacterCreationModal: React.FC<{
    onCharacterCreated: (character: Entity) => void;
    onCancel: () => void;
}> = ({ onCharacterCreated, onCancel }) => {
    const [name, setName] = useState('');
    const [availablePoints, setAvailablePoints] = useState(27); // D&D style point buy
    const [stats, setStats] = useState({
        health: 10,
        mana: 10,
        magic: 8,
        strength: 8,
        defense: 8
    });

    const statCosts = {
        8: 0, 9: 1, 10: 2, 11: 3, 12: 4, 13: 5, 14: 7, 15: 9, 16: 12, 17: 15, 18: 19
    };

    const calculatePointCost = (value: number) => {
        return statCosts[value as keyof typeof statCosts] || 0;
    };

    const getTotalPointsUsed = () => {
        return Object.values(stats).reduce((total, stat) => total + calculatePointCost(stat), 0);
    };

    const canIncreaseStat = (statName: keyof typeof stats) => {
        const currentValue = stats[statName];
        if (currentValue >= 18) return false;
        
        const currentCost = calculatePointCost(currentValue);
        const nextCost = calculatePointCost(currentValue + 1);
        const additionalCost = nextCost - currentCost;
        
        return getTotalPointsUsed() + additionalCost <= 27;
    };

    const canDecreaseStat = (statName: keyof typeof stats) => {
        return stats[statName] > 8;
    };

    const handleStatChange = (statName: keyof typeof stats, increment: boolean) => {
        if (increment && canIncreaseStat(statName)) {
            setStats(prev => ({ ...prev, [statName]: prev[statName] + 1 }));
        } else if (!increment && canDecreaseStat(statName)) {
            setStats(prev => ({ ...prev, [statName]: prev[statName] - 1 }));
        }
    };

    const handleCreateCharacter = () => {
        if (name.trim() === '') {
            alert('Please enter a character name');
            return;
        }

        const newCharacter = new Entity(
            name,
            'player'
        );
        newCharacter.fixStats(
            stats.health * 10,
            stats.mana * 5,
            stats.magic,
            stats.strength,
            stats.defense
        );

        onCharacterCreated(newCharacter);
    };

    return (
        <div style={styles.modal}>
            <div style={styles.modalContent}>
                <h2 style={styles.modalTitle}>Create New Character</h2>
                
                <div style={styles.nameInput}>
                    <label style={styles.label}>Character Name:</label>
                    <input
                        type="text"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        style={styles.input}
                        placeholder="Enter character name"
                    />
                </div>

                <div style={styles.pointInfo}>
                    <p>Points Used: {getTotalPointsUsed()} / 27</p>
                    <p>Points Remaining: {27 - getTotalPointsUsed()}</p>
                </div>

                <div style={styles.statsContainer}>
                    {Object.entries(stats).map(([statName, value]) => (
                        <div key={statName} style={styles.statRow}>
                            <label style={styles.statLabel}>
                                {statName.charAt(0).toUpperCase() + statName.slice(1)}:
                            </label>
                            <div style={styles.statControls}>
                                <button
                                    style={{
                                        ...styles.statButton,
                                        opacity: canDecreaseStat(statName as keyof typeof stats) ? 1 : 0.5
                                    }}
                                    onClick={() => handleStatChange(statName as keyof typeof stats, false)}
                                    disabled={!canDecreaseStat(statName as keyof typeof stats)}
                                >
                                    -
                                </button>
                                <span style={styles.statValue}>{value}</span>
                                <button
                                    style={{
                                        ...styles.statButton,
                                        opacity: canIncreaseStat(statName as keyof typeof stats) ? 1 : 0.5
                                    }}
                                    onClick={() => handleStatChange(statName as keyof typeof stats, true)}
                                    disabled={!canIncreaseStat(statName as keyof typeof stats)}
                                >
                                    +
                                </button>
                            </div>
                            <span style={styles.statCost}>
                                Cost: {calculatePointCost(value)}
                            </span>
                        </div>
                    ))}
                </div>

                <div style={styles.modalButtons}>
                    <button style={styles.cancelButton} onClick={onCancel}>
                        Cancel
                    </button>
                    <button style={styles.confirmButton} onClick={handleCreateCharacter}>
                        Create Character
                    </button>
                </div>
            </div>
        </div>
    );
};

export default CharacterCreationModal;

const styles: {[key: string]: React.CSSProperties} = {
    modal: {
        position: "fixed",
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        backgroundColor: "rgba(0, 0, 0, 0.8)",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        zIndex: 1000,
    },
    modalContent: {
        background: "linear-gradient(145deg, #2c1810 0%, #1a0f0a 100%)",
        padding: "30px",
        borderRadius: "12px",
        border: "3px solid #8b6914",
        boxShadow: "0 0 30px rgba(139,105,20,0.4)",
        maxWidth: "500px",
        width: "90%",
        maxHeight: "90vh",
        overflowY: "auto",
    },
    modalTitle: {
        fontSize: "24px",
        marginBottom: "20px",
        textAlign: "center",
        color: "#ffd700",
    },
    nameInput: {
        marginBottom: "20px",
    },
    label: {
        display: "block",
        marginBottom: "5px",
        fontSize: "16px",
        color: "#d4af37",
    },
    input: {
        width: "100%",
        padding: "8px",
        fontSize: "16px",
        border: "2px solid #8b6914",
        borderRadius: "4px",
        background: "#1a1611",
        color: "#e6d3a3",
        fontFamily: "Garamond, serif",
    },
    pointInfo: {
        textAlign: "center",
        marginBottom: "20px",
        fontSize: "16px",
        color: "#d4af37",
    },
    statsContainer: {
        marginBottom: "20px",
    },
    statRow: {
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        marginBottom: "10px",
        padding: "5px",
        background: "rgba(26, 22, 17, 0.5)",
        borderRadius: "4px",
    },
    statLabel: {
        fontSize: "14px",
        color: "#d4af37",
        minWidth: "80px",
    },
    statControls: {
        display: "flex",
        alignItems: "center",
        gap: "10px",
    },
    statButton: {
        width: "30px",
        height: "30px",
        border: "1px solid #8b6914",
        borderRadius: "4px",
        background: "#8b4513",
        color: "#ffd700",
        cursor: "pointer",
        fontSize: "16px",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
    },
    statCost: {
        fontSize: "12px",
        color: "#a0a0a0",
        minWidth: "60px",
        textAlign: "right",
    },
    modalButtons: {
        display: "flex",
        gap: "10px",
        justifyContent: "center",
    },
    cancelButton: {
        padding: "10px 20px",
        fontSize: "16px",
        fontFamily: "Garamond, serif",
        border: "2px solid #8b0000",
        borderRadius: "8px",
        backgroundColor: "#8b0000",
        color: "#fff",
        cursor: "pointer",
        transition: "all 0.3s ease",
    },
    confirmButton: {
        padding: "10px 20px",
        fontSize: "16px",
        fontFamily: "Garamond, serif",
        border: "2px solid #d4af37",
        borderRadius: "8px",
        backgroundColor: "#8b4513",
        color: "#ffd700",
        cursor: "pointer",
        transition: "all 0.3s ease",
    },
}