import { effectType } from "../battleFunctions";
import { DamageType } from "../proficiency-elements";
import { Entity } from "./entity/entity";

export class BattleLog {
    private entries: BattleEntry[] = [];

    public addEntry(
        text: string,
        source: Entity,
        target: Entity,
        actionType: ActionType,
        damageType: DamageType,
        sent: number,
        received: number,
        magic: boolean,
        fatal: boolean
    ) {
        this.entries.push(
            new BattleEntry(
                text,
                source,
                target,
                actionType,
                damageType,
                sent,
                received,
                magic,
                fatal
            )
        );
    }

    public getEntryById(id: string): BattleEntry | undefined {
        return this.entries.find((entry) => entry.getId() === id);
    }

    public getEntriesBySource(source: Entity): BattleEntry[] {
        return this.entries.filter((entry) => entry.getSource().getEntityId() === source.getEntityId());
    }

    public getHighestSingleDamage(): Entity | undefined {
        let bestEntry: BattleEntry | undefined;
        this.entries.forEach((entry) => {
            if (!bestEntry || entry.getDamageDealt() > bestEntry.getDamageDealt()) {
                bestEntry = entry;
            }
        });
        return bestEntry?.getSource();
    }

    public getMostDamage(): Entity | undefined {
        const freqMap = new Map<string, number>;
        const entityById = new Map<string, Entity>;
        let max = 0;
        let highestEnt: Entity | undefined;
        for (const log of this.entries) {
            const source = log.getSource();
            const sourceId = source.getEntityId();
            entityById.set(sourceId, source);

            const count = (freqMap.get(sourceId) || 0) + log.getDamageDealt();
            freqMap.set(sourceId, count);

            if (count > max) {
                max = count;
                highestEnt = entityById.get(sourceId);
            }
        }

        return highestEnt;
    }

    public getMostHealer(): Entity | undefined {
        const frequencyMap = new Map<string, number>;
        const entityById = new Map<string, Entity>;
        let max = 0;
        let highestEnt: Entity | undefined;
        for (const log of this.entries) {
            if (log.getActionType() == ActionType.healing) {
                const source = log.getSource();
                const sourceId = source.getEntityId();
                entityById.set(sourceId, source);

                const count = (frequencyMap.get(sourceId) || 0) + 1;
                frequencyMap.set(sourceId, count);

                if (count > max) {
                    max = count;
                    highestEnt = entityById.get(sourceId);
                }
            }
        }
        return highestEnt;
    }

    public getFirstWasFatal(): Entity | undefined {
        return this.entries.find((source) => source.wasFatal())?.getSource();
    }

    public getMagicUser(): Entity | undefined {
        return this.entries.find((source) => source.usedMagic())?.getSource();
    }

    public getUsedMagicMost(): Entity | undefined {
        const frequencyMap = new Map<string, number>;
        const entityById = new Map<string, Entity>;
        let max = 0;
        let highestEnt: Entity | undefined;
        for (const log of this.entries) {
            if (log.usedMagic()) {
                const source = log.getSource();
                const sourceId = source.getEntityId();
                entityById.set(sourceId, source);

                const count = (frequencyMap.get(sourceId) || 0) + 1;
                frequencyMap.set(sourceId, count);

                if (count > max) {
                    max = count;
                    highestEnt = entityById.get(sourceId);
                }
            }
        }
        return highestEnt;
    }

    public getRecentActions(entity: Entity): effectType[] | undefined {
        return undefined;
    }
}

export class BattleEntry{
    private text: string;
    private source: Entity;
    private target: Entity;
    private actionType: ActionType;
    private element: DamageType;
    private sent: number;
    private received: number;
    private fatal: boolean;
    private magic: boolean;
    private id: string;
    static nextId = 0;

    constructor(text: string, source: Entity, target: Entity, actionType: ActionType, element: DamageType, sent: number, received: number, magic: boolean, fatal: boolean) {
        this.text = text;
        this.source = source;
        this.target = target;
        this.actionType = actionType;
        this.element = element;
        this.sent = sent;
        this.received = received;
        this.fatal = fatal;
        this.magic = magic;
        this.id = this.generateId();
    }

    private generateId(): string {
        return `entry-${BattleEntry.nextId++}`
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

    public getActionType(): ActionType {
        return this.actionType;
    }

    public getElement(): DamageType {
        return this.element;
    }

    public getDamageSent(): number {
        return this.sent;
    }

    public getDamageDealt(): number {
        return this.received;
    }

    public usedMagic(): boolean {
        return this.magic;
    }

    public wasFatal(): boolean {
        return this.fatal;
    }

    public getId(): string {
        return this.id;
    }
}

export enum ActionType {
    healing = 'healing',
    attack = 'attack',
    defending = 'defending',
    flee = 'flee',
    wait = 'waiting'
}