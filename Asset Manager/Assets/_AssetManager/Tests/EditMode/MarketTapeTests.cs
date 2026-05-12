using System.Collections.Generic;
using NUnit.Framework;

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

            Assert.That(tape.SellImminentCards, Has.Count.EqualTo(staticData.MarketConfig.SellImminentSlots));
            Assert.That(tape.CurrentMarketCards, Has.Count.EqualTo(staticData.MarketConfig.CurrentMarketSlots));
            Assert.That(tape.UpcomingMarketCards, Has.Count.EqualTo(staticData.MarketConfig.UpcomingMarketSlots));
            Assert.That(CollectVisibleCardIds(tape), Is.Unique);
        }

        [Test]
        public void AdvanceRemovesSellImminentAndRefillsUpcomingMarket()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var oldSellImminentCardIds = CollectCardIds(run.MarketTape.SellImminentCards);
            var oldCurrentMarketCardIds = CollectCardIds(run.MarketTape.CurrentMarketCards);
            var oldUpcomingMarketCardIds = CollectCardIds(run.MarketTape.UpcomingMarketCards);

            var advancedRun = MarketTape.Advance(run);

            AssertZoneMatches(advancedRun.MarketTape.SellImminentCards, oldCurrentMarketCardIds);
            AssertZoneMatches(advancedRun.MarketTape.CurrentMarketCards, oldUpcomingMarketCardIds);
            Assert.That(advancedRun.MarketTape.UpcomingMarketCards, Has.Count.EqualTo(run.StaticData.MarketConfig.UpcomingMarketSlots));
            Assert.That(CollectVisibleCardIds(advancedRun.MarketTape), Is.Unique);
            foreach (var cardId in oldSellImminentCardIds)
            {
                Assert.That(FindCard(advancedRun.AssetCards, cardId).State, Is.EqualTo(AssetCardRuntimeState.Removed));
            }
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
        public void AdvanceAndRefreshLeaveReservationUnchanged()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ReserveFirstCurrentMarketCard(run);
            var reservedCardId = run.Reservation.ReservedCards[0].Card.Id;

            var advancedRun = MarketTape.Advance(run);

            Assert.That(advancedRun.Reservation.ReservedCards, Has.Count.EqualTo(1));
            Assert.That(advancedRun.Reservation.ReservedCards[0].Card.Id, Is.EqualTo(reservedCardId));

            var refreshedRun = MarketTape.Refresh(advancedRun);

            Assert.That(refreshedRun.Reservation.ReservedCards, Has.Count.EqualTo(1));
            Assert.That(refreshedRun.Reservation.ReservedCards[0].Card.Id, Is.EqualTo(reservedCardId));
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

        private static RunSessionState ReserveFirstCurrentMarketCard(RunSessionState run)
        {
            var detailRun = MarketAreaFlow.OpenMarketCardDetail(run, run.MarketTape.CurrentMarketCards[0]);
            return ReservationAction.ConfirmReservation(detailRun).Run;
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
