using DG.Tweening;
using MageAcademy.Core;
using MageAcademy.Data;
using UnityEngine;

namespace MageAcademy.Gameplay.Flow
{
    /// <summary>
    /// 하루 종료 상태. 페이드아웃 이벤트를 발행하고, 화면이 검어진 뒤 끝나는 날의 종료 일러스트가
    /// 있으면 지정 시간 동안 띄운다. 그 후 날짜를 넘겨 세이브하고 다음 날 시작 상태로 전이한다.
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

            // 끝나는 날(AdvanceDay 이전)의 종료 일러스트.
            DayConfigSO endingConfig = Context.Day.TodayConfig;
            Sprite illustration = endingConfig != null ? endingConfig.endDayIllustration : null;
            float illustrationDuration = endingConfig != null ? endingConfig.endDayIllustrationDuration : 0f;

            _flowTween?.Kill();
            Sequence sequence = DOTween.Sequence()
                .AppendInterval(fadeOutDuration); // 화면이 검게 페이드아웃

            if (illustration != null && illustrationDuration > 0f)
            {
                sequence.AppendCallback(() => Context.RaiseDayEndIllustrationShown(illustration));
                sequence.AppendInterval(illustrationDuration);
                sequence.AppendCallback(Context.RaiseDayEndIllustrationHidden);
            }

            sequence.AppendCallback(() =>
            {
                Context.Day.AdvanceDay();
                Context.SaveProgress();
                GoNext();
            });

            _flowTween = sequence;
        }

        public override void Exit()
        {
            _flowTween?.Kill();
            _flowTween = null;
        }
    }
}
