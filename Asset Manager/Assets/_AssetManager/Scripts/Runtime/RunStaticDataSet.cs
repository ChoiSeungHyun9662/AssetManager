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
            var realEstateTag = new TagData("real-estate", "부동산", TagType.Sector);

            assetCards = new List<AssetCardData>
            {
                new AssetCardData(
                    "starter-office-reit",
                    "테스트 오피스 리츠",
                    "MVP 부트스트랩 확인용 자산 카드",
                    AssetRarity.Common,
                    2,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Research, 1)
                    },
                    3,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-logistics-center",
                    "테스트 물류 센터",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    2,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Credit, 1)
                    },
                    2,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-retail-strip",
                    "테스트 리테일 스트립",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    1,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Research, 1)
                    },
                    2,
                    0,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-data-center",
                    "테스트 데이터 센터",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Uncommon,
                    3,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Research, 1),
                        new ProfessionalResourceCost(ResourceType.Credit, 1)
                    },
                    4,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-warehouse-loan",
                    "테스트 창고 대출",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    1,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Commodity, 1)
                    },
                    1,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-suburban-office",
                    "테스트 교외 오피스",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    2,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Credit, 1)
                    },
                    2,
                    0,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-urban-mixed-use",
                    "테스트 복합 상가",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Uncommon,
                    3,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Research, 1)
                    },
                    3,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-hotel-note",
                    "테스트 호텔 노트",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    1,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Credit, 1)
                    },
                    1,
                    0,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-industrial-yard",
                    "테스트 산업 부지",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    2,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Commodity, 1)
                    },
                    3,
                    0,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-senior-housing",
                    "테스트 시니어 하우징",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Uncommon,
                    3,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Research, 1),
                        new ProfessionalResourceCost(ResourceType.Commodity, 1)
                    },
                    4,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-parking-rights",
                    "테스트 주차 권리",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    1,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Research, 1)
                    },
                    1,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-medical-office",
                    "테스트 메디컬 오피스",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    2,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Credit, 1)
                    },
                    3,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-campus-housing",
                    "테스트 캠퍼스 하우징",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    2,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Research, 1)
                    },
                    2,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-cold-storage",
                    "테스트 냉장 창고",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Uncommon,
                    3,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Commodity, 1)
                    },
                    4,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-renewal-lot",
                    "테스트 재개발 부지",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    1,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Credit, 1)
                    },
                    2,
                    0,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-bridge-loan",
                    "테스트 브릿지 론",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    2,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Credit, 1)
                    },
                    2,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-solar-roof-lease",
                    "테스트 태양광 지붕 임대",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Uncommon,
                    3,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Research, 1),
                        new ProfessionalResourceCost(ResourceType.Commodity, 1)
                    },
                    4,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-mall-anchor",
                    "테스트 몰 앵커",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    2,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Research, 1)
                    },
                    3,
                    0,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-self-storage",
                    "테스트 셀프 스토리지",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    1,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Commodity, 1)
                    },
                    2,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-life-science-suite",
                    "테스트 연구 시설",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Uncommon,
                    3,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Research, 1),
                        new ProfessionalResourceCost(ResourceType.Credit, 1)
                    },
                    4,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-transit-retail",
                    "테스트 역세권 리테일",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    2,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Credit, 1)
                    },
                    3,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-industrial-reit",
                    "테스트 산업 리츠",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    2,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Commodity, 1)
                    },
                    3,
                    1,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-student-housing",
                    "테스트 학생 하우징",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    1,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Research, 1)
                    },
                    2,
                    0,
                    new[] { realEstateTag }),
                new AssetCardData(
                    "starter-ground-lease",
                    "테스트 토지 임대권",
                    "MVP 시장 테이프 확인용 자산 카드",
                    AssetRarity.Common,
                    1,
                    new[]
                    {
                        new ProfessionalResourceCost(ResourceType.Credit, 1)
                    },
                    1,
                    1,
                    new[] { realEstateTag },
                    true)
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
                new FinalManagementCommentData("seed-stable", "seed", "stable", "기초 운용 체계가 작동합니다."),
                new FinalManagementCommentData("seed-watch", "seed", "watch", "기초 운용은 유지했지만 환매 압력을 낮춰야 합니다."),
                new FinalManagementCommentData("seed-critical", "seed", "critical", "운용 기반은 남았지만 환매 압력이 높습니다."),
                new FinalManagementCommentData("core-stable", "core", "stable", "안정적인 핵심 포트폴리오를 구축했습니다."),
                new FinalManagementCommentData("core-watch", "core", "watch", "성과는 충분하지만 환매 압력 관리가 필요합니다."),
                new FinalManagementCommentData("core-critical", "core", "critical", "성과는 충분하지만 환매 압력이 높습니다."),
                new FinalManagementCommentData("flagship-stable", "flagship", "stable", "대표 운용사로 인정받을 만한 성과입니다."),
                new FinalManagementCommentData("flagship-watch", "flagship", "watch", "높은 성과와 함께 환매 압력 관리 과제가 남았습니다."),
                new FinalManagementCommentData("flagship-critical", "flagship", "critical", "성과는 탁월하지만 환매 압력이 높습니다.")
            };

            marketConfig = new MarketConfigData(3, 3, 3);
            resourceConfig = new ResourceConfigData(3, 10, 3);
            redemptionPressureConfig = new RedemptionPressureConfigData(0, 10);
        }
    }
}
