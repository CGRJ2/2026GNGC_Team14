using System.Collections.Generic;
using GuildGame.Core;
using GuildGame.Data;
using UnityEngine;

namespace GuildGame.Gameplay.Managers
{
    /// <summary>
    /// 의뢰/질문 데이터베이스에 대한 접근 파사드. 무작위 의뢰 제공과 의뢰별 질문 조회를 담당한다.
    /// </summary>
    public class QuestManager : Singleton<QuestManager>
    {
        [SerializeField] private QuestDatabaseSO _questDatabase;
        [SerializeField] private QuestionDatabaseSO _questionDatabase;

        public QuestDataSO GetRandomQuest()
        {
            if (_questDatabase == null || _questDatabase.IsEmpty)
            {
                Debug.LogError("[QuestManager] 의뢰 데이터베이스가 비어 있음.");
                return null;
            }
            return _questDatabase.GetRandom();
        }

        public List<QuestionSO> GetQuestionsFor(QuestDataSO quest)
        {
            if (_questionDatabase == null)
            {
                Debug.LogError("[QuestManager] 질문 데이터베이스가 없음.");
                return new List<QuestionSO>();
            }
            return _questionDatabase.GetQuestionsFor(quest);
        }
    }
}
