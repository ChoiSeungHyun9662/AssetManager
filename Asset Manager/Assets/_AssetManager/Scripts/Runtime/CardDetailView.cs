using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class CardDetailView : MonoBehaviour
    {
        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private Text nameText;

        [SerializeField]
        private Text descriptionText;

        [SerializeField]
        private Text costText;

        [SerializeField]
        private Text managementValueText;

        [SerializeField]
        private Text incomeText;

        [SerializeField]
        private Text tagsText;

        [SerializeField]
        private Text rarityText;

        [SerializeField]
        private Text paymentSlotsText;

        [SerializeField]
        private Text finalCashCostText;

        [SerializeField]
        private List<Button> paymentSlotButtons = new List<Button>();

        [SerializeField]
        private Button placeResearchButton;

        [SerializeField]
        private Button placeCreditButton;

        [SerializeField]
        private Button placeCommodityButton;

        [SerializeField]
        private Button placeDealButton;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Button buyButton;

        [SerializeField]
        private Button reserveButton;

        public GameObject Panel => panel;
        public Text NameText => nameText;
        public Button CloseButton => closeButton;
        public Button BuyButton => buyButton;
        public Button ReserveButton => reserveButton;
        public IReadOnlyList<Button> PaymentSlotButtons => paymentSlotButtons;
        public Button PlaceResearchButton => placeResearchButton;
        public Button PlaceCreditButton => placeCreditButton;
        public Button PlaceCommodityButton => placeCommodityButton;
        public Button PlaceDealButton => placeDealButton;

        public void Bind(
            GameObject cardDetailPanel,
            Text cardName,
            Text description,
            Text cost,
            Text managementValue,
            Text income,
            Text tags,
            Text rarity,
            Text paymentSlots,
            Text finalCashCost,
            IReadOnlyList<Button> slotButtons,
            Button placeResearch,
            Button placeCredit,
            Button placeCommodity,
            Button placeDeal,
            Button close,
            Button buy,
            Button reserve)
        {
            panel = cardDetailPanel;
            nameText = cardName;
            descriptionText = description;
            costText = cost;
            managementValueText = managementValue;
            incomeText = income;
            tagsText = tags;
            rarityText = rarity;
            paymentSlotsText = paymentSlots;
            finalCashCostText = finalCashCost;
            paymentSlotButtons = new List<Button>(slotButtons);
            placeResearchButton = placeResearch;
            placeCreditButton = placeCredit;
            placeCommodityButton = placeCommodity;
            placeDealButton = placeDeal;
            closeButton = close;
            buyButton = buy;
            reserveButton = reserve;
        }

        public void Show(RunSessionState run)
        {
            var isCardDetail = run.BusinessDay.MarketArea == MarketAreaState.CardDetail;
            SetActive(panel, isCardDetail);

            if (!isCardDetail || run.CardDetail.DisplayData == null)
            {
                return;
            }

            var display = run.CardDetail.DisplayData;
            SetText(nameText, display.DisplayName);
            SetText(descriptionText, display.Description);
            SetText(costText, FormatCosts(display));
            SetText(managementValueText, "운용가치 " + display.ManagementValue);
            SetText(incomeText, "운용 수익 " + display.Income);
            SetText(tagsText, FormatTags(display));
            SetText(rarityText, "희귀도 " + display.Rarity);
            SetText(paymentSlotsText, FormatPaymentSlots(run.CardDetail.PendingPayment));
            SetText(finalCashCostText, "최종 현금 " + run.CardDetail.PendingPayment.FinalCashCost);
            SetActive(buyButton != null ? buyButton.gameObject : null, true);
            if (buyButton != null)
            {
                buyButton.interactable = PurchasePayment.CanConfirmPurchase(run);
            }

            SetActive(reserveButton != null ? reserveButton.gameObject : null, run.CardDetail.ShouldShowReserveButton);
            if (reserveButton != null)
            {
                reserveButton.interactable = ReservationAction.CanReserve(run);
            }

            SetActive(placeResearchButton != null ? placeResearchButton.gameObject : null, true);
            SetActive(placeCreditButton != null ? placeCreditButton.gameObject : null, true);
            SetActive(placeCommodityButton != null ? placeCommodityButton.gameObject : null, true);
            SetActive(placeDealButton != null ? placeDealButton.gameObject : null, true);
            ShowPaymentSlotButtons(run.CardDetail.PendingPayment);
        }

        private static string FormatCosts(CardDetailDisplayData display)
        {
            var builder = new StringBuilder();
            builder.Append("현금 ");
            builder.Append(display.CashCost);

            foreach (var cost in display.ProfessionalCosts)
            {
                builder.Append(" | ");
                builder.Append(cost.ResourceType);
                builder.Append(" ");
                builder.Append(cost.Amount);
            }

            return builder.ToString();
        }

        private static string FormatTags(CardDetailDisplayData display)
        {
            if (display.Tags.Count == 0)
            {
                return "태그 없음";
            }

            var builder = new StringBuilder();
            builder.Append("태그 ");

            for (var i = 0; i < display.Tags.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(display.Tags[i].DisplayName);
            }

            return builder.ToString();
        }

        private static string FormatPaymentSlots(PurchasePaymentState payment)
        {
            if (payment.Slots.Count == 0)
            {
                return "비용 슬롯 없음";
            }

            var builder = new StringBuilder();
            for (var i = 0; i < payment.Slots.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(" | ");
                }

                builder.Append(FormatPaymentSlot(payment.Slots[i]));
            }

            return builder.ToString();
        }

        private static string FormatPaymentSlot(PaymentSlotState slot)
        {
            var requiredResourceName = ResourceLedger.GetResourceDisplayName(slot.RequiredResourceType);
            var placedResourceName = slot.PlacedResourceType.HasValue
                ? ResourceLedger.GetResourceDisplayName(slot.PlacedResourceType.Value)
                : "비어 있음";

            return requiredResourceName + ": " + placedResourceName;
        }

        private void ShowPaymentSlotButtons(PurchasePaymentState payment)
        {
            for (var i = 0; i < paymentSlotButtons.Count; i++)
            {
                var button = paymentSlotButtons[i];
                var hasSlot = i < payment.Slots.Count;
                SetActive(button != null ? button.gameObject : null, hasSlot);

                if (button == null || !hasSlot)
                {
                    continue;
                }

                button.interactable = payment.Slots[i].IsFilled;
                SetButtonText(button, FormatPaymentSlot(payment.Slots[i]));
            }
        }

        private static void SetActive(GameObject gameObject, bool isActive)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(isActive);
            }
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void SetButtonText(Button button, string value)
        {
            var text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
