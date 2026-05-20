using System;
using System.Collections.Generic;

namespace AssetManager
{
    public static class PurchasePayment
    {
        public static PurchasePaymentState CreateForCard(AssetCardData card)
        {
            return CreateForCard(card, 0);
        }

        public static PurchasePaymentState CreateForCard(AssetCardData card, int inflationCostModifier)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            return new PurchasePaymentState(card, inflationCostModifier);
        }

        public static PurchasePaymentResult PlaceChip(RunSessionState run, ResourceType resourceType)
        {
            ValidateRun(run);

            if (!CanEditPendingPayment(run))
            {
                return new PurchasePaymentResult(run, false, "매수 결제 상태가 없습니다.");
            }

            if (!IsPlaceableChip(resourceType))
            {
                return new PurchasePaymentResult(run, false, "배치할 수 없는 자원입니다.");
            }

            var payment = run.CardDetail.PendingPayment;
            if (CountPlaced(payment, resourceType) >= run.Resources.Get(resourceType))
            {
                return new PurchasePaymentResult(run, false, "보유 자원이 부족합니다.");
            }

            var slots = new List<PaymentSlotState>(payment.Slots);
            for (var i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                if (!slot.IsFilled && CanFillSlot(slot, resourceType))
                {
                    slots[i] = new PaymentSlotState(slot.RequiredResourceType, resourceType);
                    return new PurchasePaymentResult(
                        WithPendingPayment(
                            run,
                            new PurchasePaymentState(
                                payment.CardId,
                                payment.CashCost,
                                slots,
                                payment.InflationCostModifier)),
                        true,
                        string.Empty);
                }
            }

            return new PurchasePaymentResult(run, false, "배치할 비용 슬롯이 없습니다.");
        }

        public static PurchasePaymentResult RemoveChip(RunSessionState run, int slotIndex)
        {
            ValidateRun(run);

            if (!CanEditPendingPayment(run))
            {
                return new PurchasePaymentResult(run, false, "매수 결제 상태가 없습니다.");
            }

            var payment = run.CardDetail.PendingPayment;
            if (slotIndex < 0 || slotIndex >= payment.Slots.Count)
            {
                return new PurchasePaymentResult(run, false, "비용 슬롯을 찾을 수 없습니다.");
            }

            var slots = new List<PaymentSlotState>(payment.Slots);
            var slot = slots[slotIndex];
            slots[slotIndex] = new PaymentSlotState(slot.RequiredResourceType, null);

            return new PurchasePaymentResult(
                WithPendingPayment(
                    run,
                    new PurchasePaymentState(
                        payment.CardId,
                        payment.CashCost,
                        slots,
                        payment.InflationCostModifier)),
                true,
                string.Empty);
        }

        public static bool CanConfirmPurchase(RunSessionState run)
        {
            ValidateRun(run);
            return GetPurchaseValidationMessage(run) == string.Empty;
        }

