using UnityEngine;

namespace MageAcademy.Data
{
    /// <summary>
    /// 하루치 기획 데이터. 날짜별 검사 인원 제한과 하루 시작 컷씬을 정의한다.
    /// </summary>
    [CreateAssetMenu(fileName = "DayConfig", menuName = "MageAcademy/Day Config", order = 40)]
    public class DayConfigSO : ScriptableObject
    {
        [Min(1)]
        [Tooltip("몇 일차 데이터인지. 1일차 = 1")]
        public int day = 1;

        [Min(1)]
        [Tooltip("이 날 검사할 학생 수")]
        public int studentLimit = 4;

        [Header("하루 시작 이벤트")]
        [Tooltip("하루 시작 연출이 끝난 뒤 재생할 컷씬. 없으면 바로 학생 입장")]
        public CutsceneSO dayStartCutscene;

        [Header("하루 종료 일러스트")]
        [Tooltip("이 날 종료 시 검은 화면 위에 띄울 풀스크린 일러스트. 없으면 생략")]
        public Sprite endDayIllustration;

        [Min(0f)]
        [Tooltip("종료 일러스트 노출 시간(초)")]
        public float endDayIllustrationDuration = 3f;
    }
}
