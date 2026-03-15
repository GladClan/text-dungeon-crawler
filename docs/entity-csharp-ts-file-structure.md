# File Structure (C# Backend + TypeScript Frontend)

Current state of the project structure. Items marked **TODO** do not yet exist and are planned for future implementation.
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
│     ├─ appsettings.json                    # TODO
│     ├─ appsettings.Development.json        # TODO
│     │
│     ├─ Api/                                # TODO
│     │  └─ EntitiesController.cs            # TODO: GET/POST endpoints for Entity
│     │
│     ├─ Domain/
│     │  ├─ Entity/
│     │  │  ├─ DamageableEntity.cs
│     │  │  ├─ Entity.cs
│     │  │  ├─ EntityAI.cs                  # TODO: Algorithm base for decision-making
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
│     ├─ Contracts/                          # TODO
│     │  ├─ EntityDto.cs                     # TODO: API output models
│     │  ├─ InventoryDto.cs                  # TODO
│     │  ├─ SkillsDto.cs                     # TODO
│     │  ├─ SetSpeedRequest.cs               # TODO: API input model
│     │  └─ EntityMapper.cs                  # TODO: Domain <-> DTO mapping
│     │
│     ├─ Infrastructure/                     # TODO
│     │  └─ EntityStore.cs                   # TODO: In-memory repository/store
│     │
│     └─ Realtime/                           # TODO (optional)
│        └─ BattleHub.cs                     # TODO: SignalR for live updates
│
├─ package.json
├─ tsconfig.json
└─ README.md
```

## Still TODO

Next files to create for the backend API layer:

```txt
backend/GameServer/Program.cs
backend/GameServer/appsettings.json
backend/GameServer/appsettings.Development.json
backend/GameServer/Api/EntitiesController.cs
backend/GameServer/Contracts/EntityDto.cs
backend/GameServer/Contracts/InventoryDto.cs
backend/GameServer/Contracts/SkillsDto.cs
backend/GameServer/Contracts/SetSpeedRequest.cs
backend/GameServer/Contracts/EntityMapper.cs
backend/GameServer/Infrastructure/EntityStore.cs
src/types/entityApi.ts
src/entityApiClient.ts
```
