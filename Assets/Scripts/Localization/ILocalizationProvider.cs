using System.Collections.Generic;

namespace MageAcademy.Localization
{
    public interface ILocalizationProvider
    {
        string Get(string key);

        IReadOnlyList<string> GetAll(string key);

        string GetRandom(string key);

        string GetFormatted(string key, params object[] args);

        string GetFormattedRandom(string key, params object[] args);

        Language CurrentLanguage { get; }

        void SetLanguage(Language language);

        /// <summary>언어가 바뀌면 발행된다. View가 구독해 표시 텍스트를 갱신한다.</summary>
        event System.Action<Language> OnLanguageChanged;
    }
}
