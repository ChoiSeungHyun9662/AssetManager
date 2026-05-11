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
            : this(card.Id, card.CashCost, CreateEmptySlots(card.ProfessionalCosts))
        {
        }

        public PurchasePaymentState(string cardId, int cashCost, IEnumerable<PaymentSlotState> slots)
        {
            CardId = cardId;
            CashCost = cashCost;
            Slots = new List<PaymentSlotState>(slots).AsReadOnly();
        }

        public string CardId { get; }
        public int CashCost { get; }
        public IReadOnlyList<PaymentSlotState> Slots { get; }
        public int FinalCashCost => Math.Max(0, CashCost - PlacedDealCount);

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

        private static IReadOnlyList<PaymentSlotState> CreateEmptySlots(
            IEnumerable<ProfessionalResourceCost> professionalCosts)
        {
            var slots = new List<PaymentSlotState>();
            foreach (var cost in professionalCosts)
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
        public static readonly CardDetailState Empty = new CardDetailState(null, null, null, null, false);

        public CardDetailState(
            AssetCardRuntimeData selectedCard,
            PurchaseSource? purchaseSource,
            CardDetailDisplayData displayData,
            PurchasePaymentState pendingPayment,
            bool isOpenedDuringExtraBuy)
        {
            SelectedCard = selectedCard;
            PurchaseSource = purchaseSource;
            DisplayData = displayData;
            PendingPayment = pendingPayment;
            IsOpenedDuringExtraBuy = isOpenedDuringExtraBuy;
        }

        public AssetCardRuntimeData SelectedCard { get; }
        public PurchaseSource? PurchaseSource { get; }
        public CardDetailDisplayData DisplayData { get; }
        public PurchasePaymentState PendingPayment { get; }
        public bool IsOpenedDuringExtraBuy { get; }
        public bool ShouldShowReserveButton =>
            SelectedCard != null
            && PurchaseSource == AssetManager.PurchaseSource.MarketTape
            && !IsOpenedDuringExtraBuy;

        public static CardDetailState Open(
            AssetCardRuntimeData selectedCard,
            PurchaseSource purchaseSource,
            bool isOpenedDuringExtraBuy)
        {
            return new CardDetailState(
                selectedCard,
                purchaseSource,
                new CardDetailDisplayData(selectedCard.Card),
                new PurchasePaymentState(selectedCard.Card),
                isOpenedDuringExtraBuy);
        }
    }
}
