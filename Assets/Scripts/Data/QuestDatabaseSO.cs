using System.Collections.Generic;
using UnityEngine;

namespace GuildGame.Data
{
    /// <summary>의뢰 풀. 모험가 입장 시 여기서 무작위로 한 건을 뽑는다.</summary>
    [CreateAssetMenu(fileName = "QuestDatabase", menuName = "GuildGame/Quest Database", order = 10)]
    public class QuestDatabaseSO : ScriptableObject
    {
        public List<QuestDataSO> quests = new();

        public bool IsEmpty => quests == null || quests.Count == 0;

        /// <summary>무작위 의뢰 하나를 반환. 풀이 비면 null.</summary>
        public QuestDataSO GetRandom()
        {
            if (IsEmpty)
                return null;

            int index = Random.Range(0, quests.Count);
            return quests[index];
        }
    }
}
