namespace GameServer.Domain.Skills;

public interface ISkillsIndex
{
    public Skill GetSkillById(string id);
    public Skill GetSkillByTag(string tag);
}