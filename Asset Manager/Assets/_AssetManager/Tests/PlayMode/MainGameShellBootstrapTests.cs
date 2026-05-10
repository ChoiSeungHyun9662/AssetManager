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
