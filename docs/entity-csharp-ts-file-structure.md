# Proposed File Structure (C# Backend + TypeScript Frontend)

Below is a practical structure showing how to add an ASP.NET Core backend alongside your current Next.js app.

```txt
text_dungeon_crawler/
├─ docs/
│  ├─ entity-csharp-ts-draft.md
│  └─ entity-csharp-ts-file-structure.md
│
├─ src/                                      # Existing Next.js frontend
│  ├─ app/
│  │  ├─ anew/
│  │  │  └─ page.tsx
│  │  └─ ...
│  ├─ components/
│  │  └─ ...
│  ├─ context/
│  │  └─ gameContext.tsx
│  ├─ lib/
│  │  ├─ entityApiClient.ts                  # NEW: frontend API calls to C# backend
│  │  └─ ...
│  ├─ types/
│  │  └─ entityApi.ts                        # NEW: DTO types matching backend JSON
│  └─ ...
│
├─ backend/                                  # NEW: ASP.NET Core Web API
│  ├─ GameServer.sln
│  └─ GameServer/
│     ├─ GameServer.csproj
│     ├─ Program.cs                          # CORS + DI + controllers
│     ├─ appsettings.json
│     ├─ appsettings.Development.json
│     │
│     ├─ Api/
│     │  └─ EntitiesController.cs            # GET/POST endpoints for Entity
│     │
│     ├─ Domain/
│     │  ├─ Entity.cs                        # Core entity logic (hide/reveal/speed/etc.)
│     │  ├─ EntityInventory.cs
│     │  └─ EntitySkills.cs
│     │
│     ├─ Contracts/
│     │  ├─ EntityDto.cs                     # API output models
│     │  ├─ InventoryDto.cs
│     │  ├─ SkillsDto.cs
│     │  ├─ SetSpeedRequest.cs               # API input model
│     │  └─ EntityMapper.cs                  # Domain <-> DTO mapping
│     │
│     ├─ Infrastructure/
│     │  └─ EntityStore.cs                   # In-memory repository/store (starter)
│     │
│     └─ Realtime/                           # OPTIONAL later
│        └─ BattleHub.cs                     # SignalR for live updates
│
├─ package.json
├─ tsconfig.json
└─ README.md
```

## Minimal First Pass

If you want the leanest setup first, create only these new files:

```txt
backend/GameServer/Program.cs
backend/GameServer/Api/EntitiesController.cs
backend/GameServer/Domain/Entity.cs
backend/GameServer/Contracts/EntityDto.cs
backend/GameServer/Contracts/SetSpeedRequest.cs
backend/GameServer/Contracts/EntityMapper.cs
backend/GameServer/Infrastructure/EntityStore.cs
src/types/entityApi.ts
src/lib/entityApiClient.ts
```

Then expand into separate files (`EntityInventory.cs`, `EntitySkills.cs`, etc.) as complexity grows.
