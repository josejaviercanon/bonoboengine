# 🐒 Bonobo Engine

[![Platform](https://img.shields.io/badge/platform-Cross--Platform-blue)](#-cross-platform-reach)
[![Framework](https://img.shields.io/badge/framework-MonoGame-red)](https://monogame.net)
[![Architecture](https://img.shields.io/badge/architecture-ECS-green)](#-pure-data-driven-ecs)

**Bonobo Engine** is a lightweight, high-performance, and fully open-source game framework built on top of [MonoGame](https://monogame.net). Designed for modern **C# / .NET** developers, it provides a powerful, data-driven architecture to build 2D and desktop 3D games that deploy seamlessly to mobile.

***

## 🌿 The Bonobo Philosophy

In the wild, bonobos are celebrated as the most cooperative, peaceful, and egalitarian of all the great apes. They survive and thrive not through individual aggression, but through shared resources, mutual care, and community bonding.

We believe game development frameworks should work the exact same way:

*   **Cooperation Over Conflict:** Say goodbye to monolithic, tangled inheritance trees. Our architecture prioritizes clean, isolated code modules that play nice together.
*   **Shared Troop Resources:** Built by the community, for the community. Every component, system, and utility tool is built to be easily shared, repurposed, and expanded.
*   **Egalitarian Ecosystem:** Whether you are writing a custom low-level rendering pipeline or fixing a typo in the documentation, every contributor is an equal member of the troop.

***

## ⚡ Core Pillars

### 🧩 Pure Data-Driven ECS
No more bloated game objects. Bonobo decouples your game logic into pure **Entities**, cache-friendly **Components** (structs), and highly optimized global **Systems**. Run thousands of active elements on mobile hardware without dropping a single frame.

### 🎮 Cross-Platform Reach
Write your code once in modern C#. Bonobo abstracts the underlying layout so you can easily target **Windows, Linux, macOS, Android, and iOS** right out of the box.

### 🏗️ Built on MonoGame
We don't reinvent the wheel. By leveraging the time-tested bedrock of [MonoGame](https://monogame.net), Bonobo gives you rock-solid windowing, hardware-accelerated graphics, audio, and inputs, leaving us free to focus purely on elegant engine architecture.

***

## 🤝 Join the Troop!

We are actively looking for developers, technical artists, documentation writers, and testers to help grow the ecosystem. 

```bash
# Clone the repository and check out the framework layout
git clone https://github.com/josejaviercanon/bonoboengine.git
```
## 🗺️ Feature Roadmap & Checklist

This checklist tracks the implementation status of **Bonobo Engine** features across **2D**, **Desktop 3D**, and **Cross-Platform Mobile**. It highlights what we inherit immediately from our core stack (**MonoGame**, **MonoGame.Extended**, `Arch`, and `Arch.Extended`) versus what our troop needs to build from scratch.

### 🛠️ Core Integration & System Architecture
*We use `Arch.Extended` to dramatically reduce boilerplate code and eliminate reflection overhead via ahead-of-time (AOT) source generation.*

- [x] **Source-Generated ECS Systems**: Handled via `Arch.System` and `Arch.System.SourceGenerator`. Developers can use declarative `[Query]` attributes instead of writing raw query loops.
- [x] **High-Performance Messaging**: Handled via `Arch.EventBus`. Allows decoupled systems to communicate across platforms with zero allocation.
- [x] **Low-Level Zero-GC Utilities**: Handled via `Arch.LowLevel` to keep frame rates locked on strict mobile runtimes.
- [ ] **Save/Load State Serialization**: **[IN PROGRESS]** Integrating `Arch.Persistence` to handle automated JSON/Binary (de)serialization of the entire game world.
- [ ] **Entity Hierarchy & Parent-Child Trees**: **[NEEDS BUILDING]** Utilizing `Arch.Relationships` to bind complex object transformations together.

---

### 🎨 2D Graphics & Systems
*We lean heavily on `MonoGame.Extended` to keep this layer lightweight and hyper-optimized.*

- [x] **Sprite & Animation Pipeline**: Handled via `MonoGame.Extended.Sprites` (Spritesheets, texture regions, texture slicing).
- [x] **2D Camera Framework**: Handled via `MonoGame.Extended.Camera2D` (Zoom, pan, tracking targets).
- [x] **Tilemap Engine**: Native support for Tiled maps via `MonoGame.Extended.Tiled`.
- [ ] **Custom `Render2DSystem`**: **[NEEDS BUILDING]** An source-generated `BaseSystem<World, float>` query to automatically batch and render `SpriteComponent` structural arrays.
- [ ] **2D Particle System ECS Wrapper**: **[NEEDS BUILDING]** Wrap `MonoGame.Extended.Particles` into cache-friendly Arch components.

---

### 🌐 Desktop 3D Pipeline
*MonoGame provides low-level graphics devices, but MonoGame.Extended offers little 3D support. We must architect this layer ourselves.*

- [x] **Low-Level Shader Compilation**: Supported via MonoGame’s MGCB content tool (`BasicEffect`, custom HLSL effects).
- [ ] **3D Transform System**: **[NEEDS BUILDING]** Custom `Transform3DComponent` tracking `Vector3` Position, `Quaternion` Rotation, and `Vector3` Scale.
- [ ] **3D Matrix Hierarchy System**: **[NEEDS BUILDING]** Using `Arch.Relationships` to automatically compute parent-child local-to-world matrices.
- [ ] **Mesh RenderSystem**: **[NEEDS BUILDING]** A source-generated system to query `MeshComponent` data and draw `Model` or vertex primitives.
- [ ] **3D View/Projection Camera System**: **[NEEDS BUILDING]** Custom perspective camera components and frustum culling algorithms.
- [ ] **Lighting & Material Pipeline**: **[NEEDS BUILDING]** Systems managing structural components for Directional, Point, and Ambient light data.

---

### 📱 Cross-Platform Mobile Optimization
*Ensuring our code translates beautifully to small screens and touch targets.*

- [x] **Touch Input Mapper**: Fully supported via MonoGame's `TouchPanel` API.
- [ ] **Touch Interface ECS Layer**: **[NEEDS BUILDING]** A custom input system translating raw screen touches into in-game gestures.
- [ ] **Mobile Memory Management**: **[NEEDS BUILDING]** Profiling and locking down system queries to strictly prevent garbage collection (GC) spikes on mobile runtimes.
- [ ] **Screen Resolution Scaler**: **[NEEDS BUILDING]** Virtual resolution system to automatically scale 2D viewports across different device aspect ratios.
