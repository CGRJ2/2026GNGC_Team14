# 하루 일과(Day Cycle) 시스템 설계

- 날짜: 2026-07-04
- 상태: 사용자 승인 완료
- 범위: 일일 학생 검사 인원 제한, 하루 전환(페이드아웃/인 + 날짜 표시), 일자별 시작 이벤트(컷씬) 실행

## 개요

Papers-Please식 검사 루프에 "하루" 개념을 추가한다. 하루에 검사할 수 있는 학생 수가 정해져 있고, 그 인원을 모두 처리하면 화면이 페이드아웃되며 다음 날로 넘어간다. 다음 날 시작 시 날짜("Day N")를 표시하고 페이드인하며, 해당 일자에 정의된 시작 이벤트(컷씬)가 있으면 실행한 뒤 학생 입장을 시작한다.

인원 제한은 **공통 기본값 + 일자별 오버라이드** 구조다. 일자별 데이터는 날짜당 ScriptableObject 1개(`DayConfigSO`)로 관리하고, 전체를 `DayScheduleSO`가 묶는다. 날짜 수는 Day 7 내외로 적다고 가정한다.

## 데이터 계층 (`Assets/Scripts/Data`)

### DayConfigSO — 하루치 기획 데이터

```csharp
[CreateAssetMenu(fileName = "DayConfig", menuName = "GuildGame/Day Config")]
public class DayConfigSO : ScriptableObject
{
    public int day;                       // 1일차 = 1

    [Header("인원 제한")]
    public bool overrideStudentLimit;     // false면 DayScheduleSO.defaultStudentLimit 사용
    public int studentLimit = 4;

    [Header("하루 시작 이벤트")]
    public CutsceneSO dayStartCutscene;   // null이면 이벤트 없이 바로 시작
}
```

- 인원 제한이 `bool + int` 오버라이드인 이유: "이벤트만 있고 인원은 공통값인 날"을 표현하기 위함. SO 존재 여부만으로 오버라이드를 판정하면 이 케이스를 표현할 수 없다.
- "특정 학생 지정 등장"은 이번 범위 밖. 필요 시 `DayConfigSO`에 필드를 추가하는 방식으로 확장한다.

### DayScheduleSO — 전체 일정 묶음

```csharp
[CreateAssetMenu(fileName = "DaySchedule", menuName = "GuildGame/Day Schedule")]
public class DayScheduleSO : ScriptableObject
{
    public int defaultStudentLimit = 8;   // 모든 날짜 공통 제한
    [SerializeField] private List<DayConfigSO> _days;

    private Dictionary<int, DayConfigSO> _byDay;  // 최초 조회 시 lazy 빌드

    public DayConfigSO GetConfig(int day);        // 없으면 null
    public int GetStudentLimit(int day);          // 오버라이드 없으면 defaultStudentLimit
}
```

- 딕셔너리는 `day`를 key로 초기화 시 1회 빌드. 중복 `day`나 null 항목은 `Debug.LogWarning` 후 무시(먼저 등록된 항목 우선).
- `StudentDatabaseSO`와 동일한 역할 패턴.

### 에셋 배치

- `Assets/Data/Days/DaySchedule.asset` 1개 + `Assets/Data/Days/Day01.asset` 등 일자별 SO.

## 런타임 모델 (`Assets/Scripts/Gameplay/Models`)

### DayModel — 순수 C# 모델 (ReputationModel과 동급)

```csharp
public class DayModel
{
    public ObservableProperty<int> CurrentDay { get; }      // 1부터 시작
    public ObservableProperty<int> ProcessedToday { get; }  // 오늘 처리한 학생 수

    public int TodayLimit { get; }        // schedule.GetStudentLimit(CurrentDay.Value)
    public bool IsQuotaReached { get; }   // ProcessedToday >= TodayLimit
    public DayConfigSO TodayConfig { get; } // schedule.GetConfig(CurrentDay.Value), null 가능

    public void CountProcessed();         // ProcessedToday += 1 (ResolutionState에서 호출)
    public void AdvanceDay();             // CurrentDay += 1, ProcessedToday = 0
}
```

- 생성자에서 `DayScheduleSO`를 주입받는다. 모델은 SO를 읽기만 하고 변경하지 않는다.

## 플로우 (`Assets/Scripts/Gameplay/Flow`)

### 상태 전이

```
DayStart → StudentEnter → Inspection → Resolution ─┬─ (인원 남음) → StudentEnter
                                                   └─ (제한 도달) → DayEnd → DayStart
```

- 게임 시작 시 첫 진입 상태는 `DayStartState` (1일차 시작 연출부터 시작).
- 튜토리얼 모드(`isTutorial`)는 루프를 돌지 않으므로 기존처럼 `StudentEnterState`로 시작하고 날짜 시스템의 영향을 받지 않는다.

### GameContext 변경

- 의존성 추가: `DayModel Day`
- 출력 이벤트 추가(상태 → 뷰):
  - `event Action<int> DayStarted` / `RaiseDayStarted(int day)` — 페이드인 + 날짜 표시 트리거
  - `event Action<int> DayEnded` / `RaiseDayEnded(int day)` — 페이드아웃 트리거
  - `event Action<CutsceneSO> CutscenePlayRequested` / `RequestCutscene(CutsceneSO)` — 상태가 컷씬 실행을 요청하면 `CutsceneDialogueRunner`가 구독해 재생. 상태가 뷰를 직접 호출하지 않기 위한 이벤트.

