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
            Assert.That(statusText.GetComponent<Text>().text, Does.Contain("남은 8영업일"));

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
            Assert.That(researchText.text, Is.EqualTo("0"));
            Assert.That(creditText.text, Is.EqualTo("0"));
            Assert.That(commodityText.text, Is.EqualTo("0"));
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
            Assert.That(updatedResearchText, Is.EqualTo("2"));

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
        public IEnumerator MainGameShellBootstrapResourceHudShowsHoldingAmountAndMastery()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapResourceHudMasteryTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var run = ResourceLedger.AddInvestmentPhilosophy(bootstrap.CurrentRun, ResourceType.Reading, 3).Run;
            run = ResourceLedger.AddInvestmentPhilosophy(run, ResourceType.Meditation, 2).Run;
            run = ResourceLedger.AddInvestmentPhilosophyMastery(run, ResourceType.Reading, 2).Run;
            SetCurrentRun(bootstrap, run);
            RefreshRunUi(bootstrap);

            yield return null;

            Assert.That(
                FindUiObject(ProjectShell.ResourceHudResearchTextName).GetComponent<Text>().text,
                Is.EqualTo("3 <size=14>+2</size>"));
            Assert.That(
                FindUiObject(ProjectShell.ResourceHudCreditTextName).GetComponent<Text>().text,
                Is.EqualTo("2"));
            Assert.That(
                FindUiObject(ProjectShell.ResourceHudCommodityTextName).GetComponent<Text>().text,
                Is.EqualTo("0"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapDealCannotDragWhenEmpty()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapDealEmptyDragTests");
            SceneManager.SetActiveScene(scene);

            var bootstrap = CreateStartedShell("Deal Empty Drag Shell");
            var resourceHud = FindUiObject(ProjectShell.UiRootName).GetComponent<ResourceHud>();
            SetPrivateSprite(resourceHud, "dealChipSprite", CreateTestSprite());
            RefreshRunUi(bootstrap);

            yield return null;

            var dealImage = FindUiObject(ProjectShell.ResourceHudDealImageName);
            PointerDown(dealImage, new Vector2(900f, 260f));
            DragPointer(dealImage, new Vector2(920f, 280f), new Vector2(20f, 20f));

            yield return null;

            Assert.That(FindUiObject(ProjectShell.ResourceHudDealGuidePanelName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.ResourceHudDealDragImageName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(0));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapDealHoverAndOutsideDropRestoresDeal()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapDealOutsideDropTests");
            SceneManager.SetActiveScene(scene);

            var bootstrap = CreateStartedShell("Deal Outside Drop Shell");
            var resourceHud = FindUiObject(ProjectShell.UiRootName).GetComponent<ResourceHud>();
            SetPrivateSprite(resourceHud, "dealChipSprite", CreateTestSprite());
            SetCurrentRun(bootstrap, ResourceLedger.AddDeal(bootstrap.CurrentRun, 1).Run);
            RefreshRunUi(bootstrap);

            yield return null;

            var dealImage = FindUiObject(ProjectShell.ResourceHudDealImageName);
            EnterPointer(dealImage);

            yield return null;

            var guidePanel = FindUiObject(ProjectShell.ResourceHudDealGuidePanelName);
            Assert.That(guidePanel.activeSelf, Is.True);
            Assert.That(
                FindUiObject(ProjectShell.ResourceHudDealGuideTextName).GetComponent<Text>().text,
                Is.EqualTo(ResourceHud.DealGuideText));

            var dragPosition = new Vector2(920f, 280f);
            PointerDown(dealImage, new Vector2(900f, 260f));
            DragPointer(dealImage, dragPosition, new Vector2(20f, 20f));

            yield return null;

            Assert.That(dealImage.GetComponent<Image>().enabled, Is.False);
            Assert.That(FindUiObject(ProjectShell.ResourceHudDealDragImageName).activeSelf, Is.True);
            Assert.That(guidePanel.GetComponent<RectTransform>().pivot, Is.EqualTo(new Vector2(1f, 0f)));
            Assert.That(
                Vector2.Distance(guidePanel.GetComponent<RectTransform>().position, dragPosition),
                Is.LessThan(0.5f));

            PointerUp(dealImage, new Vector2(100f, 100f));

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.InvestmentPhilosophyMastery.Reading, Is.EqualTo(0));
            Assert.That(dealImage.GetComponent<Image>().enabled, Is.True);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapDealDropOnReadingConsumesDealAndAddsMastery()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapDealMasteryDropTests");
            SceneManager.SetActiveScene(scene);

            var bootstrap = CreateStartedShell("Deal Mastery Drop Shell");
            var resourceHud = FindUiObject(ProjectShell.UiRootName).GetComponent<ResourceHud>();
            SetPrivateSprite(resourceHud, "dealChipSprite", CreateTestSprite());
            SetCurrentRun(bootstrap, ResourceLedger.AddDeal(bootstrap.CurrentRun, 1).Run);
            RefreshRunUi(bootstrap);

            yield return null;

            var dealImage = FindUiObject(ProjectShell.ResourceHudDealImageName);
            var readingLanePosition = GetScreenCenter(FindUiObject(ProjectShell.ResourceHudResearchTextName));
            PointerDown(dealImage, new Vector2(900f, 260f));
            DragPointer(dealImage, readingLanePosition, new Vector2(-40f, 20f));
            PointerUp(dealImage, readingLanePosition);

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(0));
            Assert.That(bootstrap.CurrentRun.InvestmentPhilosophyMastery.Reading, Is.EqualTo(1));
            Assert.That(
                FindUiObject(ProjectShell.ResourceHudResearchTextName).GetComponent<Text>().text,
                Is.EqualTo("0 <size=14>+1</size>"));
            Assert.That(FindUiObject(ProjectShell.ResourceHudDealImageName).GetComponent<Image>().enabled, Is.False);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapDealDropOnMaxMasteryShowsFailureWithoutConsumingDeal()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapDealMaxMasteryDropTests");
            SceneManager.SetActiveScene(scene);

            var bootstrap = CreateStartedShell("Deal Max Mastery Drop Shell");
            var resourceHud = FindUiObject(ProjectShell.UiRootName).GetComponent<ResourceHud>();
            SetPrivateSprite(resourceHud, "dealChipSprite", CreateTestSprite());
            var run = ResourceLedger.AddDeal(bootstrap.CurrentRun, 1).Run;
            run = ResourceLedger.AddInvestmentPhilosophyMastery(run, ResourceType.Patience, 5).Run;
            SetCurrentRun(bootstrap, run);
            RefreshRunUi(bootstrap);

            yield return null;

            var dealImage = FindUiObject(ProjectShell.ResourceHudDealImageName);
            var patienceLanePosition = GetScreenCenter(FindUiObject(ProjectShell.ResourceHudCommodityTextName));
            PointerDown(dealImage, new Vector2(900f, 260f));
            DragPointer(dealImage, patienceLanePosition, new Vector2(-40f, 20f));
            PointerUp(dealImage, patienceLanePosition);

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.InvestmentPhilosophyMastery.Patience, Is.EqualTo(5));
            Assert.That(
                FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text,
                Is.EqualTo(DealMasteryAction.MaxMasteryMessage));
            Assert.That(FindUiObject(ProjectShell.ResourceHudDealImageName).GetComponent<Image>().enabled, Is.True);

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

            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(4));
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.Empty);

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
                Assert.That(firstCurrentMarketButtonText, Does.Contain(firstCurrentMarketCard.Tags[0].DisplayName));
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

            var selectedSlotIndex = 0;
            var selectedRuntimeCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "single-market-state-stock",
                    "Single Market State Stock",
                    "Single market state test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedRuntimeCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            var selectedCard = selectedRuntimeCard.Card;
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
            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.True);
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
        public IEnumerator MainGameShellBootstrapMarketCardHoverPositionsCardsOneToSixToTheRight()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapMarketHoverRightPositionTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1");
            var hoverPanel = FindUiObject(ProjectShell.MarketCardHoverPanelName);
            var buttonRect = cardButtonObject.GetComponent<RectTransform>();
            var hoverRect = hoverPanel.GetComponent<RectTransform>();

            EnterPointer(cardButtonObject);
            yield return null;

            Assert.That(hoverPanel.activeSelf, Is.True);
            Assert.That(hoverRect.position, Is.EqualTo(buttonRect.position + new Vector3(300f, 0f, 0f)));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapMarketCardHoverPositionsCardsSevenAndEightToTheLeft()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapMarketHoverLeftPositionTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "8");
            var hoverPanel = FindUiObject(ProjectShell.MarketCardHoverPanelName);
            var buttonRect = cardButtonObject.GetComponent<RectTransform>();
            var hoverRect = hoverPanel.GetComponent<RectTransform>();

            EnterPointer(cardButtonObject);
            yield return null;

            Assert.That(hoverPanel.activeSelf, Is.True);
            Assert.That(hoverRect.position, Is.EqualTo(buttonRect.position + new Vector3(-300f, 0f, 0f)));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapMarketCardDragHidesHoverAndMovesOriginalCard()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapMarketDragTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape);
            var currentMarketButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            var hoverPanel = FindUiObject(ProjectShell.MarketCardHoverPanelName);
            var buttonRect = currentMarketButtonObject.GetComponent<RectTransform>();
            var originalPosition = buttonRect.anchoredPosition;

            EnterPointer(currentMarketButtonObject);
            yield return null;

            Assert.That(hoverPanel.activeSelf, Is.True);

            PointerDown(currentMarketButtonObject, new Vector2(500f, 500f));
            DragPointer(currentMarketButtonObject, new Vector2(500f, 420f), new Vector2(0f, -80f));
            yield return null;

            Assert.That(hoverPanel.activeSelf, Is.False);
            Assert.That(buttonRect.anchoredPosition, Is.EqualTo(originalPosition + new Vector2(0f, -80f)));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapMarketCardHoverUsesSlotPositionAfterDragEnds()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapMarketHoverAfterDragTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "hover-after-drag-stock",
                    "Hover After Drag Stock",
                    "Hover after drag test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1");
            var hoverPanel = FindUiObject(ProjectShell.MarketCardHoverPanelName);
            var buttonRect = cardButtonObject.GetComponent<RectTransform>();
            var hoverRect = hoverPanel.GetComponent<RectTransform>();
            var originalButtonPosition = buttonRect.position;

            PointerDown(cardButtonObject, new Vector2(500f, 500f));
            DragPointer(cardButtonObject, new Vector2(500f, 460f), new Vector2(0f, -40f));
            PointerUp(cardButtonObject, new Vector2(500f, 460f));
            yield return null;

            FindUiObject(ProjectShell.PurchaseConfirmationBackButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            EnterPointer(cardButtonObject);
            yield return null;

            Assert.That(hoverPanel.activeSelf, Is.True);
            Assert.That(buttonRect.position, Is.EqualTo(originalButtonPosition));
            Assert.That(hoverRect.position, Is.EqualTo(buttonRect.position + new Vector3(300f, 0f, 0f)));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapMarketCardDropOnPortfolioImmediatelyPurchasesWithoutModal()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPortfolioDropPurchaseTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "portfolio-drop-stock",
                    "Portfolio Drop Stock",
                    "Portfolio drop purchase test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            var portfolioDropPosition = GetScreenCenter(FindUiObject(ProjectShell.PortfolioSummaryPanelName));
            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;

            PointerDown(cardButtonObject, new Vector2(500f, 500f));
            DragPointer(cardButtonObject, portfolioDropPosition, new Vector2(0f, -260f));
            PointerUp(cardButtonObject, portfolioDropPosition);
            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays - 1));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapSmallPointerMovementOnStockCardOpensPurchaseConfirmationModal()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapStockSmallMovementClickTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "small-movement-stock",
                    "Small Movement Stock",
                    "Small movement click test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            var buttonRect = cardButtonObject.GetComponent<RectTransform>();
            var originalPosition = buttonRect.anchoredPosition;
            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;

            PointerDown(cardButtonObject, new Vector2(500f, 500f));
            DragPointer(cardButtonObject, new Vector2(500f, 496f), new Vector2(0f, -4f));
            PointerUp(cardButtonObject, new Vector2(500f, 496f));
            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.True);
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard.RuntimeId, Is.EqualTo(selectedCard.RuntimeId));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Is.Empty);
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(buttonRect.anchoredPosition, Is.EqualTo(originalPosition));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapSmallPointerMovementOnConsumableResourceCardPurchasesImmediately()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapConsumableSmallMovementClickTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var resourceCard = new AssetCardData(
                "small-movement-resource",
                "Small Movement Resource",
                "Small movement resource click test card.",
                AssetRarity.Common,
                1,
                new ProfessionalResourceCost[0],
                0,
                0,
                new TagData[0],
                cardDomain: CardDomain.ConsumableResource,
                providedResourceType: ResourceType.Patience,
                providedResourceAmount: 2);
            var selectedCard = new AssetCardRuntimeData(
                resourceCard,
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            var cashBeforePurchase = bootstrap.CurrentRun.Resources.Cash;
            var patienceBeforePurchase = bootstrap.CurrentRun.Resources.Patience;

            PointerDown(cardButtonObject, new Vector2(500f, 500f));
            DragPointer(cardButtonObject, new Vector2(500f, 496f), new Vector2(0f, -4f));
            PointerUp(cardButtonObject, new Vector2(500f, 496f));
            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(cashBeforePurchase - resourceCard.CashCost));
            Assert.That(bootstrap.CurrentRun.Resources.Patience, Is.EqualTo(patienceBeforePurchase + resourceCard.ProvidedResourceAmount));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Is.Empty);
            Assert.That(CollectSlotRuntimeIds(bootstrap.CurrentRun.MarketTape), Does.Not.Contain(selectedCard.RuntimeId));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapMarketCardDragReleaseOutsidePortfolioIsNoOp()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapDragReleaseOutsideNoOpTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "drag-release-modal-stock",
                    "Drag Release Modal Stock",
                    "Drag release modal test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            var buttonRect = cardButtonObject.GetComponent<RectTransform>();
            var originalPosition = buttonRect.anchoredPosition;
            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var cash = bootstrap.CurrentRun.Resources.Cash;

            PointerDown(cardButtonObject, new Vector2(500f, 500f));
            DragPointer(cardButtonObject, new Vector2(500f, 460f), new Vector2(0f, -40f));
            PointerUp(cardButtonObject, new Vector2(500f, 460f));
            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard, Is.Null);
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Is.Empty);
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(cash));
            Assert.That(bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].Card.RuntimeId, Is.EqualTo(selectedCard.RuntimeId));
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.Empty);
            Assert.That(cardButtonObject.GetComponent<MarketCardFailureFeedback>().ShakeRequestCount, Is.EqualTo(0));
            Assert.That(buttonRect.anchoredPosition, Is.EqualTo(originalPosition));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapFailedPortfolioDropRestoresMarketCardAndShowsFailureFeedback()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPortfolioDropFailureTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "portfolio-drop-failure-stock",
                    "Portfolio Drop Failure Stock",
                    "Portfolio drop failure test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            var run = WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex);
            run = WithOwnedAssets(run, new OwnedAssetState(CreateOwnedStockCards(8)));
            SetCurrentRun(bootstrap, run);
            RefreshRunUi(bootstrap);

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            var buttonRect = cardButtonObject.GetComponent<RectTransform>();
            var originalPosition = buttonRect.anchoredPosition;
            var portfolioDropPosition = GetScreenCenter(FindUiObject(ProjectShell.PortfolioSummaryPanelName));

            PointerDown(cardButtonObject, new Vector2(500f, 500f));
            DragPointer(cardButtonObject, portfolioDropPosition, new Vector2(0f, -260f));
            PointerUp(cardButtonObject, portfolioDropPosition);
            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(8));
            Assert.That(bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].Card.RuntimeId, Is.EqualTo(selectedCard.RuntimeId));
            Assert.That(cardButtonObject.activeSelf, Is.True);
            Assert.That(buttonRect.anchoredPosition, Is.EqualTo(originalPosition));
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.EqualTo("주식 매도가 필요합니다"));
            Assert.That(cardButtonObject.GetComponent<MarketCardFailureFeedback>().ShakeRequestCount, Is.EqualTo(1));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapAffordableMarketCardClickOpensPurchaseConfirmationModal()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPurchaseConfirmationOpenTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "confirmation-open-stock",
                    "Confirmation Open Stock",
                    "Confirmation modal opening test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var cardButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            var cardText = cardButton.GetComponentInChildren<Text>().text;

            cardButton.GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.True);
            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationCardTextName).GetComponent<Text>().text, Does.Contain(cardText));
            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationConfirmButtonName).activeSelf, Is.True);
            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationBackButtonName).activeSelf, Is.True);
            Assert.That(FindUiObject(ProjectShell.CardDetailPanelName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Is.Empty);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapConsumableResourceCardClickPurchasesImmediatelyWithoutConfirmationModal()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapConsumableImmediatePurchaseTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var resourceCard = new AssetCardData(
                "confirmation-skip-resource",
                "Confirmation Skip Resource",
                "Consumable resource confirmation skip test card.",
                AssetRarity.Common,
                1,
                new ProfessionalResourceCost[0],
                0,
                0,
                new TagData[0],
                cardDomain: CardDomain.ConsumableResource,
                providedResourceType: ResourceType.Patience,
                providedResourceAmount: 2);
            var selectedCard = new AssetCardRuntimeData(
                resourceCard,
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            var cashBeforePurchase = bootstrap.CurrentRun.Resources.Cash;
            var patienceBeforePurchase = bootstrap.CurrentRun.Resources.Patience;

            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();
            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(cashBeforePurchase - resourceCard.CashCost));
            Assert.That(bootstrap.CurrentRun.Resources.Patience, Is.EqualTo(patienceBeforePurchase + resourceCard.ProvidedResourceAmount));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(7));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Is.Empty);
            Assert.That(CollectSlotRuntimeIds(bootstrap.CurrentRun.MarketTape), Does.Not.Contain(selectedCard.RuntimeId));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapPurchaseConfirmationConfirmBuysSelectedCard()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPurchaseConfirmationConfirmTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "confirmation-confirm-stock",
                    "Confirmation Confirm Stock",
                    "Confirmation modal confirm test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();
            yield return null;

            FindUiObject(ProjectShell.PurchaseConfirmationConfirmButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(7));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapPurchaseConfirmationShowsDiscountedAndInsufficientCosts()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPurchaseConfirmationCostTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "confirmation-cost-stock",
                    "Confirmation Cost Stock",
                    "Confirmation modal cost test card.",
                    AssetRarity.Common,
                    0,
                    new[] { new ProfessionalResourceCost(ResourceType.Reading, 2) },
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            var run = WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex);
            run = WithResources(run, new ResourceState(run.Resources.Cash, 1, 0, 0, run.Resources.Deal));
            run = WithMastery(run, new InvestmentPhilosophyMasteryState(1, 0, 0));
            SetCurrentRun(bootstrap, run);
            RefreshRunUi(bootstrap);

            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();
            yield return null;

            var modalText = FindUiObject(ProjectShell.PurchaseConfirmationCardTextName).GetComponent<Text>().text;
            Assert.That(modalText, Does.Contain("비용 $0, R2 -> R1"));

            SetCurrentRun(bootstrap, WithResources(bootstrap.CurrentRun, new ResourceState(bootstrap.CurrentRun.Resources.Cash, 0, 0, 0, bootstrap.CurrentRun.Resources.Deal)));
            RefreshRunUi(bootstrap);
            yield return null;

            Assert.That(
                FindUiObject(ProjectShell.PurchaseConfirmationCardTextName).GetComponent<Text>().text,
                Does.Contain("<color=red>R1</color>"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapPurchaseConfirmationBackCancelsWithoutChangingRun()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPurchaseConfirmationBackTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "confirmation-cancel-stock",
                    "Confirmation Cancel Stock",
                    "Confirmation modal cancel test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var cash = bootstrap.CurrentRun.Resources.Cash;
            var slotRuntimeIds = CollectSlotRuntimeIds(bootstrap.CurrentRun.MarketTape);

            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();
            yield return null;

            FindUiObject(ProjectShell.PurchaseConfirmationBackButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard, Is.Null);
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(cash));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Is.Empty);
            Assert.That(CollectSlotRuntimeIds(bootstrap.CurrentRun.MarketTape), Is.EqualTo(slotRuntimeIds));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapPurchaseConfirmationRevalidatesAndFailsWithCardFeedback()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPurchaseConfirmationRevalidationTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "confirmation-revalidation-stock",
                    "Confirmation Revalidation Stock",
                    "Confirmation modal revalidation test card.",
                    AssetRarity.Common,
                    1,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            var run = WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex);
            run = WithResources(run, new ResourceState(1, run.Resources.Reading, run.Resources.Meditation, run.Resources.Patience, run.Resources.Deal));
            SetCurrentRun(bootstrap, run);
            RefreshRunUi(bootstrap);

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            cardButtonObject.GetComponent<Button>().onClick.Invoke();
            yield return null;

            SetCurrentRun(
                bootstrap,
                WithResources(bootstrap.CurrentRun, new ResourceState(0, 0, 0, 0, bootstrap.CurrentRun.Resources.Deal)));
            RefreshRunUi(bootstrap);

            FindUiObject(ProjectShell.PurchaseConfirmationConfirmButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Is.Empty);
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.Empty);
            Assert.That(cardButtonObject.GetComponent<MarketCardFailureFeedback>().ShakeRequestCount, Is.EqualTo(1));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapPurchaseConfirmationBlocksBackgroundButtons()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPurchaseConfirmationBlockTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "confirmation-block-stock",
                    "Confirmation Block Stock",
                    "Confirmation modal background block test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();
            yield return null;

            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var cash = bootstrap.CurrentRun.Resources.Cash;
            var otherSlotIndex = selectedSlotIndex + 1;

            FindUiObject(ProjectShell.NextBusinessDayButtonName).GetComponent<Button>().onClick.Invoke();
            FindUiObject(ProjectShell.ResourceDevFundingCashButtonName).GetComponent<Button>().onClick.Invoke();
            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (otherSlotIndex + 1))
                .GetComponent<Button>()
                .onClick
                .Invoke();
            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.True);
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(cash));
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard.RuntimeId, Is.EqualTo(selectedCard.RuntimeId));

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
        public IEnumerator MainGameShellBootstrapMarketPurchaseAutomaticallyPaysWithoutPaymentPotControls()
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

            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentPotBackgroundName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentSlotsTextName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailFinalCashCostTextName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPlaceResearchButtonName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPlaceCreditButtonName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPlaceCommodityButtonName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPlaceDealButtonName).activeSelf, Is.False);

            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(previousCash));

            bootstrap.ConfirmPurchase();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(7));
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
            var saleDropZone = FindUiObject(ProjectShell.PortfolioStockSaleDropZoneName);
            var saleDropText = FindUiObject(ProjectShell.PortfolioStockSaleDropZoneTextName).GetComponent<Text>();
            var saleDropImage = saleDropZone.GetComponent<Image>();
            var ownedDragPanel = FindUiObject(ProjectShell.OwnedStockDragDetailPanelName);
            var ownedDragText = FindUiObject(ProjectShell.OwnedStockDragDetailTextName).GetComponent<Text>();
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
            Assert.That(saleDropZone.activeSelf, Is.True);
            Assert.That(saleDropText.text, Is.EqualTo("$"));
            Assert.That(saleDropImage.color.r, Is.GreaterThan(saleDropImage.color.g));
            Assert.That(saleDropImage.color.r, Is.GreaterThan(saleDropImage.color.b));
            Assert.That(ownedDragPanel.activeSelf, Is.False);

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
            Assert.That(firstSellButton.gameObject.activeSelf, Is.False);

            var outsideDropPosition = new Vector2(1800f, 900f);
            PointerDown(firstOwnedStockCardButton, new Vector2(500f, 500f));
            DragPointer(firstOwnedStockCardButton, outsideDropPosition, new Vector2(1300f, 400f));
            yield return null;

            Assert.That(firstOwnedStockCard.GetComponent<CanvasGroup>().alpha, Is.EqualTo(0f));
            Assert.That(ownedDragPanel.activeSelf, Is.True);
            Assert.That(ownedDragText.text, Does.Contain(selectedCard.Card.DisplayName));
            Assert.That(ownedDragText.text, Does.Not.Contain("$"));
            Assert.That(saleDropText.text, Is.EqualTo("+1"));
            Assert.That(ownedDragPanel.GetComponent<RectTransform>().pivot, Is.EqualTo(new Vector2(0f, 0f)));
            Assert.That(
                Vector2.Distance(ownedDragPanel.GetComponent<RectTransform>().position, outsideDropPosition),
                Is.LessThan(0.5f));

            PointerUp(firstOwnedStockCardButton, outsideDropPosition);
            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(cashBeforeSale));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(quarterRevenueBeforeSale));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.Count, Is.EqualTo(1));
            Assert.That(firstOwnedStockCard.GetComponent<CanvasGroup>().alpha, Is.EqualTo(1f));
            Assert.That(ownedDragPanel.activeSelf, Is.False);
            Assert.That(saleDropText.text, Is.EqualTo("$"));

            var saleDropPosition = GetScreenCenter(saleDropZone);
            PointerDown(firstOwnedStockCardButton, new Vector2(500f, 500f));
            DragPointer(firstOwnedStockCardButton, saleDropPosition, saleDropPosition - new Vector2(500f, 500f));
            yield return null;
            Assert.That(firstOwnedStockCard.GetComponent<CanvasGroup>().alpha, Is.EqualTo(0f));
            Assert.That(saleDropText.text, Is.EqualTo("+1"));

            PointerUp(firstOwnedStockCardButton, saleDropPosition);
            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(cashBeforeSale + 1));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(quarterRevenueBeforeSale + 1));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(7));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.Count, Is.EqualTo(0));
            Assert.That(FindUiObject("Owned Stock Card 1").activeSelf, Is.False);
            Assert.That(firstSellButton.gameObject.activeSelf, Is.False);
            Assert.That(ownedDragPanel.activeSelf, Is.False);
            Assert.That(saleDropText.text, Is.EqualTo("$"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapPurchaseCostShortageShakesCardWithoutSystemMessage()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPurchaseCostShortageFeedbackTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "cost-shortage-feedback-stock",
                    "Cost Shortage Feedback Stock",
                    "Cost shortage feedback test card.",
                    AssetRarity.Common,
                    bootstrap.CurrentRun.Resources.Cash + 1,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            cardButtonObject.GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Is.Empty);
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.Empty);
            Assert.That(cardButtonObject.GetComponent<MarketCardFailureFeedback>().ShakeRequestCount, Is.EqualTo(1));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapNonCostPurchaseFailureShakesCardAndShowsMessage()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPurchaseNonCostFailureFeedbackTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "portfolio-full-feedback-stock",
                    "Portfolio Full Feedback Stock",
                    "Portfolio full feedback test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            var run = WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex);
            run = WithOwnedAssets(run, new OwnedAssetState(CreateOwnedStockCards(8)));
            SetCurrentRun(bootstrap, run);
            RefreshRunUi(bootstrap);

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            cardButtonObject.GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(8));
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.EqualTo("주식 매도가 필요합니다"));
            Assert.That(cardButtonObject.GetComponent<MarketCardFailureFeedback>().ShakeRequestCount, Is.EqualTo(1));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapSuccessfulPurchaseDoesNotShakeCard()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapPurchaseSuccessFeedbackTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = 0;
            var selectedCard = new AssetCardRuntimeData(
                new AssetCardData(
                    "success-feedback-stock",
                    "Success Feedback Stock",
                    "Success feedback test card.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    2,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            SetCurrentRun(bootstrap, WithCurrentMarketCard(bootstrap.CurrentRun, selectedCard, selectedSlotIndex));
            RefreshRunUi(bootstrap);

            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            cardButtonObject.GetComponent<Button>().onClick.Invoke();
            yield return null;

            bootstrap.ConfirmPurchase();
            yield return null;

            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(cardButtonObject.GetComponent<MarketCardFailureFeedback>().ShakeRequestCount, Is.EqualTo(0));

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

            FindUiObject(ProjectShell.PurchaseConfirmationConfirmButtonName).GetComponent<Button>().onClick.Invoke();
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

            var secondCardButton = FindUiObject("Owned Stock Card 2 Card Button");
            EnterPointer(secondCardButton);
            yield return null;

            var secondSellButton = FindUiObject("Owned Stock Card 2 Sell Button").GetComponent<Button>();
            Assert.That(secondSellButton.gameObject.activeSelf, Is.False);

            var saleDropPosition = GetScreenCenter(FindUiObject(ProjectShell.PortfolioStockSaleDropZoneName));
            PointerDown(secondCardButton, new Vector2(500f, 500f));
            DragPointer(secondCardButton, saleDropPosition, saleDropPosition - new Vector2(500f, 500f));
            PointerUp(secondCardButton, saleDropPosition);
            yield return null;

            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(stock.Id));
            Assert.That(FindUiObject("Owned Stock Card 1").activeSelf, Is.True);
            Assert.That(FindUiObject("Owned Stock Card 2").activeSelf, Is.False);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapDoesNotExposePaymentPotOrManualCostSlotsInNewPlayPath()
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
            var selectedCard = bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].Card;
            OpenLegacyCardDetailForTest(bootstrap, selectedCard);
            yield return null;

            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentPotBackgroundName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentSlotsTextName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailFinalCashCostTextName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentSlotButtonPrefix + "1").activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPlaceResearchButtonName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPlaceDealButtonName).activeSelf, Is.False);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapDealIsNotExposedAsPurchasePayment()
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
            run = ResourceLedger.AddInvestmentPhilosophy(run, ResourceType.Reading, 1).Run;
            SetCurrentRun(bootstrap, run);

            var selectedSlotIndex = FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape, ResourceType.Reading);
            var selectedCard = bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].Card;
            bootstrap.OpenMarketCardDetail(selectedCard);

            yield return null;

            var dealBeforePurchase = bootstrap.CurrentRun.Resources.Deal;

            yield return null;

            Assert.That(FindUiObject(ProjectShell.CardDetailPlaceDealButtonName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.CardDetail.PendingPayment.FinalCashCost, Is.EqualTo(selectedCard.Card.CashCost + 1));

            bootstrap.ConfirmPurchase();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(dealBeforePurchase));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapStockHoverShowsReserveButtonAndConsumableHoverDoesNot()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapReservationButtonStockOnlyTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var stock = new AssetCardRuntimeData(
                new AssetCardData(
                    "hover-reserve-stock",
                    "Hover Reserve Stock",
                    "Hover reserve stock test.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    1,
                    0,
                    new TagData[0]),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            var resource = new AssetCardRuntimeData(
                new AssetCardData(
                    "hover-reserve-resource",
                    string.Empty,
                    "Hover reserve resource test.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    0,
                    0,
                    new TagData[0],
                    cardDomain: CardDomain.ConsumableResource,
                    providedResourceType: ResourceType.Cash,
                    providedResourceAmount: 1),
                AssetCardRuntimeState.Available,
                PurchaseSource.MarketTape);
            var run = WithCurrentMarketCard(bootstrap.CurrentRun, stock, 0);
            run = WithCurrentMarketCard(run, resource, 1);
            SetCurrentRun(bootstrap, run);
            RefreshRunUi(bootstrap);

            var stockButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1");
            var resourceButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "2");
            var stockReserveButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketReserveButtonPrefix + "1");
            var resourceReserveButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketReserveButtonPrefix + "2");

            Assert.That(stockReserveButton.activeSelf, Is.False);
            Assert.That(resourceReserveButton.activeSelf, Is.False);

            EnterPointer(stockButton);

            yield return null;

            Assert.That(stockReserveButton.activeSelf, Is.True);

            ExitPointer(stockButton);
            EnterPointer(resourceButton);

            yield return null;

            Assert.That(stockReserveButton.activeSelf, Is.False);
            Assert.That(resourceReserveButton.activeSelf, Is.False);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapReserveButtonLocksMarketSlotWithoutPurchasePropagation()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapReserveButtonClickTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape);
            var selectedCard = bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].Card;
            var previousSlotIds = CollectSlotCardIds(bootstrap.CurrentRun.MarketTape);
            var previousRemainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var previousDeal = bootstrap.CurrentRun.Resources.Deal;
            var previousPressure = bootstrap.CurrentRun.RedemptionPressure.CurrentPressure;
            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            var cardRect = cardButtonObject.GetComponent<RectTransform>();
            var normalPosition = cardRect.anchoredPosition;
            var reserveButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketReserveButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>();

            EnterPointer(cardButtonObject);
            reserveButton.onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard, Is.Null);
            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.False);
            Assert.That(bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].Card.Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].IsReserved, Is.True);
            Assert.That(CountReservedSlots(bootstrap.CurrentRun.MarketTape), Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(previousRemainingBusinessDays));
            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(previousDeal));
            Assert.That(bootstrap.CurrentRun.RedemptionPressure.CurrentPressure, Is.EqualTo(previousPressure));
            Assert.That(bootstrap.CurrentRun.Reservation.ReservedCards, Is.Empty);
            Assert.That(CollectSlotCardIds(bootstrap.CurrentRun.MarketTape), Is.EqualTo(previousSlotIds));
            Assert.That(cardRect.anchoredPosition.y, Is.LessThan(normalPosition.y));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapReservedCardHoverKeepsUnreserveButtonWhilePointerMovesToButton()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapUnreserveButtonHoverTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape);
            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            var reserveButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketReserveButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>();

            EnterPointer(cardButtonObject);
            reserveButton.onClick.Invoke();

            yield return null;

            var unreserveButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketUnreserveButtonPrefix + (selectedSlotIndex + 1));
            var unreserveButton = unreserveButtonObject.GetComponent<Button>();
            Assert.That(unreserveButtonObject.activeSelf, Is.False);

            EnterPointer(cardButtonObject);

            yield return null;

            Assert.That(unreserveButtonObject.activeSelf, Is.True);

            EnterPointer(unreserveButtonObject);
            ExitPointer(cardButtonObject);

            yield return null;

            Assert.That(unreserveButtonObject.activeSelf, Is.True);

            unreserveButton.onClick.Invoke();

            yield return null;

            Assert.That(CountReservedSlots(bootstrap.CurrentRun.MarketTape), Is.EqualTo(0));
            Assert.That(bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].IsReserved, Is.False);
            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.False);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapReservedCardUsesLoweredPositionForHoverAndOutsideDragRestore()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapReservedLoweredInteractionTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedSlotIndex = FindFirstAvailableMarketSlotIndex(bootstrap.CurrentRun.MarketTape);
            var cardButtonObject = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + (selectedSlotIndex + 1));
            var cardRect = cardButtonObject.GetComponent<RectTransform>();
            var normalPosition = cardRect.anchoredPosition;
            var reserveButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketReserveButtonPrefix + (selectedSlotIndex + 1))
                .GetComponent<Button>();

            EnterPointer(cardButtonObject);
            reserveButton.onClick.Invoke();

            yield return null;

            var loweredPosition = cardRect.anchoredPosition;
            Assert.That(loweredPosition.y, Is.LessThan(normalPosition.y));

            var hoverPanel = FindUiObject(ProjectShell.MarketCardHoverPanelName);
            var hoverRect = hoverPanel.GetComponent<RectTransform>();
            EnterPointer(cardButtonObject);

            yield return null;

            Assert.That(hoverPanel.activeSelf, Is.True);
            Assert.That(hoverRect.position, Is.EqualTo(cardRect.position + new Vector3(300f, 0f, 0f)));

            PointerDown(cardButtonObject, new Vector2(500f, 500f));
            DragPointer(cardButtonObject, new Vector2(500f, 460f), new Vector2(0f, -40f));
            PointerUp(cardButtonObject, new Vector2(500f, 460f));

            yield return null;

            Assert.That(FindUiObject(ProjectShell.PurchaseConfirmationPanelName).activeSelf, Is.False);
            Assert.That(cardRect.anchoredPosition, Is.EqualTo(loweredPosition));
            Assert.That(bootstrap.CurrentRun.MarketTape.Slots[selectedSlotIndex].IsReserved, Is.True);

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

            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(7));

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
            for (var i = 0; i < 8; i++)
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
                for (var day = 0; day < 8; day++)
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

        private static void PointerDown(GameObject target, Vector2 position)
        {
            ExecutePointerEvent(target, ExecuteEvents.pointerDownHandler, position, Vector2.zero);
        }

        private static void DragPointer(GameObject target, Vector2 position, Vector2 delta)
        {
            ExecutePointerEvent(target, ExecuteEvents.dragHandler, position, delta);
        }

        private static void PointerUp(GameObject target, Vector2 position)
        {
            ExecutePointerEvent(target, ExecuteEvents.pointerUpHandler, position, Vector2.zero);
        }

        private static Vector2 GetScreenCenter(GameObject target)
        {
            var rectTransform = target.GetComponent<RectTransform>();
            Assert.That(rectTransform, Is.Not.Null);
            return RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
        }

        private static MainGameShellBootstrap CreateStartedShell(string shellName)
        {
            var shell = new GameObject(shellName);
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);
            return bootstrap;
        }

        private static void ExecutePointerEvent<T>(GameObject target, ExecuteEvents.EventFunction<T> eventFunction)
            where T : IEventSystemHandler
        {
            Assert.That(EventSystem.current, Is.Not.Null);
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), eventFunction);
        }

        private static void ExecutePointerEvent<T>(
            GameObject target,
            ExecuteEvents.EventFunction<T> eventFunction,
            Vector2 position,
            Vector2 delta)
            where T : IEventSystemHandler
        {
            Assert.That(EventSystem.current, Is.Not.Null);
            ExecuteEvents.Execute(
                target,
                new PointerEventData(EventSystem.current)
                {
                    position = position,
                    delta = delta
                },
                eventFunction);
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

        private static RunSessionState WithResources(RunSessionState run, ResourceState resources)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery);
        }

        private static RunSessionState WithMastery(RunSessionState run, InvestmentPhilosophyMasteryState mastery)
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
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                mastery);
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

        private static System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> CreateOwnedStockCards(int count)
        {
            var cards = new System.Collections.Generic.List<AssetCardRuntimeData>();
            for (var i = 0; i < count; i++)
            {
                var card = new AssetCardData(
                    "owned-feedback-stock-" + i,
                    "Owned Feedback Stock " + i,
                    "Owned stock for feedback tests.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    1,
                    0,
                    new TagData[0]);
                cards.Add(new AssetCardRuntimeData(card, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, i + 1));
            }

            return cards;
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

        private static void OpenLegacyCardDetailForTest(MainGameShellBootstrap bootstrap, AssetCardRuntimeData card)
        {
            SetCurrentRun(bootstrap, MarketAreaFlow.OpenMarketCardDetail(bootstrap.CurrentRun, card));
            RefreshRunUi(bootstrap);
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
