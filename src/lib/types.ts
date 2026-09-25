export type EventResult = {
    success: boolean;
    requiresTarget: boolean;
    message: string;
    targets: TargetOption[];
    error: string;
}

export type TargetOption = {
    entityId: string;
    name: string;
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

export type Entity = {
    id: string;
    name: string;
    entityType: string;
    race: string;
    partyId: string;
    maxHealth: number;
    currentHealth: number;
    healthBuffer: number;
    magic: number;
    maxMana: number;
    currentMana: number;
    strength: number;
    defense: number;
    attackDamageType: string;
    dealsMagicDamage: boolean;
    level: number;
    experience: number;
    isEntityAlive: boolean;
    displayStats: boolean;
    speed: number;
    resistances: {[key: string]: number};
    proficiencies: {[key: string]: number};

    inventory: Inventory;
    skills: Skill[];

    aI: string;
    deathMessage: string;
    playerControlled: boolean;
    error: string;

    // Only if entity is BestiaryEntity
    description: string;
    bestiaryEntry: string;
    journalEntry: string;
    loreEntry1: string;
    loreEntry2: string;
}

export type Skill = {
    id: string;
    name: string;
    description: string;
    tag: string;
    cost: number;
    element: string;
    proficiency: string;
    multiTarget: boolean;
    targetsLimit: number;
    level: number;
    error: string;
}

export type Inventory = {
    gold: number;
    items: Item[];
}

export type Item = {
    id: string;
    name: string;
    tag: string;
    value: string;
    description: string;
    consumable: boolean;
    sellable: boolean;
    canUse: boolean;
    element: string;
    proficiency: string;
    armorType: string;
    armorTypeLimit: number;
    targetsLimit: number;
    equipped: boolean;
    error: string;
}

export type Battle = {
    partyId: string;
    opponentPartyId: string;
    currentRound: number;
    initiativeOrder: Initiative[];
    entityDtos: Entity[];
}

export type Initiative = {
    initiative: number;
    entityName: string;
    entityId: string;
}
