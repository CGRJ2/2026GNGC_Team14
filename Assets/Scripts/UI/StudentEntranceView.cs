using DG.Tweening;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// 학생 입장 연출 디렉터. 학생 입장 시 일러스트 등장(아래→위)이 끝난 뒤
    /// 학생증 버튼 등장(위→아래)이 이어지도록 DOTween Sequence로 조립한다.
    /// 수치는 <see cref="UIAnimationSettingsSO"/>에서 조절한다.
    /// </summary>
    public class StudentEntranceView : UIViewBase
    {
        [SerializeField] private UIAnimationSettingsSO _animationSettings;
        [SerializeField] private UIFadeSlideAnimator _illustrationAnimator;
        [SerializeField] private UIFadeSlideAnimator _studentIdButtonAnimator;

        private Sequence _sequence;

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            if (_animationSettings == null || _illustrationAnimator == null || _studentIdButtonAnimator == null)
                return;

            _sequence?.Kill();

            // 버튼은 일러스트 연출이 끝날 때까지 숨겨진 상태로 대기해야 하므로 먼저 스냅해 둔다.
            _illustrationAnimator.PrepareHidden(_animationSettings.studentEnter);
            _studentIdButtonAnimator.PrepareHidden(_animationSettings.studentIdButton);

            _sequence = DOTween.Sequence()
                .AppendInterval(_animationSettings.studentEnter.delay)
                .Append(_illustrationAnimator.CreateAppearTween(_animationSettings.studentEnter))
                .AppendInterval(_animationSettings.studentIdButton.delay)
                .Append(_studentIdButtonAnimator.CreateAppearTween(_animationSettings.studentIdButton))
                .SetLink(gameObject);
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
            if (Context != null)
                Context.CaseStarted -= OnCaseStarted;
        }
    }
}
