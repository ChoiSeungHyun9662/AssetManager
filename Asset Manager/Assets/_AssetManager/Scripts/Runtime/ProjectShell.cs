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
        public const string RunFailurePlaceholderPanelName = "Run Failure Placeholder Panel";
        public const string RunFailurePlaceholderTextName = "Run Failure Placeholder Text";
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
        public const string ReservationPanelName = "Reservation Panel";
        public const string ReservationTitleTextName = "Reservation Title Text";
        public const string ReservationCardButtonPrefix = "Reservation Card Button ";
        public const string MarketTapeAdvanceButtonName = "Market Tape Advance Button";
        public const string MarketTapeRefreshButtonName = "Market Tape Refresh Button";
        public const string CentralBankButtonName = "Central Bank Button";
        public const string LiquidityActionPanelName = "Liquidity Action Panel";
        public const string LiquidityActionSelectionTextName = "Liquidity Action Selection Text";
        public const string LiquidityActionMessageTextName = "Liquidity Action Message Text";
        public const string LiquidityActionCloseButtonName = "Liquidity Action Close Button";
        public const string LiquidityActionCashButtonName = "Liquidity Action Cash Button";
        public const string LiquidityActionResearchButtonName = "Liquidity Action Research Button";
        public const string LiquidityActionCreditButtonName = "Liquidity Action Credit Button";
        public const string LiquidityActionCommodityButtonName = "Liquidity Action Commodity Button";
        public const string CardDetailPanelName = "Card Detail Panel";
        public const string CardDetailNameTextName = "Card Detail Name Text";
        public const string CardDetailDescriptionTextName = "Card Detail Description Text";
        public const string CardDetailCostTextName = "Card Detail Cost Text";
        public const string CardDetailManagementValueTextName = "Card Detail Management Value Text";
        public const string CardDetailIncomeTextName = "Card Detail Income Text";
        public const string CardDetailTagsTextName = "Card Detail Tags Text";
        public const string CardDetailRarityTextName = "Card Detail Rarity Text";
        public const string CardDetailPaymentSlotsTextName = "Card Detail Payment Slots Text";
        public const string CardDetailFinalCashCostTextName = "Card Detail Final Cash Cost Text";
        public const string CardDetailPaymentSlotButtonPrefix = "Card Detail Payment Slot Button ";
        public const string CardDetailPlaceResearchButtonName = "Card Detail Place Research Button";
        public const string CardDetailPlaceCreditButtonName = "Card Detail Place Credit Button";
        public const string CardDetailPlaceCommodityButtonName = "Card Detail Place Commodity Button";
        public const string CardDetailPlaceDealButtonName = "Card Detail Place Deal Button";
        public const string CardDetailCloseButtonName = "Card Detail Close Button";
        public const string CardDetailBuyButtonName = "Card Detail Buy Button";
        public const string CardDetailReserveButtonName = "Card Detail Reserve Button";
        public const string ResourceHudPanelName = "Resource Hud Panel";
        public const string ResourceHudTextName = "Resource Hud Text";
        public const string ResourceMessageTextName = "Resource Message Text";
        public const string PortfolioSummaryPanelName = "Portfolio Summary Panel";
        public const string PortfolioSummaryTextName = "Portfolio Summary Text";
        public const string PortfolioOwnedCardsTextName = "Portfolio Owned Cards Text";
        public const string ResourceDevFundingCashButtonName = "Resource Dev Funding Cash Button";
        public const string ResourceDevEarnedCashButtonName = "Resource Dev Earned Cash Button";
        public const string ResourceDevResearchButtonName = "Resource Dev Research Button";
        public const string ResourceDevCreditButtonName = "Resource Dev Credit Button";
        public const string ResourceDevCommodityButtonName = "Resource Dev Commodity Button";
        public const string ResourceDevDealButtonName = "Resource Dev Deal Button";

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
            EnsureResourceHud(uiRoot.transform);
            EnsurePortfolioSummaryView(uiRoot.transform);
            EnsureMarketTapeView(uiRoot.transform);
            EnsureReservationView(uiRoot.transform);
            EnsureLiquidityActionView(uiRoot.transform);
            EnsureCardDetailView(uiRoot.transform);
            EnsureMarketTapeDevControls(uiRoot.transform);
            EnsureResourceDevControls(uiRoot.transform);
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
            var runFailure = EnsurePlaceholderPanel(
                uiRoot,
                RunFailurePlaceholderPanelName,
                RunFailurePlaceholderTextName);

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
                finalSettlement.Text,
                runFailure.Panel,
                runFailure.Text);

            return controls;
        }

        public static ResourceHud EnsureResourceHud(Transform uiRoot)
        {
            var panel = EnsurePanel(
                uiRoot,
                ResourceHudPanelName,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(32f, -84f),
                new Vector2(820f, 92f),
                new Color(0.06f, 0.08f, 0.10f, 0.92f));

            var resourceText = EnsurePanelText(
                panel.transform,
                ResourceHudTextName,
                new Vector2(18f, -12f),
                new Vector2(784f, 36f),
                20,
                TextAnchor.MiddleLeft);
            var messageText = EnsurePanelText(
                panel.transform,
                ResourceMessageTextName,
                new Vector2(18f, -52f),
                new Vector2(784f, 30f),
                17,
                TextAnchor.MiddleLeft);

            var hud = uiRoot.GetComponent<ResourceHud>();
            if (hud == null)
            {
                hud = uiRoot.gameObject.AddComponent<ResourceHud>();
            }

            hud.Bind(panel, resourceText, messageText);
            return hud;
        }

        public static PortfolioSummaryView EnsurePortfolioSummaryView(Transform uiRoot)
        {
            var panel = EnsurePanel(
                uiRoot,
                PortfolioSummaryPanelName,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(32f, -736f),
                new Vector2(820f, 156f),
                new Color(0.06f, 0.08f, 0.10f, 0.92f));

            var summaryText = EnsurePanelText(
                panel.transform,
                PortfolioSummaryTextName,
                new Vector2(18f, -14f),
                new Vector2(784f, 34f),
                20,
                TextAnchor.MiddleLeft);
            var ownedCardsText = EnsurePanelText(
                panel.transform,
                PortfolioOwnedCardsTextName,
                new Vector2(18f, -54f),
                new Vector2(784f, 84f),
                17,
                TextAnchor.UpperLeft);

            var view = uiRoot.GetComponent<PortfolioSummaryView>();
            if (view == null)
            {
                view = uiRoot.gameObject.AddComponent<PortfolioSummaryView>();
            }

            view.Bind(panel, summaryText, ownedCardsText);
            return view;
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

            ApplyMarketTapeLayout(
                marketPanel,
                sellImminentText,
                currentMarketText,
                upcomingMarketText,
                sellImminentButtons,
                currentMarketButtons,
                upcomingMarketButtons);

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
                new Vector2(1280f, 360f),
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
            var paymentSlotsText = EnsurePanelText(
                panel.transform,
                CardDetailPaymentSlotsTextName,
                new Vector2(584f, -220f),
                new Vector2(520f, 32f),
                16,
                TextAnchor.MiddleLeft);
            var finalCashCostText = EnsurePanelText(
                panel.transform,
                CardDetailFinalCashCostTextName,
                new Vector2(584f, -254f),
                new Vector2(260f, 32f),
                18,
                TextAnchor.MiddleLeft);
            var paymentSlotButtons = EnsureCardDetailPaymentSlotButtons(panel.transform);
            var placeResearchButton = EnsureButton(
                panel.transform,
                CardDetailPlaceResearchButtonName,
                "리서치",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-492f, -112f),
                new Vector2(112f, 40f));
            var placeCreditButton = EnsureButton(
                panel.transform,
                CardDetailPlaceCreditButtonName,
                "신용",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-372f, -112f),
                new Vector2(112f, 40f));
            var placeCommodityButton = EnsureButton(
                panel.transform,
                CardDetailPlaceCommodityButtonName,
                "원자재",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-252f, -112f),
                new Vector2(112f, 40f));
            var placeDealButton = EnsureButton(
                panel.transform,
                CardDetailPlaceDealButtonName,
                "딜",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-132f, -112f),
                new Vector2(112f, 40f));

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

            ApplyCardDetailLayout(
                panel,
                nameText,
                descriptionText,
                costText,
                managementValueText,
                incomeText,
                tagsText,
                rarityText,
                paymentSlotsText,
                finalCashCostText,
                paymentSlotButtons,
                placeResearchButton,
                placeCreditButton,
                placeCommodityButton,
                placeDealButton,
                closeButton,
                buyButton,
                reserveButton);

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
                paymentSlotsText,
                finalCashCostText,
                paymentSlotButtons,
                placeResearchButton,
                placeCreditButton,
                placeCommodityButton,
                placeDealButton,
                closeButton,
                buyButton,
                reserveButton);
            panel.SetActive(false);
            return view;
        }

        public static ReservationView EnsureReservationView(Transform uiRoot)
        {
            var marketPanel = EnsureMarketPanel(uiRoot);
            var panel = EnsurePanel(
                marketPanel.transform,
                ReservationPanelName,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -84f),
                new Vector2(820f, 112f),
                new Color(0.08f, 0.11f, 0.14f, 0.92f));

            var title = EnsurePanelText(
                panel.transform,
                ReservationTitleTextName,
                new Vector2(18f, -12f),
                new Vector2(160f, 32f),
                18,
                TextAnchor.MiddleLeft);

            var buttons = EnsureReservationCardButtons(panel.transform);

            ApplyReservationLayout(panel, title, buttons);

            var view = uiRoot.GetComponent<ReservationView>();
            if (view == null)
            {
                view = uiRoot.gameObject.AddComponent<ReservationView>();
            }

            view.Bind(panel, title, buttons);
            return view;
        }

        public static LiquidityActionView EnsureLiquidityActionView(Transform uiRoot)
        {
            var marketPanel = EnsureMarketPanel(uiRoot);
            var centralBankButton = EnsureButton(
                marketPanel.transform,
                CentralBankButtonName,
                "중앙 은행",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -520f),
                new Vector2(240f, 52f));

            var panel = EnsurePanel(
                uiRoot,
                LiquidityActionPanelName,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -220f),
                new Vector2(960f, 300f),
                new Color(0.07f, 0.09f, 0.11f, 0.96f));

            var selectionText = EnsurePanelText(
                panel.transform,
                LiquidityActionSelectionTextName,
                new Vector2(24f, -24f),
                new Vector2(640f, 44f),
                24,
                TextAnchor.MiddleLeft);
            var messageText = EnsurePanelText(
                panel.transform,
                LiquidityActionMessageTextName,
                new Vector2(24f, -76f),
                new Vector2(640f, 36f),
                18,
                TextAnchor.MiddleLeft);
            var cashButton = EnsureButton(
                panel.transform,
                LiquidityActionCashButtonName,
                "현금",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(24f, -140f),
                new Vector2(144f, 48f));
            var researchButton = EnsureButton(
                panel.transform,
                LiquidityActionResearchButtonName,
                "리서치",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(184f, -140f),
                new Vector2(144f, 48f));
            var creditButton = EnsureButton(
                panel.transform,
                LiquidityActionCreditButtonName,
                "신용",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(344f, -140f),
                new Vector2(144f, 48f));
            var commodityButton = EnsureButton(
                panel.transform,
                LiquidityActionCommodityButtonName,
                "원자재",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(504f, -140f),
                new Vector2(144f, 48f));
            var closeButton = EnsureButton(
                panel.transform,
                LiquidityActionCloseButtonName,
                "닫기",
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-112f, 32f),
                new Vector2(144f, 48f));

            var view = uiRoot.GetComponent<LiquidityActionView>();
            if (view == null)
            {
                view = uiRoot.gameObject.AddComponent<LiquidityActionView>();
            }

            view.Bind(
                centralBankButton,
                panel,
                selectionText,
                messageText,
                closeButton,
                cashButton,
                researchButton,
                creditButton,
                commodityButton);
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

        public static ResourceDevControls EnsureResourceDevControls(Transform uiRoot)
        {
            var fundingCashButton = EnsureButton(
                uiRoot,
                ResourceDevFundingCashButtonName,
                "조달 현금 +1",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -296f),
                new Vector2(240f, 42f));
            var earnedCashButton = EnsureButton(
                uiRoot,
                ResourceDevEarnedCashButtonName,
                "운용 수익 +1",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -346f),
                new Vector2(240f, 42f));
            var researchButton = EnsureButton(
                uiRoot,
                ResourceDevResearchButtonName,
                "리서치 +1",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -396f),
                new Vector2(240f, 42f));
            var creditButton = EnsureButton(
                uiRoot,
                ResourceDevCreditButtonName,
                "신용 +1",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -446f),
                new Vector2(240f, 42f));
            var commodityButton = EnsureButton(
                uiRoot,
                ResourceDevCommodityButtonName,
                "원자재 +1",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -496f),
                new Vector2(240f, 42f));
            var dealButton = EnsureButton(
                uiRoot,
                ResourceDevDealButtonName,
                "딜 +1",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -546f),
                new Vector2(240f, 42f));

            var controls = uiRoot.GetComponent<ResourceDevControls>();
            if (controls == null)
            {
                controls = uiRoot.gameObject.AddComponent<ResourceDevControls>();
            }

            controls.Bind(
                fundingCashButton,
                earnedCashButton,
                researchButton,
                creditButton,
                commodityButton,
                dealButton);
            return controls;
        }

        private static void ApplyMarketTapeLayout(
            GameObject marketPanel,
            Text sellImminentText,
            Text currentMarketText,
            Text upcomingMarketText,
            IReadOnlyList<Button> sellImminentButtons,
            IReadOnlyList<Button> currentMarketButtons,
            IReadOnlyList<Button> upcomingMarketButtons)
        {
            SetRect(
                marketPanel,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                Vector2.zero,
                new Vector2(1500f, 720f));

            ApplyMarketTapeZoneLayout(sellImminentText, new Vector2(-470f, -220f));
            ApplyMarketTapeZoneLayout(currentMarketText, new Vector2(0f, -220f));
            ApplyMarketTapeZoneLayout(upcomingMarketText, new Vector2(470f, -220f));
            ApplyMarketTapeCardLayout(sellImminentButtons);
            ApplyMarketTapeCardLayout(currentMarketButtons);
            ApplyMarketTapeCardLayout(upcomingMarketButtons);
        }

        private static void ApplyMarketTapeZoneLayout(Text title, Vector2 anchoredPosition)
        {
            if (title == null || title.transform.parent == null)
            {
                return;
            }

            SetRect(
                title.transform.parent.gameObject,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                anchoredPosition,
                new Vector2(420f, 500f));
            SetTextRect(title, new Vector2(18f, -14f), new Vector2(384f, 54f), 19, TextAnchor.MiddleLeft);
        }

        private static void ApplyMarketTapeCardLayout(IReadOnlyList<Button> buttons)
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                if (button == null)
                {
                    continue;
                }

                SetRect(
                    button.gameObject,
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(0f, -92f - (i * 140f)),
                    new Vector2(372f, 124f));
                SetButtonTextInset(button, new Vector2(14f, 10f), new Vector2(14f, 10f), 16, TextAnchor.UpperLeft);
            }
        }

        private static void ApplyReservationLayout(
            GameObject panel,
            Text title,
            IReadOnlyList<Button> buttons)
        {
            SetRect(
                panel,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -84f),
                new Vector2(900f, 126f));
            SetTextRect(title, new Vector2(18f, -16f), new Vector2(150f, 34f), 18, TextAnchor.MiddleLeft);

            for (var i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                if (button == null)
                {
                    continue;
                }

                SetRect(
                    button.gameObject,
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(180f + (i * 230f), -18f),
                    new Vector2(212f, 90f));
                SetButtonTextInset(button, new Vector2(10f, 8f), new Vector2(10f, 8f), 14, TextAnchor.UpperLeft);
            }
        }

        private static void ApplyCardDetailLayout(
            GameObject panel,
            Text nameText,
            Text descriptionText,
            Text costText,
            Text managementValueText,
            Text incomeText,
            Text tagsText,
            Text rarityText,
            Text paymentSlotsText,
            Text finalCashCostText,
            IReadOnlyList<Button> paymentSlotButtons,
            Button placeResearchButton,
            Button placeCreditButton,
            Button placeCommodityButton,
            Button placeDealButton,
            Button closeButton,
            Button buyButton,
            Button reserveButton)
        {
            SetRect(
                panel,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -160f),
                new Vector2(1280f, 520f));

            SetTextRect(nameText, new Vector2(28f, -24f), new Vector2(500f, 44f), 30, TextAnchor.MiddleLeft);
            SetTextRect(rarityText, new Vector2(28f, -76f), new Vector2(180f, 32f), 17, TextAnchor.MiddleLeft);
            SetTextRect(tagsText, new Vector2(220f, -76f), new Vector2(300f, 32f), 17, TextAnchor.MiddleLeft);
            SetTextRect(costText, new Vector2(28f, -126f), new Vector2(500f, 84f), 20, TextAnchor.UpperLeft);
            SetTextRect(descriptionText, new Vector2(28f, -236f), new Vector2(500f, 176f), 17, TextAnchor.UpperLeft);
            SetTextRect(managementValueText, new Vector2(560f, -48f), new Vector2(270f, 88f), 26, TextAnchor.MiddleCenter);
            SetTextRect(incomeText, new Vector2(560f, -156f), new Vector2(270f, 88f), 26, TextAnchor.MiddleCenter);
            SetTextRect(finalCashCostText, new Vector2(560f, -274f), new Vector2(270f, 92f), 19, TextAnchor.MiddleCenter);
            SetTextRect(paymentSlotsText, new Vector2(880f, -48f), new Vector2(360f, 72f), 17, TextAnchor.UpperLeft);

            for (var i = 0; i < paymentSlotButtons.Count; i++)
            {
                var button = paymentSlotButtons[i];
                if (button == null)
                {
                    continue;
                }

                SetRect(
                    button.gameObject,
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(880f, -136f - (i * 46f)),
                    new Vector2(360f, 36f));
                SetButtonTextInset(button, new Vector2(10f, 4f), new Vector2(10f, 4f), 15, TextAnchor.MiddleLeft);
            }

            SetDetailActionButton(placeResearchButton, new Vector2(880f, -334f), "리서치");
            SetDetailActionButton(placeCreditButton, new Vector2(1004f, -334f), "신용");
            SetDetailActionButton(placeCommodityButton, new Vector2(1128f, -334f), "원자재");
            SetDetailActionButton(placeDealButton, new Vector2(880f, -388f), "딜");
            SetRect(closeButton != null ? closeButton.gameObject : null, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-110f, -38f), new Vector2(150f, 46f));
            SetButtonTextInset(closeButton, new Vector2(8f, 4f), new Vector2(8f, 4f), 18, TextAnchor.MiddleCenter);
            SetButtonLabel(closeButton, "닫기");
            SetRect(buyButton != null ? buyButton.gameObject : null, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-286f, 34f), new Vector2(170f, 52f));
            SetButtonTextInset(buyButton, new Vector2(8f, 4f), new Vector2(8f, 4f), 20, TextAnchor.MiddleCenter);
            SetButtonLabel(buyButton, "매수");
            SetRect(reserveButton != null ? reserveButton.gameObject : null, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-102f, 34f), new Vector2(170f, 52f));
            SetButtonTextInset(reserveButton, new Vector2(8f, 4f), new Vector2(8f, 4f), 20, TextAnchor.MiddleCenter);
            SetButtonLabel(reserveButton, "예약");
        }

        private static void SetDetailActionButton(Button button, Vector2 anchoredPosition, string label)
        {
            SetRect(
                button != null ? button.gameObject : null,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                anchoredPosition,
                new Vector2(112f, 42f));
            SetButtonTextInset(button, new Vector2(8f, 4f), new Vector2(8f, 4f), 16, TextAnchor.MiddleCenter);
            SetButtonLabel(button, label);
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
                new Vector2(1500f, 720f),
                new Color(0f, 0f, 0f, 0f));
        }

        private static void SetRect(
            GameObject gameObject,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            if (gameObject == null)
            {
                return;
            }

            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                return;
            }

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
        }

        private static void SetTextRect(
            Text text,
            Vector2 anchoredPosition,
            Vector2 sizeDelta,
            int fontSize,
            TextAnchor alignment)
        {
            if (text == null)
            {
                return;
            }

            SetRect(
                text.gameObject,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                anchoredPosition,
                sizeDelta);
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(10, fontSize - 6);
            text.resizeTextMaxSize = fontSize;
        }

        private static void SetButtonTextInset(
            Button button,
            Vector2 lowerLeftInset,
            Vector2 upperRightInset,
            int fontSize,
            TextAnchor alignment)
        {
            if (button == null)
            {
                return;
            }

            var text = button.GetComponentInChildren<Text>();
            if (text == null)
            {
                return;
            }

            var rectTransform = text.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = lowerLeftInset;
            rectTransform.offsetMax = new Vector2(-upperRightInset.x, -upperRightInset.y);
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(10, fontSize - 5);
            text.resizeTextMaxSize = fontSize;
        }

        private static void SetButtonLabel(Button button, string label)
        {
            if (button == null)
            {
                return;
            }

            var text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = label;
            }
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

        private static IReadOnlyList<Button> EnsureCardDetailPaymentSlotButtons(Transform panel)
        {
            var buttons = new List<Button>();
            for (var i = 0; i < 4; i++)
            {
                var button = EnsureButton(
                    panel,
                    CardDetailPaymentSlotButtonPrefix + (i + 1),
                    string.Empty,
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(24f + (i * 132f), -254f),
                    new Vector2(124f, 36f));

                var text = button.GetComponentInChildren<Text>();
                text.fontSize = 15;
                buttons.Add(button);
            }

            return buttons;
        }

        private static IReadOnlyList<Button> EnsureReservationCardButtons(Transform panel)
        {
            var buttons = new List<Button>();
            for (var i = 0; i < 3; i++)
            {
                var button = EnsureButton(
                    panel,
                    ReservationCardButtonPrefix + (i + 1),
                    "비어 있음",
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(188f + (i * 204f), -18f),
                    new Vector2(188f, 76f));

                var text = button.GetComponentInChildren<Text>();
                text.alignment = TextAnchor.MiddleLeft;
                text.fontSize = 14;
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
            rectTransform.sizeDelta = new Vector2(-64f, 58f);

            var text = textObject.GetComponent<Text>();
            if (text == null)
            {
                text = textObject.AddComponent<Text>();
            }

            text.text = RunStatusPlaceholderText;
            text.alignment = TextAnchor.MiddleLeft;
            text.fontSize = 23;
            text.color = Color.white;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 16;
            text.resizeTextMaxSize = 23;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;

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
