'use client';
import React from 'react';
import { useRouter } from 'next/navigation';

import { gameContext } from '@/context/gameContext';
import { StoryBox } from '@/components/storybox';
import { Entity } from '@/lib/obj/entity';
import { EntityInventory } from '@/lib/obj/entity/entityInventory';
import { exampleItems } from '@/lib/obj/exampleItems';
import { Element } from '@/lib/proficiency-elements';
import { gamestate } from '@/lib/gamestateLib';

export default function App() {
    const router = useRouter();
    const contextValue = React.useContext(gameContext);
    if (!contextValue) { throw new Error("gameContext is not available"); }
    const { setEnemies, setGameState, eventTime } = contextValue;

    const handleContinueToBattle = () => {
        // The old method: store monsters in local storage
        // localStorage.setItem('battleMonsters', JSON.stringify(["Skeleton Warrior", "Skeleton Archer", "Skeleton Mage"]));
        setEnemies([
            new Entity("Skeleton Warrior", "monster", 25, 0, 0, 40, 50, { ice: 0.5, poison: 1, lightning: 0.2, healing: 1.75, necrotic: 2 }, {}, Element.none, 0, 100)
                .setInventory(new EntityInventory().setGold(7).addItem(exampleItems.rustySabre)).setInitiative(16),
            new Entity("Skeleton Warrior", "monster", 25, 0, 0, 40, 50, { ice: 0.5, poison: 1, lightning: 0.2, healing: 1.75, necrotic: 2 }, {}, Element.none, 0, 100)
                .setInventory(new EntityInventory().setGold(12).addItem(exampleItems.oldSword)).setInitiative(16),
            new Entity("Skeleton Archer", "monster", 20, 0, 0, 30, 20, { ice: 0.5, poison: 1, lightning: 0.2, healing: 1.75, necrotic: 2 }, {}, Element.none, 0, 80)
                .setInventory(new EntityInventory().setGold(5).addItem(exampleItems.crumblingBow)),
            new Entity("Skeleton Archer", "monster", 20, 0, 0, 30, 20, { ice: 0.5, poison: 1, lightning: 0.2, healing: 1.75, necrotic: 2 }, {}, Element.none, 0, 80)
                .setInventory(new EntityInventory().setGold(5).addItem(exampleItems.crumblingBow)),
            new Entity("Skeleton Mage", "monster", 18, 60, 25, 5, 10, { ice: 0.5, poison: 1, lightning: 0.25, healing: 1.75, necrotic: 2 }, {}, Element.none, 0, 90)
                .setInventory(new EntityInventory().setGold(10).addItem(exampleItems.oldStaff).addItem(exampleItems.fireScroll).addItem(exampleItems.iceScroll)),
        ]);
        setGameState(gamestate.Battle);
        router.push('/battle');
    };

    const story = {
        intro: [
            "You descend the stairs of the dark dungeon, stepping down, down, down into the darkness. The wizard of your party casts magical tendrils of light to illuminate the way until you reach a wide, smooth hallway. The air is cool and musty, and the silence is only broken by the sound of your footsteps.",
            "You can see a faint light coming from the end of the hallway, the only hint of illumination in this dark place. As you approach, you notice that there is a distinct lack of dust or grime as you would expect in a tunnel that hasn't been opened in who knows how long.",
            "You reach the end of the hallway and find yourself in a large, open chamber. The walls are smooth and polished, and the floor is made of a strange, dark stone. In the center of the chamber, you see a large, ornate door made of a dark, polished wood. The door is adorned with intricate carvings and symbols that you don't recognize.",
            "You can feel a strange energy emanating from the door, and you know that this is the real entrance to the dungeon. You take a deep breath and prepare to enter, knowing that whatever lies beyond this door will test your skills and courage like never before.",
            "As you approach the door, the tendrils of light from the wizard's spell begin flow towards the dark walls, becoming absorbed into the stone. The light flickers and fades, leaving you in darkness only broken by the faint light coming from around the door of the chamber. You can hear the sound of your own breathing and the faint rustle of your companions' gear as they shuffle restlessly.",
            "There is a clattering sound from above you and you look up to see something sliding down from openings near the ceiling. Not one, but many thin, pale sticks and objects that you realize in horror are bones. They fall to the ground around your party with a clatter, and you see that among the bones is and assortment of armor and weapons, all of which are old and worn.",
            "You and your companions exchange looks and you can see your own shock mirrored in their faces. You're not sure what to make of this gruesome display, but it seems to be a warning of the dangers that lie ahead.",
            "Without warning, the bones begin to shift, rising and forming into skeletal figures that completely surround you and your party. The figures are tall and thin, with hollow eye sockets that seem to glow with an eerie light. They move with a jerky, unnatural grace, and you can hear the sound of their bones creaking as they move.",
            "You and your companions draw your weapons, ready to fight as the skeletons close in around you.",
        ]
    }

    return (
        <div style={styles.container}>
            <h1>Text Dungeon Crawler</h1>
            <StoryBox textArr={[...story.intro]} autonext={false} continueFunction={handleContinueToBattle} speed={eventTime * 25} />
        </div>
    )
}

const styles: { [key: string]: React.CSSProperties } = {
    container: {
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "100vh",
        padding: "20px",
        fontFamily: "Arial, sans-serif",
    }
}