using GuildGame.Core;
using GuildGame.Data;

namespace GuildGame.Gameplay.Models
{
    /// <summary>
    /// 날짜 진행 스탯. 현재 일차와 오늘 처리한 학생 수를 반응형으로 노출하고,
    /// 일자별 인원 제한은 DayScheduleSO에서 조회한다. 변경은 CountProcessed/AdvanceDay로만 한다.
    /// </summary>
    public class DayModel
    {
        private readonly DayScheduleSO _schedule;

        public ObservableProperty<int> CurrentDay { get; }
        public ObservableProperty<int> ProcessedToday { get; }

        /// <summary>오늘의 검사 인원 제한.</summary>
        public int TodayLimit => _schedule.GetStudentLimit(CurrentDay.Value);

        /// <summary>오늘 제한 인원을 모두 처리했는지.</summary>
        public bool IsQuotaReached => ProcessedToday.Value >= TodayLimit;

        /// <summary>오늘의 일자별 데이터. 등록되지 않은 날짜면 null.</summary>
        public DayConfigSO TodayConfig => _schedule.GetConfig(CurrentDay.Value);

        public DayModel(DayScheduleSO schedule, int startDay = 1)
        {
            _schedule = schedule;
            CurrentDay = new ObservableProperty<int>(startDay);
            ProcessedToday = new ObservableProperty<int>(0);
        }

        /// <summary>학생 1명 처리 완료를 기록한다(Resolution에서 호출).</summary>
        public void CountProcessed()
        {
            ProcessedToday.Value += 1;
        }

        /// <summary>다음 날로 넘어간다. 처리 인원은 리셋된다.</summary>
        public void AdvanceDay()
        {
            CurrentDay.Value += 1;
            ProcessedToday.Value = 0;
        }
    }
}
