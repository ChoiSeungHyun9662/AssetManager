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
        private List<MissionDefinitionData> missions = new List<MissionDefinitionData>();

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
        public IReadOnlyList<MissionDefinitionData> Missions => missions.Count > 0
            ? missions
            : CreateDefaultMissions(
                StockTagCatalog.Technology,
                StockTagCatalog.Consumer,
                StockTagCatalog.Energy,
                StockTagCatalog.Financials,
                StockTagCatalog.Industrials);
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
            && Missions.Count > 0
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
            var technologyTag = StockTagCatalog.Technology;
            var consumerTag = StockTagCatalog.Consumer;
            var energyTag = StockTagCatalog.Energy;
            var financialsTag = StockTagCatalog.Financials;
            var industrialsTag = StockTagCatalog.Industrials;

            assetCards = new List<AssetCardData>
            {
                CreateStock("starter-index-fund", "Index Fund", financialsTag, 2, ResourceType.Reading, null, 3, 1, 5, 2, 2, 4),
                CreateStock("starter-chip-maker", "Chip Maker", technologyTag, 2, ResourceType.Meditation, null, 2, 1, 4, 2, 2, 4),
                CreateStock("starter-coffee-chain", "Coffee Chain", consumerTag, 1, ResourceType.Reading, null, 2, 0, 4, 1, 2, 5),
                CreateStock("starter-cloud-platform", "Cloud Platform", technologyTag, 3, ResourceType.Reading, ResourceType.Meditation, 4, 1, 7, 2, 1, 3),
                CreateStock("starter-battery-maker", "Battery Maker", energyTag, 1, ResourceType.Patience, null, 1, 1, 3, 2, 2, 4),
                CreateStock("starter-payment-network", "Payment Network", financialsTag, 2, ResourceType.Meditation, null, 2, 0, 4, 1, 2, 4),
                CreateStock("starter-cybersecurity", "Cybersecurity", technologyTag, 3, ResourceType.Reading, null, 3, 1, 5, 2, 2, 3),
                CreateStock("starter-discount-store", "Discount Store", consumerTag, 1, ResourceType.Meditation, null, 1, 0, 3, 1, 3, 5),
                CreateStock("starter-solar-grid", "Solar Grid", energyTag, 2, ResourceType.Patience, null, 3, 0, 5, 1, 2, 4),
                CreateStock("starter-semiconductor-etf", "Semiconductor ETF", technologyTag, 3, ResourceType.Reading, ResourceType.Patience, 4, 1, 7, 2, 1, 3),
                CreateStock("starter-rail-operator", "Rail Operator", industrialsTag, 1, ResourceType.Reading, null, 1, 1, 3, 2, 2, 4),
                CreateStock("starter-healthcare-platform", "Healthcare Platform", consumerTag, 2, ResourceType.Meditation, null, 3, 1, 5, 2, 2, 4),
                CreateStock("starter-game-studio", "Game Studio", technologyTag, 2, ResourceType.Reading, null, 2, 1, 4, 2, 2, 4),
                CreateStock("starter-cold-chain", "Cold Chain", industrialsTag, 3, ResourceType.Patience, null, 4, 1, 6, 2, 1, 3),
                CreateStock("starter-water-utility", "Water Utility", energyTag, 1, ResourceType.Meditation, null, 2, 0, 4, 1, 2, 5),
                CreateStock("starter-automation-robotics", "Automation Robotics", industrialsTag, 2, ResourceType.Meditation, null, 2, 1, 4, 2, 2, 4),
                CreateStock("starter-wind-turbine", "Wind Turbine", energyTag, 3, ResourceType.Reading, ResourceType.Patience, 4, 1, 7, 2, 1, 3),
                CreateStock("starter-grocery-app", "Grocery App", consumerTag, 2, ResourceType.Reading, null, 3, 0, 5, 1, 2, 4),
                CreateStock("starter-memory-maker", "Memory Maker", technologyTag, 1, ResourceType.Patience, null, 2, 1, 4, 2, 2, 4),
                CreateStock("starter-luxury-brand", "Luxury Brand", consumerTag, 3, ResourceType.Reading, ResourceType.Meditation, 4, 1, 7, 2, 1, 3),
                CreateStock("starter-data-broker", "Data Broker", financialsTag, 2, ResourceType.Meditation, null, 3, 1, 5, 2, 2, 4),
                CreateStock("starter-grid-storage", "Grid Storage", energyTag, 2, ResourceType.Patience, null, 3, 1, 5, 2, 2, 4),
                CreateStock("starter-streaming-service", "Streaming Service", consumerTag, 1, ResourceType.Reading, null, 2, 0, 4, 1, 2, 5),
                CreateStock("starter-ai-tools", "AI Tools", technologyTag, 2, ResourceType.Meditation, null, 3, 1, 6, 2, 1, 3, true),
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

            missions = CreateDefaultMissions(
                technologyTag,
                consumerTag,
                energyTag,
                financialsTag,
                industrialsTag);

            quarters = new List<QuarterData>
            {
                new QuarterData(1, 1, 8, 3),
                new QuarterData(1, 2, 8, 4, 1),
                new QuarterData(1, 3, 8, 4),
                new QuarterData(2, 1, 8, 4),
                new QuarterData(2, 2, 8, 4),
                new QuarterData(2, 3, 8, 4),
                new QuarterData(3, 1, 10, 5),
                new QuarterData(3, 2, 10, 5),
                new QuarterData(3, 3, 10, 5),
                new QuarterData(3, 4, 10, 5)
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
                new FinalManagementCommentData("seed-watch", "seed", "watch", "Small portfolio, rent piling up."),
                new FinalManagementCommentData("seed-critical", "seed", "critical", "Barely balanced the run."),
                new FinalManagementCommentData("core-stable", "core", "stable", "A steady portfolio held together."),
                new FinalManagementCommentData("core-watch", "core", "watch", "Good value with rent piling up."),
                new FinalManagementCommentData("core-critical", "core", "critical", "성과는 충분하지만 월세 밀림이 높습니다."),
                new FinalManagementCommentData("flagship-stable", "flagship", "stable", "A flagship portfolio."),
                new FinalManagementCommentData("flagship-watch", "flagship", "watch", "Strong value despite late rent."),
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

        private static List<MissionDefinitionData> CreateDefaultMissions(
            TagData technologyTag,
            TagData consumerTag,
            TagData energyTag,
            TagData financialsTag,
            TagData industrialsTag)
        {
            return new List<MissionDefinitionData>
            {
                CreateMission("fast-technology", "Compounding Software Thesis", MissionTemplate.FastEntry, new[] { technologyTag }, "Hold 3 or more Technology stocks.", "Technology stock count x +1 mission revenue.", "Easy", 3, 1),
                CreateMission("concentration-technology", "Semiconductor Conviction", MissionTemplate.Concentration, new[] { technologyTag }, "Hold 5 or more Technology stocks.", "Technology stock count x +2 mission revenue.", "Medium", 5, 2),
                CreateMission("foil-technology", "Platform Roll-Up", MissionTemplate.Foil, new[] { technologyTag }, "Hold at least one Technology foil stock.", "Technology stock count x +2, plus foil premium.", "Hard", 1, 2),
                CreateMission("high-value-technology", "Winner-Takes-Most Growth", MissionTemplate.HighValue, new[] { technologyTag }, "Hold a Technology stock with final value 7 or higher.", "Technology final value contributes mission revenue.", "Hard", 7, 0),
                CreateMission("stable-technology-consumer", "Digital Staples Barbell", MissionTemplate.TwoTagStable, new[] { technologyTag, consumerTag }, "Hold 2 or more Technology and 2 or more Consumer stocks.", "Technology and Consumer stock count x +1 mission revenue.", "Medium", 2, 1),

                CreateMission("fast-consumer", "Everyday Demand Thesis", MissionTemplate.FastEntry, new[] { consumerTag }, "Hold 3 or more Consumer stocks.", "Consumer stock count x +1 mission revenue.", "Easy", 3, 1),
                CreateMission("concentration-consumer", "Brand Moat Concentration", MissionTemplate.Concentration, new[] { consumerTag }, "Hold 5 or more Consumer stocks.", "Consumer stock count x +2 mission revenue.", "Medium", 5, 2),
                CreateMission("foil-consumer", "Category Captain Roll-Up", MissionTemplate.Foil, new[] { consumerTag }, "Hold at least one Consumer foil stock.", "Consumer stock count x +2, plus foil premium.", "Hard", 1, 2),
                CreateMission("high-value-consumer", "Premiumization Curve", MissionTemplate.HighValue, new[] { consumerTag }, "Hold a Consumer stock with final value 7 or higher.", "Consumer final value contributes mission revenue.", "Hard", 7, 0),
                CreateMission("stable-consumer-financials", "Cash Register And Rails", MissionTemplate.TwoTagStable, new[] { consumerTag, financialsTag }, "Hold 2 or more Consumer and 2 or more Financials stocks.", "Consumer and Financials stock count x +1 mission revenue.", "Medium", 2, 1),

                CreateMission("fast-energy", "Infrastructure Yield Thesis", MissionTemplate.FastEntry, new[] { energyTag }, "Hold 3 or more Energy stocks.", "Energy stock count x +1 mission revenue.", "Easy", 3, 1),
                CreateMission("concentration-energy", "Grid Scale Conviction", MissionTemplate.Concentration, new[] { energyTag }, "Hold 5 or more Energy stocks.", "Energy stock count x +2 mission revenue.", "Medium", 5, 2),
                CreateMission("foil-energy", "Energy Platform Roll-Up", MissionTemplate.Foil, new[] { energyTag }, "Hold at least one Energy foil stock.", "Energy stock count x +2, plus foil premium.", "Hard", 1, 2),
                CreateMission("high-value-energy", "Scarce Capacity Premium", MissionTemplate.HighValue, new[] { energyTag }, "Hold an Energy stock with final value 7 or higher.", "Energy final value contributes mission revenue.", "Hard", 7, 0),
                CreateMission("stable-energy-industrials", "Picks And Power", MissionTemplate.TwoTagStable, new[] { energyTag, industrialsTag }, "Hold 2 or more Energy and 2 or more Industrials stocks.", "Energy and Industrials stock count x +1 mission revenue.", "Medium", 2, 1),

                CreateMission("fast-financials", "Payment Rails Thesis", MissionTemplate.FastEntry, new[] { financialsTag }, "Hold 3 or more Financials stocks.", "Financials stock count x +1 mission revenue.", "Easy", 3, 1),
                CreateMission("concentration-financials", "Fee Stream Conviction", MissionTemplate.Concentration, new[] { financialsTag }, "Hold 5 or more Financials stocks.", "Financials stock count x +2 mission revenue.", "Medium", 5, 2),
                CreateMission("foil-financials", "Network Effects Roll-Up", MissionTemplate.Foil, new[] { financialsTag }, "Hold at least one Financials foil stock.", "Financials stock count x +2, plus foil premium.", "Hard", 1, 2),
                CreateMission("high-value-financials", "Toll Road Repricing", MissionTemplate.HighValue, new[] { financialsTag }, "Hold a Financials stock with final value 7 or higher.", "Financials final value contributes mission revenue.", "Hard", 7, 0),
                CreateMission("stable-financials-industrials", "Rails And Factories", MissionTemplate.TwoTagStable, new[] { financialsTag, industrialsTag }, "Hold 2 or more Financials and 2 or more Industrials stocks.", "Financials and Industrials stock count x +1 mission revenue.", "Medium", 2, 1),

                CreateMission("fast-industrials", "Operating Leverage Thesis", MissionTemplate.FastEntry, new[] { industrialsTag }, "Hold 3 or more Industrials stocks.", "Industrials stock count x +1 mission revenue.", "Easy", 3, 1),
                CreateMission("concentration-industrials", "Automation Conviction", MissionTemplate.Concentration, new[] { industrialsTag }, "Hold 5 or more Industrials stocks.", "Industrials stock count x +2 mission revenue.", "Medium", 5, 2),
                CreateMission("foil-industrials", "Supply Chain Roll-Up", MissionTemplate.Foil, new[] { industrialsTag }, "Hold at least one Industrials foil stock.", "Industrials stock count x +2, plus foil premium.", "Hard", 1, 2),
                CreateMission("high-value-industrials", "Replacement Cost Premium", MissionTemplate.HighValue, new[] { industrialsTag }, "Hold an Industrials stock with final value 7 or higher.", "Industrials final value contributes mission revenue.", "Hard", 7, 0),
                CreateMission("stable-technology-financials", "Data And Distribution", MissionTemplate.TwoTagStable, new[] { technologyTag, financialsTag }, "Hold 2 or more Technology and 2 or more Financials stocks.", "Technology and Financials stock count x +1 mission revenue.", "Medium", 2, 1)
            };
        }

        private static MissionDefinitionData CreateMission(
            string id,
            string displayName,
            MissionTemplate template,
            IEnumerable<TagData> targetTags,
            string clearConditionDescription,
            string settlementFormulaDescription,
            string difficultyDisplay,
            int clearTargetCount,
            int settlementRewardPerCard)
        {
            return new MissionDefinitionData(
                id,
                displayName,
                template,
                targetTags,
                clearConditionDescription,
                settlementFormulaDescription,
                difficultyDisplay,
                clearTargetCount,
                settlementRewardPerCard);
        }
    }
}
