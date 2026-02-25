import { Entity } from "@/lib/obj/entity";
import React from "react";

const ActionButtons: React.FC<{
    member: Entity,
    // action: action, // action to be used
    isActive?: boolean, // to determine if the buttons should be disabled
}> = ({ member, /* action, */ isActive = false}) => {
    return (
        <div>
            {/* recent actions */}
            {/* attack (unarmed strike or use weapons) */}
            {/* skills */}
            {/* items */}
            {/* defend or flee*/}
        </div>
    )
}

export default ActionButtons;

const styles: { [key: string]: React.CSSProperties } = {
    container: {
        //
    }
}