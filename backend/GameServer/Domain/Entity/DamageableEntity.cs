namespace GameServer.Domain.Entity;

public class DamageableEntity : EntityMetadata
{
    // 
}

/**
private stats = new EntityStats(defaults.health, defaults.mana, defaults.magic, defaults.strength, defaults.defense, defaults.proficiencies);
private resistances: Partial<Record<DamageType, number>>;

constructor(name: string, type: string, resistances?: Partial<Record<DamageType, number>>, id?: string) {
        super(name, type, id);
        this.resistances = resistances || {};
    }
*/