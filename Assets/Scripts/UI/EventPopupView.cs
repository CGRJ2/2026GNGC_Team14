using GuildGame.Gameplay.Models;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// 판정 결과 이벤트 대사를 팝업으로 표시하고, [다음 손님] 버튼으로 사이클을 이어간다.
    /// </summary>
    public class EventPopupView : UIViewBase
    {
        [SerializeField] private GameObject _panel;
        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;

            HidePanel();
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            HidePanel();
        }

        private void HidePanel()
        {
            if (_panel != null)
                _panel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (Context == null)
                return;
            Context.CaseStarted -= OnCaseStarted;
        }
    }
}
