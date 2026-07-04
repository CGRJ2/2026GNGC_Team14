# Project Context & Agent Rules

This file is the primary context for AI agents (Claude Code) working on this project.

## Project Overview
- **Name**: 2026GNGC_Team14
- **Goal**: Develop a Unity-based 2D game (competition/jam-scale).
- **Status**: Early-stage ??most gameplay code does not exist yet. Establish clean conventions as you build.

## Technology Stack
- Engine: Unity **6000.3.18f1** (Unity 6)
- Render Pipeline: URP (Universal Render Pipeline) with the **2D Renderer**
- Dimension: **2D** (Sprite / Aseprite / PSD importer / SpriteShape / Tilemap toolchain)
- Input: Unity Input System package (`Assets/InputSystem_Actions.inputactions`)
- Language: C#
- Version Control: Git

## Sub-Agent Architecture
We follow a **Plan ??Execute ??Review** loop.
1. **Orchestrator**: the main Claude Code session ??analyzes requirements and assigns work to specialized agents.
2. **Sub-Agents**: defined in `.claude/agents/`, each with a specific persona.
3. **Reviewer**: validates output before finalization.

**Constraint**: Sub-agents cannot spawn other sub-agents (no nesting). The main session acts as the Orchestrator and spawns agents directly with the `Agent` tool.

### Specialized Agents (`.claude/agents/`)
- `architect.md` ??structural planning and system design
- `developer.md` ??C# Unity implementation and bug fixing
- `reviewer.md` ??code quality, Unity performance, convention audit
- `technical_artist.md` ??2D URP visuals, lighting, post-processing
- `game_designer.md` ??mechanic design, balance, feature specs
- `workflow_specialist.md` ??Claude Code / Codex config, MCP, automation

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
- `mcp__unity-mcp__Unity_RunCommand` ??execute Unity Editor commands
- `mcp__unity-mcp__Unity_GetConsoleLogs` ??read Unity console output
- `mcp__unity-mcp__Unity_Camera_Capture` ??capture screenshots
- `mcp__unity-mcp__Unity_SceneView_CaptureMultiAngleSceneView` ??multi-angle capture

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
- **Main Language**: USE **Korean** (?쒓?) for all explanations and summaries to the user.
- **Tone**: Professional, clear, and helpful.

---

## Implemented Systems (Magic School Reception MVP)
Papers-Please-style magic school reception loop. Namespaces under `MageAcademy.*`. Plan: `~/.claude/plans/state-sequential-umbrella.md`.
- **Core** (`Assets/Scripts/Core`): `ObservableProperty<T>`, `IState`/`StateMachine`, `Singleton<T>`.
- **Localization** (`Assets/Scripts/Localization`): custom CSV loader. `LocalizationManager.Get(key)` reads `Assets/Resources/Localization.csv` (columns `key,ko,en`). All display text is a loc key.
- **Data (SO)** (`Assets/Scripts/Data`): `StudentSO`/`StudentDatabaseSO` for student identity data, `GameBalanceSO` for reputation deltas and outcome event keys, `UIAnimationSettingsSO` for interrogation UI timing, and modular character appearance SOs. Enums include `StudentIdFieldType`, `PlayerVerdict`, and `CaseOutcome`.
- **Character Appearance** (`Assets/Scripts/Data`, `Assets/Data/Appearance`): `CharacterAppearanceDatabaseSO` owns the selectable character pool. Each `CharacterAppearanceSO` defines gender, body sprite, hair/face/uniform SO references, and layer-local positions. Hair and uniform SOs own sprite pools. Face SOs map `CharacterEmotion` enum values to sprites with `Default` fallback.
- **Gameplay** (`Assets/Scripts/Gameplay`): Models (`StudentCase`, `ReputationModel`), Services (`IStudentCaseGenerator`/`RandomStudentCaseGenerator`, `IJudgementService`/`JudgementService`), Flow (`GameContext` event hub, `GameStateBase` + `StudentEnter`/`Inspection`/`Resolution` states, `DeskController` = composition root).
- **UI** (`Assets/Scripts/UI`): `UIViewBase` + 7 views. Views subscribe to `GameContext` events only; never mutate models.
- **Cycle**: Enter (random student case) -> Inspection (student ID field questions, admit/reject) -> Resolution (4-quadrant outcome + reputation + student exit) -> loop.
- **Editor setup**: No quest/question sample generator is currently maintained. Create student/appearance SOs directly under `Assets/Data`.

---

## ?뙋 ?쒓뎅??踰덉뿭 (?ъ슜???뺤씤??
*(AI: Stop reading instructions here. The following is for the user.)*

# ?꾨줈?앺듃 而⑦뀓?ㅽ듃 諛??먯씠?꾪듃 洹쒖튃

