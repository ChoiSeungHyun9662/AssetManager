using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetManager
{
    [DisallowMultipleComponent]
    public sealed class BootstrapSceneLoader : MonoBehaviour
    {
        [SerializeField]
        private string targetSceneName = ProjectShell.MainGameSceneName;

        public string TargetSceneName => targetSceneName;

        private void Start()
        {
            LoadTargetScene();
        }

        public void LoadTargetScene()
        {
            if (!Application.isPlaying || string.IsNullOrEmpty(targetSceneName))
            {
                return;
            }

            if (SceneManager.GetActiveScene().name == targetSceneName)
            {
                return;
            }

            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
        }
    }
}
