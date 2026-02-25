import { Entity } from "@/lib/obj/entity";
import React from "react";

const EnemyCard: React.FC<{
    enemy: Entity,
    id?: string,
    showAllStats?: boolean
}> = ({ enemy, id = enemy.getMetadata().getEntityId(), showAllStats = enemy.getStats().isVisible() }) => {
    const metadata = enemy.getMetadata();
    const stats = enemy.getStats();
    const inventory = enemy.getInventory();
    const skills = enemy.getSkills();

    React.useEffect(() => {
        showAllStats = stats.isVisible();
    }, [enemy, showAllStats, stats.isVisible()]);

    // Calculate health percentage for the bar
    const healthPercentage = (stats.getHealth() / stats.getMaxHealth()) * 100;
    
    // Determine health bar color based on percentage
    const getHealthBarColor = (percentage: number) => {
        if (percentage > 75) return "#4ade80"; // Green
        if (percentage > 50) return "#facc15"; // Yellow
        if (percentage > 25) return "#fb923c"; // Orange
        return "#ef4444"; // Red
    };

    return (
        <div style={styles.container}>
            <div style={styles.header}>
                <h3 style={styles.monsterName}>{metadata.getName()}</h3>
            </div>
            
            {/* Health Section */}
            <div style={styles.healthSection}>
                {showAllStats && 
                    <div style={styles.healthText}>
                        <span style={styles.statName}>Health:</span>
                        <span style={styles.statValue}>{Math.round(stats.getHealth())}/{stats.getMaxHealth()}</span>
                    </div>
                }
                
                {/* Health Bar Container */}
                <div style={styles.healthBarContainer}>
                    <div 
                        style={{
                            ...styles.healthBar,
                            width: `${healthPercentage}%`,
                            backgroundColor: getHealthBarColor(healthPercentage)
                        }}
                    />
                    <div style={styles.healthBarBackground} />
                </div>
            </div>

            {/* Additional Stats */}
            { showAllStats && 
                <div style={styles.stats}>
                    <div style={styles.statItem}>
                        <span style={styles.statName}>Level:</span>
                        <span style={styles.statValue}>{stats.getLevel()}</span>
                    </div>
                    <div style={styles.statItem}>
                        <span style={styles.statName}>Mana:</span>
                        <span style={styles.statValue}>{stats.getMana()}/{stats.getMaxMana()}</span>
                    </div>
                    <div style={styles.statItem}>
                        <span style={styles.statName}>Strength:</span>
                        <span style={styles.statValue}>{stats.getStrength()}</span>
                    </div>
                    <div style={styles.statItem}>
                        <span style={styles.statName}>Defense:</span>
                        <span style={styles.statValue}>{stats.getDefense()}</span>
                    </div>
                </div>
            }
        </div>
    );
};

export default EnemyCard;

const styles: { [key: string]: React.CSSProperties } = {
    container: {
        width: "300px",
        padding: "20px",
        border: "3px solid #8b6914",
        borderRadius: "12px",
        background: "linear-gradient(145deg, #1a1611 0%, #0d0b08 100%)",
        boxShadow: "inset 0 0 20px rgba(139,105,20,0.3), 0 0 30px rgba(139,105,20,0.2)",
        fontFamily: "Garamond, serif",
        color: "#e6d3a3",
    },
    header: {
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        marginBottom: "15px",
    },
    monsterName: {
        fontSize: "20px",
        margin: 0,
        color: "#ffd700",
        textShadow: "2px 2px 4px rgba(0,0,0,0.8)",
    },
    healthSection: {
        marginBottom: "15px",
    },
    healthText: {
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        marginBottom: "8px",
    },
    healthBarContainer: {
        position: "relative",
        width: "100%",
        height: "20px",
        borderRadius: "10px",
        overflow: "hidden",
        border: "2px solid #8b6914",
        boxShadow: "inset 0 2px 4px rgba(0,0,0,0.3)",
    },
    healthBarBackground: {
        position: "absolute",
        top: 0,
        left: 0,
        width: "100%",
        height: "100%",
        backgroundColor: "#2d1810",
        zIndex: 0,
    },
    healthBar: {
        position: "absolute",
        top: 0,
        left: 0,
        height: "100%",
        borderRadius: "8px",
        transition: "width 0.5s ease-in-out, background-color 0.3s ease",
        zIndex: 1,
        boxShadow: "inset 0 2px 4px rgba(255,255,255,0.2), 0 0 8px currentColor",
    },
    stats: {
        display: "flex",
        flexDirection: "column",
        gap: "8px",
    },
    statItem: {
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        padding: "4px 0",
        borderBottom: "1px solid rgba(139,105,20,0.3)",
    },
    statName: {
        fontSize: "14px",
        color: "#d4af37",
    },
    statValue: {
        fontSize: "14px",
        fontWeight: "bold",
        color: "#e6d3a3",
    }
}