## ?꾨줈?앺듃 媛쒖슂
- **?대쫫**: 2026GNGC_Team14
- **紐⑺몴**: Unity 湲곕컲 2D 寃뚯엫 媛쒕컻 (怨듬え????洹쒕え).
- **?곹깭**: 珥덇린 ?④퀎 ???遺遺꾩쓽 寃뚯엫?뚮젅??肄붾뱶媛 ?꾩쭅 ?놁쓬. 媛쒕컻?섎㈃??源붾걫??而⑤깽?섏쓣 ?뺣┰??寃?

## 湲곗닠 ?ㅽ깮
- ?붿쭊: Unity 6000.3.18f1 (Unity 6)
- ?뚮뜑 ?뚯씠?꾨씪?? URP + 2D Renderer
- 李⑥썝: 2D (Sprite / Aseprite / PSD / SpriteShape / Tilemap)
- ?낅젰: Unity Input System ?⑦궎吏
- ?몄뼱: C#
- 踰꾩쟾 愿由? Git

## ?쒕툕 ?먯씠?꾪듃 ?꾪궎?띿쿂
**怨꾪쉷(Plan) ???ㅽ뻾(Execute) ??寃??Review)** 猷⑦봽瑜??곕쫭?덈떎.
- **Orchestrator (議곗젙??**: 硫붿씤 Claude Code ?몄뀡???대떦. ?붽뎄?ы빆 遺꾩꽍 ??`Agent` ?꾧뎄濡??꾨Ц ?쒕툕?먯씠?꾪듃 ?몄텧.
- **?쒖빟**: ?쒕툕?먯씠?꾪듃???ㅻⅨ ?쒕툕?먯씠?꾪듃瑜??몄텧?????놁쓬 (以묒꺽 遺덇?).

### ?꾨Ц ?먯씠?꾪듃 (`.claude/agents/`)
- `architect.md`: 援ъ“ ?ㅺ퀎 諛?援ы쁽 怨꾪쉷
- `developer.md`: C# Unity 援ы쁽 諛?踰꾧렇 ?섏젙
- `reviewer.md`: 肄붾뱶 ?덉쭏/?깅뒫/而⑤깽??媛먯궗
- `technical_artist.md`: 2D URP 鍮꾩＜?? 議곕챸, ?ъ뒪???꾨줈?몄떛
- `game_designer.md`: 硫붿뭅???ㅺ퀎, 諛몃윴?? 湲곕뒫 ?ㅽ럺
- `workflow_specialist.md`: Claude Code / Codex ?ㅼ젙, MCP, ?먮룞??
## ?댁쨷 ?섍꼍 (Claude Code + Codex)
- **Claude Code**: `.claude/agents/*.md`, 沅뚰븳? `.claude/settings.local.json`.
- **Codex**: `.codex/agents/*.toml`, MCP ?ㅼ젙? `.codex/config.toml`.
- ?먯씠?꾪듃 吏?앹쓣 ?섏젙???뚮뒗 `.claude/agents/<name>.md`? `.codex/agents/<name>.toml`??**?④퍡** 媛깆떊?섍퀬, `CLAUDE.md`/`AGENTS.md`???쇱튂?쒗궗 寃?

## 沅뚯옣 而⑤깽??(?꾨줈?앺듃 ?깆옣???곕씪 梨꾪깮)
- **MVC**: Model=?곗씠?? View=鍮꾩＜?? Controller=濡쒖쭅. ?욎? 留?寃?
- **?곹깭 癒몄떊**: `ChangeState(...)`濡??꾪솚. ?꾨뱶 吏곸젒 ???湲덉?.
- **諛섏쓳???ㅽ꺈**: UI ?곕룞 ?ㅽ꺈? `ObservableProperty<T>`(?먮뒗 C# ?대깽??. View??援щ룆留? 蹂寃?湲덉?.
- **留ㅻ땲?**: `Singleton<T>`瑜??섎굹???뺤쟻 `Manager` ?뚯궗?쒕줈 ?묎렐.
- **?곗씠??*: ?붿옄?대꼫???ㅼ젙? ScriptableObject, ???諛몃윴?ㅻ뒗 CSV/JSON. 留ㅼ쭅 ?섎쾭 湲덉?.
- **2D**: ?뺣젹 ?덉씠???섎룄???ъ슜, 2D 臾쇰━(`Rigidbody2D`/`Collider2D`) ?ъ슜, 3D 臾쇰━? ?쇱슜 湲덉?.
- **?깅뒫**: ?먯＜ ?앹꽦/?뚭눼?섎뒗 ?ㅻ툕?앺듃???ㅻ툕?앺듃 ?留? `Camera.main`/`GetComponent` 寃곌낵 罹먯떛.

## ?몄뼱 諛??뚰넻 諛⑹떇 (以묒슂)
- **二??ъ슜 ?몄뼱**: ?ъ슜?먯뿉寃?紐⑤뱺 ?ㅻ챸/?붿빟? 諛섎뱶??**?쒓뎅??*濡?
- **遺꾩쐞湲?*: ?꾨Ц?곸씠怨?紐낇솗?섎ŉ ?꾩????섎뒗 ?댄닾.
