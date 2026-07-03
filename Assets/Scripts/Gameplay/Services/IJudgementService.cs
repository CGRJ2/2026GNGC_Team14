using GuildGame.Data;
using GuildGame.Gameplay.Models;

namespace GuildGame.Gameplay.Services
{
    /// <summary>사건과 플레이어 판정으로부터 4분면 결과를 도출한다.</summary>
    public interface IJudgementService
    {
        CaseOutcome Judge(AdventurerCase adventurerCase, PlayerVerdict verdict);
    }
}
