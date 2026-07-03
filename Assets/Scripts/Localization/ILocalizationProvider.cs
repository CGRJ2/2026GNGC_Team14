using System.Collections.Generic;

namespace GuildGame.Localization
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
    }
}
