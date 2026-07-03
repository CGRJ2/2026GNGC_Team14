using System.Collections.Generic;
using GuildGame.Data;

namespace GuildGame.Gameplay.Models
{
    /// <summary>
    /// 현재 손님 1명의 사건 상태(런타임). 할당된 의뢰와 항목별 거짓 여부(증언)를 보유한다.
    /// 진실 여부 판정과 증언 값 조회의 단일 출처. 생성 후 불변.
    /// </summary>
    public class AdventurerCase
    {
        private readonly Dictionary<QuestFactType, bool> _lies;

        public QuestDataSO Quest { get; }

        /// <summary>손님 이름 로컬라이제이션 키.</summary>
        public string AdventurerNameKey { get; }

        public AdventurerCase(QuestDataSO quest, Dictionary<QuestFactType, bool> lies, string adventurerNameKey)
        {
            Quest = quest;
            _lies = lies ?? new Dictionary<QuestFactType, bool>();
            AdventurerNameKey = adventurerNameKey;
        }

        /// <summary>어떤 항목도 거짓이 아니면 정직한 모험가.</summary>
        public bool IsHonest
        {
            get
            {
                foreach (var isLie in _lies.Values)
                {
                    if (isLie)
                        return false;
                }
                return true;
            }
        }

        /// <summary>지정 항목이 거짓으로 증언되는지.</summary>
        public bool IsLie(QuestFactType type)
        {
            return _lies.TryGetValue(type, out var lie) && lie;
        }

        /// <summary>해당 항목에 대해 모험가가 주장하는 값의 로컬라이제이션 키(거짓이면 거짓값).</summary>
        public string GetClaimedValueKey(QuestFactType type)
        {
            var fact = Quest != null ? Quest.GetFact(type) : null;
            if (fact == null)
                return string.Empty;

            return IsLie(type) ? fact.lieValueKey : fact.trueValueKey;
        }
    }
}
