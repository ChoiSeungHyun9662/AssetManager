using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssetManager
{
    public readonly struct ProjectShellRoots
    {
        public ProjectShellRoots(GameObject gameRoot, Canvas uiCanvas)
        {
            GameRoot = gameRoot;
            UiCanvas = uiCanvas;
        }

        public GameObject GameRoot { get; }
        public Canvas UiCanvas { get; }
    }

    public static class ProjectShell
    {
        public const string AssetRootPath = "Assets/_AssetManager";
        public const string BootstrapSceneName = "Bootstrap";
        public const string MainGameSceneName = "MainGame";
        public const string BootstrapScenePath = AssetRootPath + "/Scenes/" + BootstrapSceneName + ".unity";
        public const string MainGameScenePath = AssetRootPath + "/Scenes/" + MainGameSceneName + ".unity";
        public const string GameRootName = "Game Root";
        public const string UiRootName = "UI Root";
        public const string ReadyStatusTextName = "Shell Ready Text";
        public const string ReadyStatusText = "Asset Manager MVP Ready";
        public const string DataRootPath = AssetRootPath + "/Data";
        public const string MvpRunStaticDataPath = DataRootPath + "/MvpRunStaticData.asset";
        public const string RunStatusTextName = "Run Status Text";
        public const string RunStatusPlaceholderText = "Run not started";
        public const string NextBusinessDayButtonName = "Next Business Day Button";
        public const string ContinueScheduleButtonName = "Continue Schedule Button";
        public const string QuarterSettlementPlaceholderPanelName = "Quarter Settlement Placeholder Panel";
        public const string QuarterSettlementPlaceholderTextName = "Quarter Settlement Placeholder Text";
        public const string VacationPlaceholderPanelName = "Vacation Placeholder Panel";
        public const string VacationPlaceholderTextName = "Vacation Placeholder Text";
        public const string FinalSettlementPlaceholderPanelName = "Final Settlement Placeholder Panel";
        public const string FinalSettlementPlaceholderTextName = "Final Settlement Placeholder Text";
        public const string MarketTapeSellImminentPanelName = "Market Tape Sell Imminent Panel";
        public const string MarketTapeSellImminentTextName = "Market Tape Sell Imminent Text";
        public const string MarketTapeCurrentMarketPanelName = "Market Tape Current Market Panel";
        public const string MarketTapeCurrentMarketTextName = "Market Tape Current Market Text";
        public const string MarketTapeUpcomingMarketPanelName = "Market Tape Upcoming Market Panel";
        public const string MarketTapeUpcomingMarketTextName = "Market Tape Upcoming Market Text";
        public const string MarketAreaMarketPanelName = "Market Area Market Panel";
        public const string MarketTapeSellImminentCardButtonPrefix = "Market Tape Sell Imminent Card Button ";
        public const string MarketTapeCurrentMarketCardButtonPrefix = "Market Tape Current Market Card Button ";
        public const string MarketTapeUpcomingMarketCardButtonPrefix = "Market Tape Upcoming Market Card Button ";
        public const string MarketTapeAdvanceButtonName = "Market Tape Advance Button";
        public const string MarketTapeRefreshButtonName = "Market Tape Refresh Button";
        public const string CardDetailPanelName = "Card Detail Panel";
        public const string CardDetailNameTextName = "Card Detail Name Text";
        public const string CardDetailDescriptionTextName = "Card Detail Description Text";
        public const string CardDetailCostTextName = "Card Detail Cost Text";
        public const string CardDetailManagementValueTextName = "Card Detail Management Value Text";
        public const string CardDetailIncomeTextName = "Card Detail Income Text";
        public const string CardDetailTagsTextName = "Card Detail Tags Text";
        public const string CardDetailRarityTextName = "Card Detail Rarity Text";
        public const string CardDetailCloseButtonName = "Card Detail Close Button";
        public const string CardDetailBuyButtonName = "Card Detail Buy Button";
        public const string CardDetailReserveButtonName = "Card Detail Reserve Button";

        public static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);

        public static ProjectShellRoots EnsureMainGameRoots()
        {
            var gameRoot = FindOrCreateRoot(GameRootName);
            var uiRoot = FindOrCreateRoot(UiRootName);

            var canvas = uiRoot.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = uiRoot.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = uiRoot.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = uiRoot.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.matchWidthOrHeight = 0.5f;

            if (uiRoot.GetComponent<GraphicRaycaster>() == null)
            {
                uiRoot.AddComponent<GraphicRaycaster>();
            }

            EnsureReadyStatusText(uiRoot.transform);
            EnsureRunStatusHud(uiRoot.transform);
            EnsureMarketTapeView(uiRoot.transform);
            EnsureCardDetailView(uiRoot.transform);
            EnsureMarketTapeDevControls(uiRoot.transform);
            EnsureRunProgressControls(uiRoot.transform);
            EnsureEventSystem();

            return new ProjectShellRoots(gameRoot, canvas);
        }

        private static GameObject FindOrCreateRoot(string name)
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.IsValid())
            {
                foreach (var root in scene.GetRootGameObjects())
                {
                    if (root.name == name)
                    {
                        return root;
                    }
                }
            }

            return new GameObject(name);
        }

        private static void EnsureReadyStatusText(Transform uiRoot)
        {
            var existing = uiRoot.Find(ReadyStatusTextName);
            var textObject = existing != null
                ? existing.gameObject
                : new GameObject(ReadyStatusTextName, typeof(RectTransform));

            textObject.transform.SetParent(uiRoot, false);

            var rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(640f, 80f);

            var text = textObject.GetComponent<Text>();
            if (text == null)
            {
                text = textObject.AddComponent<Text>();
            }

            text.text = ReadyStatusText;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 32;
            text.color = Color.white;

            if (text.font == null)
            {
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
        }

        public static RunStatusHud EnsureRunStatusHud(Transform uiRoot)
        {
            var statusText = EnsureRunStatusText(uiRoot);

            var hud = uiRoot.GetComponent<RunStatusHud>();
            if (hud == null)
            {
                hud = uiRoot.gameObject.AddComponent<RunStatusHud>();
            }

            hud.Bind(statusText);
            return hud;
        }

        public static RunProgressControls EnsureRunProgressControls(Transform uiRoot)
        {
            var nextBusinessDayButton = EnsureButton(
                uiRoot,
                NextBusinessDayButtonName,
                "다음 영업일",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -92f));

            var continueScheduleButton = EnsureButton(
                uiRoot,
                ContinueScheduleButtonName,
                "계속",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -88f));

            var quarterSettlement = EnsurePlaceholderPanel(
                uiRoot,
                QuarterSettlementPlaceholderPanelName,
                QuarterSettlementPlaceholderTextName);

            var vacation = EnsurePlaceholderPanel(
                uiRoot,
                VacationPlaceholderPanelName,
                VacationPlaceholderTextName);

            var finalSettlement = EnsurePlaceholderPanel(
                uiRoot,
                FinalSettlementPlaceholderPanelName,
                FinalSettlementPlaceholderTextName);

            var controls = uiRoot.GetComponent<RunProgressControls>();
            if (controls == null)
            {
                controls = uiRoot.gameObject.AddComponent<RunProgressControls>();
            }

            controls.Bind(
                nextBusinessDayButton,
                continueScheduleButton,
                quarterSettlement.Panel,
                quarterSettlement.Text,
                vacation.Panel,
                vacation.Text,
                finalSettlement.Panel,
                finalSettlement.Text);

            return controls;
        }

        public static MarketTapeView EnsureMarketTapeView(Transform uiRoot)
        {
            var marketPanel = EnsureMarketPanel(uiRoot);
            var sellImminentText = EnsureMarketTapeZoneText(
                marketPanel.transform,
                MarketTapeSellImminentPanelName,
                MarketTapeSellImminentTextName,
                new Vector2(-440f, -220f));

            var currentMarketText = EnsureMarketTapeZoneText(
                marketPanel.transform,
                MarketTapeCurrentMarketPanelName,
                MarketTapeCurrentMarketTextName,
                new Vector2(0f, -220f));

            var upcomingMarketText = EnsureMarketTapeZoneText(
                marketPanel.transform,
                MarketTapeUpcomingMarketPanelName,
                MarketTapeUpcomingMarketTextName,
                new Vector2(440f, -220f));

            var sellImminentButtons = EnsureMarketTapeCardButtons(
                sellImminentText.transform.parent,
                MarketTapeSellImminentCardButtonPrefix);
            var currentMarketButtons = EnsureMarketTapeCardButtons(
                currentMarketText.transform.parent,
                MarketTapeCurrentMarketCardButtonPrefix);
            var upcomingMarketButtons = EnsureMarketTapeCardButtons(
                upcomingMarketText.transform.parent,
                MarketTapeUpcomingMarketCardButtonPrefix);

            var view = uiRoot.GetComponent<MarketTapeView>();
            if (view == null)
            {
                view = uiRoot.gameObject.AddComponent<MarketTapeView>();
            }

            view.Bind(
                marketPanel,
                sellImminentText,
                currentMarketText,
                upcomingMarketText,
                sellImminentButtons,
                currentMarketButtons,
                upcomingMarketButtons);
            return view;
        }

        public static CardDetailView EnsureCardDetailView(Transform uiRoot)
        {
            var panel = EnsurePanel(
                uiRoot,
                CardDetailPanelName,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -220f),
                new Vector2(1280f, 300f),
                new Color(0.07f, 0.09f, 0.11f, 0.96f));

            var nameText = EnsurePanelText(
                panel.transform,
                CardDetailNameTextName,
                new Vector2(24f, -24f),
                new Vector2(520f, 44f),
                26,
                TextAnchor.MiddleLeft);
            var descriptionText = EnsurePanelText(
                panel.transform,
                CardDetailDescriptionTextName,
                new Vector2(24f, -76f),
                new Vector2(520f, 76f),
                18,
                TextAnchor.UpperLeft);
            var costText = EnsurePanelText(
                panel.transform,
                CardDetailCostTextName,
                new Vector2(584f, -40f),
                new Vector2(360f, 36f),
                18,
                TextAnchor.MiddleLeft);
            var managementValueText = EnsurePanelText(
                panel.transform,
                CardDetailManagementValueTextName,
                new Vector2(584f, -86f),
                new Vector2(260f, 36f),
                18,
                TextAnchor.MiddleLeft);
            var incomeText = EnsurePanelText(
                panel.transform,
                CardDetailIncomeTextName,
                new Vector2(584f, -132f),
                new Vector2(260f, 36f),
                18,
                TextAnchor.MiddleLeft);
            var tagsText = EnsurePanelText(
                panel.transform,
                CardDetailTagsTextName,
                new Vector2(24f, -176f),
                new Vector2(920f, 36f),
                18,
                TextAnchor.MiddleLeft);
            var rarityText = EnsurePanelText(
                panel.transform,
                CardDetailRarityTextName,
                new Vector2(584f, -178f),
                new Vector2(260f, 36f),
                18,
                TextAnchor.MiddleLeft);

            var closeButton = EnsureButton(
                panel.transform,
                CardDetailCloseButtonName,
                "닫기",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-132f, -48f),
                new Vector2(160f, 48f));
            var buyButton = EnsureButton(
                panel.transform,
                CardDetailBuyButtonName,
                "매수",
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-312f, 36f),
                new Vector2(160f, 48f));
            var reserveButton = EnsureButton(
                panel.transform,
                CardDetailReserveButtonName,
                "예약",
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-132f, 36f),
                new Vector2(160f, 48f));

            var view = uiRoot.GetComponent<CardDetailView>();
            if (view == null)
            {
                view = uiRoot.gameObject.AddComponent<CardDetailView>();
            }

            view.Bind(
                panel,
                nameText,
                descriptionText,
                costText,
                managementValueText,
                incomeText,
                tagsText,
                rarityText,
                closeButton,
                buyButton,
                reserveButton);
            panel.SetActive(false);
            return view;
        }

        public static MarketTapeDevControls EnsureMarketTapeDevControls(Transform uiRoot)
        {
            var advanceButton = EnsureButton(
                uiRoot,
                MarketTapeAdvanceButtonName,
                "시장 테이프 진행",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -160f));

            var refreshButton = EnsureButton(
                uiRoot,
                MarketTapeRefreshButtonName,
                "시장 테이프 갱신",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -228f));

            var controls = uiRoot.GetComponent<MarketTapeDevControls>();
            if (controls == null)
            {
                controls = uiRoot.gameObject.AddComponent<MarketTapeDevControls>();
            }

            controls.Bind(advanceButton, refreshButton);
            return controls;
        }

        private static GameObject EnsureMarketPanel(Transform uiRoot)
        {
            return EnsurePanel(
                uiRoot,
                MarketAreaMarketPanelName,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                Vector2.zero,
                new Vector2(1440f, 560f),
                new Color(0f, 0f, 0f, 0f));
        }

        private static GameObject EnsurePanel(
            Transform parent,
            string panelName,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta,
            Color color)
        {
            var existing = parent.Find(panelName);
            var panel = existing != null
                ? existing.gameObject
                : new GameObject(panelName, typeof(RectTransform));

            panel.transform.SetParent(parent, false);

            var rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;

            var image = panel.GetComponent<Image>();
            if (image == null)
            {
                image = panel.AddComponent<Image>();
            }

            image.color = color;
            image.raycastTarget = color.a > 0f;
            return panel;
        }

        private static IReadOnlyList<Button> EnsureMarketTapeCardButtons(Transform zonePanel, string namePrefix)
        {
            var buttons = new List<Button>();
            for (var i = 0; i < 3; i++)
            {
                var button = EnsureButton(
                    zonePanel,
                    namePrefix + (i + 1),
                    string.Empty,
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(0f, -82f - (i * 64f)),
                    new Vector2(360f, 52f));

                var text = button.GetComponentInChildren<Text>();
                text.alignment = TextAnchor.MiddleLeft;
                text.fontSize = 16;
                buttons.Add(button);
            }

            return buttons;
        }

        private static Text EnsureRunStatusText(Transform uiRoot)
        {
            var existing = uiRoot.Find(RunStatusTextName);
            var textObject = existing != null
                ? existing.gameObject
                : new GameObject(RunStatusTextName, typeof(RectTransform));

            textObject.transform.SetParent(uiRoot, false);

            var rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -24f);
            rectTransform.sizeDelta = new Vector2(-64f, 48f);

            var text = textObject.GetComponent<Text>();
            if (text == null)
            {
                text = textObject.AddComponent<Text>();
            }

            text.text = RunStatusPlaceholderText;
            text.alignment = TextAnchor.MiddleLeft;
            text.fontSize = 24;
            text.color = Color.white;

            if (text.font == null)
            {
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            return text;
        }

        private static Text EnsureMarketTapeZoneText(
            Transform uiRoot,
            string panelName,
            string textName,
            Vector2 anchoredPosition)
        {
            var existing = uiRoot.Find(panelName);
            var panel = existing != null
                ? existing.gameObject
                : new GameObject(panelName, typeof(RectTransform));

            panel.transform.SetParent(uiRoot, false);

            var rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(400f, 280f);

            var image = panel.GetComponent<Image>();
            if (image == null)
            {
                image = panel.AddComponent<Image>();
            }

            image.color = new Color(0.08f, 0.11f, 0.14f, 0.92f);

            var text = EnsureChildText(panel.transform, textName, string.Empty);
            var textTransform = text.GetComponent<RectTransform>();
            textTransform.anchorMin = new Vector2(0f, 1f);
            textTransform.anchorMax = new Vector2(1f, 1f);
            textTransform.pivot = new Vector2(0.5f, 1f);
            textTransform.anchoredPosition = new Vector2(0f, -16f);
            textTransform.sizeDelta = new Vector2(-32f, 40f);
            text.alignment = TextAnchor.MiddleLeft;
            text.fontSize = 20;
            text.color = Color.white;

            return text;
        }

        private static Button EnsureButton(
            Transform uiRoot,
            string name,
            string label,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2? sizeDelta = null)
        {
            var existing = uiRoot.Find(name);
            var buttonObject = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform));

            buttonObject.transform.SetParent(uiRoot, false);

            var rectTransform = buttonObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta ?? new Vector2(240f, 56f);

            var image = buttonObject.GetComponent<Image>();
            if (image == null)
            {
                image = buttonObject.AddComponent<Image>();
            }

            image.color = new Color(0.11f, 0.16f, 0.20f, 0.94f);

            var button = buttonObject.GetComponent<Button>();
            if (button == null)
            {
                button = buttonObject.AddComponent<Button>();
            }

            var text = EnsureChildText(buttonObject.transform, name + " Text", label);
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 22;
            text.color = Color.white;

            return button;
        }

        private static Text EnsurePanelText(
            Transform parent,
            string name,
            Vector2 anchoredPosition,
            Vector2 sizeDelta,
            int fontSize,
            TextAnchor alignment)
        {
            var text = EnsureChildText(parent, name, string.Empty);
            var rectTransform = text.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
            text.alignment = alignment;
            text.fontSize = fontSize;
            text.color = Color.white;
            return text;
        }

        private static PlaceholderPanel EnsurePlaceholderPanel(Transform uiRoot, string panelName, string textName)
        {
            var existing = uiRoot.Find(panelName);
            var panel = existing != null
                ? existing.gameObject
                : new GameObject(panelName, typeof(RectTransform));

            panel.transform.SetParent(uiRoot, false);

            var rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(520f, 180f);

            var image = panel.GetComponent<Image>();
            if (image == null)
            {
                image = panel.AddComponent<Image>();
            }

            image.color = new Color(0.06f, 0.08f, 0.10f, 0.92f);

            var text = EnsureChildText(panel.transform, textName, string.Empty);
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 26;
            text.color = Color.white;

            panel.SetActive(false);
            return new PlaceholderPanel(panel, text);
        }

        private static Text EnsureChildText(Transform parent, string name, string value)
        {
            var existing = parent.Find(name);
            var textObject = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform));

            textObject.transform.SetParent(parent, false);

            var rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            var text = textObject.GetComponent<Text>();
            if (text == null)
            {
                text = textObject.AddComponent<Text>();
            }

            text.text = value;
            text.raycastTarget = false;

            if (text.font == null)
            {
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            return text;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private readonly struct PlaceholderPanel
        {
            public PlaceholderPanel(GameObject panel, Text text)
            {
                Panel = panel;
                Text = text;
            }

            public GameObject Panel { get; }
            public Text Text { get; }
        }
    }
}
