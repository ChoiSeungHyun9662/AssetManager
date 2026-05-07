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
    }
}
