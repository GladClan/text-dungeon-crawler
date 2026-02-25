"use client";
import React, { useState, useEffect } from "react";
import { StoryBox } from "@/components/storybox";
import { gameContext } from "@/context/gameContext";
import EnemyCard from "@/components/enemyCard";
import { Entity } from "@/lib/obj/entity";
import { EntityAI } from "@/lib/entityAI";
import PartyMemberCard from "@/components/partyMemberCard";
import ActionButtons from "./actionButtons";
import { ContinuousEffects } from "@/lib/obj/ContinuousEffects";
import { gamestate } from "@/lib/gamestateLib";
import { useRouter } from "next/navigation";

const BattlePage: React.FC = () => {
  const router = useRouter();
  const [events, setEvents] = useState<string[]>([]);
  const [currentTurnIndex, setCurrentTurnIndex] = useState(0);
  const [turnOrder, setTurnOrder] = useState<Entity[]>([]);
  const [processingTurn, setProcessingTurn] = useState(false);
  const [targetedEntity, setTargetedEntity] = useState<Entity | null>(null);
  const [turnoverEffects] = useState(() => new ContinuousEffects());

  const contextValue = React.useContext(gameContext);
  if (!contextValue) {
    throw new Error("gameContext is not available");
  }
  const { party, enemies, setGameState, eventTime } = contextValue;

  // Initialize turn order based on initiative
  useEffect(() => {
    // Ensure all entities have AI initialized
    enemies.forEach(enemy => {
      if (!enemy.getAI()) {
        enemy.setAI(new EntityAI(enemy));
      }
    });

    // Sort by initiative (highest first)
    const sortedOrder = [...party, ...enemies].sort((a, b) => b.getInitiative() - a.getInitiative());
    setTurnOrder(sortedOrder);
    turnoverEffects.setEntityCount(turnOrder.length);
  }, [party, enemies]);

  // Skip dead and handle enemy turns
  useEffect(() => {
    const currentEntity = getCurrentEntity();
    // On defeat: all party defeated
    if (party.every(member => !member.getStats().isAlive())) {
      setGameState(gamestate.PartyDefeat);
      setEvents(prev => [...prev, "💀 Your party has been defeated..."]);
      // Go to defeat screen
      setTimeout(() => {
        router.push('/battleResult')
      }, 4000 * eventTime)
    }
    // On victory: all enemies defeated
    if (enemies.every(enemy => !enemy.getStats().isAlive())) {
      setGameState(gamestate.BattleVictory);
      setEvents(prev => [...prev, "Victory! All the enemies have been defeated."])
      document.body.style.cursor = 'wait';
      // Go to victory screen
      setTimeout(() => {
        document.body.style.cursor = 'default';
        router.push('/battleResult')
      }, 4000 * eventTime)
    }
    // Skip dead entities
    if (currentEntity && !currentEntity.getStats().isAlive()) {
      setEvents(prev => [...prev, `${currentEntity.getMetadata().getName()} is unconscious (or dead)`]);
      nextTurn();
      return;
    }
    // Enemy turn
    if (currentEntity && !isPartyMember(currentEntity) && !processingTurn) {
      processEnemyTurn(currentEntity)
        .then(() => {
          nextTurn();
        })
        .catch(error => {
          console.error("Error processing enemy turn:", error);
          setProcessingTurn(false);
        });
    }
  }, [currentTurnIndex, turnOrder, processingTurn]);

  // Process enemy turn with the enemy's AI
  const processEnemyTurn = async(enemy: Entity) => {
    setProcessingTurn(true);

    await new Promise(resolve => setTimeout(resolve, eventTime * 1000)); // add delay to make enemy 'think'
    const ai = enemy.getAI();
    if (ai) {
      const aliveParty = party.filter(member => member.getStats().isAlive());

      const actionDescription = ai.determineAction(aliveParty, enemies);
      setEvents(prev => [...prev, actionDescription]);
    }
  }

  const nextTurn = async() => {
    if (!targetedEntity?.getStats().isAlive()) setTargetedEntity(null);
    const turnoverEvents = turnoverEffects.processEffects();
    setEvents(prev => [...prev, ...turnoverEvents]);
    await new Promise(resolve => setTimeout(resolve, eventTime * 1000)); // add delay for turn processing
    setProcessingTurn(false);
    setCurrentTurnIndex(prev => (prev + 1) % turnOrder.length);
  };

  const getCurrentEntity = () => {
    return turnOrder[currentTurnIndex];
  };

  const isEntityTurn = (entity: Entity) => {
    return getCurrentEntity()?.getMetadata().getEntityId() === entity.getMetadata().getEntityId();
  };

  const isPartyMember = (entity: Entity) => {
    return party.some(member => member.getMetadata().getEntityId() === entity.getMetadata().getEntityId());
  };

  const isPlayerTurn = () => {
    return getCurrentEntity() && isPartyMember(getCurrentEntity()) && getCurrentEntity().getStats().isAlive();
  };

  return (
    <div style={styles.container}>
      {/* Turn Order Queue Header */}
      <div style={styles.turnOrderContainer}>
        <div style={styles.turnQueue}>
          {turnOrder.map((member, index) => {
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
          })}
        </div>
      </div>

      <div style={styles.battleArea}>
        {/* Party Section */}
        <div style={styles.section}>
          <h2 style={styles.sectionTitle}>🛡️ Party</h2>
          
          {/* Action Buttons - only show when it's a player's turn */}
          {isPlayerTurn() && (
            <div style={{display: "flex", flexDirection:"column", gap: 5}}>
              {targetedEntity ? (
                <div style={{textAlign: "center"}}>Targeting {targetedEntity.getMetadata().getName()}</div>
              ) : (
                <span style={{ color: "#ff0000", textAlign: "center" }}>⚠️ Select a target to attack!</span>
              )}
              <ActionButtons
                processingTurn={processingTurn}
                setProcessingTurn={setProcessingTurn}
                addEvent={(event: string) => setEvents(prev => [...prev, event])}
                nextTurn={nextTurn}
                targetedEntity={targetedEntity}
                currentEntity={getCurrentEntity()}
                aftereffects={turnoverEffects}
              />
            </div>
          )}

          <div style={styles.combatantContainer}>
            {party.map((member, index) => {
              const isActive = isEntityTurn(member);
              return (
                <div 
                  key={member.getMetadata().getEntityId()} 
                  style={{
                    ...styles.combatantCard,
                    ...(isActive ? styles.activeCard : styles.inactiveCard),
                    order: index
                  }}
                  onClick={() => setTargetedEntity(member)}
                >
                  <PartyMemberCard
                    member={member}
                    index={index}
                    isActive={isActive}
                  />
                </div>
              );
            })}
          </div>
        </div>

        {/* Battle Scene */}
        <div style={styles.battleScene}>
          <h1 style={styles.battleTitle}>⚔️ Battle Scene ⚔️</h1>
          <StoryBox textArr={events} autonext={true} showCursor={false} speed={10 * eventTime} />
          
          {/* Current Turn Indicator */}
          <div style={styles.currentTurnDisplay}>
            {getCurrentEntity() && (
              <>
                <h3 style={styles.currentTurnText}>
                  {isPlayerTurn() ? "🛡️ Your Turn:" : "⚔️ Enemy Turn:"}
                </h3>
                <div style={styles.currentEntityName}>
                  {getCurrentEntity().getMetadata().getName()}
                </div>
              </>
            )}
          </div>
        </div>

        {/* Enemies Section */}
        <div style={styles.section}>
          <h2 style={styles.sectionTitle}>⚔️ Enemies</h2>
          <div style={styles.combatantContainer}>
            {enemies.map((enemy, index) => {
              return (
                <button 
                  key={enemy.getMetadata().getEntityId()} 
                  style={{
                    ...styles.combatantCard,
                    ...(isEntityTurn(enemy) ? styles.activeCard : styles.inactiveCard),
                    ...(isPlayerTurn() ? {cursor: "pointer"} : {cursor: "auto"})
                  }}
                  onClick={() => {
                    setTargetedEntity(enemy)
                    if (!enemy.getStats().isAlive())
                      setEvents(prev => ([...prev, `${enemy.getMetadata().getName()} is already dead!`]))
                  }}
                  disabled={processingTurn || !isPlayerTurn()}
                >
                  <EnemyCard enemy={enemy} />
                </button>
              );
            })}
          </div>
        </div>
      </div>
    </div>
  );
};

