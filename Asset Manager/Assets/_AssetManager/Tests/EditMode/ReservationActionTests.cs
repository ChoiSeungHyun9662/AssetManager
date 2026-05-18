using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class ReservationActionTests
    {
        [Test]
        public void MarketCardReservationLocksCardInMarketSlotGrantsDealAndPressureAndConsumesBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            var selectedCard = FindFirstReservableMarketCard(run.MarketTape);
            var selectedSlotIndex = FindSlotIndex(run.MarketTape, selectedCard.Card.Id);
            var previousSlotIds = CollectSlotCardIds(run.MarketTape);
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;

            var result = ReservationAction.ConfirmReservation(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Reservation.ReservedCards, Is.Empty);
            Assert.That(FindCard(result.Run.AssetCards, selectedCard.Card.Id).State, Is.EqualTo(AssetCardRuntimeState.Reserved));
            Assert.That(result.Run.Resources.Research, Is.EqualTo(1));
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(1));
            Assert.That(result.Run.RedemptionPressure.CurrentPressure, Is.EqualTo(1));
            Assert.That(result.Run.MarketTape.Slots[selectedSlotIndex].Card.Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(result.Run.MarketTape.Slots[selectedSlotIndex].IsReserved, Is.True);
            Assert.That(CountReservedSlots(result.Run.MarketTape), Is.EqualTo(1));
            Assert.That(CollectSlotCardIds(result.Run.MarketTape), Is.EqualTo(previousSlotIds));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(run.Calendar.RemainingBusinessDays - 1));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(result.Run.CardDetail.SelectedCard, Is.Null);
        }

        [Test]
        public void ReservationAtDealCapStillSucceedsAndReportsDiscardedDeal()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddDeal(run, 3).Run;
            run = MarketAreaFlow.OpenMarketCardDetail(run, FindFirstReservableMarketCard(run.MarketTape));

            var result = ReservationAction.ConfirmReservation(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(3));
            Assert.That(CountReservedSlots(result.Run.MarketTape), Is.EqualTo(1));
            Assert.That(result.Message, Is.EqualTo("딜 한도: 추가 딜 버림"));
        }

        [Test]
        public void ConsumableResourceMarketCardCannotBeReserved()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var resourceCard = CreateConsumableResourceCard();
            run = WithMarketSlotCard(run, resourceCard, 0);
            run = MarketAreaFlow.OpenMarketCardDetail(run, resourceCard);

            var result = ReservationAction.ConfirmReservation(run);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Run.MarketTape.Slots[0].IsReserved, Is.False);
            Assert.That(FindCard(result.Run.AssetCards, resourceCard.Card.Id).State, Is.EqualTo(AssetCardRuntimeState.Available));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(run.Calendar.RemainingBusinessDays));
        }

        [Test]
        public void FullReservationAreaDoesNotReserveOrConsumeBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ReserveFirstCurrentMarketCard(run);
            run = ReserveFirstCurrentMarketCard(run);
            run = ReserveFirstCurrentMarketCard(run);
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;
            var deal = run.Resources.Deal;
            var pressure = run.RedemptionPressure.CurrentPressure;
            run = MarketAreaFlow.OpenMarketCardDetail(run, FindFirstReservableMarketCard(run.MarketTape));

            var result = ReservationAction.ConfirmReservation(run);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(CountReservedSlots(result.Run.MarketTape), Is.EqualTo(3));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(deal));
            Assert.That(result.Run.RedemptionPressure.CurrentPressure, Is.EqualTo(pressure));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));
            Assert.That(result.Message, Is.EqualTo("예약 구역이 가득 찼습니다."));
        }

        [Test]
        public void ReservationAtNinePressureFailsRunImmediately()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithRedemptionPressure(run, 9);
            run = MarketAreaFlow.OpenMarketCardDetail(run, FindFirstReservableMarketCard(run.MarketTape));

            var result = ReservationAction.ConfirmReservation(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.State, Is.EqualTo(RunState.Failed));
            Assert.That(result.Run.RedemptionPressure.CurrentPressure, Is.EqualTo(10));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(result.Run.CardDetail.SelectedCard, Is.Null);
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

        private static RunSessionState WithRedemptionPressure(RunSessionState run, int currentPressure)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                new RedemptionPressureState(currentPressure, run.RedemptionPressure.MaxPressure),
                run.CardDetail,
                run.LiquidityAction);
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

        private static System.Collections.Generic.List<string> CollectSlotCardIds(MarketTapeState tape)
        {
            var cardIds = new System.Collections.Generic.List<string>();
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty)
                {
                    cardIds.Add(slot.Card.Card.Id);
                }
            }

            return cardIds;
        }

        private static AssetCardRuntimeData FindCard(System.Collections.Generic.IEnumerable<AssetCardRuntimeData> cards, string cardId)
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

        private static AssetCardRuntimeData CreateConsumableResourceCard()
        {
            return new AssetCardRuntimeData(
                new AssetCardData(
                    "reservation-resource-card",
                    string.Empty,
                    "Reservation resource card test.",
                    AssetRarity.Common,
                    1,
                    new ProfessionalResourceCost[0],
                    0,
                    0,
                    new TagData[0],
                    cardDomain: CardDomain.ConsumableResource,
                    providedResourceType: ResourceType.Cash,
                    providedResourceAmount: 1),
                AssetCardRuntimeState.Available,
                null);
        }

        private static RunSessionState WithMarketSlotCard(
            RunSessionState run,
            AssetCardRuntimeData runtimeCard,
            int slotIndex)
        {
            var slots = new System.Collections.Generic.List<MarketTapeSlotState>(run.MarketTape.Slots);
            slots[slotIndex] = new MarketTapeSlotState(runtimeCard, false);

            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                ReplaceCard(run.AssetCards, runtimeCard),
                new MarketTapeState(slots),
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> ReplaceCard(
            System.Collections.Generic.IEnumerable<AssetCardRuntimeData> cards,
            AssetCardRuntimeData replacement)
        {
            var updatedCards = new System.Collections.Generic.List<AssetCardRuntimeData>();
            var replaced = false;
            foreach (var card in cards)
            {
                if (card.Card.Id == replacement.Card.Id)
                {
                    updatedCards.Add(replacement);
                    replaced = true;
                }
                else
                {
                    updatedCards.Add(card);
                }
            }

            if (!replaced)
            {
                updatedCards.Add(replacement);
            }

            return updatedCards;
        }

        private static int CountReservedSlots(MarketTapeState tape)
        {
            var count = 0;
            foreach (var slot in tape.Slots)
            {
                if (slot.IsReserved)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
