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

        [Header("Dialogue")]
        [Tooltip("질문을 표시한 뒤 답변을 표시하기까지 기다리는 시간(초)")]
        [Min(0f)]
        public float dialogueAnswerDelay = 0.6f;

        [Tooltip("판정 결과 대사를 보여준 뒤 학생 퇴장을 시작하기까지 기다리는 시간(초)")]
        [Min(0f)]
        public float outcomeDialogueDelay = 1.2f;

        [Tooltip("학생 퇴장 완료 후 다음 학생이 나타나기까지 기다리는 시간(초)")]
        [Min(0f)]
        public float nextStudentDelay = 0.4f;

        [Tooltip("학생 입장 연출이 끝난 뒤 첫 인사 대사를 표시하기까지 추가로 기다리는 시간(초)")]
        [Min(0f)]
        public float studentGreetingDelay = 0f;

        [Tooltip("컷씬 대사 한 줄이 표시된 뒤 다음 줄로 넘어가기까지 기다리는 기본 시간(초)")]
        [Min(0f)]
        public float cutsceneLineDelay = 1.0f;

        [Header("학생 일러스트 퇴장 (위로 이동 + 페이드 아웃)")]
        public FadeSlideSettings studentExit = new()
        {
            duration = 0.4f,
            startOffset = new Vector2(0f, 80f),
        };

        [Header("학생증 버튼 퇴장")]
        public FadeSlideSettings studentIdButtonExit = new()
        {
            duration = 0.25f,
            startOffset = new Vector2(0f, 40f),
        };
    }
}
