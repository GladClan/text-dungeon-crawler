export class EntityMetadata {
    private static entityCounter: number = 0; // Static counter for unique IDs
    private entityId: string; // Unique identifier for the entity
    private entityType: string; // Type of the entity (e.g., player, npc, monster)
    private name: string; // Name of the entity

    constructor(name: string, entityType: string, entityId?: string) {
        this.name = name;
        this.entityType = entityType;
        this.entityId = entityId || this.generateEntityId(); // Use provided ID or generate a new one
    }

    private generateEntityId(): string {
        // Increment counter for uniqueness
        EntityMetadata.entityCounter++;

        // Create a readable ID with timestamp and counter
        const timestamp = Date.now().toString(36); // Base36 for shorter string
        const counter = EntityMetadata.entityCounter.toString(36).padStart(3, '0');
        const typePrefix = this.entityType.substring(0, 3).toLowerCase();
        
        return `${typePrefix}_${timestamp}_${counter}`;
    }

    // Alternative UUID-style generator (more complex but globally unique)
    private generateUUID(): string {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
            const r = Math.random() * 16 | 0;
            const v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }

    public getEntityId(): string {
        return this.entityId;
    }

    public getEntityType(): string {
        return this.entityType;
    }

    public getName(): string {
        return this.name;
    }

    public setName(name: string): void {
        this.name = name;
    }
}