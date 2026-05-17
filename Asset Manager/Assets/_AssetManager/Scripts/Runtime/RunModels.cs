using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssetManager
{
    public enum ResourceType
    {
        Cash = 0,
        Reading = 1,
        Meditation = 2,
        Patience = 3,
        Deal = 4,
        Research = Reading,
        Credit = Meditation,
        Commodity = Patience
    }

    public enum CardDomain
    {
        Stock,
        ConsumableResource
    }

    public enum AssetRarity
    {
        Common,
        Uncommon,
        Rare
    }

    public enum TagType
    {
        Sector,
        Strategy,
        Region
    }

    public enum PurchaseSource
    {
        MarketTape,
        Reserved
    }

    public enum AssetCardRuntimeState
    {
        Available,
        Reserved,
        Owned,
        Removed
    }

    public enum MarketAreaState
    {
        Market,
        CardDetail,
        GainLiquidity
    }

    public enum MarketTapeZone
    {
        SellImminent,
        CurrentMarket,
        UpcomingMarket
    }

    public enum RunState
    {
        NotStarted,
        Playing,
        Failed,
        Completed
    }

    public enum BusinessDayPhase
    {
        AwaitingAction,
        ResolvingAction,
        QuarterSettlement,
        Vacation,
        FinalSettlement
    }

    [Serializable]
    public sealed class ProfessionalResourceCost
    {
        [SerializeField]
        private ResourceType resourceType;

        [SerializeField]
        private int amount;

        public ProfessionalResourceCost()
        {
        }

        public ProfessionalResourceCost(ResourceType resourceType, int amount)
        {
            this.resourceType = resourceType;
            this.amount = amount;
        }

        public ResourceType ResourceType => resourceType;
        public int Amount => amount;
    }

    [Serializable]
    public sealed class TagData
    {
        [SerializeField]
        private string id = string.Empty;

        [SerializeField]
        private string displayName = string.Empty;

        [SerializeField]
        private TagType tagType;

        public TagData()
        {
        }

        public TagData(string id, string displayName, TagType tagType)
        {
            this.id = id;
            this.displayName = displayName;
            this.tagType = tagType;
        }

        public string Id => id;
        public string DisplayName => displayName;
        public TagType TagType => tagType;
    }

    [Serializable]
    public sealed class AssetCardData
    {
        [SerializeField]
        private string id = string.Empty;

        [SerializeField]
        private string displayName = string.Empty;

        [SerializeField]
        private string description = string.Empty;

        [SerializeField]
        private AssetRarity rarity;

        [SerializeField]
        private CardDomain cardDomain = CardDomain.Stock;

        [SerializeField]
        private int cashCost;

        [SerializeField]
        private List<ProfessionalResourceCost> professionalCosts = new List<ProfessionalResourceCost>();

        [SerializeField]
        private int managementValue;

        [SerializeField]
        private int income;

        [SerializeField]
        private int foilValue;

        [SerializeField]
        private int foilDividend;

        [SerializeField]
        private int minDeckCopies = 1;

        [SerializeField]
        private int maxDeckCopies = 1;

        [SerializeField]
        private List<TagData> tags = new List<TagData>();

        [SerializeField]
        private bool grantsExtraBuyAction;

        [SerializeField]
        private ResourceType providedResourceType = ResourceType.Cash;

        [SerializeField]
        private int providedResourceAmount;

        public AssetCardData()
        {
        }

        public AssetCardData(
            string id,
            string displayName,
            string description,
            AssetRarity rarity,
            int cashCost,
            IEnumerable<ProfessionalResourceCost> professionalCosts,
            int managementValue,
            int income,
            IEnumerable<TagData> tags,
            bool grantsExtraBuyAction = false,
            int foilValue = 0,
            int foilDividend = 0,
            int minDeckCopies = 1,
            int maxDeckCopies = 1,
            CardDomain cardDomain = CardDomain.Stock,
            ResourceType providedResourceType = ResourceType.Cash,
            int providedResourceAmount = 0)
        {
            this.id = id;
            this.displayName = displayName;
            this.description = description;
            this.rarity = rarity;
            this.cardDomain = cardDomain;
            this.cashCost = cashCost;
            this.professionalCosts = new List<ProfessionalResourceCost>(professionalCosts);
            this.managementValue = managementValue;
            this.income = income;
            this.foilValue = foilValue > 0 ? foilValue : managementValue;
            this.foilDividend = foilDividend >= 0 ? foilDividend : income;
            this.minDeckCopies = minDeckCopies;
            this.maxDeckCopies = maxDeckCopies;
            this.tags = new List<TagData>(tags);
            this.grantsExtraBuyAction = grantsExtraBuyAction;
            this.providedResourceType = providedResourceType;
            this.providedResourceAmount = providedResourceAmount;
        }

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public AssetRarity Rarity => rarity;
        public CardDomain CardDomain => cardDomain;
        public int CashCost => cashCost;
        public IReadOnlyList<ProfessionalResourceCost> ProfessionalCosts => professionalCosts;
        public int BaseValue => managementValue;
        public int BaseDividend => income;
        public int ManagementValue => managementValue;
        public int Income => income;
        public int FoilValue => foilValue;
        public int FoilDividend => foilDividend;
        public int MinDeckCopies => minDeckCopies;
        public int MaxDeckCopies => maxDeckCopies;
        public IReadOnlyList<TagData> Tags => tags;
        public bool GrantsExtraBuyAction => grantsExtraBuyAction;
        public ResourceType ProvidedResourceType => providedResourceType;
        public int ProvidedResourceAmount => providedResourceAmount;
    }

    [Serializable]
    public sealed class QuarterData
    {
        [SerializeField]
        private int fiscalYear;

        [SerializeField]
        private int quarter;

        [SerializeField]
        private int businessDays;

        [SerializeField]
        private int earnedCashGoal;

        [SerializeField]
        private int inflationCostModifier;

        public QuarterData()
        {
        }

        public QuarterData(int fiscalYear, int quarter, int businessDays, int earnedCashGoal)
            : this(fiscalYear, quarter, businessDays, earnedCashGoal, 0)
        {
        }

        public QuarterData(
            int fiscalYear,
            int quarter,
            int businessDays,
            int earnedCashGoal,
            int inflationCostModifier)
        {
            this.fiscalYear = fiscalYear;
            this.quarter = quarter;
            this.businessDays = businessDays;
            this.earnedCashGoal = earnedCashGoal;
            this.inflationCostModifier = inflationCostModifier;
        }

        public int FiscalYear => fiscalYear;
        public int Quarter => quarter;
        public int BusinessDays => businessDays;
        public int EarnedCashGoal => earnedCashGoal;
        public int InflationCostModifier => inflationCostModifier;
    }

    [Serializable]
    public sealed class FinalRatingData
    {
        [SerializeField]
        private string ratingId = string.Empty;

        [SerializeField]
        private string displayName = string.Empty;

        [SerializeField]
        private int minimumManagementValue;

        public FinalRatingData()
        {
        }

        public FinalRatingData(string ratingId, string displayName, int minimumManagementValue)
        {
            this.ratingId = ratingId;
            this.displayName = displayName;
            this.minimumManagementValue = minimumManagementValue;
        }

        public string RatingId => ratingId;
        public string DisplayName => displayName;
        public int MinimumManagementValue => minimumManagementValue;
    }

    [Serializable]
    public sealed class RedemptionPressureLevelData
    {
        [SerializeField]
        private string levelId = string.Empty;

        [SerializeField]
        private string displayName = string.Empty;

        [SerializeField]
        private int minimumPressure;

        [SerializeField]
        private int maximumPressure;

        public RedemptionPressureLevelData()
        {
        }

        public RedemptionPressureLevelData(string levelId, string displayName, int minimumPressure, int maximumPressure)
        {
            this.levelId = levelId;
            this.displayName = displayName;
            this.minimumPressure = minimumPressure;
            this.maximumPressure = maximumPressure;
        }

        public string LevelId => levelId;
        public string DisplayName => displayName;
        public int MinimumPressure => minimumPressure;
        public int MaximumPressure => maximumPressure;
    }

    [Serializable]
    public sealed class FinalManagementCommentData
    {
        [SerializeField]
        private string commentId = string.Empty;

        [SerializeField]
        private string ratingId = string.Empty;

        [SerializeField]
        private string pressureLevelId = string.Empty;

        [SerializeField]
        private string comment = string.Empty;

        public FinalManagementCommentData()
        {
        }

        public FinalManagementCommentData(string commentId, string ratingId, string pressureLevelId, string comment)
        {
            this.commentId = commentId;
            this.ratingId = ratingId;
            this.pressureLevelId = pressureLevelId;
            this.comment = comment;
        }

        public string CommentId => commentId;
        public string RatingId => ratingId;
        public string PressureLevelId => pressureLevelId;
        public string Comment => comment;
    }

    [Serializable]
    public sealed class MarketConfigData
    {
        [SerializeField]
        private int marketTapeSlots = 8;

        [SerializeField]
        private int sellImminentSlots = 3;

        [SerializeField]
        private int currentMarketSlots = 3;

        [SerializeField]
        private int upcomingMarketSlots = 3;

        [SerializeField]
        private double stockDeckDrawWeight = 0.75;

        public MarketConfigData()
        {
        }

        public MarketConfigData(int marketTapeSlots, double stockDeckDrawWeight)
            : this(0, marketTapeSlots, 0, stockDeckDrawWeight)
        {
        }

        public MarketConfigData(int sellImminentSlots, int currentMarketSlots, int upcomingMarketSlots)
            : this(sellImminentSlots, currentMarketSlots, upcomingMarketSlots, 0.75)
        {
        }

        public MarketConfigData(
            int sellImminentSlots,
            int currentMarketSlots,
            int upcomingMarketSlots,
            double stockDeckDrawWeight)
        {
            marketTapeSlots = sellImminentSlots + currentMarketSlots + upcomingMarketSlots;
            this.sellImminentSlots = sellImminentSlots;
            this.currentMarketSlots = currentMarketSlots;
            this.upcomingMarketSlots = upcomingMarketSlots;
            this.stockDeckDrawWeight = stockDeckDrawWeight;
        }

        public int MarketTapeSlots => marketTapeSlots;
        public int SellImminentSlots => sellImminentSlots;
        public int CurrentMarketSlots => currentMarketSlots;
        public int UpcomingMarketSlots => upcomingMarketSlots;
        public double StockDeckDrawWeight => stockDeckDrawWeight;
    }

    [Serializable]
    public sealed class ResourceConfigData
    {
        [SerializeField]
        private int startingCash = 3;

        [SerializeField]
        private int investmentPhilosophyCap = 10;

        [SerializeField]
        private int investmentPhilosophyTypeCap = 5;

        [SerializeField]
        private int maxDeal = 3;

        public ResourceConfigData()
        {
        }

        public ResourceConfigData(int startingCash, int investmentPhilosophyCap, int maxDeal)
            : this(startingCash, investmentPhilosophyCap, 5, maxDeal)
        {
        }

        public ResourceConfigData(
            int startingCash,
            int investmentPhilosophyCap,
            int investmentPhilosophyTypeCap,
            int maxDeal)
        {
            this.startingCash = startingCash;
            this.investmentPhilosophyCap = investmentPhilosophyCap;
            this.investmentPhilosophyTypeCap = investmentPhilosophyTypeCap;
            this.maxDeal = maxDeal;
        }

        public int StartingCash => startingCash;
        public int InvestmentPhilosophyCap => investmentPhilosophyCap;
        public int InvestmentPhilosophyTypeCap => investmentPhilosophyTypeCap;
        public int ProfessionalResourceCap => investmentPhilosophyCap;
        public int MaxDeal => maxDeal;
    }

    [Serializable]
    public sealed class RedemptionPressureConfigData
    {
        [SerializeField]
        private int startingPressure;

        [SerializeField]
        private int maxPressure = 10;

        public RedemptionPressureConfigData()
        {
        }

        public RedemptionPressureConfigData(int startingPressure, int maxPressure)
        {
            this.startingPressure = startingPressure;
            this.maxPressure = maxPressure;
        }

        public int StartingPressure => startingPressure;
        public int MaxPressure => maxPressure;
    }

    public sealed class ResourceState
    {
        public ResourceState(int cash, int reading, int meditation, int patience, int deal)
        {
            Cash = cash;
            Reading = reading;
            Meditation = meditation;
            Patience = patience;
            Deal = deal;
        }

        public int Cash { get; }
        public int Reading { get; }
        public int Meditation { get; }
        public int Patience { get; }
        public int Research => Reading;
        public int Credit => Meditation;
        public int Commodity => Patience;
        public int Deal { get; }
        public int InvestmentPhilosophyTotal => Reading + Meditation + Patience;
        public int ProfessionalTotal => InvestmentPhilosophyTotal;

        public int Get(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Cash:
                    return Cash;
                case ResourceType.Reading:
                    return Reading;
                case ResourceType.Meditation:
                    return Meditation;
                case ResourceType.Patience:
                    return Patience;
                case ResourceType.Deal:
                    return Deal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }
    }

    public sealed class RunCalendarState
    {
        public RunCalendarState(int fiscalYear, int quarter, int remainingBusinessDays)
        {
            FiscalYear = fiscalYear;
            Quarter = quarter;
            RemainingBusinessDays = remainingBusinessDays;
        }

        public int FiscalYear { get; }
        public int Quarter { get; }
        public int RemainingBusinessDays { get; }
    }

    public sealed class RunPerformanceState
    {
        public RunPerformanceState(int earnedCash, int fundingCash)
            : this(earnedCash, earnedCash, earnedCash, fundingCash, Array.Empty<QuarterPerformanceRecord>())
        {
        }

        public RunPerformanceState(
            int currentQuarterEarnedCash,
            int currentFiscalYearEarnedCash,
            int totalEarnedCash,
            int fundingCash)
            : this(
                currentQuarterEarnedCash,
                currentFiscalYearEarnedCash,
                totalEarnedCash,
                fundingCash,
                Array.Empty<QuarterPerformanceRecord>())
        {
        }

        public RunPerformanceState(
            int currentQuarterEarnedCash,
            int currentFiscalYearEarnedCash,
            int totalEarnedCash,
            int fundingCash,
            IEnumerable<QuarterPerformanceRecord> completedQuarterEarnedCash)
        {
            CurrentQuarterEarnedCash = currentQuarterEarnedCash;
            CurrentFiscalYearEarnedCash = currentFiscalYearEarnedCash;
            TotalEarnedCash = totalEarnedCash;
            FundingCash = fundingCash;
            CompletedQuarterEarnedCash = new List<QuarterPerformanceRecord>(
                completedQuarterEarnedCash ?? Array.Empty<QuarterPerformanceRecord>()).AsReadOnly();
        }

        public int CurrentQuarterEarnedCash { get; }
        public int CurrentFiscalYearEarnedCash { get; }
        public int TotalEarnedCash { get; }
        public int EarnedCash => TotalEarnedCash;
        public int FundingCash { get; }
        public IReadOnlyList<QuarterPerformanceRecord> CompletedQuarterEarnedCash { get; }
    }

    public sealed class QuarterPerformanceRecord
    {
        public QuarterPerformanceRecord(int fiscalYear, int quarter, int earnedCash)
        {
            FiscalYear = fiscalYear;
            Quarter = quarter;
            EarnedCash = earnedCash;
        }

        public int FiscalYear { get; }
        public int Quarter { get; }
        public int EarnedCash { get; }
    }

    public sealed class AssetCardRuntimeData
    {
        public AssetCardRuntimeData(AssetCardData card, AssetCardRuntimeState state, PurchaseSource? purchaseSource)
            : this(card, state, purchaseSource, null)
        {
        }

        public AssetCardRuntimeData(
            AssetCardData card,
            AssetCardRuntimeState state,
            PurchaseSource? purchaseSource,
            int? acquiredOrder)
        {
            Card = card;
            State = state;
            PurchaseSource = purchaseSource;
            AcquiredOrder = acquiredOrder;
        }

        public AssetCardData Card { get; }
        public AssetCardRuntimeState State { get; }
        public PurchaseSource? PurchaseSource { get; }
        public int? AcquiredOrder { get; }
    }

    public sealed class MarketTapeSlotState
    {
        public MarketTapeSlotState(AssetCardRuntimeData card, bool isReserved)
        {
            Card = card;
            IsReserved = isReserved;
        }

        public AssetCardRuntimeData Card { get; }
        public bool IsReserved { get; }
        public bool IsEmpty => Card == null;
    }

    public sealed class MarketTapeState
    {
        public MarketTapeState(
            IEnumerable<AssetCardRuntimeData> sellImminentCards,
            IEnumerable<AssetCardRuntimeData> currentMarketCards,
            IEnumerable<AssetCardRuntimeData> upcomingMarketCards)
        {
            SellImminentCards = new List<AssetCardRuntimeData>(sellImminentCards).AsReadOnly();
            CurrentMarketCards = new List<AssetCardRuntimeData>(currentMarketCards).AsReadOnly();
            UpcomingMarketCards = new List<AssetCardRuntimeData>(upcomingMarketCards).AsReadOnly();
            Slots = BuildSlots(SellImminentCards, CurrentMarketCards, UpcomingMarketCards);
        }

        public MarketTapeState(IEnumerable<MarketTapeSlotState> slots)
        {
            Slots = new List<MarketTapeSlotState>(slots).AsReadOnly();
            SellImminentCards = Array.Empty<AssetCardRuntimeData>();
            CurrentMarketCards = CollectSlotCards(Slots).AsReadOnly();
            UpcomingMarketCards = Array.Empty<AssetCardRuntimeData>();
        }

        public IReadOnlyList<MarketTapeSlotState> Slots { get; }
        public IReadOnlyList<AssetCardRuntimeData> SellImminentCards { get; }
        public IReadOnlyList<AssetCardRuntimeData> CurrentMarketCards { get; }
        public IReadOnlyList<AssetCardRuntimeData> UpcomingMarketCards { get; }

        private static IReadOnlyList<MarketTapeSlotState> BuildSlots(
            IEnumerable<AssetCardRuntimeData> sellImminentCards,
            IEnumerable<AssetCardRuntimeData> currentMarketCards,
            IEnumerable<AssetCardRuntimeData> upcomingMarketCards)
        {
            var slots = new List<MarketTapeSlotState>();
            AddSlots(slots, sellImminentCards);
            AddSlots(slots, currentMarketCards);
            AddSlots(slots, upcomingMarketCards);
            return slots.AsReadOnly();
        }

        private static void AddSlots(
            List<MarketTapeSlotState> slots,
            IEnumerable<AssetCardRuntimeData> cards)
        {
            foreach (var card in cards)
            {
                slots.Add(new MarketTapeSlotState(card, card.State == AssetCardRuntimeState.Reserved));
            }
        }

        private static List<AssetCardRuntimeData> CollectSlotCards(IEnumerable<MarketTapeSlotState> slots)
        {
            var cards = new List<AssetCardRuntimeData>();
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty)
                {
                    cards.Add(slot.Card);
                }
            }

            return cards;
        }
    }

    public sealed class ReservationState
    {
        public ReservationState(int capacity, IEnumerable<AssetCardRuntimeData> reservedCards)
        {
            Capacity = capacity;
            ReservedCards = new List<AssetCardRuntimeData>(reservedCards).AsReadOnly();
        }

        public int Capacity { get; }
        public IReadOnlyList<AssetCardRuntimeData> ReservedCards { get; }
    }

    public sealed class OwnedAssetState
    {
        public OwnedAssetState(IEnumerable<AssetCardRuntimeData> ownedCards)
        {
            OwnedCards = new List<AssetCardRuntimeData>(ownedCards).AsReadOnly();
        }

        public IReadOnlyList<AssetCardRuntimeData> OwnedCards { get; }
        public int Count
        {
            get
            {
                var count = 0;
                foreach (var card in OwnedCards)
                {
                    if (card.State == AssetCardRuntimeState.Owned)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public int CurrentManagementValue
        {
            get
            {
                var total = 0;
                foreach (var card in OwnedCards)
                {
                    if (card.State == AssetCardRuntimeState.Owned)
                    {
                        total += card.Card.ManagementValue;
                    }
                }

                return total;
            }
        }

        public int BusinessDayStartIncome
        {
            get
            {
                var total = 0;
                foreach (var card in OwnedCards)
                {
                    if (card.State == AssetCardRuntimeState.Owned)
                    {
                        total += card.Card.Income;
                    }
                }

                return total;
            }
        }
    }

    public sealed class BusinessDayState
    {
        public BusinessDayState(BusinessDayPhase phase, MarketAreaState marketArea)
            : this(phase, marketArea, false, false, false)
        {
        }

        public BusinessDayState(
            BusinessDayPhase phase,
            MarketAreaState marketArea,
            bool hasExtraBuyAction,
            bool isAwaitingExtraBuyChoice,
            bool isBuyingWithExtraBuy)
        {
            Phase = phase;
            MarketArea = marketArea;
            HasExtraBuyAction = hasExtraBuyAction;
            IsAwaitingExtraBuyChoice = isAwaitingExtraBuyChoice;
            IsBuyingWithExtraBuy = isBuyingWithExtraBuy;
        }

        public BusinessDayPhase Phase { get; }
        public MarketAreaState MarketArea { get; }
        public bool HasExtraBuyAction { get; }
        public bool IsAwaitingExtraBuyChoice { get; }
        public bool IsBuyingWithExtraBuy { get; }
    }

    public sealed class LiquidityActionState
    {
        public static readonly LiquidityActionState Empty = new LiquidityActionState(Array.Empty<ResourceType>());

        public LiquidityActionState(IEnumerable<ResourceType> selectedResources)
        {
            SelectedResources = new List<ResourceType>(selectedResources).AsReadOnly();
        }

        public IReadOnlyList<ResourceType> SelectedResources { get; }
        public bool HasGainedAnyResource => SelectedResources.Count > 0;
    }

    public sealed class RedemptionPressureState
    {
        public RedemptionPressureState(int currentPressure, int maxPressure)
        {
            CurrentPressure = currentPressure;
            MaxPressure = maxPressure;
        }

        public int CurrentPressure { get; }
        public int MaxPressure { get; }
    }

    public sealed class QuarterEndResult
    {
        public QuarterEndResult(
            int settlementIncome,
            int quarterEarnedCash,
            int targetEarnedCash,
            double achievementRate,
            int redemptionPressureIncrease,
            int currentRedemptionPressure)
        {
            SettlementIncome = settlementIncome;
            QuarterEarnedCash = quarterEarnedCash;
            TargetEarnedCash = targetEarnedCash;
            AchievementRate = achievementRate;
            RedemptionPressureIncrease = redemptionPressureIncrease;
            CurrentRedemptionPressure = currentRedemptionPressure;
        }

        public int SettlementIncome { get; }
        public int QuarterEarnedCash { get; }
        public int TargetEarnedCash { get; }
        public double AchievementRate { get; }
        public int RedemptionPressureIncrease { get; }
        public int CurrentRedemptionPressure { get; }
    }

    public sealed class RunSessionState
    {
        public RunSessionState(
            RunState state,
            RunStaticDataSet staticData,
            RunCalendarState calendar,
            ResourceState resources,
            RunPerformanceState performance,
            IEnumerable<AssetCardRuntimeData> assetCards,
            MarketTapeState marketTape,
            ReservationState reservation,
            OwnedAssetState ownedAssets,
            BusinessDayState businessDay,
            RedemptionPressureState redemptionPressure,
            CardDetailState cardDetail = null,
            LiquidityActionState liquidityAction = null,
            QuarterEndResult quarterEndResult = null,
            string failureReason = "")
        {
            State = state;
            StaticData = staticData;
            Calendar = calendar;
            Resources = resources;
            Performance = performance;
            AssetCards = new List<AssetCardRuntimeData>(assetCards).AsReadOnly();
            MarketTape = marketTape;
            Reservation = reservation;
            OwnedAssets = ownedAssets;
            BusinessDay = businessDay;
            RedemptionPressure = redemptionPressure;
            CardDetail = cardDetail ?? CardDetailState.Empty;
            LiquidityAction = liquidityAction ?? LiquidityActionState.Empty;
            QuarterEndResult = quarterEndResult;
            FailureReason = failureReason ?? string.Empty;
        }

        public RunState State { get; }
        public RunStaticDataSet StaticData { get; }
        public RunCalendarState Calendar { get; }
        public ResourceState Resources { get; }
        public RunPerformanceState Performance { get; }
        public IReadOnlyList<AssetCardRuntimeData> AssetCards { get; }
        public MarketTapeState MarketTape { get; }
        public ReservationState Reservation { get; }
        public OwnedAssetState OwnedAssets { get; }
        public BusinessDayState BusinessDay { get; }
        public RedemptionPressureState RedemptionPressure { get; }
        public CardDetailState CardDetail { get; }
        public LiquidityActionState LiquidityAction { get; }
        public QuarterEndResult QuarterEndResult { get; }
        public string FailureReason { get; }
    }
}
