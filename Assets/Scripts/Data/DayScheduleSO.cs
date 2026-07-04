using System.Collections.Generic;
using UnityEngine;

namespace MageAcademy.Data
{
    /// <summary>
    /// 전체 날짜 데이터를 묶고, day 값을 key로 DayConfigSO를 조회한다.
    /// </summary>
    [CreateAssetMenu(fileName = "DaySchedule", menuName = "MageAcademy/Day Schedule", order = 41)]
    public class DayScheduleSO : ScriptableObject
    {
        [Tooltip("날짜별 데이터. day 중복 시 먼저 등록된 항목이 우선")]
        [SerializeField] private List<DayConfigSO> _days = new();

        private Dictionary<int, DayConfigSO> _byDay;

        /// <summary>해당 일자의 DayConfigSO를 반환한다. 등록하지 않은 날짜면 null.</summary>
        public DayConfigSO GetConfig(int day)
        {
            EnsureIndex();
            return _byDay.TryGetValue(day, out DayConfigSO config) ? config : null;
        }

        public bool HasConfig(int day)
        {
            EnsureIndex();
            return _byDay.ContainsKey(day);
        }

        /// <summary>해당 일자의 검사 인원 제한. 날짜 설정이 없으면 최소값으로 방어한다.</summary>
        public int GetStudentLimit(int day)
        {
            DayConfigSO config = GetConfig(day);
            if (config == null)
            {
                Debug.LogWarning($"[DaySchedule] {name}: day {day} config가 없어 studentLimit 1로 진행합니다.");
                return 1;
            }

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

                if (config.studentLimit <= 0)
                    Debug.LogWarning($"[DaySchedule] {name}: '{config.name}' studentLimit {config.studentLimit} <= 0, 1로 클램프됨.");

                _byDay.Add(config.day, config);
            }
        }

        private void OnValidate()
        {
            _byDay = null;
        }
    }
}
