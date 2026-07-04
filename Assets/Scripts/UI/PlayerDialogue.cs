using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
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

        private void Update()
        {
            if (!_hideOnClick || !PanelRoot.activeInHierarchy)
                return;

            if (!TryGetPressedPointerPosition(out Vector2 screenPosition))
                return;

            if (!TryGetPanelRect(out RectTransform rectTransform))
                return;

            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPosition, GetEventCamera()))
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

        private bool TryGetPanelRect(out RectTransform rectTransform)
        {
            rectTransform = PanelRoot.transform as RectTransform;
            return rectTransform != null;
        }

        private Camera GetEventCamera()
        {
            Canvas canvas = PanelRoot.GetComponentInParent<Canvas>();
            if (canvas == null)
                return null;

            return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        }

        private static bool TryGetPressedPointerPosition(out Vector2 screenPosition)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                screenPosition = Mouse.current.position.ReadValue();
                return true;
            }

            if (Touchscreen.current != null)
            {
                foreach (TouchControl touch in Touchscreen.current.touches)
                {
                    if (!touch.press.wasPressedThisFrame)
                        continue;

                    screenPosition = touch.position.ReadValue();
                    return true;
                }
            }

            screenPosition = default;
            return false;
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