        public static PurchasePaymentResult ConfirmPurchase(RunSessionState run)
        {
            ValidateRun(run);

            var validationMessage = GetPurchaseValidationMessage(run);
            if (validationMessage != string.Empty)
            {
                return new PurchasePaymentResult(run, false, validationMessage);
            }

            var payment = run.CardDetail.PendingPayment;
            var selectedCard = run.CardDetail.SelectedCard;
            var purchaseSource = run.CardDetail.PurchaseSource.Value;

            if (selectedCard.Card.CardDomain == CardDomain.ConsumableResource)
            {
                return ConfirmConsumableResourcePurchase(run, payment, selectedCard, purchaseSource);
            }

            var createsFoil = run.OwnedAssets.WouldCreateFoilFromStockPurchase(selectedCard.Card);
            var ownedCard = new AssetCardRuntimeData(
                selectedCard.Card,
                AssetCardRuntimeState.Owned,
                purchaseSource,
                NextAcquiredOrder(run.OwnedAssets),
                false,
                selectedCard.RuntimeId);
            IReadOnlyList<AssetCardRuntimeData> assetCards;
            OwnedAssetState ownedAssets;
            if (createsFoil)
            {
                var foilMerge = AddFoilOwnedCard(run.OwnedAssets, ownedCard);
                assetCards = ApplyFoilMerge(run.AssetCards, foilMerge);
                ownedAssets = foilMerge.OwnedAssets;
            }
            else
            {
                assetCards = MarkCardOwned(run.AssetCards, ownedCard);
                ownedAssets = AddOwnedCard(run.OwnedAssets, ownedCard);
            }

            if (createsFoil)
            {
                assetCards = RemoveAvailableAndReservedSameStockCards(assetCards, ownedCard.Card.Id, ownedCard.RuntimeId);
            }

            var marketTape = run.MarketTape;
            var reservation = run.Reservation;
            if (createsFoil)
            {
                reservation = RemoveSameStockReservedCards(reservation, ownedCard.Card.Id);
            }

            if (purchaseSource == PurchaseSource.MarketTape)
            {
                var slotIndex = FindMarketTapeSlotIndex(run.MarketTape, selectedCard.RuntimeId);
                marketTape = MarketTape.PullFromSlotAt(
                    run.StaticData.MarketConfig,
                    assetCards,
                    run.MarketTape,
                    ownedAssets,
                    reservation,
                    slotIndex);
            }
            else if (purchaseSource == PurchaseSource.Reserved)
            {
                reservation = RemoveReservedCard(run.Reservation, selectedCard.RuntimeId);
                var zone = FindMarketTapeZone(run.MarketTape, selectedCard.RuntimeId);
                if (zone.HasValue)
                {
                    var slotIndex = FindMarketTapeSlotIndex(run.MarketTape, selectedCard.RuntimeId);
                    marketTape = MarketTape.PullFromSlotAt(
                        run.StaticData.MarketConfig,
                        assetCards,
                        run.MarketTape,
                        ownedAssets,
                        reservation,
                        slotIndex);
                }
            }

            if (createsFoil)
            {
                marketTape = RemoveSameStockFromMarketTape(marketTape, ownedCard.Card.Id);
                marketTape = MarketTape.PullAllEmptySlots(
                    run.StaticData.MarketConfig,
                    assetCards,
                    marketTape,
                    ownedAssets,
                    reservation);
            }

            return new PurchasePaymentResult(
                WithConfirmedPurchase(run, payment, assetCards, marketTape, reservation, ownedAssets),
                true,
                string.Empty);
        }

        private static PurchasePaymentResult ConfirmConsumableResourcePurchase(
            RunSessionState run,
            PurchasePaymentState payment,
            AssetCardRuntimeData selectedCard,
            PurchaseSource purchaseSource)
        {
            var removedCard = new AssetCardRuntimeData(
                selectedCard.Card,
                AssetCardRuntimeState.Removed,
                null,
                selectedCard.AcquiredOrder,
                selectedCard.IsFoil,
                selectedCard.RuntimeId);
            var assetCards = MarkCardRemoved(run.AssetCards, removedCard);
            var marketTape = run.MarketTape;
            var reservation = run.Reservation;

            if (purchaseSource == PurchaseSource.MarketTape)
            {
                var slotIndex = FindMarketTapeSlotIndex(run.MarketTape, selectedCard.RuntimeId);
                marketTape = MarketTape.PullFromSlotAt(
                    run.StaticData.MarketConfig,
                    assetCards,
                    run.MarketTape,
                    run.OwnedAssets,
                    run.Reservation,
                    slotIndex);
            }
            else if (purchaseSource == PurchaseSource.Reserved)
            {
                reservation = RemoveReservedCard(run.Reservation, selectedCard.RuntimeId);
                var zone = FindMarketTapeZone(run.MarketTape, selectedCard.RuntimeId);
                if (zone.HasValue)
                {
                    var slotIndex = FindMarketTapeSlotIndex(run.MarketTape, selectedCard.RuntimeId);
                    marketTape = MarketTape.PullFromSlotAt(
                        run.StaticData.MarketConfig,
                        assetCards,
                        run.MarketTape,
                        run.OwnedAssets,
                        reservation,
                        slotIndex);
                }
            }

            var committedRun = WithConfirmedPurchase(
                run,
                payment,
                assetCards,
                marketTape,
                reservation,
                run.OwnedAssets);
            var rewardedRun = AddConsumableResourceReward(committedRun, selectedCard.Card, out var rewardMessage);

            return new PurchasePaymentResult(rewardedRun, true, rewardMessage);
        }

