import { Element } from "../proficiency-elements";
import { Entity } from "./entity/entity";

export class BattleLog {
    private entries: BattleEntry[] = [];

    public addEntry(text: string) {
        this.entries.push(new BattleEntry(text));
    }

    public getEntryById(id: string): BattleEntry | undefined {
        this.entries.find((entry) => entry.getId() === id);
    }
}

export class BattleEntry{
    private text: string;
    private source: Entity;
    private target: Entity;
    private actionType: string;
    private element: Element;
    private sent: number;
    private received: number;
    private fatal: boolean;
    private id: string;
    static nextId = 0;

    constructor(text: string, source: Entity, target: Entity, actionType: string, element: Element, sent: number, received: number, fatal: boolean) {
        this.text = text;
        this.source = source;
        this.target = target;
        this.actionType = actionType;
        this.element = element;
        this.sent = sent;
        this.received = received;
        this.fatal = fatal;
        this.id = this.generateId();
    }

    private generateId(): string {
        return `entry${BattleEntry.nextId++}`
    }

    public getText(): string {
        return this.text;
    }

    public setText(text: string): string {
        const old = this.text;
        this.text = text;
        return old;
    }

    public getSource(): Entity {
        return this.source;
    }

    public getTarget(): Entity {
        return this.target;
    }

    public getActionType(): string {
        return this.actionType;
    }

    public getElement(): Element {
        return this.element;
    }

    public getDamageSent(): number {
        return this.sent;
    }

    public getDamageDealt(): number {
        return this.received;
    }

    public wasFatal(): boolean {
        return this.fatal;
    }

    public getId(): string {
        return this.id;
    }
}