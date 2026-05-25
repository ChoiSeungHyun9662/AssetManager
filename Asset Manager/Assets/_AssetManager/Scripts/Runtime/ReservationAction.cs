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
            var assetCards = MarkSingleCardReserved(run.AssetCards, selectedCard.RuntimeId);
            var marketTape = ReserveOnlySlot(run.MarketTape, selectedCard.RuntimeId);

            return new ReservationActionResult(
                WithReservedCard(run, assetCards, marketTape, run.Reservation),
                true,
                string.Empty);
        }

        public static ReservationActionResult UnreserveMarketCard(RunSessionState run, AssetCardRuntimeData selectedCard)
        {
            ValidateRun(run);

            if (selectedCard == null
                || run.State != RunState.Playing
                || run.BusinessDay.Phase != BusinessDayPhase.AwaitingAction
                || run.BusinessDay.MarketArea != MarketAreaState.Market
                || selectedCard.Card.CardDomain != CardDomain.Stock
                || !ContainsReservedMarketTapeCard(run.MarketTape, selectedCard.RuntimeId))
            {
                return new ReservationActionResult(run, false, "예약 해제할 수 없습니다.");
            }

            var assetCards = MarkCardAvailable(run.AssetCards, selectedCard.RuntimeId);
            var marketTape = UnreserveSlot(run.MarketTape, selectedCard.RuntimeId);

            return new ReservationActionResult(
                WithReservedCard(run, assetCards, marketTape, run.Reservation),
                true,
                string.Empty);
        }

        private static string GetReservationValidationMessage(RunSessionState run)
        {
            if (run.State != RunState.Playing
                || run.BusinessDay.Phase != BusinessDayPhase.AwaitingAction
                || run.BusinessDay.MarketArea != MarketAreaState.Market
                || run.CardDetail.SelectedCard == null
                || run.CardDetail.PurchaseSource != PurchaseSource.MarketTape
                || run.CardDetail.IsOpenedDuringExtraBuy)
            {
                return "예약할 수 없습니다.";
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
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                run.Missions);
        }

        private static IReadOnlyList<AssetCardRuntimeData> MarkSingleCardReserved(
            IEnumerable<AssetCardRuntimeData> assetCards,
            string reservedRuntimeId)
        {
            var updatedCards = new List<AssetCardRuntimeData>();
            foreach (var card in assetCards)
            {
                if (card.RuntimeId == reservedRuntimeId)
                {
                    updatedCards.Add(new AssetCardRuntimeData(
                        card.Card,
                        AssetCardRuntimeState.Reserved,
                        null,
                        card.AcquiredOrder,
                        card.IsFoil,
                        card.RuntimeId));
                }
                else if (card.State == AssetCardRuntimeState.Reserved)
                {
                    updatedCards.Add(new AssetCardRuntimeData(
                        card.Card,
                        AssetCardRuntimeState.Available,
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

        private static IReadOnlyList<AssetCardRuntimeData> MarkCardAvailable(
            IEnumerable<AssetCardRuntimeData> assetCards,
            string runtimeId)
        {
            var updatedCards = new List<AssetCardRuntimeData>();
            foreach (var card in assetCards)
            {
                updatedCards.Add(card.RuntimeId == runtimeId
                    ? new AssetCardRuntimeData(
                        card.Card,
                        AssetCardRuntimeState.Available,
                        null,
                        card.AcquiredOrder,
                        card.IsFoil,
                        card.RuntimeId)
                    : card);
            }

            return updatedCards;
        }

        private static MarketTapeState ReserveOnlySlot(MarketTapeState tape, string runtimeId)
        {
            var slots = new List<MarketTapeSlotState>();
            foreach (var slot in tape.Slots)
            {
                if (slot.IsEmpty)
                {
                    slots.Add(slot);
                    continue;
                }

                if (slot.Card.RuntimeId == runtimeId)
                {
                    slots.Add(new MarketTapeSlotState(
                        new AssetCardRuntimeData(
                            slot.Card.Card,
                            AssetCardRuntimeState.Reserved,
                            null,
                            slot.Card.AcquiredOrder,
                            slot.Card.IsFoil,
                            slot.Card.RuntimeId),
                        true));
                }
                else if (slot.IsReserved)
                {
                    slots.Add(new MarketTapeSlotState(
                        new AssetCardRuntimeData(
                            slot.Card.Card,
                            AssetCardRuntimeState.Available,
                            null,
                            slot.Card.AcquiredOrder,
                            slot.Card.IsFoil,
                            slot.Card.RuntimeId),
                        false));
                }
                else
                {
                    slots.Add(slot);
                }
            }

            return new MarketTapeState(slots);
        }

        private static MarketTapeState UnreserveSlot(MarketTapeState tape, string runtimeId)
        {
            var slots = new List<MarketTapeSlotState>();
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty && slot.Card.RuntimeId == runtimeId)
                {
                    slots.Add(new MarketTapeSlotState(
                        new AssetCardRuntimeData(
                            slot.Card.Card,
                            AssetCardRuntimeState.Available,
                            null,
                            slot.Card.AcquiredOrder,
                            slot.Card.IsFoil,
                            slot.Card.RuntimeId),
                        false));
                }
                else
                {
                    slots.Add(slot);
                }
            }

            return new MarketTapeState(slots);
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

        private static bool ContainsReservedMarketTapeCard(MarketTapeState tape, string runtimeId)
        {
            foreach (var slot in tape.Slots)
            {
                if (slot.IsReserved && !slot.IsEmpty && slot.Card.RuntimeId == runtimeId)
                {
                    return true;
                }
            }

            return false;
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
