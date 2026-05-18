using System;
using System.Collections.Generic;

namespace AssetManager
{
    public static class MarketDeck
    {
        public static MarketDeckDrawResult DrawOne(
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketConfigData marketConfig,
            double stockDrawRoll,
            IEnumerable<string> excludedCardIds)
        {
            if (cardPool == null)
            {
                throw new ArgumentNullException(nameof(cardPool));
            }

            if (marketConfig == null)
            {
                throw new ArgumentNullException(nameof(marketConfig));
            }

            if (stockDrawRoll < 0 || stockDrawRoll >= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(stockDrawRoll), stockDrawRoll, "Draw roll must be at least 0 and less than 1.");
            }

            var excludedIds = new HashSet<string>(excludedCardIds ?? Array.Empty<string>());
            var cards = new List<AssetCardRuntimeData>(cardPool);
            var primaryDomain = stockDrawRoll < marketConfig.StockDeckDrawWeight
                ? CardDomain.Stock
                : CardDomain.ConsumableResource;
            var fallbackDomain = primaryDomain == CardDomain.Stock
                ? CardDomain.ConsumableResource
                : CardDomain.Stock;

            var primaryDraw = TryDrawAvailable(cards, primaryDomain, excludedIds)
                ?? TryDrawAvailable(cards, fallbackDomain, excludedIds);
            if (primaryDraw != null)
            {
                return new MarketDeckDrawResult(primaryDraw, false);
            }

            var recycled = TryRecycleConsumable(cards, excludedIds);
            if (recycled != null)
            {
                return new MarketDeckDrawResult(recycled, true);
            }

            throw new MarketDeckExhaustedException();
        }

        private static AssetCardRuntimeData TryDrawAvailable(
            IEnumerable<AssetCardRuntimeData> cards,
            CardDomain cardDomain,
            HashSet<string> excludedCardIds)
        {
            foreach (var card in cards)
            {
                if (card.State == AssetCardRuntimeState.Available
                    && card.Card.CardDomain == cardDomain
                    && !excludedCardIds.Contains(card.RuntimeId)
                    && !excludedCardIds.Contains(card.Card.Id))
                {
                    return card;
                }
            }

            return null;
        }

        private static AssetCardRuntimeData TryRecycleConsumable(
            IEnumerable<AssetCardRuntimeData> cards,
            HashSet<string> excludedCardIds)
        {
            foreach (var card in cards)
            {
                if (card.State == AssetCardRuntimeState.Removed
                    && card.Card.CardDomain == CardDomain.ConsumableResource
                    && !excludedCardIds.Contains(card.RuntimeId)
                    && !excludedCardIds.Contains(card.Card.Id))
                {
                    return new AssetCardRuntimeData(
                        card.Card,
                        AssetCardRuntimeState.Available,
                        card.PurchaseSource,
                        card.AcquiredOrder,
                        card.IsFoil,
                        card.RuntimeId);
                }
            }

            return null;
        }
    }

    public sealed class MarketDeckDrawResult
    {
        public MarketDeckDrawResult(AssetCardRuntimeData card, bool wasRecycled)
        {
            Card = card;
            WasRecycled = wasRecycled;
        }

        public AssetCardRuntimeData Card { get; }
        public bool WasRecycled { get; }
    }

    public sealed class MarketDeckExhaustedException : Exception
    {
        public MarketDeckExhaustedException()
            : base("No market deck can supply a card.")
        {
        }
    }
}
