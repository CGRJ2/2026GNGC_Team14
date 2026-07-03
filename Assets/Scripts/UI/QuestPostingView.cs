using System.Text;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// 의뢰서. 제목·개요와 각 검증 항목의 참값(플레이어가 대조할 기준)을 표시한다.
    /// </summary>
    public class QuestPostingView : UIViewBase
    {
        [SerializeField] private TMP_Text _headerLabel;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _summaryLabel;
        [SerializeField] private TMP_Text _factsLabel;

        protected override void OnBind()
        {
            if (_headerLabel != null)
                _headerLabel.text = Context.Localization.Get("ui_posting_title");

            Context.CaseStarted += OnCaseStarted;
        }

        private void OnCaseStarted(AdventurerCase adventurerCase)
        {
            if (adventurerCase == null || adventurerCase.Quest == null)
                return;

            var loc = Context.Localization;
            QuestDataSO quest = adventurerCase.Quest;

            if (_titleLabel != null)
                _titleLabel.text = loc.Get(quest.titleKey);
            if (_summaryLabel != null)
                _summaryLabel.text = loc.Get(quest.summaryKey);

            if (_factsLabel != null)
                _factsLabel.text = BuildFactsText(quest);
        }

        private string BuildFactsText(QuestDataSO quest)
        {
            var loc = Context.Localization;
            var sb = new StringBuilder();
            foreach (var fact in quest.facts)
            {
                string label = loc.Get(FactLabelKey(fact.type));
                string value = loc.Get(fact.trueValueKey);
                sb.AppendLine($"• {label}: {value}");
            }
            return sb.ToString();
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
