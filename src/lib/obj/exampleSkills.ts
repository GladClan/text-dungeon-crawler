import { Entity } from "./entity";
import Skill from "./skill";
import { Proficiency, Element } from "../proficiency-elements";
import { ContinuousEffects } from "./ContinuousEffects";

// skills: {[key: string]: { proficiency: string, skill: Function } };
export const exampleSkills = {
    heal: new Skill(
        "Heal",
        Proficiency.healing,
        Element.healing,
        7,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const damage = -10 * source.getStats().getProficiency(Proficiency.healing);
            source.getStats().addProficiencyEntry(Proficiency.healing);
            return target.takeMagicDamage(damage, Element.healing, source);
        },
        1
    ),

    greaterHeal: new Skill(
        "Greater Heal",
        Proficiency.healing,
        Element.healing,
        15,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const damage = -30 * source.getStats().getProficiency(Proficiency.healing);
            source.getStats().addProficiencyEntry(Proficiency.healing);
            return target.takeMagicDamage(damage, Element.healing, source);
        },
        2
    ),

    fireball : new Skill(
        "Fireball",
        Proficiency.spellstrike,
        Element.fire,
        7,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const damage = 20 * source.getStats().getProficiency(Proficiency.spellstrike);
            source.getStats().addProficiencyEntry(Proficiency.spellstrike);
            return target.takeMagicDamage(damage, Element.fire, source);
        },
        1
    ),

    thunderbolt : new Skill(
        "Thunderbolt",
        Proficiency.spellstrike,
        Element.lightning,
        7,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const damage = 20 * source.getStats().getProficiency(Proficiency.spellstrike);
            source.getStats().addProficiencyEntry(Proficiency.spellstrike);
            return target.takeMagicDamage(damage, Element.lightning, source);
        },
        1
    ),

    blizzard : new Skill(
        "Blizzard",
        Proficiency.spellstrike,
        Element.ice,
        9,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const damage = 5 * source.getStats().getProficiency(Proficiency.spellstrike);
            source.getStats().addProficiencyEntry(Proficiency.spellstrike);
            damageEffect.addEffect(
                'Blizzard',
                target,
                source,
                (target: Entity, source: Entity) => {return target.damageEffect(damage, Element.ice, source)},
                0.5,
                6
            )
            const result = target.damageEffect(damage, Element.ice, source);
            return [[result[0], result[1]], [0,0], result[2]]
        },
        1
    ),

    poison: new Skill(
        "Poison",
        Proficiency.spellstrike,
        Element.poison,
        7,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const damage = 3 * source.getStats().getProficiency(Proficiency.spellstrike);
            source.getStats().addProficiencyEntry(Proficiency.spellstrike);
            damageEffect.addEffect(
                'Poison',
                target,
                source,
                (target: Entity, source: Entity) => {return target.damageEffect(damage, Element.poison, source)},
                0.125,
                30
            )
            const result = target.damageEffect(damage, Element.poison, source);
            return [[result[0], result[1]], [0,0], result[2]]
        },
        1
    ),

    steal: new Skill(
        "Steal",
        Proficiency.stealth,
        Element.none,
        5,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            source.getStats().addProficiencyEntry(Proficiency.stealth);
            const result: [[number, number], [number, number], [string, string]] = [[0, 0], [0,0], ['steal', 'steal']];
            // Attempt to steal gold
            if (Math.random() < 0.5 * source.getStats().getProficiency(Proficiency.stealth)) {
                result[0][0] = Math.min(Math.floor(Math.random() * 10 * source.getStats().getProficiency(Proficiency.stealth)), target.getInventory().getGold());
            }
            target.getInventory().setGold((target.getInventory().getGold() - result[0][0]));
            source.getInventory().setGold(source.getInventory().getGold() + result[0][0])
            // Attempt to steal an item
            if (Math.random() < 0.1 * source.getStats().getProficiency(Proficiency.stealth)) {
                result[0][1] = 1;
            }
            if (result[0][1] === 1) {
                if (target.getInventory().isEmpty()) {
                    result[0][1] = 2;
                }
                else {
                    const item = target.getInventory().removeItemByIndex(Math.floor(Math.random() * target.getInventory().size()));
                    source.getInventory().addItem(item!);
                }
            }
            return result;
        },
        1
    ),

    sacrifice: new Skill(
        'Noble Sacrifice',
        Proficiency.nobility,
        Element.holy,
        20,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const max = source.getStats().getMaxHealth()
            const current = source.getStats().getHealth()
            const multiplier = max / current;
            const damage = max * multiplier * source.getStats().getProficiency(Proficiency.nobility);
            source.takeMagicDamage(damage, Element.holy, target);
            return target.takeMagicDamage(damage, Element.holy, source);
        },
        2
    ),

    //Also make a group libra spell that targets all enemies.
    libra: new Skill(
        'Libra',
        Proficiency.stealth,
        Element.light,
        5,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            if (target.getStats().isVisible()) {
                const skills = target.getSkills().allSkills();
                const toLearn = skills[Math.min(Math.random() * skills.length)];
                // if (target.getSkills().allSkills()[len].canLearn(source) {
                //     source.getSkills().addSkill()
                // })
                if (toLearn.canLearn(source) && Math.random() * source.getStats().getProficiency(Proficiency.stealth) > 0.7) {
                    if (!source.getSkills().hasSkill(toLearn))
                    source.getSkills().addSkill(toLearn);
                }
            }
            target.getStats().setVisible(true);
            return [[0,0],[0,0],['libra', 'libra']]
        },
        1
    )
};