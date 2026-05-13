using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class BusinessDayFlowTests
    {
        [Test]
        public void AdvanceToNextBusinessDayConsumesExactlyOneBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());

            var nextRun = BusinessDayFlow.AdvanceToNextBusinessDay(run);

            Assert.That(nextRun.State, Is.EqualTo(RunState.Playing));
            Assert.That(nextRun.Calendar.FiscalYear, Is.EqualTo(1));
            Assert.That(nextRun.Calendar.Quarter, Is.EqualTo(1));
            Assert.That(nextRun.Calendar.RemainingBusinessDays, Is.EqualTo(3));
            Assert.That(nextRun.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.AwaitingAction));
            Assert.That(nextRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
        }

        [Test]
        public void AdvanceToNextBusinessDayAppliesOwnedAssetIncomeAsEarnedCash()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var ownedCard = new AssetCardRuntimeData(
                run.AssetCards[0].Card,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape);
            run = WithOwnedAssets(run, new OwnedAssetState(new[] { ownedCard }));

            var nextRun = BusinessDayFlow.AdvanceToNextBusinessDay(run);

            Assert.That(nextRun.Resources.Cash, Is.EqualTo(run.Resources.Cash + ownedCard.Card.Income));
            Assert.That(nextRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(ownedCard.Card.Income));
            Assert.That(nextRun.Performance.CurrentFiscalYearEarnedCash, Is.EqualTo(ownedCard.Card.Income));
            Assert.That(nextRun.Performance.TotalEarnedCash, Is.EqualTo(ownedCard.Card.Income));
        }

        [Test]
        public void AdvanceToNextBusinessDayEntersQuarterSettlementAfterLastBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());

            run = CompleteCurrentQuarter(run);

            Assert.That(run.State, Is.EqualTo(RunState.Playing));
            Assert.That(run.Calendar.FiscalYear, Is.EqualTo(1));
            Assert.That(run.Calendar.Quarter, Is.EqualTo(1));
            Assert.That(run.Calendar.RemainingBusinessDays, Is.EqualTo(0));
            Assert.That(run.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.QuarterSettlement));
            Assert.That(run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
        }

        [Test]
        public void LastBusinessDaySettlementAppliesQuarterEndResultWithoutStartingAnotherBusinessDayIncome()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var ownedCard = new AssetCardRuntimeData(
                run.AssetCards[0].Card,
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape);
            run = WithOwnedAssets(run, new OwnedAssetState(new[] { ownedCard }));
            run = WithCalendar(run, new RunCalendarState(1, 1, 1));

            run = BusinessDayFlow.AdvanceToNextBusinessDay(run);

            Assert.That(run.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.QuarterSettlement));
            Assert.That(run.QuarterEndResult, Is.Not.Null);
            Assert.That(run.QuarterEndResult.SettlementIncome, Is.EqualTo(ownedCard.Card.ManagementValue));
            Assert.That(run.Performance.CurrentQuarterEarnedCash, Is.EqualTo(ownedCard.Card.ManagementValue));
            Assert.That(run.Resources.Cash, Is.EqualTo(3 + ownedCard.Card.ManagementValue));
        }

        [Test]
        public void QuarterSettlementFailureStopsLaterScheduleProgress()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithCalendar(run, new RunCalendarState(1, 2, 1));
            run = WithPerformance(run, new RunPerformanceState(2, 2, 2, 0));
            run = WithRedemptionPressure(run, 8);

            run = BusinessDayFlow.AdvanceToNextBusinessDay(run);

            Assert.That(run.State, Is.EqualTo(RunState.Failed));
            Assert.That(run.RedemptionPressure.CurrentPressure, Is.EqualTo(10));
            Assert.That(run.FailureReason, Is.EqualTo("대규모 환매 발생"));
            Assert.That(run.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.QuarterSettlement));

            var continuedRun = BusinessDayFlow.ContinueAfterQuarterSettlement(run);

            Assert.That(continuedRun.State, Is.EqualTo(RunState.Failed));
            Assert.That(continuedRun.Calendar.FiscalYear, Is.EqualTo(1));
            Assert.That(continuedRun.Calendar.Quarter, Is.EqualTo(2));
            Assert.That(continuedRun.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.QuarterSettlement));
        }

        [Test]
        public void ContinueAfterQuarterSettlementStartsNextPlayableQuarterInSameFiscalYear()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = CompleteCurrentQuarter(run);

            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);

            Assert.That(run.State, Is.EqualTo(RunState.Playing));
            Assert.That(run.Calendar.FiscalYear, Is.EqualTo(1));
            Assert.That(run.Calendar.Quarter, Is.EqualTo(2));
            Assert.That(run.Calendar.RemainingBusinessDays, Is.EqualTo(4));
            Assert.That(run.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.AwaitingAction));
        }

        [Test]
        public void ContinueAfterQuarterSettlementAdvancesMarketTapeInSameFiscalYear()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var oldSellImminentCardIds = CollectZoneCardIds(run.MarketTape.SellImminentCards);
            var oldCurrentMarketCardIds = CollectZoneCardIds(run.MarketTape.CurrentMarketCards);
            run = CompleteCurrentQuarter(run);

            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);

            Assert.That(run.Calendar.FiscalYear, Is.EqualTo(1));
            Assert.That(run.Calendar.Quarter, Is.EqualTo(2));
            AssertZoneMatches(run.MarketTape.SellImminentCards, oldCurrentMarketCardIds);
            foreach (var cardId in oldSellImminentCardIds)
            {
                Assert.That(FindCard(run.AssetCards, cardId).State, Is.EqualTo(AssetCardRuntimeState.Removed));
            }
        }

        [Test]
        public void ContinueAfterThirdQuarterSettlementStartsVacationInFirstFiscalYear()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = CompleteCurrentQuarter(run);

            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);

            Assert.That(run.State, Is.EqualTo(RunState.Playing));
            Assert.That(run.Calendar.FiscalYear, Is.EqualTo(1));
            Assert.That(run.Calendar.Quarter, Is.EqualTo(4));
            Assert.That(run.Calendar.RemainingBusinessDays, Is.EqualTo(0));
            Assert.That(run.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.Vacation));
        }

        [Test]
        public void ContinueAfterThirdQuarterSettlementDoesNotProcessMarketTapeForVacation()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = CompleteCurrentQuarter(run);
            var tapeBeforeVacation = run.MarketTape;

            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);

            Assert.That(run.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.Vacation));
            AssertTapeMatches(run.MarketTape, tapeBeforeVacation);
        }

        [Test]
        public void ContinueAfterVacationStartsNextFiscalYearFirstQuarter()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);

            run = BusinessDayFlow.ContinueAfterVacation(run);

            Assert.That(run.State, Is.EqualTo(RunState.Playing));
            Assert.That(run.Calendar.FiscalYear, Is.EqualTo(2));
            Assert.That(run.Calendar.Quarter, Is.EqualTo(1));
            Assert.That(run.Calendar.RemainingBusinessDays, Is.EqualTo(4));
            Assert.That(run.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.AwaitingAction));
        }

        [Test]
        public void ContinueAfterVacationRefreshesMarketTapeForNextFiscalYear()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            var previousFiscalYearCardIds = CollectVisibleCardIds(run.MarketTape);

            run = BusinessDayFlow.ContinueAfterVacation(run);

            Assert.That(run.Calendar.FiscalYear, Is.EqualTo(2));
            Assert.That(run.Calendar.Quarter, Is.EqualTo(1));
            Assert.That(run.MarketTape.SellImminentCards, Has.Count.EqualTo(run.StaticData.MarketConfig.SellImminentSlots));
            Assert.That(run.MarketTape.CurrentMarketCards, Has.Count.EqualTo(run.StaticData.MarketConfig.CurrentMarketSlots));
            Assert.That(run.MarketTape.UpcomingMarketCards, Has.Count.EqualTo(run.StaticData.MarketConfig.UpcomingMarketSlots));
            Assert.That(CollectVisibleCardIds(run.MarketTape), Is.Unique);
            foreach (var cardId in CollectVisibleCardIds(run.MarketTape))
            {
                Assert.That(previousFiscalYearCardIds.Contains(cardId), Is.False);
            }
        }

        [Test]
        public void ReservedCardsPersistAcrossBusinessDayQuarterAndFiscalYearTransitions()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ReserveFirstCurrentMarketCard(run);
            var reservedCardId = run.Reservation.ReservedCards[0].Card.Id;

            run = BusinessDayFlow.AdvanceToNextBusinessDay(run);

            Assert.That(run.Reservation.ReservedCards[0].Card.Id, Is.EqualTo(reservedCardId));

            run = CompleteCurrentQuarter(run);

            Assert.That(run.Reservation.ReservedCards[0].Card.Id, Is.EqualTo(reservedCardId));

            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);

            Assert.That(run.Reservation.ReservedCards[0].Card.Id, Is.EqualTo(reservedCardId));

            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = CompleteCurrentQuarter(run);
            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
            run = BusinessDayFlow.ContinueAfterVacation(run);

            Assert.That(run.Calendar.FiscalYear, Is.EqualTo(2));
            Assert.That(run.Reservation.ReservedCards[0].Card.Id, Is.EqualTo(reservedCardId));
        }

        [Test]
        public void ContinueAfterFinalPlayableQuarterSettlementStartsFinalSettlement()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = AdvanceToPlayableQuarter(run, 3, 4);
            run = CompleteCurrentQuarter(run);

            run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);

            Assert.That(run.State, Is.EqualTo(RunState.Completed));
            Assert.That(run.Calendar.FiscalYear, Is.EqualTo(3));
            Assert.That(run.Calendar.Quarter, Is.EqualTo(4));
            Assert.That(run.Calendar.RemainingBusinessDays, Is.EqualTo(0));
            Assert.That(run.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.FinalSettlement));
        }

        [Test]
        public void AdvanceToNextBusinessDayDoesNotConsumeAfterQuarterSettlement()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = CompleteCurrentQuarter(run);

            var nextRun = BusinessDayFlow.AdvanceToNextBusinessDay(run);

            Assert.That(nextRun.Calendar.RemainingBusinessDays, Is.EqualTo(0));
            Assert.That(nextRun.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.QuarterSettlement));
        }

        private static RunSessionState CompleteCurrentQuarter(RunSessionState run)
        {
            while (run.BusinessDay.Phase != BusinessDayPhase.QuarterSettlement)
            {
                if (run.Calendar.RemainingBusinessDays == 1)
                {
                    run = AddEarnedCashToMeetQuarterTarget(run);
                }

                run = BusinessDayFlow.AdvanceToNextBusinessDay(run);
            }

            return run;
        }

        private static RunSessionState AddEarnedCashToMeetQuarterTarget(RunSessionState run)
        {
            var target = GetQuarterTarget(run);
            var missing = target - run.Performance.CurrentQuarterEarnedCash;
            return missing > 0 ? ResourceLedger.AddEarnedCash(run, missing) : run;
        }

        private static int GetQuarterTarget(RunSessionState run)
        {
            foreach (var quarter in run.StaticData.Quarters)
            {
                if (quarter.FiscalYear == run.Calendar.FiscalYear && quarter.Quarter == run.Calendar.Quarter)
                {
                    return quarter.EarnedCashGoal;
                }
            }

            return run.StaticData.Quarters[0].EarnedCashGoal;
        }

        private static RunSessionState ReserveFirstCurrentMarketCard(RunSessionState run)
        {
            var detailRun = MarketAreaFlow.OpenMarketCardDetail(run, run.MarketTape.CurrentMarketCards[0]);
            return ReservationAction.ConfirmReservation(detailRun).Run;
        }

        private static RunSessionState AdvanceToPlayableQuarter(RunSessionState run, int fiscalYear, int quarter)
        {
            for (var i = 0; i < 20; i++)
            {
                if (run.Calendar.FiscalYear == fiscalYear
                    && run.Calendar.Quarter == quarter
                    && run.BusinessDay.Phase == BusinessDayPhase.AwaitingAction)
                {
                    return run;
                }

                if (run.BusinessDay.Phase == BusinessDayPhase.Vacation)
                {
                    run = BusinessDayFlow.ContinueAfterVacation(run);
                }
                else
                {
                    run = CompleteCurrentQuarter(run);
                    run = BusinessDayFlow.ContinueAfterQuarterSettlement(run);
                }
            }

            Assert.Fail("Could not reach the requested playable quarter.");
            return run;
        }

        private static RunSessionState WithOwnedAssets(RunSessionState run, OwnedAssetState ownedAssets)
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
                ownedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail);
        }

        private static RunSessionState WithCalendar(RunSessionState run, RunCalendarState calendar)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithPerformance(RunSessionState run, RunPerformanceState performance)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
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

        private static System.Collections.Generic.HashSet<string> CollectVisibleCardIds(MarketTapeState tape)
        {
            var cardIds = new System.Collections.Generic.HashSet<string>();
            AddCardIds(cardIds, tape.SellImminentCards);
            AddCardIds(cardIds, tape.CurrentMarketCards);
            AddCardIds(cardIds, tape.UpcomingMarketCards);
            return cardIds;
        }

        private static void AssertTapeMatches(MarketTapeState actual, MarketTapeState expected)
        {
            AssertZoneMatches(actual.SellImminentCards, expected.SellImminentCards);
            AssertZoneMatches(actual.CurrentMarketCards, expected.CurrentMarketCards);
            AssertZoneMatches(actual.UpcomingMarketCards, expected.UpcomingMarketCards);
        }

        private static void AssertZoneMatches(
            System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> actual,
            System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> expected)
        {
            Assert.That(actual, Has.Count.EqualTo(expected.Count));
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.That(actual[i].Card.Id, Is.EqualTo(expected[i].Card.Id));
            }
        }

        private static void AssertZoneMatches(
            System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> actual,
            System.Collections.Generic.IReadOnlyList<string> expectedCardIds)
        {
            Assert.That(actual, Has.Count.EqualTo(expectedCardIds.Count));
            for (var i = 0; i < expectedCardIds.Count; i++)
            {
                Assert.That(actual[i].Card.Id, Is.EqualTo(expectedCardIds[i]));
            }
        }

        private static System.Collections.Generic.List<string> CollectZoneCardIds(
            System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> cards)
        {
            var cardIds = new System.Collections.Generic.List<string>();
            foreach (var card in cards)
            {
                cardIds.Add(card.Card.Id);
            }

            return cardIds;
        }

        private static void AddCardIds(
            System.Collections.Generic.HashSet<string> cardIds,
            System.Collections.Generic.IEnumerable<AssetCardRuntimeData> cards)
        {
            foreach (var card in cards)
            {
                cardIds.Add(card.Card.Id);
            }
        }
    }
}
