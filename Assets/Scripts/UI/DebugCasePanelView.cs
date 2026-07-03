using System.Text;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// 디버그용. 현재 학생증 사건의 "정답표"를 표시한다:
    /// 각 필드의 정답(진짜값) vs 카드 표기값, 위조 여부, 전체 진짜/위조.
    /// </summary>
    public class DebugCasePanelView : UIViewBase
    {
        private static readonly StudentIdFieldType[] Fields =
        {
            StudentIdFieldType.Name,
            StudentIdFieldType.EnrollmentDate,
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
            sb.AppendLine("<b>🔍 디버그 · 정답표</b>");
            sb.AppendLine($"본인: {studentCase.Student.studentName}");
            sb.AppendLine(studentCase.IsHonest
                ? "<color=#7CFC7C>정답: 진짜 → 승인해야 함</color>"
                : "<color=#FF7C7C>정답: 위조 → 거부해야 함</color>");
            sb.AppendLine("──────────────");

            foreach (var field in Fields)
            {
                bool forged = studentCase.IsForged(field);
                string tag = forged ? "<color=#FF7C7C>✗ 위조</color>" : "<color=#7CFC7C>✓ 참</color>";
                sb.AppendLine($"[{FieldLabel(field)}] {tag}");

                if (field == StudentIdFieldType.FacePhoto)
                {
                    string cardName = studentCase.CardPhoto != null ? studentCase.CardPhoto.name : "(없음)";
                    string trueName = studentCase.TruePhoto != null ? studentCase.TruePhoto.name : "(없음)";
                    sb.AppendLine($"   정답: {trueName}  /  카드: {cardName}");
                }
                else
                {
                    sb.AppendLine($"   정답: {studentCase.GetTrueText(field)}  /  카드: {studentCase.GetCardText(field)}");
                }
            }

            _label.text = sb.ToString();
        }

        private static string FieldLabel(StudentIdFieldType field)
        {
            switch (field)
            {
                case StudentIdFieldType.Name: return "이름";
                case StudentIdFieldType.EnrollmentDate: return "입학일";
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
