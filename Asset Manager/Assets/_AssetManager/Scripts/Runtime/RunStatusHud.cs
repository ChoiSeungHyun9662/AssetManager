using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class RunStatusHud : MonoBehaviour
    {
        [SerializeField]
        private Text statusText;

        public Text StatusText => statusText;

        public void Bind(Text text)
        {
            statusText = text;
        }

        public void Show(RunSessionState run)
        {
            if (statusText == null)
            {
                Debug.LogError("RunStatusHud is missing a status text reference.");
                return;
            }

            statusText.text = RunStatusFormatter.Format(run);
        }
    }
}
