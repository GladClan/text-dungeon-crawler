import React, { useState } from 'react';
import { Entity } from "@/lib/obj/entity/entity";
import { Useable } from '@/lib/obj/itemCases/useable';
import { getActionText } from '@/lib/battleFunctions';
import { returnType } from '@/lib/battleFunctions';
import { ContinuousEffects } from '@/lib/obj/ContinuousEffects';

const ActionButtons: React.FC<{
  processingTurn: boolean,
  setProcessingTurn: React.Dispatch<React.SetStateAction<boolean>>,
  addEvent: (event: string) => void,
  nextTurn: () => void;
  targetedEntity: Entity | null,
  currentEntity: Entity,
  aftereffects: ContinuousEffects
}> = ({ processingTurn, setProcessingTurn, addEvent, nextTurn, targetedEntity, currentEntity, aftereffects }) => {
  const [dropdownOpen, setDropdownOpen] = useState('');

  const handleAction = (action: string, resultsArr: returnType) => {
    setProcessingTurn(true);
    addEvent(getActionText(targetedEntity!, currentEntity, action, resultsArr));
    nextTurn();
  }

  return (
    <div style={styles.container}>
        <div
          style={styles.dropdown}
          onClick={() => dropdownOpen === 'attack' ? setDropdownOpen('') : setDropdownOpen('attack')}
        >
          <span>⚔️ Attack!</span>
          {dropdownOpen === 'attack' && (
            <div style={{
              ...styles.dropdownContainer,
              ...(dropdownOpen === 'attack' ? styles.dropdownOpen : styles.dropdownClosed),
              }}>
                {/* <WigglyActionButton
                  key={`attack`}
                  clickFunction={() => handleAction(
                  ` attacks ${targetedEntity?.getName()}`,
                  currentEntity.physicalAttack(targetedEntity!)
                )}
                disabled={processingTurn || !targetedEntity}
                >
                  👊 Physical Attack
                </WigglyActionButton> */}
              {
                currentEntity.getInventory().getItems().filter(item => item.getType() === 'weapon')
                  .map((item, index) => (
                    <WigglyActionButton
                      key={`waepon-${index}`}
                      clickFunction={() => handleAction(
                        ` attacks ${targetedEntity?.getName()} with ${item.getName()}`,
                        (item as Useable).getEffect()(targetedEntity!, currentEntity, aftereffects)
                      )}
                      disabled={processingTurn || !targetedEntity}
                    >
                      {item.getName()}
                    </WigglyActionButton>
                  ))}
            </div>
          )}
        </div>
        <div
          style={styles.dropdown}
          onClick={() => dropdownOpen === 'ability' ? setDropdownOpen('') : setDropdownOpen('ability')}
        >
          <span>✨ Use Ability</span>
          {dropdownOpen === 'ability' && (
            <div style={{
              ...styles.dropdownContainer,
              ...(dropdownOpen === 'ability' ? styles.dropdownOpen : styles.dropdownClosed),
              }}>
              {!currentEntity.getSkills().isEmpty() ? (
                currentEntity.getSkills().allSkills()
                  .map((skillEntry, index) => (
                    <WigglyActionButton
                      key={`ability-${index}`}
                      clickFunction={() => (currentEntity.getStats().getMana() >= skillEntry.getManaCost() ? (
                        currentEntity.expendMana(skillEntry.getManaCost()),
                        handleAction(
                          ` uses ${skillEntry.getName()} on ${targetedEntity?.getName()}`,
                          skillEntry.getEffect()(targetedEntity!, currentEntity, aftereffects)
                        )
                      ) : alert(`You don't have enough mana for ${skillEntry.getName()}`))}
                      disabled={processingTurn || !targetedEntity}
                    >
                      {skillEntry.getName()}
                    </WigglyActionButton>
                  ))
              ) : (
                <span>{currentEntity.getName()} hasn't learned any skills yet!</span>
              )}
            </div>
          )}
        </div>
        <div
          style={styles.dropdown}
          onClick={() => dropdownOpen === 'item' ? setDropdownOpen('') : setDropdownOpen('item')}
        >
          <span>🪧Use Item</span>
          {dropdownOpen === 'item' && (
            <div style={{
              ...styles.dropdownContainer,
              ...(dropdownOpen === 'item' ? styles.dropdownOpen : styles.dropdownClosed),
              }}>
              {!(currentEntity.getInventory().getItems().filter(item => (item.getType() === 'item')).length > 0) ? (
                <span>Inventory is empty</span>
              ) : (
                currentEntity.getInventory().getItems().filter(item => (item.getType() === 'item'))
                .map((item, index) => (
                  <WigglyActionButton
                    key={`item-${index}`}
                    clickFunction={() => {
                      handleAction(` uses ${item.getName()} on ${targetedEntity?.getName()}`,(item as Useable).getEffect()(targetedEntity!, currentEntity, aftereffects))
                      if (item.isConsumable()) currentEntity.getInventory().removeItemById(item.getId())
                    }}
                    disabled={processingTurn || !targetedEntity}
                  >
                    {item.getName()}
                  </WigglyActionButton>
                ))
              )}
            </div>
          )}
        </div>
        <button style={styles.dropdown}
          onClick={() => {
            handleAction(" enters a defensive stance!", [[0,0],[0,0],['defend','']]),
            setDropdownOpen('defend')
          }}
          disabled={processingTurn}
        >
          <span>🛡️Defend</span>
        </button>
        {/* use item */}
        {/* defend */}
        {/* flee */}
    </div>
  );
};

