import { Entity } from "@/lib/obj/entity/entity"
import { EntityInventory } from "@/lib/obj/entity/entityInventory";
import { exampleItems } from "@/lib/obj/exampleItems";

const skelly = new Entity("skeleton Warrior", "monster");
    skelly.fixStats(25, 0, 0, 40, 50, {bludgeoning: 1.2, slashing: 1.1}, 0, 100);
    skelly.fixResistances({ ice: 0.5, poison: 1, lightning: 0.2, healing: 1.75, necrotic: 2});
    skelly.setInventory(new EntityInventory().setGold(7).addItem(exampleItems.rustySabre));
    skelly.setSpeed(16);
const skellyShooty = new Entity("Skeleton Archer", "monster");
    skellyShooty.fixStats(20, 0, 0, 30, 20, {"piercing": 1}, 0, 80);
    skellyShooty.fixResistances({ ice: 0.5, poison: 1, lightning: 0.2, healing: 1.75, necrotic: 2});
    skellyShooty.setInventory(new EntityInventory().setGold(5).addItem(exampleItems.crumblingBow));
const skellyMage = new Entity("Skeleton Mage", "monster");
    skellyMage.fixStats(18, 60, 25, 5, 10, { spellstrike: 1.7, bludgeoning: 0.8}, 0, 90);
    skellyMage.fixResistances({ ice: 0.5, poison: 1, lightning: 0.25, healing: 1.75, necrotic: 2 });
    skellyMage.setInventory(new EntityInventory().setGold(10).addItem(exampleItems.oldStaff).addItem(exampleItems.fireScroll).addItem(exampleItems.iceScroll));

export const first = [
    skelly.clone(),
    skelly.clone(),
    skellyShooty.clone(),
    skellyShooty.clone(),
    skellyMage.clone()
]