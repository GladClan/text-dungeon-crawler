import { Entity } from "./entity/entity";
import { Equippable } from "./itemCases/equippable";
import { Useable } from "./itemCases/useable";
import { Proficiency, DamageType } from "../proficiency-elements";
import { ContinuousEffects } from "./ContinuousEffects";
import { exampleSkills } from "./exampleSkills";

export const exampleItems = {
    healthPotion: new Useable(
        "item",
        "potion",
        "Health Potion",
        "Restores 50 health.",
        10,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const amt = 50 * source.getStats().getProficiency(Proficiency.potions);
            source.getStats().addProficiencyEntry(Proficiency.potions);
            return target.heal(amt);
        },
        true
    ),
    manaPotion: new Useable(
        "item",
        "potion",
        "Mana Potion",
        "Restores 30 mana.",
        10,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const amt = -30 * source.getStats().getProficiency(Proficiency.potions);
            source.getStats().addProficiencyEntry(Proficiency.potions);
            return target.expendMana(amt)
        },
        true
    ),
    fireScroll: new Useable(
        "item",
        "scroll",
        "Fire Scroll",
        "Casts a fire spell that deals 30 damage.",
        15,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const damage = 30 * source.getStats().getProficiency(Proficiency.spellstrike);
            source.getStats().addProficiencyEntry(Proficiency.spellstrike);
            return target.takeMagicDamage(damage, DamageType.fire, source);
        },
        true
    ),
    iceScroll: new Useable(
        "item",
        "scroll",
        "Ice Scroll",
        "Casts an ice spell that deals 20 damage.",
        15,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const damage = 20 * source.getStats().getProficiency(Proficiency.spellstrike);
            source.getStats().addProficiencyEntry(Proficiency.spellstrike);
            return target.takeMagicDamage(damage, DamageType.ice, source);
        },
        true
    ),
    longsword: new Useable(
        "weapon",
        "sword",
        "Longsword",
        "A sharp longsword that deals 15 damage.",
        50,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const damage = 15 * source.getStats().getProficiency(Proficiency.slashing);
            source.getStats().addProficiencyEntry(Proficiency.slashing);
            return target.takeDamage(damage, Proficiency.slashing, source);
        }
    ),
    shield: new Equippable(
        "armor",
        "shield",
        "Shield",
        "A sturdy shield that provides 10 defense.",
        40,
        2,
        false,
        (target: Entity) => {
            target.addArmorMultiplier("shield", 1);
            return true;
        },
        (target: Entity) => {
            return target.removeArmorMultiplier("shield");
        }
    ),
    leatherArmor: new Equippable(
        "armor",
        "armor",
        "Leather Armor",
        "Light armor that provides 5 defense.",
        30,
        1,
        false,
        (target: Entity) => {
            target.addArmorMultiplier("leather armor", 1);
            return true;
        },
        (target: Entity) => {
            return target.removeArmorMultiplier("leather armor")
        }
    ),
    ringOfStrength: new Equippable(
        "artifact",
        "ring",
        "Ring of Strength",
        "Increases strength by 5.",
        100,
        10,
        false,
        (target: Entity) => {
            target.addStrengthMultiplier("ring of strength", 1.5);
            return true;
        },
        (target: Entity) => {
            target.addStrengthMultiplier("ring of strength", -1.5);
            return true;
        }
    ),
    ringOfBlock: new Equippable(
        "artifact",
        "ring",
        "Ring of Block",
        "Increases defense by 5.",
        100,
        10,
        false,
        (target: Entity) => {
            target.addArmorMultiplier("ring of block", 1.5);
            return true;
        },
        (target: Entity) => {
            target.addArmorMultiplier("ring of block", -1.5);
            return true;
        }
    ),
    greatsword: new Useable(
        "weapon",
        "sword",
        "Greatsword",
        "A truly massive sword that deals 25 damage.",
        80,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            source.getStats().addProficiencyEntry(Proficiency.slashing);
            const damage = 25 * source.getStats().getProficiency(Proficiency.slashing);
            return target.takeDamage(damage, Proficiency.slashing, source)
        },
    ),
    warHammer: new Useable(
        "weapon",
        "hammer",
        "War Hammer",
        "A heavy war hammer that deals 20 damage.",
        70,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            source.getStats().addProficiencyEntry(Proficiency.bludgeoning);
            const damage = 20 * source.getStats().getProficiency(Proficiency.bludgeoning);
            return target.takeDamage(damage, Proficiency.bludgeoning, source)
        }
    ),
    oldSword: new Useable(
        "weapon",
        "sword",
        "Ancient Sword",
        "An ancient sword that is in desperate need of repair.",
        2,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const damage = 5 * source.getStats().getProficiency(Proficiency.slashing);
            source.getStats().addProficiencyEntry(Proficiency.slashing);
            return target.takeDamage(damage, Proficiency.slashing, source);
        }
    ),
    rustySabre: new Useable(
        "weapon",
        "sword",
        "Rusty Sabre",
        "A rusty old sabre.",
        3,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const damage = 10 * source.getStats().getProficiency(Proficiency.slashing);
            source.getStats().addProficiencyEntry(Proficiency.slashing);
            return target.takeDamage(damage, Proficiency.slashing, source);
        }
    ),
    splinteringRapier: new Useable(
        "weapon",
        "sword",
        "Splintering Rapier",
        "An old, damaged rapier that looks like it may break at any time.",
        7,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            source.getStats().addProficiencyEntry(Proficiency.piercing);
            const damage = 15 * source.getStats().getProficiency(Proficiency.piercing);
            let result = target.takeDamage(damage, Proficiency.piercing, source);
            if (Math.random() < 0.2) {
                const splinter = source.damageEffect(4, Proficiency.piercing, source)
                result[1][0] = splinter[0];
                result[1][1] = splinter[1];
            }
            return result;
        }
    ),
    crumblingBow: new Useable(
        "weapon",
        "bow",
        "Crumbling Bow",
        "A bow that is falling apart, but still usable.",
        5,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            source.getStats().addProficiencyEntry(Proficiency.bow);
            const damage = 10 * source.getStats().getProficiency(Proficiency.bow);
            return target.takeDamage(damage, Proficiency.piercing, source)
        }
    ),
    oldStaff: new Useable(
        "weapon",
        "staff",
        "Old Staff",
        "An old staff that has seen better days.",
        4,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            source.getStats().addProficiencyEntry(Proficiency.bludgeoning);
            const damage = 8 * source.getStats().getProficiency(Proficiency.bludgeoning);
            return target.takeDamage(damage, Proficiency.bludgeoning, source)
        }
    ),
    healingScroll: new Useable(
        "item",
        "scroll",
        "Healing Scroll",
        "Restores 50 health to the target.",
        12,
        (target: Entity, source: Entity, damageEffect: ContinuousEffects) => {
            const amt = -50 * source.getStats().getProficiency(Proficiency.healing)
            source.getStats().addProficiencyEntry(Proficiency.healing);
            return target.takeMagicDamage(amt, DamageType.healing, source);
        },
        true
    ),
    allSeeingEye: new Useable(
        "item",
        "artifact",
        "All-Seeing Eye",
        "Description",
        120,
        exampleSkills['libra'].getEffect(),
        false
    )
};