using MageAcademy.Data;
using MageAcademy.Gameplay.Models;

namespace MageAcademy.Gameplay.Services
{
    /// <summary>
    /// 판정 매트릭스. 순수하게 (진짜/위조) × (승인/거부) → 결과 enum만 계산한다(SRP).
    /// 평판 증감·이벤트 대사는 <see cref="GameBalanceSO"/>가 담당.
    /// </summary>
    public class JudgementService : IJudgementService
    {
        public CaseOutcome Judge(IVerifiableCase verifiableCase, PlayerVerdict verdict)
        {
            bool genuine = verifiableCase != null && verifiableCase.IsHonest;
            bool approve = verdict == PlayerVerdict.ApproveComplete;

            if (genuine)
                return approve ? CaseOutcome.TruthSuccess : CaseOutcome.TruthMisjudged;

            return approve ? CaseOutcome.FalseApproved : CaseOutcome.FalseCaught;
        }
    }
}
