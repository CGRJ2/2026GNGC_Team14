using System.Collections.Generic;
using MageAcademy.Audio;
using MageAcademy.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    public class OptionsPanelView : MonoBehaviour
    {
        private const string LanguageKey = "settings.language";
        private const string ScreenModeKey = "settings.screenMode";

        [Tooltip("AudioManager가 없는 테스트 씬에서만 사용하는 fallback mixer")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_Dropdown _screenModeDropdown;
        [SerializeField] private TMP_Dropdown _languageDropdown;
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _bgmVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        private readonly List<FullScreenMode> _screenModes = new()
        {
            FullScreenMode.ExclusiveFullScreen,
            FullScreenMode.Windowed,
            FullScreenMode.FullScreenWindow,
        };

        private void Awake()
        {
            BindControls();
            LoadSettings();
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        private void BindControls()
        {
            if (_closeButton != null)
                _closeButton.onClick.AddListener(Hide);

            if (_screenModeDropdown != null)
            {
                _screenModeDropdown.ClearOptions();
                _screenModeDropdown.AddOptions(new List<string> { "전체화면", "창모드", "테두리없음" });
                _screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
            }

            if (_languageDropdown != null)
            {
                _languageDropdown.ClearOptions();
                _languageDropdown.AddOptions(new List<string> { "한국어", "English" });
                _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
            }

            if (_masterVolumeSlider != null)
                _masterVolumeSlider.onValueChanged.AddListener(ApplyMasterVolume);
            if (_bgmVolumeSlider != null)
                _bgmVolumeSlider.onValueChanged.AddListener(ApplyBgmVolume);
            if (_sfxVolumeSlider != null)
                _sfxVolumeSlider.onValueChanged.AddListener(ApplySfxVolume);
        }

        private void LoadSettings()
        {
            float masterVolume = PlayerPrefs.GetFloat(AudioManager.MasterVolumeKey, 1f);
            float bgmVolume = PlayerPrefs.GetFloat(AudioManager.BgmVolumeKey, 1f);
            float sfxVolume = PlayerPrefs.GetFloat(AudioManager.SfxVolumeKey, 1f);

            SetSliderValue(_masterVolumeSlider, masterVolume);
            SetSliderValue(_bgmVolumeSlider, bgmVolume);
            SetSliderValue(_sfxVolumeSlider, sfxVolume);

            ApplyMasterVolume(masterVolume, save: false);
            ApplyBgmVolume(bgmVolume, save: false);
            ApplySfxVolume(sfxVolume, save: false);

            var savedScreenMode = (FullScreenMode)PlayerPrefs.GetInt(ScreenModeKey, (int)Screen.fullScreenMode);
            int screenModeIndex = Mathf.Max(0, _screenModes.IndexOf(savedScreenMode));
            if (_screenModeDropdown != null)
                _screenModeDropdown.SetValueWithoutNotify(screenModeIndex);
            ApplyScreenMode(_screenModes[screenModeIndex], save: false);

            var language = (Language)PlayerPrefs.GetInt(LanguageKey, (int)Language.Korean);
            if (_languageDropdown != null)
                _languageDropdown.SetValueWithoutNotify(language == Language.English ? 1 : 0);
            ApplyLanguage(language, save: false);
        }

        private static void SetSliderValue(Slider slider, float value)
        {
            if (slider != null)
                slider.SetValueWithoutNotify(value);
        }

        private void OnScreenModeChanged(int index)
        {
            index = Mathf.Clamp(index, 0, _screenModes.Count - 1);
            ApplyScreenMode(_screenModes[index]);
        }

        private void OnLanguageChanged(int index)
        {
            ApplyLanguage(index == 1 ? Language.English : Language.Korean);
        }

        private void ApplyScreenMode(FullScreenMode mode, bool save = true)
        {
            Screen.fullScreenMode = mode;
            Screen.fullScreen = mode != FullScreenMode.Windowed;

            if (!save)
                return;

            PlayerPrefs.SetInt(ScreenModeKey, (int)mode);
            PlayerPrefs.Save();
        }

        private void ApplyLanguage(Language language, bool save = true)
        {
            LocalizationManager.Instance.SetLanguage(language);

            if (!save)
                return;

            PlayerPrefs.SetInt(LanguageKey, (int)language);
            PlayerPrefs.Save();
        }

        private void ApplyMasterVolume(float value)
        {
            ApplyMasterVolume(value, save: true);
        }

        private void ApplyBgmVolume(float value)
        {
            ApplyBgmVolume(value, save: true);
        }

        private void ApplySfxVolume(float value)
        {
            ApplySfxVolume(value, save: true);
        }

        private void ApplyMasterVolume(float value, bool save)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(value, save);
                return;
            }

            ApplyFallbackVolume(AudioManager.MasterVolumeKey, "MasterVolume", value, save);
        }

        private void ApplyBgmVolume(float value, bool save)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetBgmVolume(value, save);
                return;
            }

            ApplyFallbackVolume(AudioManager.BgmVolumeKey, "BGMVolume", value, save);
        }

        private void ApplySfxVolume(float value, bool save)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSfxVolume(value, save);
                return;
            }

            ApplyFallbackVolume(AudioManager.SfxVolumeKey, "SFXVolume", value, save);
        }

        private void ApplyFallbackVolume(string key, string mixerParameter, float value, bool save)
        {
            value = Mathf.Clamp01(value);

            if (_audioMixer != null)
                _audioMixer.SetFloat(mixerParameter, AudioManager.LinearToDecibel(value));
            else if (mixerParameter == "MasterVolume")
                AudioListener.volume = value;

            if (!save)
                return;

            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }

        private void OnDestroy()
        {
            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(Hide);
            if (_screenModeDropdown != null)
                _screenModeDropdown.onValueChanged.RemoveListener(OnScreenModeChanged);
            if (_languageDropdown != null)
                _languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
            if (_masterVolumeSlider != null)
                _masterVolumeSlider.onValueChanged.RemoveListener(ApplyMasterVolume);
            if (_bgmVolumeSlider != null)
                _bgmVolumeSlider.onValueChanged.RemoveListener(ApplyBgmVolume);
            if (_sfxVolumeSlider != null)
                _sfxVolumeSlider.onValueChanged.RemoveListener(ApplySfxVolume);
        }
    }
}
