using UnityEngine;
using UnityEngine.UI;
using System;

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
    [SerializeField] private Button btnShowReport;

    [Header("Panels")]
    [SerializeField] private GameObject panelQuestPosting;
    [SerializeField] private GameObject panelSituation;
    [SerializeField] private GameObject panelReport;

    [Header("Close (X) Buttons")]
    [SerializeField] private Button closeQuestPosting;
    [SerializeField] private Button closeSituation;
    [SerializeField] private Button closeReport;

    public event Action StudentIdPanelOpened;

    public Button StudentIdButton => btnQuestPosting;
    public GameObject StudentIdPanel => panelQuestPosting;

    private void Awake()
    {
        if (btnQuestPosting != null) btnQuestPosting.onClick.AddListener(OpenQuestPosting);
        if (btnShowSituation != null) btnShowSituation.onClick.AddListener(OpenSituation);
        if (btnShowReport != null) btnShowReport.onClick.AddListener(OpenReport);
        if (closeQuestPosting != null) closeQuestPosting.onClick.AddListener(CloseQuestPosting);
        if (closeSituation != null) closeSituation.onClick.AddListener(CloseSituation);
        if (closeReport != null) closeReport.onClick.AddListener(CloseReport);
    }

    private void OnDestroy()
    {
        if (btnQuestPosting != null) btnQuestPosting.onClick.RemoveListener(OpenQuestPosting);
        if (btnShowSituation != null) btnShowSituation.onClick.RemoveListener(OpenSituation);
        if (btnShowReport != null) btnShowReport.onClick.RemoveListener(OpenReport);
        if (closeQuestPosting != null) closeQuestPosting.onClick.RemoveListener(CloseQuestPosting);
        if (closeSituation != null) closeSituation.onClick.RemoveListener(CloseSituation);
        if (closeReport != null) closeReport.onClick.RemoveListener(CloseReport);
    }

    private void OpenQuestPosting()
    {
        SetActive(panelQuestPosting, true);
        StudentIdPanelOpened?.Invoke();
    }

    public void SetStudentIdButtonInteractable(bool interactable)
    {
        if (btnQuestPosting != null)
            btnQuestPosting.interactable = interactable;
    }
    private void OpenSituation() => SetActive(panelSituation, true);
    private void OpenReport() => SetActive(panelReport, true);
    private void CloseQuestPosting() => SetActive(panelQuestPosting, false);
    private void CloseSituation() => SetActive(panelSituation, false);
    private void CloseReport() => SetActive(panelReport, false);

    private static void SetActive(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }
}
