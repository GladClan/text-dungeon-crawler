using GameServer.Contracts.DTOs;
using GameServer.Domain.Battle;
using GameServer.Domain.Items;
using GameServer.Domain.Skills;

namespace GameServer.Domain.Entities.EntityAI.AILibrary;

public class DefaultAI: IEntityAI
{
    public string Tag { get; } = "default";
    // For keeping track of the target entity
    public string SignificantEntityId { get; set; } = "";
    // For alternating between using items and skills
    private bool _item_or_skill = false;
    // Counter for how many items or skills source has used, used for varying item and skill uses.
    private int _optionCount = 0;
    // Used to keep track of who has received buffs and who has not, as well as what buffs have been used
    private int _buffsPerformed = 0;
    // Used to help vary the actions performed by source
    private int _actionSeed = 0;

    private static EffectDto Confused(string sourceName)
    {
        return new(
            message: $"{sourceName} is confused and cannot determine what to do...",
            results: [],
            wasMagic: false
        );
    }
    // No use in this case
    public bool SetSignificantEntityId(string id)
    {
        SignificantEntityId = id;
        return true;
    }

    public EffectDto GetAction(DamageableEntity source, BattleTracker battle)
    {
        // get a list of the options that source can use, items and skills.
        List<Useable> attackItems = [.. 
            source.Inventory.Items
                .Where(i => i is Useable u && u.ItemType == Enums.ActionType.Attack)
                .Select(i => (Useable)i)
            ];
        List<Skill> attackSkills = [.. 
            source.Skills
                .Where(s => s.SkillType == Enums.ActionType.Attack)
            ];
        int healthPercent = (int)(source.CurrentHealth / source.MaxHealth * 100);
        
        // assign target to one who does first damage to self
        SignificantEntityId = battle.Log.GetHasAttackedSource(source.ID) ?? "";
        string enemyPartyId = battle.PartyId.Equals(source.PartyId, StringComparison.InvariantCultureIgnoreCase) ? battle.OpponentPartyId : source.PartyId;
        DamageableEntity? target = null;

        // make sure target is not dead, if is dead then move to the next target
        int i = 0;
        while (SignificantEntityId != "" && (target is null || target.IsEntityAlive))
        {
            target = battle.GetEntity(SignificantEntityId);
            if (target is not null && !target.IsEntityAlive)
            {
                target = null;
                SignificantEntityId = battle.Log.GetHasAttackedSource(source.ID, ++i) ?? "";
            }
        }

        // declare GetAction result now so it can be assigned
        EffectDto result = Confused(source.Name);

        // do action based on health percentage
        if (healthPercent > 70)
        {
            // attack target
            if (target is not null && target.IsEntityAlive && _actionSeed > 4 && _actionSeed < 9)
            {
                result = UseItemOrSkill(source, target, battle, attackItems, attackSkills);
                _actionSeed++;
            }
            // do buffs
            else if (source.Skills.Any(s => s.SkillType == Enums.ActionType.Buff) && battle.GetPartyIds(source.PartyId).Count > 1)
            {
                result = DoBuffs(source, battle);
                _actionSeed++;
            }
            // Heal critical party members
            else if (
                    battle.ExistsPartyMemberAtCriticalHealth(source.PartyId) && 
                    (
                        source.Skills.Any(s => s.SkillType == Enums.ActionType.Healing) || 
                        source.Inventory.Items.Any(i => i is Useable u && u is not null && u.ItemType == Enums.ActionType.Healing)
                    )
                )
            {
                string? targetId = battle.GetPartyMemberAtCriticalHealth(source.PartyId);
                if (targetId is not null)
                {
                    target = battle.GetEntity(targetId);
                    if (target is not null)
                    {
                        List<Useable> items = [.. 
                            source.Inventory.Items
                                .Where(i => i is Useable u && u.ItemType == Enums.ActionType.Healing)
                                .Select(i => (Useable)i)
                        ];
                        List<Skill> skills = [..
                            source.Skills
                                .Where(s => s.SkillType == Enums.ActionType.Healing)
                        ];
                        result = UseItemOrSkill(source, target, battle, items, skills);
                    }
                }
            }
            else
            {
                // attack highest single damage
                result = AttackHighestDamageToSelf(source, battle, attackItems, attackSkills);
            }
        }
        else if (healthPercent > 50)
        {
            // attack target
            if (target is not null && target.IsEntityAlive && _actionSeed % 3 != 0)
            {
                result = UseItemOrSkill(source, target, battle, attackItems, attackSkills);
            }
            else if (_actionSeed < 4 || _actionSeed > 8)
            {
                // attack healers
                string targetId = battle.Log.GetMostHealer() ?? "";
                target = battle.GetEntity(targetId);
                i = 1;
                while (target is not null && (!target.IsEntityAlive || target.PartyId.Equals(source.PartyId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    targetId = battle.Log.GetMostHealer() ?? "";
                    target = battle.GetEntity(targetId);
                }
                if (target is not null && target.IsEntityAlive)
                {
                    result = UseItemOrSkill(source, target, battle, attackItems, attackSkills);
                }
            }
            // heal
            else if (_actionSeed % 2 == 0  && (
                    source.Skills.Any(s => s.SkillType == Enums.ActionType.Healing) ||
                    source.Inventory.Items.Any(i => i is Useable u && u.ItemType == Enums.ActionType.Healing)
                ))
            {
                _actionSeed++;
                List<Skill> healSkills = [..
                    source.Skills
                        .Where(s => s.SkillType == Enums.ActionType.Healing)
                ];
                List<Useable> healItems = [..
                    source.Inventory.Items
                        .Where(i => i is Useable u && u.ItemType == Enums.ActionType.Healing)
                        .Select(i => (Useable)i)
                ];
                result = UseItemOrSkill(source, source, battle, healItems, healSkills);
            }
            // do buffs
            else if (_actionSeed % 2 == 1 && (
                    source.Skills.Any(s => s.SkillType == Enums.ActionType.Buff) ||
                    source.Inventory.Items.Any(i => i is Useable u && u.ItemType == Enums.ActionType.Buff)
                ))
            {
                _actionSeed++;
                result = DoBuffs(source, battle);
            }
            else
            {
                // attack highest damage to self
                result = AttackHighestDamageToSelf(source, battle, attackItems, attackSkills);
            }
        }
        else if (healthPercent > 30)
        {
            if (_actionSeed % 2 == 0 && (
                    source.Skills.Any(s => s.SkillType == Enums.ActionType.Buff) ||
                    source.Inventory.Items.Any(i => i is Useable u && u.ItemType == Enums.ActionType.Buff)
                ))
            {
                // do buffs
                _actionSeed++;
                result = DoBuffs(source, battle);
            }
            else if (
                    source.Skills.Any(s => s.SkillType == Enums.ActionType.Healing) ||
                    source.Inventory.Items.Any(i => i is Useable u && u.ItemType == Enums.ActionType.Healing)
                )
            {
                // heal 
                _actionSeed++;
                List<Skill> healSkills = [..
                    source.Skills
                        .Where(s => s.SkillType == Enums.ActionType.Healing)
                ];
                List<Useable> healItems = [..
                    source.Inventory.Items
                        .Where(i => i is Useable u && u.ItemType == Enums.ActionType.Healing)
                        .Select(i => (Useable)i)
                ];
                result = UseItemOrSkill(source, source, battle, healItems, healSkills);
            }
            else
            {
                // attack highest damage to self
                result = AttackHighestDamageToSelf(source, battle, attackItems, attackSkills);
            }
        }
        else if (healthPercent > 20)
        {
            if (_actionSeed % 2 == 0 && (
                    source.Skills.Any(s => s.SkillType == Enums.ActionType.Defense) ||
                    source.Inventory.Items.Any(i => i is Useable u && u.ItemType == Enums.ActionType.Defense)
                ))
            {
                // defend
                _actionSeed++;
                List<Skill> defenseSkills = [..
                    source.Skills
                        .Where(s => s.SkillType == Enums.ActionType.Defense)
                ];
                List<Useable> defenseItems = [..
                    source.Inventory.Items
                        .Where(i => i is Useable u && u.ItemType == Enums.ActionType.Defense)
                        .Select(i => (Useable)i)
                ];
                result = UseItemOrSkill(source, source, battle, defenseItems, defenseSkills);
            }
            else if (
                    source.Skills.Any(s => s.SkillType == Enums.ActionType.Healing) ||
                    source.Inventory.Items.Any(i => i is Useable u && u.ItemType == Enums.ActionType.Healing)
                )
            {
                // heal
                _actionSeed++;
                List<Skill> healSkills = [..
                    source.Skills
                        .Where(s => s.SkillType == Enums.ActionType.Healing)
                ];
                List<Useable> healItems = [..
                    source.Inventory.Items
                        .Where(i => i is Useable u && u.ItemType == Enums.ActionType.Healing)
                        .Select(i => (Useable)i)
                ];
                result = UseItemOrSkill(source, source, battle, healItems, healSkills);
            }
            else if (target is not null && target.IsEntityAlive)
            {
                // attack target
                result = UseItemOrSkill(source, target, battle, attackItems, attackSkills);
            }
            else
            {
                // attack highest damage to self
                result = AttackHighestDamageToSelf(source, battle, attackItems, attackSkills);
            }
        }
        else
        {
            // for heavier, death-defying attacks, increase strength
            source.Strength *= 1.5;
            // heavy attacks to target
            if (target is not null)
            {
                result = UseItemOrSkill(source, target, battle, attackItems, attackSkills);
            }
            else
            {
                // heavy attacks to highest damage to self
                result = AttackHighestDamageToSelf(source, battle, attackItems, attackSkills);
            }

            // ensure strength is reset to normal
            source.Strength /= 1.5;
        }
        return result;
    }

    private EffectDto DoBuffs(DamageableEntity source, BattleTracker battle)
    {
        List<Skill> skills = [.. source.Skills.Where(s => s.SkillType == Enums.ActionType.Buff)];
        List<Useable> items = [..
                source.Inventory.Items
                    .Where(i => i is Useable u && u.ItemType == Enums.ActionType.Buff)
                    .Select(i => (Useable)i)
            ];
        var ids = battle.GetPartyIds(source.PartyId);
        List<DamageableEntity> sourceParty = [];
        foreach (string id in ids)
        {
            var entity = battle.GetEntity(id);
            if (entity is not null && entity.IsEntityAlive)
            {
                sourceParty.Add(entity);
            }
        }
        EffectDto result = Confused(source.Name);
        if (items.Count > 0)
        {
            var buffChoice = items[_buffsPerformed / ids.Count % items.Count];
            sourceParty = TakeNPartyMembers(buffChoice.TargetsLimit, sourceParty);
            var target = sourceParty.Count > 0 ? sourceParty[_buffsPerformed % sourceParty.Count] : source;
            result = buffChoice.ItemEffect(
                source: source,
                mainTarget: target,
                subTargets: [.. sourceParty],
                battle: battle
            );
        }
        else if (skills.Count > 0)
        {
            var buffChoice = skills[_buffsPerformed / ids.Count % skills.Count];
            sourceParty = TakeNPartyMembers(buffChoice.TargetsLimit, sourceParty);
            var target = sourceParty.Count > 0 ? sourceParty[_buffsPerformed % sourceParty.Count] : source;
            result = buffChoice.SkillEffect(
                source: source,
                mainTarget: target,
                subTargets: [.. sourceParty],
                battle: battle
            );
        }
        _buffsPerformed++;
        return result;
    }

    private static List<DamageableEntity> TakeNPartyMembers(int n, List<DamageableEntity> party)
    {
        if (party.Count > 1)
        {
            int i = n > party.Count ? party.Count - 1 : n;
            return [.. party.Take(i)];
        }
        return [];
    }

    private EffectDto AttackHighestDamageToSelf(DamageableEntity source, BattleTracker battle, List<Useable> items, List<Skill> skills)
    {
        // attack highest single damage
        string targetId = battle.Log.GetHighestSingleDamage() ?? "";
        var target = battle.GetEntity(targetId);
        if (target is not null)
        {
            int i = 1;
            // ensure the target is alive and not the same party as source
            while (target is not null && (!target.IsEntityAlive || target.PartyId.Equals(source.PartyId, StringComparison.InvariantCultureIgnoreCase)))
            {
                targetId = battle.Log.GetHighestSingleDamage(i) ?? "";
                target = battle.GetEntity(targetId);
            }
            if (target is not null)
            {
                return UseItemOrSkill(source, target, battle, items, skills);
            }
        }
        return Confused(source.Name);
    }

    private EffectDto UseItemOrSkill(DamageableEntity source, DamageableEntity target, BattleTracker battle, List<Useable> items, List<Skill> skills)
    {
        EffectDto result = Confused(source.Name);

        // alternates between using items and skills, makes sure there are skills or items to use
        if ((!_item_or_skill || skills.Count == 0) && items.Count > 0)
        {
            // choose what attack to use based on how many items and skills have been used
            var attackChoice = items[_optionCount % items.Count];
            List<DamageableEntity> subTargets = [];
            if (attackChoice.MultiTarget)
            {
                var ids = battle.GetPartyIds(target.PartyId);
                for (int i = 0; i < attackChoice.TargetsLimit && i < ids.Count; i++)
                {
                    var subTarget = battle.GetEntity(ids[i]);
                    if (subTarget is not null && subTarget.IsEntityAlive)
                    {
                        subTargets.Add(subTarget);
                    }
                }
            }
            result = attackChoice.ItemEffect(
                source: source,
                mainTarget: target,
                subTargets: subTargets,
                battle: battle
            );
        }
        else if (skills.Count > 0)
        {
            var attackChoice = skills[_optionCount % skills.Count];
            if (attackChoice is not null)
            {
                List<DamageableEntity> subTargets = [];
                if (attackChoice.MultiTarget)
                {
                    var ids = battle.GetPartyIds(target.PartyId);
                    for (int i = 0; i < attackChoice.TargetsLimit && i < ids.Count; i++)
                    {
                        var subTarget = battle.GetEntity(ids[i]);
                        if (subTarget is not null)
                        {
                            subTargets.Add(subTarget);
                        }
                    }
                }
                result = attackChoice.SkillEffect(
                    source: source,
                    mainTarget: target,
                    subTargets: subTargets,
                    battle: battle
                );
            }
        }
        _optionCount++;
        _item_or_skill = !_item_or_skill;
        return result;
    }
}

public class HatesMagic: IEntityAI
{
    public string Tag { get; } = "hates-magic";
    // For keeping track of the target entity
    public string SignificantEntityId { get; set; } = "";
    public bool SetSignificantEntityId(string id)
    {
        throw new NotImplementedException();
    }

    public EffectDto GetAction(DamageableEntity source, BattleTracker battle)
    {
        int healthPercent = (int)(source.MaxHealth / source.CurrentHealth * 100);
        // assign targetId to entity who last used magic
        if (healthPercent > 70)
        {
            // attack target
            if (
                    source.Skills.Any(s => s.SkillType == Enums.ActionType.Buff) ||
                    source.Inventory.Items.Any(i => i is Useable u && u.ItemType == Enums.ActionType.Buff)
                )
            {
                // do buffs
            }
        }
        else if (healthPercent > 50)
        {
            // attack target
            // attack healers
        }
        else if (healthPercent > 30)
        {
            // do buffs
            // attack target
            // heal
        }
        else if (healthPercent > 20)
        {
            // attack target
            // defend
            // heal
        }
        else
        {
            // buff: strength increase
            // heavy attacks to target
            // heal
        }
        throw new NotImplementedException();
    }
}

public class Berserk: IEntityAI
{
    public string Tag { get; } = "berserk";
    // For keeping track of the target entity
    public string SignificantEntityId { get; set; } = "";
    public bool SetSignificantEntityId(string id)
    {
        SignificantEntityId = id;
        return true;
    }

    public EffectDto GetAction(DamageableEntity source, BattleTracker battle)
    {
        int healthPercent = (int)(source.MaxHealth / source.CurrentHealth * 100);
        if (healthPercent > 60)
        {
            SignificantEntityId = battle.Log.GetHasAttackedSource(source.ID) ?? "";
            // check if target is alive, false = GetHasAttackedSource(sourceID, n + 1)
            // attack target
            // attack first enemy
        }
        else if (healthPercent > 30)
        {
            source.Strength *= 2;
            // attack target
            // attack first enemy
            source.Strength /= 2;
        }
        else
        {
            source.Strength *= 3;
            // attack target
            // attack first enemy
            source.Strength /= 3;
        }
        throw new NotImplementedException();
    }
}