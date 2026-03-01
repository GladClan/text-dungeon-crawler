"use client"
import React, { useContext} from "react";
import Image from "next/image";
import Link from "next/link";
import { gameContext } from "@/context/gameContext";
import { toVictory } from "@/lib/errorMonster";
import { gamestate } from "@/lib/gamestateLib";

export default function Home() {
    const contextValue = useContext(gameContext);
    if (!contextValue) {
        throw new Error("Game context is not available!");
    }
    const { setEnemies, setGameState } = contextValue;

  return (
    <div style={styles.page}>
      <main style={styles.main}>
        <Image
          src="/explosion.gif"
          alt="Explosively capturing"
          width={180}
          height={180}
          priority
        />
        <ol style={styles.ol}>
          <li style={{marginBottom: "8px"}}>
            Get started by editing <code style={styles.code}>src/app/page.tsx</code>.
          </li>
          <li style={{marginBottom: 0}}>Save and see your changes instantly.</li>
        </ol>
        <ul style={{...styles.ol, display: "flex", flexDirection: "column"}}>
          <Link href={'/story'}>Go see what's up</Link>
          <Link href={'/party'}>Check out the gang</Link>
          <Link href={'/battle'} onClick={() => (
            setEnemies(toVictory),
            setGameState(gamestate.Battle)
          )}>Just fight</Link>
        </ul>

      </main>
      <footer style={styles.footer}>
        <a>This is the footer</a>
      </footer>
    </div>
  );
}

const styles: { [key: string]: React.CSSProperties } = {
  page: {
    display: "grid",
    gridTemplateRows: "20px 1fr 20px",
    alignItems: "center",
    justifyItems: "center",
    minHeight: "100svh",
    padding: "80px",
    gap: "20px",
    fontFamily: "geist sans, sans-serif",
  },
  main: {
    display: "flex",
    flexDirection: "column",
    gap: "32px",
    gridRowStart: 2,
  },
  ol: {
    fontFamily: "var(--font-geist-mono)",
    paddingLeft: 0,
    margin: 0,
    fontSize: "14px",
    lineHeight: "24px",
    letterSpacing: "-0.01em",
    listStylePosition: "inside",
  },
  code: {
    fontFamily: "var(--font-geist-mono)",
    background: "var(--gray-alpha-100)",
    padding: "2px 4px",
    borderRadius: "4px",
    fontWeight: 600,
  },
  footer: {
    display: "flex",
    alignItems: "center",
    gap: "8px",
    gridRowStart: 3,
  }
}