import { EntityMetadata } from "./entityMetadata";
import { EntityStats } from "./entityStats"
import { BattleLog } from "../battleLog";
import { defaults } from "./entityDefaults";
import { DamageType, Proficiency } from "@/lib/proficiency-elements";

export default class DamageableEntity extends EntityMetadata {
    private stats = new EntityStats(defaults.health, defaults.mana, defaults.magic, defaults.strength, defaults.defense, defaults.proficiencies);
    private resistances: Partial<Record<DamageType, number>>;
    // private armorMultipliers: { [key: string]: number };
    // private strength: number;
    // private strengthMultipliers: { [key: string]: number };
    // private statusEffects: string[];
    // private element: Element;
    constructor(name: string, type: string, resistances?: Partial<Record<DamageType, number>>, id?: string) {
        super(name, type, id);
        this.resistances = resistances || {};
    }

    public getStats(): EntityStats {
        return this.stats;
    }

    public fixStats(health: number, mana: number, magic: number, strength: number, defense: number, proficiencies?: Partial<Record<Proficiency, number>>, level?: number, experience?: number) {
        this.stats = new EntityStats(health, mana, magic, strength, defense, proficiencies, level, experience)
    }

    public setStats(stats: EntityStats): void {
        this.stats = stats;
    }

    public getResistance(key: DamageType): number {
        return this.resistances[key] || 1;
    }

    public getAllResistances(): { [key: string]: number} {
        return this.resistances;
    }

    public setResistance(key: DamageType, amount: number): number {
        this.resistances[key] = amount;
        return this.resistances[key];
    }

    public increaseResistance(key: DamageType, amount: number): number {
        this.resistances[key] = (this.resistances[key] ?? 0) + amount;
        return this.resistances[key];
    }

    public decreaseResistance(key: DamageType, amount: number): number {
        this.resistances[key] = (this.resistances[key] ?? 0) + amount;
        return this.resistances[key];
    }

    public heal(source: DamageableEntity, amount: number, battleLog: BattleLog): void {
        if (!this.getStats().isAlive()) {
            console.warn(`Entity is not alive and cannot be healed.`);
        }
        let actual = (amount * this.getResistance(DamageType.healing)); // Apply healing resistance if exists
        actual = Math.min(amount, this.stats.getMaxHealth() - this.stats.getHealth());
        this.stats.setHealth(this.stats.getHealth() + actual);
    }

    public takeDamage(source: DamageableEntity, amount: number, damageType: DamageType, battleLog: BattleLog): void {
        if (!this.getStats().isAlive()) {
            console.warn(`Entity is not alive and cannot take damage.`);
        }
        let actual = amount / this.getResistance(damageType);
    }

    public expendMana(amount: number): number {
        const stats = this.getStats();
        if (!stats.isAlive()) {
            console.warn(`Entity is not alive and cannot use mana`);
        } if (stats.getMana() < amount) {
            console.warn(`Entity does not have sufficient mana to accomplish the requested action`);
            return stats.getMana();
        }
        stats.setMana(stats.getMana() - amount);
        return stats.getMana();
    }

    public gainMana(amt: number): number {
        const stats = this.getStats();
        if (!stats.isAlive()) {
            console.warn(`Entity is not alive and cannot gain mana`);
        }
        const actual = Math.min(stats.getMaxMana() - stats.getMana(), amt);
        stats.setMana(stats.getMana() + actual)
        return actual;
    }
}