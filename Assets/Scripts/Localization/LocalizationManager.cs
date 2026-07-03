using System.Collections.Generic;
using GuildGame.Core;
using UnityEngine;

namespace GuildGame.Localization
{
    /// <summary>
    /// Resources의 CSV 표(key, ko, en, ...)를 로드해 키→언어별 텍스트로 보관하는 매니저.
    /// 언어 전환 시 <see cref="OnLanguageChanged"/>를 발행해 View가 갱신하도록 한다.
    /// </summary>
    public class LocalizationManager : Singleton<LocalizationManager>, ILocalizationProvider
    {
        [Tooltip("Resources 하위 CSV 파일 경로(확장자 제외).")]
        [SerializeField] private string _resourcePath = "Localization";

        [SerializeField] private Language _currentLanguage = Language.Korean;

        // key -> (Language -> text)
        private readonly Dictionary<string, Dictionary<Language, string>> _table = new();

        // CSV 컬럼 인덱스 -> Language
        private readonly Dictionary<int, Language> _columnToLanguage = new();

        private bool _loaded;

        /// <summary>언어가 바뀌었을 때 발행. 인자는 새 언어.</summary>
        public event System.Action<Language> OnLanguageChanged;

        public Language CurrentLanguage => _currentLanguage;

        protected override void OnAwake()
        {
            EnsureLoaded();
        }

        private void EnsureLoaded()
        {
            if (_loaded)
                return;

            var asset = Resources.Load<TextAsset>(_resourcePath);
            if (asset == null)
            {
                Debug.LogError($"[Localization] CSV를 찾을 수 없음: Resources/{_resourcePath}");
                _loaded = true;
                return;
            }

            LoadFromText(asset.text);
        }

        /// <summary>CSV 원문에서 표를 구축한다(테스트에서도 직접 호출 가능).</summary>
        public void LoadFromText(string csv)
        {
            _table.Clear();
            _columnToLanguage.Clear();

            var rows = CsvParser.Parse(csv);
            if (rows.Count == 0)
            {
                Debug.LogWarning("[Localization] CSV가 비어 있음.");
                _loaded = true;
                return;
            }

            // 헤더: 0번은 key, 이후 컬럼명을 Language로 매핑.
            var header = rows[0];
            for (int col = 1; col < header.Length; col++)
            {
                if (TryParseLanguage(header[col], out var lang))
                    _columnToLanguage[col] = lang;
                else
                    Debug.LogWarning($"[Localization] 알 수 없는 언어 컬럼 무시: '{header[col]}'");
            }

            for (int r = 1; r < rows.Count; r++)
            {
                var row = rows[r];
                if (row.Length == 0)
                    continue;

                string key = row[0].Trim();
                if (string.IsNullOrEmpty(key))
                    continue;

                var byLang = new Dictionary<Language, string>();
                foreach (var kv in _columnToLanguage)
                {
                    int col = kv.Key;
                    if (col < row.Length)
                        byLang[kv.Value] = row[col];
                }
                _table[key] = byLang;
            }

            _loaded = true;
            Debug.Log($"[Localization] 로드 완료: {_table.Count}개 키, 언어 {_columnToLanguage.Count}종.");
        }

        public string Get(string key)
        {
            EnsureLoaded();

            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (_table.TryGetValue(key, out var byLang))
            {
                if (byLang.TryGetValue(_currentLanguage, out var text) && !string.IsNullOrEmpty(text))
                    return text;

                // 현재 언어 값이 비면 한국어로 폴백.
                if (byLang.TryGetValue(Language.Korean, out var ko) && !string.IsNullOrEmpty(ko))
                    return ko;
            }

            Debug.LogWarning($"[Localization] 누락 키: '{key}'");
            return $"!{key}!";
        }

        public string GetFormatted(string key, params object[] args)
        {
            string template = Get(key);
            if (args == null || args.Length == 0)
                return template;

            try
            {
                return string.Format(template, args);
            }
            catch (System.FormatException)
            {
                Debug.LogWarning($"[Localization] 포맷 실패 키: '{key}' template='{template}'");
                return template;
            }
        }

        public void SetLanguage(Language language)
        {
            if (_currentLanguage == language)
                return;

            _currentLanguage = language;
            OnLanguageChanged?.Invoke(_currentLanguage);
        }

        private static bool TryParseLanguage(string column, out Language language)
        {
            switch (column.Trim().ToLowerInvariant())
            {
                case "ko":
                case "korean":
                    language = Language.Korean;
                    return true;
                case "en":
                case "english":
                    language = Language.English;
                    return true;
                default:
                    language = Language.Korean;
                    return false;
            }
        }
    }
}
