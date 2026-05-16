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
        private GameObject paymentPotBackground;

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
            GameObject paymentPot,
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
            paymentPotBackground = paymentPot;
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
            var payment = run.CardDetail.PendingPayment;
            var isTransactionDetail = run.CardDetail.ShouldShowBuyButton && payment != null;
            SetText(nameText, display.DisplayName);
            SetText(descriptionText, display.Description);
            SetText(costText, FormatCosts(display));
            SetText(managementValueText, "장기\n운용가치 " + display.ManagementValue);
            SetText(incomeText, "단기\n운용 수익 " + display.Income);
            SetText(tagsText, FormatTags(display));
            SetText(rarityText, "희귀도 " + display.Rarity);
            SetActive(paymentPotBackground, isTransactionDetail);
            SetActive(paymentSlotsText != null ? paymentSlotsText.gameObject : null, isTransactionDetail);
            SetActive(finalCashCostText != null ? finalCashCostText.gameObject : null, isTransactionDetail);
            if (isTransactionDetail)
            {
                SetText(paymentSlotsText, "Payment Pot\n" + FormatPaymentSlots(payment));
                SetText(finalCashCostText, FormatFinalCashCost(payment));
            }

            SetActive(buyButton != null ? buyButton.gameObject : null, isTransactionDetail);
            if (buyButton != null)
            {
                buyButton.interactable = isTransactionDetail && PurchasePayment.CanConfirmPurchase(run);
            }

            SetActive(reserveButton != null ? reserveButton.gameObject : null, run.CardDetail.ShouldShowReserveButton);
            if (reserveButton != null)
            {
                reserveButton.interactable = ReservationAction.CanReserve(run);
            }

            SetActive(placeResearchButton != null ? placeResearchButton.gameObject : null, isTransactionDetail);
            SetActive(placeCreditButton != null ? placeCreditButton.gameObject : null, isTransactionDetail);
            SetActive(placeCommodityButton != null ? placeCommodityButton.gameObject : null, isTransactionDetail);
            SetActive(placeDealButton != null ? placeDealButton.gameObject : null, isTransactionDetail);
            ShowPaymentSlotButtons(payment);
        }

        private static string FormatCosts(CardDetailDisplayData display)
        {
            var builder = new StringBuilder();
            builder.AppendLine("매수 비용");
            builder.Append("현금 ");
            builder.Append(display.CashCost);
            builder.AppendLine();
            builder.Append(FormatProfessionalCosts(display.ProfessionalCosts));
            return builder.ToString();
        }

        private static string FormatProfessionalCosts(IReadOnlyList<ProfessionalResourceCost> costs)
        {
            if (costs.Count == 0)
            {
                return "전문자원 없음";
            }

            var builder = new StringBuilder();
            builder.Append("전문자원 ");
            for (var i = 0; i < costs.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(ResourceLedger.GetResourceDisplayName(costs[i].ResourceType));
                builder.Append(" ");
                builder.Append(costs[i].Amount);
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
                    builder.Append(" / ");
                }

                builder.Append(display.Tags[i].DisplayName);
            }

            return builder.ToString();
        }

        private static string FormatFinalCashCost(PurchasePaymentState payment)
        {
            var placedDealCount = CountPlacedDeal(payment);
            return "최종 현금 "
                + payment.FinalCashCost
                + "\n기본 "
                + payment.CashCost
                + " - 딜 "
                + placedDealCount
                + " + 인플레 "
                + payment.InflationCostModifier;
        }

        private static int CountPlacedDeal(PurchasePaymentState payment)
        {
            var count = 0;
            foreach (var slot in payment.Slots)
            {
                if (slot.PlacedResourceType == ResourceType.Deal)
                {
                    count++;
                }
            }

            return count;
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
                var hasSlot = payment != null && i < payment.Slots.Count;
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
