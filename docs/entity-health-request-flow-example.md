# Full Request Flow Example: ChangeHealthRequest (Damage + Heal)

This guide shows the minimum backend pieces needed for one complete command flow.

Flow summary:

1. Frontend calls an endpoint with route id + JSON body.
2. ASP.NET binds JSON to `ChangeHealthRequest`.
3. Controller finds target entity from route id.
4. Controller chooses behavior (`TakeDamage` or `Heal`).
5. Domain entity mutates state.
6. Controller maps domain object to `EntityDto` and returns JSON.

---

## What each input means

- `targetId` (route): which entity should change.
- `request.Amount` (body): how much to change health.
- `request.SourceEntityId` (body, optional): who caused the effect.
- Endpoint path (`/damage` vs `/heal`): what operation to execute.

The request DTO does not execute logic by itself. It only carries values.

---

## Required scope (minimum files)

1. Request/response DTOs + mapper (Contracts).
2. Entity lookup store/repository (Infrastructure).
3. Controller endpoint(s) (API layer).
4. Program startup registration for DI + controllers.

---

## 1) Request DTO (Contracts)

Use this as body input for health-changing endpoints.

```csharp
using System.ComponentModel.DataAnnotations;

namespace GameServer.Contracts;

public sealed class ChangeHealthRequest
{
    // Optional source for combat logs, threat systems, etc.
    public Guid? SourceEntityId { get; init; }

    [Range(typeof(double), "-999999", "999999", ConvertValueInInvariantCulture = true)]
    public double Amount { get; init; }
}
```

Why needed:

- Gives a stable JSON contract for clients.
- Centralizes basic validation rules.

---

## 2) Store/Repository (Infrastructure)

The controller needs a way to resolve entity ids to domain objects.

```csharp
using System.Collections.Concurrent;
using GameServer.Domain.Entities;

namespace GameServer.Infrastructure;

public sealed class EntityStore
{
    private readonly ConcurrentDictionary<Guid, Entity> _entities = new();

    public bool TryGet(Guid id, out Entity? entity)
    {
        return _entities.TryGetValue(id, out entity);
    }

    public IEnumerable<Entity> GetAll()
    {
        return _entities.Values;
    }

    public void Upsert(Entity entity)
    {
        _entities[entity.ID] = entity;
    }
}
```

Why needed:

- Route ids are just values until you resolve them to actual entities.

---

## 3) Controller Endpoints (API)

This is where intent is chosen.

- `POST /api/entities/{targetId}/damage` calls `TakeDamage`.
- `POST /api/entities/{targetId}/heal` calls `Heal`.

```csharp
using GameServer.Contracts;
using GameServer.Domain.Entities;
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

    [HttpPost("{targetId:guid}/damage")]
    public ActionResult<EntityDto> Damage(Guid targetId, [FromBody] ChangeHealthRequest request)
    {
        if (!_store.TryGet(targetId, out var target) || target is null)
        {
            return NotFound("Target entity not found.");
        }

        // Default to self-source if source id is not supplied.
        DamageableEntity source = target;

        if (request.SourceEntityId is Guid sourceId)
        {
            if (!_store.TryGet(sourceId, out var sourceEntity) || sourceEntity is null)
            {
                return NotFound("Source entity not found.");
            }

            source = sourceEntity;
        }

        target.TakeDamage(source, request.Amount);
        return Ok(target.ToDto());
    }

    [HttpPost("{targetId:guid}/heal")]
    public ActionResult<EntityDto> Heal(Guid targetId, [FromBody] ChangeHealthRequest request)
    {
        if (!_store.TryGet(targetId, out var target) || target is null)
        {
            return NotFound("Target entity not found.");
        }

        DamageableEntity source = target;

        if (request.SourceEntityId is Guid sourceId)
        {
            if (!_store.TryGet(sourceId, out var sourceEntity) || sourceEntity is null)
            {
                return NotFound("Source entity not found.");
            }

            source = sourceEntity;
        }

        target.Heal(source, request.Amount);
        return Ok(target.ToDto());
    }
}
```

Why needed:

- This layer decides what command to run and on which entity.

---

## 4) Program Startup (wiring)

Register your dependencies so ASP.NET can construct controllers and services.

```csharp
using GameServer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<EntityStore>();

var app = builder.Build();

app.MapControllers();
app.Run();
```

Why needed:

- Without DI registration, controller dependencies cannot be created.

---

## 5) Mapper usage (response contract)

Return domain state as DTOs, not domain objects.

```csharp
// In controller actions after mutation:
return Ok(target.ToDto());
```

Why needed:

- Keeps API payload stable and decoupled from domain internals.

---

## 6) Example HTTP calls

### Damage

```http
POST /api/entities/5ec75f58-5bf5-435c-a7d8-49eec8e86f9b/damage
Content-Type: application/json

{
  "sourceEntityId": "47f97150-ef6b-435b-8e11-50e8e6ce11db",
  "amount": 18.5
}
```

### Heal

```http
POST /api/entities/5ec75f58-5bf5-435c-a7d8-49eec8e86f9b/heal
Content-Type: application/json

{
  "amount": 12
}
```

---

## 7) Decision map (quick reference)

- "What entity changes?" -> Route id (`targetId`).
- "How much?" -> `ChangeHealthRequest.Amount`.
- "Who caused it?" -> `ChangeHealthRequest.SourceEntityId`.
- "Damage or heal?" -> Which endpoint was called.
- "Where does the mutation happen?" -> Domain methods (`TakeDamage`/`Heal`).
- "What goes back to UI?" -> `EntityDto` from mapper.

---

## 8) Practical build order

1. Keep contracts and mapper in place.
2. Add/finish store lookup methods.
3. Add one endpoint (`/damage`) and test it.
4. Add second endpoint (`/heal`).
5. Add logging, validation responses, and tests.

## 9) Process overview

1. Frontend sends POST /api/entities/{targetId}/damage with Amount and optional SourceEntityId.
2. ASP.NET model binding creates ChangeHealthRequest.
3. Validation runs (for example Range on Amount).
4. Controller finds target entity by targetId.
5. Controller optionally finds source entity by SourceEntityId.
6. Controller calls target.TakeDamage(source, request.Amount).
7. Domain updates Health and checks alive/death logic.
8. Controller maps target to EntityDto via mapper.
9. API returns updated EntityDto to frontend.

- The endpoint path/method defines intent.
    - Example: `POST /api/entities/{id}/damage` means apply damage.
    - Example: `POST /api/entities/{id}/heal` means apply healing.
- The route id identifies the target entity
- The JSON body is bound into ChangeHealthRequest.
- The controller loads the target (and optional source) from storage.
- The controller calls the correct domain method:
    - Damage uses `TakeDamage(...)`
    - Healing uses `Heal(...)`
- The domain method updates health and enforces rules.
- The updated entity is mapped to EntityDto and returned.
