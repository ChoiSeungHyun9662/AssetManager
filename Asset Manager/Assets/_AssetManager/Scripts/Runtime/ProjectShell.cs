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
        public const string MarketTapeCurrentMarketReserveButtonPrefix = "Market Tape Current Market Reserve Button ";
        public const string MarketTapeCurrentMarketUnreserveButtonPrefix = "Market Tape Current Market Unreserve Button ";
        public const string MarketTapeUpcomingMarketCardButtonPrefix = "Market Tape Upcoming Market Card Button ";
        public const string MarketCardHoverPanelName = "Market Card Hover Panel";
        public const string MarketCardHoverTextName = "Market Card Hover Text";
        public const string PurchaseConfirmationPanelName = "Purchase Confirmation Panel";
        public const string PurchaseConfirmationCardTextName = "Purchase Confirmation Card Text";
        public const string PurchaseConfirmationConfirmButtonName = "Purchase Confirmation Confirm Button";
        public const string PurchaseConfirmationBackButtonName = "Purchase Confirmation Back Button";
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
        public const string CardDetailPaymentPotBackgroundName = "Card Detail Payment Pot Background";
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
        public const string ResourceHudCashTextName = "Resource Hud Cash Text";
        public const string ResourceHudResearchTextName = "Resource Hud Research Text";
        public const string ResourceHudCreditTextName = "Resource Hud Credit Text";
        public const string ResourceHudCommodityTextName = "Resource Hud Commodity Text";
        public const string ResourceHudDealTextName = "Resource Hud Deal Text";
        public const string PortfolioSummaryPanelName = "Portfolio Summary Panel";
        public const string PortfolioSummaryTextName = "Portfolio Summary Text";
        public const string PortfolioOwnedCardsTextName = "Portfolio Owned Cards Text";
        public const string OwnedStockCardPrefix = "Owned Stock Card ";
        public const string OwnedStockCardButtonSuffix = " Card Button";
        public const string OwnedStockCardTextSuffix = " Text";
        public const string OwnedStockCardSellButtonSuffix = " Sell Button";
        public const string ResourceDevFundingCashButtonName = "Resource Dev Funding Cash Button";
        public const string ResourceDevEarnedCashButtonName = "Resource Dev Earned Cash Button";
        public const string ResourceDevResearchButtonName = "Resource Dev Research Button";
        public const string ResourceDevCreditButtonName = "Resource Dev Credit Button";
        public const string ResourceDevCommodityButtonName = "Resource Dev Commodity Button";
        public const string ResourceDevDealButtonName = "Resource Dev Deal Button";
        private const string LegacyReservationPanelName = "Reservation Panel";

        public static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);
        private static readonly HashSet<GameObject> CreatedLayoutObjects = new HashSet<GameObject>();

        public static ProjectShellRoots EnsureMainGameRoots()
        {
            CreatedLayoutObjects.Clear();

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
            EnsurePurchaseConfirmationView(uiRoot.transform);
            RemoveLegacyDisconnectedUiObjects(uiRoot.transform);
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

        private static GameObject CreateLayoutObject(string name)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            CreatedLayoutObjects.Add(gameObject);
            return gameObject;
        }

        private static bool WasCreatedThisPass(GameObject gameObject)
        {
            return gameObject != null && CreatedLayoutObjects.Contains(gameObject);
        }

        private static void EnsureReadyStatusText(Transform uiRoot)
        {
            var existing = uiRoot.Find(ReadyStatusTextName);
            var textObject = existing != null
                ? existing.gameObject
                : CreateLayoutObject(ReadyStatusTextName);

            textObject.transform.SetParent(uiRoot, false);

            var rectTransform = textObject.GetComponent<RectTransform>();
            if (WasCreatedThisPass(textObject))
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(640f, 80f);
            }

            var text = textObject.GetComponent<Text>();
            if (text == null)
            {
                text = textObject.AddComponent<Text>();
            }

            text.text = ReadyStatusText;
            if (WasCreatedThisPass(textObject))
            {
                text.alignment = TextAnchor.MiddleCenter;
                text.fontSize = 32;
                text.color = Color.white;
            }

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
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 24f),
                new Vector2(1500f, 136f),
                new Color(0.06f, 0.08f, 0.10f, 0.92f));

            var resourceText = EnsurePanelText(
                panel.transform,
                ResourceHudTextName,
                new Vector2(18f, -10f),
                new Vector2(760f, 28f),
                18,
                TextAnchor.MiddleLeft);
            var messageText = EnsurePanelText(
                panel.transform,
                ResourceMessageTextName,
                new Vector2(830f, -10f),
                new Vector2(630f, 28f),
                16,
                TextAnchor.MiddleLeft);
            var cashText = EnsurePanelText(
                panel.transform,
                ResourceHudCashTextName,
                new Vector2(18f, -46f),
                new Vector2(250f, 74f),
                18,
                TextAnchor.UpperLeft);
            var researchText = EnsurePanelText(
                panel.transform,
                ResourceHudResearchTextName,
                new Vector2(288f, -46f),
                new Vector2(250f, 74f),
                34,
                TextAnchor.UpperLeft);
            var creditText = EnsurePanelText(
                panel.transform,
                ResourceHudCreditTextName,
                new Vector2(558f, -46f),
                new Vector2(250f, 74f),
                34,
                TextAnchor.UpperLeft);
            var commodityText = EnsurePanelText(
                panel.transform,
                ResourceHudCommodityTextName,
                new Vector2(828f, -46f),
                new Vector2(250f, 74f),
                34,
                TextAnchor.UpperLeft);
            var dealText = EnsurePanelText(
                panel.transform,
                ResourceHudDealTextName,
                new Vector2(1098f, -46f),
                new Vector2(250f, 74f),
                18,
                TextAnchor.UpperLeft);
            var cashImage = EnsureResourceObjectImage(
                panel.transform,
                "Resource Hud Cash Image",
                new Vector2(178f, -50f),
                new Vector2(70f, 58f));
            var researchImage = EnsureResourceObjectImage(
                panel.transform,
                "Resource Hud Research Image",
                new Vector2(448f, -50f),
                new Vector2(58f, 58f));
            var creditImage = EnsureResourceObjectImage(
                panel.transform,
                "Resource Hud Credit Image",
                new Vector2(718f, -50f),
                new Vector2(58f, 58f));
            var commodityImage = EnsureResourceObjectImage(
                panel.transform,
                "Resource Hud Commodity Image",
                new Vector2(988f, -50f),
                new Vector2(58f, 58f));
            var dealImage = EnsureResourceObjectImage(
                panel.transform,
                "Resource Hud Deal Image",
                new Vector2(1258f, -50f),
                new Vector2(58f, 58f));

            var hud = uiRoot.GetComponent<ResourceHud>();
            if (hud == null)
            {
                hud = uiRoot.gameObject.AddComponent<ResourceHud>();
            }

            hud.Bind(
                panel,
                resourceText,
                messageText,
                cashText,
                researchText,
                creditText,
                commodityText,
                dealText,
                cashImage,
                researchImage,
                creditImage,
                commodityImage,
                dealImage);
            return hud;
        }

        public static PortfolioSummaryView EnsurePortfolioSummaryView(Transform uiRoot)
        {
            var panel = EnsurePanel(
                uiRoot,
                PortfolioSummaryPanelName,
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 1f),
                new Vector2(30f, -53.9f),
                new Vector2(1210f, 300f),
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
            var ownedStockCards = EnsureOwnedStockCards(panel.transform);
            var ownedStockCardButtons = CollectOwnedStockCardButtons(ownedStockCards);
            var ownedStockCardTexts = CollectOwnedStockCardTexts(ownedStockCards);
            var ownedStockSellButtons = CollectOwnedStockSellButtons(ownedStockCards);

            var view = uiRoot.GetComponent<PortfolioSummaryView>();
            if (view == null)
            {
                view = uiRoot.gameObject.AddComponent<PortfolioSummaryView>();
            }

            view.Bind(
                panel,
                summaryText,
                ownedCardsText,
                ownedStockCards,
                ownedStockCardButtons,
                ownedStockCardTexts,
                ownedStockSellButtons);
            return view;
        }

        public static MarketTapeView EnsureMarketTapeView(Transform uiRoot)
        {
            var marketPanel = EnsureMarketPanel(uiRoot);
            MoveOrRemoveRootMarketTapeZone(uiRoot, marketPanel.transform, MarketTapeSellImminentPanelName);
            MoveOrRemoveRootMarketTapeZone(uiRoot, marketPanel.transform, MarketTapeCurrentMarketPanelName);
            MoveOrRemoveRootMarketTapeZone(uiRoot, marketPanel.transform, MarketTapeUpcomingMarketPanelName);

            var sellImminentPanel = EnsureMarketTapeZonePanel(
                marketPanel.transform,
                MarketTapeSellImminentPanelName,
                new Vector2(-440f, -220f));

            var currentMarketPanel = EnsureMarketTapeZonePanel(
                marketPanel.transform,
                MarketTapeCurrentMarketPanelName,
                new Vector2(0f, -220f));

            var upcomingMarketPanel = EnsureMarketTapeZonePanel(
                marketPanel.transform,
                MarketTapeUpcomingMarketPanelName,
                new Vector2(440f, -220f));

            RemoveChildIfPresent(sellImminentPanel.transform, MarketTapeSellImminentTextName);
            RemoveChildIfPresent(currentMarketPanel.transform, MarketTapeCurrentMarketTextName);
            RemoveChildIfPresent(upcomingMarketPanel.transform, MarketTapeUpcomingMarketTextName);

            var sellImminentButtons = EnsureMarketTapeCardButtons(
                sellImminentPanel.transform,
                MarketTapeSellImminentCardButtonPrefix,
                0);
            var currentMarketButtons = EnsureMarketTapeCardButtons(
                currentMarketPanel.transform,
                MarketTapeCurrentMarketCardButtonPrefix,
                8);
            var upcomingMarketButtons = EnsureMarketTapeCardButtons(
                upcomingMarketPanel.transform,
                MarketTapeUpcomingMarketCardButtonPrefix,
                0);
            var hoverPanel = EnsurePanel(
                marketPanel.transform,
                MarketCardHoverPanelName,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -300f),
                new Vector2(360f, 300f),
                new Color(0.08f, 0.10f, 0.12f, 0.98f));
            var hoverText = EnsurePanelText(
                hoverPanel.transform,
                MarketCardHoverTextName,
                new Vector2(18f, -18f),
                new Vector2(324f, 264f),
                24,
                TextAnchor.UpperLeft);
            var hoverImage = hoverPanel.GetComponent<Image>();
            if (hoverImage != null)
            {
                hoverImage.raycastTarget = false;
            }

            ApplyMarketTapeLayout(
                marketPanel,
                sellImminentPanel,
                currentMarketPanel,
                upcomingMarketPanel,
                sellImminentButtons,
                currentMarketButtons,
                upcomingMarketButtons);
            ApplyMarketCardHoverLayout(hoverPanel, hoverText);

            var view = uiRoot.GetComponent<MarketTapeView>();
            if (view == null)
            {
                view = uiRoot.gameObject.AddComponent<MarketTapeView>();
            }

            view.Bind(
                marketPanel,
                hoverPanel,
                hoverText,
                sellImminentButtons,
                currentMarketButtons,
                upcomingMarketButtons);
            hoverPanel.SetActive(false);
            return view;
        }

        public static PurchaseConfirmationView EnsurePurchaseConfirmationView(Transform uiRoot)
        {
            var panel = EnsurePanel(
                uiRoot,
                PurchaseConfirmationPanelName,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(620f, 500f),
                new Color(0.04f, 0.06f, 0.08f, 0.98f));
            panel.transform.SetAsLastSibling();

            var image = panel.GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = true;
            }

            var cardText = EnsurePanelText(
                panel.transform,
                PurchaseConfirmationCardTextName,
                new Vector2(30f, -58f),
                new Vector2(560f, 330f),
                24,
                TextAnchor.UpperLeft);
            if (cardText != null)
            {
                cardText.resizeTextForBestFit = true;
                cardText.resizeTextMinSize = 16;
                cardText.resizeTextMaxSize = 24;
            }

            var confirmButton = EnsureButton(
                panel.transform,
                PurchaseConfirmationConfirmButtonName,
                "확인",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 28f),
                new Vector2(560f, 62f));
            var backButton = EnsureButton(
                panel.transform,
                PurchaseConfirmationBackButtonName,
                "돌아가기",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-28f, -28f),
                new Vector2(150f, 44f));

            var view = uiRoot.GetComponent<PurchaseConfirmationView>();
            if (view == null)
            {
                view = uiRoot.gameObject.AddComponent<PurchaseConfirmationView>();
            }

            view.Bind(panel, cardText, confirmButton, backButton);
            panel.SetActive(false);
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
            var paymentPotBackground = EnsurePaymentPotBackground(panel.transform);
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
                "독서",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-492f, -112f),
                new Vector2(112f, 40f));
            var placeCreditButton = EnsureButton(
                panel.transform,
                CardDetailPlaceCreditButtonName,
                "명상",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-372f, -112f),
                new Vector2(112f, 40f));
            var placeCommodityButton = EnsureButton(
                panel.transform,
                CardDetailPlaceCommodityButtonName,
                "인내",
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
                paymentPotBackground,
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
                paymentPotBackground,
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

        private static void RemoveLegacyDisconnectedUiObjects(Transform uiRoot)
        {
            RemoveChild(uiRoot, LegacyReservationPanelName);
            RemoveChild(uiRoot, CentralBankButtonName);
            RemoveChild(uiRoot, LiquidityActionPanelName);

            var marketPanel = uiRoot.Find(MarketAreaMarketPanelName);
            if (marketPanel != null)
            {
                RemoveChild(marketPanel, LegacyReservationPanelName);
                RemoveChild(marketPanel, CentralBankButtonName);
                RemoveChild(marketPanel, LiquidityActionPanelName);
            }

            var view = uiRoot.GetComponent<LiquidityActionView>();
            if (view != null)
            {
                DestroyObject(view);
            }
        }

        public static LiquidityActionView EnsureLiquidityActionView(Transform uiRoot)
        {
            var marketPanel = EnsureMarketPanel(uiRoot);
            MoveOrRemoveMarketAreaChild(uiRoot, marketPanel.transform, CentralBankButtonName);

            var centralBankButton = EnsureButton(
                uiRoot,
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
                new Vector2(960f, 320f),
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
                "현금\n지폐다발 +1",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(24f, -140f),
                new Vector2(176f, 92f));
            var researchButton = EnsureButton(
                panel.transform,
                LiquidityActionResearchButtonName,
                "독서\n칩 +1",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(220f, -140f),
                new Vector2(176f, 92f));
            var creditButton = EnsureButton(
                panel.transform,
                LiquidityActionCreditButtonName,
                "명상\n칩 +1",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(416f, -140f),
                new Vector2(176f, 92f));
            var commodityButton = EnsureButton(
                panel.transform,
                LiquidityActionCommodityButtonName,
                "인내\n칩 +1",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(612f, -140f),
                new Vector2(176f, 92f));
            var cashImage = EnsureResourceObjectImage(
                cashButton.transform,
                "Liquidity Action Cash Image",
                new Vector2(52f, -12f),
                new Vector2(62f, 48f));
            var researchImage = EnsureResourceObjectImage(
                researchButton.transform,
                "Liquidity Action Research Image",
                new Vector2(52f, -12f),
                new Vector2(48f, 48f));
            var creditImage = EnsureResourceObjectImage(
                creditButton.transform,
                "Liquidity Action Credit Image",
                new Vector2(52f, -12f),
                new Vector2(48f, 48f));
            var commodityImage = EnsureResourceObjectImage(
                commodityButton.transform,
                "Liquidity Action Commodity Image",
                new Vector2(52f, -12f),
                new Vector2(48f, 48f));
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
                commodityButton,
                cashImage,
                researchImage,
                creditImage,
                commodityImage);
            panel.SetActive(false);
            return view;
        }

        private static void RemoveChild(Transform parent, string childName)
        {
            var child = parent.Find(childName);
            if (child != null)
            {
                DestroyObject(child.gameObject);
            }
        }

        private static void DestroyObject(Object target)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(target);
            }
            else
            {
                Object.DestroyImmediate(target);
            }
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
                "수익 +1",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -346f),
                new Vector2(240f, 42f));
            var researchButton = EnsureButton(
                uiRoot,
                ResourceDevResearchButtonName,
                "독서 +1",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -396f),
                new Vector2(240f, 42f));
            var creditButton = EnsureButton(
                uiRoot,
                ResourceDevCreditButtonName,
                "명상 +1",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-176f, -446f),
                new Vector2(240f, 42f));
            var commodityButton = EnsureButton(
                uiRoot,
                ResourceDevCommodityButtonName,
                "인내 +1",
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
            GameObject sellImminentPanel,
            GameObject currentMarketPanel,
            GameObject upcomingMarketPanel,
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

            ApplyMarketTapeZoneLayout(sellImminentPanel, new Vector2(-470f, -220f));
            ApplyMarketTapeZoneLayout(currentMarketPanel, new Vector2(0f, -220f));
            ApplyMarketTapeZoneLayout(upcomingMarketPanel, new Vector2(470f, -220f));
            sellImminentPanel.SetActive(false);
            upcomingMarketPanel.SetActive(false);
            ApplyMarketTapeCardLayout(sellImminentButtons, false);
            ApplyMarketTapeCardLayout(currentMarketButtons, false);
            ApplyMarketTapeCardLayout(upcomingMarketButtons, true);
        }

        private static void ApplyMarketTapeZoneLayout(GameObject zonePanel, Vector2 anchoredPosition)
        {
            if (zonePanel == null)
            {
                return;
            }

            SetRect(
                zonePanel,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                anchoredPosition,
                    zonePanel.name == MarketTapeCurrentMarketPanelName
                        ? new Vector2(1380f, 210f)
                        : new Vector2(420f, 500f));
        }

        private static void ApplyMarketTapeCardLayout(IReadOnlyList<Button> buttons, bool isPreviewColumn)
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                if (button == null)
                {
                    continue;
                }

                var isTapeRow = !isPreviewColumn && buttons.Count == 8;
                SetRect(
                    button.gameObject,
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f),
                    isTapeRow
                        ? new Vector2(-595f + (i * 170f), -96f)
                        : new Vector2(0f, isPreviewColumn ? -96f - (i * 112f) : -92f - (i * 140f)),
                    isTapeRow
                        ? new Vector2(156f, 140f)
                        : isPreviewColumn ? new Vector2(324f, 94f) : new Vector2(372f, 124f));
                SetButtonTextInset(
                    button,
                    new Vector2(14f, 10f),
                    new Vector2(14f, 10f),
                    isTapeRow ? 14 : isPreviewColumn ? 14 : 16,
                    TextAnchor.UpperLeft);
            }
        }

        private static void ApplyMarketCardHoverLayout(GameObject hoverPanel, Text hoverText)
        {
            SetRect(
                hoverPanel,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -292f),
                new Vector2(360f, 300f));
            SetTextRect(hoverText, new Vector2(18f, -18f), new Vector2(324f, 264f), 24, TextAnchor.UpperLeft);
            if (hoverText != null)
            {
                hoverText.resizeTextForBestFit = true;
                hoverText.resizeTextMinSize = 16;
                hoverText.resizeTextMaxSize = 24;
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
            GameObject paymentPotBackground,
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
            SetRect(
                paymentPotBackground,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(856f, -34f),
                new Vector2(408f, 304f));

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

            SetDetailActionButton(placeResearchButton, new Vector2(880f, -334f), "독서");
            SetDetailActionButton(placeCreditButton, new Vector2(1004f, -334f), "명상");
            SetDetailActionButton(placeCommodityButton, new Vector2(1128f, -334f), "인내");
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

        private static void MoveOrRemoveRootMarketTapeZone(
            Transform uiRoot,
            Transform marketPanel,
            string panelName)
        {
            var rootPanel = uiRoot.Find(panelName);
            if (rootPanel == null)
            {
                return;
            }

            var nestedPanel = marketPanel.Find(panelName);
            if (nestedPanel == null)
            {
                rootPanel.SetParent(marketPanel, false);
                return;
            }

            rootPanel.gameObject.SetActive(false);
            if (Application.isPlaying)
            {
                Object.Destroy(rootPanel.gameObject);
            }
            else
            {
                Object.DestroyImmediate(rootPanel.gameObject);
            }
        }

        private static void MoveOrRemoveMarketAreaChild(
            Transform uiRoot,
            Transform marketPanel,
            string objectName)
        {
            var nestedObject = marketPanel.Find(objectName);
            if (nestedObject == null)
            {
                return;
            }

            var rootObject = uiRoot.Find(objectName);
            if (rootObject == null)
            {
                nestedObject.SetParent(uiRoot, false);
                return;
            }

            nestedObject.gameObject.SetActive(false);
            if (Application.isPlaying)
            {
                Object.Destroy(nestedObject.gameObject);
            }
            else
            {
                Object.DestroyImmediate(nestedObject.gameObject);
            }
        }

        private static void RemoveChildIfPresent(Transform parent, string childName)
        {
            var child = parent.Find(childName);
            if (child == null)
            {
                return;
            }

            child.gameObject.SetActive(false);
            if (Application.isPlaying)
            {
                Object.Destroy(child.gameObject);
            }
            else
            {
                Object.DestroyImmediate(child.gameObject);
            }
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

            if (!WasCreatedThisPass(gameObject))
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

            if (!WasCreatedThisPass(text.gameObject))
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

            if (!WasCreatedThisPass(text.gameObject))
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
                : CreateLayoutObject(panelName);

            panel.transform.SetParent(parent, false);

            var rectTransform = panel.GetComponent<RectTransform>();
            if (WasCreatedThisPass(panel))
            {
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.pivot = pivot;
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = sizeDelta;
            }

            var image = panel.GetComponent<Image>();
            var shouldApplyImageDefaults = WasCreatedThisPass(panel) || image == null;
            if (image == null)
            {
                image = panel.AddComponent<Image>();
            }

            if (shouldApplyImageDefaults)
            {
                image.color = color;
                image.raycastTarget = color.a > 0f;
            }

            return panel;
        }

        private static IReadOnlyList<Button> EnsureMarketTapeCardButtons(Transform zonePanel, string namePrefix, int count)
        {
            var buttons = new List<Button>();
            for (var i = 0; i < count; i++)
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
                if (WasCreatedThisPass(text.gameObject))
                {
                    text.alignment = TextAnchor.MiddleLeft;
                    text.fontSize = 16;
                }

                buttons.Add(button);
            }

            return buttons;
        }

        private static IReadOnlyList<GameObject> EnsureOwnedStockCards(Transform panel)
        {
            var cards = new List<GameObject>();
            for (var i = 0; i < OwnedAssetState.DefaultMaxStockSlots; i++)
            {
                var cardNumber = i + 1;
                var card = EnsureOwnedStockCardContainer(panel, cardNumber);
                EnsureOwnedStockCardButton(card.transform, cardNumber);
                EnsureOwnedStockSellButton(card.transform, cardNumber);
                cards.Add(card);
            }

            return cards;
        }

        private static GameObject EnsureOwnedStockCardContainer(Transform panel, int cardNumber)
        {
            var name = OwnedStockCardPrefix + cardNumber;
            var legacyName = "Portfolio Slot " + cardNumber;
            var existing = panel.Find(name) ?? panel.Find(legacyName);
            var card = existing != null
                ? existing.gameObject
                : CreateLayoutObject(name);

            card.name = name;
            card.transform.SetParent(panel, false);
            if (WasCreatedThisPass(card))
            {
                SetRect(
                    card,
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(40f + ((cardNumber - 1) * 144f), -54f),
                    new Vector2(124f, 215f));
            }

            var legacyButton = card.GetComponent<Button>();
            if (legacyButton != null)
            {
                DestroyObject(legacyButton);
            }

            var legacyImage = card.GetComponent<Image>();
            if (legacyImage != null)
            {
                DestroyObject(legacyImage);
            }

            RemoveChildIfPresent(card.transform, legacyName + " Text");
            return card;
        }

        private static Button EnsureOwnedStockCardButton(Transform card, int cardNumber)
        {
            var cardName = OwnedStockCardPrefix + cardNumber;
            var buttonObjectName = cardName + OwnedStockCardButtonSuffix;
            var existing = card.Find(buttonObjectName);
            var buttonObject = existing != null
                ? existing.gameObject
                : CreateLayoutObject(buttonObjectName);

            buttonObject.transform.SetParent(card, false);
            if (WasCreatedThisPass(buttonObject))
            {
                SetRect(
                    buttonObject,
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    Vector2.zero,
                    new Vector2(124f, 200f));
            }

            var image = buttonObject.GetComponent<Image>();
            if (image == null)
            {
                image = buttonObject.AddComponent<Image>();
            }

            if (WasCreatedThisPass(buttonObject))
            {
                image.color = new Color(0.08f, 0.10f, 0.12f, 0.72f);
            }

            var button = buttonObject.GetComponent<Button>();
            if (button == null)
            {
                button = buttonObject.AddComponent<Button>();
            }

            button.targetGraphic = image;
            var text = EnsureChildText(buttonObject.transform, cardName + OwnedStockCardTextSuffix, string.Empty);
            if (WasCreatedThisPass(text.gameObject))
            {
                var rectTransform = text.GetComponent<RectTransform>();
                rectTransform.offsetMin = new Vector2(6f, 6f);
                rectTransform.offsetMax = new Vector2(-6f, -6f);
                text.alignment = TextAnchor.MiddleCenter;
                text.fontSize = 13;
                text.color = Color.white;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 9;
                text.resizeTextMaxSize = 13;
            }

            return button;
        }

        private static Button EnsureOwnedStockSellButton(Transform card, int cardNumber)
        {
            var cardName = OwnedStockCardPrefix + cardNumber;
            var button = EnsureButton(
                card,
                cardName + OwnedStockCardSellButtonSuffix,
                string.Empty,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 1f),
                new Vector2(0f, 15f),
                new Vector2(124f, 30f));

            if (WasCreatedThisPass(button.gameObject))
            {
                var text = button.GetComponentInChildren<Text>();
                if (text != null)
                {
                    text.fontSize = 13;
                }
            }

            button.gameObject.SetActive(false);
            return button;
        }

        private static IReadOnlyList<Button> CollectOwnedStockCardButtons(IReadOnlyList<GameObject> cards)
        {
            var buttons = new List<Button>();
            for (var i = 0; i < cards.Count; i++)
            {
                buttons.Add(FindOwnedStockCardChild<Button>(
                    cards[i],
                    OwnedStockCardPrefix + (i + 1) + OwnedStockCardButtonSuffix));
            }

            return buttons;
        }

        private static IReadOnlyList<Text> CollectOwnedStockCardTexts(IReadOnlyList<GameObject> cards)
        {
            var texts = new List<Text>();
            for (var i = 0; i < cards.Count; i++)
            {
                texts.Add(FindOwnedStockCardChild<Text>(
                    cards[i],
                    OwnedStockCardPrefix + (i + 1) + OwnedStockCardTextSuffix));
            }

            return texts;
        }

        private static IReadOnlyList<Button> CollectOwnedStockSellButtons(IReadOnlyList<GameObject> cards)
        {
            var buttons = new List<Button>();
            for (var i = 0; i < cards.Count; i++)
            {
                buttons.Add(FindOwnedStockCardChild<Button>(
                    cards[i],
                    OwnedStockCardPrefix + (i + 1) + OwnedStockCardSellButtonSuffix));
            }

            return buttons;
        }

        private static T FindOwnedStockCardChild<T>(GameObject card, string childName)
            where T : Component
        {
            var child = FindChildRecursive(card.transform, childName);
            return child != null ? child.GetComponent<T>() : null;
        }

        private static Transform FindChildRecursive(Transform parent, string childName)
        {
            if (parent.name == childName)
            {
                return parent;
            }

            for (var i = 0; i < parent.childCount; i++)
            {
                var match = FindChildRecursive(parent.GetChild(i), childName);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
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
                if (WasCreatedThisPass(text.gameObject))
                {
                    text.fontSize = 15;
                }

                buttons.Add(button);
            }

            return buttons;
        }

        private static GameObject EnsurePaymentPotBackground(Transform panel)
        {
            var existing = panel.Find(CardDetailPaymentPotBackgroundName);
            var background = existing != null
                ? existing.gameObject
                : CreateLayoutObject(CardDetailPaymentPotBackgroundName);

            background.transform.SetParent(panel, false);
            if (WasCreatedThisPass(background))
            {
                background.transform.SetAsFirstSibling();
            }

            var image = background.GetComponent<Image>();
            var shouldApplyImageDefaults = WasCreatedThisPass(background) || image == null;
            if (image == null)
            {
                image = background.AddComponent<Image>();
            }

            if (shouldApplyImageDefaults)
            {
                image.sprite = Resources.Load<Sprite>("PaymentPot_Background_Default");
                image.color = image.sprite != null
                    ? Color.white
                    : new Color(0.13f, 0.11f, 0.08f, 0.92f);
                image.raycastTarget = false;
            }

            return background;
        }

        private static Text EnsureRunStatusText(Transform uiRoot)
        {
            var existing = uiRoot.Find(RunStatusTextName);
            var textObject = existing != null
                ? existing.gameObject
                : CreateLayoutObject(RunStatusTextName);

            textObject.transform.SetParent(uiRoot, false);

            var rectTransform = textObject.GetComponent<RectTransform>();
            if (WasCreatedThisPass(textObject))
            {
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);
                rectTransform.anchoredPosition = new Vector2(0f, -24f);
                rectTransform.sizeDelta = new Vector2(-64f, 58f);
            }

            var text = textObject.GetComponent<Text>();
            if (text == null)
            {
                text = textObject.AddComponent<Text>();
            }

            text.text = RunStatusPlaceholderText;
            if (WasCreatedThisPass(textObject))
            {
                text.alignment = TextAnchor.MiddleLeft;
                text.fontSize = 23;
                text.color = Color.white;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 16;
                text.resizeTextMaxSize = 23;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
            }

            if (text.font == null)
            {
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            return text;
        }

        private static GameObject EnsureMarketTapeZonePanel(
            Transform uiRoot,
            string panelName,
            Vector2 anchoredPosition)
        {
            var existing = uiRoot.Find(panelName);
            var panel = existing != null
                ? existing.gameObject
                : CreateLayoutObject(panelName);

            panel.transform.SetParent(uiRoot, false);

            var rectTransform = panel.GetComponent<RectTransform>();
            if (WasCreatedThisPass(panel))
            {
                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = new Vector2(400f, 280f);
            }

            var image = panel.GetComponent<Image>();
            var shouldApplyImageDefaults = WasCreatedThisPass(panel) || image == null;
            if (image == null)
            {
                image = panel.AddComponent<Image>();
            }

            if (shouldApplyImageDefaults)
            {
                image.color = new Color(0.08f, 0.11f, 0.14f, 0.92f);
            }

            return panel;
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
                : CreateLayoutObject(name);

            buttonObject.transform.SetParent(uiRoot, false);

            var rectTransform = buttonObject.GetComponent<RectTransform>();
            if (WasCreatedThisPass(buttonObject))
            {
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.pivot = pivot;
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = sizeDelta ?? new Vector2(240f, 56f);
            }

            var image = buttonObject.GetComponent<Image>();
            var shouldApplyImageDefaults = WasCreatedThisPass(buttonObject) || image == null;
            if (image == null)
            {
                image = buttonObject.AddComponent<Image>();
            }

            if (shouldApplyImageDefaults)
            {
                image.color = new Color(0.11f, 0.16f, 0.20f, 0.94f);
            }

            var button = buttonObject.GetComponent<Button>();
            if (button == null)
            {
                button = buttonObject.AddComponent<Button>();
            }

            var text = EnsureChildText(buttonObject.transform, name + " Text", label);
            if (WasCreatedThisPass(text.gameObject))
            {
                text.alignment = TextAnchor.MiddleCenter;
                text.fontSize = 22;
                text.color = Color.white;
            }

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
            if (WasCreatedThisPass(text.gameObject))
            {
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.pivot = new Vector2(0f, 1f);
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = sizeDelta;
                text.alignment = alignment;
                text.fontSize = fontSize;
                text.color = Color.white;
            }

            return text;
        }

        private static Image EnsureResourceObjectImage(
            Transform parent,
            string name,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            var image = EnsureChildImage(parent, name);
            if (WasCreatedThisPass(image.gameObject))
            {
                image.transform.SetAsFirstSibling();
            }

            var rectTransform = image.GetComponent<RectTransform>();
            if (WasCreatedThisPass(image.gameObject))
            {
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.pivot = new Vector2(0f, 1f);
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = sizeDelta;
            }

            image.enabled = image.sprite != null;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = Color.white;
            return image;
        }

        private static PlaceholderPanel EnsurePlaceholderPanel(Transform uiRoot, string panelName, string textName)
        {
            var existing = uiRoot.Find(panelName);
            var panel = existing != null
                ? existing.gameObject
                : CreateLayoutObject(panelName);

            panel.transform.SetParent(uiRoot, false);

            var rectTransform = panel.GetComponent<RectTransform>();
            if (WasCreatedThisPass(panel))
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(520f, 180f);
            }

            var image = panel.GetComponent<Image>();
            var shouldApplyImageDefaults = WasCreatedThisPass(panel) || image == null;
            if (image == null)
            {
                image = panel.AddComponent<Image>();
            }

            if (shouldApplyImageDefaults)
            {
                image.color = new Color(0.06f, 0.08f, 0.10f, 0.92f);
            }

            var text = EnsureChildText(panel.transform, textName, string.Empty);
            if (WasCreatedThisPass(text.gameObject))
            {
                text.alignment = TextAnchor.MiddleCenter;
                text.fontSize = 26;
                text.color = Color.white;
            }

            panel.SetActive(false);
            return new PlaceholderPanel(panel, text);
        }

        private static Text EnsureChildText(Transform parent, string name, string value)
        {
            var existing = parent.Find(name);
            var textObject = existing != null
                ? existing.gameObject
                : CreateLayoutObject(name);

            textObject.transform.SetParent(parent, false);

            var rectTransform = textObject.GetComponent<RectTransform>();
            if (WasCreatedThisPass(textObject))
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }

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

        private static Image EnsureChildImage(Transform parent, string name)
        {
            var existing = parent.Find(name);
            var imageObject = existing != null
                ? existing.gameObject
                : CreateLayoutObject(name);

            imageObject.transform.SetParent(parent, false);

            var image = imageObject.GetComponent<Image>();
            if (image == null)
            {
                image = imageObject.AddComponent<Image>();
            }

            return image;
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