### DayEndState (신규)

1. `Enter()`: `Context.RaiseDayEnded(Day.CurrentDay.Value)` 발행.
2. DOTween 시퀀스로 페이드아웃 시간(`UIAnimationSettingsSO.dayFadeOutDuration`)만큼 대기.
3. `Day.AdvanceDay()` 호출 후 `DayStartState`로 전환.
4. `Exit()`에서 시퀀스 Kill (ResolutionState와 동일 패턴).

### DayStartState (신규)

1. `Enter()`: `Context.RaiseDayStarted(Day.CurrentDay.Value)` 발행 → 뷰가 "Day N" 표시 후 페이드인.
2. 날짜 표시 유지 시간 + 페이드인 시간(`UIAnimationSettingsSO.dayLabelHoldDuration`, `dayFadeInDuration`)만큼 대기.
3. `Day.TodayConfig?.dayStartCutscene`이 있으면 `Context.RequestCutscene(...)` 발행 후 `CutsceneEnded` 이벤트를 기다렸다가 `StudentEnterState`로 전환. 없으면 바로 전환.
4. `Exit()`에서 시퀀스 Kill 및 `CutsceneEnded` 구독 해제.

### ResolutionState 변경

- 판정 적용 직후 `Context.Day.CountProcessed()` 호출.
- 시퀀스 마지막 분기: `Context.Day.IsQuotaReached`이면 `DayEnd` 상태로, 아니면 기존 `Next`(StudentEnter)로. (`Next` 외에 `DayEnd` 상태 참조 필드 추가)
- 분기 판정은 시퀀스 콜백 시점(퇴장 연출 완료 후)에 수행한다.
- 튜토리얼 모드(`LoopAfterResolution == false`)는 기존과 동일하게 전환 콜백 자체를 추가하지 않는다.

### DeskController 변경

- `[SerializeField] DayScheduleSO _daySchedule` 필드 추가. null이면 에러 로그 후 비활성화(기존 가드와 동일).
- `DayModel` 생성 후 `GameContext`에 주입.
- 상태 생성/배선: `dayStart.Next = enter`, `resolution.Next = enter`, `resolution.DayEnd = dayEnd`, `dayEnd.Next = dayStart`.
- 일반 모드 첫 상태: `dayStart`. 튜토리얼 모드 첫 상태: 기존대로 `enter`.

## UI (`Assets/Scripts/UI`)

### DayTransitionView (신규, UIViewBase 상속)

- 전체 화면 검은 오버레이 `CanvasGroup` + "Day N" 텍스트(TMP).
- `DayEnded` 구독 → 알파 0→1 페이드아웃(입력 차단 `blocksRaycasts = true`).
- `DayStarted` 구독 → 날짜 텍스트 갱신 후 표시 유지, 이후 알파 1→0 페이드인, 완료 시 `blocksRaycasts = false`.
- 날짜 텍스트는 로컬라이제이션 키(`day_label`, 예: "{0}일차" / "Day {0}") 포맷 사용. `Localization.csv`에 키 추가.
- 게임 시작 시(1일차) 오버레이는 검은 상태에서 시작해 `DayStarted`로 페이드인.
- 뷰는 구독/표시만 하고 모델을 변경하지 않는다 (기존 MVC 경계 유지).

### UIAnimationSettingsSO 추가 항목

```csharp
[Header("Day Transition")]
public float dayFadeOutDuration = 0.8f;   // 하루 종료 페이드아웃
public float dayLabelHoldDuration = 1.5f; // 검은 화면에서 날짜 표시 유지
public float dayFadeInDuration = 0.8f;    // 다음 날 페이드인
```

## 에러 처리

- `DayScheduleSO` 미할당: `DeskController.Start()`에서 에러 로그 + 비활성화.
- `DayScheduleSO._days`에 중복 `day` 또는 null: 딕셔너리 빌드 시 경고 로그, 해당 항목 무시.
- `DayConfigSO`가 없는 날짜: 정상 케이스. 공통 제한 인원으로 진행, 이벤트 없음.
- `studentLimit <= 0`인 오버라이드: 딕셔너리 빌드 시 경고 로그 후 1로 클램프 (0이면 하루가 즉시 끝나 무한 전환 루프 위험).

## 테스트 전략

프로젝트에 테스트 인프라(EditMode 테스트 asmdef)가 아직 없다. 순수 C# 계층(`DayModel`)과 SO 조회 로직(`DayScheduleSO.GetStudentLimit`)은 Unity 의존이 없거나 적으므로, 이번 작업에서 EditMode 테스트 어셈블리를 추가할 수 있으면 다음을 검증한다:

- 오버라이드가 있는 날 → 오버라이드 값, 없는 날 → 공통값.
- `overrideStudentLimit == false`인 SO가 있는 날 → 공통값.
- `CountProcessed` 누적 → `IsQuotaReached` 판정 → `AdvanceDay` 후 리셋.

테스트 인프라 추가가 과하면(잼 규모 고려) Unity Editor에서 플레이 모드 수동 검증으로 대체하고, 검증 시나리오를 구현 계획에 명시한다.

## 범위 밖 (후속 과제)

- 마지막 날 이후 엔딩/게임오버 처리 — DayConfigSO가 없는 날은 공통 제한으로 무한 진행.
- 특정 학생 지정 등장(스크립트된 학생) — `DayConfigSO` 필드 확장으로 대응 예정.
- 하루 결산 화면(수입/평판 요약 등).
