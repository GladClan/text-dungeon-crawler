import React from "react";

const HealthBar: React.FC<{
    max: number,
    current: number,
    fullColor?: string,
    midColor?: string,
    lowColor?: string,
    dangerColor?: string,
    backgroundColor?: string,
    width?: number,
    height?: number
}> = ({ max, current, fullColor = "#4ade80", midColor = "#facc15", lowColor = "#fb923c", dangerColor = "#ef4444", backgroundColor = "#2d1810", width = 100, height = 13 }) => {
    const healthPercentage = (current / max) * 100;
    
    const getColor = () => {
        if (healthPercentage > 75) return fullColor;
        if (healthPercentage > 75) return midColor;
        if (healthPercentage > 75) return lowColor;
        else return dangerColor;
    }

    return (
        <div style={{...styles.container, backgroundColor: backgroundColor, height: `${height}px`, width: `${width}px`}}>
            <div style={{
                backgroundColor: getColor(),
                height: '100%',
                width: `${healthPercentage}%`,
                transition: "width 0.5s ease-in-out, background-color 0.3s ease",
                }}/>
        </div>
    )
}

export default HealthBar;

const styles: {[key: string]: React.CSSProperties} = {
    container: {
        width: "100%",
        borderRadius: "8px",
        overflow: "hidden",
        border: "2px solid #8b6914",
    }
}