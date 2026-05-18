using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class MarketAreaFlowTests
    {
        [Test]
        public void MarketCardDetailGatesNextBusinessDayUntilClosed()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;
            var selectedCard = FindFirstReservableMarketCard(run.MarketTape);

            var detailRun = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);

            Assert.That(detailRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));
            Assert.That(detailRun.CardDetail.SelectedCard, Is.SameAs(selectedCard));
            Assert.That(detailRun.CardDetail.PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
            Assert.That(detailRun.CardDetail.DisplayData.CardId, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(detailRun.CardDetail.PendingPayment, Is.Not.Null);
            Assert.That(detailRun.CardDetail.ShouldShowReserveButton, Is.True);
            Assert.That(MarketAreaFlow.CanAdvanceToNextBusinessDay(detailRun), Is.False);

            var blockedRun = BusinessDayFlow.AdvanceToNextBusinessDay(detailRun);

            Assert.That(blockedRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(blockedRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));

            var closedRun = MarketAreaFlow.CloseCardDetail(blockedRun);

            Assert.That(closedRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(closedRun.CardDetail.SelectedCard, Is.Null);
            Assert.That(closedRun.CardDetail.PendingPayment, Is.Null);
            Assert.That(closedRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(MarketAreaFlow.CanAdvanceToNextBusinessDay(closedRun), Is.True);
        }

        [Test]
        public void ReservedCardDetailHookRecordsMarketTapeSourceAndHidesReserveButton()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var marketCard = FindFirstReservableMarketCard(run.MarketTape);
            var reservedCard = new AssetCardRuntimeData(
                marketCard.Card,
                AssetCardRuntimeState.Reserved,
                null);

            var detailRun = MarketAreaFlow.OpenReservedCardDetail(run, reservedCard);

            Assert.That(detailRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));
            Assert.That(detailRun.CardDetail.SelectedCard, Is.SameAs(reservedCard));
            Assert.That(detailRun.CardDetail.PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
            Assert.That(detailRun.CardDetail.PendingPayment, Is.Not.Null);
            Assert.That(detailRun.CardDetail.ShouldShowReserveButton, Is.False);
        }

        [Test]
        public void ExtraBuyChoiceOpensCardDetailAsExtraBuyAndBlocksReservation()
        {
            var run = ExtraBuyAction.BeginChoice(RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults()));
            var selectedCard = FindFirstReservableMarketCard(run.MarketTape);

            var detailRun = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);

            Assert.That(detailRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));
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
    }
}
