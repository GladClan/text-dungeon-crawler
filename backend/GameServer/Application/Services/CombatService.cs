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
        private readonly int _defenseConstant = 40;
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

        public ResistanceDto? GetResistanceMultiplier(string id, string damageType)
        {
                if (!TryGetEntity(id, out var target))
                {
                        return null;
                }
                if (!Enum.TryParse(damageType, true, out DamageType dtEnum))
                {
                        return new(string.Empty, 0);
                }
                var result = target.Resistances.TryGetValue(dtEnum, out var value) ? value : 1d;
                if (dtEnum == DamageType.crushing || dtEnum == DamageType.slashing || dtEnum == DamageType.piercing)
                {
                        result += Math.Abs(result) * (target.Defense / _defenseConstant);
                }
                return new(dtEnum.ToString(), result);
        }

        private static bool DidEntityDie(DamageableEntity entity)
        {
                if (entity.CurrentHealth <= 0)
                {
                        entity.CurrentHealth = 0;
                        entity.IsEntityAlive = false;
                        entity.ProficiencyEntries = [];
                        entity.Experience = 0;
                        // Proficiencies decrease?
                        // Level decreases?
                        // Resistance to necro or radiant increases?
                        return true;
                }
                return false;
        }
        
        public DamageResultDto? TakeDamage(DamageRequest request)
        {
                if (!TryGetEntity(request.SourceId, out var source) || !TryGetEntity(request.TargetId, out var target))
                {
                        return null;
                }
                ResistanceDto? resistance = GetResistanceMultiplier(target.ID, request.DamageType);
                if (resistance is null)
                {
                        return new DamageResultDto(
                                sent: request.DamageSent,
                                actual: 0d,
                                error: $"Invalid damage type: {request.DamageType}"
                        );
                }
                if (!source.IsEntityAlive)
                {
                        return new DamageResultDto(
                                sent: request.DamageSent,
                                actual: 0d,
                                error: $"{source.Name} is not alive and cannot deal damage."
                        );
                }
                if (!target.IsEntityAlive)
                {
                        return new DamageResultDto(
                                sent: request.DamageSent,
                                actual: 0d,
                                error: $"{target.Name} is not alive and cannot take damage."
                        );
                }
                double actual = (double)(request.DamageSent * resistance.Value);
                target.CurrentHealth -= actual;
                bool fatal = DidEntityDie(target);
                return new DamageResultDto(
                        sent: request.DamageSent,
                        actual: actual,
                        result: target.CurrentHealth,
                        blocked: request.DamageSent - actual,
                        fatal: fatal
                );
        }

        public HealResultDto? Heal(HealRequest request)
        {
                if (!TryGetEntity(request.SourceId, out var source) || !TryGetEntity(request.TargetId, out var target))
                {
                        return null;
                }
                ResistanceDto? resistance = GetResistanceMultiplier(request.TargetId, DamageType.healing.ToString());
                if (resistance is null)
                {
                        return null;
                }
                if (!source.IsEntityAlive)
                {
                        return new HealResultDto(
                                sent: request.AmountToHeal,
                                error: $"{source.Name} is not alive and cannot heal {target.Name}."
                        );
                }
                if (!target.IsEntityAlive)
                {
                        return new HealResultDto(
                                sent: request.AmountToHeal,
                                error: $"{target.Name} is not alive and cannot be healed."
                        );
                }
                double actual = (double)(request.AmountToHeal * resistance.Value);
                target.CurrentHealth += actual;
                bool isAlive = DidEntityDie(target);
                return new HealResultDto(
                        sent: request.AmountToHeal,
                        actual: actual,
                        result: target.CurrentHealth,
                        fatal: isAlive
                );
        }

        public ManaChangeDto? ChangeMana(ManaRequest request)
        {
                if (!TryGetEntity(request.TargetId, out var target))
                {
                        return null;
                }
                double actual = request.Amount;
                if (request.Amount < -target.CurrentMana)
                {
                        actual = -target.CurrentMana;
                } else
                if (request.Amount > target.MaxMana - target.CurrentMana)
                {
                        actual = target.MaxMana - target.CurrentMana;
                }
                target.CurrentMana += actual;
                return new(
                        amountSent: request.Amount,
                        amountActual: actual,
                        newMana: target.CurrentMana
                );
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

        public StringDoubleDto? AddProficiencyEntry(AddProficiencyEntryRequest request)
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
                if (target.ProficiencyEntries.TryGetValue(profEnum, out _))
                {
                        target.ProficiencyEntries[profEnum] ++;
                }
                else
                {
                        target.ProficiencyEntries[profEnum] = 1;
                }
                return new(profEnum.ToString(), target.ProficiencyEntries[profEnum]);
        }

        public double GetProficiency(DamageableEntity target, Proficiency proficiency)
        {
                if (target.ProficiencyEntries.TryGetValue(proficiency, out _))
                {
                        return target.Proficiencies[proficiency];
                }
                return 0.5d;
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

// hasProficiency
// setProficiency

// setVisible
// isVisible
// isHidden
// setIsHidden

// getExperience
// setExperience
// getLevel
// setLevel

// getStats
// getMaxHealth
// setMaxHealth
// getHealth
// setHealth
// getMaxMana
// setMaxMana
// getMana
// setMana
// getMagic
// setMagic
// getStrength
// setStrength
// getDefense
// setDefense
// getAllProficiencies