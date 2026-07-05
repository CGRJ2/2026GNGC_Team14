using MageAcademy.Localization;
using TMPro;
using UnityEngine;

namespace MageAcademy.UI
{
    /// <summary>평판 스탯을 구독해 표시한다. 값 변경·언어 변경 시 자동 갱신.</summary>
    public class ReputationView : UIViewBase
    {
        [SerializeField] private TMP_Text _label;

        private string _prefix;

        protected override void OnBind()
        {
            _prefix = Context.Localization.Get("ui_reputation");
            Context.Reputation.Value.Subscribe(OnReputationChanged);
            Context.Localization.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnReputationChanged(int value)
        {
            if (_label != null)
                _label.text = $"{_prefix}: {value}";
        }

        private void OnLanguageChanged(Language language)
        {
            _prefix = Context.Localization.Get("ui_reputation");
            OnReputationChanged(Context.Reputation.Value.Value);
        }

        private void OnDestroy()
        {
            if (Context != null)
            {
                Context.Reputation.Value.Unsubscribe(OnReputationChanged);
                Context.Localization.OnLanguageChanged -= OnLanguageChanged;
            }
        }
    }
}
