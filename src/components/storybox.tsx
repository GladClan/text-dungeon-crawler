"use client";
import React from "react";

type StoryBoxProps = {
  textArr: string[];
  autonext: boolean;
  continueFunction?: Function;
  showCursor?: boolean;
  showHistory?: boolean;
  speed?: number; // Speed in milliseconds for typewriter effect
};

export const StoryBox: React.FC<StoryBoxProps> = ({ textArr, autonext, continueFunction, showCursor = true, showHistory = true, speed = 20 }) => {
  const [displayText, setDisplayText] = React.useState<string[]>([]); // Store already displayed text in the typing animation
  const [currentText, setCurrentText] = React.useState<string>(""); // Current text being typed out
  const [isTyping, setIsTyping] = React.useState<boolean>(false); // State to control typing animation
  const [currentIndex, setCurrentIndex] = React.useState<number>(0);  // Current character index in the current text for the typing animation
  const [textIndex, setTextIndex] = React.useState<number>(0); // Track which text we're on
  const scrollRef = React.useRef<HTMLDivElement>(null);

  // Typewriter effect
  React.useEffect(() => {
    if (isTyping && textIndex < textArr.length && currentIndex < textArr[textIndex].length) {
      const timer = setTimeout(() => {
        setCurrentText(prev => prev + textArr[textIndex][currentIndex]);
        setCurrentIndex(prev => prev + 1);
        
        // Auto-scroll to bottom
        if (scrollRef.current) {
          scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
        }
      }, speed);

      return () => clearTimeout(timer);
    } else if (isTyping && currentIndex >= textArr[textIndex]?.length) {
      // Finished typing current text
      setIsTyping(false);
      setDisplayText(prev => [...prev, currentText]);
      setCurrentText("");
      setCurrentIndex(0);
      setTextIndex(prev => prev + 1); // Move to next text
    }
  }, [isTyping, currentIndex, textArr, currentText, textIndex]);

  // Start typing animation when new events are added
    React.useEffect(() => {
      if (autonext && textArr.length > 0 && !isTyping && textIndex < textArr.length) {
        setIsTyping(true);
        setCurrentIndex(0);
        setCurrentText("");
      }
    }, [textArr, isTyping, textIndex]);

  const handleNext = () => {
    // Skip animation and show full text immediately
    if (isTyping) {
      const fullText = textArr[textIndex] || "";
      setDisplayText(prev => [...prev, fullText]);
      setCurrentText("");
      setCurrentIndex(0);
      setIsTyping(false);
      setTextIndex(prev => prev + 1); // Move to next text
      // Start typing animation
    } else if (textIndex < textArr.length) {
      setIsTyping(true);
      setCurrentIndex(0);
      setCurrentText("");
      // End of text array, call continue function if provided
    } else if (continueFunction) {
      continueFunction();
    }
  };

  return (
    <div style={styles.container}>
      <div ref={scrollRef} style={{...styles.scrollBox, overflowY: showHistory ? "auto" : "hidden"}}>
        {displayText.map((text, index) => (
          <p key={index} style={{marginBottom: 20, lineHeight: 1.5}}>{text}</p>
        ))}
        {isTyping && (
          <p style={{marginBottom: 20, lineHeight: 1.5}}>
            {currentText}
            {showCursor && <span style={styles.cursor}>🔥</span>}
          </p>
        )}
      </div>
      {!autonext && 
        <button
          onClick={handleNext}
          disabled={textIndex >= textArr.length && !isTyping && !continueFunction}
          style={{
            ...styles.btn,
            backgroundColor: isTyping ? "#4a3f2a" : "#8b4513",
            color: isTyping ? "#d4af37" : "#ffd700",boxShadow: isTyping ? "0 2px 4px rgba(212,175,55,0.3)" : "0 4px 8px rgba(255,215,0,0.4)",
            transform: isTyping ? "scale(0.98)" : "scale(1)"
          }}
          onMouseEnter={(e) => {
            if (!isTyping && textIndex < textArr.length) {
              e.currentTarget.style.backgroundColor = "#a0522d";
              e.currentTarget.style.boxShadow = "0 6px 12px rgba(255,215,0,0.6)";
            }
          }}
          onMouseLeave={(e) => {
            if (!isTyping && textIndex < textArr.length) {
              e.currentTarget.style.backgroundColor = "#8b4513";
              e.currentTarget.style.boxShadow = "0 4px 8px rgba(255,215,0,0.4)";
            }
          }}
        >
          {textIndex == 0 && !isTyping ? "Begin" : isTyping ? "Skip" : textIndex < textArr.length ? "Next" : "Continue"}
        </button>
      }
    </div>
  );
}

const styles: { [key: string]: React.CSSProperties } = {
  container: {
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    justifyContent: "center",
    // minHeight: "100vh",
    padding: "20px",
    // fontFamily: "Garamond, serif",
    background: "linear-gradient(135deg, #2c1810 0%, #1a0f0a 50%, #0f0704 100%)",
    color: "#e6d3a3",
  },
  scrollBox: {
    height: "400px",
    width: "600px",
    overflowY: "auto",
    padding: "25px",
    border: "3px solid #8b6914",
    borderRadius: "12px",
    background: "linear-gradient(145deg, #1a1611 0%, #0d0b08 100%)",
    fontFamily: "Garamond, serif",
    fontSize: "18px",
    lineHeight: 1.8,
    scrollBehavior: "smooth",
    boxShadow: "inset 0 0 20px rgba(139,105,20,0.3), 0 0 30px rgba(139,105,20,0.2)",
    // Custom scrollbar styling
    scrollbarWidth: "thin",
    scrollbarColor: "#8b6914 #1a1611",
  },
  cursor: {
    color: "#ffd700",
    fontWeight: "bold",
    textShadow: "0 0 8px #ffd700",
  },
  btn: {
    marginTop: 20, 
    padding: "12px 24px", 
    fontSize: "18px",
    fontFamily: "Garamond, serif",
    border: "2px solid #d4af37",
    borderRadius: "8px",
    cursor: "pointer",
    transition: "all 0.3s ease",
    textShadow: "2px 2px 4px rgba(0,0,0,0.8)",
  }
};

// Add CSS animation for blinking cursor and fantasy effects
if (typeof window !== 'undefined') {
  const styleSheet = document.createElement("style");
  styleSheet.textContent = `   
    /* Fantasy glow effect for scrollbox */
    .scroll-box::-webkit-scrollbar {
      width: 8px;
    }
    
    .scroll-box::-webkit-scrollbar-track {
      background: #1a1611;
      border-radius: 4px;
    }
    
    .scroll-box::-webkit-scrollbar-thumb {
      background: linear-gradient(45deg, #8b6914, #d4af37);
      border-radius: 4px;
      box-shadow: 0 0 5px rgba(212,175,55,0.5);
    }
    
    .scroll-box::-webkit-scrollbar-thumb:hover {
      background: linear-gradient(45deg, #d4af37, #ffd700);
    }
  `;
  document.head.appendChild(styleSheet);
}