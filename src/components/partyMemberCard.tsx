import React, { useState } from 'react';
import { Entity } from '@/lib/obj/entity';
import EquipModal from './equipModal';

const PartyMemberCard: React.FC<{
    id?: string;
    index: number;
    member: Entity;
    isActive?: boolean;
    onRemove?: (index: number) => void;
}> = ({ member, id = member.getMetadata().getEntityId(), index, onRemove, isActive = false }) => {
    const stats = member.getStats();
    const metadata = member.getMetadata();
    const inventory = member.getInventory();
    const skills = member.getSkills();
    const [showEquipment, setShowEquipment] = useState(false)

    const getHealthBarColor = () => {
        const healthPercentage = (stats.getHealth() / stats.getMaxHealth()) * 100;
        if (healthPercentage > 75) return "#4ade80";
        if (healthPercentage > 50) return "#facc15";
        if (healthPercentage > 25) return "#fb923c";
        return "#ef4444";
    }

    // State for toggling sections
    const [showInventory, setShowInventory] = useState(false);
    const [showSkills, setShowSkills] = useState(false);

    return (
        <div style={{
            ...styles.memberCard,
            ...(isActive ? styles.activeMemberCard : {}),
            ...(stats.isAlive() ? {} : { background: "linear-gradient(145deg, #880b0bff 0%, #1a1611 100%)" })
        }}>
            <div style={styles.memberHeader}>
                <h3 style={styles.memberName}>{metadata.getName()}</h3>
                {onRemove && (
                    <button 
                        style={styles.removeButton}
                        onClick={() => onRemove(index)}
                    >
                        ×
                    </button>
                )}
                {isActive && <div style={styles.activeIndicator}>⭐ ACTIVE</div>}
            </div>
            
            <div style={styles.cardBody}>
                {/* Left Section - Skills & Inventory */ (onRemove || isActive) && (
                    <div style={styles.leftSection}>
                        {/* Skills Section */}
                        <div style={styles.section}>
                            <div 
                                style={styles.sectionHeader}
                                onClick={() => setShowSkills(!showSkills)}
                            >
                                <span style={styles.sectionTitle}>Skills</span>
                                <span style={styles.toggleIcon}>{showSkills ? '▼' : '▶'}</span>
                            </div>
                            
                            {showSkills && (
                                <div style={styles.sectionContent}>
                                    {skills.allSkills().length > 0 ? (
                                        skills.allSkills().map((skill, index) => (
                                            <div key={index} style={styles.skillItem}>
                                                <span style={styles.skillName}>{skill.getName()}</span>
                                                <span style={styles.skillValue}>
                                                    Proficiency: {stats.getProficiency(skill.getProficiency())}
                                                </span>
                                            </div>
                                        ))
                                    ) : (
                                        <div style={styles.emptyState}>No skills learned</div>
                                    )}
                                </div>
                            )}
                        </div>

                        {/* Inventory Section */}
                        <div style={styles.section}>
                            <div 
                                style={styles.sectionHeader}
                                onClick={() => setShowInventory(!showInventory)}
                            >
                                <span style={styles.sectionTitle}>Inventory</span>
                                <span style={styles.toggleIcon}>{showInventory ? '▼' : '▶'}</span>
                            </div>
                            
                            {showInventory && (
                                <div style={styles.sectionContent}>
                                    {/* Gold */}
                                    <div style={styles.inventoryItem}>
                                        <span style={styles.itemName}>💰 Gold:</span>
                                        <span style={styles.itemValue}>{inventory.getGold()}</span>
                                    </div>

                                    {/* Items */}
                                    {inventory.getItems().length > 0 ? (
                                        inventory.getItems().map((item, itemIndex) => (
                                            <div key={itemIndex} style={styles.inventoryItem}>
                                                <span style={styles.itemName}>{item.getName()}</span>
                                                {/* <span style={styles.itemValue}>x{item.getQuantity ? item.getQuantity() : 1}</span> */}
                                            </div>
                                        ))
                                    ) : (
                                        <div style={styles.emptyState}>No items</div>
                                    )}
                                </div>
                            )}
                        </div>
                        <div style={styles.section}>       
                            <div
                                style={styles.sectionHeader}
                                onClick={() => setShowEquipment(true)}
                            >
                                <span style={styles.sectionTitle}>
                                    Equipment
                                </span>
                            </div>
                        </div>
                    </div>
                )}

                {/* Right Section - Stats */}
                <div style={styles.rightSection}>
                    <div style={styles.memberStats}>
                        <div style={styles.statGroup}>
                            <div style={styles.statItem}>
                                <span style={styles.statName}>Health:</span>
                                <span style={styles.statValue}>{Math.round(stats.getHealth())}/{stats.getMaxHealth()}</span>
                            </div>
                            <div style={styles.statBarContainer}>
                                <div style={{
                                    ...styles.statBar,
                                    width: `${(stats.getHealth() / stats.getMaxHealth()) * 100}%`,
                                    backgroundColor: getHealthBarColor()
                                }} />
                                <div style={styles.statBarBackground} />
                            </div>
                            <div style={styles.statItem}>
                                <span style={styles.statName}>Mana:</span>
                                <span style={styles.statValue}>{stats.getMana()}/{stats.getMaxMana()}</span>
                            </div>
                            <div style={{...styles.statBarContainer, /*width: `${(stats.getMaxMana() / stats.getMaxHealth()) * 100}%`*/}}>
                                <div style={{
                                    ...styles.statBar,
                                    width: `${(stats.getMana() / stats.getMaxMana()) * 100}%`,
                                    backgroundColor: "#3b82f6" // Blue for mana
                                }} />
                                <div style={styles.statBarBackground} />
                            </div>
                            <div style={styles.statItem}>
                                <span style={styles.statName}>Level:</span>
                                <span style={styles.statValue}>{stats.getLevel()}</span>
                            </div>
                            {/* <div style={styles.statItem}>
                                <span style={styles.statName}>Experience:</span>
                                <span style={styles.statValue}>{stats.getExperience()} / {stats.getExperienceForNextLevel()}</span>
                            </div> */}
                        </div>
                        
                        <div style={styles.statGroup}>
                            <div style={styles.statItem}>
                                <span style={styles.statName}>Strength:</span>
                                <span style={styles.statValue}>{stats.getStrength()}</span>
                            </div>
                            <div style={styles.statItem}>
                                <span style={styles.statName}>Defense:</span>
                                <span style={styles.statValue}>{stats.getDefense()}</span>
                            </div>
                            <div style={styles.statItem}>
                                <span style={styles.statName}>Magic:</span>
                                <span style={styles.statValue}>{stats.getMagic()}</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            {showEquipment && (
                <EquipModal
                    member={member}
                    isOpen={showEquipment}
                    onClose={() => setShowEquipment(false)}
                    onEquipmentChange={(member) => {
                        //
                    }}
                />
            )}
        </div>
    );
};

