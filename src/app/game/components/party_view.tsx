import { GetParty } from "@/lib/api";
import { Entity } from "@/lib/types";
import React from "react";

interface props {
    partyId: string,
    setViewParty: (value: boolean) => void  //React.Dispatch<React.SetStateAction<boolean>>
}

const X = <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M18 6 6 18"/><path d="m6 6 12 12"/></svg>

const PartyViewer: React.FC<props> = ({partyId, setViewParty}) => {
    const [refresh, setRefresh] = React.useState(false);
    const [party, setParty] = React.useState<Entity[] | null>(null);
    const [inventoryOpen, setInventoryOpen] = React.useState('');
    const [skillsOpen, setSkillsOpen] = React.useState('');
    const [itemOrSkillSelected, setItemOrSkillSelected] = React.useState('');
    const [showResistances, setShowResistances] = React.useState('');
    const [showProficiencies, setShowProficiencies] = React.useState('');

    const MatchOrNull = (item: string, match: string) => {
        if (item === match)
            return 'null';
        else
            return item;
    }

    React.useEffect(() => {
        // Get the party using the partyId parameter from the constructor
        async function LoadParty() {
            try {
                const result = await GetParty(partyId);
                setParty(result);
            } catch (err) {
                const message = err instanceof Error ? err.message : "Failed to load party";
                console.log(message);
            } finally {
                // 
            }
        }

        LoadParty();
    }, [refresh]);

    return (
        <div style={styles.modal}>
            <div style={styles.container}>
                <button 
                    style={styles.button}
                    onClick={() => setViewParty(false)}
                >{X}</button>
                <button
                    style={styles.button}
                    onClick={() => setRefresh(!refresh)}
                >
                    Refresh
                </button>
                <div style={styles.memberslist}>
                    {party && party.map((member, i) => (
                        <div key={member.id} style={styles.card}>
                            <h2>{member.name}</h2>
                            <p>{member.race}</p>
                            <p>Health: {member.currentHealth}/{member.maxHealth}</p>
                            <p>Mana: {member.currentMana}/{member.maxMana}</p>
                            <p>Strength: {member.strength}</p>
                            <p>Defense: {member.defense}</p>
                            <p>Magic: {member.magic}</p>
                            <p>Speed: {member.speed}</p>
                            <p>Level: {member.level}</p>
                            <p>Experience: {member.experience}</p>
                            <h3 style={styles.selector} onClick={() => setShowResistances(MatchOrNull(member.id, showResistances))}>Resistances</h3>
                            <ul>
                                {showResistances === member.id && Object.entries(member.resistances).map(([key, value], n) =>
                                    (value !== 0 && <li key={`${key}-${n}`} style={{marginLeft: "20px"}}>{key}: {value.toLocaleString('en-US', { style: 'percent' })}</li>)
                                )}
                            </ul>
                            <h3 style={styles.selector} onClick={() => setShowProficiencies(MatchOrNull(member.id, showProficiencies))}>Proficiencies</h3>
                            <ul>
                                {showProficiencies === member.id && Object.entries(member.proficiencies).map(([key, value], n) =>
                                    <li key={`${key}-${n}`} style={{marginLeft: "20px"}}>{key}: {value}</li>
                                )}
                            </ul>
                            <h2>Inventory</h2>
                            <h3>Gold: {member.inventory.gold}</h3>
                            <p
                                style={styles.selector} onClick={() => setInventoryOpen(MatchOrNull(member.id, inventoryOpen))}
                            >
                                {inventoryOpen === member.id ? "Hide inventory" : "Show inventory"}
                            </p>
                            <ul>
                                {inventoryOpen === member.id && 
                                    (member.inventory.items.length == 0 ?
                                        <li>Empty...</li>
                                        :
                                        member.inventory.items.map((item, n) =>
                                            <li key={`${n}-${item.tag}`} style={{marginLeft: "20px"}}>
                                                <p style={styles.selector} onClick={() => setItemOrSkillSelected(MatchOrNull(item.name, itemOrSkillSelected))}>{item.name}</p>
                                                {itemOrSkillSelected === item.name &&
                                                    <ul style={{marginLeft: "20px"}}>
                                                        <li>{item.description}</li>
                                                        <li>Sell price: {item.sellable ? item.value : "Not sellable"}</li>
                                                        {item.armorType && item.equipped && 
                                                            <li><strong>Equipped.</strong></li>
                                                        }
                                                        {item.armorType &&
                                                            <li>Equipment type: {item.armorType}</li>
                                                        }
                                                        <li>Element: {item.element}</li>
                                                        <li>Proficiency: {item.proficiency}</li>
                                                    </ul>
                                                }
                                            </li>
                                        )
                                    )
                                }
                            </ul>
                            <h2>Skills</h2>
                            <p
                                style={styles.selector} onClick={() => setSkillsOpen(MatchOrNull(member.id, skillsOpen))}
                            >
                                {skillsOpen === member.id ? "Hide skills" : "Show skills"}
                            </p>
                            <ul>
                                {skillsOpen === member.id && (
                                    member.skills.length == 0 ?
                                    <li>Empty...</li>
                                    :
                                    member.skills.map((s, n) => 
                                        <li key={`${n}-${s.tag}`} style={{marginLeft: "20px"}}>
                                            <p style={styles.selector} onClick={() => setItemOrSkillSelected(s.name)}>{s.name}</p>
                                            {itemOrSkillSelected === s.name &&
                                                <ul style={{marginLeft: "20px"}}>
                                                    <li>{s.description}</li>
                                                    <li>Cost: {s.cost}</li>
                                                    <li>Level: {s.level}</li>
                                                    <li>Element: {s.element}</li>
                                                    <li>Proficiency: {s.proficiency}</li>
                                                </ul>
                                            }
                                        </li>
                                    )
                                )}
                            </ul>
                            {/* 
                                id: string;
                                
                                entityType: string;
                                
                                partyId: string;
                                
                                healthBuffer: number;
                                
                                attackDamageType: string;
                                dealsMagicDamage: boolean;
                                isEntityAlive: boolean;
                                displayStats: boolean;
                            */}
                        </div>
                        ))
                    }
                </div>
            </div>
        </div>
    )
}

const styles: { [key: string]: React.CSSProperties} = {
    modal: {
        position: "fixed",
        inset: 0,
        display: "flex",
        backgroundColor: "rgba(0, 0, 0, 0.5)",
        backdropFilter: "blur(10px)",
        zIndex: 99,
    },
    container: {
        alignSelf: "center",
        width: "90%",
        height: "90%",
        background: "linear-gradient(135deg, #160d00, #342000)",
        margin: "10%",
        overflow: "auto",
    },
    button: {
        cursor: "pointer",
        backgroundColor: "#e6d3a3",
        margin: "20px",
        padding: "5px",
        borderRadius: "50px",
    },
    memberslist: {
        marginLeft: "20px",
        display: "flex",
        flexDirection: "row",
        // flex: "1 1 200px",
        flexWrap: "wrap",
        gap: "10px",
    },
    card: {
        width: "250px",
        boxShadow: "0 0 20px rgba(139,105,20,0.7)",
        padding: "10px",
        // alignItems: "normal",
    },
    selector: {
        cursor: "pointer",
        textDecoration: "underline"
    }
}

export default PartyViewer;