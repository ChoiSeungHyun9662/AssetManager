using System.Collections.Generic;
using UnityEngine;

namespace AssetManager
{
    [CreateAssetMenu(menuName = "Asset Manager/Run Static Data Set")]
    public sealed class RunStaticDataSet : ScriptableObject
    {
        [SerializeField]
        private List<AssetCardData> assetCards = new List<AssetCardData>();

        [SerializeField]
        private List<QuarterData> quarters = new List<QuarterData>();

        [SerializeField]
        private List<FinalRatingData> finalRatings = new List<FinalRatingData>();

        [SerializeField]
        private List<RedemptionPressureLevelData> redemptionPressureLevels = new List<RedemptionPressureLevelData>();

        [SerializeField]
        private List<FinalManagementCommentData> finalManagementComments = new List<FinalManagementCommentData>();

        [SerializeField]
        private MarketConfigData marketConfig = new MarketConfigData();

        [SerializeField]
        private ResourceConfigData resourceConfig = new ResourceConfigData();

        [SerializeField]
        private RedemptionPressureConfigData redemptionPressureConfig = new RedemptionPressureConfigData();

        public IReadOnlyList<AssetCardData> AssetCards => assetCards;
        public IReadOnlyList<QuarterData> Quarters => quarters;
        public IReadOnlyList<FinalRatingData> FinalRatings => finalRatings;
        public IReadOnlyList<RedemptionPressureLevelData> RedemptionPressureLevels => redemptionPressureLevels;
        public IReadOnlyList<FinalManagementCommentData> FinalManagementComments => finalManagementComments;
        public MarketConfigData MarketConfig => marketConfig;
        public ResourceConfigData ResourceConfig => resourceConfig;
        public RedemptionPressureConfigData RedemptionPressureConfig => redemptionPressureConfig;

        public int GetInflationCostModifier(int fiscalYear, int quarter)
        {
            foreach (var quarterData in quarters)
            {
                if (quarterData.FiscalYear == fiscalYear && quarterData.Quarter == quarter)
                {
                    return quarterData.InflationCostModifier;
                }
            }

            return 0;
        }

        public bool HasRequiredMvpData =>
            assetCards.Count > 0
            && quarters.Count > 0
            && finalRatings.Count > 0
            && resourceConfig != null
            && redemptionPressureConfig != null;

        public static RunStaticDataSet CreateMvpDefaults()
        {
            var dataSet = CreateInstance<RunStaticDataSet>();
            dataSet.ResetToMvpDefaults();
            return dataSet;
        }

