using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    /// <summary>
    /// 상시 노출되는 [의뢰 완료]/[의뢰 실패] 판정 버튼. 클릭 시 판정을 요청한다.
    /// </summary>
    public class VerdictButtonsView : UIViewBase
    {
        [SerializeField] private Button _completeButton;
        [SerializeField] private Button _failButton;
        [SerializeField] private TMP_Text _completeLabel;
        [SerializeField] private TMP_Text _failLabel;
        [SerializeField] private JudgeActionView _judgeAction;

        protected override void OnBind()
        {
            if (_completeLabel != null)
                _completeLabel.text = Context.Localization.Get("ui_verdict_complete");
            if (_failLabel != null)
                _failLabel.text = Context.Localization.Get("ui_verdict_fail");

            if (_completeButton != null)
                _completeButton.onClick.AddListener(OnCompleteClicked);
            if (_failButton != null)
                _failButton.onClick.AddListener(OnFailClicked);

            Context.CaseStarted += OnCaseStarted;
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            SetButtonsInteractable(studentCase != null, studentCase != null);
        }

        private void OnCompleteClicked()
        {
            PlayJudgeAction(PlayerVerdict.ApproveComplete);
        }

        private void OnFailClicked()
        {
            PlayJudgeAction(PlayerVerdict.RejectFail);
        }

        private void PlayJudgeAction(PlayerVerdict verdict)
        {
            SetButtonsInteractable(false, false);

            if (_judgeAction == null)
            {
                Context.RequestVerdict(verdict);
                return;
            }

            _judgeAction.Play(verdict, Context.UIAnimationSettings, () => Context.RequestVerdict(verdict));
        }

        public Button FailButton => _failButton;
        public Button CompleteButton => _completeButton;

        public void SetButtonsInteractable(bool completeInteractable, bool failInteractable)
        {
            if (_completeButton != null)
                _completeButton.interactable = completeInteractable;
            if (_failButton != null)
                _failButton.interactable = failInteractable;
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.CaseStarted -= OnCaseStarted;
            if (_completeButton != null)
                _completeButton.onClick.RemoveListener(OnCompleteClicked);
            if (_failButton != null)
                _failButton.onClick.RemoveListener(OnFailClicked);
        }
    }
}
