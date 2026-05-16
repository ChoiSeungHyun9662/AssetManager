using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace AssetManager.Tests
{
    public sealed class MainGameShellBootstrapTests
    {
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
        public IEnumerator MainGameShellBootstrapKeepsMarketOverlaysUnderUiRoot()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapKeepsMarketOverlaysUnderUiRootTests");
            SceneManager.SetActiveScene(scene);

            var uiRoot = new GameObject(ProjectShell.UiRootName);
            var marketPanel = new GameObject(ProjectShell.MarketAreaMarketPanelName, typeof(RectTransform));
            marketPanel.transform.SetParent(uiRoot.transform, false);
            CreateChild(marketPanel.transform, ProjectShell.ReservationPanelName);
            CreateChild(marketPanel.transform, ProjectShell.CentralBankButtonName);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var updatedUiRoot = GameObject.Find(ProjectShell.UiRootName).transform;
            var updatedMarketPanel = updatedUiRoot.Find(ProjectShell.MarketAreaMarketPanelName);

            Assert.That(updatedUiRoot.Find(ProjectShell.ReservationPanelName), Is.Not.Null);
            Assert.That(updatedUiRoot.Find(ProjectShell.CentralBankButtonName), Is.Not.Null);
            Assert.That(updatedMarketPanel.Find(ProjectShell.ReservationPanelName), Is.Null);
            Assert.That(updatedMarketPanel.Find(ProjectShell.CentralBankButtonName), Is.Null);

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapKeepsResourcesOutOfTopStatusAndShowsObjectChoices()
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
            Assert.That(statusText, Does.Not.Contain("리서치"));
            Assert.That(statusText, Does.Not.Contain("신용"));
            Assert.That(statusText, Does.Not.Contain("원자재"));
            Assert.That(statusText, Does.Not.Contain("딜"));

            FindUiObject(ProjectShell.CentralBankButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(FindUiObject(ProjectShell.LiquidityActionCashButtonName).GetComponentInChildren<Text>().text, Does.Contain("지폐다발"));
            Assert.That(FindUiObject(ProjectShell.LiquidityActionResearchButtonName).GetComponentInChildren<Text>().text, Does.Contain("칩"));
            Assert.That(FindUiObject(ProjectShell.LiquidityActionCreditButtonName).GetComponentInChildren<Text>().text, Does.Contain("칩"));
            Assert.That(FindUiObject(ProjectShell.LiquidityActionCommodityButtonName).GetComponentInChildren<Text>().text, Does.Contain("칩"));
            Assert.That(
                FindChild(GameObject.Find(ProjectShell.UiRootName).transform, "Liquidity Action Deal Button"),
                Is.Null);

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

            Assert.That(bootstrap.CurrentRun.Resources.Research, Is.EqualTo(10));
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.EqualTo("자원칩 최대 보유: 리서치 +1 폐기"));

            var dealButton = FindUiObject(ProjectShell.ResourceDevDealButtonName).GetComponent<Button>();
            for (var i = 0; i < 4; i++)
            {
                dealButton.onClick.Invoke();
            }

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(3));
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.EqualTo("딜 최대 보유: 추가 딜 폐기"));

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
        public IEnumerator MainGameShellBootstrapMarketCardClickShowsCardDetailUntilClosed()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapCardDetailTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedCard = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0].Card;
            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var marketPanel = FindUiObject(ProjectShell.MarketAreaMarketPanelName);
            var cardDetailPanel = FindUiObject(ProjectShell.CardDetailPanelName);
            var currentMarketButton = FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1")
                .GetComponent<Button>();
            var nextBusinessDayButton = FindUiObject(ProjectShell.NextBusinessDayButtonName)
                .GetComponent<Button>();

            Assert.That(marketPanel.activeSelf, Is.True);
            Assert.That(cardDetailPanel.activeSelf, Is.False);
            Assert.That(nextBusinessDayButton.interactable, Is.True);

            currentMarketButton.onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard.Card.Id, Is.EqualTo(selectedCard.Id));
            Assert.That(marketPanel.activeSelf, Is.False);
            Assert.That(cardDetailPanel.activeSelf, Is.True);
            Assert.That(nextBusinessDayButton.interactable, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailNameTextName).GetComponent<Text>().text, Does.Contain(selectedCard.DisplayName));
            Assert.That(FindUiObject(ProjectShell.CardDetailReserveButtonName).activeSelf, Is.True);

            FindUiObject(ProjectShell.CardDetailCloseButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard, Is.Null);
            Assert.That(marketPanel.activeSelf, Is.True);
            Assert.That(cardDetailPanel.activeSelf, Is.False);
            Assert.That(nextBusinessDayButton.interactable, Is.True);

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

            var selectedCard = bootstrap.CurrentRun.MarketTape.UpcomingMarketCards[0].Card;

            FindUiObject(ProjectShell.MarketTapeUpcomingMarketCardButtonPrefix + "1")
                .GetComponent<Button>()
                .onClick
                .Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard.Card.Id, Is.EqualTo(selectedCard.Id));
            Assert.That(FindUiObject(ProjectShell.CardDetailPanelName).activeSelf, Is.True);
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

            var selectedCard = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0];
            var previousCash = bootstrap.CurrentRun.Resources.Cash;
            var previousCurrentMarketSecondCardId = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[1].Card.Id;
            var previousUpcomingMarketFirstCardId = bootstrap.CurrentRun.MarketTape.UpcomingMarketCards[0].Card.Id;
            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1")
                .GetComponent<Button>()
                .onClick
                .Invoke();

            yield return null;

            var buyButton = FindUiObject(ProjectShell.CardDetailBuyButtonName).GetComponent<Button>();
            Assert.That(buyButton.interactable, Is.False);
            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentSlotsTextName).GetComponent<Text>().text, Does.Contain("리서치: 비어 있음"));

            FindUiObject(ProjectShell.CardDetailPlaceResearchButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Research, Is.EqualTo(1));
            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentSlotsTextName).GetComponent<Text>().text, Does.Contain("리서치: 리서치"));

            FindUiObject(ProjectShell.CardDetailPaymentSlotButtonPrefix + "1").GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Research, Is.EqualTo(1));
            Assert.That(FindUiObject(ProjectShell.CardDetailPaymentSlotsTextName).GetComponent<Text>().text, Does.Contain("리서치: 비어 있음"));

            FindUiObject(ProjectShell.CardDetailPlaceResearchButtonName).GetComponent<Button>().onClick.Invoke();
            FindUiObject(ProjectShell.CardDetailPlaceCreditButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(previousCash));
            Assert.That(FindUiObject(ProjectShell.CardDetailFinalCashCostTextName).GetComponent<Text>().text, Does.Contain("최종 현금 3"));
            Assert.That(buyButton.interactable, Is.True);

            buyButton.onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(3));
            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(selectedCard.Card.Income));
            Assert.That(bootstrap.CurrentRun.Resources.Research, Is.EqualTo(0));
            Assert.That(bootstrap.CurrentRun.Resources.Credit, Is.EqualTo(0));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0].Card.Id, Is.EqualTo(previousUpcomingMarketFirstCardId));
            Assert.That(bootstrap.CurrentRun.MarketTape.CurrentMarketCards[1].Card.Id, Is.EqualTo(previousCurrentMarketSecondCardId));
            Assert.That(FindUiObject(ProjectShell.CardDetailPanelName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.PortfolioSummaryTextName).GetComponent<Text>().text, Does.Contain("보유 자산 1"));
            Assert.That(
                FindUiObject(ProjectShell.PortfolioSummaryTextName).GetComponent<Text>().text,
                Does.Contain("현재 운용가치 " + selectedCard.Card.ManagementValue));
            Assert.That(
                FindUiObject(ProjectShell.PortfolioSummaryTextName).GetComponent<Text>().text,
                Does.Contain("분기 운용 수익 " + selectedCard.Card.Income));
            Assert.That(
                FindUiObject(ProjectShell.PortfolioOwnedCardsTextName).GetComponent<Text>().text,
                Does.Contain(selectedCard.Card.DisplayName));

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

            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1")
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
            Assert.That(slotButtonText, Does.Contain("리서치"));
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

            var selectedCard = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0];
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
        public IEnumerator MainGameShellBootstrapReserveButtonMovesMarketCardToReservationAndUpdatesUi()
        {
            var scene = SceneManager.CreateScene("MainGameShellBootstrapReservationTests");
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);

            yield return null;

            var selectedCard = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0];
            var previousCurrentMarketSecondCardId = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[1].Card.Id;
            var previousUpcomingMarketFirstCardId = bootstrap.CurrentRun.MarketTape.UpcomingMarketCards[0].Card.Id;
            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;

            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1")
                .GetComponent<Button>()
                .onClick
                .Invoke();

            yield return null;

            var reserveButton = FindUiObject(ProjectShell.CardDetailReserveButtonName).GetComponent<Button>();
            Assert.That(reserveButton.interactable, Is.True);

            reserveButton.onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Reservation.ReservedCards, Has.Count.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Reservation.ReservedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.RedemptionPressure.CurrentPressure, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays - 1));
            Assert.That(bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0].Card.Id, Is.EqualTo(previousUpcomingMarketFirstCardId));
            Assert.That(bootstrap.CurrentRun.MarketTape.CurrentMarketCards[1].Card.Id, Is.EqualTo(previousCurrentMarketSecondCardId));
            Assert.That(FindUiObject(ProjectShell.CardDetailPanelName).activeSelf, Is.False);
            Assert.That(FindUiObject(ProjectShell.ReservationTitleTextName).GetComponent<Text>().text, Does.Contain("1/3"));
            Assert.That(
                FindUiObject(ProjectShell.ReservationCardButtonPrefix + "1").GetComponentInChildren<Text>().text,
                Does.Contain(selectedCard.Card.DisplayName));
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.EqualTo("환매 압력 +1"));
            Assert.That(
                FindUiObject(ProjectShell.RunStatusTextName).GetComponent<Text>().text,
                Does.Contain("환매 압력 1/10"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapReservedCardClickShowsDetailAndPurchaseClearsReservationOnly()
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

            var selectedCard = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0];
            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1")
                .GetComponent<Button>()
                .onClick
                .Invoke();

            yield return null;

            FindUiObject(ProjectShell.CardDetailReserveButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            var previousRemainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var previousSellImminentIds = CollectCardIds(bootstrap.CurrentRun.MarketTape.SellImminentCards);
            var previousCurrentMarketIds = CollectCardIds(bootstrap.CurrentRun.MarketTape.CurrentMarketCards);
            var previousUpcomingMarketIds = CollectCardIds(bootstrap.CurrentRun.MarketTape.UpcomingMarketCards);

            FindUiObject(ProjectShell.ReservationCardButtonPrefix + "1")
                .GetComponent<Button>()
                .onClick
                .Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));
            Assert.That(bootstrap.CurrentRun.CardDetail.SelectedCard.Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.CardDetail.PurchaseSource, Is.EqualTo(PurchaseSource.Reserved));
            Assert.That(FindUiObject(ProjectShell.CardDetailPanelName).activeSelf, Is.True);
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
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards[0].PurchaseSource, Is.EqualTo(PurchaseSource.Reserved));
            Assert.That(bootstrap.CurrentRun.Reservation.ReservedCards, Is.Empty);
            AssertZoneMatches(bootstrap.CurrentRun.MarketTape.SellImminentCards, previousSellImminentIds);
            AssertZoneMatches(bootstrap.CurrentRun.MarketTape.CurrentMarketCards, previousCurrentMarketIds);
            AssertZoneMatches(bootstrap.CurrentRun.MarketTape.UpcomingMarketCards, previousUpcomingMarketIds);
            Assert.That(FindUiObject(ProjectShell.ReservationTitleTextName).GetComponent<Text>().text, Does.Contain("0/3"));
            Assert.That(
                FindUiObject(ProjectShell.ReservationCardButtonPrefix + "1").GetComponentInChildren<Text>().text,
                Is.EqualTo("비어 있음"));

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
                bootstrap.OpenMarketCardDetail(bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0]);
                bootstrap.ConfirmReservation();
            }

            var remainingBusinessDays = bootstrap.CurrentRun.Calendar.RemainingBusinessDays;
            var deal = bootstrap.CurrentRun.Resources.Deal;
            var pressure = bootstrap.CurrentRun.RedemptionPressure.CurrentPressure;

            bootstrap.OpenMarketCardDetail(bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0]);

            yield return null;

            var reserveButton = FindUiObject(ProjectShell.CardDetailReserveButtonName).GetComponent<Button>();
            Assert.That(reserveButton.interactable, Is.False);
            Assert.That(FindUiObject(ProjectShell.ResourceMessageTextName).GetComponent<Text>().text, Is.EqualTo("예약 구역이 가득 찼습니다."));

            bootstrap.ConfirmReservation();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Reservation.ReservedCards, Has.Count.EqualTo(3));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(deal));
            Assert.That(bootstrap.CurrentRun.RedemptionPressure.CurrentPressure, Is.EqualTo(pressure));
            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));

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
            bootstrap.OpenMarketCardDetail(bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0]);

            yield return null;

            FindUiObject(ProjectShell.CardDetailReserveButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.State, Is.EqualTo(RunState.Failed));
            Assert.That(bootstrap.CurrentRun.RedemptionPressure.CurrentPressure, Is.EqualTo(10));
            Assert.That(FindUiObject(ProjectShell.RunFailurePlaceholderPanelName).activeSelf, Is.True);
            Assert.That(
                FindUiObject(ProjectShell.RunFailurePlaceholderTextName).GetComponent<Text>().text,
                Does.Contain("대규모 환매"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        [UnityTest]
        public IEnumerator MainGameShellBootstrapLiquidityActionButtonsDriveGainLiquidity()
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
            var startingCash = bootstrap.CurrentRun.Resources.Cash;
            var marketPanel = FindUiObject(ProjectShell.MarketAreaMarketPanelName);
            var liquidityPanel = FindUiObject(ProjectShell.LiquidityActionPanelName);
            var centralBankButton = FindUiObject(ProjectShell.CentralBankButtonName).GetComponent<Button>();
            var closeButton = FindUiObject(ProjectShell.LiquidityActionCloseButtonName).GetComponent<Button>();
            var cashButton = FindUiObject(ProjectShell.LiquidityActionCashButtonName).GetComponent<Button>();
            var researchButton = FindUiObject(ProjectShell.LiquidityActionResearchButtonName).GetComponent<Button>();
            var nextBusinessDayButton = FindUiObject(ProjectShell.NextBusinessDayButtonName).GetComponent<Button>();

            Assert.That(marketPanel.activeSelf, Is.True);
            Assert.That(liquidityPanel.activeSelf, Is.False);
            Assert.That(centralBankButton.interactable, Is.True);
            Assert.That(nextBusinessDayButton.interactable, Is.True);

            centralBankButton.onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.GainLiquidity));
            Assert.That(marketPanel.activeSelf, Is.False);
            Assert.That(liquidityPanel.activeSelf, Is.True);
            Assert.That(closeButton.interactable, Is.True);
            Assert.That(cashButton.interactable, Is.True);
            Assert.That(researchButton.interactable, Is.True);
            Assert.That(nextBusinessDayButton.interactable, Is.False);

            closeButton.onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(liquidityPanel.activeSelf, Is.False);
            Assert.That(nextBusinessDayButton.interactable, Is.True);

            centralBankButton.onClick.Invoke();

            yield return null;

            cashButton.onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.GainLiquidity));
            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(startingCash + 1));
            Assert.That(closeButton.interactable, Is.False);
            Assert.That(nextBusinessDayButton.interactable, Is.False);

            cashButton.onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays - 1));
            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(startingCash + 2));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(0));
            Assert.That(liquidityPanel.activeSelf, Is.False);
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

            var expectedSellImminentCard = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0].Card;
            var buttonObject = GameObject.Find(ProjectShell.MarketTapeAdvanceButtonName);
            Assert.That(buttonObject, Is.Not.Null);

            buttonObject.GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.MarketTape.SellImminentCards[0].Card.Id, Is.EqualTo(expectedSellImminentCard.Id));

            var sellImminentCardButtonText = FindUiObject(ProjectShell.MarketTapeSellImminentCardButtonPrefix + "1")
                .GetComponentInChildren<Text>()
                .text;
            Assert.That(sellImminentCardButtonText, Does.Contain(expectedSellImminentCard.DisplayName));

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
            var centralBankButton = FindUiObject(ProjectShell.CentralBankButtonName);
            var marketTapeAdvanceButton = FindUiObject(ProjectShell.MarketTapeAdvanceButtonName);
            var resourceDevCashButton = FindUiObject(ProjectShell.ResourceDevFundingCashButtonName);
            var nextBusinessDayButton = FindUiObject(ProjectShell.NextBusinessDayButtonName).GetComponent<Button>();
            var nextBusinessDayButtonText = nextBusinessDayButton.GetComponentInChildren<Text>();

            Assert.That(statusText.text, Does.Contain("추가 매수 가능"));
            Assert.That(centralBankButton.activeSelf, Is.False);
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
            Assert.That(text, Does.Contain("분기 운용 수익"));
            Assert.That(text, Does.Contain("분기 목표"));
            Assert.That(text, Does.Contain("목표 달성률"));
            Assert.That(text, Does.Contain("환매 압력 +3"));

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
            Assert.That(failureText, Does.Contain("대규모 환매 발생"));
            Assert.That(failureText, Does.Contain("도달 지점 1회계년도 2Q"));
            Assert.That(failureText, Does.Contain("총 운용 수익 2"));
            Assert.That(failureText, Does.Contain("환매 압력 10/10"));

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
            Assert.That(vacationText, Does.Contain("현재 운용가치"));
            Assert.That(vacationText, Does.Contain("올해 운용 수익"));
            Assert.That(vacationText, Does.Contain("분기별 운용 수익"));
            Assert.That(vacationText, Does.Contain("보유 자산"));
            Assert.That(vacationText, Does.Contain("환매 압력"));

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
                bootstrap.CurrentRun.AssetCards[3].Card,
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
            Assert.That(finalText, Does.Contain("최종 운용가치 7"));
            Assert.That(finalText, Does.Contain("최종 평가 Core"));
            Assert.That(finalText, Does.Contain("총 운용 수익"));
            Assert.That(finalText, Does.Contain("보유 자산 2"));
            Assert.That(finalText, Does.Contain("환매 압력"));
            Assert.That(finalText, Does.Contain("운용 코멘트"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }

        private static GameObject FindUiObject(string objectName)
        {
            var uiRoot = GameObject.Find(ProjectShell.UiRootName);
            Assert.That(uiRoot, Is.Not.Null);

            var match = FindChild(uiRoot.transform, objectName);
            Assert.That(match, Is.Not.Null, "Expected to find UI object " + objectName + ".");
            return match.gameObject;
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
