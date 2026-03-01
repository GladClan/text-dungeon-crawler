import { DialogueEntry } from "@/lib/storylines/storyline-type";
import React from "react";

interface TypewriterState {
    currentIndex: number;
    currentText: string;
    completedIndices: Set<number>;
    isTyping: boolean;
    isFinished: boolean;
    skipCurrent: () => void;
}

export function useSequentialTypewriter(
    chat: DialogueEntry[],
    speedMs = 22,
    pauseBetweenEntriesMs = 250,
): TypewriterState {
    const [currentIndex, setCurrentIndex] = React.useState(0);
    const [charCount, setCharCount] = React.useState(0);
    const [completedIndices, setCompletedIndices] = React.useState<Set<number>>(new Set());
    const [isTyping, setIsTyping] = React.useState(chat.length > 0);

    const hasDialogue = chat.length > 0;
    const isFinished = !hasDialogue || currentIndex >= chat.length;
    const activeEntry = hasDialogue && currentIndex < chat.length ? chat[currentIndex] : undefined;

    React.useEffect(() => {
        setCurrentIndex(0);
        setCharCount(0);
        setCompletedIndices(new Set());
        setIsTyping(chat.length > 0);
    }, [chat]);

    React.useEffect(() => {
        if (!activeEntry || !isTyping) {
            return;
        }

        if (charCount >= activeEntry.dialogue.length) {
            setIsTyping(false);
            setCompletedIndices((prev) => {
                const next = new Set(prev);
                next.add(currentIndex);
                return next;
            });
            return;
        }

    const timer = window.setTimeout(() => {
        setCharCount((prev) => prev + 1);
    }, speedMs);

    return () => window.clearTimeout(timer);
  }, [activeEntry, charCount, currentIndex, isTyping, speedMs]);

    React.useEffect(() => {
        if (isTyping || !hasDialogue || currentIndex >= chat.length || currentIndex === chat.length - 1) {
            return;
        }

        const timer = window.setTimeout(() => {
            setCurrentIndex((prev) => prev + 1);
            setCharCount(0);
            setIsTyping(true);
        }, pauseBetweenEntriesMs);

        return () => window.clearTimeout(timer);
    }, [chat.length, currentIndex, hasDialogue, isTyping, pauseBetweenEntriesMs]);

    const skipCurrent = () => {
        if (!activeEntry || !isTyping) {
            return;
        }
        setCharCount(activeEntry.dialogue.length);
        setIsTyping(false);
        setCompletedIndices((prev) => {
            const next = new Set(prev);
            next.add(currentIndex);
            return next;
        });
    };

    const currentText = activeEntry ? activeEntry.dialogue.slice(0, charCount) : '';

    const trulyFinished =
        !hasDialogue || (currentIndex === chat.length - 1 && completedIndices.has(chat.length - 1));

    return {
        currentIndex,
        currentText,
        completedIndices,
        isTyping,
        isFinished: trulyFinished || isFinished,
        skipCurrent,
    };
}