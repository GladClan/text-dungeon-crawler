import { Proficiency } from "@/lib/proficiency-elements";

export const defaults = {
    health: 100,
    mana: 80,
    magic: 20,
    strength: 20,
    defense: 20,
    proficiencies: { [Proficiency.bludgeoning]: 0.85, [Proficiency.potions]: 0.85, [Proficiency.slashing]: 0.65, [Proficiency.healing]: 0.6 },
    //
}