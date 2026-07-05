using System.Collections.Generic;
using MageAcademy.Gameplay.Models;
using UnityEngine;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    public class UVRevealStageView : UIViewBase
    {
        [SerializeField] private SpriteRenderer _topCover;
        [SerializeField] private SpriteRenderer _bottomHidden;
        [SerializeField] private List<Sprite> _uvSprites = new();
        [SerializeField] private Transform _hiddenClickAreaRoot;
        [SerializeField] private bool _assignRandomSpriteOnCaseStarted = true;

        [Tooltip("골렘 흔적(서명)을 표시할 SpriteRenderer(HiddenInfo_ClickArea)")]
        [SerializeField] private SpriteRenderer _hiddenSignatureRenderer;

        [Tooltip("테이블의 UV 열기 버튼. UV 검증 없는 날엔 숨긴다.")]
        [SerializeField] private Button _openButton;
        [SerializeField] private string _topCoverName = "UV_TopCover";
        [SerializeField] private string _bottomHiddenName = "UV_BottomHidden";
        [SerializeField] private string _hiddenClickAreaRootName = "UV_HiddenClickAreas";
        [SerializeField] private string _hiddenSignatureRendererName = "HiddenInfo_ClickArea";

        private readonly List<UVRevealClickable> _subscribedClickables = new();

        protected override void OnBind()
        {
            CacheSceneReferences();
            Context.CaseStarted += OnCaseStarted;
            SubscribeClickables();

            // 레포트/수정구슬 버튼과 동일하게, 케이스가 UV를 활성화하기 전엔 열기 버튼을 숨긴다.
            if (_openButton != null)
                _openButton.gameObject.SetActive(false);
        }

        private void SubscribeClickables()
        {
            if (_hiddenClickAreaRoot == null)
                return;

            foreach (UVRevealClickable clickable in _hiddenClickAreaRoot.GetComponentsInChildren<UVRevealClickable>(true))
            {
                clickable.Clicked += OnHiddenSignatureClicked;
                _subscribedClickables.Add(clickable);
            }
        }

        /// <summary>UV로 골렘 흔적(HiddenInfo_ClickArea)을 클릭하면 추궁을 요청한다.</summary>
        public void OnHiddenSignatureClicked()
        {
            if (Context != null)
                Context.RequestUVInterrogation();
        }

        public void AssignRandomSprite()
        {
            Sprite sprite = GetRandomSprite();
            if (sprite == null)
                return;

            if (_topCover != null)
                _topCover.sprite = sprite;
            if (_bottomHidden != null)
                _bottomHidden.sprite = sprite;
        }

        public void RandomizeHiddenClickAreasInsideTopCover()
        {
            if (_topCover == null || _hiddenClickAreaRoot == null)
                return;

            Bounds coverBounds = _topCover.bounds;
            Collider2D[] clickAreas = _hiddenClickAreaRoot.GetComponentsInChildren<Collider2D>(true);
            foreach (Collider2D clickArea in clickAreas)
                MoveClickAreaInsideBounds(clickArea, coverBounds);
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            CacheSceneReferences();

            if (_assignRandomSpriteOnCaseStarted)
                AssignRandomSprite();

            ConfigureGolem(studentCase != null ? studentCase.UV : null);
        }

        /// <summary>
        /// 골렘 흔적 상태를 설정한다. 골렘 작품이면 흔적 클릭영역을 활성화·랜덤배치하고 서명 스프라이트를 넣는다.
        /// 아니면 흔적 클릭영역을 비활성화한다. UV 검증면이 없는 날엔 열기 버튼을 숨긴다.
        /// </summary>
        private void ConfigureGolem(UVData uv)
        {
            if (_openButton != null)
                _openButton.gameObject.SetActive(uv != null);

            bool golem = uv != null && uv.IsGolemWork;

            if (golem && _hiddenSignatureRenderer != null && uv.SignatureSprite != null)
                _hiddenSignatureRenderer.sprite = uv.SignatureSprite;

            if (_hiddenClickAreaRoot != null)
                _hiddenClickAreaRoot.gameObject.SetActive(golem);

            if (golem)
                RandomizeHiddenClickAreasInsideTopCover();
        }

        private Sprite GetRandomSprite()
        {
            if (_uvSprites == null || _uvSprites.Count == 0)
                return null;

            for (int i = 0; i < _uvSprites.Count; i++)
            {
                Sprite sprite = _uvSprites[Random.Range(0, _uvSprites.Count)];
                if (sprite != null)
                    return sprite;
            }

            return null;
        }

        private void CacheSceneReferences()
        {
            if (_topCover == null)
                _topCover = FindSpriteRendererByName(_topCoverName);
            if (_bottomHidden == null)
                _bottomHidden = FindSpriteRendererByName(_bottomHiddenName);
            if (_hiddenClickAreaRoot == null)
                _hiddenClickAreaRoot = FindTransformByName(_hiddenClickAreaRootName);
            if (_hiddenSignatureRenderer == null)
                _hiddenSignatureRenderer = FindSpriteRendererByName(_hiddenSignatureRendererName);
        }

        private static SpriteRenderer FindSpriteRendererByName(string objectName)
        {
            Transform transform = FindTransformByName(objectName);
            return transform != null ? transform.GetComponent<SpriteRenderer>() : null;
        }

        private static Transform FindTransformByName(string objectName)
        {
            foreach (Transform transform in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (transform != null && transform.gameObject.name == objectName)
                    return transform;

            return null;
        }

        private static void MoveClickAreaInsideBounds(Collider2D clickArea, Bounds coverBounds)
        {
            if (clickArea == null)
                return;

            Vector3 extents = clickArea.bounds.extents;
            if (extents.x <= 0f)
                extents.x = 0.05f;
            if (extents.y <= 0f)
                extents.y = 0.05f;

            float minX = coverBounds.min.x + extents.x;
            float maxX = coverBounds.max.x - extents.x;
            float minY = coverBounds.min.y + extents.y;
            float maxY = coverBounds.max.y - extents.y;

            if (minX > maxX)
                (minX, maxX) = (coverBounds.center.x, coverBounds.center.x);
            if (minY > maxY)
                (minY, maxY) = (coverBounds.center.y, coverBounds.center.y);

            Transform target = clickArea.transform;
            Vector3 position = target.position;
            position.x = Random.Range(minX, maxX);
            position.y = Random.Range(minY, maxY);
            target.position = position;
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.CaseStarted -= OnCaseStarted;

            foreach (UVRevealClickable clickable in _subscribedClickables)
                if (clickable != null)
                    clickable.Clicked -= OnHiddenSignatureClicked;
            _subscribedClickables.Clear();
        }
    }
}
