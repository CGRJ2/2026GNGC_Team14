using GuildGame.Core;
using GuildGame.Data;
using DG.Tweening;

namespace GuildGame.Gameplay.Flow
{
    /// <summary>
    /// 검증 상태. 필드 질문에 학생이 '진짜값'을 증언하도록 답변을 발행하고, 판정 요청 시 결과로 넘어간다.
    /// (플레이어는 이 증언을 학생증 카드값과 대조해 위조를 찾는다.)
    /// </summary>
    public class InspectionState : GameStateBase
    {
        private Tween _answerDelayTween;

        public InspectionState(GameContext context, StateMachine machine) : base(context, machine) { }

        public override void Enter()
        {
            Context.QuestionRequested += OnQuestionRequested;
            Context.VerdictRequested += OnVerdictRequested;
        }

        public override void Exit()
        {
            Context.QuestionRequested -= OnQuestionRequested;
            Context.VerdictRequested -= OnVerdictRequested;
            _answerDelayTween?.Kill();
            _answerDelayTween = null;
        }

        private void OnQuestionRequested(StudentIdFieldType field)
        {
            if (Context.CurrentCase == null)
                return;

            string questionText = Context.Localization.Get(QuestionKey(field));
            string trueValue = Context.CurrentCase.GetTrueText(field);
            string answerText = Context.Localization.GetFormatted("a_student", trueValue);

            Context.RaiseQuestion(questionText);

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
            Context.PendingVerdict = verdict;
            GoNext();
        }

        private static string QuestionKey(StudentIdFieldType field)
        {
            switch (field)
            {
                case StudentIdFieldType.Name: return "q_student_name";
                case StudentIdFieldType.EnrollmentDate: return "q_student_enrollment";
                case StudentIdFieldType.Grade: return "q_student_grade";
                case StudentIdFieldType.Major: return "q_student_major";
                case StudentIdFieldType.FacePhoto: return "q_student_photo";
                default: return field.ToString();
            }
        }
    }
}
