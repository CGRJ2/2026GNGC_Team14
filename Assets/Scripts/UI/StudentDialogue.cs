using DG.Tweening;
using MageAcademy.Audio;
using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using TMPro;
using UnityEngine;

namespace MageAcademy.UI
{
    /// <summary>
    /// Displays the current student's greeting, answers, and outcome lines.
    /// </summary>
    public class StudentDialogue : UIViewBase
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private TMP_Text _speechLabel;
        [SerializeField] private AudioClip[] _golemDialogueClips;

        private Tween _studentGreetingTween;
        private int _golemDialogueClipIndex;
        private StudentSO _activeStudent;

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
            Context.AnswerGiven += OnAnswerGiven;
            Context.StudentReactionRequested += OnStudentReaction;
            Context.OutcomeResolved += OnOutcomeResolved;
            Context.CutsceneDialogueRequested += OnCutsceneDialogueRequested;
            Context.CutsceneStudentEnterRequested += OnCutsceneStudentEnterRequested;
            Context.CutsceneStudentExitRequested += OnCutsceneStudentExitRequested;
            Context.CutsceneEnded += OnCutsceneEnded;
            Context.StudentExitRequested += OnStudentExitRequested;

            Hide();
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            Hide();
            _golemDialogueClipIndex = 0;
            _activeStudent = studentCase != null ? studentCase.Student : null;

            _studentGreetingTween?.Kill();
            float delay = StudentEntranceDelay;
            _studentGreetingTween = DOVirtual.DelayedCall(delay, () =>
            {
                Show();
                PlayDialogueVoice();
                Render(Context.Localization.GetRandom("ui_student_hi"));
                Context.RaiseStudentGreetingShown();
                _studentGreetingTween = null;
            });
        }

        private void OnAnswerGiven(string question, string answer)
        {
            _studentGreetingTween?.Kill();
            _studentGreetingTween = null;
            Show();
            PlayDialogueVoice();
            Render($"<b>A.</b> {answer}");
        }

        private void OnStudentReaction(string line)
        {
            _studentGreetingTween?.Kill();
            _studentGreetingTween = null;
            Show();
            PlayDialogueVoice();
            Render(line);
        }

        private void OnOutcomeResolved(CaseOutcome outcome, string eventText)
        {
            _studentGreetingTween?.Kill();
            _studentGreetingTween = null;
            Show();
            PlayDialogueVoice();
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
            PlayDialogueVoice();
            Render(text);
        }

        private void OnCutsceneEnded()
        {
            _activeStudent = null;
            Hide();
        }

        private void OnCutsceneStudentEnterRequested(StudentSO student)
        {
            _activeStudent = student;
            _golemDialogueClipIndex = 0;
        }

        private void OnCutsceneStudentExitRequested()
        {
            _studentGreetingTween?.Kill();
            _studentGreetingTween = null;
            _activeStudent = null;
            Hide();
        }

        private void OnStudentExitRequested()
        {
            _studentGreetingTween?.Kill();
            _studentGreetingTween = null;
            _activeStudent = null;
            Hide();
        }

        private void Render(string text)
        {
            if (_speechLabel != null)
                _speechLabel.text = text;
        }

        private void PlayDialogueVoice()
        {
            if (Context != null && Context.IsTutorial)
            {
                PlayTutorialGolemDialogue();
                return;
            }

            PlayStudentVoice();
        }

        private void PlayTutorialGolemDialogue()
        {
            if (_golemDialogueClips == null || _golemDialogueClips.Length == 0 || AudioManager.Instance == null || Context == null)
                return;

            AudioClip clip = _golemDialogueClips[_golemDialogueClipIndex % _golemDialogueClips.Length];
            _golemDialogueClipIndex++;

            if (clip != null)
                AudioManager.Instance.PlaySfx(clip);
        }

        private void PlayStudentVoice()
        {
            if (AudioManager.Instance == null)
                return;

            StudentSO student = _activeStudent != null
                ? _activeStudent
                : Context != null && Context.CurrentCase != null
                    ? Context.CurrentCase.Student
                    : null;

            if (student == null || student.dialogueVoiceClips == null || student.dialogueVoiceClips.Count == 0)
                return;

            for (int i = 0; i < student.dialogueVoiceClips.Count; i++)
            {
                AudioClip clip = student.dialogueVoiceClips[Random.Range(0, student.dialogueVoiceClips.Count)];
                if (clip == null)
                    continue;

                AudioManager.Instance.PlaySfx(clip);
                return;
            }
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
                    settings.studentDoorOpenLeadDelay + settings.studentEnter.delay + settings.studentEnter.duration,
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
            Context.StudentReactionRequested -= OnStudentReaction;
            Context.OutcomeResolved -= OnOutcomeResolved;
            Context.CutsceneDialogueRequested -= OnCutsceneDialogueRequested;
            Context.CutsceneStudentEnterRequested -= OnCutsceneStudentEnterRequested;
            Context.CutsceneStudentExitRequested -= OnCutsceneStudentExitRequested;
            Context.CutsceneEnded -= OnCutsceneEnded;
            Context.StudentExitRequested -= OnStudentExitRequested;
        }
    }
}
