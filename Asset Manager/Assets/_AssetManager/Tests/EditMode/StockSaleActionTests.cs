using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class StockSaleActionTests
    {
        [Test]
        public void SellingOwnedNormalStockPaysInflationAdjustedCashRecordsRevenueAndLeavesBusinessDayOpen()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithCalendar(run, new RunCalendarState(1, 2, run.Calendar.RemainingBusinessDays));
            var ownedCard = WithState(run.AssetCards[0], AssetCardRuntimeState.Owned, false);
            run = WithOwnedAssets(run, new OwnedAssetState(new[] { ownedCard }));
            run = WithAssetCards(run, ReplaceRuntimeCard(run, ownedCard));

            var result = StockSaleAction.ConfirmSale(run, 0);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(run.Resources.Cash + 2));
            Assert.That(result.Run.Performance.CurrentQuarterEarnedCash, Is.EqualTo(2));
            Assert.That(result.Run.Performance.CurrentFiscalYearEarnedCash, Is.EqualTo(2));
            Assert.That(result.Run.Performance.TotalEarnedCash, Is.EqualTo(2));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(run.Calendar.RemainingBusinessDays));
            Assert.That(result.Run.BusinessDay.Phase, Is.EqualTo(BusinessDayPhase.AwaitingAction));
            Assert.That(result.Run.OwnedAssets.StockSlots[0], Is.Null);
            Assert.That(FindRuntimeCard(result.Run, ownedCard.RuntimeId).State, Is.EqualTo(AssetCardRuntimeState.Removed));
        }

        [Test]
        public void SellingOwnedFoilStockPaysLargerInflationAdjustedCashAndCanBeRepeatedSameDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithCalendar(run, new RunCalendarState(1, 2, run.Calendar.RemainingBusinessDays));
            var firstOwned = WithState(run.AssetCards[0], AssetCardRuntimeState.Owned, true);
            var secondOwned = WithState(run.AssetCards[1], AssetCardRuntimeState.Owned, false);
            run = WithOwnedAssets(run, new OwnedAssetState(new[] { firstOwned, secondOwned }));
            run = WithAssetCards(run, ReplaceRuntimeCard(run, firstOwned, secondOwned));

            var firstSale = StockSaleAction.ConfirmSale(run, 0);
            var secondSale = StockSaleAction.ConfirmSale(firstSale.Run, 1);

            Assert.That(firstSale.Succeeded, Is.True);
            Assert.That(secondSale.Succeeded, Is.True);
            Assert.That(secondSale.Run.Resources.Cash, Is.EqualTo(run.Resources.Cash + 4 + 2));
            Assert.That(secondSale.Run.Performance.CurrentQuarterEarnedCash, Is.EqualTo(6));
            Assert.That(secondSale.Run.Calendar.RemainingBusinessDays, Is.EqualTo(run.Calendar.RemainingBusinessDays));
            Assert.That(secondSale.Run.OwnedAssets.Count, Is.EqualTo(0));
        }

        [Test]
        public void SellingOwnedStockUsesOriginalPortfolioSlotIndex()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var firstOwned = WithState(run.AssetCards[0], AssetCardRuntimeState.Owned, false);
            var secondOwned = WithState(run.AssetCards[1], AssetCardRuntimeState.Owned, false);
            run = WithOwnedAssets(run, new OwnedAssetState(new[] { firstOwned, null, secondOwned }));
            run = WithAssetCards(run, ReplaceRuntimeCard(run, firstOwned, secondOwned));

            var result = StockSaleAction.ConfirmSale(run, 2);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.OwnedAssets.StockSlots[0], Is.EqualTo(firstOwned));
            Assert.That(result.Run.OwnedAssets.StockSlots[1], Is.Null);
            Assert.That(result.Run.OwnedAssets.StockSlots[2], Is.Null);
            Assert.That(FindRuntimeCard(result.Run, secondOwned.RuntimeId).State, Is.EqualTo(AssetCardRuntimeState.Removed));
            Assert.That(FindRuntimeCard(result.Run, firstOwned.RuntimeId).State, Is.EqualTo(AssetCardRuntimeState.Owned));
        }

        private static AssetCardRuntimeData WithState(
            AssetCardRuntimeData card,
            AssetCardRuntimeState state,
            bool isFoil)
        {
            return new AssetCardRuntimeData(
                card.Card,
                state,
                PurchaseSource.MarketTape,
                1,
                isFoil,
                card.RuntimeId);
        }

        private static AssetCardRuntimeData[] ReplaceRuntimeCard(
            RunSessionState run,
            params AssetCardRuntimeData[] replacements)
        {
            var cards = new AssetCardRuntimeData[run.AssetCards.Count];
            for (var i = 0; i < run.AssetCards.Count; i++)
            {
                cards[i] = run.AssetCards[i];
                foreach (var replacement in replacements)
                {
                    if (run.AssetCards[i].RuntimeId == replacement.RuntimeId)
                    {
                        cards[i] = replacement;
                    }
                }
            }

            return cards;
        }

        private static AssetCardRuntimeData FindRuntimeCard(RunSessionState run, string runtimeId)
        {
            foreach (var card in run.AssetCards)
            {
                if (card.RuntimeId == runtimeId)
                {
                    return card;
                }
            }

            return null;
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
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithAssetCards(
            RunSessionState run,
            AssetCardRuntimeData[] assetCards)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                assetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }
    }
}
