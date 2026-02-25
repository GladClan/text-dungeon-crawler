import { Entity } from "@/lib/obj/entity";

export class EntityAI {
    private self: Entity;
    private defending: boolean = false;

    constructor(entity: Entity) {
        this.self = entity;
    }

    public decideAction(): string {
        const stats = this.self.getStats();
        // Simple AI decision-making process
        if (stats.getHealth() < stats.getMaxHealth() * 0.15 || stats.getHealth() < 10) {
            return "defend";
        }
        return "attack";
    }

    determineAction(opponents: Entity[], allies: Entity[]): string {
        // AI logic to determine the best action based on opponents and allies
        let result = '';
        if (this.defending) {
            this.defending = false; // Reset defending state after action
            this.self.removeArmorMultiplier("defending");
            result += `${this.self.getMetadata().getName()} leaves its defensive stance.\n`;
        }
        const action = this.decideAction();
        if (action === "attack") {
            // Find the weakest opponent to attack
            if (opponents.length === 0) {
                result += "Everyone in your party is dead. Wap wap.";
            } else {
                const target = opponents.reduce((weakest, current) => {
                    return current.getStats().getHealth() < weakest.getStats().getHealth() ? current : weakest;
                });
                const damage = this.self.physicalAttack(target);
                result += `${this.self.getMetadata().getName()} attacks ${target.getMetadata().getName()}, dealing ${damage[0][1].toPrecision(2)} ${damage[2][0]} damage!\n`;
                if (!target.getStats().isAlive()) {
                    result += `\n${target.getMetadata().getName()} falls unconscious. Or perhaps they're dead.\n`;
                }
                return result;
            }
        } else if (action === "defend") {
            this.defending = true;
            this.self.addArmorMultiplier("defending", 2);
            return result + `${this.self.getMetadata().getName()} enters a defensive stance!`;
        }
        return result + `${this.self.getMetadata().getName()} sits idly by.`;
    }
}