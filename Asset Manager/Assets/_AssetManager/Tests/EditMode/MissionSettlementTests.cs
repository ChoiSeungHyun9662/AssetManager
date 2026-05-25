using System.Collections.Generic;
using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class MissionSettlementTests
    {
        [Test]
        public void FastEntryAndConcentrationSettleFromOwnedTargetTagCardCounts()
        {
            var tag = StockTagCatalog.Technology;
            var mission = CreateMission("count", MissionTemplate.Concentration, new[] { tag }, rewardPerCard: 2);
            var ownedAssets = new OwnedAssetState(new[]
            {
                CreateRuntimeStock("target-1", tag, value: 3),
                CreateRuntimeStock("target-2", tag, value: 4),
                CreateRuntimeStock("other", StockTagCatalog.Consumer, value: 9)
            });

            var revenue = MissionSettlement.Calculate(mission, ownedAssets);

            Assert.That(revenue, Is.EqualTo(4));
        }

        [Test]
        public void FoilSettlementUsesTargetTagCardCountAndTargetTagFoilCount()
        {
            var tag = StockTagCatalog.Consumer;
            var mission = CreateMission("foil", MissionTemplate.Foil, new[] { tag }, rewardPerCard: 2);
            var ownedAssets = new OwnedAssetState(new[]
            {
                CreateRuntimeStock("target-normal", tag, value: 3),
                CreateRuntimeStock("target-foil", tag, value: 5, foil: true),
                CreateRuntimeStock("other-foil", StockTagCatalog.Energy, value: 5, foil: true)
            });

            var revenue = MissionSettlement.Calculate(mission, ownedAssets);

            Assert.That(revenue, Is.EqualTo(6));
        }

        [Test]
        public void HighValueSettlementUsesTargetTagTotalValue()
        {
            var tag = StockTagCatalog.Energy;
            var mission = CreateMission("value", MissionTemplate.HighValue, new[] { tag }, rewardPerCard: 0);
            var ownedAssets = new OwnedAssetState(new[]
            {
                CreateRuntimeStock("target-1", tag, value: 4),
                CreateRuntimeStock("target-2", tag, value: 6),
                CreateRuntimeStock("other", StockTagCatalog.Industrials, value: 10)
            });

            var revenue = MissionSettlement.Calculate(mission, ownedAssets);

            Assert.That(revenue, Is.EqualTo(10));
        }

        [Test]
        public void SettlementCanTargetAllOwnedCardsWhenNoTargetTagIsConfigured()
        {
            var mission = CreateMission("all", MissionTemplate.FastEntry, new TagData[0], rewardPerCard: 3);
            var ownedAssets = new OwnedAssetState(new[]
            {
                CreateRuntimeStock("technology", StockTagCatalog.Technology, value: 4),
                CreateRuntimeStock("financials", StockTagCatalog.Financials, value: 7)
            });

            var revenue = MissionSettlement.Calculate(mission, ownedAssets);

            Assert.That(revenue, Is.EqualTo(6));
        }

        [Test]
        public void SettlementReturnsZeroWhenCurrentPortfolioCreatesNoMissionRevenue()
        {
            var mission = CreateMission("empty", MissionTemplate.FastEntry, new[] { StockTagCatalog.Technology }, rewardPerCard: 1);
            var ownedAssets = new OwnedAssetState(new[]
            {
                CreateRuntimeStock("consumer", StockTagCatalog.Consumer, value: 4)
            });

            var revenue = MissionSettlement.Calculate(mission, ownedAssets);

            Assert.That(revenue, Is.EqualTo(0));
        }

        private static MissionDefinitionData CreateMission(
            string id,
            MissionTemplate template,
            IEnumerable<TagData> tags,
            int rewardPerCard)
        {
            return new MissionDefinitionData(
                id,
                id,
                template,
                tags,
                "clear",
                "formula",
                "Test",
                1,
                rewardPerCard);
        }

        private static AssetCardRuntimeData CreateRuntimeStock(
            string id,
            TagData tag,
            int value,
            bool foil = false)
        {
            var card = new AssetCardData(
                id,
                id,
                "test stock",
                AssetRarity.Common,
                0,
                new ProfessionalResourceCost[0],
                value,
                0,
                new[] { tag },
                foilValue: value);
            return new AssetCardRuntimeData(card, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, null, foil, id + "#runtime");
        }
    }
}
