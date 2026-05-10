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

    public sealed class PurchasePaymentState
    {
        public PurchasePaymentState(AssetCardData card)
        {
            CardId = card.Id;
            CashCost = card.CashCost;
            ProfessionalCosts = new List<ProfessionalResourceCost>(card.ProfessionalCosts).AsReadOnly();
        }

        public string CardId { get; }
        public int CashCost { get; }
        public IReadOnlyList<ProfessionalResourceCost> ProfessionalCosts { get; }
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
