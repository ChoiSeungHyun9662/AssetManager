using System;
using System.Collections.Generic;

namespace AssetManager
{
    public static class RunBootstrapper
    {
        public static RunSessionState CreateNewRun(RunStaticDataSet staticData)
        {
            if (staticData == null)
            {
                throw new ArgumentNullException(nameof(staticData));
            }

            if (!staticData.HasRequiredMvpData)
            {
                throw new InvalidOperationException("Run static data is missing required MVP bootstrap data.");
            }

            var openingQuarter = staticData.Quarters[0];
            var resourceConfig = staticData.ResourceConfig;
            var redemptionPressureConfig = staticData.RedemptionPressureConfig;
            var runtimeCards = CreateRuntimeCards(staticData);

            var run = new RunSessionState(
                RunState.Playing,
                staticData,
                new RunCalendarState(openingQuarter.FiscalYear, openingQuarter.Quarter, openingQuarter.BusinessDays),
                new ResourceState(resourceConfig.StartingCash, 0, 0, 0, 0),
                new RunPerformanceState(0, 0),
                runtimeCards,
                new MarketTapeState(Array.Empty<AssetCardRuntimeData>(), Array.Empty<AssetCardRuntimeData>(), Array.Empty<AssetCardRuntimeData>()),
                new ReservationState(resourceConfig.MaxDeal, Array.Empty<AssetCardRuntimeData>()),
                new OwnedAssetState(Array.Empty<AssetCardRuntimeData>()),
                new BusinessDayState(BusinessDayPhase.AwaitingAction, MarketAreaState.Market),
                new RedemptionPressureState(redemptionPressureConfig.StartingPressure, redemptionPressureConfig.MaxPressure));

            return MarketTape.Refresh(run);
        }

        private static IReadOnlyList<AssetCardRuntimeData> CreateRuntimeCards(RunStaticDataSet staticData)
        {
            var runtimeCards = new List<AssetCardRuntimeData>();
            foreach (var card in staticData.AssetCards)
            {
                runtimeCards.Add(new AssetCardRuntimeData(card, AssetCardRuntimeState.Available, null));
            }

            return runtimeCards;
        }
    }
}
