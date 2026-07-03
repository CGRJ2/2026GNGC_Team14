using UnityEngine;

namespace GuildGame.Data
{
    /// <summary>
    /// 하루치 기획 데이터. 인원 제한 오버라이드와 하루 시작 이벤트(컷씬)를 정의한다.
    /// 이 SO가 없는 날짜는 DayScheduleSO의 공통 기본값으로 진행된다.
    /// </summary>
    [CreateAssetMenu(fileName = "DayConfig", menuName = "GuildGame/Day Config", order = 40)]
    public class DayConfigSO : ScriptableObject
    {
        [Min(1)]
        [Tooltip("몇 일차 데이터인지 (1일차 = 1)")]
        public int day = 1;

        [Header("인원 제한")]
        [Tooltip("체크하면 이 날은 공통 기본값 대신 아래 studentLimit을 사용")]
        public bool overrideStudentLimit;

        [Min(1)]
        [Tooltip("이 날의 검사 인원 제한 (overrideStudentLimit이 true일 때만 적용)")]
        public int studentLimit = 4;

        [Header("하루 시작 이벤트")]
        [Tooltip("하루 시작(페이드인 완료) 후 재생할 컷씬. 없으면 바로 학생 입장")]
        public CutsceneSO dayStartCutscene;
    }
}
