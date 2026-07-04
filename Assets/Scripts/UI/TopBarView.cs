using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    public class TopBarView : UIViewBase
    {
        [SerializeField] private Button _fastForwardButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private OptionsPanelView _optionsPanel;

        [Tooltip("상단바 현재 날짜 표기 라벨(Tmp_DayCount)")]
        [SerializeField] private TMP_Text _dayCountLabel;

        public event UnityAction FastForwardClicked;

        public Button FastForwardButton => _fastForwardButton;
        public Button SettingsButton => _settingsButton;
        public OptionsPanelView OptionsPanel => _optionsPanel;

        private void Awake()
        {
            if (_settingsButton == null)
                _settingsButton = CreateFallbackSettingsButton();
            else
                EnsureButtonLabel(_settingsButton, "\uC124\uC815");

            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OpenOptionsPanel);

            if (_fastForwardButton != null)
                _fastForwardButton.onClick.AddListener(OnFastForwardClicked);
        }

        protected override void OnBind()
        {
            Context.DayStarted += OnDayStarted;
            if (Context.Day != null)
                UpdateDay(Context.Day.CurrentDay.Value);
        }

        private void OnDayStarted(int day) => UpdateDay(day);

        private void UpdateDay(int day)
        {
            if (_dayCountLabel != null)
                _dayCountLabel.text = Context.Localization.GetFormatted("day_label", day);
        }

        private void OpenOptionsPanel()
        {
            if (_optionsPanel != null)
                _optionsPanel.Show();
        }

        private void OnFastForwardClicked()
        {
            FastForwardClicked?.Invoke();
        }

        private Button CreateFallbackSettingsButton()
        {
            GameObject buttonObject = new("Btn_Settings");
            buttonObject.layer = gameObject.layer;
            buttonObject.transform.SetParent(transform, false);

            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-16f, 0f);
            rect.sizeDelta = new Vector2(68f, 36f);

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.18f, 0.18f, 0.2f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;

            GameObject labelObject = new("Label");
            labelObject.layer = gameObject.layer;
            labelObject.transform.SetParent(buttonObject.transform, false);

            RectTransform labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            TMP_Text label = labelObject.AddComponent<TextMeshProUGUI>();
            label.text = "\uC124\uC815";
            label.fontSize = 18;
            label.alignment = TextAlignmentOptions.Center;
            label.color = new Color(0.96f, 0.94f, 0.9f, 1f);
            label.raycastTarget = false;

            return button;
        }

        private void EnsureButtonLabel(Button button, string text)
        {
            if (button.GetComponentInChildren<TMP_Text>(true) != null)
                return;

            GameObject labelObject = new("Label");
            labelObject.layer = button.gameObject.layer;
            labelObject.transform.SetParent(button.transform, false);

            RectTransform labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            TMP_Text label = labelObject.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = 18;
            label.alignment = TextAlignmentOptions.Center;
            label.color = new Color(0.96f, 0.94f, 0.9f, 1f);
            label.raycastTarget = false;
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.DayStarted -= OnDayStarted;

            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OpenOptionsPanel);

            if (_fastForwardButton != null)
                _fastForwardButton.onClick.RemoveListener(OnFastForwardClicked);
        }
    }
}
