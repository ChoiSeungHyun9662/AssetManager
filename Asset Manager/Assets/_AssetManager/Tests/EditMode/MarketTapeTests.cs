using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace AssetManager.Tests
{
    public sealed class MarketTapeTests
    {
        [Test]
        public void RefreshFillsConfiguredSlotsWithoutShowingDuplicateCards()
        {
            var staticData = RunStaticDataSet.CreateMvpDefaults();
            var run = RunBootstrapper.CreateNewRun(staticData);

            var tape = MarketTape.Refresh(
                staticData.MarketConfig,
                run.AssetCards,
                run.MarketTape,
                run.OwnedAssets,
                run.Reservation);

            Assert.That(tape.Slots, Has.Count.EqualTo(staticData.MarketConfig.MarketTapeSlots));
            Assert.That(CollectSlotCardIds(tape), Is.Unique);
        }

        [Test]
        public void RefreshFillsEightOrderedTapeSlotsWithoutShowingDuplicateCards()
        {
            var staticData = RunStaticDataSet.CreateMvpDefaults();
            var run = RunBootstrapper.CreateNewRun(staticData);

            var tape = MarketTape.Refresh(
                staticData.MarketConfig,
                run.AssetCards,
                run.MarketTape,
                run.OwnedAssets,
                run.Reservation);

            Assert.That(tape.Slots, Has.Count.EqualTo(8));
            Assert.That(CollectSlotCardIds(tape), Is.Unique);
        }

        [Test]
        public void AdvanceRemovesLeftmostNonReservedCardAndMovesRemainingSlotsLeft()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var oldSlotCardIds = new List<string>(CollectSlotRuntimeIds(run.MarketTape));

            var advancedRun = MarketTape.Advance(run);

            var advancedSlotCardIds = new List<string>(CollectSlotRuntimeIds(advancedRun.MarketTape));
            Assert.That(advancedRun.MarketTape.Slots, Has.Count.EqualTo(8));
            Assert.That(advancedSlotCardIds, Has.Count.EqualTo(8));
            for (var i = 0; i < oldSlotCardIds.Count - 1; i++)
            {
                Assert.That(advancedSlotCardIds[i], Is.EqualTo(oldSlotCardIds[i + 1]));
            }

            Assert.That(advancedSlotCardIds, Does.Not.Contain(oldSlotCardIds[0]));
            Assert.That(FindRuntimeCard(advancedRun.AssetCards, oldSlotCardIds[0]).State, Is.EqualTo(AssetCardRuntimeState.Removed));
        }

        [Test]
        public void RefreshExcludesOwnedReservedRemovedAndCurrentlyVisibleCards()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var visibleCardIds = new HashSet<string>(CollectVisibleCardIds(run.MarketTape));
            var ownedCard = run.AssetCards[5];
            var reservedCard = run.AssetCards[6];
            var removedCard = run.AssetCards[7];
            var assetCards = new List<AssetCardRuntimeData>();

            foreach (var card in run.AssetCards)
            {
                if (card.Card.Id == ownedCard.Card.Id)
                {
                    assetCards.Add(new AssetCardRuntimeData(card.Card, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape));
                }
                else if (card.Card.Id == reservedCard.Card.Id)
                {
                    assetCards.Add(new AssetCardRuntimeData(card.Card, AssetCardRuntimeState.Reserved, null));
                }
                else if (card.Card.Id == removedCard.Card.Id)
                {
                    assetCards.Add(new AssetCardRuntimeData(card.Card, AssetCardRuntimeState.Removed, null));
                }
                else
                {
                    assetCards.Add(card);
                }
            }

            var tape = MarketTape.Refresh(
                new MarketConfigData(1, 1, 1),
                assetCards,
                run.MarketTape,
                new OwnedAssetState(new[] { ownedCard }),
                new ReservationState(3, new[] { reservedCard }));

            var refreshedCardIds = new HashSet<string>(CollectVisibleCardIds(tape));
            Assert.That(refreshedCardIds.Overlaps(visibleCardIds), Is.False);
            Assert.That(refreshedCardIds.Contains(ownedCard.Card.Id), Is.False);
            Assert.That(refreshedCardIds.Contains(reservedCard.Card.Id), Is.False);
            Assert.That(refreshedCardIds.Contains(removedCard.Card.Id), Is.False);
        }

        [Test]
        public void RefreshUsesMarketDeckDrawRulesForEachSuppliedCard()
        {
            var stockCard = CreateCard("stock-card", CardDomain.Stock);
            var resourceCard = CreateCard("resource-card", CardDomain.ConsumableResource);

            var tape = MarketTape.Refresh(
                new MarketConfigData(0, 0, 2, 0.75),
                new[] { stockCard, resourceCard },
                null,
                null,
                null,
                new[] { 0.10, 0.90 });

            Assert.That(tape.Slots[0].Card.Card.CardDomain, Is.EqualTo(CardDomain.Stock));
            Assert.That(tape.Slots[1].Card.Card.CardDomain, Is.EqualTo(CardDomain.ConsumableResource));
        }

        [Test]
        public void RefreshWithoutSuppliedRollsCanDrawConsumableResourceCardsFromDefaultDrawSource()
        {
            var cards = new List<AssetCardRuntimeData>();
            for (var i = 0; i < 24; i++)
            {
                cards.Add(CreateCard("stock-card-" + i, CardDomain.Stock));
                cards.Add(CreateCard("resource-card-" + i, CardDomain.ConsumableResource));
            }

            var sawConsumableResourceCard = false;
            for (var seed = 0; seed < 25; seed++)
            {
                Random.InitState(seed);
                var tape = MarketTape.Refresh(
                    new MarketConfigData(8, 0.75),
                    cards,
                    null,
                    null,
                    null);

                if (ContainsCardDomain(tape, CardDomain.ConsumableResource))
                {
                    sawConsumableResourceCard = true;
                    break;
                }
            }

            Assert.That(sawConsumableResourceCard, Is.True);
        }

        [Test]
        public void RefreshThrowsWhenBothMarketDecksCannotSupplyARequiredCard()
        {
            Assert.Throws<MarketDeckExhaustedException>(() =>
                MarketTape.Refresh(
                    new MarketConfigData(0, 0, 1, 0.75),
                    new AssetCardRuntimeData[0],
                    null,
                    null,
                    null,
                    new[] { 0.10 }));
        }

        [Test]
        public void PullFromEmptySlotCompressesOnlyCardsToTheRightAndRefillsRightmostEmptySlot()
        {
            var cards = CreateCards("a", "c", "d", "e", "f", "g", "h", "new");
            var tape = new MarketTapeState(new[]
            {
                new MarketTapeSlotState(cards[0], false),
                new MarketTapeSlotState(null, false),
                new MarketTapeSlotState(cards[1], false),
                new MarketTapeSlotState(cards[2], false),
                new MarketTapeSlotState(cards[3], false),
                new MarketTapeSlotState(cards[4], false),
                new MarketTapeSlotState(cards[5], false),
                new MarketTapeSlotState(cards[6], false)
            });

            var pulled = MarketTape.PullFromEmptySlot(
                new MarketConfigData(8, 0.75),
                cards,
                tape,
                null,
                null,
                1);

            AssertSlotIds(pulled, "a", "c", "d", "e", "f", "g", "h", "new");
        }

        [Test]
        public void PullAllEmptySlotsAppliesPullsFromTheLeftmostEmptySlotFirst()
        {
            var cards = CreateCards("a", "c", "e", "f", "new-1", "new-2", "new-3", "new-4");
            var tape = new MarketTapeState(new[]
            {
                new MarketTapeSlotState(cards[0], false),
                new MarketTapeSlotState(null, false),
                new MarketTapeSlotState(cards[1], false),
                new MarketTapeSlotState(null, false),
                new MarketTapeSlotState(cards[2], false),
                new MarketTapeSlotState(cards[3], false),
                new MarketTapeSlotState(null, false),
                new MarketTapeSlotState(null, false)
            });

            var pulled = MarketTape.PullAllEmptySlots(
                new MarketConfigData(8, 0.75),
                cards,
                tape,
                null,
                null);

            AssertSlotIds(pulled, "a", "c", "e", "f", "new-1", "new-2", "new-3", "new-4");
        }

        [Test]
        public void AdvanceAndRefreshLeaveReservationUnchanged()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ReserveFirstCurrentMarketCard(run);
            var reservedCardId = FindReservedSlotCardId(run.MarketTape);
            var reservedSlotIndex = FindSlotIndex(run.MarketTape, reservedCardId);

            var advancedRun = MarketTape.Advance(run);

            Assert.That(advancedRun.Reservation.ReservedCards, Is.Empty);
            Assert.That(advancedRun.MarketTape.Slots[reservedSlotIndex].Card.Card.Id, Is.EqualTo(reservedCardId));
            Assert.That(advancedRun.MarketTape.Slots[reservedSlotIndex].IsReserved, Is.True);

            var refreshedRun = MarketTape.Refresh(advancedRun);

            Assert.That(refreshedRun.Reservation.ReservedCards, Is.Empty);
            Assert.That(refreshedRun.MarketTape.Slots[reservedSlotIndex].Card.Card.Id, Is.EqualTo(reservedCardId));
            Assert.That(refreshedRun.MarketTape.Slots[reservedSlotIndex].IsReserved, Is.True);
        }

        private static AssetCardRuntimeData CreateCard(string id, CardDomain cardDomain)
        {
            return new AssetCardRuntimeData(
                new AssetCardData(
                    id,
                    id,
                    "Market tape test card.",
                    AssetRarity.Common,
                    1,
                    new ProfessionalResourceCost[0],
                    cardDomain == CardDomain.Stock ? 1 : 0,
                    0,
                    new TagData[0],
                    cardDomain: cardDomain,
                    providedResourceType: ResourceType.Cash,
                    providedResourceAmount: cardDomain == CardDomain.ConsumableResource ? 1 : 0),
                AssetCardRuntimeState.Available,
                null);
        }

        private static IReadOnlyList<AssetCardRuntimeData> CreateCards(params string[] ids)
        {
            var cards = new List<AssetCardRuntimeData>();
            foreach (var id in ids)
            {
                cards.Add(CreateCard(id, CardDomain.Stock));
            }

            return cards;
        }

        private static void AssertSlotIds(MarketTapeState tape, params string[] expectedCardIds)
        {
            Assert.That(tape.Slots, Has.Count.EqualTo(expectedCardIds.Length));
            for (var i = 0; i < expectedCardIds.Length; i++)
            {
                Assert.That(tape.Slots[i].Card.Card.Id, Is.EqualTo(expectedCardIds[i]));
            }
        }

        private static IEnumerable<string> CollectVisibleCardIds(MarketTapeState tape)
        {
            foreach (var card in tape.SellImminentCards)
            {
                yield return card.Card.Id;
            }

            foreach (var card in tape.CurrentMarketCards)
            {
                yield return card.Card.Id;
            }

            foreach (var card in tape.UpcomingMarketCards)
            {
                yield return card.Card.Id;
            }
        }

        private static IEnumerable<string> CollectSlotCardIds(MarketTapeState tape)
        {
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty)
                {
                    yield return slot.Card.Card.Id;
                }
            }
        }

        private static IEnumerable<string> CollectSlotRuntimeIds(MarketTapeState tape)
        {
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty)
                {
                    yield return slot.Card.RuntimeId;
                }
            }
        }

        private static bool ContainsCardDomain(MarketTapeState tape, CardDomain cardDomain)
        {
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty && slot.Card.Card.CardDomain == cardDomain)
                {
                    return true;
                }
            }

            return false;
        }

        private static RunSessionState ReserveFirstCurrentMarketCard(RunSessionState run)
        {
            var detailRun = MarketAreaFlow.OpenMarketCardDetail(run, FindFirstReservableMarketCard(run.MarketTape));
            return ReservationAction.ConfirmReservation(detailRun).Run;
        }

        private static AssetCardRuntimeData FindFirstReservableMarketCard(MarketTapeState tape)
        {
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsReserved
                    && !slot.IsEmpty
                    && slot.Card.State == AssetCardRuntimeState.Available
                    && slot.Card.Card.CardDomain == CardDomain.Stock)
                {
                    return slot.Card;
                }
            }

            Assert.Fail("Expected to find a reservable market card.");
            return null;
        }

        private static string FindReservedSlotCardId(MarketTapeState tape)
        {
            foreach (var slot in tape.Slots)
            {
                if (slot.IsReserved && !slot.IsEmpty)
                {
                    return slot.Card.Card.Id;
                }
            }

            Assert.Fail("Expected to find a reserved market slot.");
            return string.Empty;
        }

        private static int FindSlotIndex(MarketTapeState tape, string cardId)
        {
            for (var i = 0; i < tape.Slots.Count; i++)
            {
                if (!tape.Slots[i].IsEmpty && tape.Slots[i].Card.Card.Id == cardId)
                {
                    return i;
                }
            }

            Assert.Fail("Expected to find market slot for " + cardId + ".");
            return -1;
        }

        private static AssetCardRuntimeData FindCard(IEnumerable<AssetCardRuntimeData> cards, string cardId)
        {
            foreach (var card in cards)
            {
                if (card.Card.Id == cardId)
                {
                    return card;
                }
            }

            Assert.Fail("Expected to find card " + cardId + ".");
            return null;
        }

        private static AssetCardRuntimeData FindRuntimeCard(IEnumerable<AssetCardRuntimeData> cards, string runtimeId)
        {
            foreach (var card in cards)
            {
                if (card.RuntimeId == runtimeId)
                {
                    return card;
                }
            }

            Assert.Fail("Expected to find runtime card " + runtimeId + ".");
            return null;
        }

        private static List<string> CollectCardIds(IReadOnlyList<AssetCardRuntimeData> cards)
        {
            var cardIds = new List<string>();
            foreach (var card in cards)
            {
                cardIds.Add(card.Card.Id);
            }

            return cardIds;
        }

        private static void AssertZoneMatches(IReadOnlyList<AssetCardRuntimeData> actualCards, IReadOnlyList<string> expectedCardIds)
        {
            Assert.That(actualCards, Has.Count.EqualTo(expectedCardIds.Count));
            for (var i = 0; i < expectedCardIds.Count; i++)
            {
                Assert.That(actualCards[i].Card.Id, Is.EqualTo(expectedCardIds[i]));
            }
        }
    }
}
