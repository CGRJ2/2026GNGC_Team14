# Project Context & Agent Rules

This file is the primary context for AI agents (Codex) working on this project.

## Project Overview
- **Name**: 2026GNGC_Team14
- **Goal**: Develop a Unity-based 2D game (competition/jam-scale).
- **Status**: Early-stage — most gameplay code does not exist yet. Establish clean conventions as you build.

## Technology Stack
- Engine: Unity **6000.3.18f1** (Unity 6)
- Render Pipeline: URP (Universal Render Pipeline) with the **2D Renderer**
- Dimension: **2D** (Sprite / Aseprite / PSD importer / SpriteShape / Tilemap toolchain)
- Input: Unity Input System package (`Assets/InputSystem_Actions.inputactions`)
- Language: C#
- Version Control: Git

## Sub-Agent Architecture
We follow a **Plan → Execute → Review** loop.
1. **Orchestrator**: the main Codex session — analyzes requirements and assigns work to specialized agents.
2. **Sub-Agents**: defined in `.codex/agents/`, each with a specific persona.
3. **Reviewer**: validates output before finalization.

**Constraint**: Sub-agents cannot spawn other sub-agents (no nesting). The main session acts as the Orchestrator and spawns agents directly with the `Agent` tool.

### Specialized Agents (`.codex/agents/`)
- `architect.toml` — structural planning and system design
- `developer.toml` — C# Unity implementation and bug fixing
- `reviewer.toml` — code quality, Unity performance, convention audit
- `technical_artist.toml` — 2D URP visuals, lighting, post-processing
- `game_designer.toml` — mechanic design, balance, feature specs
- `workflow_specialist.toml` — Claude Code / Codex config, MCP, automation

### How to Invoke Sub-Agents
```
Agent(subagent_type="architect")        # structural planning
Agent(subagent_type="developer")        # implementation
Agent(subagent_type="reviewer")         # post-implementation audit
Agent(subagent_type="technical_artist") # visual/rendering work
Agent(subagent_type="game_designer")    # mechanic design, balance specs
```

### Unity MCP Tools
- `mcp__unity-mcp__Unity_RunCommand` — execute Unity Editor commands
- `mcp__unity-mcp__Unity_GetConsoleLogs` — read Unity console output
- `mcp__unity-mcp__Unity_Camera_Capture` — capture screenshots
- `mcp__unity-mcp__Unity_SceneView_CaptureMultiAngleSceneView` — multi-angle capture

The Unity MCP server is configured in `.codex/config.toml`.

## Dual Environment (Claude Code + Codex)
- **Codex**: sub-agents in `.codex/agents/*.toml`; MCP config in `.codex/config.toml`.
- **Claude Code**: sub-agents in `.claude/agents/*.md`; permissions in `.claude/settings.local.json`.
- When updating an agent's knowledge, update **both** the `.codex/agents/<name>.toml` and the matching `.claude/agents/<name>.md`, and keep `AGENTS.md`/`CLAUDE.md` aligned.

## Recommended Conventions (adopt as the project grows)
- **MVC**: Model = data, View = visuals, Controller = logic. Never mix.
- **State machines**: transitions via `StateMachine.ChangeState(...)`, never direct field assignment.
- **Reactive stats**: `ObservableProperty<T>` (or C# events) for UI-linked stats; View subscribes, never mutates.
- **Managers**: `Singleton<T>` accessed through one static `Manager` facade.
- **Data**: ScriptableObjects for designer-facing config; CSV/JSON for bulk balance data. No magic numbers.
- **2D**: use `SpriteRenderer` sorting layers deliberately; use 2D physics (`Rigidbody2D`/`Collider2D`); never mix with 3D physics.
- **Performance**: Object Pooling for frequently spawned/destroyed objects; cache `Camera.main` and `GetComponent` results.

## Guiding Principles
- **Modularity**: small, focused agents over one monolith.
- **Context Preservation**: update `AGENTS.md` / `CLAUDE.md` when architectural decisions are made.
- **Safety**: never run destructive commands without verification unless explicitly marked safe.
- **Unity Editor**: should be open during development.
- **C# Scripts**: standard Unity naming conventions (PascalCase for classes/methods).

## Language & Communication
- **Main Language**: USE **Korean** (한글) for all explanations and summaries to the user.
- **Tone**: Professional, clear, and helpful.
