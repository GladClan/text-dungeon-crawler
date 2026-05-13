using System.Diagnostics.CodeAnalysis;
using Gameserver.Contracts.Requests;
using GameServer.Contracts.DTOs;
using GameServer.Contracts.Mappers;
using GameServer.Domain.Battle;
using GameServer.Domain.Entities;
using GameServer.Domain.Enums;
using GameServer.Domain.Items;
using Microsoft.VisualBasic;

namespace Gameserver.Application.Services;

public sealed class CombatService(EntityStore entityStore, BattleTracker battleTracker)
{
        private readonly EntityStore _entities = entityStore;
        private readonly BattleTracker _battle = battleTracker;
        private readonly double _levelStackMultiplier = 1.2;
        private readonly int _proficiencyEntryAddition = 1;
        
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

        public DamageResultDto? Heal(HealRequest request)
        {
                if (!TryGetEntity(request.SourceId, out var source) || !TryGetEntity(request.TargetId, out var target))
                {
                        return null;
                }
                return target.Heal(source, request.AmountToHeal);
        }

        public DamageResultDto? ChangeMana(ManaRequest request)
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

        public int? GetExperienceForNextLevel(string id)
        {
                if (!TryGetEntity(id, out var target))
                {
                        return null;
                }
                return GetExperienceForNextLevel(target.Level);
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
                return target.AddProficiencyEntry(profEnum, request.Amount > 0 ? request.Amount : _proficiencyEntryAddition);
        }

        public string? GetDeathMessage(string id)
        {
                if (!TryGetEntity(id, out var target))
                {
                        return null;
                }
                return target.DeathMessage;
        }

        public EffectDto? UseItem(string id, string itemId, string targetId = "", List<string>? subTargetIds = null)
        {
                if (_entities.TryGet(id, out var source) && source is not null)
                {
                        var item = source.Inventory.Items.FirstOrDefault(i => i.Id.Equals(itemId, StringComparison.InvariantCultureIgnoreCase));
                        if (item is not null)
                        {
                                if (item is Equippable equippable)
                                {
                                        if (equippable.Equipped)
                                        {
                                                return equippable.OnUnequip(source);
                                        }
                                        else
                                        {
                                                return equippable.OnEquip(source);
                                        }
                                }
                                else if (item is Useable useable)
                                {
                                        if (useable.CanUse(source))
                                        {
                                                if (_entities.TryGet(targetId, out var target) && target is not null)
                                                {
                                                        List<DamageableEntity>? targets = null;
                                                        if (subTargetIds is not null)
                                                        {
                                                                string error = "";
                                                                foreach (string tId in subTargetIds)
                                                                {
                                                                        targets ??= [];
                                                                        if (_entities.TryGet(tId, out var entity) && entity is not null)
                                                                        {
                                                                                targets.Add(entity);
                                                                        }
                                                                        else
                                                                        {
                                                                              error += $"Target {tId} could not be found. {source.Name} might be sad that they're missing out.\n";
                                                                        }
                                                                }
                                                                if (error.Length > 0)
                                                                {
                                                                        return new(error);
                                                                }
                                                        }
                                                        return useable.ItemEffect(target, source, targets, _battle);
                                                }
                                                else
                                                {
                                                        return new EffectDto
                                                        {
                                                                Error = $"Target {targetId} could not be found. {source.Name} isn't about to use it on themselves!"
                                                        };
                                                }
                                        }
                                        else
                                        {
                                                return new EffectDto
                                                {
                                                        Error = $"{source.Name} cannot use {item.Name}"
                                                };
                                        }
                                }
                        }
                        return new EffectDto
                        {
                                Error = $"Item id {itemId} could not be found"
                        };
                }
                return null;
        }

        public EffectDto? UseSkill(string id, string skillId, string targetId, List<string>? subTargetIds = null)
        {
                if (!TryGetEntity(id, out var source) || !TryGetEntity(targetId, out var target))
                {
                        return null;
                }
                var skill = source.Skills.FirstOrDefault(s => s.Id.Equals(skillId, StringComparison.InvariantCultureIgnoreCase));
                if (skill is null)
                {
                        return new EffectDto
                        {
                                Error = $"Could not find skill id: {skillId} in the skills of {source.Name} ({source.ID})"
                        };
                }
                List<DamageableEntity>? targets = null;
                if (subTargetIds is not null)
                {
                        string error = "";
                        foreach (string tId in subTargetIds)
                        {
                                targets ??= [];
                                if (_entities.TryGet(tId, out var entity) && entity is not null)
                                {
                                        targets.Add(entity);
                                }
                                else
                                {
                                        error += $"Target {tId} could not be found. {source.Name} might be sad that they're missing out.\n";
                                }
                        }
                        if (error.Length > 0)
                        {
                                return new(error);
                        }
                }
                return skill.SkillEffect(source, target, targets, _battle);
        }
}
