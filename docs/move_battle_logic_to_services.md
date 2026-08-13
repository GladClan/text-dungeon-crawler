## Plan: Move battle orchestration to services

The current `/Domain/Battle/` folder mixes three different responsibilities: battle state storage, battle rule execution, and battle analytics. The code that coordinates other application services and session lifecycle should move out of the domain folder into `Application/Services`, while the pure effect rules should stay in domain.

**Steps**
1. Move `BattleTracker` into a dedicated application service, or turn it into a thin domain state holder with a new `BattleService` wrapper in `Application/Services`. The move candidate includes the constructor dependencies on `StatisticsTracker` and `EntityService`, turn/round progression, battle start/end orchestration, party lookup helpers, continuous-effect management, and entity lookup helpers. This is the highest-priority move because the current domain type directly depends on an application service, which violates the layering boundary.
2. Move the battle-log summary/query methods out of `BattleLog` into a service that computes results from log entries. The move candidates are the analytics-style methods: `GetHighestSingleDamage`, `GetHasAttackedSource`, `GetHighestDamageSent`, `GetMostDamage`, `GetMostHealer`, `GetFirstFatalDamage`, `GetMagicUser`, `GetUsedMagicMost`, and the unfinished `GetRecentActionsFromSource`. Keep `AddEntry` and the entry list in the log itself.
3. Keep `IBattleEffect`, `StatBuffEffect`, `StatusEffect`, and `BattleLogEntry` in the domain folder for now. These types encode battle rules by mutating `DamageableEntity` state directly, so they are domain mechanics rather than orchestration or reporting.
4. Update the application-facing callers to depend on the new service boundary. The main call sites are `EventServices`, `CombatService`, and AI/skill/item code that currently receives `BattleTracker`. If needed, preserve a small facade or interface so `Skill`, `Useable`, and AI code do not all need immediate signature churn.
5. Decide whether the new service should return DTOs, entity ids, or domain objects. The recommended shape is a service that keeps orchestration in `Application/Services` and returns existing DTOs or primitive ids for readouts, while leaving the log and effects as domain-owned data and behavior.

**Relevant files**
- `backend/GameServer/Domain/Battle/BattleTracker.cs` - main orchestration candidate; currently depends on `EntityService` and manages battle lifecycle.
- `backend/GameServer/Domain/Battle/BattleLog.cs` - split between append-only log state and summary queries.
- `backend/GameServer/Domain/Battle/IBattleEffect.cs` - should stay domain-owned as the effect contract.
- `backend/GameServer/Domain/Battle/StatBuffEffect.cs` - should stay domain-owned as battle rule execution.
- `backend/GameServer/Domain/Battle/StatusEffect.cs` - should stay domain-owned as battle rule execution.
- `backend/GameServer/Application/Services/EventServices.cs` - current battle lifecycle owner and likely the first caller to switch.
- `backend/GameServer/Application/Services/CombatService.cs` - passes the battle context through to item/skill effects.
- `backend/GameServer/Domain/Skills/Skill.cs` and `backend/GameServer/Domain/Items/Useable.cs` - effect entry points that currently depend on `BattleTracker`.
- `backend/GameServer/Domain/Entity/EntityAI/AILibrary.cs/InitialRelease/EntityAIInitialRelease.cs` - AI logic currently consumes battle-log queries.
- `backend/GameServer/Domain/Statistics/StatisticsTracker.cs` - end-of-battle aggregation target that is already application-facing in practice.

**Verification**
1. Rebuild the mental dependency graph after the move and confirm no domain file depends on `Application.Services` types.
2. Check the battle-start and battle-end call chain in `EventServices` to confirm the new service owns session lifecycle cleanly.
3. Confirm any code that needs battle summaries can still get the same ids or DTOs without reading from the domain log directly.

**Decisions**
- `BattleTracker` is the clearest candidate to move because it already acts like a session service rather than a domain entity.
- `BattleLog` should be split logically rather than physically if the team wants minimal churn: keep storage in the log object, move summary queries to a service.
- The effect classes should stay where they are unless you also want to redesign how continuous effects are applied.
- This plan intentionally excludes frontend changes and excludes redesigning the battle rules themselves.

## Recommended Move Plan

Move the orchestration-heavy parts of `BattleTracker.cs` into `Application/Services`, ideally as a `BattleService` or a relocated `BattleTracker` service. The parts to move are the constructor dependency on each of:

- `EntityService`
- `NextTurn`
- `OnBattleEnd`
- `AddEntityToBattle`
- `GetPartyIds`
- `GetEntity`
- `ExistsPartyMemberAtCriticalHealth`
- `GetPartyMemberAtCriticalHealth`
- `AddContinuousEffect`
- `RemoveContinuousEffect`
- `RemoveAllContinuousEffects`
- the initiative/round bookkeeping

This code is application orchestration, not pure domain logic, because it coordinates other services, owns battle-session lifecycle, and crosses aggregate boundaries.

Move the summary/query methods out of `BattleLog.cs` into a service that reads battle history, such as `BattleLogService` or `BattleAnalyticsService`. The best candidates are:

- `GetHighestSingleDamage`
- `GetHasAttackedSource`
- `GetHighestDamageSent`
- `GetMostDamage`
- `GetMostHealer`
- `GetFirstFatalDamage`
- `GetMagicUser`
- `GetUsedMagicMost`
- `GetRecentActionsFromSource`

Keep `AddEntry` and the entry storage in `BattleLog`. Those query methods are read-model computations over historical data, so they fit a service better than a mutable domain object.

Keep `IBattleEffect.cs`, `StatBuffEffect.cs`, `StatusEffect.cs`, and the `BattleLogEntry` type in the domain layer. These types directly mutate `DamageableEntity` and encode battle rules, so they are combat mechanics rather than orchestration or reporting.

Update the callers that currently depend on the battle object as a domain type, especially `EventServices.cs`, `CombatService.cs`, `Skill.cs`, `Useable.cs`, and the AI logic in `AILibrary.cs`. The cleanest path is to keep their public behavior the same while swapping the underlying battle dependency to a service or facade.

### Why the boundary changes

In the domain layer, code should be about rules and state transitions. In the service layer, code can orchestrate multiple collaborators, reach into `EntityService` and `StatisticsTracker`, and manage the lifetime of a battle session.

Moving the analytics methods out of `BattleLog` changes them from object methods on mutable state into read operations over stored entries. That makes them easier to test, easier to reuse, and less tied to one specific battle instance.

Leaving the effect classes in the domain keeps the actual combat math and state mutation close to the entity model, which is where they belong. Only the coordination around when those effects are applied should move.

