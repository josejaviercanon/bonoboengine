---
name: bonobo-monogame-lifecycle
description: Work on MonoGame game-loop, content, rendering, windowing, or host-project code in the Bonobo Engine repo (MonoGame 3.8.5 WindowsDX12, .NET 10). USE FOR: Game.cs lifecycle methods, Content.mgcb asset wiring, SpriteBatch/draw systems, GraphicsDeviceManager/window settings, input polling placement, BonoboGame.Dx12 host changes (manifest, icon, platform settings), pipeline extension stubs. DO NOT USE FOR: ECS component/system/query logic (use bonobo-ecs-authoring), feature scoping decisions (use game-scope-guardian), or docs-only changes.
---

# Bonobo MonoGame Lifecycle

Load first:

1. `docs/ai-agents/codebase-truth.md` — platform is **WindowsDX12** (not DesktopGL/DX11); build currently broken (5 pre-existing errors).
2. `docs/ai-agents/monogame-authoring.md` — lifecycle order, content rules, rendering rules.

## Hard rules (summary)

- Respect the lifecycle: setup in `Initialize`, assets in `LoadContent`, ECS world/systems in `BeginRun`, logic in `Update`, rendering only in `Draw`, teardown in `EndRun`. No rendering in `Update`; no game logic in `Draw`.
- Assets: declared in `Content/Content.mgcb`, loaded via `Content.Load<T>()`. Never raw file I/O, never manual copies into `bin`.
- Draw systems are separate from update systems; `SpriteBatch.Begin/End` in `BeforeUpdate`/`AfterUpdate` of the draw system.
- Host project (`BonoboGame.Dx12`) holds only platform concerns (`Program.cs`, `app.manifest`, `Icon.ico`, `MonoGamePlatform=WindowsDX12`). Game code goes in `BonoboGame.Core.Dx12`.
- Runtime packages in Core use `PrivateAssets=all`; keep it that way.
- Raw input polling stays in `Game.Update` (or a dedicated input system) until the planned `InputManager` exists — don't scatter `Keyboard.GetState()` through gameplay systems.
- Ignore `.vscode/launch.json` (stale); verify with `dotnet run --project BonoboGame.Dx12`.

## Before finishing

- `dotnet build bonoboengine-dx12.slnx` — only the 5 known pre-existing errors may remain.
- If you changed content: confirm the `.mgcb` entry, not just the file on disk.
