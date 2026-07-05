using System;
using System.Collections.Generic;
using MageAcademy.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Panel_Table에 부착. 테이블 버튼으로 각 패널을 열고 스택으로 관리한다.
/// - 패널을 클릭/드래그하면 맨 위로 올라온다.
/// - 패널 밖 빈 배경을 클릭하면 최상단 패널이 닫힌다(맨 위부터).
/// - X(닫기) 버튼은 사용하지 않는다(비활성화).
/// </summary>
public class PanelTableController : MonoBehaviour
{
    [Header("Open Buttons (테이블)")]
    [SerializeField] private Button btnQuestPosting;
    [SerializeField] private Button btnShowSituation;
    [SerializeField] private Button btnShowReport;
    [SerializeField] private Button btnShowUVPics;

    [Header("Panels")]
    [SerializeField] private GameObject panelQuestPosting;
    [SerializeField] private GameObject panelSituation;
    [SerializeField] private GameObject panelReport;
    [SerializeField] private GameObject panelUVPics;

    [Header("Close (X) Buttons (미사용 - 빈공간 클릭으로 닫음)")]
    [SerializeField] private Button closeQuestPosting;
    [SerializeField] private Button closeSituation;
    [SerializeField] private Button closeReport;
    [SerializeField] private Button closeUVPics;

    [Header("SFX")]
    [SerializeField] private AudioClip crystalPanelToggleClip;

    public event Action StudentIdPanelOpened;

    public Button StudentIdButton => btnQuestPosting;
    public GameObject StudentIdPanel => panelQuestPosting;

    private readonly List<GameObject> _panels = new();
    private readonly List<GameObject> _openStack = new();
    private static readonly List<RaycastResult> _raycastResults = new();

    // 튜토리얼 등에서 학생증 패널이 빈 배경 클릭으로 닫히는 것을 막는 잠금.
    private bool _studentIdLocked;

    private void Awake()
    {
        _panels.Clear();
        AddPanel(panelQuestPosting);
        AddPanel(panelSituation);
        AddPanel(panelReport);
        AddPanel(panelUVPics);

        if (btnQuestPosting != null) btnQuestPosting.onClick.AddListener(OpenQuestPosting);
        if (btnShowSituation != null) btnShowSituation.onClick.AddListener(OpenSituation);
        if (btnShowReport != null) btnShowReport.onClick.AddListener(OpenReport);
        if (btnShowUVPics != null) btnShowUVPics.onClick.AddListener(OpenUVPics);

        // X(닫기) 버튼 제거: 비활성화하고 배선하지 않는다.
        DisableCloseButton(closeQuestPosting);
        DisableCloseButton(closeSituation);
        DisableCloseButton(closeReport);
        DisableCloseButton(closeUVPics);
    }

    private void OnDestroy()
    {
        if (btnQuestPosting != null) btnQuestPosting.onClick.RemoveListener(OpenQuestPosting);
        if (btnShowSituation != null) btnShowSituation.onClick.RemoveListener(OpenSituation);
        if (btnShowReport != null) btnShowReport.onClick.RemoveListener(OpenReport);
        if (btnShowUVPics != null) btnShowUVPics.onClick.RemoveListener(OpenUVPics);
    }

    private void Update()
    {
        PruneStack();

        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector2 pos = Mouse.current.position.ReadValue();
        GameObject clickedPanel = FindClickedOpenPanel(pos, out bool overSelectable);

        if (clickedPanel != null)
        {
            BringToTop(clickedPanel); // 클릭/드래그한 패널을 맨 위로
            return;
        }

        if (overSelectable)
            return; // 버튼 등 인터랙티브 UI는 무시(그 동작 수행)

        CloseTop(); // 빈 배경 클릭 → 최상단 패널 닫기
    }

    public void SetStudentIdButtonInteractable(bool interactable)
    {
        if (btnQuestPosting != null)
            btnQuestPosting.interactable = interactable;
    }

