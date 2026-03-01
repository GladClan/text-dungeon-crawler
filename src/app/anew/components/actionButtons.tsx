import { Entity } from "@/lib/obj/entity/entity";
import React from "react";

const ActionButtons: React.FC<{
    member: Entity,
    // action: action, // action to be used
    isActive?: boolean, // to determine if the buttons should be disabled
}> = ({ member, /* action, */ isActive = false}) => {
    return (
        <div style={styles.container}>
            {/* recent actions */}
            <div style={styles.section}>
                Recent Actions
            </div>
            {/* attack (unarmed strike or use weapons) */}
            <div style={styles.section}>
                Attacks
            </div>
            {/* skills */}
            <div style={styles.section}>
                Skills
            </div>
            {/* items */}
            <div style={styles.section}>
                Items
            </div>
            {/* defend or flee*/}
            <div style={styles.section}>
                Defend or flee
            </div>
        </div>
    )
}

export default ActionButtons;

const styles: { [key: string]: React.CSSProperties } = {
    container: {
        width: 225
    },
    section: {
        padding: 5,
        paddingLeft: 15,
        border: "1px solid rgba(212, 175, 55, 0.3)",
    }
}