export default BattlePage;

const styles: { [key: string]: React.CSSProperties } = {
  container: {
    display: "flex",
    flexDirection: "column",
    minHeight: "100vh",
    padding: "20px",
    fontFamily: "Garamond, serif",
    background: "linear-gradient(135deg, #2c1810 0%, #1a0f0a 50%, #0f0704 100%)",
    color: "#e6d3a3",
  },
  turnOrderContainer: {
    marginBottom: "20px",
    padding: "15px",
    border: "3px solid #8b6914",
    borderRadius: "12px",
    background: "linear-gradient(145deg, #1a1611 0%, #0d0b08 100%)",
    boxShadow: "inset 0 0 20px rgba(139,105,20,0.3)",
  },
  turnQueue: {
    display: "flex",
    gap: "10px",
    overflowX: "auto",
    padding: "10px 0",
  },
  turnQueueItem: {
    position: "relative",
    minWidth: "120px",
    padding: "8px 12px",
    border: "2px solid",
    borderRadius: "8px",
    background: "rgba(26, 22, 17, 0.7)",
    textAlign: "center",
    transition: "all 0.3s ease",
  },
  currentTurnItem: {
    background: "linear-gradient(145deg, #8b4513, #654321)",
    boxShadow: "0 0 15px rgba(255, 215, 0, 0.5)",
    transform: "scale(1.1)",
  },
  queueEntityName: {
    fontSize: "12px",
    fontWeight: "bold",
    marginBottom: "4px",
    // border: "5px solid #fff"
  },
  queueEntityInfo: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    position: "absolute",
    bottom: "10px",
    left: "10px",
    fontSize: "10px",
    // border: "5px solid #fff"
  },
  currentTurnIndicator: {
    fontSize: "8px",
    color: "#ffd700",
    fontWeight: "bold",
    marginTop: "4px",
  },
  battleArea: {
    display: "flex",
    gap: "20px",
    flex: 1,
  },
  section: {
    flex: "1",
    display: "flex",
    flexDirection: "column",
  },
  battleScene: {
    flex: "1.5",
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    maxWidth: "600px",
  },
  sectionTitle: {
    fontSize: "24px",
    marginBottom: "15px",
    textAlign: "center",
    color: "#ffd700",
  },
  battleTitle: {
    fontSize: "28px",
    marginBottom: "20px",
    textAlign: "center",
    color: "#ffd700",
  },
  combatantContainer: {
    display: "flex",
    flexDirection: "column",
    gap: "15px",
    // paddingTop: "25px",
    alignItems: "center",
    maxHeight: "70vh",
    overflowY: "auto",
    scrollbarWidth: "thin",
    scrollbarColor: "#8b6914 #1a1611",
  },
  combatantCard: {
    transition: "all 0.5s ease",
    width: "100%",
    maxWidth: "280px",
    transformOrigin: "top",
    borderRadius: "16px",
  },
  activeCard: {
    transform: "scale(1.15)",
    zIndex: 5,
    boxShadow: "0 0 25px rgba(255, 215, 0, 0.6)",
    marginBottom: "35px",
  },
  inactiveCard: {
    transform: "scale(0.9)",
    opacity: 0.7,
    marginBottom: 0,
  },
  currentTurnDisplay: {
    textAlign: "center",
    marginTop: "20px",
    padding: "15px",
    border: "2px solid #8b6914",
    borderRadius: "8px",
    background: "rgba(26, 22, 17, 0.8)",
  },
  currentTurnText: {
    margin: "0 0 5px 0",
    fontSize: "16px",
    color: "#d4af37",
  },
  currentEntityName: {
    fontSize: "20px",
    fontWeight: "bold",
    color: "#ffd700",
  },
};