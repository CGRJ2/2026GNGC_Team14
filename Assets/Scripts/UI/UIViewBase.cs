using MageAcademy.Gameplay.Flow;
using UnityEngine;

namespace MageAcademy.UI
{
    /// <summary>
    /// 모든 UI 뷰의 베이스. 컨트롤러가 <see cref="Bind"/>로 컨텍스트를 주입한다.
    /// 뷰는 이벤트를 구독만 하고 모델/상태를 직접 변경하지 않는다.
    /// </summary>
    public abstract class UIViewBase : MonoBehaviour
    {
        protected GameContext Context { get; private set; }

        public void Bind(GameContext context)
        {
            Context = context;
            OnBind();
        }

        /// <summary>컨텍스트 주입 후 구독을 설정하는 훅.</summary>
        protected abstract void OnBind();
    }
}
