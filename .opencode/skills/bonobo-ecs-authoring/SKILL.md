---
name: bonobo-ecs-authoring
description: Create or modify ECS components, systems, queries, or world/entity code in the Bonobo Engine repo (vendored Arch 2.1.0 via project reference). USE FOR: adding a component struct, writing a new BaseSystem, changing query logic, entity creation/destruction, component add/remove, save/load serialization of the world, EventBus messaging. DO NOT USE FOR: MonoGame lifecycle/content/rendering work (use bonobo-monogame-lifecycle), deciding whether a feature should exist (use game-scope-guardian), or generic C# refactors with no ECS surface.
---

# Bonobo ECS Authoring

Load these files first (they contain the enforced rules and the current build state):

1. `docs/ai-agents/codebase-truth.md` — build is green; source generators (`[Query]`→`*Query`, `[Event]`) run via the `Arch.Generators` analyzer reference.
2. `docs/ai-agents/ecs-authoring.md` — signatures, patterns, checklists. Follow it exactly.

## Hard rules (summary — details in ecs-authoring.md)

- Components = pure data structs in `BonoboGame.Core.Dx12/Components/`. No methods, no logic.
- Systems inherit `Arch.Systems.BaseSystem<World, GameTime>`, live in `BonoboGame.Core.Dx12/Systems/`.
- `[Query]`/`[Event]` source generation works (via the `Arch.Generators` analyzer); **manual `QueryDescription` + `World.Query` loops** are equally valid. Use either; don't mix blindly.
- `Group<T>` is `Arch.Systems.Group<GameTime>` (not `Systems.Group` — that resolves to the game namespace and fails to compile).
- No structural changes (create/destroy/add/remove) inside query iteration; collect then mutate after the loop.
- `ref` on every component parameter of query lambdas; arity must match `WithAll<...>`.
- Default build config only — do not add `#if PURE_ECS`/`EVENTS` branches.
- Never modify the vendored `Arch/` sources; use its API.
- Serialization: runtime/GPU objects get `[IgnoreDataMember]` + an id field + a custom formatter in `Serializers/` (pattern: `SpriteSerializer`).

## Before finishing

- Run `dotnet build bonoboengine-dx12.slnx`. Expected: **0 errors**. Report new errors immediately; do not work around them silently.
- Run the review checklist at the bottom of `ecs-authoring.md`.
