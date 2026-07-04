using System.Collections.Generic;
using MageAcademy.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    public class ButtonClickSfxBinder : MonoBehaviour
    {
        private static readonly HashSet<Button> RegisteredButtons = new();

        [SerializeField] private AudioClip _clickClip;
        [SerializeField] private bool _includeInactive = true;

        private readonly List<Button> _boundButtons = new();

        private void Awake()
        {
            BindButtons();
        }

        private void BindButtons()
        {
            GetComponentsInChildren(_includeInactive, _boundButtons);
            for (int i = _boundButtons.Count - 1; i >= 0; i--)
            {
                Button button = _boundButtons[i];
                if (button == null || !RegisteredButtons.Add(button))
                {
                    _boundButtons.RemoveAt(i);
                    continue;
                }

                button.onClick.AddListener(PlayClickSfx);
            }
        }

        private void PlayClickSfx()
        {
            if (_clickClip != null && AudioManager.Instance != null)
                AudioManager.Instance.PlaySfx(_clickClip);
        }

        private void OnDestroy()
        {
            foreach (Button button in _boundButtons)
            {
                if (button != null)
                {
                    button.onClick.RemoveListener(PlayClickSfx);
                    RegisteredButtons.Remove(button);
                }
            }

            _boundButtons.Clear();
        }
    }
}
