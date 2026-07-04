using MageAcademy.Core;
using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using UnityEngine;

namespace MageAcademy.Gameplay.Flow
{
    /// <summary>학생 입장 상태. 랜덤 학생+위조를 생성해 사건을 시작하고 검증 상태로 넘어간다.</summary>
    public class StudentEnterState : GameStateBase
    {
        public StudentEnterState(GameContext context, StateMachine machine) : base(context, machine) { }

        public override void Enter()
        {
            DayConfigSO today = Context.Day.TodayConfig;
            bool includeReport = today != null && today.requiresReport;
            bool includeCrystal = today != null && today.requiresCrystal;
            bool includeUV = today != null && today.requiresUV;
            StudentCase newCase = Context.Generator.Generate(includeReport, includeCrystal, includeUV);
            if (newCase == null)
            {
                Debug.LogError("[Flow] 학생 데이터가 없어 사이클을 진행할 수 없음.");
                return;
            }

            Context.CurrentCase = newCase;
            Context.RaiseCaseStarted(newCase);
            GoNext();
        }

        public override void Exit() { }
    }
}
