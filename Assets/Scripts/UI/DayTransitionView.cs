using DG.Tweening;
using MageAcademy.Data;
using TMPro;
using UnityEngine;

namespace MageAcademy.UI
{
    public class DayTransitionView : UIViewBase
    {
        [SerializeField] private CanvasGroup _overlay;
        [SerializeField] private TMP_Text _dayLabel;
        [SerializeField] private RectTransform _dayLabelBackground;
        [SerializeField] private Vector2 _dayLabelBackgroundPadding = new(28f, 14f);

        private Tween _tween;
        private Tween _eventPanelTween;
        private GameObject _activeEventPanel;
        private CanvasGroup _activeEventPanelGroup;

        protected override void OnBind()
        {
            Context.DayEnded += OnDayEnded;
            Context.DayStarted += OnDayStarted;
            Context.EventPanelShown += OnEventPanelShown;
            Context.DayEndIllustrationHidden += OnDayEndIllustrationHidden;

            SetOverlay(0f, false);
            SetLabelVisible(false);
            HideEventPanel();
        }

        private void OnEventPanelShown(GameObject eventPanelPrefab)
        {
            HideEventPanel();

            if (eventPanelPrefab == null || _overlay == null)
                return;

            _activeEventPanel = Instantiate(eventPanelPrefab, _overlay.transform);
            RectTransform rect = _activeEventPanel.transform as RectTransform;
            if (rect != null)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.localScale = Vector3.one;
            }

            _activeEventPanelGroup = _activeEventPanel.GetComponent<CanvasGroup>();
            if (_activeEventPanelGroup == null)
                _activeEventPanelGroup = _activeEventPanel.AddComponent<CanvasGroup>();

            _activeEventPanelGroup.alpha = 0f;
            _activeEventPanelGroup.interactable = false;
            _activeEventPanelGroup.blocksRaycasts = false;
            _activeEventPanel.SetActive(true);
            _eventPanelTween?.Kill();
            _eventPanelTween = _activeEventPanelGroup
                .DOFade(1f, GetEventPanelFadeDuration())
                .OnComplete(() =>
                {
                    if (_activeEventPanelGroup == null)
                        return;

                    _activeEventPanelGroup.interactable = true;
                    _activeEventPanelGroup.blocksRaycasts = true;
                });
        }

        private void OnDayEndIllustrationHidden()
        {
            FadeOutEventPanel();
            SetLabelVisible(false);
        }

        private void HideEventPanel()
        {
            _eventPanelTween?.Kill();
            _eventPanelTween = null;

            if (_activeEventPanel == null)
                return;

            _activeEventPanel.SetActive(false);
            Destroy(_activeEventPanel);
            _activeEventPanel = null;
            _activeEventPanelGroup = null;
        }

        private void FadeOutEventPanel()
        {
            _eventPanelTween?.Kill();

            if (_activeEventPanel == null || _activeEventPanelGroup == null)
            {
                HideEventPanel();
                return;
            }

            _activeEventPanelGroup.interactable = false;
            _activeEventPanelGroup.blocksRaycasts = false;
            _eventPanelTween = _activeEventPanelGroup
                .DOFade(0f, GetEventPanelFadeDuration())
                .OnComplete(HideEventPanel);
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

            SetLabelVisible(true);
            if (_dayLabel != null)
            {
                _dayLabel.text = Context.Localization.GetFormatted("day_label", day);
                LayoutLabelBackground();
            }

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
            if (_dayLabelBackground != null)
                _dayLabelBackground.gameObject.SetActive(visible);
        }

        private void LayoutLabelBackground()
        {
            if (_dayLabelBackground == null || _dayLabel == null)
                return;

            _dayLabel.ForceMeshUpdate();
            Vector2 rendered = _dayLabel.GetRenderedValues(false);
            if (rendered.x <= 0f || float.IsNaN(rendered.x) || float.IsNaN(rendered.y))
                rendered = _dayLabel.GetPreferredValues();

            _dayLabelBackground.sizeDelta = new Vector2(
                rendered.x + _dayLabelBackgroundPadding.x * 2f,
                rendered.y + _dayLabelBackgroundPadding.y * 2f);
            _dayLabelBackground.anchoredPosition = Vector2.zero;

            _dayLabelBackground.SetAsLastSibling();
            _dayLabel.transform.SetAsLastSibling();
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

        private float GetEventPanelFadeDuration()
        {
            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            return settings != null ? settings.dayEventPanelFadeDuration : 0.35f;
        }

        private float GetFadeInDuration()
        {
            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            return settings != null ? settings.dayFadeInDuration : 0.8f;
        }

        private void OnDestroy()
        {
            _tween?.Kill();
            _eventPanelTween?.Kill();
            if (Context != null)
            {
                Context.DayEnded -= OnDayEnded;
                Context.DayStarted -= OnDayStarted;
                Context.EventPanelShown -= OnEventPanelShown;
                Context.DayEndIllustrationHidden -= OnDayEndIllustrationHidden;
            }

            HideEventPanel();
        }
    }
}
