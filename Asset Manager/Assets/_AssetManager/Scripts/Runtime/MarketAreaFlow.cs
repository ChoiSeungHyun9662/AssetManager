using System;

namespace AssetManager
{
    public static class MarketAreaFlow
    {
        public static RunSessionState OpenMarketCardDetail(
            RunSessionState run,
            AssetCardRuntimeData selectedCard)
        {
            return OpenCardDetail(run, selectedCard, PurchaseSource.MarketTape);
        }

        public static RunSessionState OpenReservedCardDetail(
            RunSessionState run,
            AssetCardRuntimeData selectedCard)
        {
            return OpenCardDetail(run, selectedCard, PurchaseSource.Reserved);
        }

        public static RunSessionState CloseCardDetail(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (run.BusinessDay.MarketArea != MarketAreaState.CardDetail)
            {
                return run;
            }

            return WithMarketArea(run, MarketAreaState.Market, CardDetailState.Empty);
        }

        public static bool CanAdvanceToNextBusinessDay(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            return run.State == RunState.Playing
                && run.BusinessDay.Phase == BusinessDayPhase.AwaitingAction
                && run.BusinessDay.MarketArea == MarketAreaState.Market
                && run.Calendar.RemainingBusinessDays > 0;
        }

        private static RunSessionState OpenCardDetail(
            RunSessionState run,
            AssetCardRuntimeData selectedCard,
            PurchaseSource purchaseSource)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (selectedCard == null)
            {
                throw new ArgumentNullException(nameof(selectedCard));
            }

            if (run.BusinessDay.Phase != BusinessDayPhase.AwaitingAction
                || run.BusinessDay.MarketArea != MarketAreaState.Market)
            {
                return run;
            }

            return WithMarketArea(
                run,
                MarketAreaState.CardDetail,
                CardDetailState.Open(selectedCard, purchaseSource, false));
        }

        private static RunSessionState WithMarketArea(
            RunSessionState run,
            MarketAreaState marketArea,
            CardDetailState cardDetail)
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
                new BusinessDayState(run.BusinessDay.Phase, marketArea),
                run.RedemptionPressure,
                cardDetail);
        }
    }
}
