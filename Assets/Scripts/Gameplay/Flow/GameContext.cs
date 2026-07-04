using System;
using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using MageAcademy.Gameplay.Services;
using MageAcademy.Localization;
using MageAcademy.SaveSystem;
using UnityEngine;

namespace MageAcademy.Gameplay.Flow
{
    /// <summary>
    /// 상태 · 뷰 · 서비스가 공유하는 블랙보드 겸 이벤트 허브(학생증 검증용).
    /// - 출력 이벤트(상태 → 뷰): CaseStarted / AnswerGiven / OutcomeResolved
    /// - 입력 이벤트(뷰 → 상태): QuestionRequested / VerdictRequested
    /// </summary>
    public class GameContext
    {
        // ---- 의존성 ----
        public ILocalizationProvider Localization { get; }
        public IStudentCaseGenerator Generator { get; }
        public StudentDatabaseSO StudentDatabase { get; }
        public IJudgementService Judgement { get; }
        public GameBalanceSO Balance { get; }
        public ReputationModel Reputation { get; }
        public DayModel Day { get; }
        public UIAnimationSettingsSO UIAnimationSettings { get; }
        public bool LoopAfterResolution { get; }
        public bool IsTutorial { get; }

        // ---- 사이클 상태 ----
        public StudentCase CurrentCase { get; set; }
        public PlayerVerdict PendingVerdict { get; set; }

        /// <summary>검사 제한시간 초과로 결과가 확정되어야 하면 true.</summary>
        public bool PendingTimeout { get; set; }

        /// <summary>
        /// 현재 진행 상황(날짜·평판)을 세이브로 기록한다. 저장 항목이 늘어나면 여기서 조립한다.
        /// 튜토리얼 모드에서는 저장하지 않는다.
        /// </summary>
        public void SaveProgress()
        {
            if (IsTutorial)
                return;

            SaveSystem.SaveSystem.Save(new SaveData
            {
                currentDay = Day.CurrentDay.Value,
                reputation = Reputation.Value.Value,
            });
        }

        public GameContext(
            ILocalizationProvider localization,
            IStudentCaseGenerator generator,
            StudentDatabaseSO studentDatabase,
            IJudgementService judgement,
            GameBalanceSO balance,
            ReputationModel reputation,
            DayModel day,
            UIAnimationSettingsSO uiAnimationSettings,
            bool loopAfterResolution = true,
            bool isTutorial = false)
        {
            Localization = localization;
            Generator = generator;
            StudentDatabase = studentDatabase;
            Judgement = judgement;
            Balance = balance;
            Reputation = reputation;
            Day = day;
            UIAnimationSettings = uiAnimationSettings;
            LoopAfterResolution = loopAfterResolution;
            IsTutorial = isTutorial;
        }

        // ---- 출력 이벤트 (상태가 발행, 뷰가 구독) ----
        public event Action<StudentCase> CaseStarted;
        public event Action<string> QuestionAsked;
        public event Action<string, string> AnswerGiven;          // (질문, 답변)
        public event Action<CaseOutcome, string> OutcomeResolved; // (결과, 이벤트 대사)
        public event Action<CutsceneSpeaker, string> CutsceneDialogueRequested;
        public event Action<StudentSO> CutsceneStudentEnterRequested;
        public event Action CutsceneStudentExitRequested;
        public event Action CutsceneEnded;

        public event Action StudentExitRequested;
        public event Action<int> DayStarted;
        public event Action<int> DayEnded;
        public event Action<CutsceneSO> CutscenePlayRequested;
        public event Action<Sprite> DayEndIllustrationShown;
        public event Action DayEndIllustrationHidden;
        public event Action<string> StudentReactionRequested;      // 추궁 반응 대사
        public event Action<StudentEmotion> StudentEmotionChanged; // 표정 스왑
        public event Action<float, float> InspectionTimerUpdated;  // (남은 시간, 총 시간)
        public event Action InspectionTimerHidden;

        public void RaiseCaseStarted(StudentCase c) => CaseStarted?.Invoke(c);
        public void RaiseQuestion(string question) => QuestionAsked?.Invoke(question);
        public void RaiseAnswer(string question, string answer) => AnswerGiven?.Invoke(question, answer);
        public void RaiseOutcome(CaseOutcome outcome, string eventText) => OutcomeResolved?.Invoke(outcome, eventText);
        public void RaiseCutsceneDialogue(CutsceneSpeaker speaker, string text) => CutsceneDialogueRequested?.Invoke(speaker, text);
        public void RaiseCutsceneStudentEnter(StudentSO student) => CutsceneStudentEnterRequested?.Invoke(student);
        public void RaiseCutsceneStudentExit() => CutsceneStudentExitRequested?.Invoke();
        public void RaiseCutsceneEnded() => CutsceneEnded?.Invoke();
        public void RequestStudentExit() => StudentExitRequested?.Invoke();
        public void RaiseDayStarted(int day) => DayStarted?.Invoke(day);
        public void RaiseDayEnded(int day) => DayEnded?.Invoke(day);
        public void RequestCutscene(CutsceneSO cutscene) => CutscenePlayRequested?.Invoke(cutscene);
        public void RaiseDayEndIllustrationShown(Sprite illustration) => DayEndIllustrationShown?.Invoke(illustration);
        public void RaiseDayEndIllustrationHidden() => DayEndIllustrationHidden?.Invoke();
        public void RaiseStudentReaction(string line) => StudentReactionRequested?.Invoke(line);
        public void RaiseStudentEmotion(StudentEmotion emotion) => StudentEmotionChanged?.Invoke(emotion);
        public void RaiseInspectionTimer(float remaining, float total) => InspectionTimerUpdated?.Invoke(remaining, total);
        public void RaiseInspectionTimerHidden() => InspectionTimerHidden?.Invoke();

        // ---- 입력 이벤트 (뷰가 요청, 상태가 구독) ----
        public event Action<StudentIdFieldType> QuestionRequested;
        public event Action<PlayerVerdict> VerdictRequested;
        public event Action<int> ReportInterrogationRequested; // 레포트 문단 클릭(추궁)

        public void RequestQuestion(StudentIdFieldType field) => QuestionRequested?.Invoke(field);
        public void RequestVerdict(PlayerVerdict verdict) => VerdictRequested?.Invoke(verdict);
        public void RequestReportInterrogation(int paragraphIndex) => ReportInterrogationRequested?.Invoke(paragraphIndex);
    }
}
