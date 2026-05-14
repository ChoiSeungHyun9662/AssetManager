using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace AssetManager.Tests
{
    public sealed class MvpSmokeScenarioTests
    {
        [UnityTest]
        public IEnumerator MvpSmoke_NewRunAdvancesFourBusinessDaysIntoQuarterSettlement()
        {
            var bootstrap = CreateStartedShell("MvpSmokeBasicProgression");
            yield return null;

            var nextBusinessDayButton = FindUiObject(ProjectShell.NextBusinessDayButtonName).GetComponent<Button>();
            for (var i = 0; i < 4; i++)
            {
                nextBusinessDayButton.onClick.Invoke();
            }

            yield return null;

            Assert.That(bootstrap.CurrentRun.Calendar.FiscalYear, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Calendar.Quarter, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(0));
            Assert.That(bootstrap.CurrentRun.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.QuarterSettlement));
            Assert.That(FindUiObject(ProjectShell.QuarterSettlementPlaceholderPanelName).activeSelf, Is.True);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }

        [UnityTest]
        public IEnumerator MvpSmoke_MarketCardPurchaseAddsOwnedAssetAndNextDayIncome()
        {
            var bootstrap = CreateStartedShell("MvpSmokeMarketPurchase");
            yield return null;

            FindUiObject(ProjectShell.ResourceDevResearchButtonName).GetComponent<Button>().onClick.Invoke();
            FindUiObject(ProjectShell.ResourceDevCreditButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            var selectedCard = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0];
            var expectedCashAfterPurchaseAndIncome =
                bootstrap.CurrentRun.Resources.Cash - selectedCard.Card.CashCost + selectedCard.Card.Income;

            FindUiObject(ProjectShell.MarketTapeCurrentMarketCardButtonPrefix + "1")
                .GetComponent<Button>()
                .onClick
                .Invoke();
            yield return null;

            FindUiObject(ProjectShell.CardDetailPlaceResearchButtonName).GetComponent<Button>().onClick.Invoke();
            FindUiObject(ProjectShell.CardDetailPlaceCreditButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            var buyButton = FindUiObject(ProjectShell.CardDetailBuyButtonName).GetComponent<Button>();
            Assert.That(buyButton.interactable, Is.True);

            buyButton.onClick.Invoke();
            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(3));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(expectedCashAfterPurchaseAndIncome));
            Assert.That(FindUiObject(ProjectShell.PortfolioSummaryTextName).GetComponent<Text>().text, Does.Contain("보유 자산 1"));

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }

        [UnityTest]
        public IEnumerator MvpSmoke_LiquidityCashTwiceAddsFundingCashAndEndsBusinessDay()
        {
            var bootstrap = CreateStartedShell("MvpSmokeLiquidity");
            yield return null;

            var startingCash = bootstrap.CurrentRun.Resources.Cash;

            FindUiObject(ProjectShell.CentralBankButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.GainLiquidity));
            Assert.That(FindUiObject(ProjectShell.NextBusinessDayButtonName).GetComponent<Button>().interactable, Is.False);

            FindUiObject(ProjectShell.LiquidityActionCashButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;
            FindUiObject(ProjectShell.LiquidityActionCashButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(3));
            Assert.That(bootstrap.CurrentRun.Resources.Cash, Is.EqualTo(startingCash + 2));
            Assert.That(bootstrap.CurrentRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(0));
            Assert.That(FindUiObject(ProjectShell.LiquidityActionPanelName).activeSelf, Is.False);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }

        [UnityTest]
        public IEnumerator MvpSmoke_ReservationAddsReservedCardDealPressureAndAdvancesTape()
        {
            var bootstrap = CreateStartedShell("MvpSmokeReservation");
            yield return null;

            var selectedCard = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0];
            var previousCurrentMarketSecondCard = bootstrap.CurrentRun.MarketTape.CurrentMarketCards[1];
            var nextColumnCard = bootstrap.CurrentRun.MarketTape.UpcomingMarketCards[0];

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
            Assert.That(bootstrap.CurrentRun.Calendar.RemainingBusinessDays, Is.EqualTo(3));
            Assert.That(bootstrap.CurrentRun.Reservation.ReservedCards, Has.Count.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.Reservation.ReservedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.Resources.Deal, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.RedemptionPressure.CurrentPressure, Is.EqualTo(1));
            Assert.That(bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0].Card.Id, Is.EqualTo(nextColumnCard.Card.Id));
            Assert.That(bootstrap.CurrentRun.MarketTape.CurrentMarketCards[1].Card.Id, Is.EqualTo(previousCurrentMarketSecondCard.Card.Id));
            Assert.That(FindUiObject(ProjectShell.ReservationCardButtonPrefix + "1").GetComponentInChildren<Text>().text, Does.Contain(selectedCard.Card.DisplayName));

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }

        [UnityTest]
        public IEnumerator MvpSmoke_ReservationAtNineRedemptionPressureShowsFailureScreen()
        {
            var bootstrap = CreateStartedShell("MvpSmokeRedemptionFailure");
            yield return null;

            SetCurrentRun(bootstrap, WithRedemptionPressure(bootstrap.CurrentRun, 9));
            RefreshRunUi(bootstrap);
            yield return null;

            bootstrap.OpenMarketCardDetail(bootstrap.CurrentRun.MarketTape.CurrentMarketCards[0]);
            yield return null;

            FindUiObject(ProjectShell.CardDetailReserveButtonName).GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.That(bootstrap.CurrentRun.State, Is.EqualTo(RunState.Failed));
            Assert.That(bootstrap.CurrentRun.RedemptionPressure.CurrentPressure, Is.EqualTo(10));
            Assert.That(FindUiObject(ProjectShell.RunFailurePlaceholderPanelName).activeSelf, Is.True);
            Assert.That(FindUiObject(ProjectShell.ContinueScheduleButtonName).activeSelf, Is.False);

            var failureText = FindUiObject(ProjectShell.RunFailurePlaceholderTextName).GetComponent<Text>().text;
            Assert.That(failureText, Does.Contain("대규모 환매 발생"));
            Assert.That(failureText, Does.Contain("환매 압력 10/10"));

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }

        [UnityTest]
        public IEnumerator MvpSmoke_ThirdFiscalYearFourthQuarterContinuesToFinalSettlement()
        {
            var bootstrap = CreateStartedShell("MvpSmokeFinalSettlement");
            yield return null;

            var ownedOffice = new AssetCardRuntimeData(
                bootstrap.CurrentRun.AssetCards[0].Card,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape);
            var ownedDataCenter = new AssetCardRuntimeData(
                bootstrap.CurrentRun.AssetCards[3].Card,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape);

            var run = WithCalendar(bootstrap.CurrentRun, new RunCalendarState(3, 4, 1));
            run = WithOwnedAssets(run, new OwnedAssetState(new[] { ownedOffice, ownedDataCenter }));
            SetCurrentRun(bootstrap, run);
            RefreshRunUi(bootstrap);
            yield return null;

            bootstrap.AdvanceToNextBusinessDay();
            yield return null;

            Assert.That(bootstrap.CurrentRun.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.QuarterSettlement));
            Assert.That(FindUiObject(ProjectShell.QuarterSettlementPlaceholderPanelName).activeSelf, Is.True);

            bootstrap.ContinueSchedule();
            yield return null;

            Assert.That(bootstrap.CurrentRun.State, Is.EqualTo(RunState.Completed));
            Assert.That(bootstrap.CurrentRun.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.FinalSettlement));
            Assert.That(FindUiObject(ProjectShell.FinalSettlementPlaceholderPanelName).activeSelf, Is.True);

            var finalText = FindUiObject(ProjectShell.FinalSettlementPlaceholderTextName).GetComponent<Text>().text;
            Assert.That(finalText, Does.Contain("최종 운용가치 7"));
            Assert.That(finalText, Does.Contain("최종 평가 Core"));
            Assert.That(finalText, Does.Contain("총 운용 수익"));

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }

        private static MainGameShellBootstrap CreateStartedShell(string sceneName)
        {
            var scene = SceneManager.CreateScene(sceneName);
            SceneManager.SetActiveScene(scene);

            var shell = new GameObject("Main Game Shell");
            shell.SetActive(false);

            var bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            bootstrap.StaticData = RunStaticDataSet.CreateMvpDefaults();

            shell.SetActive(true);
            return bootstrap;
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
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);
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
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);
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
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);
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
    }
}
