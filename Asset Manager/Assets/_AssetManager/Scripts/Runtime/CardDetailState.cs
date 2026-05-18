using System;
using System.Collections.Generic;

namespace AssetManager
{
    public sealed class CardDetailDisplayData
    {
        public CardDetailDisplayData(AssetCardData card)
        {
            CardId = card.Id;
            DisplayName = card.DisplayName;
            Description = card.Description;
            Rarity = card.Rarity;
            CashCost = card.CashCost;
            ProfessionalCosts = new List<ProfessionalResourceCost>(card.ProfessionalCosts).AsReadOnly();
            ManagementValue = card.ManagementValue;
            Income = card.Income;
            Tags = new List<TagData>(card.Tags).AsReadOnly();
        }

        public string CardId { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public AssetRarity Rarity { get; }
        public int CashCost { get; }
        public IReadOnlyList<ProfessionalResourceCost> ProfessionalCosts { get; }
        public int ManagementValue { get; }
        public int Income { get; }
        public IReadOnlyList<TagData> Tags { get; }
    }

    public sealed class PaymentSlotState
    {
        public PaymentSlotState(ResourceType requiredResourceType, ResourceType? placedResourceType)
        {
            RequiredResourceType = requiredResourceType;
            PlacedResourceType = placedResourceType;
        }

        public ResourceType RequiredResourceType { get; }
        public ResourceType? PlacedResourceType { get; }
        public bool IsFilled => PlacedResourceType.HasValue;
    }

    public sealed class PurchasePaymentState
    {
        public PurchasePaymentState(AssetCardData card)
            : this(card, 0)
        {
        }

        public PurchasePaymentState(AssetCardData card, int inflationCostModifier)
            : this(card.Id, card.CashCost, CreateEmptySlots(card), inflationCostModifier)
        {
        }

        public PurchasePaymentState(string cardId, int cashCost, IEnumerable<PaymentSlotState> slots)
            : this(cardId, cashCost, slots, 0)
        {
        }

        public PurchasePaymentState(
            string cardId,
            int cashCost,
            IEnumerable<PaymentSlotState> slots,
            int inflationCostModifier)
        {
            CardId = cardId;
            CashCost = cashCost;
            Slots = new List<PaymentSlotState>(slots).AsReadOnly();
            InflationCostModifier = inflationCostModifier;
        }

        public string CardId { get; }
        public int CashCost { get; }
        public IReadOnlyList<PaymentSlotState> Slots { get; }
        public int InflationCostModifier { get; }
        public int FinalCashCost => Math.Max(0, CashCost - PlacedDealCount + InflationCostModifier);

        private int PlacedDealCount
        {
            get
            {
                var count = 0;
                foreach (var slot in Slots)
                {
                    if (slot.PlacedResourceType == ResourceType.Deal)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        private static IReadOnlyList<PaymentSlotState> CreateEmptySlots(AssetCardData card)
        {
            if (card.CardDomain == CardDomain.ConsumableResource)
            {
                return Array.Empty<PaymentSlotState>();
            }

            var slots = new List<PaymentSlotState>();
            foreach (var cost in card.ProfessionalCosts)
            {
                for (var i = 0; i < cost.Amount; i++)
                {
                    slots.Add(new PaymentSlotState(cost.ResourceType, null));
                }
            }

            return slots;
        }
    }

    public sealed class CardDetailState
    {
        public static readonly CardDetailState Empty = new CardDetailState(null, null, null, null, false, false);

        public CardDetailState(
            AssetCardRuntimeData selectedCard,
            PurchaseSource? purchaseSource,
            CardDetailDisplayData displayData,
            PurchasePaymentState pendingPayment,
            bool isOpenedDuringExtraBuy,
            bool isPreview)
        {
            SelectedCard = selectedCard;
            PurchaseSource = purchaseSource;
            DisplayData = displayData;
            PendingPayment = pendingPayment;
            IsOpenedDuringExtraBuy = isOpenedDuringExtraBuy;
            IsPreview = isPreview;
        }

        public AssetCardRuntimeData SelectedCard { get; }
        public PurchaseSource? PurchaseSource { get; }
        public CardDetailDisplayData DisplayData { get; }
        public PurchasePaymentState PendingPayment { get; }
        public bool IsOpenedDuringExtraBuy { get; }
        public bool IsPreview { get; }
        public bool ShouldShowBuyButton => SelectedCard != null && !IsPreview;
        public bool ShouldShowReserveButton =>
            SelectedCard != null
            && PurchaseSource == AssetManager.PurchaseSource.MarketTape
            && SelectedCard.State == AssetCardRuntimeState.Available
            && SelectedCard.Card.CardDomain == CardDomain.Stock
            && !IsOpenedDuringExtraBuy
            && !IsPreview;

        public static CardDetailState Open(
            AssetCardRuntimeData selectedCard,
            PurchaseSource purchaseSource,
            bool isOpenedDuringExtraBuy,
            int inflationCostModifier)
        {
            return new CardDetailState(
                selectedCard,
                purchaseSource,
                new CardDetailDisplayData(selectedCard.Card),
                new PurchasePaymentState(selectedCard.Card, inflationCostModifier),
                isOpenedDuringExtraBuy,
                false);
        }

        public static CardDetailState OpenPreview(AssetCardRuntimeData selectedCard)
        {
            return new CardDetailState(
                selectedCard,
                null,
                new CardDetailDisplayData(selectedCard.Card),
                null,
                false,
                true);
        }
    }
}
