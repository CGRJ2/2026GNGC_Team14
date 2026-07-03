namespace GuildGame.Core
{
    /// <summary>
    /// 상태 머신의 단일 상태. 각 상태는 하나의 책임만 가진다(SRP).
    /// 전이는 반드시 <see cref="StateMachine.ChangeState"/>를 통해서만 이뤄진다.
    /// </summary>
    public interface IState
    {
        /// <summary>상태 진입 시 1회 호출.</summary>
        void Enter();

        /// <summary>매 프레임 호출(필요 없으면 비워둔다).</summary>
        void Tick();

        /// <summary>상태 이탈 시 1회 호출. 구독 해제 등 정리.</summary>
        void Exit();
    }
}