export default PartyMemberCard;

const styles: { [key: string]: React.CSSProperties } = {
    memberCard: {
        width: "275px",
        padding: "20px",
        borderWidth: '3px',
        borderStyle: 'solid',
        borderColor: "#8b6914",
        borderRadius: "12px",
        background: "linear-gradient(145deg, #1a1611 0%, #0d0b08 100%)",
        boxShadow: "inset 0 0 20px rgba(139,105,20,0.3), 0 0 30px rgba(139,105,20,0.2)",
        fontFamily: "Garamond, serif",
        color: "#e6d3a3",
    },
    activeMemberCard: {
        borderColor: "#ffd700",
        background: "linear-gradient(145deg, #2a1f15 0%, #1a1611 100%)",
    },
    memberHeader: {
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        marginBottom: "10px",
    },
    memberName: {
        fontSize: "18px",
        margin: 0,
        color: "#ffd700",
        textShadow: "2px 2px 4px rgba(0,0,0,0.8)",
    },
    activeIndicator: {
        fontSize: "10px",
        color: "#ffd700",
        fontWeight: "bold",
    },
    removeButton: {
        background: "#8b0000",
        color: "#fff",
        border: "none",
        borderRadius: "50%",
        width: "25px",
        height: "25px",
        cursor: "pointer",
        fontSize: "16px",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        transition: "all 0.3s ease",
    },
    cardBody: {
        display: "flex",
        gap: "15px",
    },
    leftSection: {
        flex: "1",
        display: "flex",
        flexDirection: "column",
        gap: "10px",
    },
    rightSection: {
        flex: "1",
    },
    section: {
        border: "1px solid rgba(139,105,20,0.4)",
        borderRadius: "8px",
        background: "rgba(26, 22, 17, 0.3)",
    },
    sectionHeader: {
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        padding: "8px 12px",
        cursor: "pointer",
        borderBottom: "1px solid rgba(139,105,20,0.2)",
        transition: "background-color 0.3s ease",
    },
    sectionTitle: {
        fontSize: "14px",
        fontWeight: "bold",
        color: "#d4af37",
    },
    toggleIcon: {
        fontSize: "12px",
        color: "#8b6914",
        transition: "transform 0.3s ease",
    },
    sectionContent: {
        padding: "8px 12px",
        maxHeight: "120px",
        overflowY: "auto",
        scrollbarWidth: "thin",
        scrollbarColor: "#8b6914 transparent",
    },
    skillItem: {
        display: "flex",
        flexDirection: 'column',
        justifyContent: "space-between",
        alignItems: "center",
        marginBottom: "4px",
        fontSize: "12px",
    },
    skillName: {
        color: "#d4af37",
        fontWeight: "bold",
    },
    skillValue: {
        color: "#e6d3a3",
        fontSize: "11px",
    },
    inventoryItem: {
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        marginBottom: "4px",
        fontSize: "12px",
    },
    itemName: {
        color: "#d4af37",
        fontWeight: "bold",
    },
    itemValue: {
        color: "#e6d3a3",
    },
    emptyState: {
        fontSize: "11px",
        color: "#8b8b8b",
        fontStyle: "italic",
        textAlign: "center",
        padding: "8px 0",
    },
    memberStats: {
        display: "flex",
        flexDirection: "column",
        gap: "10px",
    },
    statGroup: {
        display: "flex",
        flexDirection: "column",
        gap: "5px",
    },
    statItem: {
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        padding: "2px 0",
    },
    statName: {
        fontSize: "14px",
        color: "#d4af37",
    },
    statValue: {
        fontSize: "14px",
        fontWeight: "bold",
        color: "#e6d3a3",
    },
    statBarContainer: {
        position: "relative",
        width: "100%",
        height: "16px",
        borderRadius: "8px",
        overflow: "hidden",
        border: "2px solid #8b6914",
        boxShadow: "inset 0 2px 4px rgba(0,0,0,0.3)",
    },
    statBarBackground: {
        position: "absolute",
        top: 0,
        left: 0,
        width: "100%",
        height: "100%",
        backgroundColor: "#2d1810",
        zIndex: 0,
    },
    statBar: {
        position: "absolute",
        top: 0,
        left: 0,
        height: "100%",
        borderRadius: "6px",
        transition: "width 0.5s ease-in-out, background-color 0.3s ease",
        zIndex: 1,
        boxShadow: "inset 0 2px 4px rgba(255,255,255,0.2)",
    },
}