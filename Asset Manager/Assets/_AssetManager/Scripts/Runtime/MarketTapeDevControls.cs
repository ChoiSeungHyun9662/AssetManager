using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class MarketTapeDevControls : MonoBehaviour
    {
        [SerializeField]
        private Button advanceButton;

        [SerializeField]
        private Button refreshButton;

        public Button AdvanceButton => advanceButton;
        public Button RefreshButton => refreshButton;

        public void Bind(Button advance, Button refresh)
        {
            advanceButton = advance;
            refreshButton = refresh;
        }

        public void Show(RunSessionState run)
        {
            var isActive = run.State == RunState.Playing
                && run.BusinessDay.Phase == BusinessDayPhase.AwaitingAction
                && run.BusinessDay.MarketArea == MarketAreaState.Market;
            SetActive(advanceButton, isActive);
            SetActive(refreshButton, isActive);
        }

        private static void SetActive(Button button, bool isActive)
        {
            if (button != null)
            {
                button.gameObject.SetActive(isActive);
            }
        }
    }
}
