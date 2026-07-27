# Agentic Workflow Rules

Distilled from `docs/game-development/ai-workflow/gamedev-rules.md`, `CLAUDE.md`, and `docs/game-development/session/session-prompt.md`, adapted to this repo. These govern **how** agents work here, not what the code does.

## Generation rules

1. **Small units, always.** One concern per generation, ~100 lines max per piece. Movement first, build, then collision — never both at once.
2. **Build after every change:** `dotnet build bonoboengine-dx12.slnx`. Remember the 5 pre-existing errors (AGENTS.md § Build status) — your change must not add new ones.
3. **Interfaces/contracts first** when adding a new subsystem; get the shape reviewed before filling in implementation.
4. **Explicit over clever.** Verbose readable code; game code is read more than written.
5. **Match existing style:** 4-space indent, XML doc comments on public members, `_camelCase` private fields, PascalCase constants, one concern per file.
6. **No dead code, no commented-out alternatives, no placeholder methods, no speculative abstractions** ("just in case" base classes, plugin systems).

## What agents may generate freely

- Component structs from design descriptions; system scaffolding (query-iterate-transform).
- Data-driven config (constants, tuning tables, JSON level/item definitions).
- Boilerplate: factories, serialization formatters, event wiring, doc comments.
- Unit tests for deterministic logic — note: **no test project exists yet**; creating one is a scope decision, propose it first.
- Refactors: interface extraction, class splitting.

## What agents must NOT auto-generate (flag for the human)

- Game feel: jump arcs, attack timing, camera behavior, screen shake, easing choices.
- Core game loop / fixed-timestep integration changes.
- Physics/collision edge-case resolution; state-machine transitions with subtle timing.
- Shader hot paths; architecture decisions (discuss, don't auto-generate).
- Anything targeting packages that aren't installed (see `codebase-truth.md` § Stack reality).

## Review checklist (run after every generation)

Hallucinated APIs against vendored Arch · LINQ/allocations in hot paths · missing null/empty checks · swallowed exceptions · correct query component lists · structural changes outside loops · `ref` on query lambda params · builds clean.

Spend ~5 minutes reviewing per 1 minute of generation.

## Scope control (enforced — see `game-scope-guardian` skill)

- No unrequested features. "Wouldn't it be cool if…" ideas get acknowledged and parked (FUTURE_IDEAS pattern), not implemented.
- "Just a small thing" = implementation + testing + integration; estimate ×3.
- If a request grows beyond its original boundary while working, say so explicitly instead of silently absorbing it.
- New subsystems/libraries: propose first (docs' planned stack ≠ installed stack).
- Large/planned features go through the openspec workflow (`.opencode/commands/opsx-*.md`, `openspec/`).

## Decisions & records

- Architecturally significant choices get an ADR: date, status, context, options, decision, consequences. Store in `docs/adr/` (create it; none exist yet).
- Document **why** for ECS component/system design decisions near the code — coherence erodes without it.
- Mark unavoidable hacks `// HACK: <reason>`; never document a hack you could just fix.

## Session behavior

- State what you're doing in small steps; show progress on multi-step tasks.
- Ask a short clarifying question when intent is ambiguous — don't guess at unseen dependency chains.
- The human decides direction; you recommend with reasoning.
- Stale docs are worse than none: when you change code that a doc describes, update the doc in the same change.
