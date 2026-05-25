using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class QuarterSettlementTests
    {
        [Test]
        public void SettleQuarterUsesCashFlowAndMissionRevenueForAchievementWithoutIncreasingCashByMission()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var tag = StockTagCatalog.Technology;
            var mission = CreateMission("mission", MissionTemplate.FastEntry, new[] { tag }, rewardPerCard: 1);
            var firstOwnedStock = CreateRuntimeStock("target-1", tag, value: 3);
            var secondOwnedStock = CreateRuntimeStock("target-2", tag, value: 5);
            run = WithOwnedAssets(run, new OwnedAssetState(new[] { firstOwnedStock, secondOwnedStock }));
            run = WithMissions(run, new MissionRunState(
                new[] { new MissionCandidateSlotState(mission, false) },
                1,
                mission));
            run = WithPerformance(run, new RunPerformanceState(1, 1, 1, 0));
            run = ResourceLedger.AddFundingCash(run, 10);

            var result = QuarterSettlement.Settle(run);

            Assert.That(result.QuarterRevenue, Is.EqualTo(1));
            Assert.That(result.MissionRevenue, Is.EqualTo(2));
            Assert.That(result.QuarterEvaluationValue, Is.EqualTo(3));
            Assert.That(result.QuarterRevenueTarget, Is.EqualTo(3));
            Assert.That(result.AchievementRate, Is.EqualTo(1d));
            Assert.That(result.RentArrearsIncrease, Is.EqualTo(0));
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(run.Resources.Cash));
            Assert.That(result.Run.Performance.CurrentQuarterEarnedCash, Is.EqualTo(1));
            Assert.That(result.Run.Performance.CurrentQuarterMissionRevenue, Is.EqualTo(2));
            Assert.That(result.Run.Performance.TotalMissionRevenue, Is.EqualTo(2));
            Assert.That(result.Run.Performance.FundingCash, Is.EqualTo(run.Performance.FundingCash));
            Assert.That(result.Run.QuarterEndResult, Is.Not.Null);
            Assert.That(result.Run.Performance.CompletedQuarterRevenue, Has.Count.EqualTo(1));
            Assert.That(result.Run.Performance.CompletedQuarterRevenue[0].Revenue, Is.EqualTo(result.QuarterRevenue));
        }

        [TestCase(4, 0)]
        [TestCase(3, 1)]
        [TestCase(2, 2)]
        [TestCase(1, 3)]
        public void SettleQuarterIncreasesRentArrearsByAchievementBand(
            int existingQuarterEarnedCash,
            int expectedPressureIncrease)
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithCalendar(run, new RunCalendarState(1, 2, 0));
            run = WithPerformance(
                run,
                new RunPerformanceState(existingQuarterEarnedCash, existingQuarterEarnedCash, existingQuarterEarnedCash, 0));

            var result = QuarterSettlement.Settle(run);

            Assert.That(result.RentArrearsIncrease, Is.EqualTo(expectedPressureIncrease));
            Assert.That(result.CurrentRentArrears, Is.EqualTo(expectedPressureIncrease));
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
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                run.Missions);
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
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                run.Missions);
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
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                run.Missions);
        }

        private static RunSessionState WithMissions(RunSessionState run, MissionRunState missions)
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
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                missions);
        }

        private static MissionDefinitionData CreateMission(
            string id,
            MissionTemplate template,
            System.Collections.Generic.IEnumerable<TagData> tags,
            int rewardPerCard)
        {
            return new MissionDefinitionData(
                id,
                id,
                template,
                tags,
                "clear",
                "formula",
                "Test",
                1,
                rewardPerCard);
        }

        private static AssetCardRuntimeData CreateRuntimeStock(string id, TagData tag, int value)
        {
            var card = new AssetCardData(
                id,
                id,
                "test stock",
                AssetRarity.Common,
                0,
                new ProfessionalResourceCost[0],
                value,
                0,
                new[] { tag },
                foilValue: value);
            return new AssetCardRuntimeData(card, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, null, false, id + "#runtime");
        }
    }
}
