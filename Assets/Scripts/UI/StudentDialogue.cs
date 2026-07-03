using DG.Tweening;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// Displays the current student's greeting, answers, and outcome lines.
    /// </summary>
    public class StudentDialogue : UIViewBase
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private TMP_Text _speechLabel;

        private Tween _studentGreetingTween;

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
            Context.AnswerGiven += OnAnswerGiven;
            Context.OutcomeResolved += OnOutcomeResolved;
            Context.CutsceneDialogueRequested += OnCutsceneDialogueRequested;
            Context.CutsceneStudentExitRequested += OnCutsceneStudentExitRequested;
            Context.CutsceneEnded += OnCutsceneEnded;
            Context.StudentExitRequested += OnStudentExitRequested;

            Hide();
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            Hide();

            _studentGreetingTween?.Kill();
            float delay = StudentEntranceDelay;
            _studentGreetingTween = DOVirtual.DelayedCall(delay, () =>
            {
                Show();
                Render(Context.Localization.GetRandom("ui_student_hi"));
                _studentGreetingTween = null;
            });
        }

        private void OnAnswerGiven(string question, string answer)
        {
            _studentGreetingTween?.Kill();
            _studentGreetingTween = null;
            Show();
            Render($"<b>A.</b> {answer}");
        }

        private void OnOutcomeResolved(CaseOutcome outcome, string eventText)
        {
            _studentGreetingTween?.Kill();
            _studentGreetingTween = null;
            Show();
            Render(eventText);
        }

        private void OnCutsceneDialogueRequested(CutsceneSpeaker speaker, string text)
        {
            _studentGreetingTween?.Kill();
            _studentGreetingTween = null;

            if (speaker != CutsceneSpeaker.Student)
            {
                Hide();
                return;
            }

            Show();
            Render(text);
        }

        private void OnCutsceneEnded()
        {
            Hide();
        }

        private void OnCutsceneStudentExitRequested()
        {
            _studentGreetingTween?.Kill();
            _studentGreetingTween = null;
            Hide();
        }

        private void OnStudentExitRequested()
        {
            _studentGreetingTween?.Kill();
            _studentGreetingTween = null;
            Hide();
        }

        private void Render(string text)
        {
            if (_speechLabel != null)
                _speechLabel.text = text;
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

        private float StudentEntranceDelay
        {
            get
            {
                UIAnimationSettingsSO settings = Context.UIAnimationSettings;
                if (settings == null)
                    return 0f;

                return Mathf.Max(
                    settings.studentEnter.delay + settings.studentEnter.duration,
                    settings.studentIdButton.delay + settings.studentIdButton.duration)
                    + settings.studentGreetingDelay;
            }
        }

        private void OnDestroy()
        {
            _studentGreetingTween?.Kill();
            if (Context == null)
                return;

            Context.CaseStarted -= OnCaseStarted;
            Context.AnswerGiven -= OnAnswerGiven;
            Context.OutcomeResolved -= OnOutcomeResolved;
            Context.CutsceneDialogueRequested -= OnCutsceneDialogueRequested;
            Context.CutsceneStudentExitRequested -= OnCutsceneStudentExitRequested;
            Context.CutsceneEnded -= OnCutsceneEnded;
            Context.StudentExitRequested -= OnStudentExitRequested;
        }
    }
}
