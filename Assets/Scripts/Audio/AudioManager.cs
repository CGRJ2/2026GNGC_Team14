using System.Collections.Generic;
using MageAcademy.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace MageAcademy.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        public const string MasterVolumeKey = "settings.volume.master";
        public const string BgmVolumeKey = "settings.volume.bgm";
        public const string SfxVolumeKey = "settings.volume.sfx";

        private const string MasterVolumeParameter = "MasterVolume";
        private const string BgmVolumeParameter = "BGMVolume";
        private const string SfxVolumeParameter = "SFXVolume";
        private const float MinDecibel = -80f;

        [Header("Mixer")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioMixerGroup _bgmGroup;
        [SerializeField] private AudioMixerGroup _sfxGroup;

        [Header("BGM")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioClip _initialBgm;
        [SerializeField] private bool _playInitialBgmOnAwake = true;

        [Header("SFX Pool")]
        [SerializeField] private AudioSource _sfxSourcePrefab;
        [SerializeField, Min(1)] private int _initialSfxPoolSize = 8;

        private readonly Queue<AudioSource> _availableSfxSources = new();
        private readonly List<AudioSource> _sfxSources = new();
        private readonly List<AudioSource> _playingSfxSources = new();
        private readonly Dictionary<AudioSource, float> _sfxBaseVolumes = new();

        protected override void OnAwake()
        {
            EnsureBgmSource();
            InitializeSfxPool();
            ApplySavedVolumes();

            if (_playInitialBgmOnAwake && _initialBgm != null)
                PlayBgm(_initialBgm);
        }

        private void Update()
        {
            RecycleFinishedSfxSources();
        }

        public void PlayBgm(AudioClip clip, bool loop = true)
        {
            if (clip == null)
                return;

            EnsureBgmSource();
            if (_bgmSource.clip == clip && _bgmSource.isPlaying)
                return;

            _bgmSource.clip = clip;
            _bgmSource.loop = loop;
            _bgmSource.outputAudioMixerGroup = _bgmGroup;
            _bgmSource.Play();
        }

        public void StopBgm()
        {
            if (_bgmSource != null)
                _bgmSource.Stop();
        }

        public AudioSource PlaySfx(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
        {
            if (clip == null)
                return null;

            AudioSource source = GetSfxSource();
            source.clip = clip;
            source.volume = Mathf.Clamp01(GetSfxBaseVolume(source) * Mathf.Clamp01(volumeScale));
            source.pitch = Mathf.Max(0.01f, pitch);
            source.loop = false;
            source.outputAudioMixerGroup = _sfxGroup;
            source.gameObject.SetActive(true);
            source.Play();
            _playingSfxSources.Add(source);
            return source;
        }

        public void SetMasterVolume(float value, bool save = true)
        {
            SetVolume(MasterVolumeKey, MasterVolumeParameter, value, save);
        }

        public void SetBgmVolume(float value, bool save = true)
        {
            SetVolume(BgmVolumeKey, BgmVolumeParameter, value, save);
        }

        public void SetSfxVolume(float value, bool save = true)
        {
            SetVolume(SfxVolumeKey, SfxVolumeParameter, value, save);
        }

        public float GetMasterVolume() => PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        public float GetBgmVolume() => PlayerPrefs.GetFloat(BgmVolumeKey, 1f);
        public float GetSfxVolume() => PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

        public void ApplySavedVolumes()
        {
            SetMasterVolume(GetMasterVolume(), save: false);
            SetBgmVolume(GetBgmVolume(), save: false);
            SetSfxVolume(GetSfxVolume(), save: false);
        }

        public static float LinearToDecibel(float value)
        {
            if (value <= 0.0001f)
                return MinDecibel;

            return Mathf.Log10(value) * 20f;
        }

        private void SetVolume(string key, string mixerParameter, float value, bool save)
        {
            value = Mathf.Clamp01(value);

            if (_audioMixer != null)
                _audioMixer.SetFloat(mixerParameter, LinearToDecibel(value));
            else if (mixerParameter == MasterVolumeParameter)
                AudioListener.volume = value;

            if (!save)
                return;

            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }

        private void EnsureBgmSource()
        {
            if (_bgmSource == null)
                _bgmSource = gameObject.AddComponent<AudioSource>();

            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
            _bgmSource.outputAudioMixerGroup = _bgmGroup;
        }

        private void InitializeSfxPool()
        {
            for (int i = _sfxSources.Count; i < _initialSfxPoolSize; i++)
                CreateSfxSource();
        }

        private AudioSource GetSfxSource()
        {
            if (_availableSfxSources.Count == 0)
                CreateSfxSource();

            return _availableSfxSources.Dequeue();
        }

        private AudioSource CreateSfxSource()
        {
            AudioSource source;
            GameObject sourceObject;

            if (_sfxSourcePrefab != null)
            {
                source = Instantiate(_sfxSourcePrefab, transform);
                sourceObject = source.gameObject;
                sourceObject.name = $"SFX_Source_{_sfxSources.Count:00}";
            }
            else
            {
                sourceObject = new GameObject($"SFX_Source_{_sfxSources.Count:00}");
                sourceObject.transform.SetParent(transform);
                source = sourceObject.AddComponent<AudioSource>();
                source.volume = 0.7f;
            }

            sourceObject.transform.SetParent(transform);
            sourceObject.SetActive(false);

            source.playOnAwake = false;
            source.outputAudioMixerGroup = _sfxGroup;
            _sfxBaseVolumes[source] = Mathf.Clamp01(source.volume);

            _sfxSources.Add(source);
            _availableSfxSources.Enqueue(source);
            return source;
        }

        private float GetSfxBaseVolume(AudioSource source)
        {
            return source != null && _sfxBaseVolumes.TryGetValue(source, out float volume)
                ? volume
                : 0.7f;
        }

        private void RecycleFinishedSfxSources()
        {
            for (int i = _playingSfxSources.Count - 1; i >= 0; i--)
            {
                AudioSource source = _playingSfxSources[i];
                if (source == null || source.isPlaying)
                    continue;

                _playingSfxSources.RemoveAt(i);
                source.Stop();
                source.clip = null;
                source.pitch = 1f;
                source.volume = GetSfxBaseVolume(source);
                source.gameObject.SetActive(false);
                _availableSfxSources.Enqueue(source);
            }
        }
    }
}
