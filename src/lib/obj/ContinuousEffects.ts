import { Entity } from "./entity";
import { reducedEffectType, getEffectText } from "../battleFunctions";

export class ContinuousEffects {
    effects: ContinuousEffect[] = [];
    entityCount: number;
    roundNumber: number;
    turnNumber: number = 0;
    static nextId: number;

    constructor(entityCount?: number, roundNumber?: number) {
        this.entityCount = entityCount || 2;
        this.roundNumber = roundNumber || 0;
    }

    public getEntityCount(): number {
        return this.entityCount;
    }

    public setEntityCount(entityCount: number): void {
        this.entityCount = entityCount;
    }

    public addEffect(name: string, target: Entity, source: Entity, effect: reducedEffectType, interval: number, duration: number): void {
        this.effects.push(new ContinuousEffect(
            name,
            target,
            source,
            effect,
            Math.floor(6 * interval * this.entityCount),
            Math.ceil(duration / 6) + this.roundNumber
        ));
    }

    public processEffects(): string[] {
        this.turnNumber++;
        const result = [''];
        this.effects.forEach(effect => {
            const resultArr = effect.processEffect(this.roundNumber);
            result.push(resultArr[0])
            if (resultArr[1] === true) this.removeEffect(effect.getId())
        })
        if (this.turnNumber >= this.entityCount) {
            this.turnNumber = 0;
            this.nextRound();
        }
        return result;
    }

    public removeEffect(id: number): ContinuousEffect | boolean {
        const effect = this.effects.find(effect => effect.getId() === id);
        if (effect) {
            this.effects = this.effects.filter(effect => effect.getId() !== id);
            return effect;
        }
        return false;
    }

    public nextRound(): number {
        return this.roundNumber++;
    }

    public getRound(): number {
        return this.roundNumber;
    }
}

class ContinuousEffect {
    id: number;
    name: string;
    static nextId = 0;
    target: Entity;
    source: Entity;
    effect: reducedEffectType; // The effect that lasts
    interval: number; // The interval of turns between the effect
    duration: number; // The round effect lasts until
    sinceLastActivated: number; // Turns since last activated
    playOnFightEnd: boolean; // Play effect upon ending fight

    constructor(
        name: string,
        target: Entity,
        source: Entity,
        effect: reducedEffectType,
        interval: number,
        duration: number,
        playOnFightEnd?: boolean
    ) {
        this.id = ContinuousEffect.nextId++;
        this.name = name;
        this.target = target;
        this.source = source;
        this.effect = effect;
        this.interval = interval;
        this.duration = duration;
        this.sinceLastActivated = 0;
        this.playOnFightEnd = playOnFightEnd || false;
    }

    public getId(): number {
        return this.id;
    }

    public getName(): string {
        return this.name;
    }

    public processEffect(round: number): [string, boolean] {
        const result: [string, boolean] = ['', false];
        if (this.sinceLastActivated >= this.interval) {
            result[0] = (getEffectText(this.target, this.effect(this.target, this.source)) + this.name)
            this.sinceLastActivated = 0;
        } else {
            this.sinceLastActivated++;
        }
        if (this.duration <= round || !this.target.getStats().isAlive()) {
            result[1] = true;
        }
        return result;
    }

    public getDuration(): number {
        return this.duration;
    }

    public getInterval(): number {
        return this.interval;
    }

    public getTurnsSinceLastActivation(): number {
        return this.sinceLastActivated;
    }
}