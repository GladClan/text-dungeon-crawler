import React from "react"

const WigglyButton: React.FC<{func: Function, children: any}> = ({func, children}) => {
    return (
        <div
            style={styles.button}
            onMouseEnter={(e) => {
                e.currentTarget.style.scale = "1.02"
                e.currentTarget.style.backgroundColor = "#a0522d";
                e.currentTarget.style.boxShadow = "0 6px 12px rgba(255,215,0,0.6)";
            }}
            onMouseLeave={(e) => {
                e.currentTarget.style.scale = "0.98"
                e.currentTarget.style.backgroundColor = "#8b4513";
                e.currentTarget.style.boxShadow = "0 4px 8px rgba(255,215,0,0.4)";
            }}
            onClick={() => func()}
            >
            {children}
        </div>
    )
}

export default WigglyButton;

const styles: {[key: string]: React.CSSProperties} = {
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
    }
}