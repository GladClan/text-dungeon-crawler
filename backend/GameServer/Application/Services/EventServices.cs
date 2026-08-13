using GameServer.Contracts.DTOs;
using GameServer.Domain.Enums;
using GameServer.Domain.Statistics;
using GameServer.Infrastructure;

namespace GameServer.Application.Services;

public sealed class EventServices(EntityStore entityStore)
{
    private readonly EntityStore _entities = entityStore;

    public ChallengeResultDto? ChallengeEntitySkill(string id, double value, Proficiency proficiency)
    {
        if (_entities.TryGet(id, out var target) && target is not null)
        {
            Random random = new();
            double chance = random.NextDouble();
            double result = target.GetProficiencyMultiplier(proficiency).Value * chance;
            return new ChallengeResultDto
            {
                Success = result > value,
                Margin = value - result
            };
        }
        return null;
    }
}