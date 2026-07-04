using System;
using DG.Tweening;
using MageAcademy.Data;
using UnityEngine;

namespace MageAcademy.UI
{
    public class JudgeActionView : MonoBehaviour
    {
        [Header("Position Offsets")]
        [SerializeField] private Vector2 _fadeInStartOffset = new(0f, -120f);
        [SerializeField] private Vector2 _fadeOutEndOffset = new(0f, 120f);

        [Header("Stamp")]
        [SerializeField] private GameObject _passedImage;
        [SerializeField] private GameObject _failImage;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _stampClip;

        private RectTransform _rect;
        private CanvasGroup _canvasGroup;
        private Sequence _sequence;
        private Vector2 _restPosition;
        private bool _hasRestPosition;

        public void Play(PlayerVerdict verdict, UIAnimationSettingsSO settings, Action onComplete)
        {
            _sequence?.Kill();

            EnsureComponents();
            CaptureRestPositionIfNeeded();

            gameObject.SetActive(true);
            SetStampVisible(verdict, visible: false);

            UIAnimationSettingsSO.FadeSlideSettings enter = settings != null ? settings.judgeActionEnter : null;
            UIAnimationSettingsSO.FadeSlideSettings exit = settings != null ? settings.judgeActionExit : null;
            float holdDuration = settings != null ? settings.judgeActionStampHoldDuration : 1f;

            enter ??= new UIAnimationSettingsSO.FadeSlideSettings { startOffset = new Vector2(0f, -80f), duration = 0.35f };
            exit ??= new UIAnimationSettingsSO.FadeSlideSettings { startOffset = new Vector2(0f, 80f), duration = 0.4f };

            Vector2 fadeInStartPosition = _restPosition + _fadeInStartOffset;
            Vector2 fadeOutEndPosition = _restPosition + _fadeOutEndOffset;

            _rect.DOKill();
            _canvasGroup.DOKill();
            _rect.anchoredPosition = fadeInStartPosition;
            _canvasGroup.alpha = 0f;

            _sequence = DOTween.Sequence()
                .AppendInterval(enter.delay)
                .Append(CreateMoveFadeTween(_restPosition, 1f, enter.duration, enter.ease))
                .AppendCallback(() =>
                {
                    SetStampVisible(verdict, visible: true);
                    PlayStampSound();
                })
                .AppendInterval(Mathf.Max(0f, holdDuration))
                .AppendInterval(exit.delay)
                .Append(CreateMoveFadeTween(fadeOutEndPosition, 0f, exit.duration, exit.ease))
                .OnComplete(() =>
                {
                    SetStampVisible(verdict, visible: false);
                    _rect.anchoredPosition = fadeInStartPosition;
                    _canvasGroup.alpha = 0f;
                    gameObject.SetActive(false);
                    _sequence = null;
                    onComplete?.Invoke();
                })
                .SetLink(gameObject);
        }

        private void SetStampVisible(PlayerVerdict verdict, bool visible)
        {
            if (_passedImage != null)
                _passedImage.SetActive(visible && verdict == PlayerVerdict.ApproveComplete);
            if (_failImage != null)
                _failImage.SetActive(visible && verdict == PlayerVerdict.RejectFail);
        }

        private void PlayStampSound()
        {
            if (_audioSource != null && _stampClip != null)
                _audioSource.PlayOneShot(_stampClip);
        }

        private Tween CreateMoveFadeTween(Vector2 targetPosition, float targetAlpha, float duration, Ease ease)
        {
            return DOTween.Sequence()
                .Join(_rect.DOAnchorPos(targetPosition, duration).SetEase(ease))
                .Join(_canvasGroup.DOFade(targetAlpha, duration).SetEase(ease))
                .SetLink(gameObject);
        }

        private void EnsureComponents()
        {
            if (_rect == null)
                _rect = (RectTransform)transform;
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void CaptureRestPositionIfNeeded()
        {
            if (_hasRestPosition)
                return;

            _restPosition = _rect.anchoredPosition;
            _hasRestPosition = true;
        }

        private void OnDisable()
        {
            _sequence?.Kill();
            _sequence = null;
        }
    }
}
