# AI-Agent Guides — Bonobo Engine

This folder is the **agent-facing** documentation layer. The rest of `docs/` was written for humans (long-form prose, theory, tutorials); everything here is distilled into imperative rules and verified facts for coding agents.

## Read order

1. `../../AGENTS.md` — repo-level instructions (build state, commands, boundaries).
2. `codebase-truth.md` — verified facts about this repo; **overrides human docs and training-data assumptions**.
3. Task-specific rule file:
   - `ecs-authoring.md` — creating/modifying components, systems, queries, serialization.
   - `monogame-authoring.md` — game loop, content pipeline, rendering, windowing.
   - `agentic-workflow.md` — how to structure work (increments, scope control, ADRs, reviews).
4. `doc-map.md` — retrieval index into the 227 human docs when a task needs depth (e.g. "how should a tilemap system work").

## Rules for using the human docs

- Treat human guides (`G*`, `P*`, `E*`, `C*`, `R*` files) as **design reference**, not code truth. Their snippets assume packages (MonoGame.Extended, Gum, Apos.Input…) that are **not installed** — see `codebase-truth.md` § Stack reality.
- `docs/2d-games/examples/**/*.cs` are illustrative snippets, not compiled code. Adapt to the vendored Arch API before use; never copy verbatim.
- If a human doc conflicts with a csproj, the csproj wins. If a human doc conflicts with `codebase-truth.md`, truth wins (and please report the stale doc).