        private static bool CanEditPendingPayment(RunSessionState run)
        {
            return run.State == RunState.Playing
                && run.BusinessDay.Phase == BusinessDayPhase.AwaitingAction
                && run.BusinessDay.MarketArea == MarketAreaState.Market
                && run.CardDetail.PendingPayment != null;
        }

        private static bool IsPlaceableChip(ResourceType resourceType)
        {
            return resourceType == ResourceType.Research
                || resourceType == ResourceType.Credit
                || resourceType == ResourceType.Commodity
                || resourceType == ResourceType.Deal;
        }

        private static bool CanFillSlot(PaymentSlotState slot, ResourceType resourceType)
        {
            return resourceType == ResourceType.Deal
                || slot.RequiredResourceType == resourceType;
        }

        private static int CountPlaced(PurchasePaymentState payment, ResourceType resourceType)
        {
            var count = 0;
            foreach (var slot in payment.Slots)
            {
                if (slot.PlacedResourceType == resourceType)
                {
                    count++;
                }
            }

            return count;
        }

        private static string GetPurchaseValidationMessage(RunSessionState run)
        {
            if (!CanEditPendingPayment(run)
                || run.CardDetail.SelectedCard == null
                || !run.CardDetail.PurchaseSource.HasValue)
            {
                return "매수할 수 없습니다.";
            }

            if (run.CardDetail.PurchaseSource == PurchaseSource.MarketTape
                && !FindMarketTapeZone(run.MarketTape, run.CardDetail.SelectedCard.RuntimeId).HasValue)
            {
                return "매수 출처를 찾을 수 없습니다.";
            }

            if (run.CardDetail.PurchaseSource == PurchaseSource.Reserved
                && !ContainsCard(run.Reservation.ReservedCards, run.CardDetail.SelectedCard.RuntimeId))
            {
                return "매수 출처를 찾을 수 없습니다.";
            }

            if (run.CardDetail.IsOpenedDuringExtraBuy
                && !ExtraBuyAction.CanPurchaseCandidate(run.CardDetail.SelectedCard))
            {
                return "Extra buy cannot purchase this card.";
            }

            if (!run.OwnedAssets.CanAcceptStockPurchase(run.CardDetail.SelectedCard.Card))
            {
                return "주식 매도가 필요합니다";
            }

            var payment = run.CardDetail.PendingPayment;
            if (!AllSlotsFilled(payment))
            {
                return "비용 슬롯을 모두 채워야 합니다.";
            }

            if (!PlacedResourcesAreAvailable(run.Resources, payment))
            {
                return "보유 자원이 부족합니다.";
            }

            if (payment.FinalCashCost > run.Resources.Cash)
            {
                return "현금이 부족합니다.";
            }

            return string.Empty;
        }

