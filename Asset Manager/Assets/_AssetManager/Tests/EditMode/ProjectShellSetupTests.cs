using System.Linq;
using AssetManager.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace AssetManager.Tests
{
    public sealed class ProjectShellSetupTests
    {
        [Test]
        public void EnsureProjectShellCreatesScenesAndRegistersThemForBuild()
        {
            AssetManagerProjectSetup.EnsureProjectShell();

            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectShell.BootstrapScenePath), Is.Not.Null);
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectShell.MainGameScenePath), Is.Not.Null);

            var enabledScenePaths = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            Assert.That(enabledScenePaths, Contains.Item(ProjectShell.BootstrapScenePath));
            Assert.That(enabledScenePaths, Contains.Item(ProjectShell.MainGameScenePath));
        }

        [Test]
        public void MainGameSceneContainsExpectedRoots()
        {
            AssetManagerProjectSetup.EnsureProjectShell();

            var scene = EditorSceneManager.OpenScene(ProjectShell.MainGameScenePath);

            Assert.That(scene.GetRootGameObjects().Any(root => root.name == ProjectShell.GameRootName), Is.True);
            Assert.That(scene.GetRootGameObjects().Any(root => root.name == ProjectShell.UiRootName), Is.True);
        }

        [Test]
        public void EnsureProjectShellCreatesMvpRunDataAndConnectsMainGameBootstrap()
        {
            AssetManagerProjectSetup.EnsureProjectShell();

            var staticData = AssetDatabase.LoadAssetAtPath<RunStaticDataSet>(ProjectShell.MvpRunStaticDataPath);
            Assert.That(staticData, Is.Not.Null);
            Assert.That(staticData.HasRequiredMvpData, Is.True);
            Assert.That(staticData.AssetCards, Has.Count.GreaterThanOrEqualTo(1));
            Assert.That(staticData.Quarters, Has.Count.GreaterThanOrEqualTo(1));
            Assert.That(staticData.FinalRatings, Has.Count.GreaterThanOrEqualTo(1));
            Assert.That(staticData.ResourceConfig, Is.Not.Null);
            Assert.That(staticData.RedemptionPressureConfig, Is.Not.Null);

            var scene = EditorSceneManager.OpenScene(ProjectShell.MainGameScenePath);
            var shell = scene.GetRootGameObjects().Single(root => root.name == "Main Game Shell");
            var bootstrap = shell.GetComponent<MainGameShellBootstrap>();

            Assert.That(bootstrap, Is.Not.Null);
            Assert.That(bootstrap.StaticData, Is.SameAs(staticData));
        }
    }
}
