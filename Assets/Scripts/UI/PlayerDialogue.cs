using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    /// <summary>
    /// Displays the player's current student ID question in the player speech bubble.
    /// StudentIdPanelView owns the question trigger buttons.
    /// </summary>
    public class PlayerDialogue : UIViewBase, IPointerClickHandler
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private TMP_Text _speechLabel;
        [SerializeField] private bool _hideOnClick = true;

        protected override void OnBind()
        {
            EnableClickRaycast();

            Context.CaseStarted += OnCaseStarted;
            Context.QuestionAsked += OnQuestionAsked;
            Context.CutsceneDialogueRequested += OnCutsceneDialogueRequested;
            Context.CutsceneEnded += OnCutsceneEnded;
            Context.StudentExitRequested += OnStudentExitRequested;

            Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_hideOnClick || !PanelRoot.activeSelf)
                return;

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

        private void OnCutsceneDialogueRequested(CutsceneSpeaker speaker, string text)
        {
            if (speaker != CutsceneSpeaker.Player)
            {
                Hide();
                return;
            }

            Show();
            if (_speechLabel != null)
                _speechLabel.text = text;
        }

        private void OnCutsceneEnded()
        {
            Hide();
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

        private void EnableClickRaycast()
        {
            if (PanelRoot.TryGetComponent(out Graphic graphic))
                graphic.raycastTarget = true;
        }

        private void OnDestroy()
        {
            if (Context == null)
                return;

            Context.CaseStarted -= OnCaseStarted;
            Context.QuestionAsked -= OnQuestionAsked;
            Context.CutsceneDialogueRequested -= OnCutsceneDialogueRequested;
            Context.CutsceneEnded -= OnCutsceneEnded;
            Context.StudentExitRequested -= OnStudentExitRequested;
        }
    }
}
