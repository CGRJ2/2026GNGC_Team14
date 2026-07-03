using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;

namespace GuildGame.UI
{
    /// <summary>손님의 이름과 상투적 주장 대사를 표시한다.</summary>
    public class AdventurerView : UIViewBase
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _claimLabel;

        protected override void OnBind()
        {
            if (_claimLabel != null)
                _claimLabel.text = Context.Localization.Get("ui_adventurer_claim");

            Context.CaseStarted += OnCaseStarted;
        }

        private void OnCaseStarted(AdventurerCase adventurerCase)
        {
            if (_nameLabel != null && adventurerCase != null)
                _nameLabel.text = Context.Localization.Get(adventurerCase.AdventurerNameKey);
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.CaseStarted -= OnCaseStarted;
        }
    }
}
