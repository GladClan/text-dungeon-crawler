using GameServer.Domain.Skills.SkillsLibrary.InitialRelease;

namespace GameServer.Domain.Skills.SkillsLibrary;

public class SkillsIndexInitialRelease : ISkillsIndex
{
    public Skill GetSkillById(string id)
    {
        return id switch
        {
            _ => new ErrorSkill(),
        };
    }

    public Skill GetSkillByTag(string tag)
    {
        return tag switch
        {
            _ => new ErrorSkill(),
        };
    }
}