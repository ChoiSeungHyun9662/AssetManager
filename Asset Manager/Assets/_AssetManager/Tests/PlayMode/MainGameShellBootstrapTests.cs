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
    }
}
