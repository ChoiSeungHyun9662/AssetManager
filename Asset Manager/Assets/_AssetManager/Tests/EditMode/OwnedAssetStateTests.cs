using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class OwnedAssetStateTests
    {
        [Test]
        public void OwnedAssetsCountAndManagementValueIgnoreMarketAndReservedCards()
        {
            var ownedCard = CreateRuntimeCard("owned-card", 7, 2, AssetCardRuntimeState.Owned);
            var marketCard = CreateRuntimeCard("market-card", 11, 3, AssetCardRuntimeState.Available);
            var reservedCard = CreateRuntimeCard("reserved-card", 13, 4, AssetCardRuntimeState.Reserved);

            var ownedAssets = new OwnedAssetState(new[] { ownedCard, marketCard, reservedCard });

            Assert.That(ownedAssets.Count, Is.EqualTo(1));
            Assert.That(ownedAssets.CurrentManagementValue, Is.EqualTo(7));
            Assert.That(ownedAssets.BusinessDayStartIncome, Is.EqualTo(2));
        }

        [Test]
        public void PortfolioReportsEightStockSlotsAndFullStateFromOwnedCards()
        {
            var ownedCards = new System.Collections.Generic.List<AssetCardRuntimeData>();
            for (var i = 0; i < 8; i++)
            {
                ownedCards.Add(CreateRuntimeCard("owned-card-" + i, 1, 0, AssetCardRuntimeState.Owned));
            }

            var ownedAssets = new OwnedAssetState(ownedCards);

            Assert.That(ownedAssets.MaxStockSlots, Is.EqualTo(8));
            Assert.That(ownedAssets.OpenStockSlots, Is.EqualTo(0));
            Assert.That(ownedAssets.IsPortfolioFull, Is.True);
        }

        private static AssetCardRuntimeData CreateRuntimeCard(
            string cardId,
            int managementValue,
            int income,
            AssetCardRuntimeState state)
        {
            return new AssetCardRuntimeData(
                new AssetCardData(
                    cardId,
                    cardId,
                    cardId,
                    AssetRarity.Common,
                    1,
                    new ProfessionalResourceCost[0],
                    managementValue,
                    income,
                    new TagData[0]),
                state,
                null);
        }
    }
}
