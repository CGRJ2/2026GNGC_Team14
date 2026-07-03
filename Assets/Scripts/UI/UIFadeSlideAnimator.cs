using DG.Tweening;
using GuildGame.Data;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// CanvasGroup 알파 + RectTransform anchoredPosition을 DOTween으로 트위닝하는 등장 연출 컴포넌트.
    /// 수치는 <see cref="UIAnimationSettingsSO.FadeSlideSettings"/>에서 받는다(하드코딩 금지).
    /// 재생 순서는 소유자(예: StudentEntranceView)가 Sequence로 조립한다.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIFadeSlideAnimator : MonoBehaviour
    {
        private RectTransform _rect;
        private CanvasGroup _canvasGroup;
        private Vector2 _restPosition;
        private bool _initialized;

        private void EnsureInitialized()
        {
            if (_initialized)
                return;

            _rect = (RectTransform)transform;
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            _restPosition = _rect.anchoredPosition;
            _initialized = true;
        }

        /// <summary>등장 대기 상태(알파 0, 시작 오프셋 위치)로 즉시 스냅한다.</summary>
        public void PrepareHidden(UIAnimationSettingsSO.FadeSlideSettings settings)
        {
            EnsureInitialized();
            KillActiveTweens();
            _rect.anchoredPosition = _restPosition + settings.startOffset;
            _canvasGroup.alpha = 0f;
        }

        /// <summary>
        /// 현재 상태에서 원래 위치·알파 1로 등장하는 트윈을 만든다.
        /// 반드시 <see cref="PrepareHidden"/> 이후에 사용하고, 생성 즉시 Sequence에 넣을 것.
        /// </summary>
        public Tween CreateAppearTween(UIAnimationSettingsSO.FadeSlideSettings settings)
        {
            EnsureInitialized();

            return DOTween.Sequence()
                .Join(_rect.DOAnchorPos(_restPosition, settings.duration).SetEase(settings.ease))
                .Join(_canvasGroup.DOFade(1f, settings.duration).SetEase(settings.ease))
                .SetLink(gameObject);
        }

        /// <summary>원래 위치·알파 1로 즉시 복귀한다.</summary>
        public void SnapToRest()
        {
            EnsureInitialized();
            KillActiveTweens();
            _rect.anchoredPosition = _restPosition;
            _canvasGroup.alpha = 1f;
        }

        private void KillActiveTweens()
        {
            _rect.DOKill();
            _canvasGroup.DOKill();
        }
    }
}
