using System;
using UnityEngine;
using UnityEngine.Events;

namespace MageAcademy.UI
{
    public class UVRevealClickable : MonoBehaviour
    {
        [SerializeField] private string _clickId;
        [SerializeField] private UnityEvent _clicked;

        /// <summary>코드 구독용 클릭 이벤트(뷰가 구독).</summary>
        public event Action Clicked;

        public string ClickId => _clickId;

        public void Click()
        {
            _clicked?.Invoke();
            Clicked?.Invoke();
            Debug.Log($"[UVReveal] Clicked hidden area: {_clickId}", this);
        }
    }
}
