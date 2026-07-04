namespace MageAcademy.Data
{
    /// <summary>학생 표정 상태. 일러스트 스왑에 사용한다.</summary>
    public enum StudentEmotion
    {
        Normal,     // 기본
        Happy,      // 기쁨 (참·통과)
        Flustered,  // 당황 (거짓 정보 추궁)
        Angry,      // 화남 (검사 시간 초과)
        Sneer       // 비열 (거짓·통과)
    }
}
