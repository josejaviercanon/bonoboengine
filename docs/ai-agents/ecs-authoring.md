# ECS Authoring (Arch, vendored 2.1.0)

Rules for writing components and systems **in this repo**. Signatures below come from the vendored source, not from public Arch docs — versions differ.

## Non-negotiables

1. Components are pure data `struct`s: public fields, no methods, no logic. See `BonoboGame.Core.Dx12/Components/Components.cs`.
2. Systems are pure logic: inherit `Arch.Systems.BaseSystem<World, GameTime>` (`Arch/Arch.Systems/Systems.cs:51`).
3. **No structural changes during query iteration** — no `World.Create`, `Destroy`, `Add<T>`, `Remove<T>` inside a `World.Query` loop. Collect entities/components during the loop, mutate after (the sample does this in `Game.Update` and `EventHandler.OnDeleteStopEntities`).
4. Frame-rate independence: multiply by `(float)gameTime.ElapsedGameTime.TotalSeconds` (sample uses `Milliseconds` — match whatever the neighboring system uses, but prefer seconds).
5. No LINQ / per-frame string allocation in hot paths.

## Authoring systems: manual queries and source generators

The Arch source generators run (wired via the `Arch.Generators` analyzer — see `codebase-truth.md` § Build state), so `[Query]` methods, generated `*Query`/`Update` overrides, and `[Event]` all work. Manual `QueryDescription` loops are equally valid and preferred when you need full control:

```csharp
using Arch.Core;
using Arch.Systems;
using Microsoft.Xna.Framework;

public class MovementSystem : BaseSystem<World, GameTime>
{
    private readonly QueryDescription _query = new QueryDescription().WithAll<Position, Velocity>();

    public MovementSystem(World world) : base(world) { }

    public override void Update(in GameTime time)
    {
        var dt = (float)time.ElapsedGameTime.TotalSeconds;
        World.Query(in _query, (ref Position pos, ref Velocity vel) =>
        {
            pos.Vector2 += vel.Vector2 * dt;
        });
    }
}
```

- `ref` on every lambda component parameter — Arch passes components by reference; missing `ref` silently copies.
- Lambda parameter list must match `WithAll<...>` arity exactly.
- Query filters: `WithAll`, `WithAny`, `WithNone`, `WithExclusive` (generated overloads up to ~25 type args).
- Register systems in `Arch.Systems.Group<GameTime>` (`Group<T>` runs them in order) — **not** `Systems.Group`; that resolves to the game's `Systems` namespace and fails to compile.
- Draw systems stay separate from update systems (MonoGame splits `Update`/`Draw`); override `BeforeUpdate`/`AfterUpdate` for `SpriteBatch.Begin/End` (see `DrawSystem`).

## API shape gotchas

| Task | Correct call (default config) |
|---|---|
| Create entity | `_world.Create(new Position{...}, new Velocity{...})` |
| Bulk create | `_world.Create(entitiesSpan, [typeof(Position), typeof(Velocity)], amount)` then `entity.Set(...)` |
| Get/Set | `entity.Get<T>()` / `entity.Set(new T{...})` (extension methods, `Arch.Core.Extensions`) |
| Add/Remove component | `entity.Add<T>()` / `entity.Remove<T>()` — outside queries only |
| Query-wide add/remove | `_world.Add(in queryDesc, new T{...})` / `_world.Remove<T>(in queryDesc)` — outside iteration |
| Liveness | `entity.IsAlive()` before using a stored `Entity` |
| Destroy | `_world.Destroy(entity)` then `World.Destroy(_world)` on shutdown |

Under `PURE_ECS` configurations the entity extension methods don't exist — use `_world.Get/Set/Has/Add/Remove(entity, …)`. Don't add new `#if` branches; target the default configuration.

## EventBus

`EventBus.Send(ref evt)` and `[Event]`-attributed receivers work via the `Arch.Generators` analyzer (generated dispatcher plus `Hook()`/`Unhook()` per receiving class). Instance receivers must call `Hook()` to subscribe — see `DebugSystem`. The generated `Arch.Bus.EventBus` class shadows the compiled generator-model struct in Arch.dll (CS0436 warning — intended, source wins).

## Serialization

Pattern in use: `ArchBinarySerializer` from `Arch.Persistence` + a custom `ISerializationFormatter` per unserializable component (see `Serializers/SpriteSerializer.cs`: stores `TextureId`, rehydrates `Texture2D` with a `GraphicsDevice` reference). GPU handles and other runtime objects must be `[IgnoreDataMember]` + resolved via an id. The JSON serializer path is stale (commented out in `Game.cs`).

## Multithreading

`World.SharedJobScheduler` is set to a `ZeroAllocJobScheduler` instance in `Game.BeginRun` and disposed in `EndRun`. `[Query(Parallel = true)]` works (source generator wired); alternatively use the `World.ParallelQuery`-family templates in `Arch/Templates/` after checking the generated signature.

## Review checklist for ECS changes

- [ ] Builds (`dotnet build bonoboengine-dx12.slnx`) — must stay at 0 errors.
- [ ] Components still logic-free structs.
- [ ] No structural changes inside any query loop.
- [ ] `[Query]`/`[Event]` usage binds to generator output (`*Query`/`Update`/`Send` resolve; build stays green).
- [ ] No LINQ, no string interpolation in per-frame code paths.
- [ ] System registered in the right group; draw logic only in draw systems.
