# Game Development Rules

## Project Context

Architecture: MonoGame 3.8.5 (WindowsDX12) + Arch ECS 2.1.0 (vendored in `Arch/`). Planned, NOT yet installed: Gum UI, MonoGame.Extended, BrainAI, Apos.Input, FontStashSharp — verify against csproj before using any library
Patterns: Service Locator for ambient services, vertical slice features, ECS components as pure data structs
Conventions: C# 12, nullable enabled, readonly structs for data components, one class per file, files under 300 lines
See: `AGENTS.md` for build state & commands, `docs/ai-agents/` for agent guides, `docs/ai-agents/doc-map.md` for the full human documentation tree

## AI Code Generation Rules

- ALWAYS write the interface/contract first, then ask for implementation
- ALWAYS paste Arch ECS type signatures into context when generating ECS code — models lack MonoGame/Arch training data
- NEVER generate game feel code (jump arcs, attack timing, camera behavior, screen shake) — write these by hand
- NEVER trust AI-generated performance-critical inner loops without profiling
- Components are pure data structs. Systems are pure logic functions. Keep them separated.
- Check every generated file against: hallucinated APIs, LINQ in hot paths, missing null/empty checks, swallowed exceptions
- Spend 5 minutes reviewing for every 1 minute of generation
- After generating ECS systems, verify: correct query components, proper command buffer usage, no entity structural changes during iteration

## What AI Should Generate

- Component record structs from design descriptions
- System scaffolding (query-iterate-transform boilerplate)
- Unit tests for deterministic systems (damage calc, state machines)
- XML doc comments and documentation
- JSON data templates (level definitions, item databases, wave configs)
- Interface implementations and data models
- Refactoring: extracting interfaces, splitting large classes

## What AI Should NOT Generate

- Core game loop and fixed timestep integration
- Physics/collision resolution edge cases
- State machine transitions with subtle timing
- Anything involving unique game feel
- Shader hot paths without manual review
- Architecture decisions — discuss these, don't auto-generate

## Art Pipeline Rules

- AI art is for exploration and rough drafts ONLY, never final assets
- Workflow: hand sketch → img2img (0.7-0.8 denoise) → manual cleanup → lower denoise pass → final hand polish
- Budget 50%+ of art time for manual refinement of any AI-assisted output
- Train a custom LoRA (15-30 reference images) for style consistency across assets
- Target specs: 16×16 tiles, 480×270 native resolution, 4× scaling, characters at 16×32

## Project Management Rules

- Every feature must support one of the design pillars — if it doesn't, it goes in the Future Ideas doc
- Use vertical slices: each 1-2 week sprint ends with a playable build
- MoSCoW everything: Must Have → Should Have → Could Have → Won't Have
- Multiply time estimates by 2-3×. Bug fixing consumes 30% of dev time.
- Reserve last 20-30% of development for polish/bugfix ONLY — no new features in this phase
- NO new features without checking the scope doc first

## Scope Control

- Hard deadlines with cuts, not delays
- Keep a separate FUTURE_IDEAS.md — acknowledge cool ideas, defer them
- The Polaris tiers: Essentials (without them game loses USP) → Baseline (minimum complete game) → Accessories (not needed to ship)
- If generating a new feature takes minutes with AI, that is MORE reason to scrutinize scope, not less

## Task Structure

```
Design Pillars (3-5 statements) → what makes this game unique
  └── Milestones: Prototype → Demo → Early Access → Release
       └── Feature Categories: Core Mechanics | Content | UI/UX | Audio | Art | Systems
            └── Tasks: 1-4 hour units, Kanban flow (Backlog → To Do → In Progress → Done)
                 └── Bugs: P0 game-breaking | P1 major | P2 minor | P3 cosmetic
```

## Documentation

- Architecture Decision Records for every significant tech choice (date, context, decision, rationale)
- Weekly dev notes: what was done, blockers, next week plans
- Update CONTEXT.md after every major architectural change
- Document WHY for ECS component/system design decisions — AI will erode coherence without this record

## Commands

- `dotnet build bonoboengine-dx12.slnx`: Build (currently fails with 5 pre-existing errors — see AGENTS.md)
- `dotnet run --project BonoboGame.Dx12`: Run the game (once build is fixed)
- `dotnet publish -c Release -o ./publish BonoboGame.Dx12`: Package for distribution
- `dotnet test`: No-op — no test projects exist yet

## File Boundaries

- Read `AGENTS.md` + `docs/ai-agents/codebase-truth.md` for current project state (CONTEXT.md does not exist yet)
- Read `docs/ai-agents/doc-map.md` to locate architecture/design reference docs
- `DESIGN.md` / `FUTURE_IDEAS.md` do not exist yet — create them when a concrete game project starts; until then, apply the scope rules above to every feature
- NEVER modify DESIGN.md pillars without explicit human approval (once it exists)
