using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssetManager
{
    public readonly struct ProjectShellRoots
    {
        public ProjectShellRoots(GameObject gameRoot, Canvas uiCanvas)
        {
            GameRoot = gameRoot;
            UiCanvas = uiCanvas;
        }

        public GameObject GameRoot { get; }
        public Canvas UiCanvas { get; }
    }

    public static class ProjectShell
    {
        public const string AssetRootPath = "Assets/_AssetManager";
        public const string BootstrapSceneName = "Bootstrap";
        public const string MainGameSceneName = "MainGame";
        public const string BootstrapScenePath = AssetRootPath + "/Scenes/" + BootstrapSceneName + ".unity";
        public const string MainGameScenePath = AssetRootPath + "/Scenes/" + MainGameSceneName + ".unity";
        public const string GameRootName = "Game Root";
        public const string UiRootName = "UI Root";
        public const string ReadyStatusTextName = "Shell Ready Text";
        public const string ReadyStatusText = "Asset Manager MVP Ready";

        public static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);

        public static ProjectShellRoots EnsureMainGameRoots()
        {
            var gameRoot = FindOrCreateRoot(GameRootName);
            var uiRoot = FindOrCreateRoot(UiRootName);

            var canvas = uiRoot.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = uiRoot.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = uiRoot.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = uiRoot.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.matchWidthOrHeight = 0.5f;

            if (uiRoot.GetComponent<GraphicRaycaster>() == null)
            {
                uiRoot.AddComponent<GraphicRaycaster>();
            }

            EnsureReadyStatusText(uiRoot.transform);

            return new ProjectShellRoots(gameRoot, canvas);
        }

        private static GameObject FindOrCreateRoot(string name)
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.IsValid())
            {
                foreach (var root in scene.GetRootGameObjects())
                {
                    if (root.name == name)
                    {
                        return root;
                    }
                }
            }

            return new GameObject(name);
        }

        private static void EnsureReadyStatusText(Transform uiRoot)
        {
            var existing = uiRoot.Find(ReadyStatusTextName);
            var textObject = existing != null
                ? existing.gameObject
                : new GameObject(ReadyStatusTextName, typeof(RectTransform));

            textObject.transform.SetParent(uiRoot, false);

            var rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(640f, 80f);

            var text = textObject.GetComponent<Text>();
            if (text == null)
            {
                text = textObject.AddComponent<Text>();
            }

            text.text = ReadyStatusText;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 32;
            text.color = Color.white;

            if (text.font == null)
            {
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
        }
    }
}
