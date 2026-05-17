using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class MarketDeckTests
    {
        [Test]
        public void DrawOneUsesStockDeckBelowConfiguredStockWeightAndConsumableDeckAtOrAboveIt()
        {
            var cards = new[]
            {
                Stock("stock-card"),
                Consumable("resource-card")
            };
            var marketConfig = new MarketConfigData(1, 1, 1, 0.75);

            var stockDraw = MarketDeck.DrawOne(cards, marketConfig, 0.74, new string[0]);
            var consumableDraw = MarketDeck.DrawOne(cards, marketConfig, 0.75, new string[0]);

            Assert.That(stockDraw.Card.Card.CardDomain, Is.EqualTo(CardDomain.Stock));
            Assert.That(consumableDraw.Card.Card.CardDomain, Is.EqualTo(CardDomain.ConsumableResource));
        }

        [Test]
        public void DrawOneFallsBackToOppositeDeckWhenSelectedDeckIsEmpty()
        {
            var marketConfig = new MarketConfigData(1, 1, 1, 0.75);

            var stockFallback = MarketDeck.DrawOne(new[] { Stock("stock-card") }, marketConfig, 0.90, new string[0]);
            var consumableFallback = MarketDeck.DrawOne(new[] { Consumable("resource-card") }, marketConfig, 0.10, new string[0]);

            Assert.That(stockFallback.Card.Card.CardDomain, Is.EqualTo(CardDomain.Stock));
            Assert.That(consumableFallback.Card.Card.CardDomain, Is.EqualTo(CardDomain.ConsumableResource));
        }

        [Test]
        public void DrawOneCanRecycleRemovedConsumableButNotRemovedStock()
        {
            var marketConfig = new MarketConfigData(1, 1, 1, 0.75);

            var recycled = MarketDeck.DrawOne(
                new[] { Consumable("resource-card", AssetCardRuntimeState.Removed) },
                marketConfig,
                0.90,
                new string[0]);

            Assert.That(recycled.Card.State, Is.EqualTo(AssetCardRuntimeState.Available));
            Assert.That(recycled.WasRecycled, Is.True);
            Assert.Throws<MarketDeckExhaustedException>(() =>
                MarketDeck.DrawOne(
                    new[] { Stock("stock-card", AssetCardRuntimeState.Removed) },
                    marketConfig,
                    0.10,
                    new string[0]));
        }

        [Test]
        public void DrawOneThrowsWhenBothDecksCannotSupplyCards()
        {
            Assert.Throws<MarketDeckExhaustedException>(() =>
                MarketDeck.DrawOne(
                    new AssetCardRuntimeData[0],
                    new MarketConfigData(1, 1, 1, 0.75),
                    0.10,
                    new string[0]));
        }

        private static AssetCardRuntimeData Stock(
            string id,
            AssetCardRuntimeState state = AssetCardRuntimeState.Available)
        {
            var card = new AssetCardData(
                id,
                id,
                "Stock test card.",
                AssetRarity.Common,
                1,
                new ProfessionalResourceCost[0],
                1,
                0,
                new TagData[0],
                cardDomain: CardDomain.Stock);

            return new AssetCardRuntimeData(card, state, null);
        }

        private static AssetCardRuntimeData Consumable(
            string id,
            AssetCardRuntimeState state = AssetCardRuntimeState.Available)
        {
            var card = new AssetCardData(
                id,
                string.Empty,
                "Resource test card.",
                AssetRarity.Common,
                1,
                new ProfessionalResourceCost[0],
                0,
                0,
                new TagData[0],
                cardDomain: CardDomain.ConsumableResource,
                providedResourceType: ResourceType.Cash,
                providedResourceAmount: 1);

            return new AssetCardRuntimeData(card, state, null);
        }
    }
}
