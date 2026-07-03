namespace GuildGame.Data
{
    /// <summary>
    /// 의뢰의 검증 가능한 항목 종류. 질문(<see cref="QuestionSO"/>)이 이 타입을 캐묻고,
    /// 의뢰서(<see cref="QuestDataSO"/>)가 이 타입별 참/거짓 값을 보유한다.
    /// 컬럼 추가 지점: 새 검증 항목이 필요하면 여기에 추가한다.
    /// </summary>
    public enum QuestFactType
    {
        TargetName,   // 처리 대상(고블린/월광초 등)
        TargetCount,  // 수량
        Location,     // 수행 장소
        Difficulty    // 난이도
    }
}
