namespace GuildGame.Gameplay.Models
{
    /// <summary>
    /// 판정 서비스가 다루는 사건의 최소 계약. 정직(=진짜) 여부만 노출한다.
    /// 학생증(StudentCase) 등 구체 사건이 구현한다.
    /// </summary>
    public interface IVerifiableCase
    {
        /// <summary>어떤 항목도 위조/거짓이 아니면 true.</summary>
        bool IsHonest { get; }
    }
}
