using GuildGame.Core;
using GuildGame.Data;

namespace GuildGame.Gameplay.Flow
{
    /// <summary>
    /// 검증 상태. 플레이어의 질문 요청에 증언 답변을 조합해 발행하고, 판정 요청이 오면
    /// 결과 상태로 전이한다. 질문은 무제한이며 판정 버튼은 상시 유효하다.
    /// </summary>
    public class InspectionState : GameStateBase
    {
        public InspectionState(GameContext context, StateMachine machine) : base(context, machine) { }

        public override void Enter()
        {
            Context.QuestionRequested += OnQuestionRequested;
            Context.VerdictRequested += OnVerdictRequested;
        }

        public override void Exit()
        {
            Context.QuestionRequested -= OnQuestionRequested;
            Context.VerdictRequested -= OnVerdictRequested;
        }

        private void OnQuestionRequested(QuestionSO question)
        {
            if (question == null || Context.CurrentCase == null)
                return;

            string questionText = Context.Localization.Get(question.questionTextKey);

            string claimedValueKey = Context.CurrentCase.GetClaimedValueKey(question.targetFact);
            string claimedValue = Context.Localization.Get(claimedValueKey);
            string answerText = Context.Localization.GetFormatted(question.answerTemplateKey, claimedValue);

            Context.RaiseAnswer(questionText, answerText);
        }

        private void OnVerdictRequested(PlayerVerdict verdict)
        {
            Context.PendingVerdict = verdict;
            GoNext();
        }
    }
}
