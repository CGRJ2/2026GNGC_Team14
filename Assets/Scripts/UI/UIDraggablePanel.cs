using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIDraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private bool _clampToRootCanvas = true;

        private RectTransform _rect;
        private RectTransform _parentRect;
        private RectTransform _clampRect;
        private UIFadeSlideAnimator _fadeSlideAnimator;
        private Graphic _raycastGraphic;
        private Vector2 _pointerOffset;
        private readonly Vector3[] _clampWorldCorners = new Vector3[4];
        private readonly Vector3[] _selfWorldCorners = new Vector3[4];

        private void Awake()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            _rect = (RectTransform)transform;
            _parentRect = _rect.parent as RectTransform;
            _clampRect = ResolveClampRect();
            _fadeSlideAnimator = GetComponent<UIFadeSlideAnimator>();
            _raycastGraphic = GetComponent<Graphic>();
            if (_raycastGraphic != null)
                _raycastGraphic.raycastTarget = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            EnsureInitialized();
            if (!TryGetPointerLocalPosition(eventData, out Vector2 localPointerPosition))
                return;

            _pointerOffset = _rect.anchoredPosition - localPointerPosition;
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            EnsureInitialized();
            if (!TryGetPointerLocalPosition(eventData, out Vector2 localPointerPosition))
                return;

            Vector2 targetPosition = localPointerPosition + _pointerOffset;
            _rect.anchoredPosition = _clampToRootCanvas
                ? ClampToRootCanvas(targetPosition)
                : targetPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EnsureInitialized();
            _fadeSlideAnimator ??= GetComponent<UIFadeSlideAnimator>();
            _fadeSlideAnimator?.CaptureRestPosition();
        }

        private bool TryGetPointerLocalPosition(PointerEventData eventData, out Vector2 localPointerPosition)
        {
            localPointerPosition = default;
            if (_parentRect == null)
                return false;

            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPointerPosition);
        }

        private Vector2 ClampToRootCanvas(Vector2 targetPosition)
        {
            if (_parentRect == null || _clampRect == null)
                return targetPosition;

            _clampRect.GetWorldCorners(_clampWorldCorners);
            _rect.GetWorldCorners(_selfWorldCorners);

            Vector2 clampMin = ToParentLocal(_clampWorldCorners[0]);
            Vector2 clampMax = ToParentLocal(_clampWorldCorners[2]);
            Vector2 selfMin = ToParentLocal(_selfWorldCorners[0]);
            Vector2 selfMax = ToParentLocal(_selfWorldCorners[2]);

            Vector2 currentPosition = _rect.anchoredPosition;
            float leftExtent = currentPosition.x - selfMin.x;
            float rightExtent = selfMax.x - currentPosition.x;
            float bottomExtent = currentPosition.y - selfMin.y;
            float topExtent = selfMax.y - currentPosition.y;

            float minX = clampMin.x + leftExtent;
            float maxX = clampMax.x - rightExtent;
            float minY = clampMin.y + bottomExtent;
            float maxY = clampMax.y - topExtent;

            targetPosition.x = minX > maxX ? 0f : Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = minY > maxY ? 0f : Mathf.Clamp(targetPosition.y, minY, maxY);
            return targetPosition;
        }

        private RectTransform ResolveClampRect()
        {
            Canvas canvas = GetComponentInParent<Canvas>(true);
            if (canvas != null && canvas.rootCanvas != null)
                return canvas.rootCanvas.transform as RectTransform;

            return _parentRect;
        }

        private Vector2 ToParentLocal(Vector3 worldPosition)
        {
            return _parentRect.InverseTransformPoint(worldPosition);
        }
    }
}
