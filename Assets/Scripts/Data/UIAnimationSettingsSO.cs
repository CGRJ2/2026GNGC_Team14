using System;
using DG.Tweening;
using UnityEngine;

namespace GuildGame.Data
{
    /// <summary>
    /// UI 등장 연출(페이드+슬라이드)의 시간·거리·이징을 모은 설정.
    /// 코드에서 연출 수치를 하드코딩하지 않기 위한 단일 출처.
    /// </summary>
    [CreateAssetMenu(fileName = "UIAnimationSettings", menuName = "GuildGame/UI Animation Settings", order = 21)]
    public class UIAnimationSettingsSO : ScriptableObject
    {
        [Serializable]
        public class FadeSlideSettings
        {
            [Tooltip("이 연출이 시작되기 전 대기 시간(초)")]
            public float delay;

            [Tooltip("연출 길이(초)")]
            public float duration = 0.35f;

            [Tooltip("시작 위치 오프셋. 최종 위치 기준 (0,-60)이면 아래에서 위로 올라오며 등장")]
            public Vector2 startOffset = new Vector2(0f, -60f);

            [Tooltip("DOTween 이징")]
            public Ease ease = Ease.OutQuad;
        }

        [Header("학생 일러스트 등장 (아래 → 위)")]
        public FadeSlideSettings studentEnter = new()
        {
            duration = 0.4f,
            startOffset = new Vector2(0f, -60f),
        };

        [Header("학생증 버튼 등장 (위 → 아래, 일러스트 등장 완료 후)")]
        public FadeSlideSettings studentIdButton = new()
        {
            duration = 0.35f,
            startOffset = new Vector2(0f, 60f),
        };
    }
}