    /// <summary>학생증 패널을 빈 배경 클릭으로 닫지 못하도록 잠근다(튜토리얼 보호용).</summary>
    public void SetStudentIdPanelLocked(bool locked) => _studentIdLocked = locked;

    /// <summary>현재 열린 패널들을 스택 위(최근)부터 순서대로 반환한다(파도타기 퇴장용).</summary>
    public List<GameObject> GetOpenPanelsTopFirst(bool includeStudentId)
    {
        var result = new List<GameObject>();
        for (int i = _openStack.Count - 1; i >= 0; i--)
        {
            GameObject p = _openStack[i];
            if (p == null || !p.activeInHierarchy)
                continue;
            if (!includeStudentId && p == panelQuestPosting)
                continue;
            result.Add(p);
        }
        return result;
    }

    /// <summary>외부(퇴장 연출)가 패널들을 닫았음을 알린다. 스택을 비운다.</summary>
    public void NotifyPanelsDismissed()
    {
        _openStack.Clear();
    }

    private void OpenQuestPosting()
    {
        OpenPanel(panelQuestPosting);
        StudentIdPanelOpened?.Invoke();
    }

    private void OpenSituation()
    {
        PlaySfx(crystalPanelToggleClip);
        OpenPanel(panelSituation);
    }

    private void OpenReport() => OpenPanel(panelReport);
    private void OpenUVPics() => OpenPanel(panelUVPics);

    private void OpenPanel(GameObject panel)
    {
        if (panel == null)
            return;

        panel.SetActive(true);
        BringToTop(panel);
    }

    private void BringToTop(GameObject panel)
    {
        _openStack.Remove(panel);
        _openStack.Add(panel);
        panel.transform.SetAsLastSibling();
    }

    private void CloseTop()
    {
        for (int i = _openStack.Count - 1; i >= 0; i--)
        {
            GameObject p = _openStack[i];
            if (p == null || !p.activeInHierarchy)
            {
                _openStack.RemoveAt(i);
                continue;
            }

            // 잠금 중엔 학생증 패널을 닫지 않고 그 아래 패널을 탐색한다.
            if (_studentIdLocked && p == panelQuestPosting)
                continue;

            _openStack.RemoveAt(i);
            p.SetActive(false);
            if (p == panelSituation)
                PlaySfx(crystalPanelToggleClip);
            return;
        }
    }

    private void PruneStack()
    {
        for (int i = _openStack.Count - 1; i >= 0; i--)
            if (_openStack[i] == null || !_openStack[i].activeInHierarchy)
                _openStack.RemoveAt(i);
    }

    private GameObject FindClickedOpenPanel(Vector2 screenPos, out bool overSelectable)
    {
        overSelectable = false;
        if (EventSystem.current == null)
            return null;

        var pointer = new PointerEventData(EventSystem.current) { position = screenPos };
        _raycastResults.Clear();
        EventSystem.current.RaycastAll(pointer, _raycastResults);

        if (_raycastResults.Count > 0)
            overSelectable = _raycastResults[0].gameObject.GetComponentInParent<Selectable>() != null;

        foreach (RaycastResult r in _raycastResults)
        {
            GameObject panel = FindOwningOpenPanel(r.gameObject);
            if (panel != null)
                return panel;
        }
        return null;
    }

    private GameObject FindOwningOpenPanel(GameObject hit)
    {
        if (hit == null)
            return null;

        Transform ht = hit.transform;
        foreach (GameObject panel in _panels)
        {
            if (panel == null || !panel.activeInHierarchy)
                continue;
            if (ht == panel.transform || ht.IsChildOf(panel.transform))
                return panel;
        }
        return null;
    }

    private void AddPanel(GameObject panel)
    {
        if (panel != null)
            _panels.Add(panel);
    }

    private static void DisableCloseButton(Button button)
    {
        if (button != null)
            button.gameObject.SetActive(false);
    }

    private static void PlaySfx(AudioClip clip)
    {
        if (clip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(clip);
    }
}
