# Codebase Truth (verified facts)

Facts below were verified by reading the csproj/slnx/source and running builds on 2026-07-26. When this file and any prose doc disagree, **this file wins**.

## Stack reality

- .NET SDK **10.0.302**; projects target `net10.0` (Core, Arch, PipelineExtensions) and `net10.0-windows` (game host). C# 12 / `LangVersion latest` (Arch).
- MonoGame **3.8.5**, platform **`WindowsDX12`** (`MonoGame.Framework.Native` + `MonoGame.Runtime.Windows.DX12`, plus `MonoGame.Content.Builder.Task` in the host). Not DesktopGL, not DX11.
- ECS: **Arch 2.1.0 vendored as source** in `Arch/` (project reference, not NuGet). Transitive deps: MessagePack 3.1.8, Utf8Json 1.3.7, ZeroAllocJobScheduler 1.1.2, Collections.Pooled, CommunityToolkit.HighPerformance, Microsoft.Extensions.ObjectPool.
- **Not installed** (despite appearing in `docs/index.md` as the planned stack): Gum, FontStashSharp, MonoGame.Extended, Apos.Input, Aether.Physics2D, BrainAI, ImGui.NET, LDtk/Tiled loaders. Do not write code against them; propose adding the package first.

## Solution & projects

`bonoboengine-dx12.slnx` (XML format; needs .NET 10 SDK):

| Project | Role | Notes |
|---|---|---|
| `BonoboGame.Dx12` | WinExe host | `Program.cs` only; `MonoGamePlatform=WindowsDX12`; references Core |
| `BonoboGame.Core.Dx12` | Game library | `Game.cs` (still the Arch sample scene), `Components/`, `Systems/`, `Extensions/`, `Serializers/`, `Content/Content.mgcb` |
| `BonoboGame.PipelineExtensions.Dx12` | MGCB stubs | `Importer1.cs`/`Processor1.cs` template placeholders |
| `Arch/Arch.csproj` | Vendored ECS | Single csproj compiling Core+EventBus+Persistence+Relationships+Systems+LowLevel+generators+T4 output |
| `Arch.Generators/Arch.Generators.csproj` | Analyzer packaging | netstandard2.0; links the vendored generator sources into a lean Roslyn analyzer assembly; referenced by Core as analyzer only |

The `/Tests/` solution folder is **empty** — there is no test project and `dotnet test` does nothing.

## Build state: GREEN (fixed 2026-07-26)

`dotnet build bonoboengine-dx12.slnx` succeeds with 0 errors, verified from a fully clean tree (all `bin`/`obj` deleted). The 5 pre-existing errors were fixed as follows:

1. `Game.cs` used `Systems.Group<GameTime>`, which resolved against the `BonoboGame.Core.Dx12.Systems` namespace. Fixed to `Arch.Systems.Group<GameTime>` (`Arch/Arch.Systems/Systems.cs:89`).
2. `[Event]` / `EventAttribute` (and the whole source-gen layer) did not exist because the Arch generators only run when referenced as an **analyzer**, not via a plain `ProjectReference`. Fixed by the new `Arch.Generators/Arch.Generators.csproj` (netstandard2.0, Roslyn-only deps) which links the generator sources from `Arch/Arch.EventBus/`, `Arch/Arch.Systems.SourceGenerator/`, `Arch/Arch.AOT.SourceGenerator/`, `Arch/Pollyfilling/`; the game references it with `OutputItemType="Analyzer" ReferenceOutputAssembly="false"`.
   - Referencing `Arch.dll` itself as an analyzer does **not** work: Roslyn's analyzer load context cannot resolve Arch.dll's runtime dependencies (MessagePack, Utf8Json, …) while scanning its types, and csc dies with "Could not load file or assembly 'MessagePack'". Loading those dependency DLLs as analyzers too fixes that but breaks on clean builds (MSBuild item globs evaluate before Arch's `bin` exists) — hence the lean project instead.
   - The vendored query generator's attribute filter was `Arch.System.QueryAttribute` (singular) while the vendored attributes live in `Arch.Systems` — fixed in `Arch/Arch.Systems.SourceGenerator/SourceGenerator.cs`. `[Data]`/`[All]`/`[None]` detection is short-name based and was never broken.

Consequence: `[Query]`-generated `*Query` methods + `Update` overrides and `[Event]`-generated `EventBus.Send`/`Hook()`/`Unhook()` are produced for game code. The query generator skips emitting `Update` when a system already overrides it (`DebugSystem`). Expected warnings: 3× CS0436 (source-generated `Arch.Bus.EventBus` class shadows the compiled generator-model struct in Arch.dll — intended, source wins). Manual `QueryDescription` loops remain equally valid.

## Arch quirks that bite

- Extra configurations `Debug-PureECS`, `Release-PureECS`, `Debug-Events`, `Release-Events` define `PURE_ECS`/`EVENTS`. With `PURE_ECS`, entity extension methods vanish — use `_world.Set(entity, …)` instead of `entity.Set(…)` (see `#if` branches in `Game.cs`). Default `Debug`/`Release` = extension-method API.
- Most `World`/`Chunk`/`Entity` APIs are generated from T4 templates (`Arch/Templates/*.tt` → adjacent `.cs`). Expect ~25 overloads per operation; check the generated `.cs` before assuming a signature.
- `EmitCompilerGeneratedFiles` is on for Arch; MessagePack generator output lands in `Arch/obj/**`.
- Serialization in the sample: `ArchBinarySerializer` + custom `SpriteSerializer` (`Serializers/`) bridges `Texture2D` (marked `[IgnoreDataMember]`) via a `TextureId`. The JSON path is commented out in `Game.cs` ("Serializer is not updated yet").

## Repo hygiene state (2026-07-26)

- `git log`: 2 commits; **all code/docs are uncommitted** on `main`.
- `.vscode/launch.json` is stale (`BonoboGamedx12\BonoboGamedx12.csproj` doesn't exist). Use `dotnet run --project BonoboGame.Dx12`.
- No `global.json`, `Directory.Build.props`, `.editorconfig`, formatter or analyzer config anywhere.
- `AGENTS.md` (which absorbed `CLAUDE.md`) references `CONTEXT.md`, `DESIGN.md`, `FUTURE_IDEAS.md` — **none exist** in this repo. `CLAUDE.md` itself was removed on 2026-07-26; its game-dev rules now live in `AGENTS.md` § Game Development Rules.

## Docs corpus

- 227 markdown files under `docs/`, human-oriented. `docs/monogame-arch/`, `docs/game-development/`, and `docs/2d-games/` overlap heavily (the `G*` guides and playbook files are duplicated across them).
- `docs/2d-games/examples/` holds 27 standalone `.cs` reference snippets (character controller, tilemap, A*, fog of war, tween, procgen…) — **not compiled**, assume API drift.
- Retrieval index: `docs/ai-agents/doc-map.md`.
