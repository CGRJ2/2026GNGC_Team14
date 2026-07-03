using GuildGame.Core;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using UnityEngine;

namespace GuildGame.Gameplay.Flow
{
    /// <summary>
    /// 손님 입장 상태. 무작위 의뢰를 뽑아 증언을 생성하고 사건을 시작한 뒤 즉시 검증 상태로 넘어간다.
    /// </summary>
    public class AdventurerEnterState : GameStateBase
    {
        public AdventurerEnterState(GameContext context, StateMachine machine) : base(context, machine) { }

        public override void Enter()
        {
            QuestDataSO quest = Context.Quests.GetRandomQuest();
            if (quest == null)
            {
                Debug.LogError("[Flow] 뽑을 의뢰가 없어 사이클을 진행할 수 없음.");
                return;
            }

            AdventurerCase newCase = Context.TestimonyGenerator.Generate(quest);
            Context.CurrentCase = newCase;

            Context.RaiseCaseStarted(newCase);
            GoNext();
        }

        public override void Exit() { }
    }
}
