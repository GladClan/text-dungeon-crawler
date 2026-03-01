import React, { useState } from "react";
import { Entity } from "@/lib/obj/entity/entity";
import { Equippable } from "@/lib/obj/itemCases/equippable";

interface EquipModalProps {
  member: Entity;
  isOpen: boolean;
  onClose: () => void;
  onEquipmentChange: (member: Entity) => void;
}

const EquipModal: React.FC<EquipModalProps> = ({ 
  member, 
  isOpen, 
  onClose, 
  onEquipmentChange
}) => {
  const [selectedCategory, setSelectedCategory] = useState<string>("armor");
  const [hoveredItem, setHoveredItem] = useState<Equippable | null>(null);
  const [mousePosition, setMousePosition] = useState({ x: 0, y: 0 });

  if (!isOpen) return null;

  // Get all equippable items from inventory
  const equippableItems = member.getInventory().getItems()
    .filter(item => item instanceof Equippable) as Equippable[];

  // Equipment categories with their armor type limits
  const equipmentCategories = {
    armor: { name: "Armor", maxSlots: 3, armorTypes: [1, 2, 3] },
    artifacts: { name: "Artifacts", maxSlots: 5, armorTypes: [10] },
    weapons: { name: "Weapons", maxSlots: 2, armorTypes: [0] }
  };

  // Get items by category
  const getItemsByCategory = (category: string) => {
    const categoryData = equipmentCategories[category as keyof typeof equipmentCategories];
    return equippableItems.filter(item => 
      item.getType() === category.slice(0, -1) || // Remove 's' from category name
      (category === "artifacts" && item.getType() === "artifact") ||
      (category === "weapons" && item.getType() === "weapon")
    );
  };

  // Get equipped items by category
  const getEquippedByCategory = (category: string) => {
    const categoryData = equipmentCategories[category as keyof typeof equipmentCategories];
    return equippableItems.filter(item => 
      item.isEquipped() && 
      categoryData.armorTypes.includes(item.getArmorTypeLimit())
    );
  };

  // Handle equipment toggle
  const handleEquipToggle = (item: Equippable) => {
    const isCurrentlyEquipped = item.isEquipped();
    
    if (isCurrentlyEquipped) {
      // Unequip item
      const success = item.getOnUnequip()(member);
      if (success) {
        item.setEquipped(false);
        onEquipmentChange(member);
      }
    } else {
      // Check if we can equip this item
      const category = item.getType() === "artifact" ? "artifacts" : 
                     item.getType() === "weapon" ? "weapons" : "armor";
      const equipped = getEquippedByCategory(category);
      const categoryData = equipmentCategories[category as keyof typeof equipmentCategories];
      
      if (equipped.length < categoryData.maxSlots) {
        // Equip item
        const success = item.getOnEquip()(member);
        if (success) {
          item.setEquipped(true);
          onEquipmentChange(member);
        }
      } else {
        alert(`Cannot equip more ${category}. Maximum slots: ${categoryData.maxSlots}`);
      }
    }
  };

  // Handle mouse move for tooltip positioning
  const handleMouseMove = (e: React.MouseEvent, item: Equippable) => {
    setMousePosition({ x: e.clientX, y: e.clientY });
    setHoveredItem(item);
  };

  return (
    <div style={styles.overlay} onClick={onClose}>
      <div style={styles.modal} onClick={(e) => e.stopPropagation()}>
        {/* Modal Header */}
        <div style={styles.header}>
          <h2 style={styles.title}>
            ⚔️ {member.getName()}'s Equipment
          </h2>
          <button style={styles.closeButton} onClick={onClose}>
            ✕
          </button>
        </div>

        {/* Category Tabs */}
        <div style={styles.categoryTabs}>
          {Object.entries(equipmentCategories).map(([key, category]) => {
            const equipped = getEquippedByCategory(key);
            return (
              <button
                key={key}
                style={{
                  ...styles.categoryTab,
                  ...(selectedCategory === key ? styles.activeTab : {})
                }}
                onClick={() => setSelectedCategory(key)}
              >
                <div style={styles.tabContent}>
                  <span>{category.name}</span>
                  <span style={styles.slotCounter}>
                    {equipped.length}/{category.maxSlots}
                  </span>
                </div>
              </button>
            );
          })}
        </div>

        {/* Equipment Grid */}
        <div style={styles.content}>
          <div style={styles.equipmentGrid}>
            {getItemsByCategory(selectedCategory).map((item, index) => (
              <div
                key={item.getId()}
                style={{
                  ...styles.equipmentItem,
                  ...(item.isEquipped() ? styles.equippedItem : {})
                }}
                onMouseEnter={(e) => handleMouseMove(e, item)}
                onMouseMove={(e) => handleMouseMove(e, item)}
                onMouseLeave={() => setHoveredItem(null)}
                onClick={() => handleEquipToggle(item)}
              >
                <div style={styles.itemHeader}>
                  <span style={styles.itemName}>{item.getName()}</span>
                  {item.isEquipped() && (
                    <span style={styles.equippedBadge}>✓</span>
                  )}
                </div>
                
                <div style={styles.itemStats}>
                  <span style={styles.itemType}>
                    {item.getCall().charAt(0).toUpperCase() + item.getCall().slice(1)}
                  </span>
                  <span style={styles.itemValue}>
                    💰 {item.getValue()}
                  </span>
                </div>

                <div style={styles.itemActions}>
                  <button
                    style={{
                      ...styles.actionButton,
                      ...(item.isEquipped() ? styles.unequipButton : styles.equipButton)
                    }}
                    onClick={(e) => {
                      e.stopPropagation();
                      handleEquipToggle(item);
                    }}
                  >
                    {item.isEquipped() ? "Unequip" : "Equip"}
                  </button>
                </div>
              </div>
            ))}

            {/* Empty state */}
            {getItemsByCategory(selectedCategory).length === 0 && (
              <div style={styles.emptyState}>
                <span style={styles.emptyIcon}>📦</span>
                <span style={styles.emptyText}>
                  No {equipmentCategories[selectedCategory as keyof typeof equipmentCategories].name.toLowerCase()} available
                </span>
              </div>
            )}
          </div>
        </div>

        {/* Tooltip */}
        {hoveredItem && (
          <div
            style={{
              ...styles.tooltip,
              left: mousePosition.x + 10,
              top: mousePosition.y - 10
            }}
          >
            <div style={styles.tooltipHeader}>
              <span style={styles.tooltipTitle}>{hoveredItem.getName()}</span>
              <span style={styles.tooltipType}>
                {hoveredItem.getCall().charAt(0).toUpperCase() + hoveredItem.getCall().slice(1)}
              </span>
            </div>
            <div style={styles.tooltipDescription}>
              {hoveredItem.getDescription()}
            </div>
            <div style={styles.tooltipStats}>
              <div>💰 Value: {hoveredItem.getValue()}</div>
              <div>🔧 Equip Limit: {hoveredItem.getArmorTypeLimit()}</div>
              <div>🎒 Consumable: {hoveredItem.isConsumable() ? "Yes" : "No"}</div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default EquipModal;

const styles: { [key: string]: React.CSSProperties } = {
  overlay: {
    position: "fixed",
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    backgroundColor: "rgba(0, 0, 0, 0.8)",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    zIndex: 1000,
  },
  modal: {
    // width: "90%",
    maxWidth: "800px",
    maxHeight: "90vh",
    backgroundColor: "#1a1611",
    border: "3px solid #d4af37",
    borderRadius: "12px",
    fontFamily: "Garamond, serif",
    color: "#e6d3a3",
    overflow: "hidden",
    display: "flex",
    flexDirection: "column",
  },
  header: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    padding: "20px",
    borderBottom: "2px solid #8b6914",
    backgroundColor: "#2a1f15",
  },
  title: {
    margin: 0,
    fontSize: "24px",
    color: "#ffd700",
    textShadow: "2px 2px 4px rgba(0,0,0,0.8)",
  },
  closeButton: {
    background: "#8b0000",
    color: "#fff",
    border: "none",
    borderRadius: "50%",
    width: "30px",
    height: "30px",
    cursor: "pointer",
    fontSize: "18px",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
  },
  categoryTabs: {
    display: "flex",
    borderBottom: "2px solid #8b6914",
    backgroundColor: "#0f0b08",
  },
  categoryTab: {
    flex: 1,
    padding: "15px",
    backgroundColor: "#0F0B08",
    border: "none",
    color: "#8b6914",
    cursor: "pointer",
    fontSize: "16px",
    fontFamily: "Garamond, serif",
    transition: "all 0.3s ease",
    borderBottom: "3px solid transparent",
  },
  activeTab: {
    color: "#ffd700",
    backgroundColor: "#1a1611",
    borderBottom: "3px solid #d4af37",
  },
  tabContent: {
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    gap: "5px",
  },
  slotCounter: {
    fontSize: "12px",
    color: "#8b6914",
    fontWeight: "bold",
  },
  content: {
    flex: 1,
    overflow: "auto",
    padding: "20px",
  },
  equipmentGrid: {
    display: "grid",
    gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))",
    gap: "15px",
  },
  equipmentItem: {
    padding: "15px",
    border: "2px solid #8b6914",
    borderRadius: "8px",
    backgroundColor: "#0d0b08",
    cursor: "pointer",
    transition: "all 0.3s ease",
    position: "relative",
  },
  equippedItem: {
    borderColor: "#d4af37",
    backgroundColor: "#2a1f15",
    boxShadow: "0 0 10px rgba(212, 175, 55, 0.3)",
  },
  itemHeader: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: "8px",
  },
  itemName: {
    fontSize: "16px",
    fontWeight: "bold",
    color: "#ffd700",
  },
  equippedBadge: {
    backgroundColor: "#d4af37",
    color: "#1a1611",
    borderRadius: "50%",
    width: "20px",
    height: "20px",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    fontSize: "12px",
    fontWeight: "bold",
  },
  itemStats: {
    display: "flex",
    justifyContent: "space-between",
    marginBottom: "10px",
    fontSize: "14px",
  },
  itemType: {
    color: "#8b6914",
    fontStyle: "italic",
  },
  itemValue: {
    color: "#e6d3a3",
  },
  itemActions: {
    display: "flex",
    justifyContent: "center",
  },
  actionButton: {
    padding: "8px 16px",
    borderRadius: "6px",
    border: "2px solid",
    cursor: "pointer",
    fontSize: "14px",
    fontFamily: "Garamond, serif",
    transition: "all 0.3s ease",
  },
  equipButton: {
    backgroundColor: "#2d5016",
    borderColor: "#4a7c59",
    color: "#90ee90",
  },
  unequipButton: {
    backgroundColor: "#8b0000",
    borderColor: "#cd5c5c",
    color: "#ffcccb",
  },
  emptyState: {
    gridColumn: "1 / -1",
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    justifyContent: "center",
    padding: "40px",
    color: "#8b6914",
  },
  emptyIcon: {
    fontSize: "48px",
    marginBottom: "10px",
  },
  emptyText: {
    fontSize: "18px",
    fontStyle: "italic",
  },
  tooltip: {
    position: "fixed",
    backgroundColor: "#1a1611",
    border: "2px solid #d4af37",
    borderRadius: "8px",
    padding: "12px",
    maxWidth: "300px",
    zIndex: 1001,
    boxShadow: "0 4px 12px rgba(0,0,0,0.8)",
    pointerEvents: "none",
  },
  tooltipHeader: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: "8px",
    paddingBottom: "8px",
    borderBottom: "1px solid #8b6914",
  },
  tooltipTitle: {
    fontSize: "16px",
    fontWeight: "bold",
    color: "#ffd700",
  },
  tooltipType: {
    fontSize: "12px",
    color: "#8b6914",
    fontStyle: "italic",
  },
  tooltipDescription: {
    fontSize: "14px",
    color: "#e6d3a3",
    marginBottom: "8px",
    lineHeight: "1.4",
  },
  tooltipStats: {
    fontSize: "12px",
    color: "#8b6914",
    display: "flex",
    flexDirection: "column",
    gap: "2px",
  },
};