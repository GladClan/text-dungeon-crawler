import { ContinuousEffects } from "./obj/ContinuousEffects";
import { Entity } from "./obj/entity/entity";

/* Damage sent vs received to target, damage sent vs received to self, damage type, stat affected */
export type returnType = [[number, number], [number, number], [string, string]];
export type returnTypeSingleton = [number, number, [string, string]];
export type effectType = (target: Entity, source: Entity, damageEffects: ContinuousEffects) => returnType;
export type reducedEffectType = (target: Entity, source: Entity) => returnTypeSingleton;

export function getActionText(target: Entity, source: Entity, action: string, resultsArr: returnType): string {
    const memberName = source.getMetadata().getName();
    const actionText: string[] = [];
    if (resultsArr[2][0] === 'defend') {
        if (source.getStats().getState() === 'defending') {
            return(`${memberName} continues to defend.`);
        }
        source.getStats().setState('defending');
        source.addArmorMultiplier('defending', 0.5);
        return(`${memberName} enters a defensive stance!`);
    }
    if (source.getStats().getState() === 'defending') {
        actionText.push(` leaves their defensive stance and`);
        source.removeArmorMultiplier('defending');
        source.getStats().setState('ready');
    }
    const targetName = target.getMetadata().getName();
    if (resultsArr[2][0] == "steal") {// Specific logic for if the character used the 'steal' ability.
        if (resultsArr[0][0] === 0 && resultsArr[0][1] === 0) {
            actionText.push(` failed to steal`);
        } else if (resultsArr[0][0] > 0) {
            actionText.push(` stole ${resultsArr[0][0]} gold`);
            if (resultsArr[0][1] > 0) {
                actionText.push(' and also');
            }
        }
        if (resultsArr[0][1] == 1) {
            actionText.push(` stole an item from ${targetName}!`);
        } else if (resultsArr[0][1] === 2) {
            actionText.push(` tried to steal an item, but ${targetName} has no items!`);
        }
    } else {
        actionText.push(action)
        if (resultsArr[0][1] > 0) {
            actionText.push(`. ${targetName} lost ${resultsArr[0][1].toPrecision(2)} ${resultsArr[2][1]}.`);
        } else if (resultsArr[0][1] < 0) {
            actionText.push(`. ${targetName} gained ${-resultsArr[0][1].toPrecision(2)} ${resultsArr[2][1]}.`);
        } else {
            actionText.push(`, but it had no efect!`);
        };
    }
    if (resultsArr[1][0] && resultsArr[1][0] != 0) {
        if (resultsArr[1][1] > 0) {
            actionText.push(`${memberName} lost ${resultsArr[1][1]} ${resultsArr[2][1]} in return!`)
        } else {
            actionText.push(`${memberName} gained ${-resultsArr[1][1]} ${resultsArr[2][1]} in return!`)
        }
    }
    return memberName.concat(...actionText);
}

export function getEffectText(target: Entity, resultsArr: returnTypeSingleton): string {
    const targetName = target.getMetadata().getName();
    if (resultsArr[1] > 0) {
        return `${targetName} lost ${resultsArr[1].toPrecision(2)} ${resultsArr[2][1]} from `;
    } else if (resultsArr[1] < 0) {
        return `${targetName} gained ${-resultsArr[1].toPrecision(2)} ${resultsArr[2][1]} from `;
    } else {
        return `${targetName} was unaffected by `;
    };
}

export function getTurnOrder(members: Entity[]): Entity[] {
    const sortedMembers = members.sort((a, b) => b.getSpeed() - a.getSpeed());

    const turnCounts = sortedMembers.map(member => ({
        member,
        turns: 1 + Math.floor(member.getSpeed() / 20),
    }));

    const totalTurns = turnCounts.reduce((sum, tc) => sum + tc.turns, 0);
    const turnOrder: Entity[] = [];

    while (turnOrder.length < totalTurns) {
        for (const tc of turnCounts) {
            if (tc.turns > 0) {
                turnOrder.push(tc.member);
                tc.turns--;
            }
        }
    }

    return turnOrder;
}

export function isParty(party: Entity[], target: Entity): boolean {
    return party.some(member => member.getEntityId() === target.getEntityId());
}