        private static bool AllSlotsFilled(PurchasePaymentState payment)
        {
            foreach (var slot in payment.Slots)
            {
                if (!slot.IsFilled)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool PlacedResourcesAreAvailable(ResourceState resources, PurchasePaymentState payment)
        {
            return CountPlaced(payment, ResourceType.Research) <= resources.Research
                && CountPlaced(payment, ResourceType.Credit) <= resources.Credit
                && CountPlaced(payment, ResourceType.Commodity) <= resources.Commodity
                && CountPlaced(payment, ResourceType.Deal) <= resources.Deal;
        }

        private static void ValidateRun(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }
        }

        private static RunSessionState WithPendingPayment(RunSessionState run, PurchasePaymentState payment)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                new CardDetailState(
                    run.CardDetail.SelectedCard,
                    run.CardDetail.PurchaseSource,
                    run.CardDetail.DisplayData,
                    payment,
                    run.CardDetail.IsOpenedDuringExtraBuy,
                    run.CardDetail.IsPreview));
        }

        private static IReadOnlyList<AssetCardRuntimeData> MarkCardOwned(
            IEnumerable<AssetCardRuntimeData> assetCards,
            AssetCardRuntimeData ownedCard)
        {
            var updatedCards = new List<AssetCardRuntimeData>();
            foreach (var card in assetCards)
            {
                updatedCards.Add(card.RuntimeId == ownedCard.RuntimeId ? ownedCard : card);
            }

            return updatedCards;
        }

        private static IReadOnlyList<AssetCardRuntimeData> MarkCardRemoved(
            IEnumerable<AssetCardRuntimeData> assetCards,
            AssetCardRuntimeData removedCard)
        {
            var updatedCards = new List<AssetCardRuntimeData>();
            foreach (var card in assetCards)
            {
                updatedCards.Add(card.RuntimeId == removedCard.RuntimeId ? removedCard : card);
            }

            return updatedCards;
        }

        private static IReadOnlyList<AssetCardRuntimeData> RemoveAvailableAndReservedSameStockCards(
            IEnumerable<AssetCardRuntimeData> assetCards,
            string stockId,
            string purchasedRuntimeId)
        {
            var updatedCards = new List<AssetCardRuntimeData>();
            foreach (var card in assetCards)
            {
                if (card.Card.Id == stockId
                    && card.RuntimeId != purchasedRuntimeId
                    && card.State != AssetCardRuntimeState.Owned)
                {
                    updatedCards.Add(new AssetCardRuntimeData(
                        card.Card,
                        AssetCardRuntimeState.Removed,
                        null,
                        card.AcquiredOrder,
                        card.IsFoil,
                        card.RuntimeId));
                }
                else
                {
                    updatedCards.Add(card);
                }
            }

            return updatedCards;
        }

        private static OwnedAssetState AddOwnedCard(OwnedAssetState ownedAssets, AssetCardRuntimeData ownedCard)
        {
            var slots = new List<AssetCardRuntimeData>(ownedAssets.StockSlots);
            for (var i = 0; i < slots.Count; i++)
            {
                if (slots[i] == null || slots[i].State != AssetCardRuntimeState.Owned)
                {
                    slots[i] = ownedCard;
                    return new OwnedAssetState(slots);
                }
            }

            slots.Add(ownedCard);
            return new OwnedAssetState(slots);
        }

        private static FoilMergeResult AddFoilOwnedCard(OwnedAssetState ownedAssets, AssetCardRuntimeData ownedCard)
        {
            var slots = new List<AssetCardRuntimeData>(ownedAssets.StockSlots);
            var earliestIndex = -1;
            var earliestOrder = int.MaxValue;

            for (var i = 0; i < slots.Count; i++)
            {
                var card = slots[i];
                if (card != null
                    && card.State == AssetCardRuntimeState.Owned
                    && !card.IsFoil
                    && card.Card.Id == ownedCard.Card.Id)
                {
                    var acquiredOrder = card.AcquiredOrder ?? int.MaxValue;
                    if (acquiredOrder < earliestOrder)
                    {
                        earliestIndex = i;
                        earliestOrder = acquiredOrder;
                    }
                }
            }

            if (earliestIndex < 0)
            {
                var fallback = new OwnedAssetState(new[] { ownedCard });
                return new FoilMergeResult(fallback, ownedCard, new[] { ownedCard.RuntimeId });
            }

            var earliestCard = slots[earliestIndex];
            var foilCard = new AssetCardRuntimeData(
                ownedCard.Card,
                AssetCardRuntimeState.Owned,
                earliestCard.PurchaseSource,
                earliestCard.AcquiredOrder ?? ownedCard.AcquiredOrder,
                true,
                earliestCard.RuntimeId);
            var consumedRuntimeIds = new List<string> { ownedCard.RuntimeId };

            for (var i = 0; i < slots.Count; i++)
            {
                var card = slots[i];
                if (card == null
                    || card.State != AssetCardRuntimeState.Owned
                    || card.IsFoil
                    || card.Card.Id != ownedCard.Card.Id)
                {
                    continue;
                }

                if (i == earliestIndex)
                {
                    slots[i] = foilCard;
                }
                else
                {
                    consumedRuntimeIds.Add(card.RuntimeId);
                    slots[i] = null;
                }
            }

            return new FoilMergeResult(new OwnedAssetState(slots), foilCard, consumedRuntimeIds);
        }

        private static IReadOnlyList<AssetCardRuntimeData> ApplyFoilMerge(
            IEnumerable<AssetCardRuntimeData> assetCards,
            FoilMergeResult foilMerge)
        {
            var consumedRuntimeIds = new HashSet<string>(foilMerge.ConsumedRuntimeIds);
            var updatedCards = new List<AssetCardRuntimeData>();
            foreach (var card in assetCards)
            {
                if (card.RuntimeId == foilMerge.FoilCard.RuntimeId)
                {
                    updatedCards.Add(foilMerge.FoilCard);
                }
                else if (consumedRuntimeIds.Contains(card.RuntimeId))
                {
                    updatedCards.Add(new AssetCardRuntimeData(
                        card.Card,
                        AssetCardRuntimeState.Removed,
                        null,
                        card.AcquiredOrder,
                        card.IsFoil,
                        card.RuntimeId));
                }
                else
                {
                    updatedCards.Add(card);
                }
            }

            return updatedCards;
        }

        private static int NextAcquiredOrder(OwnedAssetState ownedAssets)
        {
            var highestOrder = 0;
            foreach (var slot in ownedAssets.StockSlots)
            {
                if (slot != null && slot.AcquiredOrder.HasValue && slot.AcquiredOrder.Value > highestOrder)
                {
                    highestOrder = slot.AcquiredOrder.Value;
                }
            }

            return highestOrder + 1;
        }

        private static ReservationState RemoveReservedCard(ReservationState reservation, string runtimeId)
        {
            var reservedCards = new List<AssetCardRuntimeData>();
            foreach (var card in reservation.ReservedCards)
            {
                if (card.RuntimeId != runtimeId)
                {
                    reservedCards.Add(card);
                }
            }

            return new ReservationState(reservation.Capacity, reservedCards);
        }

        private static ReservationState RemoveSameStockReservedCards(ReservationState reservation, string stockId)
        {
            var reservedCards = new List<AssetCardRuntimeData>();
            foreach (var card in reservation.ReservedCards)
            {
                if (card.Card.Id != stockId)
                {
                    reservedCards.Add(card);
                }
            }

            return new ReservationState(reservation.Capacity, reservedCards);
        }

        private static MarketTapeState RemoveSameStockFromMarketTape(MarketTapeState tape, string stockId)
        {
            var slots = new List<MarketTapeSlotState>();
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty && slot.Card.Card.Id == stockId)
                {
                    slots.Add(new MarketTapeSlotState(null, false));
                }
                else
                {
                    slots.Add(slot);
                }
            }

            return new MarketTapeState(slots);
        }

        private static MarketTapeZone? FindMarketTapeZone(MarketTapeState tape, string runtimeId)
        {
            if (FindMarketTapeSlotIndex(tape, runtimeId) >= 0)
            {
                return MarketTapeZone.CurrentMarket;
            }

            if (ContainsCard(tape.SellImminentCards, runtimeId))
            {
                return MarketTapeZone.SellImminent;
            }

            if (ContainsCard(tape.CurrentMarketCards, runtimeId))
            {
                return MarketTapeZone.CurrentMarket;
            }

            if (ContainsCard(tape.UpcomingMarketCards, runtimeId))
            {
                return MarketTapeZone.UpcomingMarket;
            }

            return null;
        }

        private static bool ContainsCard(IReadOnlyList<AssetCardRuntimeData> cards, string runtimeId)
        {
            foreach (var card in cards)
            {
                if (card.RuntimeId == runtimeId)
                {
                    return true;
                }
            }

            return false;
        }

        private static int FindMarketTapeSlotIndex(MarketTapeState tape, string runtimeId)
        {
            for (var i = 0; i < tape.Slots.Count; i++)
            {
                if (!tape.Slots[i].IsEmpty && tape.Slots[i].Card.RuntimeId == runtimeId)
                {
                    return i;
                }
            }

            return -1;
        }

        private static RunSessionState WithConfirmedPurchase(
            RunSessionState run,
            PurchasePaymentState payment,
            IReadOnlyList<AssetCardRuntimeData> assetCards,
            MarketTapeState marketTape,
            ReservationState reservation,
            OwnedAssetState ownedAssets)
        {
            var committedRun = new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                new ResourceState(
                    run.Resources.Cash - payment.FinalCashCost,
                    run.Resources.Research - CountPlaced(payment, ResourceType.Research),
                    run.Resources.Credit - CountPlaced(payment, ResourceType.Credit),
                    run.Resources.Commodity - CountPlaced(payment, ResourceType.Commodity),
                    run.Resources.Deal - CountPlaced(payment, ResourceType.Deal)),
                run.Performance,
                assetCards,
                marketTape,
                reservation,
                ownedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                CardDetailState.Empty);

            if (!run.CardDetail.IsOpenedDuringExtraBuy && ExtraBuyAction.CanGrantFrom(run.CardDetail.SelectedCard.Card))
            {
                return ExtraBuyAction.BeginChoice(committedRun);
            }

            return BusinessDayFlow.ConsumeBusinessDay(committedRun);
        }

        private static RunSessionState AddConsumableResourceReward(
            RunSessionState run,
            AssetCardData card,
            out string message)
        {
            if (card.ProvidedResourceType == ResourceType.Cash)
            {
                message = string.Empty;
                return ResourceLedger.AddFundingCash(run, card.ProvidedResourceAmount);
            }

            var result = ResourceLedger.AddInvestmentPhilosophy(
                run,
                card.ProvidedResourceType,
                card.ProvidedResourceAmount);
            message = result.Message;
            return result.Run;
        }

        private sealed class FoilMergeResult
        {
            public FoilMergeResult(
                OwnedAssetState ownedAssets,
                AssetCardRuntimeData foilCard,
                IEnumerable<string> consumedRuntimeIds)
            {
                OwnedAssets = ownedAssets;
                FoilCard = foilCard;
                ConsumedRuntimeIds = new List<string>(consumedRuntimeIds).AsReadOnly();
            }

            public OwnedAssetState OwnedAssets { get; }
            public AssetCardRuntimeData FoilCard { get; }
            public IReadOnlyList<string> ConsumedRuntimeIds { get; }
        }
    }

    public sealed class PurchasePaymentResult
    {
        public PurchasePaymentResult(RunSessionState run, bool succeeded, string message)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            Succeeded = succeeded;
            Message = message ?? string.Empty;
        }

        public RunSessionState Run { get; }
        public bool Succeeded { get; }
        public string Message { get; }
    }
}
