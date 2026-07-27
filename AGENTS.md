# AGENTS.md — Bonobo Engine

MonoGame 3.8.5 (WindowsDX12) + **vendored** Arch ECS 2.1.0, C# 12 on .NET 10. Early-stage engine repo: the game project is still the Arch sample scene. Human-oriented design docs live in `docs/` (227 files); agent-oriented distillations live in `docs/ai-agents/`.

> **Stack & architecture source of truth:** `docs/index.md` (kept current). Verified-by-build facts: `docs/ai-agents/codebase-truth.md` (this file wins over prose when they disagree). This `AGENTS.md` holds build state, repo layout, and the game-dev generation rules (formerly in `CLAUDE.md`, now merged here).

## Read first

- `docs/index.md` — the stack, architecture, and project structure (source of truth).
- `docs/ai-agents/codebase-truth.md` — verified facts; overrides anything human docs imply.
- Skills: `bonobo-ecs-authoring`, `bonobo-monogame-lifecycle`, `gamedev-doc-retrieval`, `game-scope-guardian` under `.opencode/skills/`.

## Build status: GREEN (fixed 2026-07-26)

`dotnet build bonoboengine-dx12.slnx` succeeds with 0 errors (verified from a fully clean tree). The Arch source generators (`[Query]`, `[Event]`) are wired and functional:

- `Arch.Generators/Arch.Generators.csproj` (netstandard2.0) links the generator sources from the vendored `Arch/Arch.EventBus/`, `Arch/Arch.Systems.SourceGenerator/`, `Arch/Arch.AOT.SourceGenerator/`, and `Arch/Pollyfilling/` folders into a lean Roslyn analyzer assembly. `BonoboGame.Core.Dx12` references it with `OutputItemType="Analyzer" ReferenceOutputAssembly="false"`.
- **Never** reference `Arch.dll` itself as an analyzer: Roslyn's analyzer load context cannot resolve its runtime dependencies (MessagePack, Utf8Json, …) during generator discovery and csc crashes with "Could not load file or assembly 'MessagePack'". The lean project exists precisely to avoid that.
- Only vendored-Arch modification: `Arch/Arch.Systems.SourceGenerator/SourceGenerator.cs` `[Query]` filter string `Arch.System.QueryAttribute` → `Arch.Systems.QueryAttribute` (the vendored attributes live in namespace `Arch.Systems`; without this `[Query]` silently generates nothing).
- Expected warning noise: 3× CS0436 (generated `Arch.Bus.EventBus` class shadows the compiled generator-model `EventBus` struct in Arch.dll — intended, source wins) and NU1903 on the PipelineExtensions stub (pre-existing).
- Source-generated ECS members (`[Query]` → `*Query` methods + `Update` overrides, `[Event]` → `EventBus.Send`/`Hook()`) may be used in game code; manual `QueryDescription` loops remain equally valid.

## Commands (verified)

```powershell
dotnet build bonoboengine-dx12.slnx          # build all 5 projects (green, see above)
dotnet run --project BonoboGame.Dx12         # run the game
dotnet publish -c Release -o ./publish BonoboGame.Dx12
```

