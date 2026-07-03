using System.Collections.Generic;
using UnityEngine;

namespace GuildGame.Data
{
    /// <summary>질문 풀. 특정 의뢰가 가진 항목들에 해당하는 질문만 골라 제시한다.</summary>
    [CreateAssetMenu(fileName = "QuestionDatabase", menuName = "GuildGame/Question Database", order = 11)]
    public class QuestionDatabaseSO : ScriptableObject
    {
        public List<QuestionSO> questions = new();

        /// <summary>주어진 의뢰의 항목(fact)에 대응하는 질문만 반환한다.</summary>
        public List<QuestionSO> GetQuestionsFor(QuestDataSO quest)
        {
            var result = new List<QuestionSO>();
            if (quest == null)
                return result;

            foreach (var q in questions)
            {
                if (q != null && quest.GetFact(q.targetFact) != null)
                    result.Add(q);
            }
            return result;
        }
    }
}
