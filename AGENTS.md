# AGENTS.md — Bonobo Engine

MonoGame 3.8.5 (WindowsDX12) + **vendored** Arch ECS 2.1.0, C# 12 on .NET 10. Early-stage engine repo: the game project is still the Arch sample scene. Human-oriented design docs live in `docs/` (227 files); agent-oriented distillations live in `docs/ai-agents/`.

## Read first

- `CLAUDE.md` — game-dev generation rules (what AI may/may not generate). Still authoritative; some file paths in it were stale and are fixed in `docs/ai-agents/codebase-truth.md`.
- `docs/ai-agents/codebase-truth.md` — verified facts that override anything the human docs imply.
- Skills: `bonobo-ecs-authoring`, `bonobo-monogame-lifecycle`, `gamedev-doc-retrieval`, `game-scope-guardian` under `.opencode/skills/`.

## Build status: BROKEN (pre-existing, verified 2026-07-26)

`dotnet build` fails with exactly 5 errors, all in `BonoboGame.Core.Dx12`:

1. `Game.cs(39)` CS0234 — `Systems.Group<GameTime>` doesn't resolve. `Group<T>` lives in namespace `Arch.Systems` (see `Arch/Arch.Systems/Systems.cs`); use `Arch.Systems.Group<GameTime>`.
2. `Systems.cs(198,217)` CS0246 — `[Event]` unknown. `EventAttribute` is emitted only by the Arch.EventBus **source generator**, which does not run through the plain `ProjectReference` to `Arch.csproj` (it normally ships as a NuGet analyzer asset). Same limitation applies to `[Query]`-generated `*Query` methods — do not add new code that depends on source-generated ECS members until the generators are wired as analyzers (or write manual `QueryDescription` loops instead).

Do not "fix" these silently as a side effect of unrelated work; surface them to the user first.

## Commands (verified)

```powershell
dotnet build bonoboengine-dx12.slnx          # build all 4 projects (currently fails, see above)
dotnet run --project BonoboGame.Dx12         # run the game (after build is fixed)
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
| `docs/` | Human-oriented knowledge base (see `docs/ai-agents/doc-map.md`) | `docs/2d-games/examples/**.cs` are reference snippets, NOT compiled — copying them in must adapt to the real API |
| `openspec/` | Spec-driven change workflow (`.opencode/commands/opsx-*`) | Use for planned features |

## Verified toolchain quirks

- **MonoGame platform is `WindowsDX12`** (`MonoGame.Runtime.Windows.DX12` 3.8.5) — not DesktopGL/DX11 that most guides and training data assume. Runtime-only packages use `PrivateAssets=all` in Core.
- Arch has extra build configurations `Debug-PureECS`/`Release-PureECS`/`Debug-Events`/`Release-Events` defining `PURE_ECS`/`EVENTS`, which **change the API shape** (`_world.Set(entity,...)` vs `entity.Set(...)` — see `#if` branches in `Game.cs`). Plain `Debug`/`Release` use the extension-method API.
- Arch.csproj sets `EmitCompilerGeneratedFiles` and contains T4 `.tt` templates that generate most of `World.*`/`Chunk.*` APIs — many Arch APIs exist 25+ times as overloads; check the generated `.cs` next to each `.tt` before assuming a signature.
- Assets go through `Content/Content.mgcb` (MGCB). Never raw-copy assets into output; never `File.Read*` game content.
- No `global.json`, `Directory.Build.props`, `.editorconfig`, or formatter config — follow existing file style (4-space indent, XML doc comments on public members, one concern per file).

## Stack reality vs. docs

The csproj files are the only truth: today the stack is **MonoGame DX12 + vendored Arch** (+ its transitive deps: MessagePack, Utf8Json, ZeroAllocJobScheduler, Collections.Pooled, CommunityToolkit.HighPerformance). Gum, FontStashSharp, MonoGame.Extended, Apos.Input, Aether.Physics2D, BrainAI appear in `CLAUDE.md`/`docs/index.md`/`R1_library_stack.md` as the **planned** stack — none are referenced yet. Don't generate code against libraries that aren't installed; propose adding the package first.

## Working rules (from CLAUDE.md, condensed)

- Components = pure data structs; systems = pure logic. Never mix.
- Never auto-generate game-feel code (jump arcs, attack timing, camera behavior, screen shake) — flag it for hand-writing.
- Build after every change; review generated code for hallucinated APIs, LINQ in hot paths, swallowed exceptions.
- No entity structural changes (create/destroy/add/remove components) during query iteration — do it outside the loop.
- Don't add features without a scope check (`game-scope-guardian` skill). Big changes go through the openspec workflow.
