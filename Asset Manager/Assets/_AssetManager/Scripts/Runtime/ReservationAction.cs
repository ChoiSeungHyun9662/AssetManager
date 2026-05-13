using System;
using System.Collections.Generic;

namespace AssetManager
{
    public static class ReservationAction
    {
        public static bool CanReserve(RunSessionState run)
        {
            ValidateRun(run);
            return GetReservationValidationMessage(run) == string.Empty;
        }

        public static ReservationActionResult ConfirmReservation(RunSessionState run)
        {
            ValidateRun(run);

            var validationMessage = GetReservationValidationMessage(run);
            if (validationMessage != string.Empty)
            {
                return new ReservationActionResult(run, false, validationMessage);
            }

            var selectedCard = run.CardDetail.SelectedCard;
            var zone = FindMarketTapeZone(run.MarketTape, selectedCard.Card.Id).Value;
            var slotIndex = FindMarketTapeSlotIndex(run.MarketTape, zone, selectedCard.Card.Id);
            var reservedCard = new AssetCardRuntimeData(selectedCard.Card, AssetCardRuntimeState.Reserved, null);
            var assetCards = MarkCardReserved(run.AssetCards, reservedCard);
            var reservation = AddReservedCard(run.Reservation, reservedCard);
            var marketTape = MarketTape.AdvanceSlotAt(
                run.StaticData.MarketConfig,
                assetCards,
                run.MarketTape,
                run.OwnedAssets,
                reservation,
                zone,
                slotIndex);

            var reservedRun = WithReservedCard(run, assetCards, marketTape, reservation);
            var dealResult = ResourceLedger.AddDeal(reservedRun, 1);
            var pressureResult = RedemptionPressure.AddPressure(dealResult.Run, 1);
            var pressuredRun = pressureResult.Run;
            var message = dealResult.Message;

            if (pressureResult.DidFail)
            {
                return new ReservationActionResult(pressuredRun, true, "대규모 환매: 환매 압력 한도 도달");
            }

            if (message == string.Empty)
            {
                message = "환매 압력 +1";
            }

            return new ReservationActionResult(BusinessDayFlow.ConsumeBusinessDay(pressuredRun), true, message);
        }

        private static string GetReservationValidationMessage(RunSessionState run)
        {
            if (run.State != RunState.Playing
                || run.BusinessDay.Phase != BusinessDayPhase.AwaitingAction
                || run.BusinessDay.MarketArea != MarketAreaState.CardDetail
                || run.CardDetail.SelectedCard == null
                || run.CardDetail.PurchaseSource != PurchaseSource.MarketTape
                || run.CardDetail.IsOpenedDuringExtraBuy)
            {
                return "예약할 수 없습니다.";
            }

            if (run.Reservation.ReservedCards.Count >= run.Reservation.Capacity)
            {
                return "예약 구역이 가득 찼습니다.";
            }

            if (run.CardDetail.SelectedCard.State != AssetCardRuntimeState.Available
                || !FindMarketTapeZone(run.MarketTape, run.CardDetail.SelectedCard.Card.Id).HasValue)
            {
                return "예약할 시장 카드를 찾을 수 없습니다.";
            }

            return string.Empty;
        }

        private static RunSessionState WithReservedCard(
            RunSessionState run,
            IReadOnlyList<AssetCardRuntimeData> assetCards,
            MarketTapeState marketTape,
            ReservationState reservation)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                assetCards,
                marketTape,
                reservation,
                run.OwnedAssets,
                new BusinessDayState(run.BusinessDay.Phase, MarketAreaState.Market),
                run.RedemptionPressure,
                CardDetailState.Empty,
                run.LiquidityAction);
        }

        private static IReadOnlyList<AssetCardRuntimeData> MarkCardReserved(
            IEnumerable<AssetCardRuntimeData> assetCards,
            AssetCardRuntimeData reservedCard)
        {
            var updatedCards = new List<AssetCardRuntimeData>();
            foreach (var card in assetCards)
            {
                updatedCards.Add(card.Card.Id == reservedCard.Card.Id ? reservedCard : card);
            }

            return updatedCards;
        }

        private static ReservationState AddReservedCard(ReservationState reservation, AssetCardRuntimeData reservedCard)
        {
            var reservedCards = new List<AssetCardRuntimeData>(reservation.ReservedCards);
            reservedCards.Add(reservedCard);
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

        private static void ValidateRun(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }
        }
    }

    public sealed class ReservationActionResult
    {
        public ReservationActionResult(RunSessionState run, bool succeeded, string message)
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
