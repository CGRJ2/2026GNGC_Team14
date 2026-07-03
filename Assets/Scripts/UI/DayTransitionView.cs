using DG.Tweening;
using GuildGame.Data;
using TMPro;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// 하루 전환 오버레이. DayEnded에 페이드아웃, DayStarted에 날짜 표시 후 페이드인한다.
    /// 전환 중에는 입력을 차단한다. 뷰는 이벤트 구독만 하고 모델을 변경하지 않는다.
    /// </summary>
    public class DayTransitionView : UIViewBase
    {
        [SerializeField] private CanvasGroup _overlay;
        [SerializeField] private TMP_Text _dayLabel;

        private Tween _tween;

        protected override void OnBind()
        {
            Context.DayEnded += OnDayEnded;
            Context.DayStarted += OnDayStarted;

            SetOverlay(0f, false);
            SetLabelVisible(false);
        }

        private void OnDayEnded(int day)
        {
            if (_overlay == null)
                return;

            _tween?.Kill();
            SetLabelVisible(false);
            _overlay.blocksRaycasts = true;
            _tween = _overlay.DOFade(1f, GetFadeOutDuration());
        }

        private void OnDayStarted(int day)
        {
            if (_overlay == null)
                return;

            _tween?.Kill();
            SetOverlay(1f, true);

            if (_dayLabel != null)
                _dayLabel.text = Context.Localization.GetFormatted("day_label", day);
            SetLabelVisible(true);

            _tween = DOTween.Sequence()
                .AppendInterval(GetHoldDuration())
                .AppendCallback(() => SetLabelVisible(false))
                .Append(_overlay.DOFade(0f, GetFadeInDuration()))
                .OnComplete(() => _overlay.blocksRaycasts = false);
        }

        private void SetOverlay(float alpha, bool blocksRaycasts)
        {
            if (_overlay == null)
                return;

            _overlay.alpha = alpha;
            _overlay.blocksRaycasts = blocksRaycasts;
        }

        private void SetLabelVisible(bool visible)
        {
            if (_dayLabel != null)
                _dayLabel.gameObject.SetActive(visible);
        }

        private float GetFadeOutDuration()
        {
            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            return settings != null ? settings.dayFadeOutDuration : 0.8f;
        }

        private float GetHoldDuration()
        {
            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            return settings != null ? settings.dayLabelHoldDuration : 1.5f;
        }

        private float GetFadeInDuration()
        {
            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            return settings != null ? settings.dayFadeInDuration : 0.8f;
        }

        private void OnDestroy()
        {
            _tween?.Kill();
            if (Context != null)
            {
                Context.DayEnded -= OnDayEnded;
                Context.DayStarted -= OnDayStarted;
            }
        }
    }
}
