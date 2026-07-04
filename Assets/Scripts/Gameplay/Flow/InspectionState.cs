using MageAcademy.Core;
using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using DG.Tweening;
using UnityEngine;

namespace MageAcademy.Gameplay.Flow
{
    /// <summary>
    /// 검증 상태. 필드 질문에 학생이 '진짜값'을 증언하도록 답변을 발행하고, 판정 요청 시 결과로 넘어간다.
    /// 입장 연출이 끝나면 검사 제한시간을 카운트다운하고, 시간 초과 시 타임아웃으로 결과로 넘어간다.
    /// (플레이어는 증언을 학생증 카드값과 대조해 위조를 찾는다.)
    /// </summary>
    public class InspectionState : GameStateBase
    {
        private Tween _answerDelayTween;

        private bool _hasTimer;
        private bool _resolved;
        private float _elapsed;
        private float _startDelay;
        private float _limit;
        private bool _counting;

        public InspectionState(GameContext context, StateMachine machine) : base(context, machine) { }

        public override void Enter()
        {
            Context.QuestionRequested += OnQuestionRequested;
            Context.VerdictRequested += OnVerdictRequested;
            Context.ReportInterrogationRequested += OnReportInterrogationRequested;

            // 타이머 초기화(튜토리얼은 시간 제한 없음).
            Context.PendingTimeout = false;
            _resolved = false;
            _counting = false;
            _elapsed = 0f;
            _startDelay = EntranceReadyDelay();
            _limit = Context.Balance != null ? Context.Balance.inspectionTimeLimit : 0f;
            _hasTimer = !Context.IsTutorial && _limit > 0f;
        }

        public override void Tick()
        {
            if (!_hasTimer || _resolved)
                return;

            _elapsed += Time.deltaTime;
            if (_elapsed < _startDelay)
                return;

            float remaining = _limit - (_elapsed - _startDelay);
            if (remaining <= 0f)
            {
                OnTimeout();
                return;
            }

            _counting = true;
            Context.RaiseInspectionTimer(remaining, _limit);
        }

        public override void Exit()
        {
            Context.QuestionRequested -= OnQuestionRequested;
            Context.VerdictRequested -= OnVerdictRequested;
            Context.ReportInterrogationRequested -= OnReportInterrogationRequested;
            _answerDelayTween?.Kill();
            _answerDelayTween = null;

            if (_counting)
                Context.RaiseInspectionTimerHidden();
            _counting = false;
        }

        private void OnTimeout()
        {
            _resolved = true;
            Context.PendingTimeout = true;
            Context.RaiseStudentEmotion(StudentEmotion.Angry);
            GoNext();
        }

        private float EntranceReadyDelay()
        {
            UIAnimationSettingsSO s = Context.UIAnimationSettings;
            if (s == null)
                return 0.5f;

            return Mathf.Max(
                s.studentEnter.delay + s.studentEnter.duration,
                s.studentIdButton.delay + s.studentIdButton.duration);
        }

        private void OnReportInterrogationRequested(int paragraphIndex)
        {
            ReportData report = Context.CurrentCase != null ? Context.CurrentCase.Report : null;
            if (report == null)
                return;

            bool foreign = report.IsParagraphForeign(paragraphIndex);
            string line = Context.Localization.GetRandom(foreign ? "report_react_flustered" : "report_react_normal");

            Context.RaiseStudentEmotion(foreign ? StudentEmotion.Flustered : StudentEmotion.Normal);
            Context.RaiseStudentReaction(line);
        }

        private void OnQuestionRequested(StudentIdFieldType field)
        {
            if (Context.CurrentCase == null)
                return;

            string questionText = Context.Localization.GetRandom(QuestionKey(field));
            string trueValue = Context.CurrentCase.GetTrueText(field);

            Context.RaiseQuestion(questionText);

            if (field == StudentIdFieldType.FacePhoto)
                return;

            string answerText = Context.Localization.GetFormattedRandom("a_student", trueValue);

            _answerDelayTween?.Kill();
            float delay = Context.UIAnimationSettings != null
                ? Context.UIAnimationSettings.dialogueAnswerDelay
                : 0.6f;

            _answerDelayTween = DOVirtual.DelayedCall(delay, () =>
            {
                Context.RaiseAnswer(questionText, answerText);
                _answerDelayTween = null;
            });
        }

        private void OnVerdictRequested(PlayerVerdict verdict)
        {
            if (_resolved)
                return;

            _resolved = true;
            Context.PendingTimeout = false;
            Context.PendingVerdict = verdict;
            GoNext();
        }

        private string QuestionKey(StudentIdFieldType field)
        {
            switch (field)
            {
                case StudentIdFieldType.Name: return "q_student_name";
                case StudentIdFieldType.EnrollmentDate: return "q_student_enrollment";
                case StudentIdFieldType.Grade: return "q_student_grade";
                case StudentIdFieldType.Major: return "q_student_major";
                case StudentIdFieldType.FacePhoto:
                    return Context.IsTutorial ? "q_student_photo_tuto" : "q_student_photo";
                default: return field.ToString();
            }
        }
    }
}
