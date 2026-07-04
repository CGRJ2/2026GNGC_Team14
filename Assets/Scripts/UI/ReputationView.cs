using TMPro;
using UnityEngine;

namespace MageAcademy.UI
{
    /// <summary>평판 스탯을 구독해 표시한다. 값 변경 시 자동 갱신.</summary>
    public class ReputationView : UIViewBase
    {
        [SerializeField] private TMP_Text _label;

        private string _prefix;

        protected override void OnBind()
        {
            _prefix = Context.Localization.Get("ui_reputation");
            Context.Reputation.Value.Subscribe(OnReputationChanged);
        }

        private void OnReputationChanged(int value)
        {
            if (_label != null)
                _label.text = $"{_prefix}: {value}";
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.Reputation.Value.Unsubscribe(OnReputationChanged);
        }
    }
}
