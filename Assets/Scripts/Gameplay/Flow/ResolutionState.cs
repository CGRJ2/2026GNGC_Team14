using GuildGame.Core;
using GuildGame.Data;

namespace GuildGame.Gameplay.Flow
{
    /// <summary>
    /// 결과 상태. 판정 매트릭스로 결과를 도출해 평판을 적용하고 결과 이벤트를 발행한다.
    /// 플레이어가 '다음'을 요청하면 손님 입장 상태로 루프한다.
    /// </summary>
    public class ResolutionState : GameStateBase
    {
        public ResolutionState(GameContext context, StateMachine machine) : base(context, machine) { }

        public override void Enter()
        {
            CaseOutcome outcome = Context.Judgement.Judge(Context.CurrentCase, Context.PendingVerdict);

            GameBalanceSO.OutcomeInfo info = Context.Balance.GetInfo(outcome);
            Context.Reputation.Apply(info.reputationDelta);

            string eventText = Context.Localization.Get(info.eventKey);
            Context.RaiseOutcome(outcome, eventText);

            Context.NextRequested += OnNextRequested;
        }

        public override void Exit()
        {
            Context.NextRequested -= OnNextRequested;
        }

        private void OnNextRequested()
        {
            GoNext();
        }
    }
}
