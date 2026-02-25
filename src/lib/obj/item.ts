export class Item {
    private id : string;
    private static nextId: number = 0; // Static counter for unique IDs
    private type: string;
    private call: string;
    private name: string;
    private description: string;
    private value: number;
    private consumable: boolean = false;

    constructor(
        type: string,
        call: string,
        name: string,
        description: string,
        value: number,
        consumable: boolean = false
    ) {
        this.id = this.generateId(type, call, name);
        this.type = type;
        this.call = call;
        this.name = name;
        this.description = description;
        this.value = value;
        this.consumable = consumable;
    }

    private generateId(type: string, call: string, name: string): string {
        return `${type.substring(0, 2)}-${call.substring(0, 2)}-${name.substring(0, 3)}-${Item.nextId++}`.toLowerCase();
    }

    public getId(): string {
        return this.id;
    }
    
    public getType(): string {
        return this.type;
    }

    public getCall(): string {
        return this.call;
    }

    public getName(): string {
        return this.name;
    }

    public getDescription(): string {
        return this.description;
    }

    public getValue(): number {
        return this.value;
    }

    public isConsumable(): boolean {
        return this.consumable;
    }

    public toString(): string {
        return `${this.name} (${this.description} - Value: ${this.value})`;
    }
}