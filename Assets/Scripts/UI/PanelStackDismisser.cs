using System.Collections.Generic;
using DG.Tweening;
using MageAcademy.Data;
using UnityEngine;

namespace MageAcademy.UI
{
    /// <summary>
    /// 학생 퇴장 시, 열려 있는 패널들을 스택 순(맨 위부터)으로 위로 이동+페이드아웃한다(파도타기).
    /// PanelTableController의 스택을 참조한다. 학생증 패널은 자체 퇴장 연출을 사용하므로 제외한다.
    /// </summary>
    public class PanelStackDismisser : UIViewBase
    {
        [SerializeField] private PanelTableController _table;

        protected override void OnBind()
        {
            if (_table == null)
                _table = GetComponent<PanelTableController>();

            Context.StudentExitRequested += OnStudentExitRequested;
        }

        private void OnStudentExitRequested()
        {
            if (_table == null)
                return;

            UIAnimationSettingsSO s = Context.UIAnimationSettings;
            if (s == null)
                return;

            List<GameObject> panels = _table.GetOpenPanelsTopFirst(includeStudentId: false);
            float stagger = Mathf.Max(0f, s.panelDismissStagger);

            for (int i = 0; i < panels.Count; i++)
            {
                GameObject panel = panels[i];
                if (panel == null || !panel.activeInHierarchy)
                    continue;

                UIFadeSlideAnimator animator = panel.GetComponent<UIFadeSlideAnimator>();
                if (animator == null)
                    animator = panel.AddComponent<UIFadeSlideAnimator>();

                animator.CaptureRestPosition(); // 드래그된 현재 위치를 기준으로 위로 올린다.

                GameObject captured = panel;
                UIFadeSlideAnimator capturedAnimator = animator;
                DOTween.Sequence()
                    .AppendInterval(i * stagger)
                    .Append(animator.CreateDisappearTween(s.panelExit))
                    .AppendCallback(() =>
                    {
                        captured.SetActive(false);
                        capturedAnimator.SnapToRest(); // 다음 열림을 위해 위치·알파 복구.
                    })
                    .SetLink(panel);
            }

            _table.NotifyPanelsDismissed();
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.StudentExitRequested -= OnStudentExitRequested;
        }
    }
}
