using MageAcademy.Data;
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
        }

        private void OnCompleteClicked()
        {
            Context.RequestVerdict(PlayerVerdict.ApproveComplete);
        }

        private void OnFailClicked()
        {
            Context.RequestVerdict(PlayerVerdict.RejectFail);
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
            if (_completeButton != null)
                _completeButton.onClick.RemoveListener(OnCompleteClicked);
            if (_failButton != null)
                _failButton.onClick.RemoveListener(OnFailClicked);
        }
    }
}
