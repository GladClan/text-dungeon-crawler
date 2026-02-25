import { Element } from "@/lib/proficiency-elements";
import { Entity } from "../entity";
import { EntityStats } from "./entityStats";
import { returnType, returnTypeSingleton } from "@/lib/battleFunctions";

export class DamageableEntity {
    /**
     * TODOs:
     * - Figure out the location math in the physicalAttack and takeDamage functions
     * - Enable storing timers for damage effects to be able to cancel timers later
     * - Consider adding more entity types such as npcs, players, etc and how they would interact
     * - Consider a common damage formula: Damage = Attack / (1 + (Defense / c)), where c is a constant that determines how quickly defense reduces damage.
     *      "This formula provides diminishing returns, meaning each additional point of defense has less impact as defense increases, preventing extreme damage reduction."
     */
    // TODO Make a cap for proficiencies in entityStats.ts
    // TODO Implement adding to proficiencyEntries in entityStats.ts, perhaps when the item or skill is  used, or as part of the function in an item or skill.
    
    private stats: EntityStats;
    private armorMultipliers: { [key: string]: number };
    private strength: number;
    private strengthMultipliers: { [key: string]: number };
    private resistances: { [key: string]: number };
    private statusEffects: string[];
    private element: Element;
    private location: string; // Current location of the entity (north, center, or south)
    private facing: string = "north"; // Facing direction of the entity. Can be "north" or "south"

    constructor(
        health: number,
        mana: number,
        magic: number,
        strength: number,
        defense: number,
        proficiencies: { [key: string]: number } = {},
        resistances: { [key: string]: number } = {},
        element?: Element,
        level?: number,
        experience?: number,
    ) {
        this.stats = new EntityStats(health, mana, magic, strength, defense, proficiencies, level, experience);
        this.armorMultipliers = {};
        this.strength = strength;
        this.strengthMultipliers = {};
        this.resistances = resistances;
        this.statusEffects = [];
        this.element = element || Element.none;
        this.location = "";
    }

    public getStats(): EntityStats {
        return this.stats;
    }

    public heal(amount: number): returnType {
        if (!this.stats.isAlive()) {
            console.warn(`Entity is not alive and cannot be healed.`);
            return [[amount, 0], [0,0], ['', 'health']];
        }
        const result = Math.min(amount * (this.resistances["healing"] || 1), this.getStats().getMaxHealth()); // Apply healing resistance if exists
        this.stats.setHealth(result + this.stats.getHealth());
        return [[amount, -result], [0,0], ['', 'health']]
    }

    public expendMana(amount: number): returnType {
        if (!this.stats.isAlive()) {
            console.warn(`Entity is not alive and cannot expend mana.`);
            return [[amount, 0], [0,0], ['regained mana', 'mana']];
        }
        this.stats.setMana(this.stats.getMana() - amount);
        return [[amount, amount], [0,0], ['', 'mana']]
    }

    // Default attack function, for weaponless entities or basic attacks
    public physicalAttack(target: Entity, damageType: string = this.element): returnType {
        if (!this.stats.isAlive()) {
            console.warn(`Entity is not alive and cannot attack.`);
            return [[0, 0], [0,0], [damageType, 'health']]
        }
        let damage = this.strength * this.getStrengthMultiplier();
        damage *= this.stats.getProficiency("hand-to-hand");
        if (!target.stats.isAlive()) {
            console.warn(`Entity is already dead and cannot be attacked.`);
            return [[damage, 0], [0,0], [damageType, 'health']];
        }
        // Calculate damage based on strength and multipliers
        /*
         * Location math -_-
         */
        this.stats.addProficiencyEntry("hand-to-hand");
        const result = target.takeDamage(damage, damageType, this);
        return [[damage, result[0][1]], result[1], [damageType, 'health']];
    }

    public takeDamage(amount: number, damageType: string, source: DamageableEntity): returnType {
        if (!this.stats.isAlive()) {
            console.warn(`Entity is already dead and cannot take damage.`);
            return [[amount, 0], [0,0], [damageType, 'health']];
        }
        // Apply armor reduction
        /*
         * Location math -_-
         */
        const total = this.applyDamage(amount / this.getArmorMultiplier(), damageType);

        // Handle thorns effect
        let thorns: returnTypeSingleton = [0,0, ['','']];
        if (this.stats.isAlive() && "thorns" in this.statusEffects) {
            thorns = source.damageEffect(amount * 0.5, damageType, this);
        }
        return [[amount, total], [thorns[0], thorns[1]], [damageType, 'health']];
    }

