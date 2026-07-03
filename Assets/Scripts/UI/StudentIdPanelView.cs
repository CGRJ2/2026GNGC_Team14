using GuildGame.Data;
using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildGame.UI
{
    /// <summary>
    /// 학생증 패널. 학생증에 '표기된' 값(위조 반영)과 얼굴 사진을 보여준다.
    /// 플레이어는 이 값들을 질문 증언·실제 얼굴과 대조한다.
    /// </summary>
    public class StudentIdPanelView : UIViewBase
    {
        [SerializeField] private TMP_Text _headerLabel;
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _enrollmentLabel;
        [SerializeField] private TMP_Text _gradeLabel;
        [SerializeField] private TMP_Text _majorLabel;
        [SerializeField] private Image _photoImage;

        protected override void OnBind()
        {
            if (_headerLabel != null)
                _headerLabel.text = Context.Localization.Get("ui_student_card_title");

            Context.CaseStarted += OnCaseStarted;
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            if (studentCase == null)
                return;

            SetField(_nameLabel, "field_label_name", studentCase.GetCardText(StudentIdFieldType.Name));
            SetField(_enrollmentLabel, "field_label_enrollment", studentCase.GetCardText(StudentIdFieldType.EnrollmentDate));
            SetField(_gradeLabel, "field_label_grade", studentCase.GetCardText(StudentIdFieldType.Grade));
            SetField(_majorLabel, "field_label_major", studentCase.GetCardText(StudentIdFieldType.Major));

            if (_photoImage != null)
            {
                _photoImage.sprite = studentCase.CardPhoto;
                _photoImage.enabled = studentCase.CardPhoto != null;
            }
        }

        private void SetField(TMP_Text label, string labelKey, string value)
        {
            if (label != null)
                label.text = $"{Context.Localization.Get(labelKey)}: {value}";
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.CaseStarted -= OnCaseStarted;
        }
    }
}
