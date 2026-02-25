import Skill from '../skill'

export class EntitySkills {
    private skills: Skill[]

    constructor(skills?: Skill[]) {
        this.skills = skills || [];
    }

    public hasSkill(skill: Skill): boolean {
        if (this.skills.find(sk => sk.getId() === skill.getId()) || this.skills.find(sk => sk.getName() === skill.getName())) {
            return true;
        }
        return false;
    }

    public getSkill(name: string): Skill | undefined {
        return this.skills.find(skill => skill.getName() === name);
    }

    public getSkillById(id: string): Skill | undefined {
        return this.skills.find(skill => skill.getId() === id)
    }

    public addSkill(skill: Skill): EntitySkills {
        this.skills.push(skill);
        return this;
    }

    public removeSkill(name: string): Skill | undefined {
        const skill = this.skills.find(skill => skill.getName() === name)
        if (skill) {
            this.skills = this.skills.filter(skill => skill.getName() !== name)
        }
        return skill;
    }

    public removeSkillById(id: string): Skill | undefined {
        const skill = this.skills.find(skill => skill.getId() === id)
        if (skill) {
            this.skills = this.skills.filter(skill => skill.getId() !== id)
        }
        return skill;
    }

    public allSkills(): Skill[] {
        return this.skills;
    }

    public clear(): void {
        this.skills = [];
    }

    public isEmpty(): boolean {
        return this.skills.length === 0;
    }

    public clone(): EntitySkills {
        return new EntitySkills([...this.skills])
    }
}