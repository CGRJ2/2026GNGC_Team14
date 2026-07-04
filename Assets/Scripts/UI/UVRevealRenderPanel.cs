using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    [RequireComponent(typeof(RawImage))]
    public class UVRevealRenderPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerClickHandler
    {
        [SerializeField] private RawImage _targetImage;
        [SerializeField] private Camera _stageCamera;
        [SerializeField] private SpriteMask _revealMask;
        [SerializeField] private float _stagePlaneZ;
        [SerializeField] private LayerMask _clickableMask = ~0;
        [SerializeField] private bool _hideMaskOutsidePanel = true;

        private RectTransform _rectTransform;

        private void Awake()
        {
            CacheReferences();
            SetMaskVisible(!_hideMaskOutsidePanel);
        }

        private void OnEnable()
        {
            CacheReferences();
            SetMaskVisible(!_hideMaskOutsidePanel);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UpdateMask(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_hideMaskOutsidePanel)
                SetMaskVisible(false);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            UpdateMask(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!TryGetWorldPoint(eventData, out Vector3 worldPoint))
                return;

            Collider2D[] hits = Physics2D.OverlapPointAll(worldPoint, _clickableMask);
            foreach (Collider2D hit in hits)
            {
                UVRevealClickable clickable = hit.GetComponentInParent<UVRevealClickable>();
                if (clickable == null)
                    continue;

                clickable.Click();
                return;
            }
        }

        private void UpdateMask(PointerEventData eventData)
        {
            if (!TryGetWorldPoint(eventData, out Vector3 worldPoint))
            {
                if (_hideMaskOutsidePanel)
                    SetMaskVisible(false);
                return;
            }

            SetMaskVisible(true);
            Transform maskTransform = _revealMask.transform;
            maskTransform.position = new Vector3(worldPoint.x, worldPoint.y, maskTransform.position.z);
        }

        private bool TryGetWorldPoint(PointerEventData eventData, out Vector3 worldPoint)
        {
            worldPoint = default;
            CacheReferences();

            if (_rectTransform == null || _stageCamera == null || _revealMask == null)
                return false;

            Camera eventCamera = eventData.pressEventCamera != null ? eventData.pressEventCamera : eventData.enterEventCamera;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rectTransform,
                    eventData.position,
                    eventCamera,
                    out Vector2 localPoint))
            {
                return false;
            }

            Rect rect = _rectTransform.rect;
            if (!rect.Contains(localPoint))
                return false;

            Vector2 viewportPoint = new Vector2(
                Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x),
                Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y));

            float distance = Mathf.Abs(_stagePlaneZ - _stageCamera.transform.position.z);
            worldPoint = _stageCamera.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, distance));
            worldPoint.z = _stagePlaneZ;
            return true;
        }

        private void SetMaskVisible(bool visible)
        {
            if (_revealMask != null)
                _revealMask.gameObject.SetActive(visible);
        }

        private void CacheReferences()
        {
            if (_targetImage == null)
                _targetImage = GetComponent<RawImage>();
            if (_rectTransform == null)
                _rectTransform = _targetImage != null ? _targetImage.rectTransform : GetComponent<RectTransform>();
        }
    }
}
