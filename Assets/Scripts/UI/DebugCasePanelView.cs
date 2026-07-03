using System.Text;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// 디버그용 뷰. 현재 학생 사건의 "정답표"를 표시한다:
    /// 각 조건의 참값·증언값·참/거짓 상태와 전체 정직 여부(통과/탈락 정답).
    /// 개발 편의용이며 배포 시 비활성화한다.
    /// </summary>
    public class DebugCasePanelView : UIViewBase
    {
        [SerializeField] private TMP_Text _label;

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
        }

        private void OnCaseStarted(AdventurerCase adventurerCase)
        {
            if (_label == null)
                return;

            if (adventurerCase == null || adventurerCase.Quest == null)
            {
                _label.text = string.Empty;
                return;
            }

            var loc = Context.Localization;
            var sb = new StringBuilder();

            sb.AppendLine("<b>🔍 디버그 · 정답표</b>");
            sb.AppendLine($"학생: {loc.Get(adventurerCase.AdventurerNameKey)}");
            sb.AppendLine(adventurerCase.IsHonest
                ? "<color=#7CFC7C>정답: 정직 → 통과시켜야 함</color>"
                : "<color=#FF7C7C>정답: 거짓 → 탈락시켜야 함</color>");
            sb.AppendLine("──────────────");

            foreach (var fact in adventurerCase.Quest.facts)
            {
                string label = loc.Get(FactLabelKey(fact.type));
                string truth = loc.Get(fact.trueValueKey);
                string claim = loc.Get(adventurerCase.GetClaimedValueKey(fact.type));
                bool lie = adventurerCase.IsLie(fact.type);

                string tag = lie ? "<color=#FF7C7C>✗ 거짓</color>" : "<color=#7CFC7C>✓ 참</color>";
                sb.AppendLine($"[{label}] {tag}");
                sb.AppendLine($"   정답: {truth}  /  증언: {claim}");
            }

            _label.text = sb.ToString();
        }

        private static string FactLabelKey(QuestFactType type)
        {
            switch (type)
            {
                case QuestFactType.TargetName: return "fact_label_targetname";
                case QuestFactType.TargetCount: return "fact_label_targetcount";
                case QuestFactType.Location: return "fact_label_location";
                case QuestFactType.Difficulty: return "fact_label_difficulty";
                default: return type.ToString();
            }
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.CaseStarted -= OnCaseStarted;
        }
    }
}
