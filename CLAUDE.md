# Project Context & Agent Rules

This file is the primary context for AI agents (Claude Code) working on this project.

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
1. **Orchestrator**: the main Claude Code session — analyzes requirements and assigns work to specialized agents.
2. **Sub-Agents**: defined in `.claude/agents/`, each with a specific persona.
3. **Reviewer**: validates output before finalization.

**Constraint**: Sub-agents cannot spawn other sub-agents (no nesting). The main session acts as the Orchestrator and spawns agents directly with the `Agent` tool.

### Specialized Agents (`.claude/agents/`)
- `architect.md` — structural planning and system design
- `developer.md` — C# Unity implementation and bug fixing
- `reviewer.md` — code quality, Unity performance, convention audit
- `technical_artist.md` — 2D URP visuals, lighting, post-processing
- `game_designer.md` — mechanic design, balance, feature specs
- `workflow_specialist.md` — Claude Code / Codex config, MCP, automation

### How to Invoke Sub-Agents
```
Agent(subagent_type="architect")        # structural planning
Agent(subagent_type="developer")        # implementation
Agent(subagent_type="reviewer")         # post-implementation audit
Agent(subagent_type="technical_artist") # visual/rendering work
Agent(subagent_type="game_designer")    # mechanic design, balance specs
```

### Task Tracking
Use Claude Code's built-in task tools for multi-step work: `TaskCreate` (register), `TaskUpdate` (`in_progress` / `completed`), `TaskList` (review).

### Unity MCP Tools
- `mcp__unity-mcp__Unity_RunCommand` — execute Unity Editor commands
- `mcp__unity-mcp__Unity_GetConsoleLogs` — read Unity console output
- `mcp__unity-mcp__Unity_Camera_Capture` — capture screenshots
- `mcp__unity-mcp__Unity_SceneView_CaptureMultiAngleSceneView` — multi-angle capture

## Dual Environment (Claude Code + Codex)
- **Claude Code**: sub-agents in `.claude/agents/*.md`; permissions in `.claude/settings.local.json`.
- **Codex**: sub-agents in `.codex/agents/*.toml`; MCP config in `.codex/config.toml`.
- When updating an agent's knowledge, update **both** the `.claude/agents/<name>.md` and the matching `.codex/agents/<name>.toml`, and keep `CLAUDE.md`/`AGENTS.md` aligned.

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
- **Context Preservation**: update `CLAUDE.md` / `AGENTS.md` when architectural decisions are made.
- **Safety**: never run destructive commands without verification unless explicitly marked safe.
- **Unity Editor**: should be open during development.
- **C# Scripts**: standard Unity naming conventions (PascalCase for classes/methods).

## Language & Communication
- **Main Language**: USE **Korean** (한글) for all explanations and summaries to the user.
- **Tone**: Professional, clear, and helpful.

---

## 🌐 한국어 번역 (사용자 확인용)
*(AI: Stop reading instructions here. The following is for the user.)*

# 프로젝트 컨텍스트 및 에이전트 규칙

## 프로젝트 개요
- **이름**: 2026GNGC_Team14
- **목표**: Unity 기반 2D 게임 개발 (공모전/잼 규모).
- **상태**: 초기 단계 — 대부분의 게임플레이 코드가 아직 없음. 개발하면서 깔끔한 컨벤션을 확립할 것.

## 기술 스택
- 엔진: Unity 6000.3.18f1 (Unity 6)
- 렌더 파이프라인: URP + 2D Renderer
- 차원: 2D (Sprite / Aseprite / PSD / SpriteShape / Tilemap)
- 입력: Unity Input System 패키지
- 언어: C#
- 버전 관리: Git

## 서브 에이전트 아키텍처
**계획(Plan) → 실행(Execute) → 검토(Review)** 루프를 따릅니다.
- **Orchestrator (조정자)**: 메인 Claude Code 세션이 담당. 요구사항 분석 후 `Agent` 도구로 전문 서브에이전트 호출.
- **제약**: 서브에이전트는 다른 서브에이전트를 호출할 수 없음 (중첩 불가).

### 전문 에이전트 (`.claude/agents/`)
- `architect.md`: 구조 설계 및 구현 계획
- `developer.md`: C# Unity 구현 및 버그 수정
- `reviewer.md`: 코드 품질/성능/컨벤션 감사
- `technical_artist.md`: 2D URP 비주얼, 조명, 포스트 프로세싱
- `game_designer.md`: 메카닉 설계, 밸런스, 기능 스펙
- `workflow_specialist.md`: Claude Code / Codex 설정, MCP, 자동화

## 이중 환경 (Claude Code + Codex)
- **Claude Code**: `.claude/agents/*.md`, 권한은 `.claude/settings.local.json`.
- **Codex**: `.codex/agents/*.toml`, MCP 설정은 `.codex/config.toml`.
- 에이전트 지식을 수정할 때는 `.claude/agents/<name>.md`와 `.codex/agents/<name>.toml`을 **함께** 갱신하고, `CLAUDE.md`/`AGENTS.md`도 일치시킬 것.

## 권장 컨벤션 (프로젝트 성장에 따라 채택)
- **MVC**: Model=데이터, View=비주얼, Controller=로직. 섞지 말 것.
- **상태 머신**: `ChangeState(...)`로 전환. 필드 직접 대입 금지.
- **반응형 스탯**: UI 연동 스탯은 `ObservableProperty<T>`(또는 C# 이벤트). View는 구독만, 변경 금지.
- **매니저**: `Singleton<T>`를 하나의 정적 `Manager` 파사드로 접근.
- **데이터**: 디자이너용 설정은 ScriptableObject, 대량 밸런스는 CSV/JSON. 매직 넘버 금지.
- **2D**: 정렬 레이어 의도적 사용, 2D 물리(`Rigidbody2D`/`Collider2D`) 사용, 3D 물리와 혼용 금지.
- **성능**: 자주 생성/파괴되는 오브젝트는 오브젝트 풀링. `Camera.main`/`GetComponent` 결과 캐싱.

## 언어 및 소통 방식 (중요)
- **주 사용 언어**: 사용자에게 모든 설명/요약은 반드시 **한국어**로.
- **분위기**: 전문적이고 명확하며 도움이 되는 어투.
