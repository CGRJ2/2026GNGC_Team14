using DG.Tweening;
using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    /// <summary>
    /// 학생증 패널. 학생증에 '표기된' 값(위조 반영)과 얼굴 사진을 보여준다.
    /// 플레이어는 이 값들을 질문 증언·실제 얼굴과 대조한다.
    /// </summary>
    public class StudentIdPanelView : UIViewBase
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _gradeLabel;
        [SerializeField] private TMP_Text _majorLabel;
        [SerializeField] private Image _photoImage;

        private Button _nameButton;
        private Button _gradeButton;
        private Button _majorButton;
        private Button _photoButton;
        private UIFadeSlideAnimator _panelAnimator;
        private UIDraggablePanel _draggablePanel;
        private Sequence _exitSequence;

        protected override void OnBind()
        {
            BindQuestionClickTargets();
            EnsureDraggablePanel();
            Context.CaseStarted += OnCaseStarted;
            Context.StudentExitRequested += OnStudentExitRequested;
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            _exitSequence?.Kill();
            PanelAnimator.SnapToRest();

            if (studentCase == null)
                return;

            SetField(_nameLabel, "field_label_name", studentCase.GetCardText(StudentIdFieldType.Name));
            SetField(_gradeLabel, "field_label_grade", studentCase.GetCardText(StudentIdFieldType.Grade));
            SetField(_majorLabel, "field_label_major", studentCase.GetCardText(StudentIdFieldType.Major));

            if (_photoImage != null)
            {
                _photoImage.sprite = studentCase.CardPhoto;
                _photoImage.enabled = studentCase.CardPhoto != null;
            }
        }

        private void OnStudentExitRequested()
        {
            if (!gameObject.activeSelf)
                return;

            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            if (settings == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _exitSequence?.Kill();
            PanelAnimator.CaptureRestPosition();
            _exitSequence = DOTween.Sequence()
                .AppendInterval(settings.studentIdButtonExit.delay)
                .Append(PanelAnimator.CreateDisappearTween(settings.studentIdButtonExit))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    _exitSequence = null;
                })
                .SetLink(gameObject);
        }

        private void SetField(TMP_Text label, string labelKey, string value)
        {
            if (label != null)
                //label.text = $"{Context.Localization.Get(labelKey)}: {value}";
                label.text = $"{value}";
        }

        private void BindQuestionClickTargets()
        {
            _nameButton = BindTextQuestion(_nameLabel, StudentIdFieldType.Name);
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
            if (_gradeButton != null)
                _gradeButton.onClick.RemoveListener(OnGradeClicked);
            if (_majorButton != null)
                _majorButton.onClick.RemoveListener(OnMajorClicked);
            if (_photoButton != null)
                _photoButton.onClick.RemoveListener(OnPhotoClicked);
        }

        private void OnNameClicked() => Context.RequestQuestion(StudentIdFieldType.Name);
        private void OnGradeClicked() => Context.RequestQuestion(StudentIdFieldType.Grade);
        private void OnMajorClicked() => Context.RequestQuestion(StudentIdFieldType.Major);
        private void OnPhotoClicked() => Context.RequestQuestion(StudentIdFieldType.FacePhoto);

        public Button GetButton(StudentIdFieldType field)
        {
            switch (field)
            {
                case StudentIdFieldType.Name: return _nameButton;
                case StudentIdFieldType.Grade: return _gradeButton;
                case StudentIdFieldType.Major: return _majorButton;
                case StudentIdFieldType.FacePhoto: return _photoButton;
                default: return null;
            }
        }

        public void SetAllFieldButtonsInteractable(bool interactable)
        {
            SetFieldButtonInteractable(StudentIdFieldType.Name, interactable);
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

        private UIFadeSlideAnimator PanelAnimator
        {
            get
            {
                if (_panelAnimator == null)
                {
                    _panelAnimator = GetComponent<UIFadeSlideAnimator>();
                    if (_panelAnimator == null)
                        _panelAnimator = gameObject.AddComponent<UIFadeSlideAnimator>();
                }

                return _panelAnimator;
            }
        }

        private void EnsureDraggablePanel()
        {
            if (_draggablePanel != null)
                return;

            _draggablePanel = GetComponent<UIDraggablePanel>();
            if (_draggablePanel == null)
                _draggablePanel = gameObject.AddComponent<UIDraggablePanel>();
        }

        private void OnDestroy()
        {
            _exitSequence?.Kill();
            if (Context != null)
            {
                Context.CaseStarted -= OnCaseStarted;
                Context.StudentExitRequested -= OnStudentExitRequested;
            }

            UnbindQuestionClickTargets();
        }
    }
}
