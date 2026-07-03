using UnityEngine;
using GuildGame.UI;

/// <summary>
/// 현재 씬 Canvas 아래에 붙어 있는 UI 뷰들을 한곳에 모아두는 그룹.
/// (초기화/활성화 제어는 이후 직접 구성)
/// </summary>
public class InGameGroup : MonoBehaviour
{
    [SerializeField] private ReputationView reputationView;
    [SerializeField] private QuestPostingView questPostingView;
    [SerializeField] private AdventurerView adventurerView;
    [SerializeField] private DialogueView dialogueView;
    [SerializeField] private QuestionButtonsView questionButtonsView;
    [SerializeField] private VerdictButtonsView verdictButtonsView;
    [SerializeField] private DebugCasePanelView debugCasePanelView;
    [SerializeField] private EventPopupView eventPopupView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
