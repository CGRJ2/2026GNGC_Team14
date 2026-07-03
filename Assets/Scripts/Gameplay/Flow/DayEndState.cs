using DG.Tweening;
using GuildGame.Core;
using GuildGame.Data;

namespace GuildGame.Gameplay.Flow
{
    /// <summary>
    /// 하루 종료 상태. 페이드아웃 이벤트를 발행하고, 페이드가 끝나면 날짜를 넘긴 뒤
    /// 다음 날 시작 상태로 전이한다.
    /// </summary>
    public class DayEndState : GameStateBase
    {
        private Tween _flowTween;

        public DayEndState(GameContext context, StateMachine machine) : base(context, machine) { }

        public override void Enter()
        {
            Context.RaiseDayEnded(Context.Day.CurrentDay.Value);

            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            float fadeOutDuration = settings != null ? settings.dayFadeOutDuration : 0.8f;

            _flowTween?.Kill();
            _flowTween = DOTween.Sequence()
                .AppendInterval(fadeOutDuration)
                .AppendCallback(() =>
                {
                    Context.Day.AdvanceDay();
                    GoNext();
                });
        }

        public override void Exit()
        {
            _flowTween?.Kill();
            _flowTween = null;
        }
    }
}
