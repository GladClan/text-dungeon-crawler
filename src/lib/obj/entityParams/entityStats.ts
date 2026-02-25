export class EntityStats {
    private maxHealth: number;
    private health: number;
    private maxMana: number;
    private magic: number;
    private mana: number;
    private strength: number;
    private defense: number;
    private level: number;
    private experience: number;
    private isEntityAlive: boolean;
    private proficiencies: { [key: string]: number } = {}; // Proficiency in skills or equipment
    private proficiencyEntries: { [key: string]: number } = {}; // Skills or equipment types used during a level and number of times that was used. Used to calculate what proficiency to add on level up
    private visible: boolean = false; // Whether the entity's stats are visible in the UI
    private state: string = 'ready';

    constructor(maxHealth: number, maxMana: number, magic: number, strength: number, defense: number, proficiencies: { [key: string]: number } = {}, level?: number, experience?: number) {
        this.maxHealth = maxHealth;
        this.maxMana = maxMana;
        this.magic = magic;
        this.strength = strength;
        this.defense = defense;
        this.proficiencies = proficiencies;
        this.level = level || 1;
        this.experience = experience || 0;
        this.isEntityAlive = true;
        this.health = maxHealth;
        this.mana = maxMana;
    }

    public getStats() {
        return {
            health: this.health,
            maxHealth: this.maxHealth,
            mana: this.mana,
            maxMana: this.maxMana,
            strength: this.strength,
            defense: this.defense,
            magic: this.magic,
            level: this.level,
            experience: this.experience,
            isAlive: this.isEntityAlive
        };
    }

    public getHealth(): number {
        return this.health;
    }

    public getMaxHealth(): number {
        return this.maxHealth;
    }

    public getMana(): number {
        return this.mana;
    }

    public getMaxMana(): number {
        return this.maxMana;
    }

    public getMagic(): number {
        return this.magic;
    }

    public getStrength(): number {
        return this.strength;
    }

    public getDefense(): number {
        return this.defense;
    }

    public getLevel(): number {
        return this.level;
    }

    public getExperience(): number {
        return this.experience;
    }

    public isAlive(): boolean {
        return this.isEntityAlive;
    }

    public setHealth(health: number): EntityStats {
        this.health = Math.min(health, this.maxHealth);
        if (this.health <= 0) {
            this.isEntityAlive = false;
            this.health = 0;
        }
        return this;
    }

    public setMana(mana: number): EntityStats {
        this.mana = Math.min(mana, this.maxMana);
        if (this.mana < 0) {
            this.mana = 0;
        }
        return this;
    }

    public hasProficiency(skill: string): boolean {
        return this.proficiencies.hasOwnProperty(skill);
    }

    public getProficiency(skill: string): number {
        return this.proficiencies[skill] || 0.5;
    }

    public setProficiency(skill: string, value: number): EntityStats {
        this.proficiencies[skill] = value;
        return this;
    }

    public getAllProficiencies(): { [key: string]: number } {
        return this.proficiencies;
    }

    public proficienciesToString(): string {
        return Object.entries(this.proficiencies)
            .map(([skill, value]) => `${skill}: ${value.toFixed(2)}`)
            .join(", ");
    }

    public addExperience(exp: number): number {
        this.experience += exp;
        // Check for level up
        if (this.experience >= this.getExperienceForNextLevel()) {
            return this.levelUp();
        }
        return this.experience;
    }

    public getExperienceForNextLevel(lvl = this.level): number {
        // Example formula for experience needed to level up
        return Math.floor(100 * Math.pow(1.2, lvl));
    }

    private levelUp(): number {
        if (this.experience < this.getExperienceForNextLevel()) {
            throw new Error(`Failed to level up, need ${this.getExperienceForNextLevel() - this.experience} more experience to level up.\n
             Current exp: ${this.experience}\n
             Exp for next level: ${this.getExperienceForNextLevel()}`);
        }
        const leftover = this.experience - this.getExperienceForNextLevel();
        this.level++;

        for (const entry in this.proficiencyEntries) {
            this.proficiencies[entry] = (this.proficiencies[entry] || 1) + (this.proficiencyEntries[entry] / this.getExperienceForNextLevel(this.level - 1));
        }
        this.proficiencyEntries = {}; // Reset proficiency entries on level up

        this.health = this.maxHealth; // Restore health on level up
        this.mana = this.maxMana; // Restore mana on level up
        this.experience = leftover;
        if (this.experience > this.getExperienceForNextLevel())
            return this.levelUp();
        return this.experience;
    }

    addProficiencyEntry(entry: string): void {
        if (this.proficiencyEntries[entry]) {
            this.proficiencyEntries[entry] += 5;
        } else {
            this.proficiencyEntries[entry] = 5;
        }
    }

    public setVisible(visible: boolean): EntityStats {
        this.visible = visible;
        return this;
    }

    public isVisible(): boolean {
        return this.visible;
    }

    public getState(): string {
        return this.state;
    }

    public setState(state: string): EntityStats {
        this.state = state;
        return this;
    }

    public clone(): EntityStats {
        const clone = new EntityStats(
            this.maxHealth,
            this.maxMana,
            this.magic,
            this.strength,
            this.defense,
            { ...this.proficiencies },
            this.level,
            this.experience
        )
        // Object.entries(this.proficiencyEntries).forEach(([key, value]) => {
        //     clone.proficiencyEntries[key] = value;
        // });
        clone.proficiencyEntries = { ...this.proficiencyEntries };
        clone.setHealth(this.health);
        clone.setMana(this.mana);
        clone.setVisible(this.visible);
        clone.setState(this.state);
        return clone;
    }
}