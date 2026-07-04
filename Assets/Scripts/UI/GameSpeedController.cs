using UnityEngine;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    /// <summary>
    /// 상단 HUD의 시간 제어 토글 버튼.
    /// - Btn_AccelateTime: 진행을 2배속(대사 딜레이·검사 타이머가 2배로 소모).
    /// - Btn_StopTime: 시간 일시정지(타이머·딜레이 정지).
    /// Unity Time.timeScale을 조절하므로 DOTween 딜레이와 InspectionState 타이머가 함께 영향받는다.
    /// 두 버튼 모두 토글이며, 둘 다 켜지면 정지가 우선한다.
    /// </summary>
    public class GameSpeedController : MonoBehaviour
    {
        [SerializeField] private Button _accelerateButton;
        [SerializeField] private Button _stopButton;

        [Min(1f)]
        [Tooltip("가속 배율")]
        [SerializeField] private float _acceleratedScale = 2f;

        [Tooltip("토글 활성 시 버튼 색")]
        [SerializeField] private Color _activeColor = new(1f, 0.85f, 0.3f, 1f);

        private bool _accelerated;
        private bool _stopped;

        private Color _accelNormalColor = Color.white;
        private Color _stopNormalColor = Color.white;
        private bool _accelColorCached;
        private bool _stopColorCached;

        private void Awake()
        {
            CacheColor(_accelerateButton, ref _accelNormalColor, ref _accelColorCached);
            CacheColor(_stopButton, ref _stopNormalColor, ref _stopColorCached);

            if (_accelerateButton != null)
                _accelerateButton.onClick.AddListener(ToggleAccelerate);
            if (_stopButton != null)
                _stopButton.onClick.AddListener(ToggleStop);
        }

        private void OnEnable()
        {
            // 씬 진입/재활성 시 항상 정상 속도로 시작.
            _accelerated = false;
            _stopped = false;
            Apply();
        }

        private void ToggleAccelerate()
        {
            _accelerated = !_accelerated;
            Apply();
        }

        private void ToggleStop()
        {
            _stopped = !_stopped;
            Apply();
        }

        private void Apply()
        {
            Time.timeScale = _stopped ? 0f : (_accelerated ? _acceleratedScale : 1f);
            SetVisual(_accelerateButton, ref _accelNormalColor, ref _accelColorCached, _accelerated);
            SetVisual(_stopButton, ref _stopNormalColor, ref _stopColorCached, _stopped);
        }

        private static void CacheColor(Button button, ref Color normal, ref bool cached)
        {
            if (cached || button == null)
                return;

            Graphic graphic = button.targetGraphic != null ? button.targetGraphic : button.GetComponent<Graphic>();
            if (graphic != null)
            {
                normal = graphic.color;
                cached = true;
            }
        }

        private void SetVisual(Button button, ref Color normal, ref bool cached, bool active)
        {
            if (button == null)
                return;

            CacheColor(button, ref normal, ref cached);
            Graphic graphic = button.targetGraphic != null ? button.targetGraphic : button.GetComponent<Graphic>();
            if (graphic != null)
                graphic.color = active ? _activeColor : normal;
        }

        private void OnDisable()
        {
            // 이 컨트롤러를 벗어날 때 시간 배율을 복구한다(다른 씬에 영향 방지).
            Time.timeScale = 1f;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
            if (_accelerateButton != null)
                _accelerateButton.onClick.RemoveListener(ToggleAccelerate);
            if (_stopButton != null)
                _stopButton.onClick.RemoveListener(ToggleStop);
        }
    }
}
