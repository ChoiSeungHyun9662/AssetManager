using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssetManager.Editor
{
    public static class AssetManagerProjectSetup
    {
        [MenuItem("Asset Manager/Setup/Ensure Project Shell")]
        public static void EnsureProjectShell()
        {
            EnsureFolderPath("Assets/_AssetManager/Scenes");
            EnsureFolderPath(ProjectShell.DataRootPath);
            var staticData = EnsureMvpRunStaticData();
            EnsureBootstrapScene();
            EnsureMainGameScene(staticData);
            EnsureBuildScenes();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Asset Manager/Setup/Verify Project Shell")]
        public static void VerifyProjectShell()
        {
            RequireAsset<SceneAsset>(ProjectShell.BootstrapScenePath);
            RequireAsset<SceneAsset>(ProjectShell.MainGameScenePath);
            var staticData = RequireAsset<RunStaticDataSet>(ProjectShell.MvpRunStaticDataPath);
            if (!staticData.HasRequiredMvpData)
            {
                throw new InvalidOperationException("MVP run static data is missing required bootstrap data.");
            }

            RequireBuildScene(ProjectShell.BootstrapScenePath);
            RequireBuildScene(ProjectShell.MainGameScenePath);

            var bootstrapScene = EditorSceneManager.OpenScene(ProjectShell.BootstrapScenePath);
            var bootstrapRunner = RequireRootObject(bootstrapScene, "Bootstrap Runner");
            RequireComponent<BootstrapSceneLoader>(bootstrapRunner);

            var mainGame = EditorSceneManager.OpenScene(ProjectShell.MainGameScenePath);
            var shell = RequireRootObject(mainGame, "Main Game Shell");
            var mainGameBootstrap = RequireComponent<MainGameShellBootstrap>(shell);
            if (mainGameBootstrap.StaticData != staticData)
            {
                throw new InvalidOperationException("Main Game Shell is not connected to the MVP RunStaticDataSet asset.");
            }

            RequireRootObject(mainGame, ProjectShell.GameRootName);

            var uiRoot = RequireRootObject(mainGame, ProjectShell.UiRootName);
            RequireComponent<Canvas>(uiRoot);

            var scaler = RequireComponent<CanvasScaler>(uiRoot);
            if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                throw new InvalidOperationException("UI Root must use Scale With Screen Size.");
            }

            if (scaler.referenceResolution != ProjectShell.ReferenceResolution)
            {
                throw new InvalidOperationException("UI Root reference resolution must be 1920x1080.");
            }

            var readyStatus = uiRoot.transform.Find(ProjectShell.ReadyStatusTextName);
            if (readyStatus == null)
            {
                throw new InvalidOperationException("UI Root is missing the ready status text.");
            }

            var text = RequireComponent<Text>(readyStatus.gameObject);
            if (text.text != ProjectShell.ReadyStatusText)
            {
                throw new InvalidOperationException("Ready status text does not match the expected label.");
            }

            var runStatus = uiRoot.transform.Find(ProjectShell.RunStatusTextName);
            if (runStatus == null)
            {
                throw new InvalidOperationException("UI Root is missing the run status text.");
            }

            Debug.Log("Asset Manager project shell verification passed.");
        }

        private static void EnsureBootstrapScene()
        {
            var scene = OpenOrCreateScene(ProjectShell.BootstrapScenePath, NewSceneSetup.EmptyScene);
            SceneManager.SetActiveScene(scene);

            var runner = EnsureRootObject(scene, "Bootstrap Runner");
            if (runner.GetComponent<BootstrapSceneLoader>() == null)
            {
                runner.AddComponent<BootstrapSceneLoader>();
            }

            EditorSceneManager.SaveScene(scene, ProjectShell.BootstrapScenePath);
        }

        private static void EnsureMainGameScene(RunStaticDataSet staticData)
        {
            var scene = OpenOrCreateScene(ProjectShell.MainGameScenePath, NewSceneSetup.DefaultGameObjects);
            SceneManager.SetActiveScene(scene);

            var shell = EnsureRootObject(scene, "Main Game Shell");
            var bootstrap = shell.GetComponent<MainGameShellBootstrap>();
            if (bootstrap == null)
            {
                bootstrap = shell.AddComponent<MainGameShellBootstrap>();
            }

            bootstrap.StaticData = staticData;
            EditorUtility.SetDirty(bootstrap);

            ProjectShell.EnsureMainGameRoots();

            EditorSceneManager.SaveScene(scene, ProjectShell.MainGameScenePath);
        }

        private static RunStaticDataSet EnsureMvpRunStaticData()
        {
            var staticData = AssetDatabase.LoadAssetAtPath<RunStaticDataSet>(ProjectShell.MvpRunStaticDataPath);
            if (staticData == null)
            {
                staticData = ScriptableObject.CreateInstance<RunStaticDataSet>();
                staticData.ResetToMvpDefaults();
                AssetDatabase.CreateAsset(staticData, ProjectShell.MvpRunStaticDataPath);
                return staticData;
            }

            if (!staticData.HasRequiredMvpData)
            {
                staticData.ResetToMvpDefaults();
                EditorUtility.SetDirty(staticData);
            }

            return staticData;
        }

        private static Scene OpenOrCreateScene(string path, NewSceneSetup setup)
        {
            if (File.Exists(ToFullPath(path)))
            {
                return EditorSceneManager.OpenScene(path);
            }

            return EditorSceneManager.NewScene(setup, NewSceneMode.Single);
        }

        private static GameObject EnsureRootObject(Scene scene, string name)
        {
            var existing = scene.GetRootGameObjects().FirstOrDefault(root => root.name == name);
            if (existing != null)
            {
                return existing;
            }

            var created = new GameObject(name);
            SceneManager.MoveGameObjectToScene(created, scene);
            return created;
        }

        private static void EnsureBuildScenes()
        {
            var requiredPaths = new[]
            {
                ProjectShell.BootstrapScenePath,
                ProjectShell.MainGameScenePath
            };

            var existingExtras = EditorBuildSettings.scenes
                .Where(scene => !requiredPaths.Contains(scene.path))
                .ToArray();

            var requiredScenes = requiredPaths
                .Select(path => new EditorBuildSettingsScene(path, true));

            EditorBuildSettings.scenes = requiredScenes.Concat(existingExtras).ToArray();
        }

        private static T RequireAsset<T>(string path) where T : UnityEngine.Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                throw new InvalidOperationException("Missing required asset: " + path);
            }

            return asset;
        }

        private static void RequireBuildScene(string path)
        {
            if (!EditorBuildSettings.scenes.Any(scene => scene.enabled && scene.path == path))
            {
                throw new InvalidOperationException("Missing required build scene: " + path);
            }
        }

        private static GameObject RequireRootObject(Scene scene, string name)
        {
            var root = scene.GetRootGameObjects().FirstOrDefault(gameObject => gameObject.name == name);
            if (root == null)
            {
                throw new InvalidOperationException("Missing required root object: " + name);
            }

            return root;
        }

        private static T RequireComponent<T>(GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                throw new InvalidOperationException(gameObject.name + " is missing component " + typeof(T).Name + ".");
            }

            return component;
        }

        private static void EnsureFolderPath(string path)
        {
            var parts = path.Split('/');
            var current = parts[0];

            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        private static string ToFullPath(string assetPath)
        {
            var projectRootDirectory = Directory.GetParent(Application.dataPath);
            if (projectRootDirectory == null)
            {
                throw new DirectoryNotFoundException("Could not resolve the Unity project root.");
            }

            var projectRoot = projectRootDirectory.FullName;
            return Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
