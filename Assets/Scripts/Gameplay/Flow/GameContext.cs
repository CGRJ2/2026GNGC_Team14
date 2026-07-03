using System;
using GuildGame.Data;
using GuildGame.Gameplay.Managers;
using GuildGame.Gameplay.Models;
using GuildGame.Gameplay.Services;
using GuildGame.Localization;

namespace GuildGame.Gameplay.Flow
{
    /// <summary>
    /// 상태 · 뷰 · 서비스가 공유하는 블랙보드 겸 이벤트 허브.
    /// - 출력 이벤트(상태 → 뷰): CaseStarted / AnswerGiven / OutcomeResolved
    /// - 입력 이벤트(뷰 → 상태): QuestionRequested / VerdictRequested / NextRequested
    /// 뷰와 상태를 직접 참조 없이 이벤트로 디커플링한다.
    /// </summary>
    public class GameContext
    {
        // ---- 의존성 (구성 시 주입) ----
        public ILocalizationProvider Localization { get; }
        public ITestimonyGenerator TestimonyGenerator { get; }
        public IJudgementService Judgement { get; }
        public QuestManager Quests { get; }
        public GameBalanceSO Balance { get; }
        public ReputationModel Reputation { get; }

        // ---- 사이클 상태 ----
        public AdventurerCase CurrentCase { get; set; }
        public PlayerVerdict PendingVerdict { get; set; }

        public GameContext(
            ILocalizationProvider localization,
            ITestimonyGenerator testimonyGenerator,
            IJudgementService judgement,
            QuestManager quests,
            GameBalanceSO balance,
            ReputationModel reputation)
        {
            Localization = localization;
            TestimonyGenerator = testimonyGenerator;
            Judgement = judgement;
            Quests = quests;
            Balance = balance;
            Reputation = reputation;
        }

        // ---- 출력 이벤트 (상태가 발행, 뷰가 구독) ----
        public event Action<AdventurerCase> CaseStarted;
        public event Action<string, string> AnswerGiven;      // (질문 텍스트, 답변 텍스트)
        public event Action<CaseOutcome, string> OutcomeResolved; // (결과, 이벤트 대사)

        public void RaiseCaseStarted(AdventurerCase c) => CaseStarted?.Invoke(c);
        public void RaiseAnswer(string question, string answer) => AnswerGiven?.Invoke(question, answer);
        public void RaiseOutcome(CaseOutcome outcome, string eventText) => OutcomeResolved?.Invoke(outcome, eventText);

        // ---- 입력 이벤트 (뷰가 요청, 상태가 구독) ----
        public event Action<QuestionSO> QuestionRequested;
        public event Action<PlayerVerdict> VerdictRequested;
        public event Action NextRequested;

        public void RequestQuestion(QuestionSO question) => QuestionRequested?.Invoke(question);
        public void RequestVerdict(PlayerVerdict verdict) => VerdictRequested?.Invoke(verdict);
        public void RequestNext() => NextRequested?.Invoke();
    }
}
