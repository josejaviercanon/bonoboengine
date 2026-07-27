# MonoGame Authoring (this repo)

MonoGame **3.8.5 / WindowsDX12**. Most online guides assume DesktopGL or DX11 — check `codebase-truth.md` before copying platform-specific advice (e.g. shader model, content builder settings).

## Lifecycle — respect the order

`Game.cs` (`BonoboGame.Core.Dx12/Game.cs`) is the single game class:

1. `Initialize()` — non-graphical setup (the sample creates the square texture via `TextureExtensions.CreateSquareTexture`).
2. `LoadContent()` — `SpriteBatch` creation + `Content.Load<T>`.
3. `BeginRun()` — ECS world, `JobScheduler`, system groups are created here (not in `Initialize`).
4. `Update(GameTime)` — all logic; systems via `_systems.BeforeUpdate/Update/AfterUpdate(in gameTime)`.
5. `Draw(GameTime)` — rendering only; separate `_drawSystem`.
6. `EndRun()` — `World.Destroy(_world)`, `_jobScheduler.Dispose()`, `_systems.Dispose()`.

Rules: no rendering in `Update`, no logic in `Draw`, no ECS world use before `BeginRun`.

## Content pipeline (MGCB)

- Assets live in `Content/` and are declared in `Content/Content.mgcb` (one per project — Core and the host each have one).
- Load with `Content.Load<T>("AssetName")`. Never `File.ReadAllBytes`, never copy assets into `bin` manually.
- Adding an asset = adding it to the `.mgcb`, not just the folder.
- `BonoboGame.PipelineExtensions.Dx12` is an empty importer/processor stub — implement there if a custom content type is needed; don't hack raw loading.

## Input

The sample reads `Keyboard.GetState()`/`GamePad.GetState()` directly in `Game.Update` and forwards the state via the Arch `EventBus` (source-generated; works since the generators were wired as analyzers — see `codebase-truth.md`). The docs' planned design is a custom action-mapping `InputManager` (`docs/index.md`, G7) that doesn't exist yet. Until it does: keep raw input polling in `Game.Update` (or a dedicated input system), never scattered inside gameplay systems.

## Rendering

- One `SpriteBatch` per game, created in `LoadContent`, shared into the draw system.
- `GraphicsDeviceManager` lives in the `Game` constructor; window/viewport changes go through it, not the ECS.
- Textures for entities are created/loaded in the game class and injected into components (`Sprite.Texture2D`) — components never load content themselves.

## Windowing / platform

- Host project sets `MonoGamePlatform=WindowsDX12`; the runtime package is referenced with `PrivateAssets=all` in Core so it doesn't leak transitively.
- `app.manifest` + `Icon.ico` live in `BonoboGame.Dx12` (host concerns, not Core).

## Performance hygiene (from P12 / G13 / G33, condensed)

- Zero-allocation target in per-frame paths: no LINQ, no `string` building, no boxing, no `new` of reference types.
- `Span<T>`/`stackalloc` for temporaries (sample uses `stackalloc Entity[amount]` for bulk creation).
- Profile before optimizing; the sample's own note: at ~1M entities the bottleneck is `SpriteBatch`, not Arch.
