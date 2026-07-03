namespace GuildGame.Localization
{
    /// <summary>로컬라이제이션 조회 추상화. 소비자는 구현이 아닌 이 인터페이스에 의존한다(DIP).</summary>
    public interface ILocalizationProvider
    {
        /// <summary>현재 언어로 키에 해당하는 텍스트를 반환. 누락 시 <c>!key!</c> 폴백.</summary>
        string Get(string key);

        /// <summary>템플릿 텍스트를 조회한 뒤 <see cref="string.Format(string, object[])"/>로 인자를 대입한다.</summary>
        string GetFormatted(string key, params object[] args);

        Language CurrentLanguage { get; }

        void SetLanguage(Language language);
    }
}
