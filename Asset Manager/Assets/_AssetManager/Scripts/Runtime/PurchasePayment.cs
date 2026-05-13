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
            var ownedCard = new AssetCardRuntimeData(
                selectedCard.Card,
                AssetCardRuntimeState.Owned,
                purchaseSource,
                run.OwnedAssets.Count + 1);
            var assetCards = MarkCardOwned(run.AssetCards, ownedCard);
            var ownedAssets = AddOwnedCard(run.OwnedAssets, ownedCard);
            var marketTape = run.MarketTape;
            var reservation = run.Reservation;

            if (purchaseSource == PurchaseSource.MarketTape)
            {
                var zone = FindMarketTapeZone(run.MarketTape, selectedCard.Card.Id).Value;
                var slotIndex = FindMarketTapeSlotIndex(run.MarketTape, zone, selectedCard.Card.Id);
                marketTape = MarketTape.AdvanceSlotAt(
                    run.StaticData.MarketConfig,
                    assetCards,
                    run.MarketTape,
                    ownedAssets,
                    run.Reservation,
                    zone,
                    slotIndex);
            }
            else if (purchaseSource == PurchaseSource.Reserved)
            {
                reservation = RemoveReservedCard(run.Reservation, selectedCard.Card.Id);
            }

            return new PurchasePaymentResult(
                WithConfirmedPurchase(run, payment, assetCards, marketTape, reservation, ownedAssets),
                true,
                string.Empty);
        }

        private static bool CanEditPendingPayment(RunSessionState run)
        {
            return run.State == RunState.Playing
                && run.BusinessDay.Phase == BusinessDayPhase.AwaitingAction
                && run.BusinessDay.MarketArea == MarketAreaState.CardDetail
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
                && !FindMarketTapeZone(run.MarketTape, run.CardDetail.SelectedCard.Card.Id).HasValue)
            {
                return "매수 출처를 찾을 수 없습니다.";
            }

            if (run.CardDetail.PurchaseSource == PurchaseSource.Reserved
                && !ContainsCard(run.Reservation.ReservedCards, run.CardDetail.SelectedCard.Card.Id))
            {
                return "매수 출처를 찾을 수 없습니다.";
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
                    run.CardDetail.IsOpenedDuringExtraBuy));
        }

        private static IReadOnlyList<AssetCardRuntimeData> MarkCardOwned(
            IEnumerable<AssetCardRuntimeData> assetCards,
            AssetCardRuntimeData ownedCard)
        {
            var updatedCards = new List<AssetCardRuntimeData>();
            foreach (var card in assetCards)
            {
                updatedCards.Add(card.Card.Id == ownedCard.Card.Id ? ownedCard : card);
            }

            return updatedCards;
        }

        private static OwnedAssetState AddOwnedCard(OwnedAssetState ownedAssets, AssetCardRuntimeData ownedCard)
        {
            var ownedCards = new List<AssetCardRuntimeData>(ownedAssets.OwnedCards);
            ownedCards.Add(ownedCard);
            return new OwnedAssetState(ownedCards);
        }

        private static ReservationState RemoveReservedCard(ReservationState reservation, string cardId)
        {
            var reservedCards = new List<AssetCardRuntimeData>();
            foreach (var card in reservation.ReservedCards)
            {
                if (card.Card.Id != cardId)
                {
                    reservedCards.Add(card);
                }
            }

            return new ReservationState(reservation.Capacity, reservedCards);
        }

        private static MarketTapeZone? FindMarketTapeZone(MarketTapeState tape, string cardId)
        {
            if (ContainsCard(tape.SellImminentCards, cardId))
            {
                return MarketTapeZone.SellImminent;
            }

            if (ContainsCard(tape.CurrentMarketCards, cardId))
            {
                return MarketTapeZone.CurrentMarket;
            }

            if (ContainsCard(tape.UpcomingMarketCards, cardId))
            {
                return MarketTapeZone.UpcomingMarket;
            }

            return null;
        }

        private static bool ContainsCard(IReadOnlyList<AssetCardRuntimeData> cards, string cardId)
        {
            foreach (var card in cards)
            {
                if (card.Card.Id == cardId)
                {
                    return true;
                }
            }

            return false;
        }

        private static int FindMarketTapeSlotIndex(MarketTapeState tape, MarketTapeZone zone, string cardId)
        {
            var cards = SelectMarketTapeZone(tape, zone);
            for (var i = 0; i < cards.Count; i++)
            {
                if (cards[i].Card.Id == cardId)
                {
                    return i;
                }
            }

            return -1;
        }

        private static IReadOnlyList<AssetCardRuntimeData> SelectMarketTapeZone(MarketTapeState tape, MarketTapeZone zone)
        {
            switch (zone)
            {
                case MarketTapeZone.SellImminent:
                    return tape.SellImminentCards;
                case MarketTapeZone.CurrentMarket:
                    return tape.CurrentMarketCards;
                case MarketTapeZone.UpcomingMarket:
                    return tape.UpcomingMarketCards;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
            }
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

            return BusinessDayFlow.ConsumeBusinessDay(committedRun);
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
