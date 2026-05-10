using System;
using System.Collections.Generic;

namespace AssetManager
{
    public static class MarketTape
    {
        public static RunSessionState Refresh(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            var assetCards = MarkCardsRemoved(run.AssetCards, CollectVisibleCardIds(run.MarketTape));
            var tape = Refresh(
                run.StaticData.MarketConfig,
                assetCards,
                null,
                run.OwnedAssets,
                run.Reservation);

            return WithMarketTape(run, assetCards, tape);
        }

        public static MarketTapeState Refresh(
            MarketConfigData marketConfig,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketTapeState currentTape,
            OwnedAssetState ownedAssets,
            ReservationState reservation)
        {
            if (marketConfig == null)
            {
                throw new ArgumentNullException(nameof(marketConfig));
            }

            if (cardPool == null)
            {
                throw new ArgumentNullException(nameof(cardPool));
            }

            var excludedCardIds = new HashSet<string>();
            AddVisibleCardIds(excludedCardIds, currentTape);
            AddCardIds(excludedCardIds, ownedAssets?.OwnedCards);
            AddCardIds(excludedCardIds, reservation?.ReservedCards);

            var candidates = new List<AssetCardRuntimeData>();
            foreach (var card in cardPool)
            {
                if (card.State == AssetCardRuntimeState.Available
                    && !excludedCardIds.Contains(card.Card.Id))
                {
                    candidates.Add(card);
                }
            }

            var visibleCardIds = new HashSet<string>();
            var sellImminent = DrawCards(candidates, visibleCardIds, marketConfig.SellImminentSlots);
            var currentMarket = DrawCards(candidates, visibleCardIds, marketConfig.CurrentMarketSlots);
            var upcomingMarket = DrawCards(candidates, visibleCardIds, marketConfig.UpcomingMarketSlots);

            return new MarketTapeState(sellImminent, currentMarket, upcomingMarket);
        }

        public static RunSessionState Advance(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            var removedCardIds = CollectCardIds(run.MarketTape.SellImminentCards);
            var assetCards = MarkCardsRemoved(run.AssetCards, removedCardIds);
            var tape = Advance(
                run.StaticData.MarketConfig,
                assetCards,
                run.MarketTape,
                run.OwnedAssets,
                run.Reservation);

            return WithMarketTape(run, assetCards, tape);
        }

        public static MarketTapeState Advance(
            MarketConfigData marketConfig,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketTapeState currentTape,
            OwnedAssetState ownedAssets,
            ReservationState reservation)
        {
            if (marketConfig == null)
            {
                throw new ArgumentNullException(nameof(marketConfig));
            }

            if (cardPool == null)
            {
                throw new ArgumentNullException(nameof(cardPool));
            }

            if (currentTape == null)
            {
                return Refresh(marketConfig, cardPool, null, ownedAssets, reservation);
            }

            var sellImminent = new List<AssetCardRuntimeData>(currentTape.CurrentMarketCards);
            var currentMarket = new List<AssetCardRuntimeData>(currentTape.UpcomingMarketCards);
            var upcomingMarket = new List<AssetCardRuntimeData>();

            var advancedTape = new MarketTapeState(sellImminent, currentMarket, upcomingMarket);
            var excludedCardIds = CollectVisibleCardIds(advancedTape);
            AddCardIds(excludedCardIds, currentTape.SellImminentCards);

            var candidates = CreateCandidates(cardPool, excludedCardIds, ownedAssets, reservation);
            FillVisibleCards(upcomingMarket, candidates, marketConfig.UpcomingMarketSlots);

            return new MarketTapeState(sellImminent, currentMarket, upcomingMarket);
        }

        public static MarketTapeState RefillSlot(
            MarketConfigData marketConfig,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketTapeState currentTape,
            OwnedAssetState ownedAssets,
            ReservationState reservation,
            MarketTapeZone zone)
        {
            if (marketConfig == null)
            {
                throw new ArgumentNullException(nameof(marketConfig));
            }

            if (cardPool == null)
            {
                throw new ArgumentNullException(nameof(cardPool));
            }

            if (currentTape == null)
            {
                return Refresh(marketConfig, cardPool, null, ownedAssets, reservation);
            }

            var sellImminent = new List<AssetCardRuntimeData>(currentTape.SellImminentCards);
            var currentMarket = new List<AssetCardRuntimeData>(currentTape.CurrentMarketCards);
            var upcomingMarket = new List<AssetCardRuntimeData>(currentTape.UpcomingMarketCards);
            var targetZone = SelectZone(zone, sellImminent, currentMarket, upcomingMarket);
            var targetSlots = GetZoneSlotCount(marketConfig, zone);

            if (targetZone.Count >= targetSlots)
            {
                return currentTape;
            }

            var excludedCardIds = CollectVisibleCardIds(currentTape);
            var candidates = CreateCandidates(cardPool, excludedCardIds, ownedAssets, reservation);
            FillVisibleCards(targetZone, candidates, targetSlots);

            return new MarketTapeState(sellImminent, currentMarket, upcomingMarket);
        }

        private static List<AssetCardRuntimeData> DrawCards(
            IReadOnlyList<AssetCardRuntimeData> candidates,
            HashSet<string> visibleCardIds,
            int count)
        {
            var drawnCards = new List<AssetCardRuntimeData>();
            foreach (var candidate in candidates)
            {
                if (drawnCards.Count == count)
                {
                    break;
                }

                if (visibleCardIds.Add(candidate.Card.Id))
                {
                    drawnCards.Add(candidate);
                }
            }

            return drawnCards;
        }

