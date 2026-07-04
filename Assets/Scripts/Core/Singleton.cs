using UnityEngine;

namespace MageAcademy.Core
{
    /// <summary>
    /// MonoBehaviour 싱글턴 베이스. 씬에 배치된 인스턴스를 <see cref="Instance"/>로 노출한다.
    /// 중복 인스턴스는 파괴한다. 매니저 계층에서만 사용한다.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindAnyObjectByType<T>();
                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;
            OnAwake();
        }

        /// <summary>싱글턴이 확정된 뒤 호출되는 초기화 훅.</summary>
        protected virtual void OnAwake() { }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}