        public void ResetToMvpDefaults()
        {
            var softwareTag = new TagData("software", "Software", TagType.Sector);
            var consumerTag = new TagData("consumer", "Consumer", TagType.Sector);
            var energyTag = new TagData("energy", "Energy", TagType.Sector);

            assetCards = new List<AssetCardData>
            {
                CreateStock("starter-index-fund", "Index Fund", softwareTag, 2, ResourceType.Reading, null, 3, 1, 5, 2, 2, 4),
                CreateStock("starter-chip-maker", "Chip Maker", softwareTag, 2, ResourceType.Meditation, null, 2, 1, 4, 2, 2, 4),
                CreateStock("starter-coffee-chain", "Coffee Chain", consumerTag, 1, ResourceType.Reading, null, 2, 0, 4, 1, 2, 5),
                CreateStock("starter-cloud-platform", "Cloud Platform", softwareTag, 3, ResourceType.Reading, ResourceType.Meditation, 4, 1, 7, 2, 1, 3),
                CreateStock("starter-battery-maker", "Battery Maker", energyTag, 1, ResourceType.Patience, null, 1, 1, 3, 2, 2, 4),
                CreateStock("starter-payment-network", "Payment Network", consumerTag, 2, ResourceType.Meditation, null, 2, 0, 4, 1, 2, 4),
                CreateStock("starter-cybersecurity", "Cybersecurity", softwareTag, 3, ResourceType.Reading, null, 3, 1, 5, 2, 2, 3),
                CreateStock("starter-discount-store", "Discount Store", consumerTag, 1, ResourceType.Meditation, null, 1, 0, 3, 1, 3, 5),
                CreateStock("starter-solar-grid", "Solar Grid", energyTag, 2, ResourceType.Patience, null, 3, 0, 5, 1, 2, 4),
                CreateStock("starter-semiconductor-etf", "Semiconductor ETF", softwareTag, 3, ResourceType.Reading, ResourceType.Patience, 4, 1, 7, 2, 1, 3),
                CreateStock("starter-rail-operator", "Rail Operator", energyTag, 1, ResourceType.Reading, null, 1, 1, 3, 2, 2, 4),
                CreateStock("starter-healthcare-platform", "Healthcare Platform", consumerTag, 2, ResourceType.Meditation, null, 3, 1, 5, 2, 2, 4),
                CreateStock("starter-game-studio", "Game Studio", softwareTag, 2, ResourceType.Reading, null, 2, 1, 4, 2, 2, 4),
                CreateStock("starter-cold-chain", "Cold Chain", consumerTag, 3, ResourceType.Patience, null, 4, 1, 6, 2, 1, 3),
                CreateStock("starter-water-utility", "Water Utility", energyTag, 1, ResourceType.Meditation, null, 2, 0, 4, 1, 2, 5),
                CreateStock("starter-automation-robotics", "Automation Robotics", softwareTag, 2, ResourceType.Meditation, null, 2, 1, 4, 2, 2, 4),
                CreateStock("starter-wind-turbine", "Wind Turbine", energyTag, 3, ResourceType.Reading, ResourceType.Patience, 4, 1, 7, 2, 1, 3),
                CreateStock("starter-grocery-app", "Grocery App", consumerTag, 2, ResourceType.Reading, null, 3, 0, 5, 1, 2, 4),
                CreateStock("starter-memory-maker", "Memory Maker", softwareTag, 1, ResourceType.Patience, null, 2, 1, 4, 2, 2, 4),
                CreateStock("starter-luxury-brand", "Luxury Brand", consumerTag, 3, ResourceType.Reading, ResourceType.Meditation, 4, 1, 7, 2, 1, 3),
                CreateStock("starter-data-broker", "Data Broker", softwareTag, 2, ResourceType.Meditation, null, 3, 1, 5, 2, 2, 4),
                CreateStock("starter-grid-storage", "Grid Storage", energyTag, 2, ResourceType.Patience, null, 3, 1, 5, 2, 2, 4),
                CreateStock("starter-streaming-service", "Streaming Service", consumerTag, 1, ResourceType.Reading, null, 2, 0, 4, 1, 2, 5),
                CreateStock("starter-ai-tools", "AI Tools", softwareTag, 2, ResourceType.Meditation, null, 3, 1, 6, 2, 1, 3, true),
                CreateConsumableResource("resource-cash-small", ResourceType.Cash, 2, 1, AssetRarity.Common),
                CreateConsumableResource("resource-cash-large", ResourceType.Cash, 3, 2, AssetRarity.Uncommon),
                CreateConsumableResource("resource-reading", ResourceType.Reading, 1, 1, AssetRarity.Common),
                CreateConsumableResource("resource-reading-plus", ResourceType.Reading, 2, 2, AssetRarity.Uncommon),
                CreateConsumableResource("resource-meditation", ResourceType.Meditation, 1, 1, AssetRarity.Common),
                CreateConsumableResource("resource-meditation-plus", ResourceType.Meditation, 2, 2, AssetRarity.Uncommon),
                CreateConsumableResource("resource-patience", ResourceType.Patience, 1, 1, AssetRarity.Common),
                CreateConsumableResource("resource-patience-plus", ResourceType.Patience, 2, 2, AssetRarity.Uncommon),
                CreateConsumableResource("resource-balanced-study", ResourceType.Reading, 1, 0, AssetRarity.Rare)
            };

            quarters = new List<QuarterData>
            {
                new QuarterData(1, 1, 4, 3),
                new QuarterData(1, 2, 4, 4, 1),
                new QuarterData(1, 3, 4, 4),
                new QuarterData(2, 1, 4, 4),
                new QuarterData(2, 2, 4, 4),
                new QuarterData(2, 3, 4, 4),
                new QuarterData(3, 1, 5, 5),
                new QuarterData(3, 2, 5, 5),
                new QuarterData(3, 3, 5, 5),
                new QuarterData(3, 4, 5, 5)
            };

            finalRatings = new List<FinalRatingData>
            {
                new FinalRatingData("seed", "Seed", 0),
                new FinalRatingData("core", "Core", 5),
                new FinalRatingData("flagship", "Flagship", 10)
            };

            redemptionPressureLevels = new List<RedemptionPressureLevelData>
            {
                new RedemptionPressureLevelData("stable", "Stable", 0, 3),
                new RedemptionPressureLevelData("watch", "Watch", 4, 6),
                new RedemptionPressureLevelData("critical", "Critical", 7, 9),
                new RedemptionPressureLevelData("failed", "Failed", 10, 10)
            };

            finalManagementComments = new List<FinalManagementCommentData>
            {
                new FinalManagementCommentData("seed-stable", "seed", "stable", "Small but solvent."),
                new FinalManagementCommentData("seed-watch", "seed", "watch", "Small portfolio, rising pressure."),
                new FinalManagementCommentData("seed-critical", "seed", "critical", "Barely balanced the run."),
                new FinalManagementCommentData("core-stable", "core", "stable", "A steady portfolio held together."),
                new FinalManagementCommentData("core-watch", "core", "watch", "Good value with some pressure."),
                new FinalManagementCommentData("core-critical", "core", "critical", "성과는 충분하지만 환매 압력이 높습니다."),
                new FinalManagementCommentData("flagship-stable", "flagship", "stable", "A flagship portfolio."),
                new FinalManagementCommentData("flagship-watch", "flagship", "watch", "Strong value under pressure."),
                new FinalManagementCommentData("flagship-critical", "flagship", "critical", "Impressive and risky.")
            };

            marketConfig = new MarketConfigData(8, 0.75);
            resourceConfig = new ResourceConfigData(3, 10, 5, 3);
            redemptionPressureConfig = new RedemptionPressureConfigData(0, 10);
        }

