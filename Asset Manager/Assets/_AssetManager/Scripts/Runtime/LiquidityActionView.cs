using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class LiquidityActionView : MonoBehaviour
    {
        [SerializeField]
        private Button centralBankButton;

        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private Text selectionText;

        [SerializeField]
        private Text messageText;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Button cashButton;

        [SerializeField]
        private Button researchButton;

        [SerializeField]
        private Button creditButton;

        [SerializeField]
        private Button commodityButton;

        public Button CentralBankButton => centralBankButton;
        public Button CloseButton => closeButton;
        public Button CashButton => cashButton;
        public Button ResearchButton => researchButton;
        public Button CreditButton => creditButton;
        public Button CommodityButton => commodityButton;

        public void Bind(
            Button centralBank,
            GameObject liquidityPanel,
            Text selection,
            Text message,
            Button close,
            Button cash,
            Button research,
            Button credit,
            Button commodity)
        {
            centralBankButton = centralBank;
            panel = liquidityPanel;
            selectionText = selection;
            messageText = message;
            closeButton = close;
            cashButton = cash;
            researchButton = research;
            creditButton = credit;
            commodityButton = commodity;
        }

        public void Show(RunSessionState run, string message)
        {
            if (run == null)
            {
                SetActive(centralBankButton, false);
                SetActive(panel, false);
                return;
            }

            var canEnter = run.State == RunState.Playing
                && run.BusinessDay.Phase == BusinessDayPhase.AwaitingAction
                && run.BusinessDay.MarketArea == MarketAreaState.Market;
            var isLiquidityAction = run.BusinessDay.MarketArea == MarketAreaState.GainLiquidity;

            SetActive(centralBankButton, canEnter);
            SetActive(panel, isLiquidityAction);
            SetText(selectionText, FormatSelection(run.LiquidityAction));
            SetText(messageText, message ?? string.Empty);

            SetInteractable(closeButton, LiquidityAction.CanClose(run));
            SetInteractable(cashButton, LiquidityAction.CanSelect(run, ResourceType.Cash));
            SetInteractable(researchButton, LiquidityAction.CanSelect(run, ResourceType.Research));
            SetInteractable(creditButton, LiquidityAction.CanSelect(run, ResourceType.Credit));
            SetInteractable(commodityButton, LiquidityAction.CanSelect(run, ResourceType.Commodity));
        }

        private static string FormatSelection(LiquidityActionState action)
        {
            if (action.SelectedResources.Count == 0)
            {
                return "선택: 없음";
            }

            var builder = new StringBuilder("선택:");
            foreach (var resourceType in action.SelectedResources)
            {
                builder.Append(' ');
                builder.Append(ResourceLedger.GetResourceDisplayName(resourceType));
            }

            return builder.ToString();
        }

        private static void SetActive(Button button, bool isActive)
        {
            if (button != null)
            {
                button.gameObject.SetActive(isActive);
                button.interactable = isActive;
            }
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            if (target != null)
            {
                target.SetActive(isActive);
            }
        }

        private static void SetInteractable(Button button, bool isInteractable)
        {
            if (button != null)
            {
                button.interactable = isInteractable;
            }
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
