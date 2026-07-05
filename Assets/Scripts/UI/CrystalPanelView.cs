using MageAcademy.Gameplay.Models;
using MageAcademy.Data;
using UnityEngine;
using UnityEngine.UI;

namespace MageAcademy.UI
{
    /// <summary>
    /// 수정구슬(알리바이) 패널. 어제 장면 일러스트를 보여주고, "질문하기"로 학생 진술을 듣는다.
    /// 플레이어는 진술과 장면을 대조해 본인이 직접 했는지 판별한다.
    /// 수정구슬을 요구하지 않는 날(Crystal==null)에는 열기 버튼을 숨긴다.
    /// 뷰는 이벤트 구독만 하고 모델을 변경하지 않는다.
    /// </summary>
    public class CrystalPanelView : UIViewBase
    {
        [Tooltip("어제 장면 일러스트")]
        [SerializeField] private Image _sceneImage;
        [SerializeField] private Animator _sceneAnimator;
        [SerializeField] private string _sceneAnimatorObjectName = "AnimatedSprite";
        [SerializeField] private string _fallbackAnimationKey = "Study";

        [Tooltip("질문하기 버튼(학생 진술 유도)")]
        [SerializeField] private Button _questionButton;

        [Tooltip("테이블의 수정구슬 열기 버튼. 수정구슬 없는 날엔 숨긴다.")]
        [SerializeField] private Button _openButton;

        private UIDraggablePanel _draggablePanel;

        protected override void OnBind()
        {
            if (_questionButton != null)
                _questionButton.onClick.AddListener(OnQuestionClicked);
            EnsureDraggablePanel();

            Context.DayRequirementsPrepared += OnDayRequirementsPrepared;
            Context.CaseStarted += OnCaseStarted;

            gameObject.SetActive(false);
            SetOpenButtonVisible(false);
        }

        private void OnCaseStarted(StudentCase studentCase)
        {
            CrystalData crystal = studentCase != null ? studentCase.Crystal : null;

            if (crystal == null)
            {
                gameObject.SetActive(false);
                SetOpenButtonVisible(false);
                return;
            }

            SetOpenButtonVisible(true);

            PlaySceneAnimation(crystal);
        }

        private void OnDayRequirementsPrepared(DayConfigSO config)
        {
            gameObject.SetActive(false);
            SetOpenButtonVisible(config != null && config.requiresCrystal);
        }

        private void OnQuestionClicked()
        {
            Context.RequestCrystalQuestion();
        }

        private void SetOpenButtonVisible(bool visible)
        {
            if (_openButton != null)
                _openButton.gameObject.SetActive(visible);
        }

        private void PlaySceneAnimation(CrystalData crystal)
        {
            if (TryPlayAnimatorState(crystal != null ? crystal.AnimationKey : string.Empty))
            {
                if (_sceneImage != null)
                    _sceneImage.enabled = false;
                return;
            }

            if (_sceneImage != null && crystal != null)
            {
                _sceneImage.sprite = crystal.Scene;
                _sceneImage.enabled = crystal.Scene != null;
            }
        }

        private bool TryPlayAnimatorState(string animationKey)
        {
            Animator animator = SceneAnimator;
            if (animator == null || animator.runtimeAnimatorController == null)
                return false;

            string stateName = string.IsNullOrWhiteSpace(animationKey) ? _fallbackAnimationKey : animationKey;
            if (string.IsNullOrWhiteSpace(stateName))
                return false;

            int stateHash = Animator.StringToHash(stateName);
            if (!animator.HasState(0, stateHash))
            {
                Debug.LogWarning($"Crystal scene animation state not found: {stateName}", this);
                return false;
            }

            animator.Play(stateHash, 0, 0f);
            animator.Update(0f);
            return true;
        }

        private Animator SceneAnimator
        {
            get
            {
                if (_sceneAnimator != null)
                    return _sceneAnimator;

                if (string.IsNullOrWhiteSpace(_sceneAnimatorObjectName))
                    return null;

                GameObject animatorObject = GameObject.Find(_sceneAnimatorObjectName);
                if (animatorObject != null)
                    _sceneAnimator = animatorObject.GetComponent<Animator>();

                return _sceneAnimator;
            }
        }

        private void EnsureDraggablePanel()
        {
            if (_draggablePanel != null)
                return;

            _draggablePanel = GetComponent<UIDraggablePanel>();
            if (_draggablePanel == null)
                _draggablePanel = gameObject.AddComponent<UIDraggablePanel>();
        }

        private void OnDestroy()
        {
            if (Context != null)
            {
                Context.DayRequirementsPrepared -= OnDayRequirementsPrepared;
                Context.CaseStarted -= OnCaseStarted;
            }

            if (_questionButton != null)
                _questionButton.onClick.RemoveListener(OnQuestionClicked);
        }
    }
}
