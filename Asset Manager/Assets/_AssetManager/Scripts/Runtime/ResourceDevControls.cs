using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class ResourceDevControls : MonoBehaviour
    {
        [SerializeField]
        private Button fundingCashButton;

        [SerializeField]
        private Button earnedCashButton;

        [SerializeField]
        private Button researchButton;

        [SerializeField]
        private Button creditButton;

        [SerializeField]
        private Button commodityButton;

        [SerializeField]
        private Button dealButton;

        public Button FundingCashButton => fundingCashButton;
        public Button EarnedCashButton => earnedCashButton;
        public Button ResearchButton => researchButton;
        public Button CreditButton => creditButton;
        public Button CommodityButton => commodityButton;
        public Button DealButton => dealButton;

        public void Bind(
            Button fundingCash,
            Button earnedCash,
            Button research,
            Button credit,
            Button commodity,
            Button deal)
        {
            fundingCashButton = fundingCash;
            earnedCashButton = earnedCash;
            researchButton = research;
            creditButton = credit;
            commodityButton = commodity;
            dealButton = deal;
        }

        public void Show(RunSessionState run)
        {
            var isActive = run.State == RunState.Playing
                && run.BusinessDay.Phase == BusinessDayPhase.AwaitingAction
                && run.BusinessDay.MarketArea == MarketAreaState.Market
                && !run.BusinessDay.IsAwaitingExtraBuyChoice;

            SetActive(fundingCashButton, isActive);
            SetActive(earnedCashButton, isActive);
            SetActive(researchButton, isActive);
            SetActive(creditButton, isActive);
            SetActive(commodityButton, isActive);
            SetActive(dealButton, isActive);
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
