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
            var selectedCard = run.MarketTape.CurrentMarketCards[0];

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
        public void ReservedCardDetailHookRecordsReservedSourceAndHidesReserveButton()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var marketCard = run.MarketTape.CurrentMarketCards[0];
            var reservedCard = new AssetCardRuntimeData(
                marketCard.Card,
                AssetCardRuntimeState.Reserved,
                null);

            var detailRun = MarketAreaFlow.OpenReservedCardDetail(run, reservedCard);

            Assert.That(detailRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));
            Assert.That(detailRun.CardDetail.SelectedCard, Is.SameAs(reservedCard));
            Assert.That(detailRun.CardDetail.PurchaseSource, Is.EqualTo(PurchaseSource.Reserved));
            Assert.That(detailRun.CardDetail.PendingPayment, Is.Not.Null);
            Assert.That(detailRun.CardDetail.ShouldShowReserveButton, Is.False);
        }
    }
}
