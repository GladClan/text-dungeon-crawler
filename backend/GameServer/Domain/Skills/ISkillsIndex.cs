using GameServer.Domain.Enums;

namespace GameServer.Domain.Skills;

public interface ISkillsIndex
{
    public Skill GetSkillById(string id);
    public Skill GetSkillByTag(string tag);
    public List<Skill> GetNSkillsByElement(int n, DamageType damageType);
}