- Requires **.NET SDK 10** (10.0.302 present). The solution is the XML `.slnx` format — older SDKs/VS can't open it.
- `dotnet test` is a **no-op**: no test projects exist (the slnx `/Tests/` folder is empty). Don't claim tests pass.
- `.vscode/launch.json` is stale (points to nonexistent `BonoboGamedx12\`). Use `dotnet run` instead.

## Layout & boundaries

| Path | What it is | Rule |
|---|---|---|
| `BonoboGame.Dx12/` | Thin WinExe host (`Program.cs` only) | Platform host; no game logic here |
| `BonoboGame.Core.Dx12/` | Game code: `Game.cs`, `Components/`, `Systems/`, `Extensions/`, `Serializers/`, `Content/Content.mgcb` | Where nearly all work happens |
| `BonoboGame.PipelineExtensions.Dx12/` | Empty MGCB importer/processor stubs | Stub; touch only for content-pipeline work |
| `Arch/` | **Vendored Arch ECS 2.1.0 full source** (Core, EventBus, Persistence, Relationships, Systems, LowLevel, 2 source generators, T4 templates) | Third-party code. Never refactor, reformat, or "fix style" here. Prefer using its API over modifying it |
| `Arch.Generators/` | Roslyn analyzer packaging of the Arch source generators (netstandard2.0; links the vendored generator sources) | Referenced by Core with `OutputItemType="Analyzer"` only; no runtime code, don't add any |
| `docs/` | Human-oriented knowledge base (see `docs/ai-agents/doc-map.md`); `docs/index.md` is the stack/architecture source of truth | `docs/2d-games/examples/**.cs` are reference snippets, NOT compiled — copying them in must adapt to the real API |
| `openspec/` | Spec-driven change workflow (`.opencode/commands/opsx-*`) | Use for planned features |

## Verified toolchain quirks

- **MonoGame platform is `WindowsDX12`** (`MonoGame.Runtime.Windows.DX12` 3.8.5) — not DesktopGL/DX11 that most guides and training data assume. Runtime-only packages use `PrivateAssets=all` in Core.
- Arch has extra build configurations `Debug-PureECS`/`Release-PureECS`/`Debug-Events`/`Release-Events` defining `PURE_ECS`/`EVENTS`, which **change the API shape** (`_world.Set(entity,...)` vs `entity.Set(...)` — see `#if` branches in `Game.cs`). Plain `Debug`/`Release` use the extension-method API.
- Arch.csproj sets `EmitCompilerGeneratedFiles` and contains T4 `.tt` templates that generate most of `World.*`/`Chunk.*` APIs — many Arch APIs exist 25+ times as overloads; check the generated `.cs` next to each `.tt` before assuming a signature.
- Assets go through `Content/Content.mgcb` (MGCB). Never raw-copy assets into output; never `File.Read*` game content.
- No `global.json`, `Directory.Build.props`, `.editorconfig`, or formatter config — follow existing file style (4-space indent, XML doc comments on public members, one concern per file).

## Stack reality vs. docs

`docs/index.md` is the source of truth for the stack and architecture. Today the stack is **MonoGame DX12 + vendored Arch** (+ its transitive deps: MessagePack, Utf8Json, ZeroAllocJobScheduler, Collections.Pooled, CommunityToolkit.HighPerformance). Gum, FontStashSharp, MonoGame.Extended, Apos.Input, Aether.Physics2D, BrainAI, ImGui.NET appear in `docs/index.md` and `R1_library_stack.md` as the **planned** stack — none are referenced yet. Don't generate code against libraries that aren't installed; propose adding the package first.

---

## Game Development Rules (formerly CLAUDE.md)

### Project Context

Architecture: MonoGame 3.8.5 (WindowsDX12) + Arch ECS 2.1.0 (vendored in `Arch/`). Planned, NOT yet installed: Gum UI, MonoGame.Extended, BrainAI, Apos.Input, FontStashSharp, ImGui.NET, Aether.Physics2D — verify against csproj before using any library.
Patterns: Service Locator for ambient services, vertical slice features, ECS components as pure data structs.
Conventions: C# 12, nullable enabled, readonly structs for data components, one class per file, files under 300 lines.
See: `docs/index.md` for the stack & architecture, `docs/ai-agents/` for agent guides, `docs/ai-agents/doc-map.md` for the full human documentation tree.

### AI Code Generation Rules

- ALWAYS write the interface/contract first, then ask for implementation.
- ALWAYS paste Arch ECS type signatures into context when generating ECS code — models lack MonoGame/Arch training data.
- NEVER generate game feel code (jump arcs, attack timing, camera behavior, screen shake) — write these by hand.
- NEVER trust AI-generated performance-critical inner loops without profiling.
- Components are pure data structs. Systems are pure logic functions. Keep them separated.
- Check every generated file against: hallucinated APIs, LINQ in hot paths, missing null/empty checks, swallowed exceptions.
- Spend 5 minutes reviewing for every 1 minute of generation.
- After generating ECS systems, verify: correct query components, proper command buffer usage, no entity structural changes during iteration.
- Build after every change; the build is green (see § Build status) — keep it at 0 errors.

### What AI Should Generate

- Component record structs from design descriptions.
- System scaffolding (query-iterate-transform boilerplate).
- Unit tests for deterministic systems (damage calc, state machines) — note: no test project exists yet; creating one is a scope decision, propose it first.
- XML doc comments and documentation.
- JSON data templates (level definitions, item databases, wave configs).
- Interface implementations and data models.
- Refactoring: extracting interfaces, splitting large classes.

### What AI Should NOT Generate

- Core game loop and fixed timestep integration.
- Physics/collision resolution edge cases.
- State machine transitions with subtle timing.
- Anything involving unique game feel.
- Shader hot paths without manual review.
- Architecture decisions — discuss these, don't auto-generate.

### Working rules (condensed)

- Components = pure data structs; systems = pure logic. Never mix.
- Never auto-generate game-feel code (jump arcs, attack timing, camera behavior, screen shake) — flag it for hand-writing.
- Review generated code for hallucinated APIs, LINQ in hot paths, swallowed exceptions.
- No entity structural changes (create/destroy/add/remove components) during query iteration — do it outside the loop.
- Don't add features without a scope check (`game-scope-guardian` skill). Big changes go through the openspec workflow.

### Art Pipeline Rules

- AI art is for exploration and rough drafts ONLY, never final assets.
- Workflow: hand sketch → img2img (0.7-0.8 denoise) → manual cleanup → lower denoise pass → final hand polish.
- Budget 50%+ of art time for manual refinement of any AI-assisted output.
- Train a custom LoRA (15-30 reference images) for style consistency across assets.
- Target specs: 16×16 tiles, 480×270 native resolution, 4× scaling, characters at 16×32.

### Project Management Rules

- Every feature must support one of the design pillars — if it doesn't, it goes in the Future Ideas doc.
- Use vertical slices: each 1-2 week sprint ends with a playable build.
- MoSCoW everything: Must Have → Should Have → Could Have → Won't Have.
- Multiply time estimates by 2-3×. Bug fixing consumes 30% of dev time.
- Reserve last 20-30% of development for polish/bugfix ONLY — no new features in this phase.
- NO new features without checking the scope doc first.

### Scope Control

- Hard deadlines with cuts, not delays.
- Keep a separate FUTURE_IDEAS.md — acknowledge cool ideas, defer them.
- The Polaris tiers: Essentials (without them game loses USP) → Baseline (minimum complete game) → Accessories (not needed to ship).
- If generating a new feature takes minutes with AI, that is MORE reason to scrutinize scope, not less.

### Task Structure

```
Design Pillars (3-5 statements) → what makes this game unique
  └── Milestones: Prototype → Demo → Early Access → Release
       └── Feature Categories: Core Mechanics | Content | UI/UX | Audio | Art | Systems
            └── Tasks: 1-4 hour units, Kanban flow (Backlog → To Do → In Progress → Done)
                 └── Bugs: P0 game-breaking | P1 major | P2 minor | P3 cosmetic
```

### Documentation

- Architecture Decision Records for every significant tech choice (date, context, decision, rationale).
- Weekly dev notes: what was done, blockers, next week plans.
- Update CONTEXT.md after every major architectural change.
- Document WHY for ECS component/system design decisions — AI will erode coherence without this record.

### File Boundaries

- Read `docs/index.md` + `docs/ai-agents/codebase-truth.md` for current project state (CONTEXT.md does not exist yet).
- Read `docs/ai-agents/doc-map.md` to locate architecture/design reference docs.
- `DESIGN.md` / `FUTURE_IDEAS.md` do not exist yet — create them when a concrete game project starts; until then, apply the scope rules above to every feature.
- NEVER modify DESIGN.md pillars without explicit human approval (once it exists).