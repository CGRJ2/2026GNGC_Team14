using System;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using GuildGame.Gameplay.Services;
using GuildGame.Localization;

namespace GuildGame.Gameplay.Flow
{
    /// <summary>
    /// 상태 · 뷰 · 서비스가 공유하는 블랙보드 겸 이벤트 허브(학생증 검증용).
    /// - 출력 이벤트(상태 → 뷰): CaseStarted / AnswerGiven / OutcomeResolved
    /// - 입력 이벤트(뷰 → 상태): QuestionRequested / VerdictRequested / NextRequested
    /// </summary>
    public class GameContext
    {
        // ---- 의존성 ----
        public ILocalizationProvider Localization { get; }
        public IStudentCaseGenerator Generator { get; }
        public IJudgementService Judgement { get; }
        public GameBalanceSO Balance { get; }
        public ReputationModel Reputation { get; }

        // ---- 사이클 상태 ----
        public StudentCase CurrentCase { get; set; }
        public PlayerVerdict PendingVerdict { get; set; }

        public GameContext(
            ILocalizationProvider localization,
            IStudentCaseGenerator generator,
            IJudgementService judgement,
            GameBalanceSO balance,
            ReputationModel reputation)
        {
            Localization = localization;
            Generator = generator;
            Judgement = judgement;
            Balance = balance;
            Reputation = reputation;
        }

        // ---- 출력 이벤트 (상태가 발행, 뷰가 구독) ----
        public event Action<StudentCase> CaseStarted;
        public event Action<string, string> AnswerGiven;          // (질문, 답변)
        public event Action<CaseOutcome, string> OutcomeResolved; // (결과, 이벤트 대사)

        public void RaiseCaseStarted(StudentCase c) => CaseStarted?.Invoke(c);
        public void RaiseAnswer(string question, string answer) => AnswerGiven?.Invoke(question, answer);
        public void RaiseOutcome(CaseOutcome outcome, string eventText) => OutcomeResolved?.Invoke(outcome, eventText);

        // ---- 입력 이벤트 (뷰가 요청, 상태가 구독) ----
        public event Action<StudentIdFieldType> QuestionRequested;
        public event Action<PlayerVerdict> VerdictRequested;
        public event Action NextRequested;

        public void RequestQuestion(StudentIdFieldType field) => QuestionRequested?.Invoke(field);
        public void RequestVerdict(PlayerVerdict verdict) => VerdictRequested?.Invoke(verdict);
        public void RequestNext() => NextRequested?.Invoke();
    }
}
