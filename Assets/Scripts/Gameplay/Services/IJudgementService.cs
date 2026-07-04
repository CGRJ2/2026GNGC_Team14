using MageAcademy.Data;
using MageAcademy.Gameplay.Models;

namespace MageAcademy.Gameplay.Services
{
    /// <summary>사건(진짜/위조)과 플레이어 판정으로부터 4분면 결과를 도출한다.</summary>
    public interface IJudgementService
    {
        CaseOutcome Judge(IVerifiableCase verifiableCase, PlayerVerdict verdict);
    }
}
