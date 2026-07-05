using MageAcademy.Localization;
using TMPro;
using UnityEngine;

namespace MageAcademy.UI
{
    /// <summary>
    /// 정적 라벨을 로컬라이즈 키에 바인딩한다. 활성화 시와 언어 변경 시 자동으로 텍스트를 갱신한다.
    /// 씬에 LocalizationManager가 없으면(예: 매니저 미배치 씬) 기존 텍스트를 유지한다.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [Tooltip("Localization.csv의 키")]
        [SerializeField] private string _key;

        private TMP_Text _text;
        private bool _subscribed;

        public string Key
        {
            get => _key;
            set { _key = value; Apply(); }
        }

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            Apply();
            LocalizationManager manager = LocalizationManager.Instance;
            if (manager != null && !_subscribed)
            {
                manager.OnLanguageChanged += OnLanguageChanged;
                _subscribed = true;
            }
        }

        private void OnDisable()
        {
            if (_subscribed && LocalizationManager.HasInstance)
                LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
            _subscribed = false;
        }

        private void OnLanguageChanged(Language language) => Apply();

        private void Apply()
        {
            if (_text == null)
                _text = GetComponent<TMP_Text>();
            if (_text == null || string.IsNullOrEmpty(_key))
                return;

            LocalizationManager manager = LocalizationManager.Instance;
            if (manager != null)
                _text.text = manager.Get(_key);
        }
    }
}
