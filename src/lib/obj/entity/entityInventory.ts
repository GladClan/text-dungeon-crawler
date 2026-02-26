import { Item } from "../item";

export class EntityInventory {
    private items: Item[];
    private gold: number;

    constructor(initialItems?: Item[], initialGold?: number) {
        this.items = initialItems || [];
        this.gold = initialGold || 0;
    }

    public getGold(): number {
        return this.gold;
    }

    public setGold(amount: number): EntityInventory {
        if (amount < 0) {
            console.warn(`Cannot set gold to a negative value: ${amount}`);
            return this;
        }
        this.gold = amount;
        return this;
    }

    public addItem(item: Item): EntityInventory {
        this.items.push(item);
        return this;
    }

    public removeItemByName(itemName: string): Item | undefined {
        const item = this.items.find(item => item.getName() === itemName);
        if (item) {
            this.items = this.items.filter(item => item.getName() !== itemName);
        }
        return item;
    }

    public removeItemById(itemId: string): Item | undefined {
        const item = this.items.find(item => item.getId() === itemId);
        if (item) {
            this.items = this.items.filter(item => item.getId() !== itemId);
        }
        return item;
    }

    public removeItemByIndex(index: number): Item | undefined {
        if (index < 0 || index >= this.items.length) {
            console.warn(`Index ${index} is out of bounds for inventory.`);
            return undefined;
        }
        const item = this.items[index];
        this.items.splice(index, 1);
        return item;
    }

    public getItemByIndex(index: number): Item | undefined {
        if (index < 0 || index >= this.items.length) {
            console.warn(`Index ${index} is out of bounds for inventory.`);
            return undefined;
        }
        return this.items[index];
    }

    public getItems(): Item[] {
        return this.items;
    }

    public hasItem(itemName: string): boolean {
        return this.items.some(item => item.getName() === itemName);
    }
    
    public clear(): void {
        this.items = [];
    }
    
    public getItemCount(): number {
        return this.items.length;
    }
    
    public toString(): string {
        return `Inventory:\n${this.items.map(item => item.toString()).join("\n")}`;
    }

    public clone(): EntityInventory {
        return new EntityInventory([...this.items]);
    }
    
    public merge(other: EntityInventory): void {
        this.items = [...new Set([...this.items, ...other.getItems()])];
    }
    
    public isEmpty(): boolean {
        return this.items.length === 0;
    }
    
    public size(): number {
        return this.items.length;
    }
}