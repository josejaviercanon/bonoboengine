# Bonobo Engine — The Stack

**This file is the source of truth for the engine's stack, architecture, and project structure.** It is maintained to match the repository; when code and prose disagree, this file (kept current) wins. Companion facts verified against the csproj/source live in `docs/ai-agents/codebase-truth.md`; build state and agent workflow live in `AGENTS.md`.

---

## The Stack

### Framework & Dependencies Overview

| Component | Library / Package | Type | Purpose |
| --- | --- | --- | --- |
| **Engine Core** | [`MonoGame.Runtime.Windows.DX12`](https://github.com/MonoGame/MonoGame) | External (NuGet) | Cross-platform windowing, graphics device, audio, and platform abstraction. |
| **ECS Architecture** | `Arch` / `Arch.System` | Custom (Arch.csproj) | High-performance archetype ECS for entity management and cache-friendly systems. |
| **UI Framework** | `Gum.MonoGame` | External (NuGet + Visual Tool) | Responsive HUD, menus, and layout engine with WYSIWYG editor support. |
| **Font Rendering** | `FontStashSharp.MonoGame` | External (NuGet) | Dynamic TTF runtime rasterization and glyph atlas management. |
| **Animation & Assets** | [`MonoGame.Extended`](https://www.nuget.org/packages/MonoGame.Extended/) | External (NuGet) | Utilities for sprite sheet animations, texture atlas parsing, cameras, and 2D asset management. |
| **Debug Tooling** | `ImGui.NET` | External (NuGet) | Immediate-mode developer console, entity inspectors, and performance profiling. |
| **Physics** | `Aether.Physics2D` | External (NuGet) or Custom AI | Full Box2D rigid-body simulation. *(Omit in favor of AI AABB for arcade/tight games)* |
| **Input System** | **Custom Engine Module** | AI-Generated (In-House) | Action-based polling wrapper (`IsActionPressed`, remapping) built over native MonoGame inputs. |
| **2D Camera** | **Custom Engine Module** | AI-Generated (In-House) | Matrix transformation (`TransformMatrix`), target tracking, zooming, and viewport bounds. |
| **Game AI & FSM** | **Custom Engine Module** | AI-Generated (In-House) | Arch-compatible Finite State Machines, Behavior Trees, and grid-based A* pathfinding. |

### Install reality (what compiles today)

The table above is the **target** stack. Only the first two rows are wired into the csproj files today; the remaining NuGet libraries are **planned, not yet installed** — do not write code against them until the package is added.

- **Installed:** `MonoGame.Framework.Native` 3.8.5 + `MonoGame.Runtime.Windows.DX12` 3.8.5 (Core + host); `MonoGame.Content.Builder.Task` 3.8.* (host). **Arch 2.1.0 vendored as source** in `Arch/` and its lean analyzer packaging in `Arch.Generators/`.
- **Installed via Arch (transitive):** MessagePack 3.1.8, Utf8Json 1.3.7, ZeroAllocJobScheduler 1.1.2, Collections.Pooled, CommunityToolkit.HighPerformance, Microsoft.Extensions.ObjectPool.
- **Planned / NOT installed:** Gum, FontStashSharp, MonoGame.Extended, Apos.Input, Aether.Physics2D, BrainAI, ImGui.NET, LDtk/Tiled loaders. Propose adding the package before generating code against any of these.

The csproj files remain the ground truth for what *compiles*; this file is the source of truth for what the architecture *is* and *aims to be*.

---

## Platform & toolchain

- **MonoGame 3.8.5, platform `WindowsDX12`** (`MonoGame.Runtime.Windows.DX12`) — not DesktopGL, not DX11. Runtime-only packages use `PrivateAssets=all` in Core.
- **.NET SDK 10** (10.0.302). Projects target `net10.0` (Core, Arch, PipelineExtensions) and `net10.0-windows` (game host). **C# 12**, `LangVersion latest` in Arch.
- The solution is the XML `.slnx` format (`bonoboengine-dx12.slnx`) — older SDKs / Visual Studio cannot open it.
- No `global.json`, `Directory.Build.props`, `.editorconfig`, or formatter config — follow existing file style (4-space indent, XML doc comments on public members, one concern per file).

### Commands

```powershell
dotnet build bonoboengine-dx12.slnx          # build all 5 projects (green — 0 errors)
dotnet run --project BonoboGame.Dx12          # run the game
dotnet publish -c Release -o ./publish BonoboGame.Dx12
```

`dotnet test` is a **no-op** — the `/Tests/` solution folder is empty; no test project exists yet. Don't claim tests pass.

---

## Solution & projects

`bonoboengine-dx12.slnx` groups five projects:

| Project | Role | Notes |
|---|---|---|
| `BonoboGame.Dx12` | WinExe host | `Program.cs` only; `MonoGamePlatform=WindowsDX12`; references Core. No game logic lives here. |
| `BonoboGame.Core.Dx12` | Game library | `Game.cs` (still the Arch sample scene), `Components/`, `Systems/`, `Extensions/`, `Serializers/`, `Content/Content.mgcb`. Where nearly all work happens. |
| `BonoboGame.PipelineExtensions.Dx12` | MGCB stubs | `Importer1.cs` / `Processor1.cs` template placeholders. Touch only for content-pipeline work. |
| `Arch/Arch.csproj` | Vendored ECS 2.1.0 | Single csproj compiling Core + EventBus + Persistence + Relationships + Systems + LowLevel + generators + T4 output. Third-party code — never refactor or "fix style" here; prefer using its API. |
| `Arch.Generators/Arch.Generators.csproj` | Analyzer packaging | netstandard2.0; links the vendored generator sources into a lean Roslyn analyzer assembly. Referenced by Core with `OutputItemType="Analyzer" ReferenceOutputAssembly="false"`. No runtime code — don't add any. |

### Source generators

The Arch source generators (`[Query]`, `[Event]`) are wired through `Arch.Generators` and functional:

- `[Query]` → `*Query` methods + `Update` overrides. `[Event]` → `EventBus.Send` / `Hook()` / `Unhook()`.
- Manual `QueryDescription` loops remain equally valid; the sample (`Systems/Systems.cs`) shows both styles.
- **Never** reference `Arch.dll` itself as an analyzer — Roslyn's analyzer load context cannot resolve its runtime deps (MessagePack, Utf8Json, …) and csc crashes. The lean `Arch.Generators` project exists to avoid that.
- Only vendored-Arch modification: `Arch/Arch.Systems.SourceGenerator/SourceGenerator.cs` filters on `Arch.Systems.QueryAttribute` (the vendored attributes live in namespace `Arch.Systems`).
- Expected warning noise: 3× CS0436 (generated `Arch.Bus.EventBus` class shadows the compiled generator-model `EventBus` struct in Arch.dll — intended, source wins) and NU1903 on the PipelineExtensions stub (pre-existing).

### Arch quirks that bite

- Extra configurations `Debug-PureECS` / `Release-PureECS` / `Debug-Events` / `Release-Events` define `PURE_ECS` / `EVENTS`, which **change the API shape** (`_world.Set(entity, …)` vs `entity.Set(…)` — see `#if` branches in `Game.cs`). Plain `Debug` / `Release` use the extension-method API.
- `Arch.csproj` sets `EmitCompilerGeneratedFiles` and contains T4 `.tt` templates that generate most of `World.*` / `Chunk.*` APIs — many operations exist 25+ times as overloads. Check the generated `.cs` next to each `.tt` before assuming a signature.
- Assets go through `Content/Content.mgcb` (MGCB). Never raw-copy assets into output; never `File.Read*` game content.

---

## Repository structure

```text
bonoboengine/
├── AGENTS.md                              # Build state, layout, agent/game-dev rules (absorbed CLAUDE.md)
├── bonoboengine-dx12.slnx                 # Solution (XML .slnx; needs .NET 10 SDK)
├── BonoboGame.Dx12/                       # Thin WinExe host
│   ├── Program.cs                         # Main loop entry + window creation
│   ├── BonoboGame.Dx12.csproj             # net10.0-windows; references Core
│   ├── app.manifest, Icon.ico
│   └── ...
├── BonoboGame.Core.Dx12/                  # Game library — where nearly all work happens
│   ├── Game.cs                            # MonoGame Game: world, systems, draw/update lifecycle
│   ├── Components/Components.cs           # Pure data structs: Position, Velocity, Sprite
│   ├── Systems/Systems.cs                 # Arch ECS systems: Movement, Color, Draw, Debug + EventHandler
│   ├── Extensions/                        # RandomExtensions, TextureExtensions
│   ├── Serializers/SpriteSerializer.cs   # Texture2D bridging for Arch binary/JSON serialization
│   ├── Content/Content.mgcb               # MGCB asset pipeline target
│   └── BonoboGame.Core.Dx12.csproj        # net10.0; references Arch + Arch.Generators (analyzer) + PipelineExtensions
├── BonoboGame.PipelineExtensions.Dx12/    # Empty MGCB importer/processor stubs
├── Arch/                                  # Vendored Arch ECS 2.1.0 full source (third-party; do not refactor)
├── Arch.Generators/                       # Roslyn analyzer packaging of the Arch source generators
├── docs/                                  # Human-oriented knowledge base (see docs/ai-agents/doc-map.md)
│   ├── index.md                           # THIS FILE — stack & architecture source of truth
│   └── ai-agents/                         # Agent-oriented distillations
└── openspec/                              # Spec-driven change workflow (use for planned features)
```

Boundaries:

- `docs/2d-games/examples/**.cs` are reference snippets, **not compiled** — copying them in must adapt to the real API.
- `openspec/` is the workflow for planned features (`.opencode/commands/opsx-*`).
- `DESIGN.md` / `FUTURE_IDEAS.md` / `CONTEXT.md` do **not** exist yet — create them when a concrete game project starts.

---

## AI Agent Guidelines & System Instructions

When generating code, refactoring, or adding features in this repository, AI coding agents must adhere strictly to the following rules. (Full generation do/don't lists and game-dev workflow rules live in `AGENTS.md`; verified API facts live in `docs/ai-agents/codebase-truth.md`.)

### 1. Entity Component System (Arch ECS) Rules

* **Components as Data Structs:** Components **must** be zero-logic, public C# `struct` value types for cache locality (e.g., `public struct Position { public Vector2 Value; }`). Never use classes or put methods inside ECS components.
* **Systems as Logic Processors:** Systems must be stateless or process data purely through `QueryDescription` iterations or `Arch.Systems.BaseSystem` implementations.
* **Safe Structural Modifications:** Entity creation, destruction, and component addition/removal must happen via Arch command buffers or outside query loops to prevent invalidating memory chunks during iteration. Never mutate structure mid-query.

### 2. MonoGame Integration & Game Loop

* **Input Abstraction:** The planned design is a custom `InputManager` (`IsActionPressed`, remapping). It does **not** exist yet — until it does, keep raw `Keyboard.GetState()` / `GamePad.GetState()` polling in `Game.Update` (or a dedicated input system), never scattered inside gameplay systems. Do not check direct input inside gameplay logic once the abstraction lands.
* **Frame-Rate Independence:** Always multiply movement vectors and timer increments by `(float)gameTime.ElapsedGameTime.TotalSeconds`.
* **Camera Matrices:** Once the custom 2D camera module exists, pass its view matrix directly into `SpriteBatch.Begin(transformMatrix: …)` inside rendering systems.

### 3. Module Scope & Dependencies

* **No Third-Party Input/State Libraries:** Do not install external libraries for input handling, cameras, or basic AI state machines. Write clean, modular C# files inside the engine.
* **No code against uninstalled packages:** Gum, FontStashSharp, MonoGame.Extended, Apos.Input, Aether.Physics2D, BrainAI, ImGui.NET are planned, not referenced. Propose adding the package first; don't generate code that imports them.

---

## Asset Pipeline Workflow

* **Sprites & Animations:** Standard `.png` images, spritesheets, or texture atlases go through `BonoboGame.Core.Dx12/Content/Content.mgcb` (MGCB). Never raw-copy assets into output; never `File.Read*` game content. (Runtime texture-atlas / animation parsing will use `MonoGame.Extended` once it is added.)
* **UI Design:** Edit interface layouts in the standalone **Gum Visual Editor** (`.gumx`), rendered at runtime via `Gum.MonoGame` — both pending package install.
* **Font Management:** `.ttf` fonts rendered dynamically via `FontStashSharp.FontSystem` — pending package install.