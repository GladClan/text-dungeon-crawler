import React from "react";
import HealthBar from "@/components/healthBar";

import { Entity } from "@/lib/obj/entity/entity";

const TurnOrderCard: React.FC<{
    member: Entity,
    isParty: boolean,
    isCurrentEntity: boolean
}> = ({ member, isParty, isCurrentEntity }) => {
  return (
    <div style={{
        ...styles.turnQueueItem,
        ...(isCurrentEntity? styles.currentTurnItem : {}),
        borderColor: member.getStats().isAlive() ? (isParty ? "#4ade80" : "#ef4444") : "#6b7280"
        }}>
        <div style={styles.entityName}>
            {member.getName()}
        </div>
        <div style={styles.entityInfo}>
            <span style={{ color: "#d4af37" }}>Init: {member.getSpeed()}</span>
            <span style={{ fontSize: '12px' }}>{isParty ? "🛡️" : "⚔️"}</span>
            <HealthBar
                max={member.getStats().getMaxHealth()}
                current={member.getStats().getHealth()}
            />
        </div>
        {isCurrentEntity && <div style={styles.currentTurnIndicator}>▶ ACTIVE</div>}
    </div>
  );
};

export default TurnOrderCard;

const styles: { [key: string]: React.CSSProperties } = {
    turnQueueItem: {
        position: 'relative',
        minWidth: "120px",
        maxWidth: "280px",
        height: "100%",
        padding: "4px 6px",
        border: "2px solid",
        borderRadius: "8px",
        background: "rgba(26, 22, 17, 0.7)",
        margin: 5,
        textAlign: "center",
        transition: "all 0.3s ease",
    },
    currentTurnItem: {
        background: "linear-gradient(145deg, #8b4513, #654321)",
        boxShadow: "0 0 15px rgba(255, 215, 0, 0.5)",
        transform: "scale(1.1)",
    },
    entityName: {
        fontSize: "12px",
        fontWeight: "bold",
        marginBottom: "4px",
    },
    entityInfo: {
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        // position: "absolute",
        // bottom: "10px",
        // left: "10px",
        fontSize: "10px",
    },
    currentTurnIndicator: {
        fontSize: "8px",
        color: "#ffd700",
        fontWeight: "bold",
        marginTop: "4px",
    }
}

/**
    const isCurrent = index === currentTurnIndex;
    const isParty = isPartyMember(member);
    return (
        <div 
        key={member.getMetadata().getEntityId()} 
        style={{
            ...styles.turnQueueItem,
            ...(isCurrent ? styles.currentTurnItem : {}),
            borderColor: member.getStats().isAlive() ? (isParty ? "#4ade80" : "#ef4444") : "#6b7280"
        }}
        >
        <div style={styles.queueEntityName}>
            {member.getMetadata().getName()}
        </div>
        <div style={styles.queueEntityInfo}>
            <span style={{ color: "#d4af37" }}>Init: {member.getInitiative()}</span>
            <span style={{ fontSize: '12px' }}>{isParty ? "🛡️" : "⚔️"}</span>
        </div>
        {isCurrent && <div style={styles.currentTurnIndicator}>▶ ACTIVE</div>}
        </div>
    );
 */