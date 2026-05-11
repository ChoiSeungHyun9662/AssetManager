using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class ResourceLedgerTests
    {
        [Test]
        public void FundingCashOnlyIncreasesCashAndEarnedCashIncreasesPerformanceCounters()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());

            var fundedRun = ResourceLedger.AddFundingCash(run, 2);

            Assert.That(fundedRun.Resources.Cash, Is.EqualTo(run.Resources.Cash + 2));
            Assert.That(fundedRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(0));
            Assert.That(fundedRun.Performance.CurrentFiscalYearEarnedCash, Is.EqualTo(0));
            Assert.That(fundedRun.Performance.TotalEarnedCash, Is.EqualTo(0));

            var earnedRun = ResourceLedger.AddEarnedCash(fundedRun, 3);

            Assert.That(earnedRun.Resources.Cash, Is.EqualTo(run.Resources.Cash + 5));
            Assert.That(earnedRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(3));
            Assert.That(earnedRun.Performance.CurrentFiscalYearEarnedCash, Is.EqualTo(3));
            Assert.That(earnedRun.Performance.TotalEarnedCash, Is.EqualTo(3));
        }

        [Test]
        public void ProfessionalResourceGainCapsCombinedTotalAndReportsDiscardedOverflow()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 4).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 3).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Commodity, 2).Run;

            var result = ResourceLedger.AddProfessionalResource(run, ResourceType.Commodity, 2);

            Assert.That(result.Run.Resources.Research, Is.EqualTo(4));
            Assert.That(result.Run.Resources.Credit, Is.EqualTo(3));
            Assert.That(result.Run.Resources.Commodity, Is.EqualTo(3));
            Assert.That(result.Run.Resources.ProfessionalTotal, Is.EqualTo(10));
            Assert.That(result.GainedAmount, Is.EqualTo(1));
            Assert.That(result.DiscardedAmount, Is.EqualTo(1));
            Assert.That(result.Message, Is.EqualTo("자원칩 최대 보유: 원자재 +1 폐기"));
        }

        [Test]
        public void CashAndDealAreExcludedFromProfessionalCapAndDealOverflowIsDiscarded()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 4).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 3).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Commodity, 3).Run;
            run = ResourceLedger.AddFundingCash(run, 5);

            var result = ResourceLedger.AddDeal(run, 4);

            Assert.That(result.Run.Resources.Cash, Is.EqualTo(run.Resources.Cash));
            Assert.That(result.Run.Resources.ProfessionalTotal, Is.EqualTo(10));
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(3));
            Assert.That(result.GainedAmount, Is.EqualTo(3));
            Assert.That(result.DiscardedAmount, Is.EqualTo(1));
            Assert.That(result.Message, Is.EqualTo("딜 최대 보유: 추가 딜 폐기"));
        }
    }
}
