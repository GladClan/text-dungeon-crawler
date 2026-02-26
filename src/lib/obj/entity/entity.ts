import { DamageType } from "@/lib/proficiency-elements";
import DamageableEntity from "./damageableEntity";
import { EntityInventory } from "./entityInventory";
import { EntitySkills } from "./entitySkills";
import { EntityAI } from "@/lib/entityAI";

export class Entity extends DamageableEntity {
    private inventory: EntityInventory;
    private skills: EntitySkills;
    private isHidden: boolean = false; // Whether the entity is hidden or not
    private speed: number = 10; // Used to determine turn order
    private AI: EntityAI | null = null; // Logic object for entity, if applicable
    
    constructor(name: string, type: string, resistances?: Partial<Record<DamageType, number>>, id?: string) {
        super(name, type, {...resistances}, id);
        this.inventory = new EntityInventory;
        this.skills = new EntitySkills;
    }

    public getSkills(): EntitySkills {
        return this.skills;
    }

    public getInventory(): EntityInventory {
        return this.inventory;
    }

    public setInventory(inventory: EntityInventory): EntityInventory {
        const old = this.inventory;
        this.inventory = inventory;
        return old;
    }

    public getIsHidden(): boolean {
        return this.isHidden;
    }

    public hide(): boolean {
        if (this.isHidden) { 
            return false;
        } else {
            this.isHidden = true;
            return true;
        }
    }

    public reveal(): boolean {
        if (this.isHidden) {
            this.isHidden = false;
            return true;
        } else {
            return false;
        }
    }

    public getSpeed(): number {
        return this.speed;
    }

    public setSpeed(speed: number) {
        this.speed = speed;
    }

    public hasAI(): boolean {
        if (this.AI) {
            return true;
        } else {
            return false;
        }
    }

    public getAI(): EntityAI | null {
        return this.AI;
    }

    public setAI(ai: EntityAI): EntityAI | null {
        const old = this.AI;
        this.AI = ai;
        return old;
    }

    public clone(): Entity {
        const copy = new Entity(
            this.getName(),
            this.getEntityType(),
            {...this.getAllResistances()},
        )
        copy.setStats(this.getStats());
        copy.setInventory(new EntityInventory(this.getInventory().getItems(), this.getInventory().getGold()));
        copy.setSpeed(this.getSpeed());
        copy.getSkills().addSkills(this.getSkills().allSkills());
        if (this.hasAI()) {
            copy.setAI(this.getAI()!);
        }
        return copy;
    }

    public shadow(): Entity {
        return this;
    }
}