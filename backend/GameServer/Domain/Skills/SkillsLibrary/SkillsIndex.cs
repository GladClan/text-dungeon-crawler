using GameServer.Domain.Enums;
using GameServer.Domain.Skills.SkillsLibrary.InitialRelease;

namespace GameServer.Domain.Skills.SkillsLibrary;

public class SkillsIndex : ISkillsIndex
{
    private static readonly List<Skill> SkillCatalog = InitializeSkills();

    private static List<Skill> InitializeSkills()
    {
        var skills = new List<Skill>();
        var skillType = typeof(Skill);
        var assembly = typeof(SkillsIndex).Assembly;

        // Find all concrete types that inherit from Skill in the InitialRelease namespace
        var concreteSkillTypes = assembly.GetTypes()
            .Where(t =>
                t.Namespace == "GameServer.Domain.Skills.SkillsLibrary" &&
                !t.IsAbstract &&
                skillType.IsAssignableFrom(t)
            );

        foreach (var type in concreteSkillTypes)
        {
            try
            {
                if(Activator.CreateInstance(type) is Skill skill)
                {
                    skills.Add(skill);
                }
            }
            catch
            {
                // Skip skills taht can't be instatantiated
            }
        }
        return skills;
    }
    public Skill GetSkillById(string id)
    {
        return SkillCatalog.FirstOrDefault(s => s.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase)) ?? new ErrorSkill();
    }

    public Skill GetSkillByTag(string tag)
    {
        return SkillCatalog.FirstOrDefault(s => s.Tag.Equals(tag, StringComparison.InvariantCultureIgnoreCase)) ?? new ErrorSkill();
    }

    public List<Skill> GetNSkillsByElement(int n, DamageType damageType)
    {
        return [..
            SkillCatalog
                .Where(s => s.Element == damageType)
                .OrderBy(x => Random.Shared.Next())
                .Take(n)
        ];
    }
}