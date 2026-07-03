using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// Questions 박스에 현재 학생증 데이터 클릭으로 선택된 질문을 표시한다.
    /// 질문 버튼 생성은 StudentIdPanelView가 담당한다.
    /// </summary>
    public class StudentQuestionButtonsView : UIViewBase
    {
        [SerializeField] private TMP_Text _questionLabel;

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
            Context.QuestionAsked += OnQuestionAsked;
            Clear();
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            Clear();
        }

        private void OnQuestionAsked(string question)
        {
            if (_questionLabel != null)
                _questionLabel.text = question;
        }

        private void Clear()
        {
            if (_questionLabel != null)
                _questionLabel.text = string.Empty;
        }

        private void OnDestroy()
        {
            if (Context == null)
                return;

            Context.CaseStarted -= OnCaseStarted;
            Context.QuestionAsked -= OnQuestionAsked;
        }
    }
}
