using GuildGame.Gameplay.Models;
using GuildGame.Data;
using DG.Tweening;
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

        private Tween _studentGreetingTween;

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

            _studentGreetingTween?.Kill();
            float delay = StudentEntranceDelay;
            _studentGreetingTween = DOVirtual.DelayedCall(delay, () =>
            {
                Show();
                Render(Context.Localization.Get("ui_student_hi"));
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

        private void OnStudentExitRequested()
        {
            _studentGreetingTween?.Kill();
            _studentGreetingTween = null;
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
            Context.StudentExitRequested -= OnStudentExitRequested;
        }
    }
}
