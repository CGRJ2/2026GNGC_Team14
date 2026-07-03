using GuildGame.Gameplay.Models;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// 판정 결과 이벤트 팝업 루트를 학생 플로우 시작 시 숨김 상태로 정리한다.
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
