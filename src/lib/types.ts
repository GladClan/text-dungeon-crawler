export type EventResult = {
    Success: boolean;
    RequiresTarget: boolean;
    Message: string;
    Targets: TargetOption[];
    Error: string;
}

export type TargetOption = {
    EntityId: string;
    Name: string;
}

export type SceneEvent = {
    id: number;
    dialogues: Dialogue[];
    options: Option[];
    existsActiveBattle: boolean;
    error: string;
}

export type Dialogue = {
    orderId: number;
    source: string;
    message: string;
}

export type Option = {
    optionId: number;
    optionTitle: string;
}

/*

export type Skill = {
    name: string,
    cost: number,
    description: string,
    element: string,
    proficiency: string,
    level: number
}

export type Item = {
    name: string,
    itemType: string,
    value: number,
    description: string
}

export type DamageableEntity = {
    name: string;
    entityType: string;
    race: string;
    maxHealth: number;
    currentHealth: number;
    maxMana: number;
    currentMana: number;
    magic: number;
    strength: number;
    defense: number;
    speed: number;
    level: number;
    experience: number;
    resistances: StringNum[];
    proficiencies: StringNum[];
    items: Item[];
    skills: Skill[]
    visible: boolean;
    isHidden: boolean;
    ai: string;
}

export type EntityCreator = {
    name: string,
    entityType: string,
    race: string,
    health: number,
    mana: number,
    magic: number,
    strength: number,
    defense: number,
    speed: number,
    level?: number,
    experience?: number,
    resistances?: { resistance: string, value: number }[],
    proficiencies?: { proficiency: string, value: number }[],
    visible?: boolean,
    isHidden?: boolean,
    ai?: string
}
*/