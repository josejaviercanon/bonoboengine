# Doc Map — retrieval index for the human docs

227 markdown files + 27 C# example snippets. Use this to find the one or two docs worth loading for a task instead of scanning the tree. **All docs are design reference, not API truth** — adapt code to `codebase-truth.md` and `ecs-authoring.md`.

## Top-level layout

| Path | Contents | Duplication note |
|---|---|---|
| `docs/index.md` | **Source of truth** for the stack, architecture & project structure (target stack table + current install reality + repo layout) | Planned rows ≠ installed (see truth file § Stack reality) |
| `docs/monogame-arch/` | MonoGame+Arch rules, architecture essays, reference tables, guides G1–G70 | `guides/` also mirrored under `docs/2d-games/G/` |
| `docs/game-development/` | Engine-agnostic: AI workflow, sessions, programming, concepts, game design, project management | `2d-games/Playbook` mirrors `project-management/P0–P15` |
| `docs/2d-games/` | Mirrored guides + `Playbook/` + **`examples/` (25 compilable-style C# snippets)** | Prefer the `monogame-arch`/`game-development` copies for prose; `examples/` is unique |

## Topic → file

### Core engine topics

| Topic | Guide | Concept theory |
|---|---|---|
| Game loop / fixed timestep | `monogame-arch/guides/G15_game_loop.md` | `game-development/concepts/game-loop-theory.md` |
| Rendering / SpriteBatch | `guides/G2_rendering_and_graphics.md` | — |
| Camera | `guides/G20_camera_systems.md` | `concepts/camera-theory.md` |
| Input | `guides/G7_input_handling.md` | `concepts/input-handling-theory.md` |
| Content pipeline / assets | `guides/G8_content_pipeline.md` | — |
| Scenes | `guides/G38_scene_management.md` | `concepts/scene-management-theory.md` |
| Resolution / viewports / window | `guides/G19_display_resolution_viewports.md`, `G24_window_display_management.md` | — |
| Audio | `guides/G6_audio.md` | `concepts/audio-theory.md` |
| Save/load | `guides/G69_save_load_serialization.md` | — |
| UI / menus | `guides/G5_ui_framework.md`, `G55_settings_menu.md` | `concepts/ui-theory.md` |

### Gameplay systems

| Topic | Guide | Example snippet (`2d-games/examples/`) |
|---|---|---|
| Character controller | `guides/G52_character_controller.md` | `Character/` |
| Physics / collision | `guides/G3_physics_and_collision.md` | `Character/CollisionResolver.cs` |
| Tilemaps | `guides/G37_tilemap_systems.md` | `Tilemap/` |
| Pathfinding / AI | `guides/G40_pathfinding.md`, `G4_ai_systems.md` | `Pathfinding/` |
| Particles / VFX / trails | `guides/G23_particles.md`, `G60_trails_lines.md` | `Effects/` |
| 2D lighting | `guides/G39_2d_lighting.md` | `Lighting/` |
| Fog of war | `guides/G54_fog_of_war.md` | `FogOfWar/` (+ `Shadowcaster.cs`) |
| Procedural generation | `guides/G53_procedural_generation.md` | `Procgen/` |
| Tweening / juice | `guides/G41_tweening.md`, `G30_game_feel_tooling.md` | `Tween/` |
| Prefabs / factories | `guides/G43_entity_prefabs.md` | `Prefabs/` |
| Combat / economy / building / narrative | `guides/G64_`, `G65_`, `G66_`, `G62_narrative_systems.md` | — |
| Animation / skeletal | `guides/G31_animation_state_machines.md`, `G59_skeletal_animation.md` | — |
| Perspective variants | `guides/G28_top_down_perspective.md`, `G49_isometric.md`, `G56_side_scrolling.md` | — |
| Networking / online | `guides/G9_networking.md`, `G48_online_services.md` | — |
| Replay, minimap, weather, water, cutscenes, modding, achievements, tutorials | `guides/G70_`, `G58_`, `G57_`, `G63_`, `G45_`, `G46_`, `G47_`, `G61_*` | — |

### Code quality & process

| Topic | File |
|---|---|
| C# performance / zero-alloc | `monogame-arch/guides/G13_csharp_performance.md` |
| Data structures / design patterns / programming principles | `monogame-arch/guides/G14_`, `G12_`, `G11_*`; `game-development/programming/` |
| Testing / debugging / hot reload / crash reporting | `monogame-arch/guides/G17_`, `G16_`, `G50_`, `G51_*` |
| Profiling / perf budget | `guides/G33_profiling_optimization.md`, `P12_performance_budget.md` |
| Version control | `guides/G44_version_control.md` |
| Deployment / publishing / accessibility / localization | `guides/G32_`, `G36_`, `G35_`, `G34_*` |
| Library stack / capability matrix / project structure | `monogame-arch/reference/R1_`, `R2_`, `R3_*` |
| Architecture history (Nez dropped, engine alternatives, postmortems) | `monogame-arch/architecture/E1_`–`E8_*` |

### Game-design & production (humans lead; agents assist)

| Topic | File |
|---|---|
| Game feel / genre craft | `game-development/game-design/C2_game_feel_and_genre_craft.md`, `C1_genre_reference.md` |
| Design fundamentals / puzzle design | `game-design/E6_`, `E7_*` |
| Production pipeline P0–P15 (milestones, daily workflow, playtesting, art/audio, launch, postmortem) | `game-development/project-management/` (= `2d-games/Playbook/00_`–`15_`) |
| Solo-dev playbook / pitfalls | `project-management/E9_solo_dev_playbook.md`, `P8_pitfalls.md` |
| GDD template | `project-management/P9_gdd_template.md` |

### Already agent-oriented (written for AI, usable as-is)

- `AGENTS.md` (root) — build state, layout, and generation do/don't lists (absorbed `CLAUDE.md` on 2026-07-26).
- `docs/monogame-arch/monogame-arch-rules.md` — MonoGame+Arch rules; **caveat:** assumes NuGet Arch + generic folder layout, so defer to `ecs-authoring.md` on API details.
- `docs/game-development/ai-workflow/gamedev-rules.md` — engine-agnostic generation rules (basis of `agentic-workflow.md`).
- `docs/game-development/session/session-prompt.md` + `formatting.md` — session co-pilot protocol (briefing dashboard, Plan/Decide/Feature/Debug/Scope paths, ADR + session-state formats).
