import { DamageableEntity } from "./entityParams/damageableEntity";
import { EntityInventory } from "./entityParams/entityInventory";
import { EntityMetadata } from "./entityParams/entityMetadata";
import { EntitySkills } from "./entityParams/entitySkills";
import { EntityAI } from "../entityAI";
import { Element } from "../proficiency-elements";

export class Entity extends DamageableEntity {
    private metadata: EntityMetadata;
    private inventory: EntityInventory;
    private skills: EntitySkills;
    private initiative = 10; // Default initiative value for turn order
    private isHidden: boolean = false; // Whether the entity is hidden or not
    private AI: EntityAI | null = null; // AI logic for the entity, if applicable

    constructor(
        name: string,
        entityType: string,
        health: number,
        mana: number,
        magic: number,
        strength: number,
        defense: number,
        proficiencies?: { [key: string]: number },
        resistances?: { [key: string]: number },
        element?: Element,
        level?: number,
        experience?: number,
        entityId?: string
    ) {
        super(health, mana, magic, strength, defense, proficiencies, resistances, element, level, experience);
        this.metadata = new EntityMetadata(name, entityType, entityId);
        this.inventory = new EntityInventory();
        this.skills = new EntitySkills();
    }

    public getMetadata(): EntityMetadata {
        return this.metadata;
    }

    public getInventory(): EntityInventory {
        return this.inventory;
    }

    public setInventory(inventory: EntityInventory): Entity {
        this.inventory = inventory;
        return this;
    }

    public getSkills(): EntitySkills {
        return this.skills;
    }

    public setSkills(skills: EntitySkills): Entity {
        this.skills = skills;
        return this;
    }

    public setInitiative(value: number): Entity {
        this.initiative = value;
        return this;
    }

    public getInitiative(): number {
        return this.initiative;
    }

    public setIsHidden(value: boolean): void {
        this.isHidden = value;
    }

    public getIsHidden(): boolean {
        return this.isHidden;
    }

    public setAI(ai: EntityAI): Entity {
        this.AI = ai;
        return this;
    }

    public getAI(): EntityAI | null {
        return this.AI;
    }

    public clone(): Entity {
        const copy = new Entity(
            this.getMetadata().getName(),
            this.getMetadata().getEntityType(),
            this.getStats().getMaxHealth(),
            this.getStats().getMaxMana(),
            this.getStats().getMagic(),
            this.getStats().getStrength(),
            this.getStats().getDefense(),
            this.getStats().getAllProficiencies(),
            this.getResistances(),
            this.getElement(),
            this.getStats().getLevel(),
            this.getStats().getExperience()
        )
        copy.setInventory(this.getInventory().clone())
            .setSkills(this.getSkills().clone())
            .setInitiative(this.getInitiative())
            .getStats().setVisible(this.getStats().isVisible())
                .setHealth(this.getStats().getHealth())
                .setMana(this.getStats().getMana())
        return copy;
    }

    public shadow(): Entity {
        return this;
    }
}