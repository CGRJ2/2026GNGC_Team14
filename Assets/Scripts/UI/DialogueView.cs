using GuildGame.Gameplay.Models;
using GuildGame.Data;
using TMPro;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// 방금 누른 질문과 그 답변만 표시한다(누적하지 않고 매번 대체). 새 학생이 오면 비운다.
    /// </summary>
    public class DialogueView : UIViewBase
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private TMP_Text _logLabel;

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
            Context.AnswerGiven += OnAnswerGiven;
            Context.OutcomeResolved += OnOutcomeResolved;
            Context.StudentExitRequested += OnStudentExitRequested;

            Hide();
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            Hide();
        }

        private void OnAnswerGiven(string question, string answer)
        {
            Show();
            Render($"<b>A.</b> {answer}");
        }

        private void OnOutcomeResolved(CaseOutcome outcome, string eventText)
        {
            Show();
            Render(eventText);
        }

        private void OnStudentExitRequested()
        {
            Hide();
        }

        private void Render(string text)
        {
            if (_logLabel != null)
                _logLabel.text = text;
        }

        private void Show()
        {
            PanelRoot.SetActive(true);
        }

        private void Hide()
        {
            Render(string.Empty);
            PanelRoot.SetActive(false);
        }

        private GameObject PanelRoot => _panelRoot != null ? _panelRoot : gameObject;

        private void OnDestroy()
        {
            if (Context == null)
                return;
            Context.CaseStarted -= OnCaseStarted;
            Context.AnswerGiven -= OnAnswerGiven;
            Context.OutcomeResolved -= OnOutcomeResolved;
            Context.StudentExitRequested -= OnStudentExitRequested;
        }
    }
}
