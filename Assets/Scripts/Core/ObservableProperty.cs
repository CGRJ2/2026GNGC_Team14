using System;
using System.Collections.Generic;

namespace GuildGame.Core
{
    /// <summary>
    /// UI 연동 스탯을 위한 반응형 프로퍼티. View는 <see cref="OnChanged"/>를 구독만 하고,
    /// 값 변경은 소유자(Model/Service)만 수행한다.
    /// </summary>
    [Serializable]
    public class ObservableProperty<T>
    {
        private T _value;

        /// <summary>값이 실제로 바뀌었을 때 새 값과 함께 발행된다.</summary>
        public event Action<T> OnChanged;

        public ObservableProperty(T initialValue = default)
        {
            _value = initialValue;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                    return;

                _value = value;
                OnChanged?.Invoke(_value);
            }
        }

        /// <summary>현재 값으로 즉시 콜백을 한 번 호출한 뒤 구독한다. UI 초기 동기화에 사용.</summary>
        public void Subscribe(Action<T> handler, bool invokeImmediately = true)
        {
            if (handler == null)
                return;

            OnChanged += handler;
            if (invokeImmediately)
                handler(_value);
        }

        public void Unsubscribe(Action<T> handler)
        {
            if (handler != null)
                OnChanged -= handler;
        }
    }
}
