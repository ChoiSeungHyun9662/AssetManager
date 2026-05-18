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
            var reservedCard = new AssetCardRuntimeData(
                selectedCard.Card,
                AssetCardRuntimeState.Reserved,
                null,
                selectedCard.AcquiredOrder,
                selectedCard.IsFoil,
                selectedCard.RuntimeId);
            var assetCards = MarkCardReserved(run.AssetCards, reservedCard);
            var marketTape = MarketTape.ReserveSlot(run.MarketTape, selectedCard.RuntimeId, reservedCard);

            var reservedRun = WithReservedCard(run, assetCards, marketTape, run.Reservation);
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

            return new ReservationActionResult(ConsumeBusinessDayWithoutMarketAdvance(pressuredRun), true, message);
        }

        private static RunSessionState ConsumeBusinessDayWithoutMarketAdvance(RunSessionState run)
        {
            if (run.State != RunState.Playing
                || run.BusinessDay.Phase != BusinessDayPhase.AwaitingAction
                || run.Calendar.RemainingBusinessDays <= 0)
            {
                return run;
            }

            var remainingBusinessDays = run.Calendar.RemainingBusinessDays - 1;
            var nextPhase = remainingBusinessDays == 0
                ? BusinessDayPhase.QuarterSettlement
                : BusinessDayPhase.AwaitingAction;

            var nextRun = new RunSessionState(
                run.State,
                run.StaticData,
                new RunCalendarState(
                    run.Calendar.FiscalYear,
                    run.Calendar.Quarter,
                    remainingBusinessDays),
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                new BusinessDayState(nextPhase, MarketAreaState.Market),
                run.RedemptionPressure);

            return nextPhase == BusinessDayPhase.QuarterSettlement
                ? QuarterSettlement.Settle(nextRun).Run
                : nextRun;
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

            if (CountReservedSlots(run.MarketTape) >= run.Reservation.Capacity)
            {
                return "예약 구역이 가득 찼습니다.";
            }

            if (run.CardDetail.SelectedCard.Card.CardDomain != CardDomain.Stock)
            {
                return "주식만 예약할 수 있습니다.";
            }

            if (run.CardDetail.SelectedCard.State != AssetCardRuntimeState.Available
                || !ContainsMarketTapeCard(run.MarketTape, run.CardDetail.SelectedCard.RuntimeId))
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
                updatedCards.Add(card.RuntimeId == reservedCard.RuntimeId ? reservedCard : card);
            }

            return updatedCards;
        }

        private static bool ContainsMarketTapeCard(MarketTapeState tape, string runtimeId)
        {
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty && slot.Card.RuntimeId == runtimeId)
                {
                    return true;
                }
            }

            return false;
        }

        private static int CountReservedSlots(MarketTapeState tape)
        {
            var count = 0;
            foreach (var slot in tape.Slots)
            {
                if (slot.IsReserved)
                {
                    count++;
                }
            }

            return count;
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
