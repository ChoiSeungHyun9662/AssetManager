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

        [Header("Manual Sprite Assets")]
        [SerializeField]
        private Sprite cashObjectSprite = null;

        [SerializeField]
        private Sprite researchChipSprite = null;

        [SerializeField]
        private Sprite creditChipSprite = null;

        [SerializeField]
        private Sprite commodityChipSprite = null;

        [SerializeField]
        private Image cashImage;

        [SerializeField]
        private Image researchImage;

        [SerializeField]
        private Image creditImage;

        [SerializeField]
        private Image commodityImage;

        private bool warnedMissingSprites;

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
            Button commodity,
            Image cashSpriteImage,
            Image researchSpriteImage,
            Image creditSpriteImage,
            Image commoditySpriteImage)
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
            cashImage = cashSpriteImage;
            researchImage = researchSpriteImage;
            creditImage = creditSpriteImage;
            commodityImage = commoditySpriteImage;
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
                && run.BusinessDay.MarketArea == MarketAreaState.Market
                && !run.BusinessDay.IsAwaitingExtraBuyChoice;
            var isLiquidityAction = run.BusinessDay.MarketArea == MarketAreaState.GainLiquidity;

            SetActive(centralBankButton, canEnter);
            SetActive(panel, isLiquidityAction);
            SetText(selectionText, FormatSelection(run.LiquidityAction));
            SetText(messageText, message ?? string.Empty);
            ApplySprite(cashImage, cashObjectSprite);
            ApplySprite(researchImage, researchChipSprite);
            ApplySprite(creditImage, creditChipSprite);
            ApplySprite(commodityImage, commodityChipSprite);
            WarnMissingSpritesOnce();

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

        private void WarnMissingSpritesOnce()
        {
            if (warnedMissingSprites)
            {
                return;
            }

            warnedMissingSprites = true;
            WarnMissingSprite(cashObjectSprite, nameof(cashObjectSprite));
            WarnMissingSprite(researchChipSprite, nameof(researchChipSprite));
            WarnMissingSprite(creditChipSprite, nameof(creditChipSprite));
            WarnMissingSprite(commodityChipSprite, nameof(commodityChipSprite));
        }

        private static void WarnMissingSprite(Sprite sprite, string fieldName)
        {
            if (sprite == null)
            {
                Debug.LogWarning("LiquidityActionView sprite not assigned: " + fieldName + "; using placeholder text UI.");
            }
        }

        private static void ApplySprite(Image image, Sprite sprite)
        {
            if (image == null)
            {
                return;
            }

            image.sprite = sprite;
            image.enabled = sprite != null;
            image.preserveAspect = true;
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
