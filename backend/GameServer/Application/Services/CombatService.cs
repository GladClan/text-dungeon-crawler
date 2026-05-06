using System.Diagnostics.CodeAnalysis;
using Gameserver.Contracts.Requests;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;

namespace Gameserver.Application.Services;

public sealed class CombatService(EntityStore entityStore)
{
        private readonly EntityStore _entities = entityStore;
        private readonly double _levelStackMultiplier = 1.2;
        
        private static int GetExperienceForNextLevel(int level)
        {
                // Example formula for experience needed to level up
                return (int)Math.Floor(100 * Math.Pow(1.2, level));
        }

        private bool TryGetEntity(string id, [NotNullWhen(true)] out DamageableEntity? target)
        {
                return _entities.TryGet(id, out target) && target is not null;
        }

        public ProficiencyDto? GetProficiencyMultiplier(string id, string proficiency)
        {
                if (!TryGetEntity(id, out var target))
                {
                        return null;
                }
                if (!Enum.TryParse(proficiency, true, out Proficiency profEnum))
                {
                        return new($"{proficiency} is not a valid proficiency");
                }
                return target.GetProficiencyMultiplier(profEnum);
        }

        public ResistanceDto? GetResistanceMultiplier(string id, string damageType)
        {
                if (!TryGetEntity(id, out var target))
                {
                        return null;
                }
                if (!Enum.TryParse(damageType, true, out DamageType dtEnum))
                {
                        return new($"{damageType} is not a valid damage type");
                }
                return target.GetResistanceMultiplier(dtEnum);
        }

        public DamageResultDto? TakeDamage(DamageRequest request)
        {
                if (!TryGetEntity(request.SourceId, out var source) || !TryGetEntity(request.TargetId, out var target))
                {
                        return null;
                }
                if (!Enum.TryParse(request.DamageType, out DamageType dtEnum))
                {
                        return new DamageResultDto(
                                sent: request.DamageSent,
                                error: $"Invalid damage type: {request.DamageType}"
                        );
                }
                return target.TakeDamage(source, request.DamageSent, dtEnum);
        }

        public HealResultDto? Heal(HealRequest request)
        {
                if (!TryGetEntity(request.SourceId, out var source) || !TryGetEntity(request.TargetId, out var target))
                {
                        return null;
                }
                return target.Heal(source, request.AmountToHeal);
        }

        public ManaChangeDto? ChangeMana(ManaRequest request)
        {
                if (!TryGetEntity(request.TargetId, out var target))
                {
                        return null;
                }
                return target.ChangeMana(request.Amount);
        }

        public LevelUpDto? AddExperience(AddExperienceRequest request)
        {
                if (!TryGetEntity(request.TargetId, out var target))
                {
                        return null;
                }
                target.Experience += request.Amount;
                if (target.Experience >= GetExperienceForNextLevel(target.Level))
                {
                        return LevelUp(target);
                }
                return new();
        }

        private LevelUpDto LevelUp(DamageableEntity target)
        {
                LevelUpDto result = new(target.Level);
                target.Experience -= GetExperienceForNextLevel(target.Level);
                target.Level++;
                int stackValue = 0;

                if (target.Experience > GetExperienceForNextLevel(target.Level))
                {
                        stackValue += LevelUpStack(0, target);
                }

                result.LevelAfter = target.Level;
                result.ProficienciesAtStart = target.Proficiencies.ToStringKeyDictionary();

                foreach (var entry in target.ProficiencyEntries)
                {
                        double increase = stackValue * _levelStackMultiplier * entry.Value / GetExperienceForNextLevel(target.Level - 1);
                        if (target.Proficiencies.TryGetValue(entry.Key, out _))
                        {
                                target.Proficiencies[entry.Key] += increase;
                        }
                        else
                        {
                                target.Proficiencies[entry.Key] = 0.5 + increase;
                        }
                }

                result.ProficienciesAfter = target.Proficiencies.ToStringKeyDictionary();
                target.ProficiencyEntries = [];
                target.CurrentHealth = target.MaxHealth;
                target.CurrentMana = target.MaxMana;
                
                return result;
        }

        private static int LevelUpStack(int stack, DamageableEntity target)
        {
                if (target.Experience < GetExperienceForNextLevel(target.Level))
                {
                        return stack;
                }
                target.Experience -= GetExperienceForNextLevel(target.Level);
                target.Level++;
                return LevelUpStack(stack + 1, target);
        }

        public StringDoubleDto? AddProficiencyEntry(AddProficiencyEntryRequest request, int amount = 1)
        {
                if (!TryGetEntity(request.TargetId, out var target))
                {
                        return null;
                }
                if (!Enum.TryParse(request.Proficiency, out Proficiency profEnum))
                {
                        return new(
                                string.Empty,
                                0,
                                $"Cannot convert {request.Proficiency} to Proficiency enum."
                        );
                }
                return target.AddProficiencyEntry(profEnum, amount);
        }

        public double GetProficiency(DamageableEntity target, Proficiency proficiency)
        {
                if (target.ProficiencyEntries.TryGetValue(proficiency, out _))
                {
                        return target.Proficiencies[proficiency];
                }
                return 0.5d;
        }

        public string? GetDeathMessage(string id)
        {
                if (!TryGetEntity(id, out var target))
                {
                        return null;
                }
                return target.DeathMessage;
        }

        public ManaChangeDto? SetCurrentMana(ManaRequest request)
        {
                if (!TryGetEntity(request.TargetId, out var target))
                {
                        return null;
                }
                if (request.Amount > target.MaxMana || request.Amount < 0)
                {
                        return new(
                                amountSent: request.Amount,
                                error: $"Invalid value: {request.Amount}. Amount must be within the mana capacity of {target.Name}: 0 - {target.MaxMana}"
                        );
                }
                target.CurrentMana = request.Amount;
                return new(
                        amountSent: request.Amount,
                        amountActual: request.Amount,
                        newMana: target.CurrentMana
                );
        }
}

// Maybe some of these methods should go into an EntityStatsService file? Such as that last method--SetCurrentMana

