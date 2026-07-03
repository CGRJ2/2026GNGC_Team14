using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel_Table에 부착. 테이블의 버튼으로 각 패널을 열고, 각 패널의 X 버튼으로 닫는다.
/// - Btn_QuestPosting  → Panel_QuestPosting 활성화
/// - Btn_ShowSituation → Panel_Situation 활성화
/// - 각 패널의 X 버튼   → 해당 패널 비활성화
/// </summary>
public class PanelTableController : MonoBehaviour
{
    [Header("Open Buttons (테이블)")]
    [SerializeField] private Button btnQuestPosting;
    [SerializeField] private Button btnShowSituation;

    [Header("Panels")]
    [SerializeField] private GameObject panelQuestPosting;
    [SerializeField] private GameObject panelSituation;

    [Header("Close (X) Buttons")]
    [SerializeField] private Button closeQuestPosting;
    [SerializeField] private Button closeSituation;

    private void Awake()
    {
        if (btnQuestPosting != null) btnQuestPosting.onClick.AddListener(OpenQuestPosting);
        if (btnShowSituation != null) btnShowSituation.onClick.AddListener(OpenSituation);
        if (closeQuestPosting != null) closeQuestPosting.onClick.AddListener(CloseQuestPosting);
        if (closeSituation != null) closeSituation.onClick.AddListener(CloseSituation);
    }

    private void OnDestroy()
    {
        if (btnQuestPosting != null) btnQuestPosting.onClick.RemoveListener(OpenQuestPosting);
        if (btnShowSituation != null) btnShowSituation.onClick.RemoveListener(OpenSituation);
        if (closeQuestPosting != null) closeQuestPosting.onClick.RemoveListener(CloseQuestPosting);
        if (closeSituation != null) closeSituation.onClick.RemoveListener(CloseSituation);
    }

    private void OpenQuestPosting() => SetActive(panelQuestPosting, true);
    private void OpenSituation() => SetActive(panelSituation, true);
    private void CloseQuestPosting() => SetActive(panelQuestPosting, false);
    private void CloseSituation() => SetActive(panelSituation, false);

    private static void SetActive(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }
}
