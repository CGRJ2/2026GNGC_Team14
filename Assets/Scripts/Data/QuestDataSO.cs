using System.Collections.Generic;
using UnityEngine;

namespace GuildGame.Data
{
    /// <summary>
    /// 의뢰 1건의 정적 데이터. 키 ID + 표시용 로컬라이제이션 키 + 검증 항목 목록(진실).
    /// 로직은 갖지 않는다(순수 데이터).
    /// </summary>
    [CreateAssetMenu(fileName = "Quest_", menuName = "GuildGame/Quest Data", order = 0)]
    public class QuestDataSO : ScriptableObject
    {
        [Tooltip("고유 식별자")]
        public string questId;

        [Tooltip("의뢰 제목 로컬라이제이션 키")]
        public string titleKey;

        [Tooltip("의뢰 개요 로컬라이제이션 키")]
        public string summaryKey;

        [Tooltip("검증 가능한 세부 항목(참/거짓 쌍) 목록")]
        public List<QuestFact> facts = new();

        /// <summary>지정 타입의 항목을 반환. 없으면 null.</summary>
        public QuestFact GetFact(QuestFactType type)
        {
            for (int i = 0; i < facts.Count; i++)
            {
                if (facts[i].type == type)
                    return facts[i];
            }
            return null;
        }
    }
}
