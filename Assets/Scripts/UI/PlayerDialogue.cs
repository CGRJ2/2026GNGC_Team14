using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// Displays the player's current student ID question in the player speech bubble.
    /// StudentIdPanelView owns the question trigger buttons.
    /// </summary>
    public class PlayerDialogue : UIViewBase
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private TMP_Text _speechLabel;

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
            Context.QuestionAsked += OnQuestionAsked;
            Context.StudentExitRequested += OnStudentExitRequested;

            Hide();
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            Hide();
        }

        private void OnQuestionAsked(string question)
        {
            Show();
            if (_speechLabel != null)
                _speechLabel.text = question;
        }

        private void OnStudentExitRequested()
        {
            Hide();
        }

        private void Clear()
        {
            if (_speechLabel != null)
                _speechLabel.text = string.Empty;
        }

        private void Show()
        {
            PanelRoot.SetActive(true);
        }

        private void Hide()
        {
            Clear();
            PanelRoot.SetActive(false);
        }

        private GameObject PanelRoot => _panelRoot != null ? _panelRoot : gameObject;

        private void OnDestroy()
        {
            if (Context == null)
                return;

            Context.CaseStarted -= OnCaseStarted;
            Context.QuestionAsked -= OnQuestionAsked;
            Context.StudentExitRequested -= OnStudentExitRequested;
        }
    }
}
