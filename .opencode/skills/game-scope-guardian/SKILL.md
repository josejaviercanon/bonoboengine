---
name: game-scope-guardian
description: Scope-check feature requests, new subsystems, or new library dependencies in the Bonobo Engine game/engine repo before implementing them. USE FOR: user asks to add a feature/system/library, a task grows beyond its original boundary mid-work, "wouldn't it be cool if" ideas surface, new NuGet packages are proposed (docs' planned stack is not installed), large multi-step features that should go through the openspec workflow. DO NOT USE FOR: bug fixes to existing behavior, refactors with no new surface area, doc updates, or work already scoped via an openspec change.
---

# Game Scope Guardian

This repo's rules treat uncontrolled scope as the #1 project killer (`CLAUDE.md`, `docs/game-development/ai-workflow/gamedev-rules.md`, `P8_pitfalls.md`). Enforce politely but firmly.

## Checks (in order)

1. **Was it asked for?** If the user didn't request it, don't build it. Acknowledge the idea, offer to park it (FUTURE_IDEAS pattern — the file doesn't exist yet; propose creating it).
2. **Does the dependency exist?** Compare against `docs/ai-agents/codebase-truth.md` § Stack reality. Planned-but-uninstalled libraries (Gum, MonoGame.Extended, Apos.Input, FontStashSharp, Aether.Physics2D, BrainAI) require an explicit user decision before any code is written against them.
3. **Is it game-feel code?** Jump arcs, attack timing, camera behavior, screen shake, easing: never auto-generate — flag for hand-writing (CLAUDE.md rule).
4. **Size it.** Implementation + testing + integration ≈ 3× the naive estimate. If it's bigger than a single session, propose the openspec workflow (`.opencode/commands/opsx-new.md`) instead of ad-hoc implementation.
5. **Is it growing mid-task?** If a request expands while you work, stop and name the expansion explicitly; let the user decide to absorb or defer.

## Response pattern

- State the scope concern in 1–3 sentences: what would be added, what it costs, what exists already.
- Offer: (a) minimal version that fits the current request, (b) parked/deferred version, or (c) openspec change for the full thing.
- Never lecture; one short scope flag per task, then respect the user's call.
