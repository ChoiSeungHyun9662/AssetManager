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

            var assetCards = MarkCardsRemoved(run.AssetCards, CollectNonReservedVisibleCardIds(run.MarketTape));
            var reservedOnlyTape = KeepReservedSlotsOnly(run.MarketTape);
            var tape = Refresh(
                run.StaticData.MarketConfig,
                assetCards,
                reservedOnlyTape,
                run.OwnedAssets,
                run.Reservation,
                null);

            return WithMarketTape(run, assetCards, tape);
        }

        public static MarketTapeState Refresh(
            MarketConfigData marketConfig,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketTapeState currentTape,
            OwnedAssetState ownedAssets,
            ReservationState reservation)
        {
            return Refresh(marketConfig, cardPool, currentTape, ownedAssets, reservation, null);
        }

        public static MarketTapeState Refresh(
            MarketConfigData marketConfig,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketTapeState currentTape,
            OwnedAssetState ownedAssets,
            ReservationState reservation,
            IEnumerable<double> stockDrawRolls)
        {
            if (marketConfig == null)
            {
                throw new ArgumentNullException(nameof(marketConfig));
            }

            if (cardPool == null)
            {
                throw new ArgumentNullException(nameof(cardPool));
            }

            var excludedCardIds = CollectVisibleCardIds(currentTape);
            AddCardIds(excludedCardIds, ownedAssets?.OwnedCards);
            AddCardIds(excludedCardIds, reservation?.ReservedCards);

            var drawRolls = new DrawRollSource(stockDrawRolls);
            var slots = new List<MarketTapeSlotState>();
            var currentSlots = currentTape?.Slots ?? Array.Empty<MarketTapeSlotState>();
            for (var i = 0; i < marketConfig.MarketTapeSlots; i++)
            {
                if (i < currentSlots.Count && currentSlots[i].IsReserved && !currentSlots[i].IsEmpty)
                {
                    slots.Add(currentSlots[i]);
                    excludedCardIds.Add(currentSlots[i].Card.Card.Id);
                    continue;
                }

                slots.Add(new MarketTapeSlotState(
                    DrawOne(cardPool, marketConfig, excludedCardIds, drawRolls),
                    false));
            }

            return new MarketTapeState(slots);
        }

        public static RunSessionState Advance(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            var removedCardIds = CollectLeftmostNonReservedCardId(run.MarketTape);
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

            var slots = NormalizeSlots(currentTape, marketConfig.MarketTapeSlots);
            var movingCards = new List<AssetCardRuntimeData>();
            foreach (var slot in slots)
            {
                if (!slot.IsReserved && !slot.IsEmpty)
                {
                    movingCards.Add(slot.Card);
                }
            }

            if (movingCards.Count > 0)
            {
                movingCards.RemoveAt(0);
            }

            var nextMovingCard = 0;
            for (var i = 0; i < slots.Count; i++)
            {
                if (slots[i].IsReserved)
                {
                    continue;
                }

                var card = nextMovingCard < movingCards.Count ? movingCards[nextMovingCard] : null;
                slots[i] = new MarketTapeSlotState(card, false);
                nextMovingCard++;
            }

            FillEmptyNonReservedSlotsFromRight(slots, cardPool, marketConfig, ownedAssets, reservation);
            return new MarketTapeState(slots);
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

            var slots = NormalizeSlots(currentTape, marketConfig.MarketTapeSlots);
            FillEmptyNonReservedSlotsFromRight(slots, cardPool, marketConfig, ownedAssets, reservation);
            return new MarketTapeState(slots);
        }

        public static MarketTapeState AdvanceSlotAt(
            MarketConfigData marketConfig,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketTapeState currentTape,
            OwnedAssetState ownedAssets,
            ReservationState reservation,
            MarketTapeZone zone,
            int slotIndex)
        {
            return PullFromSlotAt(
                marketConfig,
                cardPool,
                currentTape,
                ownedAssets,
                reservation,
                slotIndex);
        }

        public static MarketTapeState PullFromSlotAt(
            MarketConfigData marketConfig,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketTapeState currentTape,
            OwnedAssetState ownedAssets,
            ReservationState reservation,
            int slotIndex)
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

            return PullFromEmptySlot(
                marketConfig,
                cardPool,
                ClearSlot(currentTape, slotIndex),
                ownedAssets,
                reservation,
                slotIndex);
        }

        public static MarketTapeState PullFromEmptySlot(
            MarketConfigData marketConfig,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketTapeState currentTape,
            OwnedAssetState ownedAssets,
            ReservationState reservation,
            int emptySlotIndex)
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

            var slots = NormalizeSlots(currentTape, marketConfig.MarketTapeSlots);
            if (emptySlotIndex < 0 || emptySlotIndex >= slots.Count)
            {
                return currentTape;
            }

            var pullPositions = new List<int>();
            var pullCards = new List<AssetCardRuntimeData>();
            for (var i = emptySlotIndex; i < slots.Count; i++)
            {
                if (slots[i].IsReserved)
                {
                    continue;
                }

                pullPositions.Add(i);
                if (!slots[i].IsEmpty)
                {
                    pullCards.Add(slots[i].Card);
                }
            }

            var nextCard = 0;
            foreach (var position in pullPositions)
            {
                var card = nextCard < pullCards.Count ? pullCards[nextCard] : null;
                slots[position] = new MarketTapeSlotState(card, false);
                nextCard++;
            }

            FillRightmostEmptyNonReservedSlot(slots, cardPool, marketConfig, ownedAssets, reservation);
            return new MarketTapeState(slots);
        }

        public static MarketTapeState PullAllEmptySlots(
            MarketConfigData marketConfig,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketTapeState currentTape,
            OwnedAssetState ownedAssets,
            ReservationState reservation)
        {
            if (currentTape == null)
            {
                return Refresh(marketConfig, cardPool, null, ownedAssets, reservation);
            }

            var tape = currentTape;
            var emptySlotIndex = FindLeftmostEmptyNonReservedSlot(tape);
            while (emptySlotIndex >= 0)
            {
                tape = PullFromEmptySlot(
                    marketConfig,
                    cardPool,
                    tape,
                    ownedAssets,
                    reservation,
                    emptySlotIndex);
                emptySlotIndex = FindLeftmostEmptyNonReservedSlot(tape);
            }

            return tape;
        }

        public static MarketTapeState ReserveSlot(
            MarketTapeState currentTape,
            string cardId,
            AssetCardRuntimeData reservedCard)
        {
            if (currentTape == null)
            {
                throw new ArgumentNullException(nameof(currentTape));
            }

            var slots = new List<MarketTapeSlotState>(currentTape.Slots);
            for (var i = 0; i < slots.Count; i++)
            {
                if (!slots[i].IsEmpty && slots[i].Card.RuntimeId == cardId)
                {
                    slots[i] = new MarketTapeSlotState(reservedCard, true);
                    return new MarketTapeState(slots);
                }
            }

            return currentTape;
        }

        private static bool CanAdvanceColumn(
            IReadOnlyList<AssetCardRuntimeData> sellImminent,
            IReadOnlyList<AssetCardRuntimeData> currentMarket,
            IReadOnlyList<AssetCardRuntimeData> upcomingMarket,
            MarketTapeZone zone,
            int slotIndex)
        {
            if (slotIndex < 0)
            {
                return false;
            }

            switch (zone)
            {
                case MarketTapeZone.SellImminent:
                    return slotIndex < sellImminent.Count
                        && slotIndex < currentMarket.Count
                        && slotIndex < upcomingMarket.Count;
                case MarketTapeZone.CurrentMarket:
                    return slotIndex < currentMarket.Count
                        && slotIndex < upcomingMarket.Count;
                case MarketTapeZone.UpcomingMarket:
                    return slotIndex < upcomingMarket.Count;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
            }
        }

        private static MarketTapeState ClearSlot(MarketTapeState currentTape, int slotIndex)
        {
            var slots = new List<MarketTapeSlotState>(currentTape.Slots);
            if (slotIndex < 0 || slotIndex >= slots.Count)
            {
                return currentTape;
            }

            slots[slotIndex] = new MarketTapeSlotState(null, false);
            return new MarketTapeState(slots);
        }

        private static int FindLeftmostEmptyNonReservedSlot(MarketTapeState tape)
        {
            for (var i = 0; i < tape.Slots.Count; i++)
            {
                if (!tape.Slots[i].IsReserved && tape.Slots[i].IsEmpty)
                {
                    return i;
                }
            }

            return -1;
        }

        private static MarketTapeState KeepReservedSlotsOnly(MarketTapeState tape)
        {
            if (tape == null)
            {
                return null;
            }

            var slots = new List<MarketTapeSlotState>();
            foreach (var slot in tape.Slots)
            {
                slots.Add(slot.IsReserved ? slot : new MarketTapeSlotState(null, false));
            }

            return new MarketTapeState(slots);
        }

        private static List<MarketTapeSlotState> NormalizeSlots(MarketTapeState tape, int targetSlotCount)
        {
            var slots = new List<MarketTapeSlotState>(tape.Slots);
            while (slots.Count < targetSlotCount)
            {
                slots.Add(new MarketTapeSlotState(null, false));
            }

            if (slots.Count > targetSlotCount)
            {
                slots.RemoveRange(targetSlotCount, slots.Count - targetSlotCount);
            }

            return slots;
        }

        private static AssetCardRuntimeData DrawOne(
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketConfigData marketConfig,
            HashSet<string> excludedCardIds,
            DrawRollSource drawRolls)
        {
            var draw = MarketDeck.DrawOne(cardPool, marketConfig, drawRolls.Next(), excludedCardIds);
            excludedCardIds.Add(draw.Card.Card.Id);
            return draw.Card;
        }

        private static void FillEmptyNonReservedSlotsFromRight(
            List<MarketTapeSlotState> slots,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketConfigData marketConfig,
            OwnedAssetState ownedAssets,
            ReservationState reservation)
        {
            while (HasEmptyNonReservedSlot(slots))
            {
                FillRightmostEmptyNonReservedSlot(slots, cardPool, marketConfig, ownedAssets, reservation);
            }
        }

        private static bool HasEmptyNonReservedSlot(IEnumerable<MarketTapeSlotState> slots)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsReserved && slot.IsEmpty)
                {
                    return true;
                }
            }

            return false;
        }

        private static void FillRightmostEmptyNonReservedSlot(
            List<MarketTapeSlotState> slots,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketConfigData marketConfig,
            OwnedAssetState ownedAssets,
            ReservationState reservation)
        {
            var fillIndex = -1;
            for (var i = slots.Count - 1; i >= 0; i--)
            {
                if (!slots[i].IsReserved && slots[i].IsEmpty)
                {
                    fillIndex = i;
                    break;
                }
            }

            if (fillIndex < 0)
            {
                return;
            }

            var excludedCardIds = CollectVisibleCardIds(new MarketTapeState(slots));
            AddCardIds(excludedCardIds, ownedAssets?.OwnedCards);
            AddCardIds(excludedCardIds, reservation?.ReservedCards);
            slots[fillIndex] = new MarketTapeSlotState(
                DrawOne(cardPool, marketConfig, excludedCardIds, new DrawRollSource(null)),
                false);
        }

        private static List<AssetCardRuntimeData> DrawCards(
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketConfigData marketConfig,
            HashSet<string> excludedCardIds,
            DrawRollSource drawRolls,
            int count)
        {
            var drawnCards = new List<AssetCardRuntimeData>();
            while (drawnCards.Count < count)
            {
                var draw = MarketDeck.DrawOne(cardPool, marketConfig, drawRolls.Next(), excludedCardIds);
                drawnCards.Add(draw.Card);
                excludedCardIds.Add(draw.Card.Card.Id);
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
                    && !excludedCardIds.Contains(card.RuntimeId)
                    && !excludedCardIds.Contains(card.Card.Id))
                {
                    candidates.Add(card);
                }
            }

            return candidates;
        }

        private static void FillVisibleCards(
            List<AssetCardRuntimeData> visibleCards,
            IEnumerable<AssetCardRuntimeData> cardPool,
            MarketConfigData marketConfig,
            HashSet<string> excludedCardIds,
            DrawRollSource drawRolls,
            int targetCount)
        {
            AddCardDefinitionIds(excludedCardIds, visibleCards);
            while (visibleCards.Count < targetCount)
            {
                var draw = MarketDeck.DrawOne(cardPool, marketConfig, drawRolls.Next(), excludedCardIds);
                visibleCards.Add(draw.Card);
                excludedCardIds.Add(draw.Card.Card.Id);
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

            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty)
                {
                    cardIds.Add(slot.Card.Card.Id);
                }
            }

            AddCardDefinitionIds(cardIds, tape.SellImminentCards);
            AddCardDefinitionIds(cardIds, tape.CurrentMarketCards);
            AddCardDefinitionIds(cardIds, tape.UpcomingMarketCards);
        }

        private static HashSet<string> CollectVisibleCardIds(MarketTapeState tape)
        {
            var cardIds = new HashSet<string>();
            AddVisibleCardIds(cardIds, tape);
            return cardIds;
        }

        private static HashSet<string> CollectNonReservedVisibleCardIds(MarketTapeState tape)
        {
            var cardIds = new HashSet<string>();
            if (tape == null)
            {
                return cardIds;
            }

            foreach (var slot in tape.Slots)
            {
                if (!slot.IsReserved && !slot.IsEmpty)
                {
                    cardIds.Add(slot.Card.RuntimeId);
                }
            }

            return cardIds;
        }

        private static HashSet<string> CollectLeftmostNonReservedCardId(MarketTapeState tape)
        {
            var cardIds = new HashSet<string>();
            if (tape == null)
            {
                return cardIds;
            }

            foreach (var slot in tape.Slots)
            {
                if (!slot.IsReserved && !slot.IsEmpty)
                {
                    cardIds.Add(slot.Card.RuntimeId);
                    break;
                }
            }

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
                if (card != null)
                {
                    cardIds.Add(card.RuntimeId);
                }
            }
        }

        private static void AddCardDefinitionIds(HashSet<string> cardIds, IEnumerable<AssetCardRuntimeData> cards)
        {
            if (cards == null)
            {
                return;
            }

            foreach (var card in cards)
            {
                if (card != null)
                {
                    cardIds.Add(card.Card.Id);
                }
            }
        }

        private static void AddStockCardIds(HashSet<string> cardIds, IEnumerable<AssetCardRuntimeData> cards)
        {
            if (cards == null)
            {
                return;
            }

            foreach (var card in cards)
            {
                if (card.Card.CardDomain == CardDomain.Stock)
                {
                    cardIds.Add(card.RuntimeId);
                }
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
                    && removedCardIds.Contains(card.RuntimeId))
                {
                    updatedCards.Add(new AssetCardRuntimeData(
                        card.Card,
                        AssetCardRuntimeState.Removed,
                        card.PurchaseSource,
                        card.AcquiredOrder,
                        card.IsFoil,
                        card.RuntimeId));
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
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery);
        }

        private sealed class DrawRollSource
        {
            private readonly IEnumerator<double> rolls;

            public DrawRollSource(IEnumerable<double> rolls)
            {
                this.rolls = rolls?.GetEnumerator();
            }

            public double Next()
            {
                if (rolls != null && rolls.MoveNext())
                {
                    return rolls.Current;
                }

                return DefaultMarketDrawRoll();
            }
        }

        private static double DefaultMarketDrawRoll()
        {
            var roll = UnityEngine.Random.value;
            return roll >= 1f ? 0.999999999d : roll;
        }
    }
}
