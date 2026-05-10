using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class RunBootstrapperTests
    {
        [Test]
        public void CreateNewRunStartsAtFirstFiscalYearFirstQuarter()
        {
            var staticData = RunStaticDataSet.CreateMvpDefaults();

            var run = RunBootstrapper.CreateNewRun(staticData);

            Assert.That(run.State, Is.EqualTo(RunState.Playing));
            Assert.That(run.Calendar.FiscalYear, Is.EqualTo(1));
            Assert.That(run.Calendar.Quarter, Is.EqualTo(1));
            Assert.That(run.Calendar.RemainingBusinessDays, Is.EqualTo(4));
        }

        [Test]
        public void CreateNewRunRefreshesMarketTape()
        {
            var staticData = RunStaticDataSet.CreateMvpDefaults();

            var run = RunBootstrapper.CreateNewRun(staticData);

            Assert.That(run.MarketTape.SellImminentCards, Has.Count.EqualTo(staticData.MarketConfig.SellImminentSlots));
            Assert.That(run.MarketTape.CurrentMarketCards, Has.Count.EqualTo(staticData.MarketConfig.CurrentMarketSlots));
            Assert.That(run.MarketTape.UpcomingMarketCards, Has.Count.EqualTo(staticData.MarketConfig.UpcomingMarketSlots));
        }
    }
}