    public takeMagicDamage(amount: number, damageType: string, source: DamageableEntity): returnType {
        const result = this.applyDamage(amount, damageType);
        
        // Handle reflect effect
        let reflect: returnTypeSingleton = [0,0,['','']]
        if (this.stats.isAlive() && "reflect" in this.statusEffects) {
            reflect = source.damageEffect(amount * 0.5, damageType, this);
        }
        return [[amount, result], [reflect[0],reflect[1]], [damageType, 'health']];
    }

    public damageEffect(amount: number, damageType: string, source: DamageableEntity): returnTypeSingleton {
        const result = this.applyDamage(amount, damageType);
        return [amount, result, [damageType, 'health']]
    }

    private applyDamage(amount: number, damageType: string): number {
        const resistance = this.resistances[damageType] || 0; // Calculate resistance (0 = no resistance, 1+ = resistance/immunity/healing)
        let damageToTake = amount * (1 - resistance);
        damageToTake = Math.min(this.stats.getHealth(), damageToTake)
        
        this.stats.setHealth(this.stats.getHealth() - damageToTake);
        return damageToTake;
    }
    
    public addStrengthMultiplier(type: string, multiplier: number): void {
        this.strengthMultipliers[type] = (this.strengthMultipliers[type] || 1) * multiplier;
    }

    private getStrengthMultiplier(): number {
        let multiplier = 1; // Default multiplier
        for (const key in this.strengthMultipliers) {
            multiplier *= this.strengthMultipliers[key];
        }
        return multiplier;
    }

    public removeStrengthMultiplier(type: string): void {
        if (this.strengthMultipliers[type]) {
            delete this.strengthMultipliers[type];
        } else {
            console.warn(`No strength multiplier of type "${type}" found.`);
        }
    }

    public addArmorMultiplier(type: string, multiplier: number): void {
        this.armorMultipliers[type] = (this.armorMultipliers[type] || 1) + multiplier;
    }

    public removeArmorMultiplier(type: string): boolean {
        if (this.armorMultipliers[type]) {
            delete this.armorMultipliers[type];
            return true;
        } else {
            console.warn(`No armor multiplier of type "${type}" found.`);
            return false;
        }
    }

    private getArmorMultiplier(): number {
        let multiplier = 1; // Default multiplier
        for (const key in this.armorMultipliers) {
            multiplier *= this.armorMultipliers[key];
        }
        return multiplier;
    }

    public getResistances(): { [key: string]: number } {
        return this.resistances;
    }

    public addResistance(type: string, value: number): void {
        if (this.resistances[type]) {
            this.resistances[type] += value;
        } else {
            this.resistances[type] = value;
        }
    }

    public getResistance(type: string): number {
        return this.resistances[type] || 0;
    }

    public removeResistance(type: string, value: number): void {
        if (this.resistances[type]) {
            this.resistances[type] -= value;
        } else {
            console.warn(`No resistance of type "${type}" found.`);
            this.resistances[type] = 0 - value; // If not found, set it to negative value
        }
    }

    public addStatusEffect(effect: string): boolean {
        if (this.statusEffects.indexOf(effect) < 0) {
            this.statusEffects.push(effect);
            return true;
        }
        return false;
    }

    public removeStatusEffect(effect: string): boolean {
        const index = this.statusEffects.indexOf(effect)
        if (index < 0) {
            return false;
        } else {
            this.statusEffects.splice(index, 1);
            return true;
        }
    }

    public getStatusEffects(): string[] {
        return this.statusEffects;
    }

    public clearStatusEffects(): void {
        this.statusEffects = [];
    }

    public setElement(element: Element): DamageableEntity {
        this.element = element;
        return this;
    }

    public getElement(): Element {
        return this.element;
    }

    public setLocation(location: string): void {
        if (["north", "center", "south"].includes(location)) {
            this.location = location
        } else {
            console.warn(`Invalid location "${location}" for entity. Must be "north", "center", or "south".`);
        }
    }

    public getLocation(): string {
        return this.location;
    }

    public setFacing(direction: string): void {
        if (["north", "south"].includes(direction)) {
            this.facing = direction;
        }
    }

    public isFacing(direction: string): boolean {
        return this.facing === direction;
    }

    public getFacing(): string {
        return this.facing;
    }
}