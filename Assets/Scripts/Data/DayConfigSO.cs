using UnityEngine;

namespace GuildGame.Data
{
    /// <summary>
    /// 하루치 기획 데이터. 날짜별 검사 인원 제한과 하루 시작 컷씬을 정의한다.
    /// </summary>
    [CreateAssetMenu(fileName = "DayConfig", menuName = "GuildGame/Day Config", order = 40)]
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
    }
}
