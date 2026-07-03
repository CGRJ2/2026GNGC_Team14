using GuildGame.Core;

namespace GuildGame.Gameplay.Flow
{
    /// <summary>
    /// 게임 사이클 상태의 공통 베이스. 컨텍스트와 상태 머신 참조를 갖고,
    /// 선형 전이(<see cref="Next"/>)를 제공한다. 구체 상태는 자기 책임만 구현한다.
    /// </summary>
    public abstract class GameStateBase : IState
    {
        protected readonly GameContext Context;
        protected readonly StateMachine Machine;

        /// <summary>이 상태 다음으로 전이할 상태(컨트롤러가 배선).</summary>
        public IState Next { get; set; }

        protected GameStateBase(GameContext context, StateMachine machine)
        {
            Context = context;
            Machine = machine;
        }

        public abstract void Enter();
        public virtual void Tick() { }
        public abstract void Exit();

        protected void GoNext()
        {
            Machine.ChangeState(Next);
        }
    }
}
