using GuildGame.Core;

namespace GuildGame.Gameplay.Models
{
    /// <summary>
    /// 길드 평판 스탯. 반응형 값을 노출해 View가 구독하고, 변경은 <see cref="Apply"/>로만 한다.
    /// </summary>
    public class ReputationModel
    {
        public ObservableProperty<int> Value { get; }

        public ReputationModel(int initial)
        {
            Value = new ObservableProperty<int>(initial);
        }

        /// <summary>평판을 증감한다(음수 가능).</summary>
        public void Apply(int delta)
        {
            Value.Value += delta;
        }
    }
}
