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

        public static RunSessionState OpenMarketPreviewCardDetail(
            RunSessionState run,
            AssetCardRuntimeData selectedCard)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (selectedCard == null)
            {
                throw new ArgumentNullException(nameof(selectedCard));
            }

            return run;
        }

        public static RunSessionState CloseCardDetail(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (run.CardDetail.SelectedCard == null)
            {
                return run;
            }

            if (run.BusinessDay.IsBuyingWithExtraBuy)
            {
                return ExtraBuyAction.ReturnToChoice(run);
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

            var isExtraBuy = run.BusinessDay.IsAwaitingExtraBuyChoice;
            if (isExtraBuy && !ExtraBuyAction.CanPurchaseCandidate(selectedCard))
            {
                return run;
            }

            var cardDetail = CardDetailState.Open(
                selectedCard,
                purchaseSource,
                isExtraBuy,
                run.StaticData.GetInflationCostModifier(run.Calendar.FiscalYear, run.Calendar.Quarter),
                run.InvestmentPhilosophyMastery);

            return isExtraBuy
                ? ExtraBuyAction.BeginPurchase(run, cardDetail)
                : WithMarketArea(run, MarketAreaState.Market, cardDetail);
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
                cardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                run.Missions);
        }
    }
}
