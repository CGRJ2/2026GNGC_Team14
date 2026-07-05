using DG.Tweening;
using MageAcademy.Core;
using MageAcademy.Data;
using MageAcademy.Gameplay.Services;

namespace MageAcademy.Gameplay.Flow
{
    /// <summary>
    /// 하루 시작 상태. 날짜 표시/페이드인 이벤트를 발행하고, 연출이 끝나면
    /// 당일 시작 컷씬이 있을 경우 재생 완료를 기다린 뒤 학생 입장 상태로 전이한다.
    /// </summary>
    public class DayStartState : GameStateBase
    {
        private Tween _flowTween;
        private bool _waitingCutscene;

        public DayStartState(GameContext context, StateMachine machine) : base(context, machine) { }

        public override void Enter()
        {
            if (Context.Generator is IStudentCaseGeneratorLifecycle lifecycle)
                lifecycle.BeginDay(Context.Day.CurrentDay.Value);

            Context.RaiseDayRequirementsPrepared(Context.Day.TodayConfig);
            Context.RaiseDayStarted(Context.Day.CurrentDay.Value);

            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            float holdDuration = settings != null ? settings.dayLabelHoldDuration : 1.5f;
            float fadeInDuration = settings != null ? settings.dayFadeInDuration : 0.8f;

            _flowTween?.Kill();
            _flowTween = DOTween.Sequence()
                .AppendInterval(holdDuration + fadeInDuration)
                .AppendCallback(OnFadeInComplete);
        }

        public override void Exit()
        {
            _flowTween?.Kill();
            _flowTween = null;

            if (_waitingCutscene)
            {
                Context.CutsceneEnded -= OnCutsceneEnded;
                _waitingCutscene = false;
            }
        }

        private void OnFadeInComplete()
        {
            DayConfigSO config = Context.Day.TodayConfig;
            CutsceneSO cutscene = config != null ? config.dayStartCutscene : null;

            if (cutscene == null)
            {
                GoNext();
                return;
            }

            _waitingCutscene = true;
            Context.CutsceneEnded += OnCutsceneEnded;
            Context.RequestCutscene(cutscene);
        }

        private void OnCutsceneEnded()
        {
            _waitingCutscene = false;
            Context.CutsceneEnded -= OnCutsceneEnded;
            GoNext();
        }
    }
}