        private static AssetCardData CreateStock(
            string id,
            string displayName,
            TagData tag,
            int cashCost,
            ResourceType firstCost,
            ResourceType? secondCost,
            int baseValue,
            int baseDividend,
            int foilValue,
            int foilDividend,
            int minDeckCopies,
            int maxDeckCopies,
            bool grantsExtraBuyAction = false)
        {
            var costs = new List<ProfessionalResourceCost>
            {
                new ProfessionalResourceCost(firstCost, 1)
            };

            if (secondCost.HasValue)
            {
                costs.Add(new ProfessionalResourceCost(secondCost.Value, 1));
            }

            return new AssetCardData(
                id,
                displayName,
                "Stock card for the starter portfolio.",
                AssetRarity.Common,
                cashCost,
                costs,
                baseValue,
                baseDividend,
                new[] { tag },
                grantsExtraBuyAction,
                foilValue,
                foilDividend,
                minDeckCopies,
                maxDeckCopies,
                CardDomain.Stock);
        }

        private static AssetCardData CreateConsumableResource(
            string id,
            ResourceType providedResourceType,
            int providedResourceAmount,
            int cashCost,
            AssetRarity rarity)
        {
            return new AssetCardData(
                id,
                string.Empty,
                "Consumable market resource card.",
                rarity,
                cashCost,
                new ProfessionalResourceCost[0],
                0,
                0,
                new TagData[0],
                cardDomain: CardDomain.ConsumableResource,
                providedResourceType: providedResourceType,
                providedResourceAmount: providedResourceAmount);
        }
    }
}
