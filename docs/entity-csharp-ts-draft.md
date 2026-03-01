# Entity Architecture Draft: C# Backend + TypeScript Frontend

This is a practical starter draft showing how your current TypeScript `Entity` style can live in a C# backend, while your frontend remains React + TypeScript.

---

## 1) C# Domain Model (game logic lives here)

```csharp
// Domain/Entity.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Domain;

public sealed class Entity
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; private set; }
    public string Type { get; private set; }

    private readonly Dictionary<string, int> _resistances;
    public IReadOnlyDictionary<string, int> Resistances => _resistances;

    public bool IsHidden { get; private set; }
    public int Speed { get; private set; } = 10;

    public EntityInventory Inventory { get; private set; } = new();
    public EntitySkills Skills { get; private set; } = new();

    // Store AI as a strategy key for serialization simplicity
    public string? AiStrategy { get; private set; }

    public Entity(string name, string type, Dictionary<string, int>? resistances = null)
    {
        Name = name;
        Type = type;
        _resistances = resistances is null
            ? new Dictionary<string, int>()
            : new Dictionary<string, int>(resistances);
    }

    public bool Hide()
    {
        if (IsHidden) return false;
        IsHidden = true;
        return true;
    }

    public bool Reveal()
    {
        if (!IsHidden) return false;
        IsHidden = false;
        return true;
    }

    public void SetSpeed(int speed)
    {
        if (speed < 0) throw new ArgumentOutOfRangeException(nameof(speed), "Speed cannot be negative.");
        Speed = speed;
    }

    public void SetInventory(EntityInventory inventory)
    {
        Inventory = inventory;
    }

    public void SetSkills(EntitySkills skills)
    {
        Skills = skills;
    }

    public void SetAi(string strategy)
    {
        AiStrategy = strategy;
    }

    public Entity Clone()
    {
        var clone = new Entity(Name, Type, _resistances.ToDictionary(pair => pair.Key, pair => pair.Value));
        clone.SetSpeed(Speed);
        clone.SetInventory(Inventory.Clone());
        clone.SetSkills(Skills.Clone());
        if (AiStrategy is not null) clone.SetAi(AiStrategy);
        if (IsHidden) clone.Hide();
        return clone;
    }
}

public sealed class EntityInventory
{
    public int Gold { get; private set; }
    public List<string> Items { get; private set; } = new();

    public EntityInventory Clone()
    {
        return new EntityInventory
        {
            Gold = Gold,
            Items = new List<string>(Items)
        };
    }
}

public sealed class EntitySkills
{
    public List<string> SkillIds { get; private set; } = new();

    public EntitySkills Clone()
    {
        return new EntitySkills
        {
            SkillIds = new List<string>(SkillIds)
        };
    }
}
```

---

## 2) C# DTOs (API contract)

```csharp
// Contracts/EntityDto.cs
using System;
using System.Collections.Generic;

namespace GameServer.Contracts;

public sealed class EntityDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public Dictionary<string, int> Resistances { get; init; } = new();
    public bool IsHidden { get; init; }
    public int Speed { get; init; }
    public InventoryDto Inventory { get; init; } = new();
    public SkillsDto Skills { get; init; } = new();
    public string? AiStrategy { get; init; }
}

public sealed class InventoryDto
{
    public int Gold { get; init; }
    public List<string> Items { get; init; } = new();
}

public sealed class SkillsDto
{
    public List<string> SkillIds { get; init; } = new();
}

public sealed class SetSpeedRequest
{
    public int Speed { get; init; }
}
```

---

## 3) Mapping + In-Memory Store (starter)

```csharp
// Infrastructure/EntityStore.cs
using System;
using System.Collections.Concurrent;
using GameServer.Domain;

namespace GameServer.Infrastructure;

public sealed class EntityStore
{
    private readonly ConcurrentDictionary<Guid, Entity> _entities = new();

    public EntityStore()
    {
        var starter = new Entity("Hero", "main", new() { ["fire"] = 10, ["ice"] = 0 });
        _entities[starter.Id] = starter;
    }

    public bool TryGet(Guid id, out Entity? entity) => _entities.TryGetValue(id, out entity);
}
```

```csharp
// Contracts/EntityMapper.cs
using GameServer.Domain;

namespace GameServer.Contracts;

public static class EntityMapper
{
    public static EntityDto ToDto(this Entity entity)
    {
        return new EntityDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Type = entity.Type,
            Resistances = new Dictionary<string, int>(entity.Resistances),
            IsHidden = entity.IsHidden,
            Speed = entity.Speed,
            Inventory = new InventoryDto
            {
                Gold = entity.Inventory.Gold,
                Items = new List<string>(entity.Inventory.Items)
            },
            Skills = new SkillsDto
            {
                SkillIds = new List<string>(entity.Skills.SkillIds)
            },
            AiStrategy = entity.AiStrategy
        };
    }
}
```

