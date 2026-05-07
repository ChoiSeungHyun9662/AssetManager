using UnityEngine;

namespace AssetManager
{
    [DisallowMultipleComponent]
    public sealed class MainGameShellBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            ProjectShell.EnsureMainGameRoots();
        }
    }
}
