using UnityEngine;

namespace MageAcademy.Data
{
    /// <summary>
    /// 하루치 기획 데이터. 날짜별 검사 인원 제한과 하루 시작 컷씬을 정의한다.
    /// </summary>
    [CreateAssetMenu(fileName = "DayConfig", menuName = "MageAcademy/Day Config", order = 40)]
    public class DayConfigSO : ScriptableObject
    {
        [Min(0)]
        [Tooltip("몇 일차 데이터인지. 1일차 = 1")]
        public int day = 1;

        [Min(1)]
        [Tooltip("이 날 검사할 학생 수")]
        public int studentLimit = 4;

        [Tooltip("이 날 학생이 레포트를 제출하는지(레포트 검증면 활성화). Day2~ = true")]
        public bool requiresReport;

        [Tooltip("이 날 수정구슬(알리바이) 검증을 사용하는지. Day3~ = true")]
        public bool requiresCrystal;

        [Tooltip("이 날 UV 지팡이(골렘 흔적) 검증을 사용하는지. Day4~ = true")]
        public bool requiresUV;

        [Header("하루 시작 이벤트")]
        [Tooltip("하루 시작 연출이 끝난 뒤 재생할 컷씬. 없으면 바로 학생 입장")]
        public CutsceneSO dayStartCutscene;

        [Header("하루 종료 일러스트")]
        [Tooltip("이 날 종료 시 검은 화면 위에 띄울 풀스크린 일러스트. 없으면 생략")]
        public Sprite endDayIllustration;

        [TextArea]
        public string endDayIllustrationText;

        [Min(0f)]
        [Tooltip("종료 일러스트 노출 시간(초)")]
        public float endDayIllustrationDuration = 3f;
    }
}