export default ActionButtons;

const styles: { [key: string]: React.CSSProperties } = {
  container: {
    display: "flex",
    flexWrap: "wrap",
    gap: "10px",
    marginBottom: "20px",
    justifyContent: "center",
  },
  dropdown: {
    position: "relative",
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    padding: "8px 16px",
    fontSize: "14px",
    fontFamily: "Garamond, serif",
    border: "2px solid #d4af37",
    borderRadius: "6px",
    backgroundColor: "#8b4513",
    color: "#ffd700",
    cursor: "pointer",
    transition: "all 0.3s ease",
    textShadow: "1px 1px 2px rgba(0,0,0,0.8)",
  },
  dropdownOpen: {
    zIndex: 12,
  },
  dropdownClosed: {
    zIndex: 10,
  },
  dropdownContainer: {
    position: "absolute",
    top: "100%",
    left: 0,
    right: 0,
    zIndex: 10,
    display: "flex",
    flexDirection: "column",
    gap: "5px",
    marginTop: "5px",
    padding: "10px",
    border: "2px solid #d4af37",
    borderRadius: "6px",
    backgroundColor: "rgba(26, 22, 17, 0.7)",
    color: "#ffd700",
    fontSize: "14px",
    width: "100%",
    boxShadow: "0 4px 8px rgba(0, 0, 0, 0.5)",
    minWidth: "200px"
  },
}

const WigglyActionButton: React.FC<{clickFunction: () => void, disabled: boolean, children: any}> = ({
  clickFunction, disabled, children
}) => {
  return (
    <button
      style={{
        ...buttonStyles.actionButton,
        ...(disabled ? buttonStyles.disabled : {})
      }}
      onClick={() => clickFunction()}
      disabled={disabled}
      onMouseEnter={(e) => {
        if (!disabled) Object.assign((e.target as HTMLButtonElement).style, buttonStyles.hover)
      }}
      onMouseLeave={(e) => {
        if (!disabled) Object.assign((e.target as HTMLButtonElement).style, buttonStyles.actionButton)
      }}
    >
      {children}
    </button>
  )
}

const buttonStyles: { [key: string]: React.CSSProperties } = {
  actionButton: {
    zIndex: 12,
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    padding: "8px",
    fontSize: "14px",
    fontFamily: "Garamond, serif",
    border: "2px solid #d4af37",
    borderRadius: "6px",
    backgroundColor: "#8b4513",
    color: "#ffd700",
    cursor: "pointer",
    transition: "all 0.3s ease",
    textShadow: "1px 1px 2px rgba(0,0,0,0.8)",
    width: "100%",
    opacity: 1,
  },
  disabled: {
    opacity: 0.5,
    cursor: 'default'
  },
  hover: {
    backgroundColor: "#d4af37",
    color: "#1a1611",
    transform: "scale(1.05)",
  },
}