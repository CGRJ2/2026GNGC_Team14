using System.Text;
using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using TMPro;
using UnityEngine;

namespace MageAcademy.UI
{
    /// <summary>
    /// 현재 학생증 케이스의 정답 정보를 표시하는 디버그 패널.
    /// </summary>
    public class DebugCasePanelView : UIViewBase
    {
        private static readonly StudentIdFieldType[] Fields =
        {
            StudentIdFieldType.Name,
            StudentIdFieldType.Grade,
            StudentIdFieldType.Major,
            StudentIdFieldType.FacePhoto
        };

        [SerializeField] private TMP_Text _label;

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            if (_label == null)
                return;

            if (studentCase == null || studentCase.Student == null)
            {
                _label.text = string.Empty;
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("<b>디버그 정답</b>");
            sb.AppendLine($"본인: {studentCase.Student.studentName}");
            sb.AppendLine(studentCase.IsHonest
                ? "<color=#7CFC7C>정답: 진짜 학생</color>"
                : "<color=#FF7C7C>정답: 위조 학생증</color>");
            sb.AppendLine("----------------");

            foreach (var field in Fields)
            {
                bool forged = studentCase.IsForged(field);
                string tag = forged ? "<color=#FF7C7C>위조</color>" : "<color=#7CFC7C>참</color>";
                sb.AppendLine($"[{FieldLabel(field)}] {tag}");

                if (field == StudentIdFieldType.FacePhoto)
                {
                    string cardName = studentCase.CardPhoto != null ? studentCase.CardPhoto.name : "(없음)";
                    string trueName = studentCase.TruePhoto != null ? studentCase.TruePhoto.name : "(없음)";
                    sb.AppendLine($"   정답: {trueName} / 카드: {cardName}");
                }
                else
                {
                    sb.AppendLine($"   정답: {studentCase.GetTrueText(field)} / 카드: {studentCase.GetCardText(field)}");
                }
            }

            _label.text = sb.ToString();
        }

        private static string FieldLabel(StudentIdFieldType field)
        {
            switch (field)
            {
                case StudentIdFieldType.Name: return "이름";
                case StudentIdFieldType.Grade: return "학년";
                case StudentIdFieldType.Major: return "전공";
                case StudentIdFieldType.FacePhoto: return "얼굴";
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
