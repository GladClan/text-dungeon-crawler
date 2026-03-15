using GameServer.Domain.Enums;

namespace GameServer.Domain.Entities;

public sealed class Entity(
    string name,
    string entityType,
    int health = 100,
    int mana = 80,
    int magic = 20,
    int strength = 20,
    int defense = 20,
    int level = 0,
    int experience = 0,
    Dictionary<DamageType, double>? resistances = null,
    Dictionary<Proficiency, double>? proficiencies = null
) : DamageableEntity(name, entityType, health, mana, magic, strength, defense, level, experience, resistances, proficiencies)
{
    public EntityInventory Inventory { get; private set; } = new();
    public EntitySkills Skills { get; private set; } = new();
    public double Speed { get; private set; } = 10d;
    // public EntityAI AI { get; private set; }
    public bool IsHidden { get; private set; }

    public bool Hide()
    {
        if (IsHidden)
        {
            return false;
        }
        IsHidden = true;
        return true;
    }

    public bool Reveal()
    {
        if (!IsHidden)
        {
            return false;
        }
        IsHidden = false;
        return true;
    }

    public Entity Clone()
    {
        return new Entity(
            Name,
            EntityType,
            MaxHealth,
            MaxMana,
            (int)Magic,
            (int)Strength,
            (int)Defense,
            Level,
            Experience,
            new Dictionary<DamageType, double>(Resistances),
            new Dictionary<Proficiency, double>(Proficiencies)
        )
        {
            Inventory = Inventory.Clone(),
            Skills = Skills.Clone(),
            Speed = Speed,
            IsHidden = IsHidden,
        };
    }
}