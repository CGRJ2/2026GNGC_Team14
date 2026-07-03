using System;
using UnityEngine;

namespace GuildGame.Data
{
    /// <summary>
    /// 판정 결과별 평판 증감과 이벤트 대사 키, 그리고 사이클 파라미터를 모은 밸런스 설정.
    /// 수치는 placeholder이며 game_designer가 튜닝한다. 매직 넘버를 코드에서 제거하는 단일 출처.
    /// </summary>
    [CreateAssetMenu(fileName = "GameBalance", menuName = "GuildGame/Game Balance", order = 20)]
    public class GameBalanceSO : ScriptableObject
    {
        [Serializable]
        public class OutcomeInfo
        {
            [Tooltip("이 결과에서의 평판 증감")]
            public int reputationDelta;

            [Tooltip("결과 이벤트 대사 로컬라이제이션 키")]
            public string eventKey;
        }

        [Header("사이클 파라미터")]
        [Tooltip("시작 평판")]
        public int startingReputation = 10;

        [Range(0f, 1f)]
        [Tooltip("학생이 거짓을 포함할 확률")]
        public float lieChance = 0.5f;

        [Header("결과별 평판/이벤트")]
        public OutcomeInfo truthSuccess = new() { reputationDelta = 1, eventKey = "outcome_truth_success" };
        public OutcomeInfo falseCaught = new() { reputationDelta = 1, eventKey = "outcome_false_caught" };
        public OutcomeInfo falseApproved = new() { reputationDelta = -2, eventKey = "outcome_false_approved" };
        public OutcomeInfo truthMisjudged = new() { reputationDelta = -1, eventKey = "outcome_truth_misjudged" };

        public OutcomeInfo GetInfo(CaseOutcome outcome)
        {
            switch (outcome)
            {
                case CaseOutcome.TruthSuccess: return truthSuccess;
                case CaseOutcome.FalseCaught: return falseCaught;
                case CaseOutcome.FalseApproved: return falseApproved;
                case CaseOutcome.TruthMisjudged: return truthMisjudged;
                default: return truthSuccess;
            }
        }
    }
}
