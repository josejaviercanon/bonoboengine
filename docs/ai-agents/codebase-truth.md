# Codebase Truth (verified facts)

Facts below were verified by reading the csproj/slnx/source and running builds on 2026-07-26. When this file and any prose doc disagree, **this file wins**.

## Stack reality

- .NET SDK **10.0.302**; projects target `net10.0` (Core, Arch, PipelineExtensions) and `net10.0-windows` (game host). C# 12 / `LangVersion latest` (Arch).
- MonoGame **3.8.5**, platform **`WindowsDX12`** (`MonoGame.Framework.Native` + `MonoGame.Runtime.Windows.DX12`, plus `MonoGame.Content.Builder.Task` in the host). Not DesktopGL, not DX11.
- ECS: **Arch 2.1.0 vendored as source** in `Arch/` (project reference, not NuGet). Transitive deps: MessagePack 3.1.8, Utf8Json 1.3.7, ZeroAllocJobScheduler 1.1.2, Collections.Pooled, CommunityToolkit.HighPerformance, Microsoft.Extensions.ObjectPool.
- **Not installed** (despite appearing in docs/CLAUDE.md): Gum, FontStashSharp, MonoGame.Extended, Apos.Input, Aether.Physics2D, BrainAI, ImGui.NET, LDtk/Tiled loaders. Do not write code against them; propose adding the package first.

## Solution & projects

`bonoboengine-dx12.slnx` (XML format; needs .NET 10 SDK):

| Project | Role | Notes |
|---|---|---|
| `BonoboGame.Dx12` | WinExe host | `Program.cs` only; `MonoGamePlatform=WindowsDX12`; references Core |
| `BonoboGame.Core.Dx12` | Game library | `Game.cs` (still the Arch sample scene), `Components/`, `Systems/`, `Extensions/`, `Serializers/`, `Content/Content.mgcb` |
| `BonoboGame.PipelineExtensions.Dx12` | MGCB stubs | `Importer1.cs`/`Processor1.cs` template placeholders |
| `Arch/Arch.csproj` | Vendored ECS | Single csproj compiling Core+EventBus+Persistence+Relationships+Systems+LowLevel+generators+T4 output |

The `/Tests/` solution folder is **empty** — there is no test project and `dotnet test` does nothing.

## Build state: broken (pre-existing)

`dotnet build bonoboengine-dx12.slnx` fails with 5 errors:

1. `Game.cs(39)` CS0234: `Systems.Group<GameTime>` — resolves against the `BonoboGame.Core.Dx12.Systems` namespace, which has no `Group`. The real type is `Arch.Systems.Group<T>` (`Arch/Arch.Systems/Systems.cs:89`).
2. `Systems.cs(198,217)` CS0246 ×4: `[Event]` / `EventAttribute` unknown. It is only emitted by the `Arch.EventBus` incremental source generator (`Arch/Arch.EventBus/SourceGenerator.cs`), which **does not execute** for consumers of a plain `ProjectReference`. (Via NuGet it ships as an analyzer asset.)

Consequence: `[Query]` attribute compiles (it's a real type in `Arch/Arch.Systems/Attributes.cs`) but its **generated `*Query` methods and `Update` overrides are not produced** in this setup. New code must use manual `QueryDescription` + `World.Query` loops, or the generators must first be wired as analyzers. Fix direction options: reference the generators with `OutputItemType="Analyzer"`, switch ECS refs to the official NuGet packages, or remove EventBus/source-gen usage from game code.

## Arch quirks that bite

- Extra configurations `Debug-PureECS`, `Release-PureECS`, `Debug-Events`, `Release-Events` define `PURE_ECS`/`EVENTS`. With `PURE_ECS`, entity extension methods vanish — use `_world.Set(entity, …)` instead of `entity.Set(…)` (see `#if` branches in `Game.cs`). Default `Debug`/`Release` = extension-method API.
- Most `World`/`Chunk`/`Entity` APIs are generated from T4 templates (`Arch/Templates/*.tt` → adjacent `.cs`). Expect ~25 overloads per operation; check the generated `.cs` before assuming a signature.
- `EmitCompilerGeneratedFiles` is on for Arch; MessagePack generator output lands in `Arch/obj/**`.
- Serialization in the sample: `ArchBinarySerializer` + custom `SpriteSerializer` (`Serializers/`) bridges `Texture2D` (marked `[IgnoreDataMember]`) via a `TextureId`. The JSON path is commented out in `Game.cs` ("Serializer is not updated yet").

## Repo hygiene state (2026-07-26)

- `git log`: 2 commits; **all code/docs are uncommitted** on `main`.
- `.vscode/launch.json` is stale (`BonoboGamedx12\BonoboGamedx12.csproj` doesn't exist). Use `dotnet run --project BonoboGame.Dx12`.
- No `global.json`, `Directory.Build.props`, `.editorconfig`, formatter or analyzer config anywhere.
- CLAUDE.md references `docs/engine_toolkit/`, `CONTEXT.md`, `DESIGN.md`, `FUTURE_IDEAS.md` — **none exist** in this repo.

## Docs corpus

- 227 markdown files under `docs/`, human-oriented. `docs/monogame-arch/`, `docs/game-development/`, and `docs/2d-games/` overlap heavily (the `G*` guides and playbook files are duplicated across them).
- `docs/2d-games/examples/` holds 27 standalone `.cs` reference snippets (character controller, tilemap, A*, fog of war, tween, procgen…) — **not compiled**, assume API drift.
- Retrieval index: `docs/ai-agents/doc-map.md`.
