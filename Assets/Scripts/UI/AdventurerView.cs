using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>
    /// 학생의 상투적 대사만 표시한다. 이름은 학생증/질문으로만 확인 가능하므로 여기선 감춘다(???).
    /// </summary>
    public class AdventurerView : UIViewBase
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _claimLabel;

        protected override void OnBind()
        {
            if (_claimLabel != null)
                _claimLabel.text = Context.Localization.Get("ui_student_hi");

            Context.CaseStarted += OnCaseStarted;
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            if (_nameLabel != null)
                _nameLabel.text = Context.Localization.Get("ui_student_unknown");
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.CaseStarted -= OnCaseStarted;
        }
    }
}
