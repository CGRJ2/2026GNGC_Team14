namespace GuildGame.Data
{
    /// <summary>
    /// 플레이어 판정 × 실제 진위의 4분면 결과.
    /// TruthSuccess / FalseCaught = 올바른 판정, FalseApproved / TruthMisjudged = 오판.
    /// </summary>
    public enum CaseOutcome
    {
        TruthSuccess,    // 정직 → 완료 승인: 보상 지급, 평판 상승
        FalseCaught,     // 거짓 → 실패 판정: 거짓 간파, 평판 상승
        FalseApproved,   // 거짓 → 완료 승인: 도주, 의뢰 미해결, 평판 하락
        TruthMisjudged   // 정직 → 실패 판정: 억울한 항의, 평판 하락
    }
}
