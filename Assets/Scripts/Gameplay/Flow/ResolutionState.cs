using MageAcademy.Core;
using MageAcademy.Data;
using DG.Tweening;

namespace MageAcademy.Gameplay.Flow
{
    /// <summary>
    /// 결과 상태. 판정 매트릭스로 결과를 도출해 평판을 적용하고 결과 이벤트를 발행한다.
    /// 결과 대사를 출력한 뒤 학생 퇴장과 다음 학생 입장 상태로 루프한다.
    /// </summary>
    public class ResolutionState : GameStateBase
    {
        private Tween _flowTween;

        /// <summary>오늘 제한 인원을 모두 처리했을 때 전이할 상태(컨트롤러가 배선).</summary>
        public IState DayEnd { get; set; }

        public ResolutionState(GameContext context, StateMachine machine) : base(context, machine) { }

        public override void Enter()
        {
            CaseOutcome outcome = Context.PendingTimeout
                ? CaseOutcome.Timeout
                : Context.Judgement.Judge(Context.CurrentCase, Context.PendingVerdict);
            Context.PendingTimeout = false;

            GameBalanceSO.OutcomeInfo info = Context.Balance.GetInfo(outcome);
            Context.Reputation.Apply(info.reputationDelta);
            Context.Day.CountProcessed();

            RaiseOutcomeEmotion(outcome);

            string eventText = Context.Localization.GetRandom(info.eventKey);
            Context.RaiseOutcome(outcome, eventText);

            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            float outcomeDialogueDelay = settings != null ? settings.outcomeDialogueDelay : 1.2f;
            float exitDuration = settings != null
                ? UnityEngine.Mathf.Max(
                    settings.studentExit.delay + settings.studentExit.duration,
                    settings.studentIdButtonExit.delay + settings.studentIdButtonExit.duration)
                : 0.4f;
            float nextStudentDelay = settings != null ? settings.nextStudentDelay : 0.4f;

            _flowTween?.Kill();
            Sequence sequence = DOTween.Sequence()
                .AppendInterval(outcomeDialogueDelay)
                .AppendCallback(Context.RequestStudentExit)
                .AppendInterval(exitDuration + nextStudentDelay);

            if (Context.LoopAfterResolution)
                sequence.AppendCallback(() =>
                {
                    if (Context.Day.IsQuotaReached && DayEnd != null)
                        Machine.ChangeState(DayEnd);
                    else
                        GoNext();
                });

            _flowTween = sequence;
        }

        private void RaiseOutcomeEmotion(CaseOutcome outcome)
        {
            switch (outcome)
            {
                case CaseOutcome.TruthSuccess:
                    Context.RaiseStudentEmotion(StudentEmotion.Happy);
                    break;
                case CaseOutcome.FalseApproved:
                    Context.RaiseStudentEmotion(StudentEmotion.Sneer);
                    break;
                case CaseOutcome.TruthMisjudged: // 억울하게 불합격 → 화남
                case CaseOutcome.Timeout:
                    Context.RaiseStudentEmotion(StudentEmotion.Angry);
                    break;
                default:
                    Context.RaiseStudentEmotion(StudentEmotion.Normal);
                    break;
            }
        }

        public override void Exit()
        {
            _flowTween?.Kill();
            _flowTween = null;
        }
    }
}
