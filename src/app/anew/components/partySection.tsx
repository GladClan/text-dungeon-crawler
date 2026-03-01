import { Entity } from "@/lib/obj/entity/entity";
import PartyMemberCard from "@/components/partyMemberCard";
import { useState } from "react";

const PartySection: React.FC<{
    party: Entity[],
    guests: Entity[],
    pets: Entity[],
    isCurrentEntity: (entity: Entity) => boolean,
}> = ({ party, guests, pets, isCurrentEntity }) => {
    const [openPane, setOpenPane] = useState('party');

    return (
        <div style={styles.container}>
            <div>
                <button
                    style={{
                        ...styles.paneButton,
                        ...(openPane === 'party' ? styles.openPaneButton : {}),
                        ...(guests.length === 0 && pets.length === 0 ? {minWidth: "250px"} : {})
                    }}
                    onClick={() => setOpenPane('party')}
                >
                    Party
                </button>
                { guests.length > 0 &&
                    <button
                        style={{...styles.paneButton, ...(openPane === 'guests' ? styles.openPaneButton : {})}}
                        onClick={() => setOpenPane('guests')}
                    >
                        Guests
                    </button>
                }
                { pets.length > 0 &&
                    <button
                        style={{...styles.paneButton, ...(openPane === 'pets' ? styles.openPaneButton : {})}}
                        onClick={() => setOpenPane('pets')}
                    >
                        Pets
                    </button>
                }
            </div>
                {/* party section */}
                { openPane === 'party' &&
                    <div>
                        {/* party */}
                        {party.map((member, index) => (
                            <div style={styles.card} key={index}>
                                <PartyMemberCard
                                    index={index}
                                    member={member}
                                    isActive={isCurrentEntity(member)}
                                />
                            </div>
                        ))}
                    </div>
                }
                {/* guests */}
                { openPane === 'guests' &&
                    <div>
                        {guests!.map((member, index) => (
                            <div style={styles.card} key={index}>
                                <PartyMemberCard
                                    index={index}
                                    member={member}
                                    isActive={isCurrentEntity(member)}
                                />
                            </div>
                        ))}
                    </div>
                }
                {/* pets */}
                { openPane === 'pets' &&
                    <div>
                        {pets!.map((member, index) => (
                            <div style={styles.card} key={index}>
                                <PartyMemberCard
                                    index={index}
                                    member={member}
                                    isActive={isCurrentEntity(member)}
                                />
                            </div>
                        ))}
                    </div>
                }
            </div>
  )
}

export default PartySection

const styles: { [key: string]: React.CSSProperties} = {
    container: {
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        maxHeight: "80vh",
        padding: "5px",
        border: "1px solid rgba(139, 105, 20, 0.5)",
        borderRadius: 10,
        boxShadow: "0 0 10px rgba(212, 175, 55, 0.3)",
        overflowY: "auto",
        // overflowX: "hidden",
        scrollbarColor: "#8b6914 #2c1810",
        scrollbarWidth: "thin",
    },
    paneButton: {
        minWidth: "50px",
        color: "#8b6914",
        margin: 5,
        backgroundColor: "#0f0b08"
    },
    openPaneButton: {
        minWidth: "150px",
        color: "#ffd700",
        backgroundColor: "#4b280e"
    },
    card: {
        margin: 10
    }
}