using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class FinalSettlementTests
    {
        [Test]
        public void CreateFinalSettlementRatesOwnedStocksOnlyAndUsesRentArrearsForComment()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var secondStock = run.StaticData.AssetCards[3];
            var ownedOffice = new AssetCardRuntimeData(
                run.AssetCards[0].Card,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape);
            var ownedDataCenter = new AssetCardRuntimeData(
                secondStock,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape);
            var reservedLogistics = new AssetCardRuntimeData(
                run.AssetCards[1].Card,
                AssetCardRuntimeState.Reserved,
                null);

            run = WithResources(run, new ResourceState(100, 5, 5, 5, 3));
            run = WithOwnedAssets(run, new OwnedAssetState(new[] { ownedOffice, ownedDataCenter }));
            run = WithReservation(run, new ReservationState(3, new[] { reservedLogistics }));
            run = WithRedemptionPressure(run, 8);

            var settlement = FinalSettlement.Create(run);

            Assert.That(settlement.FinalValue, Is.EqualTo(7));
            Assert.That(settlement.FinalRating.DisplayName, Is.EqualTo("Core"));
            Assert.That(settlement.TotalEarnedCash, Is.EqualTo(0));
            Assert.That(settlement.OwnedStockCount, Is.EqualTo(2));
            Assert.That(settlement.CurrentRentArrears, Is.EqualTo(8));
            Assert.That(settlement.FinalComment, Is.EqualTo("성과는 충분하지만 월세 밀림이 높습니다."));
        }

        private static RunSessionState WithResources(RunSessionState run, ResourceState resources)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);
        }

        private static RunSessionState WithOwnedAssets(RunSessionState run, OwnedAssetState ownedAssets)
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
                ownedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);
        }

        private static RunSessionState WithReservation(RunSessionState run, ReservationState reservation)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);
        }

        private static RunSessionState WithRedemptionPressure(RunSessionState run, int currentPressure)
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
                new RedemptionPressureState(currentPressure, run.RedemptionPressure.MaxPressure),
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);
        }
    }
}
