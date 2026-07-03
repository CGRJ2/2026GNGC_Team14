using System.Collections.Generic;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildGame.UI
{
    /// <summary>
    /// 텍스트 4필드(이름·입학일·학년·전공) 질문 버튼을 생성한다. 클릭 시 학생이 진짜값을 증언한다.
    /// (얼굴 사진은 질문이 아니라 시각 대조로 판별하므로 버튼 없음.)
    /// </summary>
    public class StudentQuestionButtonsView : UIViewBase
    {
        private static readonly StudentIdFieldType[] TextFields =
        {
            StudentIdFieldType.Name,
            StudentIdFieldType.EnrollmentDate,
            StudentIdFieldType.Grade,
            StudentIdFieldType.Major
        };

        [SerializeField] private Transform _container;
        [SerializeField] private Button _buttonPrefab;

        private readonly List<Button> _spawned = new();

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            ClearButtons();
            if (_container == null || _buttonPrefab == null)
                return;

            foreach (var field in TextFields)
                CreateButton(field);
        }

        private void CreateButton(StudentIdFieldType field)
        {
            Button button = Instantiate(_buttonPrefab, _container);
            button.gameObject.SetActive(true);

            var label = button.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = Context.Localization.Get(QuestionKey(field));

            StudentIdFieldType captured = field;
            button.onClick.AddListener(() => Context.RequestQuestion(captured));

            _spawned.Add(button);
        }

        private void ClearButtons()
        {
            foreach (var button in _spawned)
            {
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    Destroy(button.gameObject);
                }
            }
            _spawned.Clear();
        }

        private static string QuestionKey(StudentIdFieldType field)
        {
            switch (field)
            {
                case StudentIdFieldType.Name: return "q_student_name";
                case StudentIdFieldType.EnrollmentDate: return "q_student_enrollment";
                case StudentIdFieldType.Grade: return "q_student_grade";
                case StudentIdFieldType.Major: return "q_student_major";
                default: return field.ToString();
            }
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.CaseStarted -= OnCaseStarted;
        }
    }
}
