using MageAcademy.SaveSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MageAcademy.UI
{
    /// <summary>
    /// 타이틀 씬 진입 처리. 아무 키/마우스 클릭/게임패드 입력 시 세이브 유무로 분기한다.
    /// - 세이브 있음: 인게임 씬으로 이동해 저장된 지점부터 재개.
    /// - 세이브 없음: 튜토리얼 씬으로 이동.
    /// (프로젝트는 Input System 패키지를 사용하므로 레거시 Input 클래스는 쓰지 않는다.)
    /// </summary>
    public class TitleSceneController : MonoBehaviour
    {
        [SerializeField] private string _inGameSceneName = "InGameScene";
        [SerializeField] private string _tutorialSceneName = "TutorialScene";

        private bool _transitioning;

        private void Update()
        {
            if (_transitioning)
                return;

            if (AnyInputThisFrame())
                EnterGame();
        }

        private static bool AnyInputThisFrame()
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                return true;
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                return true;
            if (Gamepad.current != null &&
                (Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame))
                return true;
            return false;
        }

        private void EnterGame()
        {
            _transitioning = true;

            bool hasSave = SaveSystem.SaveSystem.HasSave();
            string target = hasSave ? _inGameSceneName : _tutorialSceneName;

            if (string.IsNullOrEmpty(target))
            {
                Debug.LogError("[Title] 이동할 씬 이름이 비어 있습니다.");
                _transitioning = false;
                return;
            }

            Debug.Log($"[Title] 세이브 {(hasSave ? "있음" : "없음")} → '{target}' 로드");
            SceneManager.LoadScene(target);
        }
    }
}
