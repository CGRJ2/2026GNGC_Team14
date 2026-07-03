using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// 방금 누른 질문과 그 답변만 표시한다(누적하지 않고 매번 대체). 새 학생이 오면 비운다.
    /// </summary>
    public class DialogueView : UIViewBase
    {
        [SerializeField] private TMP_Text _logLabel;

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
            Context.AnswerGiven += OnAnswerGiven;
        }

        private void OnCaseStarted(AdventurerCase adventurerCase)
        {
            Render(string.Empty);
        }

        private void OnAnswerGiven(string question, string answer)
        {
            Render($"<b>Q.</b> {question}\n<b>A.</b> {answer}");
        }

        private void Render(string text)
        {
            if (_logLabel != null)
                _logLabel.text = text;
        }

        private void OnDestroy()
        {
            if (Context == null)
                return;
            Context.CaseStarted -= OnCaseStarted;
            Context.AnswerGiven -= OnAnswerGiven;
        }
    }
}
