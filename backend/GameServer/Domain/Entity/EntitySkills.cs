using GameServer.Domain.Skills;

namespace GameServer.Domain.Entities;

public class EntitySkills(List<Skill> skills)
{
    public EntitySkills() : this([]) { }
    public List<Skill> Skills { get; } = [.. skills ?? []];

    public bool HasSkill(Skill skill)
    {
        if (Skills.Exists(s => s.Id == skill.Id))
        {
            return true;
        }
        return false;
    }
    public EntitySkills Clone()
    {
        return new EntitySkills([.. skills]);
    }
}