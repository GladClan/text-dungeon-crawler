/*
heal
takeDamage
expendMana
gainMana
getStats
getMaxHealth
setMaxHealth
getHealth
setHealth
getMaxMana
setMaxMana
getMana
setMana
getMagic
setMagic
getStrength
setStrength
getDefense
setDefense
getLevel
setLevel
getExperience
setExperience
addExperience
getExperienceForNextLevel
levelUp
        // if (this.experience < this.getExperienceForNextLevel()) {
        //     throw new Error(`Failed to level up: need ${this.getExperienceForNextLevel() - this.experience} more experience to level up.\n
        //      Current exp: ${this.experience}\n
        //      Exp for next level: ${this.getExperienceForNextLevel()}`);
        // }
        // const leftover = this.experience - this.getExperienceForNextLevel();
        // this.level++;

        // for (const entry of Object.keys(this.proficiencyEntries) as Proficiency[]) {
        // const gained = this.proficiencyEntries[entry] ?? 0;
        // this.proficiencies[entry] =
        //     (this.proficiencies[entry] ?? 0.5) +
        //     (gained / this.getExperienceForNextLevel(this.level - 1));
        // }

        // this.proficiencyEntries = {}; // Reset proficiency entries on level up
        // this.health = this.maxHealth; // Restore health on level up
        // this.mana = this.maxMana; // Restore mana on level up
        // this.experience = leftover;
        // if (this.experience > this.getExperienceForNextLevel())
        //     return this.levelUp();
        // return this.experience;
isAlive
hasProficiency
getProficiency
setProficiency
getAllProficiencies
proficienciesToString
addProficiencyEntry
setVisible
isVisible
getState
setState
*/