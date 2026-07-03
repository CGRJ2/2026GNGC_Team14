using System.Collections.Generic;
using GuildGame.Core;
using UnityEngine;

namespace GuildGame.Localization
{
    public class LocalizationManager : Singleton<LocalizationManager>, ILocalizationProvider
    {
        [Tooltip("CSV file path under Resources without extension.")]
        [SerializeField] private string _resourcePath = "Localization";

        [SerializeField] private Language _currentLanguage = Language.Korean;

        private readonly Dictionary<string, Dictionary<Language, List<string>>> _table = new();
        private readonly Dictionary<int, Language> _columnToLanguage = new();

        private bool _loaded;

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
                Debug.LogError($"[Localization] CSV not found: Resources/{_resourcePath}");
                _loaded = true;
                return;
            }

            LoadFromText(asset.text);
        }

        public void LoadFromText(string csv)
        {
            _table.Clear();
            _columnToLanguage.Clear();

            var rows = CsvParser.Parse(csv);
            if (rows.Count == 0)
            {
                Debug.LogWarning("[Localization] CSV is empty.");
                _loaded = true;
                return;
            }

            var header = rows[0];
            for (int col = 1; col < header.Length; col++)
            {
                if (TryParseLanguage(header[col], out var lang))
                    _columnToLanguage[col] = lang;
                else
                    Debug.LogWarning($"[Localization] Ignored unknown language column: '{header[col]}'");
            }

            string currentKey = null;
            for (int r = 1; r < rows.Count; r++)
            {
                var row = rows[r];
                if (row.Length == 0)
                    continue;

                string key = row[0].Trim();
                if (string.IsNullOrEmpty(key))
                {
                    if (!string.IsNullOrEmpty(currentKey))
                        AppendRowVariants(currentKey, row);
                    continue;
                }

                currentKey = key;
                _table[key] = new Dictionary<Language, List<string>>();
                AppendRowVariants(key, row);
            }

            _loaded = true;
            Debug.Log($"[Localization] Loaded {_table.Count} keys, {_columnToLanguage.Count} languages.");
        }

        public string Get(string key)
        {
            if (!TryGetTexts(key, out List<string> texts))
                return $"!{key}!";

            return texts[0];
        }

        public string GetRandom(string key)
        {
            if (!TryGetTexts(key, out List<string> texts))
                return $"!{key}!";

            return texts[Random.Range(0, texts.Count)];
        }

        public string GetFormatted(string key, params object[] args)
        {
            return FormatTemplate(key, Get(key), args);
        }

        public string GetFormattedRandom(string key, params object[] args)
        {
            return FormatTemplate(key, GetRandom(key), args);
        }

        public void SetLanguage(Language language)
        {
            if (_currentLanguage == language)
                return;

            _currentLanguage = language;
            OnLanguageChanged?.Invoke(_currentLanguage);
        }

        private void AppendRowVariants(string key, string[] row)
        {
            if (!_table.TryGetValue(key, out var byLang))
            {
                byLang = new Dictionary<Language, List<string>>();
                _table[key] = byLang;
            }

            foreach (var kv in _columnToLanguage)
            {
                int col = kv.Key;
                if (col >= row.Length)
                    continue;

                string text = row[col];
                if (string.IsNullOrEmpty(text))
                    continue;

                if (!byLang.TryGetValue(kv.Value, out List<string> variants))
                {
                    variants = new List<string>();
                    byLang[kv.Value] = variants;
                }

                variants.Add(text);
            }
        }

        private bool TryGetTexts(string key, out List<string> texts)
        {
            EnsureLoaded();
            texts = null;

            if (string.IsNullOrEmpty(key))
            {
                texts = new List<string> { string.Empty };
                return true;
            }

            if (_table.TryGetValue(key, out var byLang))
            {
                if (byLang.TryGetValue(_currentLanguage, out texts) && texts.Count > 0)
                    return true;

                if (byLang.TryGetValue(Language.Korean, out texts) && texts.Count > 0)
                    return true;
            }

            Debug.LogWarning($"[Localization] Missing key: '{key}'");
            return false;
        }

        private static string FormatTemplate(string key, string template, params object[] args)
        {
            if (args == null || args.Length == 0)
                return template;

            try
            {
                return string.Format(template, args);
            }
            catch (System.FormatException)
            {
                Debug.LogWarning($"[Localization] Format failed: '{key}' template='{template}'");
                return template;
            }
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
