import { EntityMetadata } from "../entityParams/entityMetadata";
import { EntityStats } from "./entityStats"
import { BattleLog } from "../battleLog";
import { defaults } from "./entityDefaults";
import { Element, Proficiency } from "@/lib/proficiency-elements";

export default class DamageableEntity extends EntityMetadata {
    private stats = new EntityStats(defaults.health, defaults.mana, defaults.magic, defaults.strength, defaults.defense, defaults.proficiencies);
    private resistances: { [key: string]: number };
    // private armorMultipliers: { [key: string]: number };
    // private strength: number;
    // private strengthMultipliers: { [key: string]: number };
    // private statusEffects: string[];
    // private element: Element;
    constructor(name: string, type: string, resistances?: {[key in Element]?: number}, id?: string) {
        super(name, type, id);
        this.resistances = resistances || {};
    }

    public getStats(): EntityStats {
        return this.stats;
    }

    public fixStats(health: number, mana: number, magic: number, strength: number, defense: number, proficiencies: {[key in Proficiency]?: number}, level?: number, experience?: number) {
        this.stats = new EntityStats(health, mana, magic, strength, defense, proficiencies, level, experience)
    }

    public setStats(stats: EntityStats): void {
        this.stats = stats;
    }

    public getResistance(key: Element): number {
        return this.resistances[key] || 1;
    }

    public heal(source: DamageableEntity, amount: number, battleLog: BattleLog): void {
        const actual = Math.min(amount, this.stats.getMaxHealth() - this.stats.getHealth());
        this.stats.setHealth(this.stats.getHealth() + actual);
    }

    public takeDamage(source: DamageableEntity, amount: number, element: Element, battleLog: BattleLog): void {
        let actual = amount / this.getResistance(element);
    }
}