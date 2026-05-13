using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class FiscalYearSummaryTests
    {
        [Test]
        public void CreateVacationSummaryUsesCompletedFiscalYearQuartersAndCurrentPortfolio()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = CompleteCurrentQuarterWithEarnedCash(run, 1);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = CompleteCurrentQuarterWithEarnedCash(run, 2);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = CompleteCurrentQuarterWithEarnedCash(run, 3);

            var ownedCard = new AssetCardRuntimeData(
                run.AssetCards[0].Card,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape);
            run = WithOwnedAssets(run, new OwnedAssetState(new[] { ownedCard }));
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);

            var summary = FiscalYearSummary.Create(run);

            Assert.That(summary.FiscalYear, Is.EqualTo(1));
            Assert.That(summary.CurrentManagementValue, Is.EqualTo(ownedCard.Card.ManagementValue));
            Assert.That(summary.FiscalYearEarnedCash, Is.EqualTo(6));
            Assert.That(summary.QuarterEarnedCash, Has.Count.EqualTo(3));
            Assert.That(summary.QuarterEarnedCash[0].Quarter, Is.EqualTo(1));
            Assert.That(summary.QuarterEarnedCash[0].EarnedCash, Is.EqualTo(1));
            Assert.That(summary.QuarterEarnedCash[1].Quarter, Is.EqualTo(2));
            Assert.That(summary.QuarterEarnedCash[1].EarnedCash, Is.EqualTo(2));
            Assert.That(summary.QuarterEarnedCash[2].Quarter, Is.EqualTo(3));
            Assert.That(summary.QuarterEarnedCash[2].EarnedCash, Is.EqualTo(3));
            Assert.That(summary.OwnedAssetCount, Is.EqualTo(1));
            Assert.That(summary.CurrentRedemptionPressure, Is.EqualTo(run.RedemptionPressure.CurrentPressure));
        }

        private static RunSessionState CompleteCurrentQuarterWithEarnedCash(RunSessionState run, int earnedCash)
        {
            run = ResourceLedger.AddEarnedCash(run, earnedCash);
            while (run.BusinessDay.Phase != BusinessDayPhase.QuarterSettlement)
            {
                run = BusinessDayFlow.AdvanceToNextBusinessDay(run);
            }

            return run;
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
    }
}
