import { Entity } from "./entity";
import { Element, Proficiency } from "../proficiency-elements";

import { effectType } from "../battleFunctions";

export default class Skill {
    private id: string;
    private static nextId = 0;
    private name: string;
    private proficiency: Proficiency;
    private element: Element;
    private cost: number;
    private effect: effectType;
    private level: number;
    private isLearnable: ((target: Entity) => boolean) | undefined;

    constructor(name: string, proficiency: Proficiency, element: Element, cost: number, effect: effectType, level: number, isLearnable?: (target: Entity) => boolean) {
        this.id = this.generateId(name, proficiency, element)
        this.name = name;
        this.proficiency = proficiency;
        this.element = element;
        this.cost = cost;
        this.effect = effect;
        this.level = level
        this.isLearnable = isLearnable;
    }

    private generateId(name: string, proficiency: string, element: string): string {
        return `${name.substring(0, 2)}-${proficiency.substring(0, 2)}-${element.substring(0, 3)}-${Skill.nextId++}`.toLowerCase();
    }

    public getId(): string {
        return this.id;
    }

    public getName(): string {
        return this.name;
    }

    public getProficiency(): Proficiency {
        return this.proficiency;
    }

    public getElement(): Element {
        return this.element;
    }

    public getManaCost(): number {
        return this.cost;
    }

    public getEffect(): effectType {
        return this.effect;
    }

    public getLevel(): number {
        return this.level;
    }

    public canLearn(target: Entity): boolean {
        return this.isLearnable?.(target) ??  true;
    }
}