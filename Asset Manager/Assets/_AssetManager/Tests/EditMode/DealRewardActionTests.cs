using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class DealRewardActionTests
    {
        [Test]
        public void StockSlotThresholdRewardsGrantDealOncePerRun()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithOwnedStocks(run, 2);

            var firstThree = DealRewardAction.ApplyPurchaseRewards(WithOwnedStocks(run, 3), false);
            var backToTwo = WithOwnedStocks(firstThree.Run, 2);
            var secondThree = DealRewardAction.ApplyPurchaseRewards(WithOwnedStocks(backToTwo, 3), false);
            var firstFive = DealRewardAction.ApplyPurchaseRewards(WithOwnedStocks(secondThree.Run, 5), false);
            var firstEight = DealRewardAction.ApplyPurchaseRewards(WithOwnedStocks(firstFive.Run, 8), false);

            Assert.That(firstThree.DealsGranted, Is.EqualTo(1));
            Assert.That(firstThree.Run.Resources.Deal, Is.EqualTo(1));
            Assert.That(secondThree.DealsGranted, Is.EqualTo(0));
            Assert.That(secondThree.Run.Resources.Deal, Is.EqualTo(1));
            Assert.That(firstFive.DealsGranted, Is.EqualTo(1));
            Assert.That(firstFive.Run.Resources.Deal, Is.EqualTo(2));
            Assert.That(firstEight.DealsGranted, Is.EqualTo(1));
            Assert.That(firstEight.Run.Resources.Deal, Is.EqualTo(3));
        }

        [Test]
        public void OccupiedStockSlotRewardsCountDuplicateStockIdsAsSeparateSlots()
        {
            var stock = new AssetCardData(
                "same-stock",
                "Same Stock",
                "Duplicate slot reward stock.",
                AssetRarity.Common,
                0,
                new ProfessionalResourceCost[0],
                1,
                0,
                new TagData[0]);
            var run = WithOwnedAssets(
                RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults()),
                new[]
                {
                    new AssetCardRuntimeData(stock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 1, false, "same-stock#a"),
                    new AssetCardRuntimeData(stock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 2, false, "same-stock#b"),
                    new AssetCardRuntimeData(stock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 3, false, "same-stock#c")
                });

            var result = DealRewardAction.ApplyPurchaseRewards(run, false);

            Assert.That(result.DealsGranted, Is.EqualTo(1));
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(1));
        }

        [Test]
        public void FirstFoilRewardGrantsOnceAndStacksWithSlotThresholdReward()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithOwnedStocks(run, 2);

            var simultaneous = DealRewardAction.ApplyPurchaseRewards(WithOwnedStocks(run, 3), true);
            var secondFoil = DealRewardAction.ApplyPurchaseRewards(simultaneous.Run, true);

            Assert.That(simultaneous.DealsGranted, Is.EqualTo(2));
            Assert.That(simultaneous.Run.Resources.Deal, Is.EqualTo(2));
            Assert.That(secondFoil.DealsGranted, Is.EqualTo(0));
            Assert.That(secondFoil.Run.Resources.Deal, Is.EqualTo(2));
        }

        private static RunSessionState WithOwnedStocks(RunSessionState run, int count)
        {
            var ownedCards = new System.Collections.Generic.List<AssetCardRuntimeData>();
            for (var i = 0; i < count; i++)
            {
                var card = new AssetCardData(
                    "owned-reward-stock-" + i,
                    "Owned Reward Stock " + i,
                    "Owned stock for Deal reward tests.",
                    AssetRarity.Common,
                    0,
                    new ProfessionalResourceCost[0],
                    1,
                    0,
                    new TagData[0]);
                ownedCards.Add(new AssetCardRuntimeData(card, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, i + 1));
            }

            return WithOwnedAssets(run, ownedCards);
        }

        private static RunSessionState WithOwnedAssets(
            RunSessionState run,
            System.Collections.Generic.IEnumerable<AssetCardRuntimeData> ownedAssets)
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
                new OwnedAssetState(ownedAssets),
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards);
        }
    }
}
