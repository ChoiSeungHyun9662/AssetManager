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
            Assert.That(fundedRun.Performance.FundingCash, Is.EqualTo(2));

            var earnedRun = ResourceLedger.AddEarnedCash(fundedRun, 3);

            Assert.That(earnedRun.Resources.Cash, Is.EqualTo(run.Resources.Cash + 5));
            Assert.That(earnedRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(3));
            Assert.That(earnedRun.Performance.CurrentFiscalYearEarnedCash, Is.EqualTo(3));
            Assert.That(earnedRun.Performance.TotalEarnedCash, Is.EqualTo(3));
        }

        [Test]
        public void InvestmentPhilosophyGainCapsCombinedTotalAndPerTypeAndReportsDiscardedOverflow()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddInvestmentPhilosophy(run, ResourceType.Reading, 5).Run;
            run = ResourceLedger.AddInvestmentPhilosophy(run, ResourceType.Meditation, 3).Run;
            run = ResourceLedger.AddInvestmentPhilosophy(run, ResourceType.Patience, 1).Run;

            var result = ResourceLedger.AddInvestmentPhilosophy(run, ResourceType.Patience, 4);

            Assert.That(result.Run.Resources.Reading, Is.EqualTo(5));
            Assert.That(result.Run.Resources.Meditation, Is.EqualTo(3));
            Assert.That(result.Run.Resources.Patience, Is.EqualTo(2));
            Assert.That(result.Run.Resources.InvestmentPhilosophyTotal, Is.EqualTo(10));
            Assert.That(result.GainedAmount, Is.EqualTo(1));
            Assert.That(result.DiscardedAmount, Is.EqualTo(3));
            Assert.That(result.Message, Is.EqualTo("투자 철학 한도: 인내 +3 버림"));
        }

        [Test]
        public void InvestmentPhilosophyGainCapsEachTypeAtFive()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddInvestmentPhilosophy(run, ResourceType.Reading, 4).Run;

            var result = ResourceLedger.AddInvestmentPhilosophy(run, ResourceType.Reading, 3);

            Assert.That(result.Run.Resources.Reading, Is.EqualTo(5));
            Assert.That(result.Run.Resources.Meditation, Is.EqualTo(0));
            Assert.That(result.Run.Resources.Patience, Is.EqualTo(0));
            Assert.That(result.Run.Resources.InvestmentPhilosophyTotal, Is.EqualTo(5));
            Assert.That(result.GainedAmount, Is.EqualTo(1));
            Assert.That(result.DiscardedAmount, Is.EqualTo(2));
            Assert.That(result.Message, Is.EqualTo("투자 철학 한도: 독서 +2 버림"));
        }

        [Test]
        public void CashAndDealAreExcludedFromInvestmentPhilosophyCapAndDealOverflowIsDiscarded()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddInvestmentPhilosophy(run, ResourceType.Reading, 5).Run;
            run = ResourceLedger.AddInvestmentPhilosophy(run, ResourceType.Meditation, 3).Run;
            run = ResourceLedger.AddInvestmentPhilosophy(run, ResourceType.Patience, 2).Run;
            run = ResourceLedger.AddFundingCash(run, 5);

            var result = ResourceLedger.AddDeal(run, 4);

            Assert.That(result.Run.Resources.Cash, Is.EqualTo(run.Resources.Cash));
            Assert.That(result.Run.Resources.InvestmentPhilosophyTotal, Is.EqualTo(10));
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(3));
            Assert.That(result.GainedAmount, Is.EqualTo(3));
            Assert.That(result.DiscardedAmount, Is.EqualTo(1));
            Assert.That(result.Message, Is.EqualTo("딜 한도: 추가 딜 버림"));
        }

        [Test]
        public void InvestmentPhilosophyDisplayNamesReplaceOldProfessionalResourceNames()
        {
            Assert.That(ResourceLedger.GetResourceDisplayName(ResourceType.Reading), Is.EqualTo("독서"));
            Assert.That(ResourceLedger.GetResourceDisplayName(ResourceType.Meditation), Is.EqualTo("명상"));
            Assert.That(ResourceLedger.GetResourceDisplayName(ResourceType.Patience), Is.EqualTo("인내"));
        }
    }
}
