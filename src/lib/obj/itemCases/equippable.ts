import { Entity } from "../entity";
import { Item } from "../item";

type equipType = (target: Entity) => boolean;

export class Equippable extends Item {
    private armorTypeLimit: number;
    private equipped: boolean;
    private onEquip: equipType;
    private onUnequip: equipType;

    constructor(
        type: string, // Armor or artifacts
        call: string,
        name: string,
        description: string,
        value: number,
        armorTypeLimit: number,
        equipped: boolean,
        onEquip: equipType,
        onUnequip: equipType,
        consumable: boolean = false // true for armor that breaks or artifacts that have only so many uses
    ) {
        super(type, call, name, description, value, consumable)
        this.armorTypeLimit = armorTypeLimit;
        this.equipped = equipped;
        this.onEquip = onEquip;
        this.onUnequip = onUnequip;
    }

    public getArmorTypeLimit(): number {
        return this.armorTypeLimit;
    }

    public isEquipped(): boolean {
        return this.equipped;
    }

    public setEquipped(equipped: boolean): void {
        this.equipped = equipped;
    }

    public getOnEquip(): equipType {
        return this.onEquip;
    }

    public getOnUnequip(): equipType {
        return this.onUnequip;
    }
}