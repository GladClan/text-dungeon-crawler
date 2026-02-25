import { Item } from "../item";

import { effectType } from "@/lib/battleFunctions";

export class Useable extends Item {
    private effect: effectType;

    constructor(
        type: string, //weapons or items
        call: string,
        name: string,
        description: string,
        value: number,
        effect: effectType,
        consumable: boolean = false // True for potions or scrolls
    ) {
        super(type, call, name, description, value, consumable);
        this.effect = effect;
    }

    public getEffect() {
        return this.effect;
    }
}