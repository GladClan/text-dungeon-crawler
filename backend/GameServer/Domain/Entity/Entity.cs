using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Domain.Entity;

public sealed class Entity : DamageableEntity
{
    public Guid Id { get; } = Guid.newGuid();
    public string Name { get; private set; }
    public string Type { get; private set; }

    private readonly Dictionary<string, int> _resistances;
    private IReadOnlyDictionary<string, int> Resistances => _resistances;

    public bool isHidden { get; private set; }
}