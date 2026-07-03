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

        private Button _nameButton;
        private Button _enrollmentButton;
        private Button _gradeButton;
        private Button _majorButton;
        private Button _photoButton;

        protected override void OnBind()
        {
            if (_headerLabel != null)
                _headerLabel.text = Context.Localization.Get("ui_student_card_title");

            BindQuestionClickTargets();
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

        private void BindQuestionClickTargets()
        {
            _nameButton = BindTextQuestion(_nameLabel, StudentIdFieldType.Name);
            _enrollmentButton = BindTextQuestion(_enrollmentLabel, StudentIdFieldType.EnrollmentDate);
            _gradeButton = BindTextQuestion(_gradeLabel, StudentIdFieldType.Grade);
            _majorButton = BindTextQuestion(_majorLabel, StudentIdFieldType.Major);
            _photoButton = BindImageQuestion(_photoImage, StudentIdFieldType.FacePhoto);
        }

        private Button BindTextQuestion(TMP_Text label, StudentIdFieldType field)
        {
            if (label == null)
                return null;

            label.raycastTarget = true;
            Button button = label.GetComponent<Button>();
            if (button == null)
                button = label.gameObject.AddComponent<Button>();

            button.targetGraphic = label;
            AddQuestionListener(button, field);
            return button;
        }

        private Button BindImageQuestion(Image image, StudentIdFieldType field)
        {
            if (image == null)
                return null;

            image.raycastTarget = true;
            Button button = image.GetComponent<Button>();
            if (button == null)
                button = image.gameObject.AddComponent<Button>();

            button.targetGraphic = image;
            AddQuestionListener(button, field);
            return button;
        }

        private void AddQuestionListener(Button button, StudentIdFieldType field)
        {
            switch (field)
            {
                case StudentIdFieldType.Name:
                    button.onClick.AddListener(OnNameClicked);
                    break;
                case StudentIdFieldType.EnrollmentDate:
                    button.onClick.AddListener(OnEnrollmentClicked);
                    break;
                case StudentIdFieldType.Grade:
                    button.onClick.AddListener(OnGradeClicked);
                    break;
                case StudentIdFieldType.Major:
                    button.onClick.AddListener(OnMajorClicked);
                    break;
                case StudentIdFieldType.FacePhoto:
                    button.onClick.AddListener(OnPhotoClicked);
                    break;
            }
        }

        private void UnbindQuestionClickTargets()
        {
            if (_nameButton != null)
                _nameButton.onClick.RemoveListener(OnNameClicked);
            if (_enrollmentButton != null)
                _enrollmentButton.onClick.RemoveListener(OnEnrollmentClicked);
            if (_gradeButton != null)
                _gradeButton.onClick.RemoveListener(OnGradeClicked);
            if (_majorButton != null)
                _majorButton.onClick.RemoveListener(OnMajorClicked);
            if (_photoButton != null)
                _photoButton.onClick.RemoveListener(OnPhotoClicked);
        }

        private void OnNameClicked() => Context.RequestQuestion(StudentIdFieldType.Name);
        private void OnEnrollmentClicked() => Context.RequestQuestion(StudentIdFieldType.EnrollmentDate);
        private void OnGradeClicked() => Context.RequestQuestion(StudentIdFieldType.Grade);
        private void OnMajorClicked() => Context.RequestQuestion(StudentIdFieldType.Major);
        private void OnPhotoClicked() => Context.RequestQuestion(StudentIdFieldType.FacePhoto);

        public Button GetButton(StudentIdFieldType field)
        {
            switch (field)
            {
                case StudentIdFieldType.Name: return _nameButton;
                case StudentIdFieldType.EnrollmentDate: return _enrollmentButton;
                case StudentIdFieldType.Grade: return _gradeButton;
                case StudentIdFieldType.Major: return _majorButton;
                case StudentIdFieldType.FacePhoto: return _photoButton;
                default: return null;
            }
        }

        public void SetAllFieldButtonsInteractable(bool interactable)
        {
            SetFieldButtonInteractable(StudentIdFieldType.Name, interactable);
            SetFieldButtonInteractable(StudentIdFieldType.EnrollmentDate, interactable);
            SetFieldButtonInteractable(StudentIdFieldType.Grade, interactable);
            SetFieldButtonInteractable(StudentIdFieldType.Major, interactable);
            SetFieldButtonInteractable(StudentIdFieldType.FacePhoto, interactable);
        }

        public void SetOnlyFieldButtonInteractable(StudentIdFieldType field)
        {
            SetAllFieldButtonsInteractable(false);
            SetFieldButtonInteractable(field, true);
        }

        private void SetFieldButtonInteractable(StudentIdFieldType field, bool interactable)
        {
            Button button = GetButton(field);
            if (button != null)
                button.interactable = interactable;
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.CaseStarted -= OnCaseStarted;
            UnbindQuestionClickTargets();
        }
    }
}
