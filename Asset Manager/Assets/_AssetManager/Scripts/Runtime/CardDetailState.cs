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
            Value = card.Value;
            Income = card.Income;
            Tags = new List<TagData>(card.Tags).AsReadOnly();
        }

        public string CardId { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public AssetRarity Rarity { get; }
        public int CashCost { get; }
        public IReadOnlyList<ProfessionalResourceCost> ProfessionalCosts { get; }
        public int Value { get; }
        public int ManagementValue => Value;
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

    public sealed class PurchaseCostToken
    {
        public PurchaseCostToken(ResourceType resourceType, int amount, bool isInsufficient)
        {
            ResourceType = resourceType;
            Amount = amount;
            IsInsufficient = isInsufficient;
        }

        public ResourceType ResourceType { get; }
        public int Amount { get; }
        public bool IsInsufficient { get; }
    }

    public sealed class PurchaseCostBreakdown
    {
        public PurchaseCostBreakdown(
            PurchaseCostToken cashToken,
            IEnumerable<PurchaseCostToken> originalPhilosophyCosts,
            IEnumerable<PurchaseCostToken> discountedPhilosophyCosts,
            bool hasDiscount)
        {
            CashToken = cashToken;
            OriginalPhilosophyCosts = new List<PurchaseCostToken>(originalPhilosophyCosts).AsReadOnly();
            DiscountedPhilosophyCosts = new List<PurchaseCostToken>(discountedPhilosophyCosts).AsReadOnly();
            HasDiscount = hasDiscount;
        }

        public PurchaseCostToken CashToken { get; }
        public IReadOnlyList<PurchaseCostToken> OriginalPhilosophyCosts { get; }
        public IReadOnlyList<PurchaseCostToken> DiscountedPhilosophyCosts { get; }
        public bool HasDiscount { get; }

        public string Format()
        {
            var parts = new List<string> { CashToken.Amount + "$" };
            AppendCostTokens(parts, OriginalPhilosophyCosts);

            if (!HasDiscount)
            {
                return string.Join(", ", parts);
            }

            var discountedParts = new List<string>();
            AppendCostTokens(discountedParts, DiscountedPhilosophyCosts);
            return string.Join(", ", parts) + " -> " + string.Join(", ", discountedParts);
        }

        private static void AppendCostTokens(List<string> parts, IReadOnlyList<PurchaseCostToken> tokens)
        {
            foreach (var token in tokens)
            {
                parts.Add(GetResourceToken(token.ResourceType) + token.Amount);
            }
        }

        private static string GetResourceToken(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Reading:
                    return "R";
                case ResourceType.Meditation:
                    return "M";
                case ResourceType.Patience:
                    return "P";
                default:
                    return resourceType.ToString();
            }
        }
    }

    public static class PurchaseCostCalculator
    {
        public static PurchaseCostBreakdown Calculate(
            AssetCardData card,
            ResourceState resources,
            InvestmentPhilosophyMasteryState mastery,
            int inflationCostModifier)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            mastery = mastery ?? InvestmentPhilosophyMasteryState.Empty;
            var finalCashCost = Math.Max(0, card.CashCost + inflationCostModifier);
            var originalCosts = AggregatePhilosophyCosts(card.ProfessionalCosts, resources, InvestmentPhilosophyMasteryState.Empty);
            var discountedCosts = AggregatePhilosophyCosts(card.ProfessionalCosts, resources, mastery);

            return new PurchaseCostBreakdown(
                new PurchaseCostToken(ResourceType.Cash, finalCashCost, resources.Cash < finalCashCost),
                originalCosts,
                discountedCosts,
                HasDiscount(originalCosts, discountedCosts));
        }

        public static IReadOnlyList<PaymentSlotState> CreateDiscountedSlots(
            AssetCardData card,
            InvestmentPhilosophyMasteryState mastery)
        {
            mastery = mastery ?? InvestmentPhilosophyMasteryState.Empty;
            var slots = new List<PaymentSlotState>();
            foreach (var cost in DiscountPhilosophyCosts(card.ProfessionalCosts, mastery))
            {
                for (var i = 0; i < cost.Amount; i++)
                {
                    slots.Add(new PaymentSlotState(cost.ResourceType, null));
                }
            }

            return slots;
        }

        private static IReadOnlyList<PurchaseCostToken> AggregatePhilosophyCosts(
            IReadOnlyList<ProfessionalResourceCost> costs,
            ResourceState resources,
            InvestmentPhilosophyMasteryState mastery)
        {
            var amounts = new Dictionary<ResourceType, int>();
            foreach (var cost in DiscountPhilosophyCosts(costs, mastery))
            {
                amounts[cost.ResourceType] = cost.Amount;
            }
            var tokens = new List<PurchaseCostToken>();
            AddToken(tokens, amounts, resources, ResourceType.Reading);
            AddToken(tokens, amounts, resources, ResourceType.Meditation);
            AddToken(tokens, amounts, resources, ResourceType.Patience);
            return tokens;
        }

        private static IReadOnlyList<ProfessionalResourceCost> DiscountPhilosophyCosts(
            IReadOnlyList<ProfessionalResourceCost> costs,
            InvestmentPhilosophyMasteryState mastery)
        {
            var originalAmounts = new Dictionary<ResourceType, int>();
            foreach (var cost in costs)
            {
                if (!originalAmounts.ContainsKey(cost.ResourceType))
                {
                    originalAmounts[cost.ResourceType] = 0;
                }

                originalAmounts[cost.ResourceType] += cost.Amount;
            }

            var discountedCosts = new List<ProfessionalResourceCost>();
            AddDiscountedCost(discountedCosts, originalAmounts, mastery, ResourceType.Reading);
            AddDiscountedCost(discountedCosts, originalAmounts, mastery, ResourceType.Meditation);
            AddDiscountedCost(discountedCosts, originalAmounts, mastery, ResourceType.Patience);
            return discountedCosts;
        }

        private static void AddDiscountedCost(
            List<ProfessionalResourceCost> costs,
            Dictionary<ResourceType, int> originalAmounts,
            InvestmentPhilosophyMasteryState mastery,
            ResourceType resourceType)
        {
            if (!originalAmounts.TryGetValue(resourceType, out var amount))
            {
                return;
            }

            costs.Add(new ProfessionalResourceCost(resourceType, Math.Max(0, amount - mastery.Get(resourceType))));
        }

        private static void AddToken(
            List<PurchaseCostToken> tokens,
            Dictionary<ResourceType, int> amounts,
            ResourceState resources,
            ResourceType resourceType)
        {
            if (!amounts.TryGetValue(resourceType, out var amount))
            {
                return;
            }

            tokens.Add(new PurchaseCostToken(resourceType, amount, amount > resources.Get(resourceType)));
        }

        private static bool HasDiscount(
            IReadOnlyList<PurchaseCostToken> originalCosts,
            IReadOnlyList<PurchaseCostToken> discountedCosts)
        {
            if (originalCosts.Count != discountedCosts.Count)
            {
                return true;
            }

            for (var i = 0; i < originalCosts.Count; i++)
            {
                if (originalCosts[i].ResourceType != discountedCosts[i].ResourceType
                    || originalCosts[i].Amount != discountedCosts[i].Amount)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class PurchasePaymentState
    {
        public PurchasePaymentState(AssetCardData card)
            : this(card, 0)
        {
        }

        public PurchasePaymentState(AssetCardData card, int inflationCostModifier)
            : this(card, InvestmentPhilosophyMasteryState.Empty, inflationCostModifier)
        {
        }

        public PurchasePaymentState(
            AssetCardData card,
            InvestmentPhilosophyMasteryState mastery,
            int inflationCostModifier)
            : this(card.Id, card.CashCost, PurchaseCostCalculator.CreateDiscountedSlots(card, mastery), inflationCostModifier)
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
        public int FinalCashCost => Math.Max(0, CashCost + InflationCostModifier);

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
            int inflationCostModifier,
            InvestmentPhilosophyMasteryState mastery)
        {
            return new CardDetailState(
                selectedCard,
                purchaseSource,
                new CardDetailDisplayData(selectedCard.Card),
                new PurchasePaymentState(selectedCard.Card, mastery, inflationCostModifier),
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
