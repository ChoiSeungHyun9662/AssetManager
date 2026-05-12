using System.Collections;
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

            Assert.That(resourceText.text, Does.Contain("전문 자원 0/10"));
            Assert.That(resourceText.text, Does.Contain("딜 0/3"));

            FindUiObject(ProjectShell.ResourceDevFundingCashButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(initialCash + 1));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(0));
            Assert.That(FindUiObject(ProjectShell.ResourceHudTextName).GetComponent<Text>().text, Does.Contain($"현금 {initialCash + 1}"));

            FindUiObject(ProjectShell.ResourceDevEarnedCashButtonName).GetComponent<Button>().onClick.Invoke();

            yield return null;

            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(initialCash + 2));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentFiscalYearEarnedCash, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Performance.TotalEarnedCash, Is.EqualTo(1));
            Assert.That(FindUiObject(ProjectShell.ResourceHudTextName).GetComponent<Text>().text, Does.Contain($"현금 {initialCash + 2}"));

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

            var sellImminentText = GameObject.Find(ProjectShell.MarketTapeSellImminentTextName);
            var currentMarketText = GameObject.Find(ProjectShell.MarketTapeCurrentMarketTextName);
            var upcomingMarketText = GameObject.Find(ProjectShell.MarketTapeUpcomingMarketTextName);

            Assert.That(sellImminentText, Is.Not.Null);
            Assert.That(currentMarketText, Is.Not.Null);
            Assert.That(upcomingMarketText, Is.Not.Null);

            var firstSellImminentCard = bootstrap.CurrentRun.MarketTape.SellImminentCards[0].Card;
            var firstSellImminentButtonText = FindUiObject(ProjectShell.MarketTapeSellImminentCardButtonPrefix + "1")
                .GetComponentInChildren<Text>()
                .text;

            Assert.That(sellImminentText.GetComponent<Text>().text, Is.Not.Empty);
            Assert.That(currentMarketText.GetComponent<Text>().text, Is.Not.Empty);
            Assert.That(upcomingMarketText.GetComponent<Text>().text, Is.Not.Empty);
            Assert.That(firstSellImminentButtonText, Does.Contain(firstSellImminentCard.DisplayName));
            Assert.That(firstSellImminentButtonText, Does.Contain(firstSellImminentCard.CashCost.ToString()));
            Assert.That(firstSellImminentButtonText, Does.Contain(firstSellImminentCard.ManagementValue.ToString()));
            Assert.That(firstSellImminentButtonText, Does.Contain(firstSellImminentCard.Income.ToString()));

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
            Assert.That(text, Is.Not.Empty);

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
    }
}
