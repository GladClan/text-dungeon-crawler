'use client';

import { useEffect, useMemo, useState } from 'react';
import type { ContentBlock, DialogueEntry, FightConfig, Option, Scene } from '@/lib/storylines/storyline-type';
import { useSequentialTypewriter } from './typewriter';
import DialogueLine from './dialogueLine';
import WigglyButton from '../wigglyButton';

const FightNotice: React.FC<{fight: FightConfig}> = ({ fight }) => {
    const fightBox: React.CSSProperties = {
        border: '1px solid #8b6914',
        borderRadius: 12,
        background: 'linear-gradient(145deg, rgba(52, 30, 18, 0.92) 0%, rgba(31, 18, 11, 0.94) 100%)',
        padding: '10px 12px',
        display: 'flex',
        flexDirection: 'column',
        gap: 4,
    };
    const fightTitle: React.CSSProperties = {
        fontWeight: 700,
        letterSpacing: 0.4,
        color: '#e6d3a3',
    };
    const fightText: React.CSSProperties = {
        fontSize: 13,
        color: '#d8c18b'
    }

    return (
        <div style={fightBox}>
            <div style={fightTitle}>⚔ Encounter Incoming</div>
            <div style={fightText}>Enemies: {fight.enemies.join(', ')}</div>
            <div style={fightText}>Target rule: {fight['ai-target']}</div>
            <div style={fightText}>
                Next scene after combet: {fight['next-scene'] ? 'Yes' : 'No'}
            </div>
        </div>
    )
}

function getFirstBlockKey(scene: Scene): string | null {
    const entries = Object.entries(scene.content);
    if (entries.length === 0) {
        return null;
    }

    const [firstKey] = entries.sort((left, right) => left[1].order - right[1].order)[0];
    return firstKey;
}

interface ChatOverlayProps {
  story: Scene[];
  onSelectOption?: (ref: string) => void;
  onFight?: (fight: FightConfig) => void;
  onSceneEnd?: (scene: Scene) => void;
  onClose?: () => void;
}

export default function ChatOverlay({ story, onSelectOption, onFight, onSceneEnd, onClose }: ChatOverlayProps) {
    const sortedScenes = useMemo(() => {
        return [...story].sort((left, right) => left.sequence - right.sequence);
    }, [story]);

    const [currentSceneSequence, setCurrentSceneSequence] = useState<number | null>(null);
    const [currentBlockKey, setCurrentBlockKey] = useState<string | null>(null);

    useEffect(() => {
        if (story.length === 0) {
            return;
        }
        const firstScene = [...story].sort((a, b) => a.sequence - b.sequence)[0] ?? null;
        if (!firstScene) {
            setCurrentSceneSequence(null);
            setCurrentBlockKey(null);
            return;
        }

        const firstBlockKey = getFirstBlockKey(firstScene);
        setCurrentSceneSequence(firstScene.sequence);
        setCurrentBlockKey(firstBlockKey);
    }, [story]);

    const currentScene = useMemo(() => {
        if (currentSceneSequence === null) {
            return null;
        }

        return sortedScenes.find((scene) => scene.sequence === currentSceneSequence) ?? null;
    }, [currentSceneSequence, sortedScenes]);

    const block: ContentBlock | null = useMemo(() => {
        if (!currentScene || !currentBlockKey) {
            return null;
        }

        return currentScene.content[currentBlockKey] ?? null;
    }, [currentBlockKey, currentScene]);

    const blockChat = block?.chat ?? [];
    const { currentIndex, currentText, completedIndices, isTyping, isFinished, skipCurrent } =
        useSequentialTypewriter(blockChat);

    useEffect(() => {
        if (!block || !isFinished) {
            return;
        }

        if (block.fight && onFight) {
            onFight(block.fight);
            return;
        }

        const hasOptions = (block.options?.length ?? 0) > 0;
        if (!hasOptions && !block.fight && currentScene && onSceneEnd) {
            onSceneEnd(currentScene);
        }
    }, [block, currentScene, isFinished, onFight, onSceneEnd]);

    const renderedDialogue = useMemo(() => {
        return blockChat
            .map((entry, index) => {
                if (index < currentIndex) {
                    return { entry, text: entry.dialogue };
                }

                if (index === currentIndex) {
                    const text = completedIndices.has(index) ? entry.dialogue : currentText;
                    return { entry, text };
            }

            return null;
        })
            .filter((item): item is { entry: DialogueEntry; text: string } => item !== null);
    }, [blockChat, completedIndices, currentIndex, currentText]);

    const options: Option[] = block?.options ?? [];
    const showOptions = isFinished && options.length > 0;
    const showFightNotice = isFinished && !!block?.fight;
    const isStoryEmpty = sortedScenes.length === 0;
    const hasValidBlock = !!block;
    const isSceneEnd = isFinished && hasValidBlock && !showOptions && !showFightNotice;

    const handleSelectOption = (optionRef: string) => {
        onSelectOption?.(optionRef);
        if (!currentScene) {
            return;
        }
        const nextBlock = currentScene.content[optionRef];
        if (!nextBlock) {
            return;
        }

        setCurrentBlockKey(optionRef);
    };

    return (
        <div style={styles.backdrop}>
            <div style={styles.modal} onClick={isTyping ? skipCurrent : undefined}>
                {onClose && (
                    <button type="button" style={styles.closeButton} onClick={onClose}>
                        ×
                    </button>
                )}

                {currentScene && <div style={styles.sceneTitle}>Scene: {currentScene.name}</div>}

                {isStoryEmpty && <div style={styles.systemMessage}>No story content found.</div>}

                {!isStoryEmpty && !hasValidBlock && (
                    <div style={styles.systemMessage}>Unable to resolve the starting story block.</div>
                )}

                {hasValidBlock && (
                    <>
                        <div style={styles.metaLine}>
                            {block.acquire && <span style={styles.metaTag}>Acquire: {block.acquire}</span>}
                            {block.effect && <span style={styles.metaTag}>Effect: {block.effect}</span>}
                            {block.require && <span style={styles.metaTag}>Require: {block.require}</span>}
                        </div>

                        <div style={styles.dialogueColumn}>
                            {renderedDialogue.map(({ entry, text }, index) => (
                                <DialogueLine key={`${entry.source}-${index}`} entry={entry} text={text} />
                            ))}
                        </div>

                        {showFightNotice && block.fight && <FightNotice fight={block.fight} />}

                        {showOptions && (
                            <div style={styles.optionsRow}>
                                {options.map((option) => (
                                    <WigglyButton
                                        func={() => handleSelectOption(option.ref)}
                                    >
                                        {option.title}
                                    </WigglyButton>
                                ))}
                            </div>
                        )}

                        {isSceneEnd && <div style={styles.systemMessage}>End of scene reached.</div>}
                    </>
                )}
            </div>
        </div>
    );
}

