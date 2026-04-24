# File Structure (C# Backend + TypeScript Frontend)

Current state of the project structure. Items marked **TODO** do not yet exist and are planned for future implementation.<br/>
*The project was changed from a 100% Next.js typescript project to a Next.js frontend with an ASP.NET Core backend*

```txt
text_dungeon_crawler/
├─ docs/
│  ├─ entity-csharp-ts-draft.md
│  └─ entity-csharp-ts-file-structure.md
│
├─ src/                                      # Next.js frontend
│  ├─ app/
│  │  ├─ globals.css
│  │  ├─ layout.tsx
│  │  ├─ page.tsx
│  │  ├─ anew/
│  │  ├─ battle/
│  │  ├─ battleResult/
│  │  ├─ game/
│  │  ├─ party/
│  │  └─ story/
│  ├─ components/
│  │  ├─ enemyCard.tsx
│  │  ├─ equipModal.tsx
│  │  ├─ healthBar.tsx
│  │  ├─ partyMemberCard.tsx
│  │  ├─ storybox.tsx
│  │  ├─ wigglyButton.tsx
│  │  └─ Chat Overlay/
│  ├─ context/
│  │  └─ gameContext.tsx
│  ├─ lib/
│  │  ├─ entityApiClient.ts                  # TODO: frontend API calls to C# backend
│  │  └─ ...
│  ├─ types/
│  │  └─ entityApi.ts                        # TODO: DTO types matching backend JSON
│  └─ entityApiClient.ts                     # TODO: frontend API calls to C# backend
│
├─ backend/
│  ├─ GameServer.sln
│  └─ GameServer/
│     ├─ GameServer.csproj
│     ├─ Program.cs                          # TODO: CORS + DI + controllers
│     ├─ appsettings.json
│     ├─ appsettings.Development.json
│     │
│     ├─ Api/
│     |  └─ Controllers/
│     |     ├─ EntitiesController.cs           # CRUD and lookups only
│     |     ├─ EntityCombatController.cs       # damage/heal/mana endpoints
│     |     ├─ EntityInventoryController.cs    # inventory and gold endpoints
│     |     └─ EntitySkillsController.cs       # skills endpoints
│     │
|     ├─ Application/                          # TODO: Controllers are for HTTP transport.
|     │  ├─ Entities/                          #        Services are for the actual use-cases and implementation of the HTTP functions.
|     │  │  ├─ IEntityService.cs               #        In essence, the controllers call the service methods, which implemet the actual logic.
|     │  │  ├─ EntityService.cs                #        All of the files here that start with `I` are interfaces for their respective implementations
|     │  │  ├─ ICombatService.cs
|     │  │  ├─ CombatService.cs
|     │  │  ├─ IInventoryService.cs
|     │  │  ├─ InventoryService.cs
|     │  │  ├─ ISkillService.cs
|     │  │  └─ SkillService.cs
|     │  └─ Common/
|     │     └─ EntityRequestParsers.cs         # enum parsing and request normalization
|     |
│     ├─ Domain/
│     │  ├─ Entity/
│     │  │  ├─ DamageableEntity.cs
│     │  │  ├─ EntityAI.cs                   # TODO: Algorithm base for decision-making
│     │  │  ├─ EntityInventory.cs
│     │  │  ├─ EntityMetadata.cs
│     │  │  └─ EntitySkills.cs
│     │  ├─ Enums/
│     │  │  ├─ DamageType.cs
│     │  │  └─ Proficiency.cs
│     │  ├─ Exceptions/
│     │  │  └─ EntityExceptions.cs
│     │  ├─ Items/
│     │  │  ├─ Equippable.cs
│     │  │  ├─ Item.cs
│     │  │  └─ Useable.cs
│     │  └─ Skills/
│     │     └─ Skill.cs
│     │
│     ├─ Contracts/
|     |  ├─ DTOs /
│     │  |   ├─ DamageableEntityDto.cs
│     │  |   ├─ EntityInventoryDto.cs
│     │  |   ├─ EntitySkillsDto.cs
│     │  |   ├─ Item.cs
│     │  |   └─ SkillDto.cs
|     │  ├─ Requests/
|     │  │  ├─ EntitiesRequests.cs
|     │  │  ├─ CombatRequests.cs            # TODO
|     │  │  ├─ InventoryRequests.cs
|     │  │  └─ SkillsRequests.cs
|     │  └─ Mappers/
|     │     └─ EntityMapper.cs
│     │
│     ├─ Infrastructure/
│     │  └─ EntityStore.cs                   # UPDATE: In-memory repository/store
│     |  └─ Repositories/                    # TODO (optional) in place of EntityStore.cs
│     |     ├─ IEntityRepository.cs
│     |     └─ InMemoryEntityRepository.cs
│     │
│     └─ Realtime/                           # TODO (optional)
│        └─ BattleHub.cs                     # TODO: SignalR for live updates
│
├─ package.json
├─ tsconfig.json
└─ README.md
```

Notes:
- Controllers should validate input and delegate only.
- Business rules should live in Application services and Domain classes.
- Repository interfaces allow swapping in-memory storage for EF Core later.

### Example Methods by Layer

**Controllers** (HTTP transport only):
- `GetEntity(id)` -> calls `IEntityService.GetById(id)`
- `PostDamage(id, request)` -> calls `ICombatService.TakeDamage(...)`
- `PostInventoryItem(id, request)` -> calls `IInventoryService.AddItem(...)`
- `DeleteSkill(id, skillId)` -> calls `ISkillService.RemoveSkill(...)`

**Application Services** (use-case orchestration):
- `EntityService.GetById(id)` -> Repository query + DTO mapping
- `EntityService.CreateEntity(request)` -> validates request + creates domain entity + saves
- `CombatService.TakeDamage(entity, source, amount, type)` -> applies resistances + handles death logic + logs
- `InventoryService.AddItem(entity, item)` -> validates capacity + updates inventory + returns updated state
- `SkillService.LearnSkill(entity, skillId)` -> resolves skill from catalog + adds to entity + returns updated skills

**Domain Entities** (game rules):
- `DamageableEntity.Heal(source, amount)` -> applies healing resistance + updates health (no side effects)
- `DamageableEntity.GetResistance(damageType)` -> returns resistance multiplier for type
- `EntityInventory.AddItem(item)` -> adds to collection (pure data mutation)
- `EntityInventory.RemoveItemById(id)` -> removes from collection + throws if not found
- `EntitySkills.HasSkill(skill)` -> checks by ID or name

**Infrastructure/Repository** (data access):
- `IEntityRepository.GetById(id)` -> returns entity or null
- `IEntityRepository.Add(entity)` -> stores in memory or database
- `IEntityRepository.Remove(id)` -> deletes entity
- `IEntityRepository.GetAll()` -> returns all entities (for listing/debugging)

**Contracts** (DTOs and mappers):
- `EntityMapper.ToDto(entity)` -> converts domain entity to API response DTO
- `EntityMapper.ToDtos(entities)` -> batch convert for list endpoints
- `ChangeHealthRequest` -> validated input model for combat endpoint
- `AddItemByIdRequest` -> validated input model for inventory endpoint
