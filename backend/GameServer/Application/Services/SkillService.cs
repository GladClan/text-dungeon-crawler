using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Domain.Skills;

namespace GameServer.Application.Services;

public sealed class SkillService(EntityStore entityStore, ISkillsIndex skillsIndex)
{
    private readonly EntityStore _entities = entityStore;
    private readonly ISkillsIndex _skillsIndexer = skillsIndex;

    public List<SkillDto>? GetAllSkills(string id)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            return [..target.Skills.Select(s => s.SkillToDto())];
        }
        return null;
    }

    public SkillDto? GetSkillById(string id, string skillId)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            var skill = target.Skills.FirstOrDefault(s => s.Id.Equals(skillId, StringComparison.InvariantCultureIgnoreCase));
            if (skill is null)
            {
                return new SkillDto
                {
                    Error = $"{skillId} not found among the skills of {target.Name} ({target.ID})"
                };
            }
            return skill.SkillToDto();
        }
        return null;
    }

    public SkillDto? GetSkillByName(string id, string name)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            var result = target.Skills.FirstOrDefault(s => s.Name.Equals(name));
            if (result is null)
            {
                return new SkillDto
                {
                    Error = $"{target.Name} does not have any skill called {name}"
                };
            }
            return result.SkillToDto();
        }
        return null;
    }

    public SkillDto? AddSkillById(string id, string skillId)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            Skill result = _skillsIndexer.GetSkillById(skillId);
            if (result.Tag.Equals("error"))
            {
                return new SkillDto
                {
                    Error = $"No skill exists with id: {skillId}"
                };
            }
            if (result.IsLearnable(target))
            {
                target.Skills.Add(result);
                return result.SkillToDto();
            }
            else
            {
                return new SkillDto
                {
                    Error = $"{target.Name} cannot learn {result.Name}"
                };
            }
        }
        return null;
    }

    public SkillDto? AddSkillByTag(string id, string tag)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            Skill result = _skillsIndexer.GetSkillByTag(tag);
            if (result.Tag.Equals("error"))
            {
                return new SkillDto
                {
                    Error = $"No skill exists with tag: {tag}"
                };
            }
            if (result.IsLearnable(target))
            {
                target.Skills.Add(result);
                return result.SkillToDto();
            }
            else
            {
                return new SkillDto
                {
                    Error = $"{target.Name} cannot learn {result.Name}"
                };
            }
        }
        return null;
    }

    public SkillDto? RemoveSkillById(string id, string skillId)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            var result = target.Skills.Find(s => s.Id.Equals(skillId, StringComparison.OrdinalIgnoreCase));
            if (result is null)
            {
                return new SkillDto
                {
                    Error = $"{skillId} not found among the skills of {target.Name} ({target.ID})"
                };
            }
            target.Skills.Remove(result);
            return result.SkillToDto();
        }
        return null;
    }

    public SkillDto? RemoveSkillByName(string id, string name)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            var result = target.Skills.Find(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (result is null)
            {
                return new SkillDto
                {
                    Error = $"{name} not found among the skills of {target.Name} ({target.ID})"
                };
            }
            target.Skills.Remove(result);
            return result.SkillToDto();
        }
        return null;
    }

    public List<SkillDto>? LearnAllSkillsFromSource(string sourceId, string targetId)
    {
        if (
            _entities.TryGet(sourceId, out var source)
            && source is not null
            && _entities.TryGet(targetId, out var target)
            && target is not null
        )
        {
            foreach (Skill s in source.Skills)
            {
                if (!target.Skills.Any(v => v.Tag.Equals(s.Tag, StringComparison.OrdinalIgnoreCase)) && s.IsLearnable(target))
                {
                    target.Skills.Add(s);
                }
            }
        }
        return null;
    }

    public List<SkillDto>? ForgetAllSkills(string id)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            var result = target.Skills;
            target.Skills.Clear();
            return [..result.Select(s => s.SkillToDto())];
        }
        return null;
    }
}

/*
clear
clone
*/