---

## 4) API Controller (commands + query)

```csharp
// Api/EntitiesController.cs
using System;
using GameServer.Contracts;
using GameServer.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Api;

[ApiController]
[Route("api/entities")]
public sealed class EntitiesController : ControllerBase
{
    private readonly EntityStore _store;

    public EntitiesController(EntityStore store)
    {
        _store = store;
    }

    [HttpGet("{id:guid}")]
    public ActionResult<EntityDto> GetById(Guid id)
    {
        if (!_store.TryGet(id, out var entity) || entity is null) return NotFound();
        return Ok(entity.ToDto());
    }

    [HttpPost("{id:guid}/hide")]
    public ActionResult<EntityDto> Hide(Guid id)
    {
        if (!_store.TryGet(id, out var entity) || entity is null) return NotFound();
        entity.Hide();
        return Ok(entity.ToDto());
    }

    [HttpPost("{id:guid}/reveal")]
    public ActionResult<EntityDto> Reveal(Guid id)
    {
        if (!_store.TryGet(id, out var entity) || entity is null) return NotFound();
        entity.Reveal();
        return Ok(entity.ToDto());
    }

    [HttpPost("{id:guid}/speed")]
    public ActionResult<EntityDto> SetSpeed(Guid id, [FromBody] SetSpeedRequest request)
    {
        if (!_store.TryGet(id, out var entity) || entity is null) return NotFound();
        entity.SetSpeed(request.Speed);
        return Ok(entity.ToDto());
    }
}
```

---

## 5) Program Setup (ASP.NET Core)

```csharp
// Program.cs
using GameServer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<EntityStore>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("frontend");
app.MapControllers();
app.Run();
```

---

## 6) TypeScript Frontend Types

```ts
// src/types/entityApi.ts
export interface InventoryDto {
  gold: number;
  items: string[];
}

export interface SkillsDto {
  skillIds: string[];
}

export interface EntityDto {
  id: string;
  name: string;
  type: string;
  resistances: Record<string, number>;
  isHidden: boolean;
  speed: number;
  inventory: InventoryDto;
  skills: SkillsDto;
  aiStrategy: string | null;
}
```

---

## 7) TypeScript API Client

```ts
// src/lib/entityApiClient.ts
import type { EntityDto } from '@/types/entityApi';

const API_BASE = 'http://localhost:5050/api/entities';

async function parseResponse(response: Response): Promise<EntityDto> {
  if (!response.ok) {
    const text = await response.text();
    throw new Error(`API ${response.status}: ${text}`);
  }
  return (await response.json()) as EntityDto;
}

export async function fetchEntity(id: string): Promise<EntityDto> {
  const response = await fetch(`${API_BASE}/${id}`);
  return parseResponse(response);
}

export async function hideEntity(id: string): Promise<EntityDto> {
  const response = await fetch(`${API_BASE}/${id}/hide`, { method: 'POST' });
  return parseResponse(response);
}

export async function revealEntity(id: string): Promise<EntityDto> {
  const response = await fetch(`${API_BASE}/${id}/reveal`, { method: 'POST' });
  return parseResponse(response);
}

export async function setEntitySpeed(id: string, speed: number): Promise<EntityDto> {
  const response = await fetch(`${API_BASE}/${id}/speed`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ speed }),
  });
  return parseResponse(response);
}
```

---

## 8) Example React usage

```tsx
'use client';

import { useEffect, useState } from 'react';
import type { EntityDto } from '@/types/entityApi';
import { fetchEntity, hideEntity, revealEntity, setEntitySpeed } from '@/lib/entityApiClient';

export default function EntityPanel({ entityId }: { entityId: string }) {
  const [entity, setEntity] = useState<EntityDto | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchEntity(entityId).then(setEntity).catch((err) => setError(err.message));
  }, [entityId]);

  if (error) return <div>Error: {error}</div>;
  if (!entity) return <div>Loading...</div>;

  return (
    <div>
      <h3>{entity.name}</h3>
      <p>Type: {entity.type}</p>
      <p>Hidden: {entity.isHidden ? 'Yes' : 'No'}</p>
      <p>Speed: {entity.speed}</p>

      <button onClick={() => hideEntity(entity.id).then(setEntity)}>Hide</button>
      <button onClick={() => revealEntity(entity.id).then(setEntity)}>Reveal</button>
      <button onClick={() => setEntitySpeed(entity.id, entity.speed + 1).then(setEntity)}>+Speed</button>
    </div>
  );
}
```

---

## Notes for your project

- Keep **game rules** in C#; TS UI should call commands and render returned state.
- Use DTOs for API contracts so frontend doesn’t depend on domain internals.
- For turn updates/combat logs, add SignalR later so UI gets server-pushed events.
