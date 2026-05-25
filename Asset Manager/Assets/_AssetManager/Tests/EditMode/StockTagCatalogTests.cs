using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class StockTagCatalogTests
    {
        [Test]
        public void MvpDefaultStocksHaveExactlyOneAllowedV3Tag()
        {
            var staticData = RunStaticDataSet.CreateMvpDefaults();

            foreach (var card in staticData.AssetCards)
            {
                if (card.CardDomain != CardDomain.Stock)
                {
                    continue;
                }

                Assert.That(
                    StockTagCatalog.HasExactlyOneAllowedStockTag(card),
                    Is.True,
                    card.Id + " should have exactly one allowed stock tag.");
            }
        }

        [Test]
        public void StockTagValidationRejectsMissingMultipleAndUnknownTags()
        {
            Assert.That(
                StockTagCatalog.HasExactlyOneAllowedStockTag(CreateStockWithTags(new TagData[0])),
                Is.False);
            Assert.That(
                StockTagCatalog.HasExactlyOneAllowedStockTag(CreateStockWithTags(new[]
                {
                    StockTagCatalog.Technology,
                    StockTagCatalog.Consumer
                })),
                Is.False);
            Assert.That(
                StockTagCatalog.HasExactlyOneAllowedStockTag(CreateStockWithTags(new[]
                {
                    new TagData("software", "Software", TagType.Sector)
                })),
                Is.False);
        }

        [Test]
        public void V3StockTagCatalogContainsOnlyTheFiveAllowedTags()
        {
            Assert.That(StockTagCatalog.AllowedStockTags.Count, Is.EqualTo(5));
            Assert.That(StockTagCatalog.IsAllowedStockTag("technology"), Is.True);
            Assert.That(StockTagCatalog.IsAllowedStockTag("consumer"), Is.True);
            Assert.That(StockTagCatalog.IsAllowedStockTag("energy"), Is.True);
            Assert.That(StockTagCatalog.IsAllowedStockTag("financials"), Is.True);
            Assert.That(StockTagCatalog.IsAllowedStockTag("industrials"), Is.True);
            Assert.That(StockTagCatalog.IsAllowedStockTag("software"), Is.False);
        }

        [Test]
        public void MarketCardFaceShowsTheStockTagWithoutAddingUniqueEffectText()
        {
            var staticData = RunStaticDataSet.CreateMvpDefaults();
            var stock = staticData.AssetCards[0];
            var runtimeCard = new AssetCardRuntimeData(stock, AssetCardRuntimeState.Available, null);

            var text = MarketCardFormatter.Format(runtimeCard, false);

            Assert.That(text, Does.Contain(stock.Tags[0].DisplayName));
            Assert.That(stock.Description, Is.EqualTo("Stock card for the starter portfolio."));
        }

        private static AssetCardData CreateStockWithTags(TagData[] tags)
        {
            return new AssetCardData(
                "test-stock",
                "Test Stock",
                "Stock card for tag validation.",
                AssetRarity.Common,
                1,
                new ProfessionalResourceCost[0],
                1,
                0,
                tags);
        }
    }
}
