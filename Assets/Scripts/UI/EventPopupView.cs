using GuildGame.Data;
using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildGame.UI
{
    /// <summary>
    /// 판정 결과 이벤트 대사를 팝업으로 표시하고, [다음 손님] 버튼으로 사이클을 이어간다.
    /// </summary>
    public class EventPopupView : UIViewBase
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _eventLabel;
        [SerializeField] private Button _nextButton;
        [SerializeField] private TMP_Text _nextLabel;

        protected override void OnBind()
        {
            if (_nextLabel != null)
                _nextLabel.text = Context.Localization.Get("ui_next");

            if (_nextButton != null)
                _nextButton.onClick.AddListener(OnNextClicked);

            Context.OutcomeResolved += OnOutcomeResolved;
            Context.CaseStarted += OnCaseStarted;

            HidePanel();
        }

        private void OnOutcomeResolved(CaseOutcome outcome, string eventText)
        {
            if (_eventLabel != null)
                _eventLabel.text = eventText;
            ShowPanel();
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            HidePanel();
        }

        private void OnNextClicked()
        {
            HidePanel();
            Context.RequestNext();
        }

        private void ShowPanel()
        {
            if (_panel != null)
                _panel.SetActive(true);
        }

        private void HidePanel()
        {
            if (_panel != null)
                _panel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_nextButton != null)
                _nextButton.onClick.RemoveListener(OnNextClicked);
            if (Context == null)
                return;
            Context.OutcomeResolved -= OnOutcomeResolved;
            Context.CaseStarted -= OnCaseStarted;
        }
    }
}
