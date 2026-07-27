---
name: gamedev-doc-retrieval
description: Find the right human-written design/technical guide inside this repo's 227-file docs/ tree before implementing a game-dev topic. USE FOR: questions or tasks touching camera, tilemap, pathfinding, particles, lighting, fog of war, procgen, tweening, UI, audio, save/load, networking, animation, scene management, performance, testing, deployment, game design, or production planning — load the mapped doc instead of guessing or scanning the tree. DO NOT USE FOR: repo API facts (use docs/ai-agents/codebase-truth.md), writing ECS/MonoGame code itself (use bonobo-ecs-authoring / bonobo-monogame-lifecycle), or scope decisions (use game-scope-guardian).
---

# Game-Dev Doc Retrieval

The full retrieval index is `docs/ai-agents/doc-map.md` — open it and jump to the topic table.

## Procedure

1. Identify the topic(s) of the task (e.g. "tilemap collision" → tilemaps + physics).
2. Look up the mapped guide(s) in `docs/ai-agents/doc-map.md`. Prefer `docs/monogame-arch/` and `docs/game-development/` copies; `docs/2d-games/` mirrors most of them.
3. Read only the 1–2 relevant docs (they are long). For implementation patterns, check `docs/2d-games/examples/<Topic>/` for C# reference snippets.
4. **Adapt before use:** docs assume packages that are not installed (MonoGame.Extended, Gum, Apos.Input…). Translate to the installed stack per `docs/ai-agents/codebase-truth.md` and the API per `docs/ai-agents/ecs-authoring.md` / `monogame-authoring.md`. Example snippets are not compiled — expect API drift.
5. If the doc conflicts with the csproj or the truth file, the doc is stale — follow the truth file and mention the discrepancy.

## Quick topic index (details in doc-map.md)

- Engine plumbing: G15 game loop, G2 rendering, G20 camera, G7 input, G8 content, G38 scenes, G19/G24 resolution & window, G6 audio, G69 save/load, G5/G55 UI.
- Gameplay: G52 character controller, G3 physics, G37 tilemap, G40 pathfinding, G23 particles, G39 lighting, G54 fog of war, G53 procgen, G41 tween, G43 prefabs, G31/G59 animation.
- Quality/process: G13 C# perf, G17 testing, G33 profiling, G12/G14 patterns & data structures, R1–R3 stack/capability/structure references.
- Design/production: C1/C2 genre & game feel, E6/E7 design, P0–P15 production playbook.
