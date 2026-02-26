import { Entity } from "./obj/entity/entity";
import { exampleItems } from "./obj/exampleItems";
import { exampleSkills } from "./obj/exampleSkills";
import { DamageType, Proficiency } from "./proficiency-elements";

const errRes: Partial<Record<DamageType, number>> = Object.values(DamageType).reduce((acc, damageType) => {
    acc[damageType] = 0.5;
    return acc;
}, {} as Partial<Record<DamageType, number>>);

const errorMonster = new Entity("m̴̪̟̒͆y̸̑͜s̸͚̜̘̲̋̋̀̕͜͜͝t̴̲̞͕̤͂͝e̵̺̤͂̑͋̀́̅r̷̟̳̼̪͛ỉ̵͙͎̟̮͎̐̊̒̈́̂ͅo̵̪͕͑̿̈̇̄ṳ̴̗̰̱̣̣͑s̸̩̠̩͐̔͝ ̴̢̒ç̴̠̏̂̇̇̾o̶̖̙͔͕͍͕̓r̶̗͆̽̍̂̓͝r̶̦̭̗̄̆̆͛̔͠u̶̬̗̮̺̪̫͂̀̋̚p̵̗̆̉t̷̤͉͍̞̫̪̂i̸̦͎̭̬̊̈́̐̅͝ͅõ̴̡̫̥͊n̶̡̡͖͈̍͂̾̔͠", "monster",  {
        ...errRes
    }, "error-retreiving-monsters-from-storage");
    errorMonster.setResistance(DamageType.spellstrike, 1.5);
    errorMonster.fixStats(10000, 10000, 20, 40, 50, {[Proficiency.default]: 1.2}, 0, 10000);

    errorMonster.getStats().setVisible(true);
export default errorMonster;

const defaultParty1 = new Entity("Defaulto", "player");
    defaultParty1.fixStats(100, 50, 8, 8, 8, {[Proficiency.slashing]: 1.5, [Proficiency.default]: 1.8});
    defaultParty1.getInventory().setGold(100).addItem(exampleItems.longsword).addItem(exampleItems.healthPotion).addItem(exampleItems.shield).addItem(exampleItems.healingScroll).addItem(exampleItems.healthPotion);
    defaultParty1.getSkills().addSkill(exampleSkills.poison).addSkill(exampleSkills.steal).addSkill(exampleSkills.sacrifice);
    defaultParty1.setSpeed(14);
const defaultParty2 = new Entity("Defaulto magicker", "player");
    defaultParty2.fixStats(85, 100, 12, 4, 4, {[Proficiency.spellstrike]: 1.7, [Proficiency.healing]: 2.9, [Proficiency.bludgeoning]: 0.9});// , 85, 100, 12, 4, 4);
    defaultParty2.getSkills().addSkill(exampleSkills.heal).addSkill(exampleSkills.fireball).addSkill(exampleSkills.blizzard).addSkill(exampleSkills.thunderbolt).addSkill(exampleSkills.greaterHeal);
    defaultParty2.getInventory().addItem(exampleItems.oldStaff).addItem(exampleItems.iceScroll).addItem(exampleItems.fireScroll).addItem(exampleItems.fireScroll).addItem(exampleItems.healthPotion);
    defaultParty2.setSpeed(10);
const defaultParty3 = new Entity("Defaulto tank", "player");
    defaultParty3.fixStats(130, 25, 4, 8, 12, {[Proficiency.bludgeoning]: 1.5, [Proficiency.default]: 1});// , 130, 25, 4, 8, 12);
    defaultParty3.getInventory().setGold(100).addItem(exampleItems.greatsword).addItem(exampleItems.shield).addItem(exampleItems.healthPotion).addItem(exampleItems.iceScroll).addItem(exampleItems.ringOfBlock).addItem(exampleItems.warHammer).addItem(exampleItems.healthPotion);
    defaultParty3.setSpeed(18);
export const defaultParty = [defaultParty1, defaultParty2, defaultParty3];

const quickEnemy1 = new Entity("Slime", "monster");
    quickEnemy1.fixStats(10, 0, 0, 5, 5, {}, 1, 5);
export const toVictory = [quickEnemy1.clone(), quickEnemy1.clone(), quickEnemy1.clone()];

const glassEnemy = new Entity("Glass Cannon", "monster");
    glassEnemy.fixStats(10, 100, 100, 90, 0, {[Proficiency.bludgeoning]: 1.2}, 1, 50);
    glassEnemy.setSpeed(10);
export const quickLevel = [glassEnemy.clone(), glassEnemy.clone(), glassEnemy.clone()];