using GuildGame.Data;
using GuildGame.Gameplay.Models;

namespace GuildGame.Gameplay.Services
{
    /// <summary>
    /// 판정 매트릭스 구현. 평판 증감·이벤트 대사 매핑은 <see cref="GameBalanceSO"/>가 담당하고,
    /// 여기서는 순수하게 판정(정직/거짓 × 승인/실패) → 결과 enum만 계산한다(SRP).
    /// </summary>
    public class JudgementService : IJudgementService
    {
        public CaseOutcome Judge(AdventurerCase adventurerCase, PlayerVerdict verdict)
        {
            bool honest = adventurerCase != null && adventurerCase.IsHonest;
            bool approve = verdict == PlayerVerdict.ApproveComplete;

            if (honest)
                return approve ? CaseOutcome.TruthSuccess : CaseOutcome.TruthMisjudged;

            return approve ? CaseOutcome.FalseApproved : CaseOutcome.FalseCaught;
        }
    }
}
