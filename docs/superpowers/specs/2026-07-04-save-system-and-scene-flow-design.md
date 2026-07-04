# 세이브 시스템 & 씬 흐름 설계

- 날짜: 2026-07-04
- 범위: 하루 경과 시 자동 세이브 + 타이틀씬 진입 흐름(세이브 유무로 분기)

## 목표
1. 하루가 지나갈 때 자동으로 진행 상황(평판, 날짜)을 저장한다. 추후 저장 항목 추가가 쉬운 데이터 모델을 만든다.
2. 타이틀씬은 아무 키나 눌러 진입한다. 세이브가 있으면 저장된 날부터(인게임씬), 없으면 튜토리얼씬부터 시작한다.

## 비목표 (YAGNI)
- 다중 세이브 슬롯, "이어하기/새 게임" 메뉴, 인게임 UI에서의 세이브 삭제.
- 하루 중간 진행(오늘 처리한 학생 수) 저장 — 항상 그날 처음부터 재개한다.

## 데이터 모델
`MageAcademy.SaveSystem` 네임스페이스.

### `SaveData` (`[Serializable]` 순수 클래스)
- `int version` — 스키마 버전. 현재 `1`. 향후 마이그레이션 분기용.
- `int currentDay` — 재개할 일차.
- `int reputation` — 마법 학교 평판.
- 확장 규칙: 필드만 추가하고 `version`을 올린다.

### `SaveSystem` (`static` 클래스)
- 저장 위치: `Application.persistentDataPath/save.json`, `JsonUtility` 직렬화.
- API:
  - `bool HasSave()` — 파일 존재 여부.
  - `void Save(SaveData data)` — JSON 기록.
  - `SaveData Load()` — 읽기. 없거나 파싱 실패 시 `null`.
  - `void Delete()` — 파일 삭제(디버그/리셋용).
- MonoBehaviour 아님 → 씬 로드 전에도 호출 가능, 타이틀/인게임 공유.
- `Singleton<T>`(씬 배치형, 상태 있는 매니저용)와는 역할이 다르므로 static 유지.

## 자동 세이브
- `GameContext.SaveProgress()` 추가: 자기 모델(`Day`, `Reputation`)에서 `SaveData`를 조립해 `SaveSystem.Save` 호출. 저장 항목을 한 곳에 모아 확장 용이.
- `DayEndState.Enter()`에서 `Day.AdvanceDay()` 직후 `Context.SaveProgress()` 호출.
  - 예: N일차 완료 → `AdvanceDay()`로 `currentDay = N+1` → 저장. 로드 시 N+1일차 시작부터 재개.
- 튜토리얼은 `DayEndState`에 진입하지 않으므로 영향 없음.

## 튜토리얼 종료 처리
- `DeskController.RunTutorialSequence()` 완료 직후:
  1. 1일차 세이브 생성 — `currentDay = 1`, `reputation = _balance.startingReputation`. (튜토리얼 케이스의 평판 변동은 반영하지 않고 깔끔한 1일차로 시작.)
  2. `InGameScene` 로드.
- 결과: 이후 타이틀에서 세이브가 존재하므로 튜토리얼을 건너뛰고 1일차부터 시작.

## 인게임씬 세이브 로드
- `DeskController.Start()` 비튜토리얼 경로:
  - `SaveSystem.Load()` 성공 시 `new DayModel(_daySchedule, save.currentDay)`, `new ReputationModel(save.reputation)`.
  - 실패(null) 시 1일차 기본값 — 에디터에서 인게임씬 직접 실행하는 경우 대비.

## 타이틀씬
- 신규 `Assets/Scenes/TitleScene.unity` (배경 + "아무 키나 누르세요" 텍스트).
- `TitleSceneController : MonoBehaviour` — 아무 키/마우스 클릭 시:
  - `SaveSystem.HasSave()` → `InGameScene`.
  - 아니면 → `TutorialScene`.
- 씬/UI/빌드세팅은 Unity MCP로 생성·구성.

## 디버그
- `SaveDebugController : MonoBehaviour` — 지정 키(기본 `F9`, 인스펙터 변경 가능) 입력 시 `SaveSystem.Delete()` 호출 후 로그. 타이틀/인게임 어디든 배치 가능.

## 빌드 세팅
- 씬 순서: `TitleScene`(index 0) → `TutorialScene` → `InGameScene`.

## 영향 파일
- 신규: `Assets/Scripts/SaveSystem/SaveData.cs`, `SaveSystem.cs`; `Assets/Scripts/UI/TitleSceneController.cs`; `Assets/Scripts/Debug/SaveDebugController.cs`; `Assets/Scenes/TitleScene.unity`.
- 수정: `GameContext.cs`(SaveProgress), `DayEndState.cs`(저장 호출), `DeskController.cs`(로드·튜토리얼 종료 전환).
