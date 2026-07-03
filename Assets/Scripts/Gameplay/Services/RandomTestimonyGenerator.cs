using System.Collections.Generic;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using UnityEngine;

namespace GuildGame.Gameplay.Services
{
    /// <summary>
    /// 확률적으로 정직/거짓 증언을 만든다. 거짓이면 의뢰 항목 중 1개 이상을 무작위로 조작한다.
    /// 거짓값 자체는 디자이너가 <see cref="QuestFact.lieValueKey"/>에 authoring한 것을 사용한다.
    /// </summary>
    public class RandomTestimonyGenerator : ITestimonyGenerator
    {
        private readonly float _lieChance;
        private readonly IReadOnlyList<string> _nameKeys;

        public RandomTestimonyGenerator(float lieChance, IReadOnlyList<string> nameKeys)
        {
            _lieChance = Mathf.Clamp01(lieChance);
            _nameKeys = nameKeys;
        }

        public AdventurerCase Generate(QuestDataSO quest)
        {
            var lies = new Dictionary<QuestFactType, bool>();

            if (quest != null)
            {
                foreach (var fact in quest.facts)
                    lies[fact.type] = false;

                bool willLie = Random.value < _lieChance && quest.facts.Count > 0;
                if (willLie)
                    AssignLies(quest, lies);
            }

            return new AdventurerCase(quest, lies, PickName());
        }

        /// <summary>최소 1개 항목을 거짓으로 지정하고, 나머지도 확률적으로 추가 조작한다.</summary>
        private static void AssignLies(QuestDataSO quest, Dictionary<QuestFactType, bool> lies)
        {
            int count = quest.facts.Count;

            // 반드시 하나는 거짓.
            int guaranteed = Random.Range(0, count);
            lies[quest.facts[guaranteed].type] = true;

            // 나머지는 각 25% 확률로 추가 거짓(여러 항목이 어긋날 수 있음).
            for (int i = 0; i < count; i++)
            {
                if (i == guaranteed)
                    continue;
                if (Random.value < 0.25f)
                    lies[quest.facts[i].type] = true;
            }
        }

        private string PickName()
        {
            if (_nameKeys == null || _nameKeys.Count == 0)
                return string.Empty;
            return _nameKeys[Random.Range(0, _nameKeys.Count)];
        }
    }
}
