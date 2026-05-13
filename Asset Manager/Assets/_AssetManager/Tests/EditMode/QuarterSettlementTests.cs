using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class QuarterSettlementTests
    {
        [Test]
        public void SettleQuarterAppliesSettlementIncomeBeforeAchievementAndExcludesFundingCash()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var ownedCard = new AssetCardRuntimeData(
                run.AssetCards[0].Card,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape);
            run = WithOwnedAssets(run, new OwnedAssetState(new[] { ownedCard }));
            run = ResourceLedger.AddFundingCash(run, 10);

            var result = QuarterSettlement.Settle(run);

            Assert.That(result.QuarterEarnedCash, Is.EqualTo(ownedCard.Card.ManagementValue));
            Assert.That(result.SettlementIncome, Is.EqualTo(ownedCard.Card.ManagementValue));
            Assert.That(result.TargetEarnedCash, Is.EqualTo(3));
            Assert.That(result.AchievementRate, Is.EqualTo(1d));
            Assert.That(result.RedemptionPressureIncrease, Is.EqualTo(0));
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(run.Resources.Cash + ownedCard.Card.ManagementValue));
            Assert.That(result.Run.Performance.CurrentQuarterEarnedCash, Is.EqualTo(ownedCard.Card.ManagementValue));
            Assert.That(result.Run.Performance.FundingCash, Is.EqualTo(run.Performance.FundingCash));
            Assert.That(result.Run.QuarterEndResult, Is.Not.Null);
        }

        [TestCase(4, 0)]
        [TestCase(3, 1)]
        [TestCase(2, 2)]
        [TestCase(1, 3)]
        public void SettleQuarterIncreasesRedemptionPressureByAchievementBand(
            int existingQuarterEarnedCash,
            int expectedPressureIncrease)
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithCalendar(run, new RunCalendarState(1, 2, 0));
            run = WithPerformance(
                run,
                new RunPerformanceState(existingQuarterEarnedCash, existingQuarterEarnedCash, existingQuarterEarnedCash, 0));

            var result = QuarterSettlement.Settle(run);

            Assert.That(result.RedemptionPressureIncrease, Is.EqualTo(expectedPressureIncrease));
            Assert.That(result.Run.RedemptionPressure.CurrentPressure, Is.EqualTo(expectedPressureIncrease));
        }

        private static RunSessionState WithCalendar(RunSessionState run, RunCalendarState calendar)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
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
                run.LiquidityAction);
        }

        private static RunSessionState WithPerformance(RunSessionState run, RunPerformanceState performance)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }
    }
}
