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

            var text = statusText.GetComponent<Text>().text;
            Assert.That(text, Does.Contain("1회계년도 1Q"));
            Assert.That(text, Does.Contain("남은 4영업일"));
            Assert.That(text, Does.Contain("현금 3"));
            Assert.That(text, Does.Contain("리서치 0"));
            Assert.That(text, Does.Contain("신용 0"));
            Assert.That(text, Does.Contain("원자재 0"));
            Assert.That(text, Does.Contain("딜 0/3"));
            Assert.That(text, Does.Contain("환매 압력 0/10"));

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
            var text = sellImminentText.GetComponent<Text>().text;
            Assert.That(text, Does.Contain("매도 임박"));
            Assert.That(text, Does.Contain(firstSellImminentCard.DisplayName));
            Assert.That(text, Does.Contain("현금 " + firstSellImminentCard.CashCost));
            Assert.That(text, Does.Contain("운용가치 " + firstSellImminentCard.ManagementValue));
            Assert.That(text, Does.Contain("인컴 " + firstSellImminentCard.Income));
            Assert.That(currentMarketText.GetComponent<Text>().text, Does.Contain("현재 시장"));
            Assert.That(upcomingMarketText.GetComponent<Text>().text, Does.Contain("예비 시장"));

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

            var sellImminentText = GameObject.Find(ProjectShell.MarketTapeSellImminentTextName).GetComponent<Text>().text;
            Assert.That(sellImminentText, Does.Contain(expectedSellImminentCard.DisplayName));

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
            Assert.That(statusText.GetComponent<Text>().text, Does.Contain("남은 3영업일"));

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
            Assert.That(text, Does.Contain("분기 마감"));
            Assert.That(text, Does.Contain("1회계년도 1Q"));

            yield return SceneManager.UnloadSceneAsync(scene);
        }
    }
}
