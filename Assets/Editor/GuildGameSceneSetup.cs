using GuildGame.Data;
using GuildGame.Gameplay.Flow;
using GuildGame.Gameplay.Managers;
using GuildGame.Localization;
using GuildGame.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GuildGame.EditorTools
{
    /// <summary>
    /// MVP 플레이 씬을 코드로 구성한다(Canvas · 뷰 7종 · 버튼 템플릿 · 매니저 · 직렬화 참조 배선).
    /// 메뉴: GuildGame/2. Build Scene. 먼저 "1. Create Sample Data"를 실행해야 한다.
    /// </summary>
    public static class GuildGameSceneSetup
    {
        private static readonly Color PanelColor = new(0.93f, 0.90f, 0.82f, 1f);
        private static readonly Color PopupColor = new(0.15f, 0.13f, 0.10f, 0.96f);
        private static readonly Color Ink = new(0.12f, 0.10f, 0.08f, 1f);

        private const string RootName = "GuildGameMVP";
        private const string SampleScenePath = "Assets/Scenes/SampleScene.unity";

        [MenuItem("GuildGame/2. Build Scene (New)")]
        public static void BuildScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            PopulateActiveScene();

            const string dir = "Assets/Scenes";
            if (!AssetDatabase.IsValidFolder(dir))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(scene, $"{dir}/GuildDesk.unity");
            AssetDatabase.Refresh();
            Debug.Log("[GuildGameSetup] GuildDesk 씬 구성 완료.");
        }

        [MenuItem("GuildGame/3. Build Into Sample Scene")]
        public static void BuildIntoSampleScene()
        {
            var scene = EditorSceneManager.OpenScene(SampleScenePath, OpenSceneMode.Single);
            PopulateActiveScene();
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.Refresh();
            Debug.Log("[GuildGameSetup] SampleScene에 임시 UI 패널 구성 완료. Play로 실행하세요.");
        }

        /// <summary>현재 활성 씬에 임시 UI를 구성한다. 기존 GuildGameMVP 루트는 제거 후 재생성(멱등).</summary>
        public static void PopulateActiveScene()
        {
            var existing = GameObject.Find(RootName);
            if (existing != null)
                Object.DestroyImmediate(existing);

            var root = new GameObject(RootName);

            // ---- Canvas ----
            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(root.transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // ---- EventSystem (Input System UI 모듈) ----
            var esGo = new GameObject("EventSystem", typeof(EventSystem));
            esGo.transform.SetParent(root.transform, false);
            var inputModule = esGo.AddComponent<InputSystemUIInputModule>();
            // 프로젝트의 InputActionAsset(UI 맵 포함)을 배선 → 버튼 클릭 동작 보장.
            var uiActions = AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(
                "Assets/InputSystem_Actions.inputactions");
            if (uiActions != null)
                inputModule.actionsAsset = uiActions;
            else
                Debug.LogWarning("[GuildGameSetup] InputSystem_Actions.inputactions를 찾을 수 없어 UI 입력이 비활성일 수 있음.");

            // ---- 배경 ----
            var bg = NewUI("Background", canvasGo.transform);
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.08f, 0.07f, 0.06f, 1f);
            Stretch(bg.GetComponent<RectTransform>(), 0, 0, 1, 1, 0);

            // ---- 상단바: 평판 ----
            var repPanel = Panel("TopBar", canvasGo.transform, 0f, 0.92f, 0.70f, 1f);
            var repView = repPanel.AddComponent<ReputationView>();
            var repLabel = Text("ReputationLabel", repPanel.transform, "평판: --", 40, TextAlignmentOptions.MidlineLeft);
            StretchChild(repLabel.rectTransform, 16);
            SetRef(repView, "_label", repLabel);

            // ---- 의뢰서 패널 (좌측) ----
            var posting = Panel("QuestPosting", canvasGo.transform, 0f, 0.05f, 0.45f, 0.92f);
            AddVerticalLayout(posting);
            var postView = posting.AddComponent<QuestPostingView>();
            var header = Text("Header", posting.transform, "의뢰서", 44, TextAlignmentOptions.Center, true);
            var title = Text("Title", posting.transform, "--", 36, TextAlignmentOptions.Left, true, bold: true);
            var summary = Text("Summary", posting.transform, "--", 28, TextAlignmentOptions.TopLeft, true);
            var facts = Text("Facts", posting.transform, "--", 28, TextAlignmentOptions.TopLeft, true);
            LayoutHeight(header, 60); LayoutHeight(title, 60); LayoutHeight(summary, 120); LayoutHeight(facts, 240);
            SetRef(postView, "_headerLabel", header);
            SetRef(postView, "_titleLabel", title);
            SetRef(postView, "_summaryLabel", summary);
            SetRef(postView, "_factsLabel", facts);

            // ---- 모험가 패널 (우상단) ----
            var advPanel = Panel("Adventurer", canvasGo.transform, 0.46f, 0.78f, 0.70f, 0.92f);
            AddVerticalLayout(advPanel);
            var advView = advPanel.AddComponent<AdventurerView>();
            var advName = Text("Name", advPanel.transform, "--", 36, TextAlignmentOptions.Left, true, bold: true);
            var advClaim = Text("Claim", advPanel.transform, "--", 28, TextAlignmentOptions.Left, true);
            LayoutHeight(advName, 50); LayoutHeight(advClaim, 50);
            SetRef(advView, "_nameLabel", advName);
            SetRef(advView, "_claimLabel", advClaim);

            // ---- 대화 로그 패널 (우중단) ----
            var dlgPanel = Panel("Dialogue", canvasGo.transform, 0.46f, 0.35f, 0.70f, 0.77f);
            var dlgView = dlgPanel.AddComponent<DialogueView>();
            var dlgLabel = Text("Log", dlgPanel.transform, "", 26, TextAlignmentOptions.TopLeft);
            StretchChild(dlgLabel.rectTransform, 16);
            SetRef(dlgView, "_logLabel", dlgLabel);

            // ---- 질문 버튼 패널 (우하단) ----
            var qPanel = Panel("Questions", canvasGo.transform, 0.46f, 0.12f, 0.70f, 0.34f);
            var content = NewUI("Content", qPanel.transform);
            StretchChild(content.GetComponent<RectTransform>(), 8);
            AddVerticalLayout(content);
            var qView = qPanel.AddComponent<QuestionButtonsView>();

            // 질문 버튼 템플릿(비활성) — 캔버스 밖에 두고 프리팹 대용으로 사용.
            var template = MakeButton("QuestionButtonTemplate", canvasGo.transform, "질문 템플릿");
            template.gameObject.SetActive(false);
            SetRef(qView, "_container", content.GetComponent<RectTransform>());
            SetRef(qView, "_buttonPrefab", template);

            // ---- 판정 버튼 패널 (맨 아래, 상시) ----
            var vPanel = Panel("Verdict", canvasGo.transform, 0.46f, 0.05f, 0.70f, 0.11f);
            AddHorizontalLayout(vPanel);
            var vView = vPanel.AddComponent<VerdictButtonsView>();
            var completeBtn = MakeButton("CompleteButton", vPanel.transform, "의뢰 완료", new Color(0.3f, 0.55f, 0.3f));
            var failBtn = MakeButton("FailButton", vPanel.transform, "의뢰 실패", new Color(0.6f, 0.3f, 0.3f));
            SetRef(vView, "_completeButton", completeBtn);
            SetRef(vView, "_failButton", failBtn);
            SetRef(vView, "_completeLabel", completeBtn.GetComponentInChildren<TMP_Text>());
            SetRef(vView, "_failLabel", failBtn.GetComponentInChildren<TMP_Text>());

            // ---- 디버그 패널 (정답표, 우측 열) ----
            var dbgPanel = Panel("DebugPanel", canvasGo.transform, 0.71f, 0.05f, 1f, 0.92f, new Color(0.10f, 0.10f, 0.13f, 0.94f));
            var dbgView = dbgPanel.AddComponent<DebugCasePanelView>();
            var dbgLabel = Text("DebugLog", dbgPanel.transform, "", 20, TextAlignmentOptions.TopLeft);
            dbgLabel.color = Color.white;
            StretchChild(dbgLabel.rectTransform, 12);
            SetRef(dbgView, "_label", dbgLabel);

            // ---- 결과 팝업 (오버레이, 초기 비활성) ----
            var popupRoot = Panel("EventPopupRoot", canvasGo.transform, 0f, 0f, 1f, 1f, PopupColor);
            var popView = popupRoot.AddComponent<EventPopupView>();
            var popupText = Text("EventText", popupRoot.transform, "--", 40, TextAlignmentOptions.Center);
            popupText.color = Color.white;
            Stretch(popupText.rectTransform, 0.15f, 0.4f, 0.85f, 0.7f, 0);
            var nextBtn = MakeButton("NextButton", popupRoot.transform, "다음 손님", new Color(0.35f, 0.35f, 0.45f));
            Stretch(nextBtn.GetComponent<RectTransform>(), 0.4f, 0.25f, 0.6f, 0.33f, 0);
            SetRef(popView, "_panel", popupRoot); // 패널 자체를 토글
            SetRef(popView, "_eventLabel", popupText);
            SetRef(popView, "_nextButton", nextBtn);
            SetRef(popView, "_nextLabel", nextBtn.GetComponentInChildren<TMP_Text>());

            // ---- 매니저 ----
            var managers = new GameObject("Managers");
            managers.transform.SetParent(root.transform, false);
            managers.AddComponent<LocalizationManager>();

            var questManager = managers.AddComponent<QuestManager>();
            SetRef(questManager, "_questDatabase", Load<QuestDatabaseSO>("Assets/Data/Databases/QuestDatabase.asset"));
            SetRef(questManager, "_questionDatabase", Load<QuestionDatabaseSO>("Assets/Data/Databases/QuestionDatabase.asset"));

            var controller = managers.AddComponent<GuildDeskController>();
            SetRef(controller, "_balance", Load<GameBalanceSO>("Assets/Data/Balance/GameBalance.asset"));

            EditorUtility.SetDirty(root);
        }

        // ---------------------------------------------------------------- helpers

        private static GameObject NewUI(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static GameObject Panel(string name, Transform parent, float minX, float minY, float maxX, float maxY)
            => Panel(name, parent, minX, minY, maxX, maxY, PanelColor);

        private static GameObject Panel(string name, Transform parent, float minX, float minY, float maxX, float maxY, Color color)
        {
            var go = NewUI(name, parent);
            var img = go.AddComponent<Image>();
            img.color = color;
            Stretch(go.GetComponent<RectTransform>(), minX, minY, maxX, maxY, 6);
            return go;
        }

        private static TextMeshProUGUI Text(string name, Transform parent, string text, int size,
            TextAlignmentOptions align, bool layoutChild = false, bool bold = false)
        {
            var go = NewUI(name, parent);
            var t = go.AddComponent<TextMeshProUGUI>();
            t.text = text;
            t.fontSize = size;
            t.alignment = align;
            t.color = Ink;
            if (bold) t.fontStyle = FontStyles.Bold;
            if (TMP_Settings.defaultFontAsset != null)
                t.font = TMP_Settings.defaultFontAsset;
            if (!layoutChild)
                StretchChild(t.rectTransform, 0);
            return t;
        }

        private static Button MakeButton(string name, Transform parent, string label)
            => MakeButton(name, parent, label, new Color(0.75f, 0.72f, 0.62f));

        private static Button MakeButton(string name, Transform parent, string label, Color color)
        {
            var go = NewUI(name, parent);
            var img = go.AddComponent<Image>();
            img.color = color;
            var btn = go.AddComponent<Button>();
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = 56; le.preferredHeight = 56; le.minWidth = 160; le.flexibleWidth = 1;

            var txt = Text("Label", go.transform, label, 30, TextAlignmentOptions.Center);
            txt.color = Color.white;
            StretchChild(txt.rectTransform, 4);
            return btn;
        }

        private static void AddVerticalLayout(GameObject go)
        {
            var v = go.AddComponent<VerticalLayoutGroup>();
            v.padding = new RectOffset(16, 16, 16, 16);
            v.spacing = 10;
            v.childForceExpandWidth = true;
            v.childForceExpandHeight = false;
            v.childControlWidth = true;
            v.childControlHeight = true;
        }

        private static void AddHorizontalLayout(GameObject go)
        {
            var h = go.AddComponent<HorizontalLayoutGroup>();
            h.padding = new RectOffset(12, 12, 6, 6);
            h.spacing = 16;
            h.childForceExpandWidth = true;
            h.childForceExpandHeight = true;
            h.childControlWidth = true;
            h.childControlHeight = true;
        }

        private static void LayoutHeight(Component c, float height)
        {
            var le = c.gameObject.AddComponent<LayoutElement>();
            le.minHeight = height;
            le.preferredHeight = height;
        }

        private static void Stretch(RectTransform rt, float minX, float minY, float maxX, float maxY, float pad = 8)
        {
            rt.anchorMin = new Vector2(minX, minY);
            rt.anchorMax = new Vector2(maxX, maxY);
            rt.offsetMin = new Vector2(pad, pad);
            rt.offsetMax = new Vector2(-pad, -pad);
        }

        private static void StretchChild(RectTransform rt, float pad)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(pad, pad);
            rt.offsetMax = new Vector2(-pad, -pad);
        }

        private static T Load<T>(string path) where T : Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
                Debug.LogError($"[GuildGameSetup] 에셋을 찾을 수 없음: {path} (먼저 '1. Create Sample Data' 실행)");
            return asset;
        }

        /// <summary>[SerializeField] private 필드를 SerializedObject로 안전하게 배선한다.</summary>
        private static void SetRef(Object target, string fieldName, Object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null)
            {
                Debug.LogWarning($"[GuildGameSetup] 필드 없음: {target.GetType().Name}.{fieldName}");
                return;
            }
            prop.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
