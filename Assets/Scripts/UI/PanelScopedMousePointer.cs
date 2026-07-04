using UnityEngine;
using UnityEngine.InputSystem;

namespace MageAcademy.UI
{
    public class PanelScopedMousePointer : MonoBehaviour
    {
        [SerializeField] private GameObject _targetPanel;
        [SerializeField] private RectTransform _pointer;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Vector2 _pointerOffset;

        private RectTransform _canvasRect;
        private bool _wasActive;
        private bool _previousCursorVisible = true;

        private void Awake()
        {
            CacheReferences();
            ApplyActiveState(false);
        }

        private void OnEnable()
        {
            CacheReferences();
            _previousCursorVisible = Cursor.visible;
            UpdateState(force: true);
        }

        private void Update()
        {
            UpdateState(force: false);
            if (_wasActive)
                MovePointer();
        }

        private void OnDisable()
        {
            ApplyActiveState(false);
            Cursor.visible = _previousCursorVisible;
        }

        private void UpdateState(bool force)
        {
            bool active = _targetPanel != null && _targetPanel.activeInHierarchy;
            if (!force && active == _wasActive)
                return;

            ApplyActiveState(active);
        }

        private void ApplyActiveState(bool active)
        {
            if (_pointer != null)
                _pointer.gameObject.SetActive(active);

            Cursor.visible = active ? false : _previousCursorVisible;
            _wasActive = active;

            if (active)
            {
                if (_pointer != null)
                    _pointer.SetAsLastSibling();
                MovePointer();
            }
        }

        private void MovePointer()
        {
            CacheReferences();
            if (_pointer == null || _canvasRect == null)
                return;

            if (Mouse.current == null)
                return;

            Camera eventCamera = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _canvas.worldCamera
                : null;

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRect,
                    screenPosition,
                    eventCamera,
                    out Vector2 localPoint))
            {
                return;
            }

            _pointer.anchoredPosition = localPoint + _pointerOffset;
        }

        private void CacheReferences()
        {
            if (_canvas == null)
                _canvas = GetComponentInParent<Canvas>();
            if (_canvasRect == null && _canvas != null)
                _canvasRect = _canvas.GetComponent<RectTransform>();
        }
    }
}
