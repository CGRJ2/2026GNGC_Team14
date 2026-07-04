using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    /// <summary>
    /// 레포트 패널. 헤더(이름·과목)는 학생증과 동일하게 클릭 시 학생에게 질문(진술과 대조),
    /// 본문은 여러 문단으로 나뉘며 수상한(외국어) 문단을 클릭하면 학생을 추궁한다.
    /// 레포트를 요구하지 않는 날(Report==null)에는 열기 버튼을 숨긴다.
    /// 뷰는 이벤트 구독만 하고 모델을 변경하지 않는다.
    /// </summary>
    public class ReportPanelView : UIViewBase
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _majorLabel;

        [Tooltip("본문 문단 라벨들(순서대로). 클릭 시 해당 문단을 추궁한다.")]
        [SerializeField] private TMP_Text[] _bodyParagraphs;

        [Tooltip("테이블의 레포트 열기 버튼. 레포트 없는 날엔 숨긴다.")]
        [SerializeField] private Button _openButton;

        private Button _nameButton;
        private Button _majorButton;
        private UIDraggablePanel _draggablePanel;

        protected override void OnBind()
        {
            _nameButton = BindQuestion(_nameLabel, StudentIdFieldType.Name);
            _majorButton = BindQuestion(_majorLabel, StudentIdFieldType.Major);
            BindParagraphs();
            EnsureDraggablePanel();

            Context.CaseStarted += OnCaseStarted;
            Context.StudentExitRequested += OnStudentExitRequested;

            gameObject.SetActive(false);
            SetOpenButtonVisible(false);
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            ReportData report = studentCase != null ? studentCase.Report : null;

            if (report == null)
            {
                gameObject.SetActive(false);
                SetOpenButtonVisible(false);
                return;
            }

            SetOpenButtonVisible(true);
            SetField(_nameLabel, "report_label_name", report.PrintedName);
            SetField(_majorLabel, "report_label_major", report.PrintedMajor);

            if (_bodyParagraphs != null)
            {
                for (int i = 0; i < _bodyParagraphs.Length; i++)
                {
                    TMP_Text label = _bodyParagraphs[i];
                    if (label == null)
                        continue;

                    bool has = i < report.ParagraphCount;
                    label.gameObject.SetActive(has);
                    if (has)
                        label.text = Context.Localization.Get(report.GetParagraphKey(i));
                }
            }
        }

        private void OnStudentExitRequested()
        {
            gameObject.SetActive(false);
        }

        private void SetOpenButtonVisible(bool visible)
        {
            if (_openButton != null)
                _openButton.gameObject.SetActive(visible);
        }

        private void SetField(TMP_Text label, string labelKey, string value)
        {
            if (label != null)
                label.text = $"{Context.Localization.Get(labelKey)}: {value}";
        }

        private Button BindQuestion(TMP_Text label, StudentIdFieldType field)
        {
            if (label == null)
                return null;

            label.raycastTarget = true;
            Button button = label.GetComponent<Button>();
            if (button == null)
                button = label.gameObject.AddComponent<Button>();

            button.targetGraphic = label;
            button.onClick.AddListener(() => Context.RequestQuestion(field));
            return button;
        }

        private void BindParagraphs()
        {
            if (_bodyParagraphs == null)
                return;

            for (int i = 0; i < _bodyParagraphs.Length; i++)
            {
                TMP_Text label = _bodyParagraphs[i];
                if (label == null)
                    continue;

                label.raycastTarget = true;
                Button button = label.GetComponent<Button>();
                if (button == null)
                    button = label.gameObject.AddComponent<Button>();

                button.targetGraphic = label;
                int index = i; // 클로저 캡처
                button.onClick.AddListener(() => Context.RequestReportInterrogation(index));
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
            if (Context != null)
            {
                Context.CaseStarted -= OnCaseStarted;
                Context.StudentExitRequested -= OnStudentExitRequested;
            }

            if (_nameButton != null)
                _nameButton.onClick.RemoveAllListeners();
            if (_majorButton != null)
                _majorButton.onClick.RemoveAllListeners();

            if (_bodyParagraphs != null)
            {
                foreach (TMP_Text label in _bodyParagraphs)
                {
                    if (label == null)
                        continue;
                    Button button = label.GetComponent<Button>();
                    if (button != null)
                        button.onClick.RemoveAllListeners();
                }
            }
        }
    }
}