const styles: Record<string, React.CSSProperties> = {
    backdrop: {
        position: 'fixed',
        inset: 0,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: 'rgba(8, 4, 2, 0.7)',
        zIndex: 9999,
        padding: 16,
    },
    modal: {
        position: 'relative',
        minWidth: "100vw",
        minHeight: "100vh",
        // width: 'min(900px, 100%)',
        // maxHeight: 'min(90vh, 900px)',
        overflowY: 'auto',
        borderRadius: 14,
        background: 'linear-gradient(145deg, rgba(26, 22, 17, 0.6) 0%, rgba(13, 11, 8, 0.8) 100%)',
        border: '2px solid #8b6914',
        boxShadow: '0 16px 36px rgba(0, 0, 0, 0.55), inset 0 0 20px rgba(139, 105, 20, 0.22)',
        padding: '20px 18px 16px',
        color: '#e6d3a3',
        display: 'flex',
        flexDirection: 'column',
        gap: 14,
        // cursor: 'pointer',
    },
    closeButton: {
        position: 'absolute',
        top: 10,
        right: 12,
        border: 'none',
        width: 30,
        height: 30,
        borderRadius: 999,
        fontSize: 20,
        lineHeight: '30px',
        textAlign: 'center',
        color: '#e6d3a3',
        backgroundColor: 'rgba(139,105,20,0.35)',
        cursor: 'pointer',
    },
    metaLine: {
        display: 'flex',
        flexWrap: 'wrap',
        gap: 8,
        paddingRight: 38,
    },
    metaTag: {
        fontSize: 12,
        borderRadius: 999,
        padding: '4px 10px',
        backgroundColor: 'rgba(139, 105, 20, 0.22)',
        border: '1px solid rgba(139, 105, 20, 0.55)',
        color: '#e6d3a3',
    },
    dialogueColumn: {
        display: 'flex',
        flexDirection: 'column',
        gap: 10,
        minHeight: 180,
        flex: 1,
    },
    optionsRow: {
        display: 'flex',
        flexWrap: 'wrap',
        gap: 10,
        justifyContent: 'center',
        alignItems: 'flex-end',
        marginTop: 'auto',
        padding: 12,
    },
    sceneTitle: {
        fontWeight: 700,
        color: '#e6d3a3',
        opacity: 0.95,
    },
    systemMessage: {
        textAlign: 'center',
        color: '#cbb27a',
        opacity: 0.9,
        fontStyle: 'italic',
    },
};