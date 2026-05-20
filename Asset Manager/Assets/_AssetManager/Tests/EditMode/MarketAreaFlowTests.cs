using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class MarketAreaFlowTests
    {
        [Test]
        public void MarketCardSelectionKeepsSingleMarketStateAndPreparesPurchase()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;
            var selectedCard = FindFirstReservableMarketCard(run.MarketTape);

            var selectedRun = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);

            Assert.That(selectedRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(selectedRun.CardDetail.SelectedCard, Is.SameAs(selectedCard));
            Assert.That(selectedRun.CardDetail.PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
            Assert.That(selectedRun.CardDetail.DisplayData.CardId, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(selectedRun.CardDetail.PendingPayment, Is.Not.Null);
            Assert.That(selectedRun.CardDetail.ShouldShowReserveButton, Is.True);
            Assert.That(MarketAreaFlow.CanAdvanceToNextBusinessDay(selectedRun), Is.True);

            var advancedRun = BusinessDayFlow.AdvanceToNextBusinessDay(selectedRun);

            Assert.That(advancedRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays - 1));
            Assert.That(advancedRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));

            var closedRun = MarketAreaFlow.CloseCardDetail(selectedRun);

            Assert.That(closedRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(closedRun.CardDetail.SelectedCard, Is.Null);
            Assert.That(closedRun.CardDetail.PendingPayment, Is.Null);
            Assert.That(closedRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(MarketAreaFlow.CanAdvanceToNextBusinessDay(closedRun), Is.True);
        }

        [Test]
        public void ReservedMarketCardDetailRecordsMarketTapeSourceAndHidesReserveButton()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var marketCard = FindFirstReservableMarketCard(run.MarketTape);
            var reservedCard = new AssetCardRuntimeData(
                marketCard.Card,
                AssetCardRuntimeState.Reserved,
                null);

            var detailRun = MarketAreaFlow.OpenMarketCardDetail(run, reservedCard);

            Assert.That(detailRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(detailRun.CardDetail.SelectedCard, Is.SameAs(reservedCard));
            Assert.That(detailRun.CardDetail.PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
            Assert.That(detailRun.CardDetail.PendingPayment, Is.Not.Null);
            Assert.That(detailRun.CardDetail.ShouldShowReserveButton, Is.False);
        }

        [Test]
        public void MarketPreviewSelectionDoesNotEnterCardDetailState()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var selectedCard = FindFirstReservableMarketCard(run.MarketTape);

            var previewRun = MarketAreaFlow.OpenMarketPreviewCardDetail(run, selectedCard);

            Assert.That(previewRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(previewRun.CardDetail.SelectedCard, Is.Null);
            Assert.That(previewRun.CardDetail.PendingPayment, Is.Null);
            Assert.That(MarketAreaFlow.CanAdvanceToNextBusinessDay(previewRun), Is.True);
        }

        [Test]
        public void ExtraBuyChoiceOpensCardDetailAsExtraBuyAndBlocksReservation()
        {
            var run = ExtraBuyAction.BeginChoice(RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults()));
            var selectedCard = FindFirstReservableMarketCard(run.MarketTape);

            var detailRun = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);

            Assert.That(detailRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(detailRun.BusinessDay.HasExtraBuyAction, Is.True);
            Assert.That(detailRun.BusinessDay.IsAwaitingExtraBuyChoice, Is.False);
            Assert.That(detailRun.BusinessDay.IsBuyingWithExtraBuy, Is.True);
            Assert.That(detailRun.CardDetail.IsOpenedDuringExtraBuy, Is.True);
            Assert.That(detailRun.CardDetail.ShouldShowReserveButton, Is.False);
            Assert.That(ReservationAction.CanReserve(detailRun), Is.False);

            var closedRun = MarketAreaFlow.CloseCardDetail(detailRun);

            Assert.That(closedRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(closedRun.BusinessDay.HasExtraBuyAction, Is.True);
            Assert.That(closedRun.BusinessDay.IsAwaitingExtraBuyChoice, Is.True);
            Assert.That(closedRun.BusinessDay.IsBuyingWithExtraBuy, Is.False);
        }

        [Test]
        public void ExtraBuyChoiceDoesNotOpenReservedStockAsPurchaseCandidate()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var marketCard = FindFirstReservableMarketCard(run.MarketTape);
            run = MarketAreaFlow.OpenMarketCardDetail(run, marketCard);
            run = ReservationAction.ConfirmReservation(run).Run;
            var reservedCard = FindFirstReservedMarketCard(run.MarketTape);
            run = ExtraBuyAction.BeginChoice(run);

            var detailRun = MarketAreaFlow.OpenMarketCardDetail(run, reservedCard);

            Assert.That(detailRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(detailRun.BusinessDay.HasExtraBuyAction, Is.True);
            Assert.That(detailRun.BusinessDay.IsAwaitingExtraBuyChoice, Is.True);
            Assert.That(detailRun.BusinessDay.IsBuyingWithExtraBuy, Is.False);
            Assert.That(detailRun.CardDetail.SelectedCard, Is.Null);
        }

        [Test]
        public void ExtraBuyChoiceOnlyOpensAllowedConsumableResourceCards()
        {
            var run = ExtraBuyAction.BeginChoice(RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults()));
            var blockedResourceCard = CreateConsumableResourceCard("blocked-extra-buy-resource", false);
            run = WithMarketSlotCard(run, blockedResourceCard, 0);

            var blockedRun = MarketAreaFlow.OpenMarketCardDetail(run, blockedResourceCard);

            Assert.That(blockedRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(blockedRun.BusinessDay.IsAwaitingExtraBuyChoice, Is.True);
            Assert.That(blockedRun.CardDetail.SelectedCard, Is.Null);

            var allowedResourceCard = CreateConsumableResourceCard("allowed-extra-buy-resource", true);
            run = WithMarketSlotCard(run, allowedResourceCard, 0);

            var allowedRun = MarketAreaFlow.OpenMarketCardDetail(run, allowedResourceCard);

            Assert.That(allowedRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(allowedRun.BusinessDay.IsBuyingWithExtraBuy, Is.True);
            Assert.That(allowedRun.CardDetail.SelectedCard, Is.SameAs(allowedResourceCard));
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

            Assert.Fail("Expected to find a reservable stock market card.");
            return null;
        }

        private static AssetCardRuntimeData FindFirstReservedMarketCard(MarketTapeState tape)
        {
            foreach (var slot in tape.Slots)
            {
                if (slot.IsReserved && !slot.IsEmpty)
                {
                    return slot.Card;
                }
            }

            Assert.Fail("Expected to find a reserved market card.");
            return null;
        }

        private static AssetCardRuntimeData CreateConsumableResourceCard(string id, bool canBePurchasedWithExtraBuy)
        {
            return new AssetCardRuntimeData(
                new AssetCardData(
                    id,
                    string.Empty,
                    "Extra buy resource candidate test.",
                    AssetRarity.Common,
                    1,
                    new ProfessionalResourceCost[0],
                    0,
                    0,
                    new TagData[0],
                    cardDomain: CardDomain.ConsumableResource,
                    providedResourceType: ResourceType.Cash,
                    providedResourceAmount: 1,
                    canBePurchasedWithExtraBuy: canBePurchasedWithExtraBuy),
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
                if (card.RuntimeId == replacement.RuntimeId)
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
    }
}
