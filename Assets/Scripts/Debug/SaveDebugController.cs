using UnityEngine;
using UnityEngine.InputSystem;

namespace MageAcademy.DebugTools
{
    /// <summary>
    /// 디버그용 세이브 초기화. 지정 키를 누르면 세이브 파일을 삭제한다.
    /// 타이틀/인게임 등 어느 씬에나 배치할 수 있다.
    /// (프로젝트는 Input System 패키지를 사용하므로 Key 열거형을 쓴다.)
    /// </summary>
    public class SaveDebugController : MonoBehaviour
    {
        [Tooltip("이 키를 누르면 세이브를 삭제한다.")]
        [SerializeField] private Key _resetKey = Key.F9;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard[_resetKey].wasPressedThisFrame)
            {
                SaveSystem.SaveSystem.Delete();
                Debug.Log($"[SaveDebug] '{_resetKey}' 입력 → 세이브 초기화");
            }
        }
    }
}
