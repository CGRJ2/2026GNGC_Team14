using System.Collections.Generic;
using UnityEngine;

namespace GuildGame.Data
{
    /// <summary>
    /// 전체 일자별 데이터 묶음. 공통 인원 제한 기본값과 일자별 DayConfigSO 목록을 갖고,
    /// day를 key로 하는 딕셔너리로 조회한다. 등록되지 않은 날짜는 공통 기본값으로 진행된다.
    /// </summary>
    [CreateAssetMenu(fileName = "DaySchedule", menuName = "GuildGame/Day Schedule", order = 41)]
    public class DayScheduleSO : ScriptableObject
    {
        [Min(1)]
        [Tooltip("모든 날짜 공통 검사 인원 제한 (일자별 오버라이드가 없을 때 사용)")]
        public int defaultStudentLimit = 8;

        [Tooltip("일자별 데이터. day 중복 시 먼저 등록된 항목이 우선")]
        [SerializeField] private List<DayConfigSO> _days = new();

        private Dictionary<int, DayConfigSO> _byDay;

        /// <summary>해당 일자의 DayConfigSO를 반환한다. 등록되지 않은 날짜면 null.</summary>
        public DayConfigSO GetConfig(int day)
        {
            EnsureIndex();
            return _byDay.TryGetValue(day, out DayConfigSO config) ? config : null;
        }

        /// <summary>해당 일자의 검사 인원 제한. 오버라이드가 없으면 공통 기본값.</summary>
        public int GetStudentLimit(int day)
        {
            DayConfigSO config = GetConfig(day);
            if (config == null || !config.overrideStudentLimit)
                return defaultStudentLimit;

            return Mathf.Max(1, config.studentLimit);
        }

        private void EnsureIndex()
        {
            if (_byDay != null)
                return;

            _byDay = new Dictionary<int, DayConfigSO>();
            foreach (DayConfigSO config in _days)
            {
                if (config == null)
                {
                    Debug.LogWarning($"[DaySchedule] {name}: null 항목이 있어 무시함.");
                    continue;
                }

                if (_byDay.ContainsKey(config.day))
                {
                    Debug.LogWarning($"[DaySchedule] {name}: day {config.day} 중복. '{config.name}' 무시함.");
                    continue;
                }

                if (config.overrideStudentLimit && config.studentLimit <= 0)
                    Debug.LogWarning($"[DaySchedule] {name}: '{config.name}' studentLimit {config.studentLimit} <= 0, 1로 클램프됨.");

                _byDay.Add(config.day, config);
            }
        }

        private void OnValidate()
        {
            // 에디터에서 목록 수정 시 다음 조회 때 딕셔너리 재빌드
            _byDay = null;
        }
    }
}
