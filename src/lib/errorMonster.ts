import { Entity } from "./obj/entity/entity";
import { exampleItems } from "./obj/exampleItems";
import { exampleSkills } from "./obj/exampleSkills";
import { DamageType, Proficiency } from "./proficiency-elements";

export const errorMonster = new Entity("m̴̪̟̒͆y̸̑͜s̸͚̜̘̲̋̋̀̕͜͜͝t̴̲̞͕̤͂͝e̵̺̤͂̑͋̀́̅r̷̟̳̼̪͛ỉ̵͙͎̟̮͎̐̊̒̈́̂ͅo̵̪͕͑̿̈̇̄ṳ̴̗̰̱̣̣͑s̸̩̠̩͐̔͝ ̴̢̒ç̴̠̏̂̇̇̾o̶̖̙͔͕͍͕̓r̶̗͆̽̍̂̓͝r̶̦̭̗̄̆̆͛̔͠u̶̬̗̮̺̪̫͂̀̋̚p̵̗̆̉t̷̤͉͍̞̫̪̂i̸̦͎̭̬̊̈́̐̅͝ͅõ̴̡̫̥͊n̶̡̡͖͈̍͂̾̔͠", "monster", 10000, 10000, 20, 40, 50, {"hand-to-hand": 1.2}, {spellstrike: 1.5, bludgeoning: 0.5, piercing: 0.5, slashing: 0.5, fire: 0.5, ice: 0.5, poison: 0.5, lightning: 0.5}, DamageType.necro, 0, 100000, "error-retreiving-monsters-from-storage");
errorMonster.getStats().setVisible(true);

const defaultParty1 = new Entity("Defaulto", "player", 100, 50, 8, 8, 8);
    defaultParty1.getInventory().setGold(100).addItem(exampleItems.longsword).addItem(exampleItems.healthPotion).addItem(exampleItems.shield).addItem(exampleItems.healingScroll).addItem(exampleItems.healthPotion);
    defaultParty1.getSkills().addSkill(exampleSkills.poison).addSkill(exampleSkills.steal).addSkill(exampleSkills.sacrifice);
    defaultParty1.getStats().setProficiency("slashing", 1.5).setProficiency("hand-to-hand", 1.8);
    defaultParty1.setInitiative(14);
const defaultParty2 = new Entity("Defaulto magicker", "player", 85, 100, 12, 4, 4);
    defaultParty2.getSkills().addSkill(exampleSkills.heal).addSkill(exampleSkills.fireball).addSkill(exampleSkills.blizzard).addSkill(exampleSkills.thunderbolt).addSkill(exampleSkills.greaterHeal);
    defaultParty2.getInventory().addItem(exampleItems.oldStaff).addItem(exampleItems.iceScroll).addItem(exampleItems.fireScroll).addItem(exampleItems.fireScroll).addItem(exampleItems.healthPotion);
    defaultParty2.getStats().setProficiency("spellstrike", 1.7).setProficiency("healing", 2.9).setProficiency("bludgeoning", 0.9);
    defaultParty2.setInitiative(10);
const defaultParty3 = new Entity("Defaulto tank", "player", 130, 25, 4, 8, 12);
    defaultParty3.getInventory().setGold(100).addItem(exampleItems.greatsword).addItem(exampleItems.shield).addItem(exampleItems.healthPotion).addItem(exampleItems.iceScroll).addItem(exampleItems.ringOfBlock).addItem(exampleItems.warHammer).addItem(exampleItems.healthPotion);
    defaultParty3.getStats().setProficiency("bludgeoning", 1.5).setProficiency("hand-to-hand", 1);
    defaultParty3.setInitiative(18);
export const defaultParty = [defaultParty1, defaultParty2, defaultParty3];

const quickEnemy1 = new Entity("Slime", "monster", 10, 0, 0, 5, 5, {}, {}, DamageType.none, 1, 5);
export const toVictory = [quickEnemy1.clone(), quickEnemy1.clone(), quickEnemy1.clone()];

const glassEnemy = new Entity("Glass Cannon", "monster", 10, 100, 100, 90, 0, {[Proficiency.bludgeoning]: 1.2}, {}, DamageType.none, 1, 50);
    glassEnemy.setInitiative(10);
export const quickLevel = [glassEnemy.clone(), glassEnemy.clone(), glassEnemy.clone()];