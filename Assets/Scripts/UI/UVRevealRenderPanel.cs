using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MageAcademy.Audio;

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
        [SerializeField] private AudioClip _wandActivateClip;
        [SerializeField] private string _stageCameraName = "UVRevealCamera";
        [SerializeField] private string _revealMaskName = "UV_CircleMask";

        private RectTransform _rectTransform;
        private Canvas _canvas;

        private void Awake()
        {
            CacheReferences();
            SetMaskVisible(!_hideMaskOutsidePanel);
        }

        private void OnEnable()
        {
            CacheReferences();
            SetMaskVisible(!_hideMaskOutsidePanel);
            PlayWandActivateSfx();
        }

        private void Update()
        {
            UpdateMaskAtScreenPosition(Input.mousePosition);
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
            if (!TryGetWorldPoint(eventData.position, GetEventCamera(eventData), out Vector3 worldPoint))
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
            if (!TryGetWorldPoint(eventData.position, GetEventCamera(eventData), out Vector3 worldPoint))
            {
                if (_hideMaskOutsidePanel)
                    SetMaskVisible(false);
                return;
            }

            SetMaskVisible(true);
            Transform maskTransform = _revealMask.transform;
            maskTransform.position = new Vector3(worldPoint.x, worldPoint.y, maskTransform.position.z);
        }

        private void UpdateMaskAtScreenPosition(Vector2 screenPosition)
        {
            if (!TryGetWorldPoint(screenPosition, GetCanvasCamera(), out Vector3 worldPoint))
            {
                if (_hideMaskOutsidePanel)
                    SetMaskVisible(false);
                return;
            }

            SetMaskVisible(true);
            Transform maskTransform = _revealMask.transform;
            maskTransform.position = new Vector3(worldPoint.x, worldPoint.y, maskTransform.position.z);
        }

        private bool TryGetWorldPoint(Vector2 screenPosition, Camera eventCamera, out Vector3 worldPoint)
        {
            worldPoint = default;
            CacheReferences();

            if (_rectTransform == null || _stageCamera == null || _revealMask == null)
                return false;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rectTransform,
                    screenPosition,
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

        private static Camera GetEventCamera(PointerEventData eventData)
        {
            return eventData.pressEventCamera != null ? eventData.pressEventCamera : eventData.enterEventCamera;
        }

        private Camera GetCanvasCamera()
        {
            if (_canvas == null)
                _canvas = GetComponentInParent<Canvas>();

            return _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _canvas.worldCamera
                : null;
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
            if (_canvas == null)
                _canvas = GetComponentInParent<Canvas>();
            if (_stageCamera == null)
                _stageCamera = FindCameraByName(_stageCameraName);
            if (_revealMask == null)
                _revealMask = FindSpriteMaskByName(_revealMaskName);
            if (_revealMask != null)
                _stagePlaneZ = _revealMask.transform.position.z;
        }

        private static Camera FindCameraByName(string cameraName)
        {
            foreach (Camera camera in FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (camera != null && camera.gameObject.name == cameraName)
                    return camera;

            return null;
        }

        private static SpriteMask FindSpriteMaskByName(string maskName)
        {
            foreach (SpriteMask mask in FindObjectsByType<SpriteMask>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (mask != null && mask.gameObject.name == maskName)
                    return mask;

            return null;
        }

        private void PlayWandActivateSfx()
        {
            if (_wandActivateClip != null && AudioManager.Instance != null)
                AudioManager.Instance.PlaySfx(_wandActivateClip);
        }
    }
}
