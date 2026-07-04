using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using UnityEngine;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    /// <summary>
    /// 테이블/HUD에 현재 학생의 단일 일러스트를 표시한다(입장 시 갱신).
    /// 등장 연출은 <see cref="StudentEntranceView"/>가 담당한다.
    /// </summary>
    public class StudentIllustrationView : UIViewBase
    {
        [SerializeField] private Image _illustrationImage;

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
            Context.CutsceneStudentEnterRequested += OnCutsceneStudentEnterRequested;

            SetSprite(null);
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            if (_illustrationImage == null)
                return;

            Sprite sprite = studentCase != null && studentCase.Student != null
                ? studentCase.Student.illustration
                : null;

            _illustrationImage.sprite = sprite;
            _illustrationImage.enabled = sprite != null;
        }

        private void OnCutsceneStudentEnterRequested(StudentSO student)
        {
            SetSprite(student != null ? student.illustration : null);
        }

        private void SetSprite(Sprite sprite)
        {
            if (_illustrationImage == null)
                return;

            _illustrationImage.sprite = sprite;
            _illustrationImage.enabled = sprite != null;
        }

        private void OnDestroy()
        {
            if (Context != null)
            {
                Context.CaseStarted -= OnCaseStarted;
                Context.CutsceneStudentEnterRequested -= OnCutsceneStudentEnterRequested;
            }
        }
    }
}
