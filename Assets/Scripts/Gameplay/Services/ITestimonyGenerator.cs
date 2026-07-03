using GuildGame.Data;
using GuildGame.Gameplay.Models;

namespace GuildGame.Gameplay.Services
{
    /// <summary>의뢰로부터 손님의 증언(정직/거짓 조합)을 생성한다.</summary>
    public interface ITestimonyGenerator
    {
        AdventurerCase Generate(QuestDataSO quest);
    }
}
