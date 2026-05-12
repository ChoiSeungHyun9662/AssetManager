using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class ReservationActionTests
    {
        [Test]
        public void MarketCardReservationMovesCardToReservationGrantsDealAndPressureAdvancesOnlyReservedColumnAndConsumesBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            var selectedCard = run.MarketTape.CurrentMarketCards[0];
            var previousCurrentMarketSecondCardId = run.MarketTape.CurrentMarketCards[1].Card.Id;
            var previousUpcomingMarketFirstCardId = run.MarketTape.UpcomingMarketCards[0].Card.Id;
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;

            var result = ReservationAction.ConfirmReservation(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Reservation.ReservedCards, Has.Count.EqualTo(1));
            Assert.That(result.Run.Reservation.ReservedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(result.Run.Reservation.ReservedCards[0].State, Is.EqualTo(AssetCardRuntimeState.Reserved));
            Assert.That(FindCard(result.Run.AssetCards, selectedCard.Card.Id).State, Is.EqualTo(AssetCardRuntimeState.Reserved));
            Assert.That(result.Run.Resources.Research, Is.EqualTo(1));
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(1));
            Assert.That(result.Run.RedemptionPressure.CurrentPressure, Is.EqualTo(1));
            Assert.That(result.Run.MarketTape.CurrentMarketCards[0].Card.Id, Is.EqualTo(previousUpcomingMarketFirstCardId));
            Assert.That(result.Run.MarketTape.CurrentMarketCards[1].Card.Id, Is.EqualTo(previousCurrentMarketSecondCardId));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(run.Calendar.RemainingBusinessDays - 1));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(result.Run.CardDetail.SelectedCard, Is.Null);
        }

        [Test]
        public void ReservationAtDealCapStillSucceedsAndReportsDiscardedDeal()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddDeal(run, 3).Run;
            run = MarketAreaFlow.OpenMarketCardDetail(run, run.MarketTape.CurrentMarketCards[0]);

            var result = ReservationAction.ConfirmReservation(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(3));
            Assert.That(result.Run.Reservation.ReservedCards, Has.Count.EqualTo(1));
            Assert.That(result.Message, Is.EqualTo("딜 최대 보유: 추가 딜 폐기"));
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
            run = MarketAreaFlow.OpenMarketCardDetail(run, run.MarketTape.CurrentMarketCards[0]);

            var result = ReservationAction.ConfirmReservation(run);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Run.Reservation.ReservedCards, Has.Count.EqualTo(3));
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
            run = MarketAreaFlow.OpenMarketCardDetail(run, run.MarketTape.CurrentMarketCards[0]);

            var result = ReservationAction.ConfirmReservation(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.State, Is.EqualTo(RunState.Failed));
            Assert.That(result.Run.RedemptionPressure.CurrentPressure, Is.EqualTo(10));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(result.Run.CardDetail.SelectedCard, Is.Null);
        }

        private static RunSessionState ReserveFirstCurrentMarketCard(RunSessionState run)
        {
            var detailRun = MarketAreaFlow.OpenMarketCardDetail(run, run.MarketTape.CurrentMarketCards[0]);
            return ReservationAction.ConfirmReservation(detailRun).Run;
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
    }
}
