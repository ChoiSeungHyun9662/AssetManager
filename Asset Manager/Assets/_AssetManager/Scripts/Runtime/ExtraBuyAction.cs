namespace AssetManager
{
    public static class ExtraBuyAction
    {
        public static bool CanGrantFrom(AssetCardData card)
        {
            return card != null && card.GrantsExtraBuyAction;
        }

        public static bool CanPurchaseCandidate(AssetCardRuntimeData card)
        {
            return card != null
                && card.State == AssetCardRuntimeState.Available
                && card.Card.CanBePurchasedWithExtraBuy;
        }

        public static RunSessionState BeginChoice(RunSessionState run)
        {
            return WithBusinessDay(
                run,
                new BusinessDayState(
                    run.BusinessDay.Phase,
                    MarketAreaState.Market,
                    true,
                    true,
                    false),
                CardDetailState.Empty);
        }

        public static RunSessionState BeginPurchase(RunSessionState run, CardDetailState cardDetail)
        {
            return WithBusinessDay(
                run,
                new BusinessDayState(
                    run.BusinessDay.Phase,
                    MarketAreaState.Market,
                    true,
                    false,
                    true),
                cardDetail);
        }

        public static RunSessionState ReturnToChoice(RunSessionState run)
        {
            return BeginChoice(run);
        }

        private static RunSessionState WithBusinessDay(
            RunSessionState run,
            BusinessDayState businessDay,
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
                businessDay,
                run.RedemptionPressure,
                cardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);
        }
    }
}
