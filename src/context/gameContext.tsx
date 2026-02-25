"use client"
import React, { createContext, useState } from 'react';
import { Entity } from '@/lib/obj/entity';
import { errorMonster, defaultParty } from '@/lib/errorMonster';

type gameContextType = {
    party: Entity[];
    setParty: React.Dispatch<React.SetStateAction<Entity[]>>;
    enemies: Entity[];
    setEnemies: React.Dispatch<React.SetStateAction<Entity[]>>;
    guests: Entity[];
    setGuests: React.Dispatch<React.SetStateAction<Entity[]>>;
    pets: Entity[];
    setPets: React.Dispatch<React.SetStateAction<Entity[]>>;
    gameState: string; // Current game state (e.g., "battle", "exploration", etc.)
    setGameState: React.Dispatch<React.SetStateAction<string>>;
    eventTime: number; // Speed of the game (e.g., normal, fast, slow)
    setEventTime: React.Dispatch<React.SetStateAction<number>>;
};

export const gameContext = createContext<gameContextType | null>(null);

export const GameProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [party, setParty] = useState<Entity[]>(defaultParty); // Default party members
    const [guests, setGuests] = useState<Entity[]>([]);
    const [pets, setPets] = useState<Entity[]>([]);
    const [enemies, setEnemies] = useState<Entity[]>([errorMonster]);
    const [gameState, setGameState] = useState<string>("story"); // Default game state is "story"
    const [eventTime, setEventTime] = useState<number>(0.2); // Default event time is 0.75, smaller is faster

    return (
        <gameContext.Provider value={{ party, setParty, guests, setGuests, pets, setPets, enemies, setEnemies, gameState, setGameState, eventTime, setEventTime }}>
            {children}
        </gameContext.Provider>
    );
};