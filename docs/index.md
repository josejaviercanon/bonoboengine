Here is the complete, production-ready **The Stack** architecture template, structured with explicit instructions for both your AI coding agents and human developer workflow:

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

---

### AI Agent Guidelines & System Instructions

When generating code, refactoring, or adding features to this repository, AI coding agents (Cursor, Claude, Copilot) must adhere strictly to the following rules:

#### 1. Entity Component System (Arch ECS) Rules

* **Components as Data Structs:** Components **must** be zero-logic, public C# `struct` value types for cache locality (e.g., `public struct Position { public Vector2 Value; }`). Never use classes or put methods inside ECS components.
* **Systems as Logic Processors:** Systems must be stateless or process data purely through `QueryDescription` iterations or `Arch.System` implementations.
* **Safe Structural Modifications:** Entity creation, destruction, and component addition/removal must happen via Arch command buffers or outside query loops to prevent invalidating memory chunks during iteration.

#### 2. MonoGame Integration & Game Loop

* **Input Abstraction:** Do **not** check direct `Keyboard.GetState()` inside gameplay logic. Query the custom `InputManager` module (e.g., `Input.IsActionJustPressed("Jump")`).
* **Frame-Rate Independence:** Always multiply movement vectors and timer increments by `(float)gameTime.ElapsedGameTime.TotalSeconds`.
* **Camera Matrices:** Pass `Camera.GetViewMatrix()` directly into `SpriteBatch.Begin(transformMatrix: ...)` inside rendering systems.

#### 3. Module Scope & Dependencies

* **No Third-Party Input/State Libraries:** Do not install external libraries for input handling, cameras, or basic AI state machines. Write clean, modular C# files inside `Engine/Core/`.

---

### Human Developer Workflow & Setup Instructions

#### 1. Project Initialization & NuGets

Run the following commands in your terminal to initialize the solution and add core packages:

```bash
# for opengl
dotnet new mgdesktopgl -n MyGame
cd MyGame

# Core ECS & System orchestration
dotnet add package Arch --version 2.1.0
dotnet add package Arch.System

# Graphics, Fonts, & UI
dotnet add package FontStashSharp.MonoGame
dotnet add package MonoGame.Extended
dotnet add package Gum.MonoGame

# Developer Tooling
dotnet add package ImGui.NET

# Optional (Only install if rigid-body Box2D physics are needed)
# dotnet add package Aether.Physics2D

```

#### 2. Directory Structure Setup

Organize the repository to separate custom engine utilities from game logic:

```text
MyGame/
├── Assets/                 # Raw .png images, texture atlases, .ttf fonts, .gumx UI projects
├── Content/                # MGCB pipeline target files
├── Engine/                 # Custom AI-generated engine infrastructure
│   ├── Camera/             # Camera2D.cs matrix logic
│   ├── Input/              # InputManager.cs action mapping
│   ├── Physics/            # Custom AABB / Spatial Hash (if not using Aether)
│   └── UI/                 # ImGui integration & Gum wrappers
├── Game/                   # Game-specific code
│   ├── Components/         # Pure C# data structs
│   ├── Systems/            # Arch ECS systems (Update & Render)
│   └── States/             # FSM logic definitions
└── Program.cs / Game1.cs   # Main loop and initialization

```

#### 3. Asset Pipeline Workflow

* **Sprites & Animations:** Save standard `.png` images, spritesheets, or texture atlases into `Assets/Sprites/`. Use `MonoGame.Extended` to load sprite sheets, manage animation frames, and parse texture atlases directly at runtime.
* **UI Design:** Edit interface layouts in the standalone **Gum Visual Editor**. Save `.gumx` files into `Assets/UI/` to render native UI components at runtime.
* **Font Management:** Place `.ttf` fonts into `Assets/Fonts/` and render them dynamically using `FontStashSharp.FontSystem`.
