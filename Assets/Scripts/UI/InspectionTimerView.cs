using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    /// <summary>
    /// 검사 제한시간 표시. 남은 시간 이벤트를 받아 채우기 바와 숫자를 갱신하고,
    /// 검사 중에만 표시한다. 뷰는 이벤트 구독만 하고 모델을 변경하지 않는다.
    /// </summary>
    public class InspectionTimerView : UIViewBase
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Image _fillImage;
        [SerializeField] private TMP_Text _timeText;

        [Tooltip("이 비율 이하로 남으면 경고 색")]
        [SerializeField] private float _warnThreshold = 0.25f;
        [SerializeField] private Color _normalColor = new(0.4f, 0.8f, 0.5f, 1f);
        [SerializeField] private Color _warnColor = new(0.9f, 0.3f, 0.3f, 1f);

        protected override void OnBind()
        {
            Context.InspectionTimerUpdated += OnTimerUpdated;
            Context.InspectionTimerHidden += OnTimerHidden;
            SetVisible(false);
        }

        private void OnTimerUpdated(float remaining, float total)
        {
            SetVisible(true);

            float normalized = total > 0f ? Mathf.Clamp01(remaining / total) : 0f;
            if (_fillImage != null)
            {
                _fillImage.fillAmount = normalized;
                _fillImage.color = normalized <= _warnThreshold ? _warnColor : _normalColor;
            }

            if (_timeText != null)
                _timeText.text = Mathf.CeilToInt(remaining).ToString();
        }

        private void OnTimerHidden()
        {
            SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            GameObject target = _root != null ? _root : gameObject;
            if (target.activeSelf != visible)
                target.SetActive(visible);
        }

        private void OnDestroy()
        {
            if (Context != null)
            {
                Context.InspectionTimerUpdated -= OnTimerUpdated;
                Context.InspectionTimerHidden -= OnTimerHidden;
            }
        }
    }
}
