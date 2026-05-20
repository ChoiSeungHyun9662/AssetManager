using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace AssetManager.Tests
{
    public sealed class MainGameShellBootstrapTests
    {
        [SetUp]
        public void SetUp()
        {
            DestroyShellObjects();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyShellObjects();
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapCreatesGameAndUiRoots()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.AddComponent<MainGameShellBootstrap>();

            yield return null;

            var gameRoot = GameObject.Find(ProjectShell.GameRootName);
            var uiRoot = GameObject.Find(ProjectShell.UiRootName);

            Assert.That(gameRoot, Is.Not.Null);
            Assert.That(uiRoot, Is.Not.Null);
            Assert.That(uiRoot.GetComponent<Canvas>(), Is.Not.Null);

            var scaler = uiRoot.GetComponent<CanvasScaler>();
            Assert.That(scaler, Is.Not.Null);
            Assert.That(scaler.uiScaleMode, Is.EqualTo(CanvasScaler.ScaleMode.ScaleWithScreenSize));
            Assert.That(scaler.referenceResolution, Is.EqualTo(ProjectShell.ReferenceResolution));

            var statusText = GameObject.Find("Shell Ready Text");
            Assert.That(statusText, Is.Not.Null);
            Assert.That(statusText.GetComponent<Text>().text, Is.EqualTo("Asset Manager MVP Ready"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapStartsRunAndShowsInitialStatus()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapRunStatusTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            Assert.That(bootstrap.CurrentRun, Is.Not.Null);
            Assert.That(bootstrap.CurrentRun.State, Is.EqualTo(RunState.Playing));
            Assert.That(bootstrap.CurrentRun.AssetCards, Has.Count.GreaterThanOrEqualTo(1));

            var statusText = GameObject.Find(ProjectShell.RunStatusTextName);
            Assert.That(statusText, Is.Not.Null);
            Assert.That(statusText.GetComponent<Text>().text, Is.EqualTo(RunStatusFormatter.Format(bootstrap.CurrentRun)));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapHidesOwnedStockCardsOnNewRun()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPortfolioBoardTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            Assert.That(bootstrap.CurrentRun.OwnedAssets.StockSlots, Is.Empty);
            for (var i = 1; i <= 8; i++)
            {
                var card = FindUiObject("Owned Stock Card " + i);
                Assert.That(card.activeSelf, Is.False);
            }

            Assert.That(FindUiObject(ProjectShell.PortfolioOwnedCardsTextName).GetComponent<Text>().text, Is.Empty);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapResourceDevButtonsUpdateResourceHud()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapResourceHudTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var initialCash = bootstrap.CurrentRun.Resources.Cash;
            var resourceText = FindUiObject(ProjectShell.ResourceHudTextName).GetComponent<Text>();
            var cashText = FindUiObject(ProjectShell.ResourceHudCashTextName).GetComponent<Text>();
            var researchText = FindUiObject(ProjectShell.ResourceHudResearchTextName).GetComponent<Text>();
            var creditText = FindUiObject(ProjectShell.ResourceHudCreditTextName).GetComponent<Text>();
            var commodityText = FindUiObject(ProjectShell.ResourceHudCommodityTextName).GetComponent<Text>();
            var dealText = FindUiObject(ProjectShell.ResourceHudDealTextName).GetComponent<Text>();
            var resourceHud = FindUiObject(ProjectShell.UiRootName).GetComponent<ResourceHud>();
            SetPrivateSprite(resourceHud, "researchChipSprite", CreateTestSprite());

            Assert.That(resourceText.text, Is.Empty);
            Assert.That(cashText.text, Is.EqualTo(initialCash + "$"));
            Assert.That(cashText.text, Does.Not.Contain("지폐다발"));
            Assert.That(cashText.text, Does.Not.Contain("칩"));
            Assert.That(researchText.text, Is.Empty);
            Assert.That(creditText.text, Is.Empty);
            Assert.That(commodityText.text, Is.Empty);
            Assert.That(dealText.text, Is.Empty);

            FindUiObject(ProjectShell.ResourceDevFundingCashButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(initialCash + 1));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(0));
            Assert.That(FindUiObject(ProjectShell.ResourceHudCashTextName).GetComponent<Text>().text, Is.EqualTo((initialCash + 1) + "$"));

            FindUiObject(ProjectShell.ResourceDevEarnedCashButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(initialCash + 2));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentFiscalYearEarnedCash, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Performance.TotalEarnedCash, Is.EqualTo(1));
            Assert.That(FindUiObject(ProjectShell.ResourceHudCashTextName).GetComponent<Text>().text, Is.EqualTo((initialCash + 2) + "$"));

            FindUiObject(ProjectShell.ResourceDevResearchButtonName).GetComponent<Button>().onClick.Invoke();
            FindUiObject(ProjectShell.ResourceDevResearchButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            var updatedResearchText = FindUiObject(ProjectShell.ResourceHudResearchTextName).GetComponent<Text>().text;
            Assert.That(updatedResearchText, Is.Empty);

            var firstResearchChip = FindUiObject("Resource Hud Research Image").GetComponent<Image>();
            var secondResearchChip = FindUiObject("Resource Hud Research Image Stack 1").GetComponent<Image>();
            Assert.That(firstResearchChip.enabled, Is.True);
            Assert.That(secondResearchChip.enabled, Is.True);
            Assert.That(
                secondResearchChip.GetComponent<RectTransform>().anchoredPosition,
                Is.Not.EqualTo(firstResearchChip.GetComponent<RectTransform>().anchoredPosition));
            Assert.That(
                secondResearchChip.GetComponent<RectTransform>().anchoredPosition.x,
                Is.GreaterThan(firstResearchChip.GetComponent<RectTransform>().anchoredPosition.x));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapPreservesExistingResourceHudPanelLayout()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPreservesHudLayoutTests");
            SceneManager.SetActiveScene(scene);

            var uiRoot = new GameObject(ProjectShell.UiRootName);
            var existingPanel = new GameObject(ProjectShell.ResourceHudPanelName, typeof(RectTransform));
            existingPanel.transform.SetParent(uiRoot.transform, false);

            var panelRect = existingPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 0f);
            panelRect.anchorMax = new Vector2(0f, 0f);
            panelRect.pivot = new Vector2(0f, 0f);
            panelRect.anchoredPosition = new Vector2(42f, 84f);
            panelRect.sizeDelta = new Vector2(640f, 220f);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var updatedRect = FindUiObject(ProjectShell.ResourceHudPanelName).GetComponent<RectTransform>();
            Assert.That(updatedRect.anchorMin, Is.EqualTo(new Vector2(0f, 0f)));
            Assert.That(updatedRect.anchorMax, Is.EqualTo(new Vector2(0f, 0f)));
            Assert.That(updatedRect.pivot, Is.EqualTo(new Vector2(0f, 0f)));
            Assert.That(updatedRect.anchoredPosition, Is.EqualTo(new Vector2(42f, 84f)));
            Assert.That(updatedRect.sizeDelta, Is.EqualTo(new Vector2(640f, 220f)));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapRemovesRootLevelMarketTapeZoneDuplicates()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapRemovesMarketTapeDuplicatesTests");
            SceneManager.SetActiveScene(scene);

            var uiRoot = new GameObject(ProjectShell.UiRootName);
            var marketPanel = new GameObject(ProjectShell.MarketAreaMarketPanelName, typeof(RectTransform));
            marketPanel.transform.SetParent(uiRoot.transform, false);

            CreateChild(marketPanel.transform, ProjectShell.MarketTapeSellImminentPanelName);
            CreateChild(marketPanel.transform, ProjectShell.MarketTapeCurrentMarketPanelName);
            CreateChild(marketPanel.transform, ProjectShell.MarketTapeUpcomingMarketPanelName);
            CreateChild(uiRoot.transform, ProjectShell.MarketTapeSellImminentPanelName);
            CreateChild(uiRoot.transform, ProjectShell.MarketTapeCurrentMarketPanelName);
            CreateChild(uiRoot.transform, ProjectShell.MarketTapeUpcomingMarketPanelName);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var updatedUiRoot = GameObject.Find(ProjectShell.UiRootName).transform;
            var updatedMarketPanel = updatedUiRoot.Find(ProjectShell.MarketAreaMarketPanelName);

            Assert.That(updatedUiRoot.Find(ProjectShell.MarketTapeSellImminentPanelName), Is.Null);
            Assert.That(updatedUiRoot.Find(ProjectShell.MarketTapeCurrentMarketPanelName), Is.Null);
            Assert.That(updatedUiRoot.Find(ProjectShell.MarketTapeUpcomingMarketPanelName), Is.Null);
            Assert.That(updatedMarketPanel.Find(ProjectShell.MarketTapeSellImminentPanelName), Is.Not.Null);
            Assert.That(updatedMarketPanel.Find(ProjectShell.MarketTapeCurrentMarketPanelName), Is.Not.Null);
            Assert.That(updatedMarketPanel.Find(ProjectShell.MarketTapeUpcomingMarketPanelName), Is.Not.Null);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapRemovesLegacyCentralBankOverlay()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapKeepsMarketOverlaysUnderUiRootTests");
            SceneManager.SetActiveScene(scene);

            const string legacyReservationPanelName = "Reservation Panel";
            var uiRoot = new GameObject(ProjectShell.UiRootName);
            var marketPanel = new GameObject(ProjectShell.MarketAreaMarketPanelName, typeof(RectTransform));
            marketPanel.transform.SetParent(uiRoot.transform, false);
            CreateChild(marketPanel.transform, legacyReservationPanelName);
            CreateChild(marketPanel.transform, ProjectShell.CentralBankButtonName);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var updatedUiRoot = GameObject.Find(ProjectShell.UiRootName).transform;
            var updatedMarketPanel = updatedUiRoot.Find(ProjectShell.MarketAreaMarketPanelName);

            Assert.That(updatedUiRoot.Find(legacyReservationPanelName), Is.Null);
            Assert.That(updatedUiRoot.Find(ProjectShell.CentralBankButtonName), Is.Null);
            Assert.That(updatedMarketPanel.Find(legacyReservationPanelName), Is.Null);
            Assert.That(updatedMarketPanel.Find(ProjectShell.CentralBankButtonName), Is.Null);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapKeepsResourcesOutOfTopStatusWithoutGainLiquidity()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapResourceObjectTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var statusText = FindUiObject(ProjectShell.RunStatusTextName).GetComponent<Text>().text;
            Assert.That(statusText, Does.Not.Contain("현금"));
            Assert.That(statusText, Does.Not.Contain("독서"));
            Assert.That(statusText, Does.Not.Contain("명상"));
            Assert.That(statusText, Does.Not.Contain("인내"));
            Assert.That(statusText, Does.Not.Contain("딜"));

            var uiRoot = GameObject.Find(ProjectShell.UiRootName).transform;
            Assert.That(FindChild(uiRoot, ProjectShell.CentralBankButtonName), Is.Null);
            Assert.That(FindChild(uiRoot, ProjectShell.LiquidityActionPanelName), Is.Null);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapResourceHudShowsCapMessages()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapResourceMessageTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var researchButton = FindUiObject(ProjectShell.ResourceDevResearchButtonName).GetComponent<Button>();
            for (var i = 0; i < 11; i++)
            {
                researchButton.onClick.Invoke();
            }

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Reading, Is.EqualTo(5));
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.EqualTo("투자 철학 한도: 독서 +1 버림"));

            var dealButton = FindUiObject(ProjectShell.ResourceDevDealButtonName).GetComponent<Button>();
            for (var i = 0; i < 4; i++)
            {
                dealButton.onClick.Invoke();
            }

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(3));
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.EqualTo("딜 한도: 추가 딜 버림"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapShowsInitialMarketTape()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapMarketTapeTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var marketPanel = FindUiObject(ProjectShell.MarketAreaMarketPanelName).transform;

            Assert.That(marketPanel.Find(ProjectShell.MarketTapeSellImminentPanelName), Is.Not.Null);
            Assert.That(marketPanel.Find(ProjectShell.MarketTapeCurrentMarketPanelName), Is.Not.Null);
            Assert.That(marketPanel.Find(ProjectShell.MarketTapeUpcomingMarketPanelName), Is.Not.Null);
            Assert.That(FindChild(marketPanel, ProjectShell.MarketTapeSellImminentTextName), Is.Null);
            Assert.That(FindChild(marketPanel, ProjectShell.MarketTapeCurrentMarketTextName), Is.Null);
            Assert.That(FindChild(marketPanel, ProjectShell.MarketTapeUpcomingMarketTextName), Is.Null);

            if (bootstrap.CurrentRun.MarketTape.SellImminentCards.Count == 0)
            {
                var firstCurrentMarketCard = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0].Card;
                var firstCurrentMarketButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1");
                var eighthCurrentMarketButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "8");
                var firstCurrentMarketButtonText = firstCurrentMarketButton.GetComponentInChildren<Text>().text;

                Assert.That(bootstrap.CurrentRun.MarketTape.UpcomingMarketCards, Is.Empty);
                Assert.That(firstCurrentMarketButtonText, Does.Contain(firstCurrentMarketCard.DisplayName));
                Assert.That(firstCurrentMarketButton.activeSelf, Is.True);
                Assert.That(eighthCurrentMarketButton.activeSelf, Is.True);
                Assert.That(marketPanel.Find(ProjectShell.MarketTapeSellImminentCardButtonPrefix + "1"), Is.Null);
                Assert.That(marketPanel.Find(ProjectShell.MarketTapeUpcomingMarketCardButtonPrefix + "1"), Is.Null);

                yield return SceneManager.UnloadSceneAsync(scene);
                yield break;
            }

            var firstSellImminentCard = bootstrap.CurrentRun.MarketTape.SellImminentCards[0].Card;
            var firstSellImminentButton = FindUiObject(ProjectShell.MarketTapeSellImminentCardButtonPrefix + "1");
            var firstUpcomingMarketButton = FindUiObject(ProjectShell.MarketTapeUpcomingMarketCardButtonPrefix + "1");
            var firstSellImminentButtonText = firstSellImminentButton.GetComponentInChildren<Text>().text;

            Assert.That(firstSellImminentButtonText, Does.Contain(firstSellImminentCard.DisplayName));
            Assert.That(firstSellImminentButtonText, Does.Contain("₩" + firstSellImminentCard.CashCost));
            Assert.That(firstSellImminentButtonText, Does.Contain("R1"));
            Assert.That(firstSellImminentButtonText, Does.Contain("↗" + firstSellImminentCard.Income));
            Assert.That(firstSellImminentButtonText, Does.Contain("◆" + firstSellImminentCard.ManagementValue));
            Assert.That(firstSellImminentButtonText, Does.Not.Contain("현금"));
            Assert.That(firstSellImminentButtonText, Does.Not.Contain("전문자원"));
            Assert.That(firstSellImminentButtonText, Does.Not.Contain("운용가치"));
            Assert.That(firstSellImminentButtonText, Does.Not.Contain("운용 수익"));
            Assert.That(
                firstSellImminentButton.GetComponent<RectTransform>().rect.height,
                Is.GreaterThan(firstUpcomingMarketButton.GetComponent<RectTransform>().rect.height));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapShowsConsumableResourceMarketCardWithoutName()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapConsumableResourceCardTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var resourceCard = new AssetCardData(
                "market-patience-resource-card",
                "Hidden Resource Name",
                "시장 표시 테스트 카드",
                AssetRarity.Uncommon,
                1,
                new ProfessionalResourceCost[0],
                0,
                0,
                new TagData[0],
                cardDomain: CardDomain.ConsumableResource,
                providedResourceType: ResourceType.Patience,
                providedResourceAmount: 2);
            var runtimeCard = new AssetCardRuntimeData(resourceCard, AssetCardRuntimeState.Available, PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, runtimeCard, 0));
            RefreshRunUi(bootstrap);

            yield return null;

            var cardText = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1")
                .GetComponentInChildren<Text>()
                .text;

            Assert.That(cardText, Does.Contain("₩1"));
            Assert.That(cardText, Does.Contain("Uncommon"));
            Assert.That(cardText, Does.Contain("인내 +2"));
            Assert.That(cardText, Does.Not.Contain(resourceCard.DisplayName));
            Assert.That(cardText, Does.Not.Contain("↗"));
            Assert.That(cardText, Does.Not.Contain("◆"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapMarketCardClickKeepsSingleMarketState()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapCardDetailTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape);
            var selectedCard = bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].Card.Card;
            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var marketPanel = FindUiObject(ProjectShell.MarketAreaMarketPanelName);
            var cardDetailPanel = FindUiObject(ProjectShell.CardDetailPanelName);
            var currentMarketButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>();
            var nextBusinessDayButton = FindUiObject(ProjectShell.NextBusinessDayButtonName)
                .GetComponent<Button>();

            Assert.That(marketPanel.activeSelf, Is.True);
            Assert.That(cardDetailPanel.activeSelf, Is.False);
            Assert.That(nextBusinessDayButton.interactable, Is.True);

            currentMarketButton.onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard.Card.Id, Is.EqualTo(selectedCard.Id));
            Assert.That(marketPanel.activeSelf, Is.True);
            Assert.That(cardDetailPanel.activeSelf, Is.False);
            Assert.That(nextBusinessDayButton.interactable, Is.True);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapMarketCardHoverShowsLargerSameCardWithoutStateChange()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapMarketHoverTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape);
            var currentMarketButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            var buttonText = currentMarketButtonObject.GetComponentInChildren<Text>().text;
            var hoverPanel = FindUiObject(ProjectShell.MarketCardHoverPanelName);
            var hoverText = FindUiObject(ProjectShell.MarketCardHoverTextName).GetComponent<Text>();
            var buttonRect = currentMarketButtonObject.GetComponent<RectTransform>();
            var hoverRect = hoverPanel.GetComponent<RectTransform>();

            Assert.That(hoverPanel.activeSelf, Is.False);

            EnterPointer(currentMarketButtonObject);

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard, Is.Null);
            Assert.That(hoverPanel.activeSelf, Is.True);
            Assert.That(hoverText.text, Is.EqualTo(buttonText));
            Assert.That(hoverRect.rect.width, Is.GreaterThan(buttonRect.rect.width));
            Assert.That(hoverRect.rect.height, Is.GreaterThan(buttonRect.rect.height));

            ExitPointer(currentMarketButtonObject);

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(hoverPanel.activeSelf, Is.False);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapUpcomingMarketCardOpensPreviewWithoutTransactionActions()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapUpcomingPreviewTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            if (bootstrap.CurrentRun.MarketTape.UpcomingMarketCards.Count == 0)
            {
                var marketPanel = FindUiObject(ProjectShell.MarketAreaMarketPanelName).transform;

                Assert.That(marketPanel.Find(ProjectShell.MarketTapeUpcomingMarketCardButtonPrefix + "1"), Is.Null);
                Assert.That(FindUiObject(ProjectShell.CardDetailPanelName).activeSelf, Is.False);

                yield return SceneManager.UnloadSceneAsync(scene);
                yield break;
            }

            var selectedCard = bootstrap.CurrentRun.MarketTape.UpcomingMarketCards[0].Card;

            FindUiObject(ProjectShell.MarketTapeUpcomingMarketCardButtonPrefix + "1")
                .GetComponent<Button>()
                .onClick
                .Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard.Card.Id, Is.EqualTo(selectedCard.Id));
            Assert.That(FindUiObject(ProjectShell.CardDetailPanelName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailNameTextName).GetComponent<Text>().text, Does.Contain(selectedCard.DisplayName));
            Assert.That(FindUiObject(ProjectShell.CardDetailBuyButtonName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailReserveButtonName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentSlotsTextName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailFinalCashCostTextName).activeSelf, Is.False);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapCardDetailPaymentPlacesRecoversAndConfirmsMarketPurchase()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapCardDetailPaymentTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            FindUiObject(ProjectShell.ResourceDevResearchButtonName).GetComponent<Button>().onClick.Invoke();
            FindUiObject(ProjectShell.ResourceDevCreditButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "payment-ui-stock",
                    "Payment UI Stock",
                    "Payment UI stock test card.",
                    AssetRarity.Common,
                    1,
                    new[] { new ProfessionalResourceCost(ResourceType.Reading, 1) },
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);
            var previousCash = bootstrap.CurrentRun.Resources.Cash;
            var expectedCashAfterPurchase = previousCash - selectedCard.Card.CashCost + selectedCard.Card.Income;
            var previousSlotIds = CollectSlotCardIds(bootstrap.CurrentRun.MarketTape);
            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();

            yield return null;

            var buyButton = FindUiObject(ProjectShell.CardDetailBuyButtonName).GetComponent<Button>();
            Assert.That(buyButton.interactable, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentSlotsTextName).GetComponent<Text>().text, Does.Contain("독서: 비어 있음"));

            FindUiObject(ProjectShell.CardDetailPlaceResearchButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Research, Is.EqualTo(1));
            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentSlotsTextName).GetComponent<Text>().text, Does.Contain("독서: 독서"));

            FindUiObject(ProjectShell.CardDetailPaymentSlotButtonPrefix + "1").GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Research, Is.EqualTo(1));
            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentSlotsTextName).GetComponent<Text>().text, Does.Contain("독서: 비어 있음"));

            FindUiObject(ProjectShell.CardDetailPlaceResearchButtonName).GetComponent<Button>().onClick.Invoke();
            FindUiObject(ProjectShell.CardDetailPlaceCreditButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(previousCash));
            Assert.That(
                FindUiObject(ProjectShell.CardDetailFinalCashCostTextName).GetComponent<Text>().text,
                Does.Contain("최종 현금 " + selectedCard.Card.CashCost));
            Assert.That(buyButton.interactable, Is.True);

            buyButton.onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(3));
            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(expectedCashAfterPurchase));
            Assert.That(
                bootstrap.CurrentRun.Resources.Research,
                Is.EqualTo(1 - CountProfessionalCost(selectedCard.Card, ResourceType.Research)));
            Assert.That(
                bootstrap.CurrentRun.Resources.Credit,
                Is.EqualTo(1 - CountProfessionalCost(selectedCard.Card, ResourceType.Credit)));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(CollectSlotCardIds(bootstrap.CurrentRun.MarketTape), Does.Not.Contain(selectedCard.Card.Id));
            Assert.That(CollectSlotCardIds(bootstrap.CurrentRun.MarketTape), Has.Count.EqualTo(previousSlotIds.Count));
            Assert.That(FindUiObject(ProjectShell.CardDetailPanelName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.PortfolioSummaryTextName).GetComponent<Text>().text, Does.Contain("보유 자산 1"));
            Assert.That(
                FindUiObject(ProjectShell.PortfolioSummaryTextName).GetComponent<Text>().text,
                Does.Contain("현재 운용가치 " + selectedCard.Card.ManagementValue));
            Assert.That(
                FindUiObject(ProjectShell.PortfolioSummaryTextName).GetComponent<Text>().text,
                Does.Contain("분기 수익 " + selectedCard.Card.Income));
            Assert.That(
                FindUiObject(ProjectShell.PortfolioOwnedCardsTextName).GetComponent<Text>().text,
                Is.Empty);
            var firstOwnedStockCard = FindUiObject("Owned Stock Card 1");
            var firstOwnedStockCardButton = FindUiObject("Owned Stock Card 1 Card Button");
            var firstOwnedStockCardText = FindUiObject("Owned Stock Card 1 Text").GetComponent<Text>().text;
            var firstSellButton = FindUiObject("Owned Stock Card 1 Sell Button").GetComponent<Button>();
            Assert.That(firstOwnedStockCard.activeSelf, Is.True);
            Assert.That(firstOwnedStockCardText, Does.Contain(selectedCard.Card.DisplayName));
            Assert.That(firstOwnedStockCardText, Does.Contain("등급 " + selectedCard.Card.Rarity));
            Assert.That(firstOwnedStockCardText, Does.Contain("가치 " + selectedCard.Card.ManagementValue));
            Assert.That(firstOwnedStockCardText, Does.Contain("배당금 " + selectedCard.Card.Income));
            Assert.That(firstOwnedStockCardText, Does.Not.Contain("₩"));
            Assert.That(firstOwnedStockCardText, Does.Not.Contain("구매"));
            Assert.That(firstOwnedStockCardText, Does.Not.Contain("예약"));
            Assert.That(firstOwnedStockCardText, Does.Not.Contain("매도"));
            Assert.That(firstSellButton.gameObject.activeSelf, Is.False);

            var cashBeforeSale = bootstrap.CurrentRun.Resources.Cash;
            var quarterRevenueBeforeSale = bootstrap.CurrentRun.Performance.CurrentQuarterEarnedCash;
            firstOwnedStockCardButton.GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(cashBeforeSale));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(quarterRevenueBeforeSale));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.Count, Is.EqualTo(1));
            Assert.That(firstSellButton.gameObject.activeSelf, Is.False);

            EnterPointer(firstOwnedStockCardButton);
            yield return null;
            Assert.That(firstSellButton.gameObject.activeSelf, Is.True);
            Assert.That(firstSellButton.GetComponentInChildren<Text>().text, Is.EqualTo("매도 +1$"));

            ExitPointer(firstOwnedStockCardButton);
            yield return null;
            Assert.That(firstSellButton.gameObject.activeSelf, Is.False);

            EnterPointer(firstOwnedStockCardButton);
            yield return null;
            Assert.That(firstSellButton.gameObject.activeSelf, Is.True);

            EnterPointer(firstSellButton.gameObject);
            ExitPointer(firstOwnedStockCardButton);
            yield return null;
            Assert.That(firstSellButton.gameObject.activeSelf, Is.True);

            EnterPointer(firstOwnedStockCardButton);
            ExitPointer(firstSellButton.gameObject);
            yield return null;
            Assert.That(firstSellButton.gameObject.activeSelf, Is.True);

            ExitPointer(firstOwnedStockCardButton);
            yield return null;
            Assert.That(firstSellButton.gameObject.activeSelf, Is.False);

            EnterPointer(firstOwnedStockCardButton);
            yield return null;
            Assert.That(firstSellButton.gameObject.activeSelf, Is.True);

            var otherMarketSlotIndex = FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape);
            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (otherMarketSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();
            yield return null;
            Assert.That(firstSellButton.gameObject.activeSelf, Is.False);

            FindUiObject(ProjectShell.CardDetailCloseButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            EnterPointer(firstOwnedStockCardButton);
            yield return null;
            Assert.That(firstSellButton.gameObject.activeSelf, Is.True);

            firstSellButton.onClick.Invoke();
            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(cashBeforeSale + 1));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(quarterRevenueBeforeSale + 1));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(3));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.Count, Is.EqualTo(0));
            Assert.That(FindUiObject("Owned Stock Card 1").activeSelf, Is.False);
            Assert.That(firstSellButton.gameObject.activeSelf, Is.False);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapPortfolioBoardShowsFoilMergeResultAndBlankConsumedSlot()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPortfolioFoilBoardTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var stock = new AssetCardData(
                "portfolio-foil-ui-stock",
                "Portfolio Foil UI Stock",
                "Portfolio foil board UI test stock",
                AssetRarity.Rare,
                0,
                new ProfessionalResourceCost[0],
                2,
                1,
                new TagData[0],
                false,
                9,
                4);
            var firstOwned = new AssetCardRuntimeData(
                stock,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape,
                1,
                false,
                "portfolio-foil-ui-stock-1");
            var secondOwned = new AssetCardRuntimeData(
                stock,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape,
                2,
                false,
                "portfolio-foil-ui-stock-2");
            var otherStock = new AssetCardData(
                "portfolio-other-ui-stock",
                "Portfolio Other UI Stock",
                "Portfolio board UI test stock",
                AssetRarity.Common,
                0,
                new ProfessionalResourceCost[0],
                3,
                0,
                new TagData[0]);
            var otherOwned = new AssetCardRuntimeData(
                otherStock,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape,
                3,
                false,
                "portfolio-other-ui-stock-1");
            var thirdCopy = new AssetCardRuntimeData(
                stock,
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape,
                null,
                false,
                "portfolio-foil-ui-stock-3");
            var run = WithOwnedAssets(
                bootstrap.CurrentRun,
                new OwnedAssetState(new[] { firstOwned, secondOwned, otherOwned }));
            run = WithCurrentMarketCard(run, thirdCopy, 0);
            SetCurrentRun(bootstrap, run);
            RefreshRunUi(bootstrap);

            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1").GetComponent<Button>().onClick.Invoke();
            yield return null;

            FindUiObject(ProjectShell.CardDetailBuyButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            var firstCard = FindUiObject("Owned Stock Card 1");
            var secondCard = FindUiObject("Owned Stock Card 2");
            var thirdCard = FindUiObject("Owned Stock Card 3");
            var firstCardText = FindUiObject("Owned Stock Card 1 Text").GetComponent<Text>().text;
            var secondCardText = FindUiObject("Owned Stock Card 2 Text").GetComponent<Text>().text;

            Assert.That(firstCard.activeSelf, Is.True);
            Assert.That(secondCard.activeSelf, Is.True);
            Assert.That(thirdCard.activeSelf, Is.False);
            Assert.That(firstCardText, Does.Contain(stock.DisplayName));
            Assert.That(firstCardText, Does.Contain("FOIL"));
            Assert.That(firstCardText, Does.Contain("가치 " + stock.FoilValue));
            Assert.That(firstCardText, Does.Contain("배당금 " + stock.FoilDividend));
            Assert.That(secondCardText, Does.Contain(otherStock.DisplayName));
            Assert.That(
                FindUiObject("Owned Stock Card 1 Card Button").GetComponent<Image>().color,
                Is.Not.EqualTo(FindUiObject("Owned Stock Card 2 Card Button").GetComponent<Image>().color));
            Assert.That(FindUiObject(ProjectShell.PortfolioOwnedCardsTextName).GetComponent<Text>().text, Is.Empty);

            EnterPointer(FindUiObject("Owned Stock Card 2 Card Button"));
            yield return null;

            var secondSellButton = FindUiObject("Owned Stock Card 2 Sell Button").GetComponent<Button>();
            Assert.That(secondSellButton.gameObject.activeSelf, Is.True);

            secondSellButton.onClick.Invoke();
            yield return null;

            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(stock.Id));
            Assert.That(FindUiObject("Owned Stock Card 1").activeSelf, Is.True);
            Assert.That(FindUiObject("Owned Stock Card 2").activeSelf, Is.False);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapCardDetailShowsPaymentPotForProfessionalCostSlotsOnly()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPaymentPotTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape, ResourceType.Reading);
            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();

            yield return null;

            var paymentPot = FindUiObject(ProjectShell.CardDetailPaymentPotBackgroundName);
            var paymentSlotsText = FindUiObject(ProjectShell.CardDetailPaymentSlotsTextName).GetComponent<Text>().text;
            var slotButtonText = FindUiObject(ProjectShell.CardDetailPaymentSlotButtonPrefix + "1")
                .GetComponentInChildren<Text>()
                .text;

            Assert.That(paymentPot.activeSelf, Is.True);
            Assert.That(paymentPot.GetComponent<Image>(), Is.Not.Null);
            Assert.That(paymentSlotsText, Does.Contain("Payment Pot"));
            Assert.That(paymentSlotsText, Does.Not.Contain("현금:"));
            Assert.That(slotButtonText, Does.Contain("독서"));
            Assert.That(slotButtonText, Does.Contain("비어 있음"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapCardDetailFinalCashCostShowsInflationAndDealPlacement()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapInflationCostTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var run = WithCalendar(
                bootstrap.CurrentRun,
                new RunCalendarState(1, 2, bootstrap.CurrentRun.Calendar.RemainingBusinessDays));
            run = ResourceLedger.AddDeal(run, 1).Run;
            SetCurrentRun(bootstrap, run);

            var selectedSlotIndex = FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape, ResourceType.Reading);
            var selectedCard = bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].Card;
            bootstrap.OpenMarketCardDetail(selectedCard);

            yield return null;

            Assert.That(
                FindUiObject(ProjectShell.CardDetailFinalCashCostTextName).GetComponent<Text>().text,
                Does.Contain("최종 현금 " + (selectedCard.Card.CashCost + 1)));

            FindUiObject(ProjectShell.CardDetailPlaceDealButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.CardDetail.PendingPayment.FinalCashCost, Is.EqualTo(selectedCard.Card.CashCost));
            Assert.That(
                FindUiObject(ProjectShell.CardDetailFinalCashCostTextName).GetComponent<Text>().text,
                Does.Contain("최종 현금 " + selectedCard.Card.CashCost));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapReserveButtonLocksMarketSlotAndUpdatesUi()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapReservationTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = FindFirstAffordableMarketSlotIndex(bootstrap.CurrentRun.MarketTape);
            var selectedCard = bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].Card;
            var previousSlotIds = CollectSlotCardIds(bootstrap.CurrentRun.MarketTape);
            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;

            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();

            yield return null;

            var reserveButton = FindUiObject(ProjectShell.CardDetailReserveButtonName).GetComponent<Button>();
            Assert.That(reserveButton.interactable, Is.True);

            reserveButton.onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Reservation.ReservedCards, Is.Empty);
            Assert.That(bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].Card.Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].IsReserved, Is.True);
            Assert.That(CountReservedSlots(bootstrap.CurrentRun.MarketTape), Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.RedemptionPressure.CurrentPressure, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays - 1));
            Assert.That(CollectSlotCardIds(bootstrap.CurrentRun.MarketTape), Is.EqualTo(previousSlotIds));
            Assert.That(FindUiObject(ProjectShell.CardDetailPanelName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.EqualTo("월세 밀림 +1"));
            Assert.That(
                FindUiObject(ProjectShell.RunStatusTextName).GetComponent<Text>().text,
                Does.Contain("월세 밀림 1/10"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapReservedMarketSlotClickShowsDetailAndPurchasePullsTape()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapReservedPurchaseTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            FindUiObject(ProjectShell.ResourceDevResearchButtonName).GetComponent<Button>().onClick.Invoke();
            FindUiObject(ProjectShell.ResourceDevCreditButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            var selectedSlotIndex = FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape);
            var selectedCard = bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].Card;
            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();

            yield return null;

            FindUiObject(ProjectShell.CardDetailReserveButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            var previousRemainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var previousSlotIds = CollectSlotCardIds(bootstrap.CurrentRun.MarketTape);

            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard.Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.CardDetail.PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
            Assert.That(FindUiObject(ProjectShell.CardDetailPanelName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailReserveButtonName).activeSelf, Is.False);

            FindUiObject(ProjectShell.CardDetailPlaceResearchButtonName).GetComponent<Button>().onClick.Invoke();
            FindUiObject(ProjectShell.CardDetailPlaceCreditButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            FindUiObject(ProjectShell.CardDetailBuyButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(previousRemainingBusinessDays - 1));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards[0].PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
            Assert.That(bootstrap.CurrentRun.Reservation.ReservedCards, Is.Empty);
            Assert.That(CollectSlotCardIds(bootstrap.CurrentRun.MarketTape), Does.Not.Contain(selectedCard.Card.Id));
            Assert.That(CollectSlotCardIds(bootstrap.CurrentRun.MarketTape), Is.Not.EqualTo(previousSlotIds));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapFullReservationAreaDisablesReserveAndPreservesBusinessDay()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapFullReservationTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            for (var i = 0; i < 3; i++)
            {
                bootstrap.OpenMarketCardDetail(FindFirstAvailableMarketSlotCard(bootstrap.CurrentRun.MarketTape));
                bootstrap.ConfirmReservation();
            }

            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var deal = bootstrap.CurrentRun.Resources.Deal;
            var pressure = bootstrap.CurrentRun.RedemptionPressure.CurrentPressure;

            bootstrap.OpenMarketCardDetail(FindFirstAvailableMarketSlotCard(bootstrap.CurrentRun.MarketTape));

            yield return null;

            var reserveButton = FindUiObject(ProjectShell.CardDetailReserveButtonName).GetComponent<Button>();
            Assert.That(reserveButton.interactable, Is.False);
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.EqualTo("예약 구역이 가득 찼습니다."));

            bootstrap.ConfirmReservation();

            yield return null;

            Assert.That(CountReservedSlots(bootstrap.CurrentRun.MarketTape), Is.EqualTo(3));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(deal));
            Assert.That(bootstrap.CurrentRun.RedemptionPressure.CurrentPressure, Is.EqualTo(pressure));
            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapReservationAtNinePressureShowsRunFailure()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapReservationFailureTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            SetCurrentRun(bootstrap, WithRedemptionPressure(bootstrap.CurrentRun, 9));
            bootstrap.OpenMarketCardDetail(
                bootstrap.CurrentRun.MarketTape.Slots[FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape)].Card);

            yield return null;

            FindUiObject(ProjectShell.CardDetailReserveButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.State, Is.EqualTo(RunState.Failed));
            Assert.That(bootstrap.CurrentRun.RedemptionPressure.CurrentPressure, Is.EqualTo(10));
            Assert.That(FindUiObject(ProjectShell.RunFailurePlaceholderPanelName).activeSelf, Is.True);
            Assert.That(
                FindUiObject(ProjectShell.RunFailurePlaceholderTextName).GetComponent<Text>().text,
                Does.Contain("파산"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapDoesNotEnterGainLiquidityFromNewPlayFlow()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapLiquidityActionTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var nextBusinessDayButton = FindUiObject(ProjectShell.NextBusinessDayButtonName).GetComponent<Button>();
            var uiRoot = GameObject.Find(ProjectShell.UiRootName).transform;

            Assert.That(FindChild(uiRoot, ProjectShell.CentralBankButtonName), Is.Null);
            Assert.That(FindChild(uiRoot, ProjectShell.LiquidityActionPanelName), Is.Null);
            Assert.That(nextBusinessDayButton.interactable, Is.True);

            bootstrap.EnterLiquidityAction();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(nextBusinessDayButton.interactable, Is.True);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapMarketTapeAdvanceButtonAdvancesTapeAndUi()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapMarketTapeAdvanceTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var pulledCard = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0];
            var previousSlotIds = CollectSlotRuntimeIds(bootstrap.CurrentRun.MarketTape);
            var buttonObject = FindUiObject(ProjectShell.MarketTapeAdvanceButtonName);
            Assert.That(buttonObject, Is.Not.Null);

            buttonObject.GetComponent<Button>().onClick.Invoke();

            yield return null;

            if (bootstrap.CurrentRun.MarketTape.SellImminentCards.Count == 0)
            {
                Assert.That(CollectSlotRuntimeIds(bootstrap.CurrentRun.MarketTape), Does.Not.Contain(pulledCard.RuntimeId));
                Assert.That(CollectSlotRuntimeIds(bootstrap.CurrentRun.MarketTape), Has.Count.EqualTo(previousSlotIds.Count));
                Assert.That(bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0].RuntimeId, Is.EqualTo(previousSlotIds[1]));

                var firstCurrentMarketCardButtonText = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1")
                    .GetComponentInChildren<Text>()
                    .text;
                if (!string.IsNullOrEmpty(pulledCard.Card.DisplayName))
                {
                    Assert.That(firstCurrentMarketCardButtonText, Does.Not.Contain(pulledCard.Card.DisplayName));
                }
            }
            else
            {
                Assert.That(bootstrap.CurrentRun.MarketTape.SellImminentCards[0].RuntimeId, Is.EqualTo(pulledCard.RuntimeId));

                var sellImminentCardButtonText = FindUiObject(ProjectShell.MarketTapeSellImminentCardButtonPrefix + "1")
                    .GetComponentInChildren<Text>()
                    .text;
                Assert.That(sellImminentCardButtonText, Does.Contain(pulledCard.Card.DisplayName));
            }

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapNextBusinessDayButtonAdvancesRunAndStatus()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapNextBusinessDayTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var buttonObject = GameObject.Find(ProjectShell.NextBusinessDayButtonName);
            Assert.That(buttonObject, Is.Not.Null);

            buttonObject.GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(3));

            var statusText = GameObject.Find(ProjectShell.RunStatusTextName);
            Assert.That(statusText.GetComponent<Text>().text, Is.EqualTo(RunStatusFormatter.Format(bootstrap.CurrentRun)));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapShowsExtraBuyChoiceState()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapExtraBuyChoiceTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            SetCurrentRun(bootstrap, ExtraBuyAction.BeginChoice(bootstrap.CurrentRun));
            RefreshRunUi(bootstrap);

            yield return null;

            var statusText = FindUiObject(ProjectShell.RunStatusTextName).GetComponent<Text>();
            var marketTapeAdvanceButton = FindUiObject(ProjectShell.MarketTapeAdvanceButtonName);
            var resourceDevCashButton = FindUiObject(ProjectShell.ResourceDevFundingCashButtonName);
            var nextBusinessDayButton = FindUiObject(ProjectShell.NextBusinessDayButtonName).GetComponent<Button>();
            var nextBusinessDayButtonText = nextBusinessDayButton.GetComponentInChildren<Text>();

            Assert.That(statusText.text, Does.Contain("추가 매수 가능"));
            Assert.That(FindChild(GameObject.Find(ProjectShell.UiRootName).transform, ProjectShell.CentralBankButtonName), Is.Null);
            Assert.That(marketTapeAdvanceButton.activeSelf, Is.False);
            Assert.That(resourceDevCashButton.activeSelf, Is.False);
            Assert.That(nextBusinessDayButton.gameObject.activeSelf, Is.True);
            Assert.That(nextBusinessDayButton.interactable, Is.True);
            Assert.That(nextBusinessDayButtonText.text, Is.EqualTo("추가 매수 포기"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapShowsQuarterSettlementPlaceholderAfterLastBusinessDay()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapQuarterSettlementTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var button = GameObject.Find(ProjectShell.NextBusinessDayButtonName).GetComponent<Button>();
            for (var i = 0; i < 4; i++)
            {
                button.onClick.Invoke();
            }

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.QuarterSettlement));

            var panel = GameObject.Find(ProjectShell.QuarterSettlementPlaceholderPanelName);
            Assert.That(panel, Is.Not.Null);
            Assert.That(panel.activeSelf, Is.True);

            var text = GameObject.Find(ProjectShell.QuarterSettlementPlaceholderTextName).GetComponent<Text>().text;
            Assert.That(text, Does.Contain("현재 분기 수익"));
            Assert.That(text, Does.Contain("분기 목표"));
            Assert.That(text, Does.Contain("목표 달성률"));
            Assert.That(text, Does.Contain("월세 밀림 +3"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapQuarterSettlementFailureShowsRunFailureSummaryAndStopsContinue()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapQuarterFailureTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var run = WithCalendar(bootstrap.CurrentRun, new RunCalendarState(1, 2, 1));
            run = WithPerformance(run, new RunPerformanceState(2, 2, 2, 0));
            run = WithRedemptionPressure(run, 8);
            SetCurrentRun(bootstrap, run);

            bootstrap.AdvanceToNextBusinessDay();

            yield return null;

            Assert.That(bootstrap.CurrentRun.State, Is.EqualTo(RunState.Failed));
            Assert.That(bootstrap.CurrentRun.RedemptionPressure.CurrentPressure, Is.EqualTo(10));
            Assert.That(FindUiObject(ProjectShell.QuarterSettlementPlaceholderPanelName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.RunFailurePlaceholderPanelName).activeSelf, Is.True);
            Assert.That(FindUiObject(ProjectShell.ContinueScheduleButtonName).activeSelf, Is.False);

            var failureText = FindUiObject(ProjectShell.RunFailurePlaceholderTextName).GetComponent<Text>().text;
            Assert.That(failureText, Does.Contain("파산"));
            Assert.That(failureText, Does.Contain("도달 지점 1회계년도 2Q"));
            Assert.That(failureText, Does.Contain("총 수익 2"));
            Assert.That(failureText, Does.Contain("월세 밀림 10/10"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapShowsVacationSummaryAndContinuesToNextFiscalYear()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapVacationTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            for (var quarter = 1; quarter <= 3; quarter++)
            {
                for (var day = 0; day < 4; day++)
                {
                    bootstrap.AdvanceToNextBusinessDay();
                }

                yield return null;
                bootstrap.ContinueSchedule();
                yield return null;
            }

            Assert.That(bootstrap.CurrentRun.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.Vacation));
            Assert.That(FindUiObject(ProjectShell.VacationPlaceholderPanelName).activeSelf, Is.True);
            Assert.That(FindUiObject(ProjectShell.ContinueScheduleButtonName).activeSelf, Is.True);

            var vacationText = FindUiObject(ProjectShell.VacationPlaceholderTextName).GetComponent<Text>().text;
            Assert.That(vacationText, Does.Contain("4Q 휴가: 1회계년도 요약"));
            Assert.That(vacationText, Does.Contain("현재 가치"));
            Assert.That(vacationText, Does.Contain("올해 수익"));
            Assert.That(vacationText, Does.Contain("분기별 수익"));
            Assert.That(vacationText, Does.Contain("보유 주식"));
            Assert.That(vacationText, Does.Contain("월세 밀림"));

            FindUiObject(ProjectShell.ContinueScheduleButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.That(bootstrap.CurrentRun.Calendar.FiscalYear, Is.EqualTo(2));
            Assert.That(bootstrap.CurrentRun.Calendar.Quarter, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.AwaitingAction));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapShowsFinalSettlementSummary()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapFinalSettlementTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var ownedOffice = new AssetCardRuntimeData(
                bootstrap.CurrentRun.AssetCards[0].Card,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape);
            var ownedDataCenter = new AssetCardRuntimeData(
                bootstrap.CurrentRun.StaticData.AssetCards[3],
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape);
            var run = WithCalendar(bootstrap.CurrentRun, new RunCalendarState(3, 4, 0));
            run = WithBusinessDay(run, new BusinessDayState(BusinessDayPhase.QuarterSettlement, MarketAreaState.Market));
            run = WithOwnedAssets(run, new OwnedAssetState(new[] { ownedOffice, ownedDataCenter }));
            SetCurrentRun(bootstrap, run);

            bootstrap.ContinueSchedule();

            yield return null;

            Assert.That(bootstrap.CurrentRun.State, Is.EqualTo(RunState.Completed));
            Assert.That(bootstrap.CurrentRun.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.FinalSettlement));
            Assert.That(FindUiObject(ProjectShell.FinalSettlementPlaceholderPanelName).activeSelf, Is.True);

            var finalText = FindUiObject(ProjectShell.FinalSettlementPlaceholderTextName).GetComponent<Text>().text;
            Assert.That(finalText, Does.Contain("최종 가치 7"));
            Assert.That(finalText, Does.Contain("최종 평가 Core"));
            Assert.That(finalText, Does.Contain("총 수익"));
            Assert.That(finalText, Does.Contain("보유 주식 2"));
            Assert.That(finalText, Does.Contain("월세 밀림"));
            Assert.That(finalText, Does.Contain("최종 코멘트"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        private static void EnterPointer(GameObject target)
        {
            ExecutePointerEvent(target, ExecuteEvents.pointerEnterHandler);
        }

        private static void ExitPointer(GameObject target)
        {
            ExecutePointerEvent(target, ExecuteEvents.pointerExitHandler);
        }

        private static void ExecutePointerEvent<T>(GameObject target, ExecuteEvents.EventFunction<T> eventFunction)
            where T : IEventSystemHandler
        {
            Assert.That(EventSystem.current, Is.Not.Null);
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), eventFunction);
        }

        private static GameObject FindUiObject(string objectName)
        {
            var uiRoot = FindRootObject(ProjectShell.UiRootName);
            Assert.That(uiRoot, Is.Not.Null);

            var match = FindChild(uiRoot.transform, objectName);
            Assert.That(match, Is.Not.Null, "Expected to find UI object " + objectName + ".");
            return match.gameObject;
        }

        private static GameObject FindRootObject(string objectName)
        {
            var scene = SceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == objectName)
                {
                    return root;
                }
            }

            return null;
        }

        private static Transform FindChild(Transform parent, string objectName)
        {
            if (parent.name == objectName)
            {
                return parent;
            }

            for (var i = 0; i < parent.childCount; i++)
            {
                var match = FindChild(parent.GetChild(i), objectName);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static GameObject CreateChild(Transform parent, string name)
        {
            var child = new GameObject(name, typeof(RectTransform));
            child.transform.SetParent(parent, false);
            return child;
        }

        private static RunSessionState WithRedemptionPressure(RunSessionState run, int currentPressure)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                new RedemptionPressureState(currentPressure, run.RedemptionPressure.MaxPressure),
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithCalendar(RunSessionState run, RunCalendarState calendar)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithPerformance(RunSessionState run, RunPerformanceState performance)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithOwnedAssets(RunSessionState run, OwnedAssetState ownedAssets)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                ownedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithBusinessDay(RunSessionState run, BusinessDayState businessDay)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                businessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithCurrentMarketCard(
            RunSessionState run,
            AssetCardRuntimeData runtimeCard,
            int slotIndex)
        {
            var currentMarketCards = new System.Collections.Generic.List<AssetCardRuntimeData>(run.MarketTape.CurrentMarketCards);
            currentMarketCards[slotIndex] = runtimeCard;

            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                ReplaceCard(run.AssetCards, runtimeCard),
                new MarketTapeState(
                    run.MarketTape.SellImminentCards,
                    currentMarketCards,
                    run.MarketTape.UpcomingMarketCards),
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> ReplaceCard(
            System.Collections.Generic.IEnumerable<AssetCardRuntimeData> cards,
            AssetCardRuntimeData replacement)
        {
            var updatedCards = new System.Collections.Generic.List<AssetCardRuntimeData>();
            var replaced = false;
            foreach (var card in cards)
            {
                if (card.Card.Id == replacement.Card.Id)
                {
                    updatedCards.Add(replacement);
                    replaced = true;
                }
                else
                {
                    updatedCards.Add(card);
                }
            }

            if (!replaced)
            {
                updatedCards.Add(replacement);
            }

            return updatedCards;
        }

        private static void SetCurrentRun(MainGameShellBootstrap bootstrap, RunSessionState run)
        {
            typeof(MainGameShellBootstrap)
                .GetProperty(nameof(MainGameShellBootstrap.CurrentRun), BindingFlags.Instance | BindingFlags.Public)
                .SetValue(bootstrap, run);
        }

        private static void RefreshRunUi(MainGameShellBootstrap bootstrap)
        {
            typeof(MainGameShellBootstrap)
                .GetMethod("RefreshRunUi", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(bootstrap, new object[0]);
        }

        private static Sprite CreateTestSprite()
        {
            var texture = new Texture2D(2, 2);
            texture.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), new Vector2(0.5f, 0.5f));
        }

        private static void SetPrivateSprite(object target, string fieldName, Sprite sprite)
        {
            target.GetType()
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(target, sprite);
        }

        private static void DestroyShellObjects()
        {
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var gameObject in objects)
            {
                if (!gameObject.scene.IsValid())
                {
                    continue;
                }

                if (gameObject.name == "Main Game Shell"
                    || gameObject.name == ProjectShell.GameRootName
                    || gameObject.name == ProjectShell.UiRootName)
                {
                    UnityEngine.Object.DestroyImmediate(gameObject);
                }
            }
        }

        private static System.Collections.Generic.List<string> CollectCardIds(
            System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> cards)
        {
            var cardIds = new System.Collections.Generic.List<string>();
            foreach (var card in cards)
            {
                cardIds.Add(card.Card.Id);
            }

            return cardIds;
        }

        private static int CountProfessionalCost(AssetCardData card, ResourceType resourceType)
        {
            var count = 0;
            foreach (var cost in card.ProfessionalCosts)
            {
                if (cost.ResourceType == resourceType)
                {
                    count += cost.Amount;
                }
            }

            return count;
        }

        private static System.Collections.Generic.List<string> CollectSlotCardIds(MarketTapeState tape)
        {
            var cardIds = new System.Collections.Generic.List<string>();
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty)
                {
                    cardIds.Add(slot.Card.Card.Id);
                }
            }

            return cardIds;
        }

        private static System.Collections.Generic.List<string> CollectSlotRuntimeIds(MarketTapeState tape)
        {
            var runtimeIds = new System.Collections.Generic.List<string>();
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty)
                {
                    runtimeIds.Add(slot.Card.RuntimeId);
                }
            }

            return runtimeIds;
        }

        private static int CountReservedSlots(MarketTapeState tape)
        {
            var count = 0;
            foreach (var slot in tape.Slots)
            {
                if (slot.IsReserved)
                {
                    count++;
                }
            }

            return count;
        }

        private static AssetCardRuntimeData FindFirstAvailableMarketSlotCard(MarketTapeState tape)
        {
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsReserved
                    && !slot.IsEmpty
                    && slot.Card.State == AssetCardRuntimeState.Available
                    && slot.Card.Card.CardDomain == CardDomain.Stock)
                {
                    return slot.Card;
                }
            }

            Assert.Fail("Expected to find an available stock market slot.");
            return null;
        }

        private static int FindFirstAvailableMarketSlotIndex(MarketTapeState tape)
        {
            for (var i = 0; i < tape.Slots.Count; i++)
            {
                var slot = tape.Slots[i];
                if (!slot.IsReserved
                    && !slot.IsEmpty
                    && slot.Card.State == AssetCardRuntimeState.Available
                    && slot.Card.Card.CardDomain == CardDomain.Stock)
                {
                    return i;
                }
            }

            Assert.Fail("Expected to find an available stock market slot.");
            return -1;
        }

        private static int FindFirstAvailableMarketSlotIndex(MarketTapeState tape, ResourceType requiredResourceType)
        {
            for (var i = 0; i < tape.Slots.Count; i++)
            {
                var slot = tape.Slots[i];
                if (!slot.IsReserved
                    && !slot.IsEmpty
                    && slot.Card.State == AssetCardRuntimeState.Available
                    && slot.Card.Card.CardDomain == CardDomain.Stock
                    && HasProfessionalCost(slot.Card.Card, requiredResourceType))
                {
                    return i;
                }
            }

            Assert.Fail("Expected to find an available stock market slot with " + requiredResourceType + " cost.");
            return -1;
        }

        private static int FindFirstAffordableMarketSlotIndex(MarketTapeState tape)
        {
            for (var i = 0; i < tape.Slots.Count; i++)
            {
                var slot = tape.Slots[i];
                if (!slot.IsReserved
                    && !slot.IsEmpty
                    && slot.Card.State == AssetCardRuntimeState.Available
                    && slot.Card.Card.CardDomain == CardDomain.Stock
                    && CanPayWithOneResearchAndOneCredit(slot.Card.Card))
                {
                    return i;
                }
            }

            Assert.Fail("Expected to find an available stock market slot payable with one research and one credit.");
            return -1;
        }

        private static bool CanPayWithOneResearchAndOneCredit(AssetCardData card)
        {
            var research = 0;
            var credit = 0;
            foreach (var cost in card.ProfessionalCosts)
            {
                if (cost.ResourceType == ResourceType.Reading)
                {
                    research += cost.Amount;
                }
                else if (cost.ResourceType == ResourceType.Meditation)
                {
                    credit += cost.Amount;
                }
                else
                {
                    return false;
                }
            }

            return research <= 1 && credit <= 1;
        }

        private static bool HasProfessionalCost(AssetCardData card, ResourceType resourceType)
        {
            foreach (var cost in card.ProfessionalCosts)
            {
                if (cost.ResourceType == resourceType)
                {
                    return true;
                }
            }

            return false;
        }

        private static void AssertZoneMatches(
            System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> actualCards,
            System.Collections.Generic.IReadOnlyList<string> expectedCardIds)
        {
            Assert.That(actualCards, Has.Count.EqualTo(expectedCardIds.Count));
            for (var i = 0; i < expectedCardIds.Count; i++)
            {
                Assert.That(actualCards[i].Card.Id, Is.EqualTo(expectedCardIds[i]));
            }
        }
    }
}
