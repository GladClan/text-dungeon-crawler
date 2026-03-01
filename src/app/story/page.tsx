'use client';
import React from 'react';
import { useRouter } from 'next/navigation';

import { gameContext } from '@/context/gameContext';
import { StoryBox } from '@/components/storybox';
import { gamestate } from '@/lib/gamestateLib';
import { first } from '@/lib/prefabs/enemies/base';
import ChatOverlay from '@/components/Chat Overlay/chatOverlay';
import storyline from '@/lib/storylines/storyline.json';
import type { Scene } from '@/lib/storylines/storyline-type';

const storylineData = storyline as unknown as Scene[];

export default function App() {
    const router = useRouter();
    const contextValue = React.useContext(gameContext);
    if (!contextValue) { throw new Error("gameContext is not available"); }
    const { setEnemies, setGameState, eventTime } = contextValue;

    const [showOverlay, toggleShowOverlay] = React.useState(false);

    const handleContinueToBattle = () => {
        setEnemies(first);
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
            <button
                onClick={() => toggleShowOverlay(!showOverlay)}
            >
                Chat {showOverlay ? "true" : "fasle"}
            </button>
            {showOverlay &&
                <ChatOverlay
                    story={storylineData}
                    onSelectOption={(ref: string) => {
                        console.log('Selected option:', ref);
                    }}
                    onFight={() => {
                        setEnemies(first);
                        setGameState(gamestate.Battle);
                        router.push('/anew');
                    }}
                    onClose={() => toggleShowOverlay(!showOverlay)}
                />
            }
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