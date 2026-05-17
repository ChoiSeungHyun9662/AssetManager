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

            Assert.That(staticData.MarketConfig.SellImminentSlots, Is.EqualTo(3));
            Assert.That(staticData.MarketConfig.CurrentMarketSlots, Is.EqualTo(3));
            Assert.That(staticData.MarketConfig.UpcomingMarketSlots, Is.EqualTo(3));
            Assert.That(run.MarketTape.SellImminentCards, Has.Count.EqualTo(staticData.MarketConfig.SellImminentSlots));
            Assert.That(run.MarketTape.CurrentMarketCards, Has.Count.EqualTo(staticData.MarketConfig.CurrentMarketSlots));
            Assert.That(run.MarketTape.UpcomingMarketCards, Has.Count.EqualTo(staticData.MarketConfig.UpcomingMarketSlots));
        }

        [Test]
        public void MvpDefaultsIncludeGrantExtraBuyActionCard()
        {
            var staticData = RunStaticDataSet.CreateMvpDefaults();

            Assert.That(ContainsExtraBuyGrant(staticData.AssetCards), Is.True);
        }

        [Test]
        public void MvpDefaultsExposePortfolioCardsAsStocksWithFoilAndDeckCopyData()
        {
            var staticData = RunStaticDataSet.CreateMvpDefaults();
            var firstStock = staticData.AssetCards[0];

            Assert.That(firstStock.CardDomain, Is.EqualTo(CardDomain.Stock));
            Assert.That(firstStock.BaseValue, Is.GreaterThan(0));
            Assert.That(firstStock.BaseDividend, Is.GreaterThanOrEqualTo(0));
            Assert.That(firstStock.FoilValue, Is.GreaterThan(firstStock.BaseValue));
            Assert.That(firstStock.FoilDividend, Is.GreaterThanOrEqualTo(firstStock.BaseDividend));
            Assert.That(firstStock.MinDeckCopies, Is.GreaterThan(0));
            Assert.That(firstStock.MaxDeckCopies, Is.GreaterThanOrEqualTo(firstStock.MinDeckCopies));
        }

        [Test]
        public void MvpDefaultStockCostsUseInvestmentPhilosophyResources()
        {
            var staticData = RunStaticDataSet.CreateMvpDefaults();
            var firstStock = staticData.AssetCards[0];

            Assert.That(firstStock.ProfessionalCosts[0].ResourceType, Is.EqualTo(ResourceType.Reading));
            Assert.That(ResourceLedger.GetResourceDisplayName(firstStock.ProfessionalCosts[0].ResourceType), Is.EqualTo("독서"));
        }

        private static bool ContainsExtraBuyGrant(System.Collections.Generic.IEnumerable<AssetCardData> cards)
        {
            foreach (var card in cards)
            {
                if (card.GrantsExtraBuyAction)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