        private static List<AssetCardRuntimeData> CreateCandidates(
            IEnumerable<AssetCardRuntimeData> cardPool,
            HashSet<string> excludedCardIds,
            OwnedAssetState ownedAssets,
            ReservationState reservation)
        {
            AddCardIds(excludedCardIds, ownedAssets?.OwnedCards);
            AddCardIds(excludedCardIds, reservation?.ReservedCards);

            var candidates = new List<AssetCardRuntimeData>();
            foreach (var card in cardPool)
            {
                if (card.State == AssetCardRuntimeState.Available
                    && !excludedCardIds.Contains(card.Card.Id))
                {
                    candidates.Add(card);
                }
            }

            return candidates;
        }

        private static void FillVisibleCards(
            List<AssetCardRuntimeData> visibleCards,
            IReadOnlyList<AssetCardRuntimeData> candidates,
            int targetCount)
        {
            var visibleCardIds = CollectCardIds(visibleCards);
            foreach (var candidate in candidates)
            {
                if (visibleCards.Count == targetCount)
                {
                    break;
                }

                if (visibleCardIds.Add(candidate.Card.Id))
                {
                    visibleCards.Add(candidate);
                }
            }
        }

        private static MarketTapeState BuildTapeFromVisibleCards(
            MarketConfigData marketConfig,
            IReadOnlyList<AssetCardRuntimeData> visibleCards)
        {
            var nextIndex = 0;
            var sellImminent = TakeCards(visibleCards, ref nextIndex, marketConfig.SellImminentSlots);
            var currentMarket = TakeCards(visibleCards, ref nextIndex, marketConfig.CurrentMarketSlots);
            var upcomingMarket = TakeCards(visibleCards, ref nextIndex, marketConfig.UpcomingMarketSlots);

            return new MarketTapeState(sellImminent, currentMarket, upcomingMarket);
        }

        private static List<AssetCardRuntimeData> TakeCards(
            IReadOnlyList<AssetCardRuntimeData> cards,
            ref int nextIndex,
            int count)
        {
            var takenCards = new List<AssetCardRuntimeData>();
            while (takenCards.Count < count && nextIndex < cards.Count)
            {
                takenCards.Add(cards[nextIndex]);
                nextIndex++;
            }

            return takenCards;
        }

        private static List<AssetCardRuntimeData> SelectZone(
            MarketTapeZone zone,
            List<AssetCardRuntimeData> sellImminent,
            List<AssetCardRuntimeData> currentMarket,
            List<AssetCardRuntimeData> upcomingMarket)
        {
            switch (zone)
            {
                case MarketTapeZone.SellImminent:
                    return sellImminent;
                case MarketTapeZone.CurrentMarket:
                    return currentMarket;
                case MarketTapeZone.UpcomingMarket:
                    return upcomingMarket;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
            }
        }

        private static int GetZoneSlotCount(MarketConfigData marketConfig, MarketTapeZone zone)
        {
            switch (zone)
            {
                case MarketTapeZone.SellImminent:
                    return marketConfig.SellImminentSlots;
                case MarketTapeZone.CurrentMarket:
                    return marketConfig.CurrentMarketSlots;
                case MarketTapeZone.UpcomingMarket:
                    return marketConfig.UpcomingMarketSlots;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
            }
        }

        private static void AddVisibleCardIds(HashSet<string> cardIds, MarketTapeState tape)
        {
            if (tape == null)
            {
                return;
            }

            AddCardIds(cardIds, tape.SellImminentCards);
            AddCardIds(cardIds, tape.CurrentMarketCards);
            AddCardIds(cardIds, tape.UpcomingMarketCards);
        }

        private static HashSet<string> CollectVisibleCardIds(MarketTapeState tape)
        {
            var cardIds = new HashSet<string>();
            AddVisibleCardIds(cardIds, tape);
            return cardIds;
        }

        private static HashSet<string> CollectCardIds(IEnumerable<AssetCardRuntimeData> cards)
        {
            var cardIds = new HashSet<string>();
            AddCardIds(cardIds, cards);
            return cardIds;
        }

        private static void AddCardIds(HashSet<string> cardIds, IEnumerable<AssetCardRuntimeData> cards)
        {
            if (cards == null)
            {
                return;
            }

            foreach (var card in cards)
            {
                cardIds.Add(card.Card.Id);
            }
        }

        private static void AddCards(List<AssetCardRuntimeData> target, IEnumerable<AssetCardRuntimeData> cards)
        {
            foreach (var card in cards)
            {
                target.Add(card);
            }
        }

        private static IReadOnlyList<AssetCardRuntimeData> MarkCardsRemoved(
            IEnumerable<AssetCardRuntimeData> cards,
            HashSet<string> removedCardIds)
        {
            var updatedCards = new List<AssetCardRuntimeData>();
            foreach (var card in cards)
            {
                if (card.State == AssetCardRuntimeState.Available
                    && removedCardIds.Contains(card.Card.Id))
                {
                    updatedCards.Add(new AssetCardRuntimeData(card.Card, AssetCardRuntimeState.Removed, card.PurchaseSource));
                }
                else
                {
                    updatedCards.Add(card);
                }
            }

            return updatedCards;
        }

        private static RunSessionState WithMarketTape(
            RunSessionState run,
            IReadOnlyList<AssetCardRuntimeData> assetCards,
            MarketTapeState tape)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                assetCards,
                tape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure);
        }
    }
}
