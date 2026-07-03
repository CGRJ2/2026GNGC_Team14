namespace GuildGame.Core
{
    /// <summary>
    /// 단순 유한 상태 머신. 이전 상태 Exit → 참조 교체 → 새 상태 Enter 순으로 전이한다.
    /// 직접 필드 대입 대신 반드시 <see cref="ChangeState"/>를 사용한다.
    /// </summary>
    public class StateMachine
    {
        public IState CurrentState { get; private set; }

        public void ChangeState(IState next)
        {
            if (next == null)
                return;

            CurrentState?.Exit();
            CurrentState = next;
            CurrentState.Enter();
        }

        /// <summary>현재 상태의 프레임 갱신을 전달한다.</summary>
        public void Tick()
        {
            CurrentState?.Tick();
        }
    }
}
