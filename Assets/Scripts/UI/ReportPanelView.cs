using DG.Tweening;
using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    /// <summary>
    /// 레포트 패널. 상단에 주제(제목)를 표시하고, 본문 여러 문단 중 주제와 어긋나는(다른 주제/오류)
    /// 문단을 클릭하면 학생을 추궁한다. 레포트를 요구하지 않는 날(Report==null)에는 열기 버튼을 숨긴다.
    /// 뷰는 이벤트 구독만 하고 모델을 변경하지 않는다.
    /// </summary>
    public class ReportPanelView : UIViewBase
    {
        [Tooltip("레포트 주제(제목) 라벨")]
        [SerializeField] private TMP_Text _topicLabel;

        [Tooltip("본문 문단 라벨들(순서대로). 클릭 시 해당 문단을 추궁한다.")]
        [SerializeField] private TMP_Text[] _bodyParagraphs;

        [Tooltip("테이블의 레포트 열기 버튼. 레포트 없는 날엔 숨긴다.")]
        [SerializeField] private Button _openButton;

        private UIDraggablePanel _draggablePanel;
        private UIFadeSlideAnimator _openButtonAnimator;
        private Tween _openButtonTween;

        protected override void OnBind()
        {
            BindParagraphs();
            EnsureDraggablePanel();
            _openButtonAnimator = EnsureOpenButtonAnimator();

            Context.DayRequirementsPrepared += OnDayRequirementsPrepared;
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

            if (_openButton != null && !_openButton.gameObject.activeSelf)
                ShowOpenButtonAnimated();

            if (_topicLabel != null)
                _topicLabel.text = Context.Localization.Get(report.TopicKey);

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
            // 패널 자체는 PanelTableController의 스택 파도타기 퇴장이 처리한다.
            // 여기선 테이블의 레포트 열기 버튼만 아래로 내리며 페이드아웃한다.
            HideOpenButtonAnimated();
        }

        private void OnDayRequirementsPrepared(DayConfigSO config)
        {
            gameObject.SetActive(false);

            if (config != null && config.requiresReport)
                ShowOpenButtonImmediate();
            else
                SetOpenButtonVisible(false);
        }

        private void SetOpenButtonVisible(bool visible)
        {
            if (_openButton != null)
                _openButton.gameObject.SetActive(visible);
        }

        private void ShowOpenButtonImmediate()
        {
            if (_openButton == null)
                return;

            _openButtonTween?.Kill();
            _openButton.gameObject.SetActive(true);

            if (_openButtonAnimator != null)
                _openButtonAnimator.SnapToRest();
            else
            {
                CanvasGroup canvasGroup = _openButton.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                    canvasGroup.alpha = 1f;
            }
        }

        private UIFadeSlideAnimator EnsureOpenButtonAnimator()
        {
            if (_openButton == null)
                return null;

            UIFadeSlideAnimator animator = _openButton.GetComponent<UIFadeSlideAnimator>();
            if (animator == null)
                animator = _openButton.gameObject.AddComponent<UIFadeSlideAnimator>();
            return animator;
        }

        /// <summary>레포트 열기 버튼을 아래에서 위로 슬라이드+페이드인(학생 등장에 맞춰).</summary>
        private void ShowOpenButtonAnimated()
        {
            if (_openButton == null)
                return;

            _openButton.gameObject.SetActive(true);

            UIAnimationSettingsSO s = Context.UIAnimationSettings;
            if (_openButtonAnimator == null || s == null)
                return;

            _openButtonTween?.Kill();
            _openButtonAnimator.PrepareHidden(s.reportButtonEnter);

            float delay = s.studentDoorOpenLeadDelay
                + s.studentEnter.delay + s.studentEnter.duration
                + s.studentIdButton.delay;

            _openButtonTween = DOTween.Sequence()
                .AppendInterval(delay)
                .Append(_openButtonAnimator.CreateAppearTween(s.reportButtonEnter))
                .SetLink(_openButton.gameObject);
        }

        /// <summary>레포트 열기 버튼을 위에서 아래로 슬라이드+페이드아웃 후 숨긴다.</summary>
        private void HideOpenButtonAnimated()
        {
            if (_openButton == null || !_openButton.gameObject.activeSelf)
                return;

            UIAnimationSettingsSO s = Context.UIAnimationSettings;
            if (_openButtonAnimator == null || s == null)
            {
                _openButton.gameObject.SetActive(false);
                return;
            }

            _openButtonTween?.Kill();
            _openButtonTween = DOTween.Sequence()
                .Append(_openButtonAnimator.CreateDisappearTween(s.reportButtonExit))
                .AppendCallback(() =>
                {
                    _openButtonAnimator.SnapToRest();
                    _openButton.gameObject.SetActive(false);
                })
                .SetLink(_openButton.gameObject);
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
            _openButtonTween?.Kill();

            if (Context != null)
            {
                Context.DayRequirementsPrepared -= OnDayRequirementsPrepared;
                Context.CaseStarted -= OnCaseStarted;
                Context.StudentExitRequested -= OnStudentExitRequested;
            }

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
