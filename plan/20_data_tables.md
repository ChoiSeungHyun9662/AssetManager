# 20. 데이터 테이블 구조

## 1. 목적

이 문서는 주식 규칙 개편 후 MVP 구현에 필요한 주요 데이터 테이블과 런타임 상태 구조를 정리한다.

본 문서는 최종 코드 구조를 강제하지 않는다.
다만 Unity 구현 시 동일한 필드 의미를 유지해야 한다.

---

## 2. 주요 enum

### 2.1 ResourceType

```csharp
public enum ResourceType
{
    Cash,
    Reading,
    Meditation,
    Patience,
    Deal
}
```

의미:

| 값 | 표시 이름 |
|---|---|
| Cash | 현금 |
| Reading | 독서 |
| Meditation | 명상 |
| Patience | 인내 |
| Deal | 딜 |

---

### 2.2 MarketCardType

```csharp
public enum MarketCardType
{
    Stock,
    ConsumableResource
}
```

---

### 2.3 MarketDeckType

```csharp
public enum MarketDeckType
{
    Stock,
    ConsumableResource
}
```

---

### 2.4 CardRarity

```csharp
public enum CardRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
```

---

### 2.5 StockRuntimeState

```csharp
public enum StockRuntimeState
{
    StockDeck,
    Market,
    Owned,
    Removed
}
```

예약은 별도 상태가 아니라 시장 슬롯의 `IsReserved` 값으로 관리한다.

---

### 2.6 StockIndustryTag

```csharp
public enum StockIndustryTag
{
    Technology,
    Consumer,
    Energy,
    Financials,
    Industrials
}
```

모든 주식은 산업 태그를 정확히 1개 가진다.
태그는 카드 자체 효과가 아니라 미션 조건과 정산 공식, 포트폴리오 방향성에 사용한다.

---

### 2.7 MissionState

```csharp
public enum MissionState
{
    Candidate,
    Confirmed,
    Discarded
}
```

---

### 2.8 MarketOfferRole

```csharp
public enum MarketOfferRole
{
    PaymentReduction,
    CharacterTransformation,
    Tempo
}
```

---

### 2.9 RunState

```csharp
public enum RunState
{
    NotStarted,
    Playing,
    Bankrupt,
    Completed
}
```

---

### 2.10 RentArrearsLevel

```csharp
public enum RentArrearsLevel
{
    Low,
    Medium,
    High
}
```

---

## 3. 정적 데이터

### 3.1 StockCardData

```csharp
public class StockCardData
{
    public string StockId;
    public string DisplayName;
    public string ImageResourcePath;

    public CardRarity Rarity;

    public int CashCost;
    public InvestmentPhilosophyCost PhilosophyCost;

    public int BaseValue;
    public int BaseDividend;
    public int FoilValue;
    public int FoilDividend;
    public StockIndustryTag IndustryTag;

    public int MinCopiesInDeck;
    public int MaxCopiesInDeck;
}
```

주의:

```text
- 호일 주식은 별도 덱 카드가 아니다.
- 호일 가치/배당금은 직접 지정한다.
- 산업 태그는 정확히 1개 지정한다.
- 종목별 덱 포함 장수는 min~max 값으로 조정한다.
```

---

### 3.2 InvestmentPhilosophyCost

```csharp
public class InvestmentPhilosophyCost
{
    public int Reading;
    public int Meditation;
    public int Patience;
}
```

---

### 3.3 ConsumableResourceCardData

```csharp
public class ConsumableResourceCardData
{
    public string Id;
    public string ImageResourcePath;
    public CardRarity Rarity;

    public int CashCost;
    public ConsumableResourceCardEffectType EffectType;
    public int Amount;

    public bool CanRecycle;
}
```

```csharp
public enum ConsumableResourceCardEffectType
{
    GainCash,
    GainReading,
    GainMeditation,
    GainPatience
}
```

소모형 자원 카드에는 이름, 가치, 배당금, 호일 상태가 없다.

---

### 3.4 QuarterData

```csharp
public class QuarterData
{
    public int FiscalYearIndex;
    public int QuarterIndex;

    public bool IsPlayable;
    public bool IsVacationQuarter;

    public int BusinessDayCount;
    public int TargetEvaluation;
}
```

`TargetEvaluation`은 현금 흐름과 미션 수익의 합으로 달성 여부를 판단하는 분기 평가 목표다.

---

### 3.5 FinalRatingData

```csharp
public class FinalRatingData
{
    public string Id;
    public string Grade;
    public string Title;
    public int MinFinalValue;
}
```

최종 평가는 최종 가치 기준으로 판정한다.

---

### 3.6 RentArrearsLevelData

```csharp
public class RentArrearsLevelData
{
    public RentArrearsLevel Level;
    public int MinRentArrears;
}
```

초기값:

```text
Low: 0
Medium: 3
High: 6
```

---

### 3.7 FinalCommentData

```csharp
public class FinalCommentData
{
    public string Id;
    public string FinalRatingGrade;
    public RentArrearsLevel RentArrearsLevel;
    public string Comment;
}
```

---

### 3.8 MarketConfigData

```csharp
public class MarketConfigData
{
    public int MarketSlotCount; // 8

    public float StockDrawWeight; // 0.75
    public float ConsumableResourceDrawWeight; // 0.25
}
```

---

### 3.9 ResourceConfigData

```csharp
public class ResourceConfigData
{
    public int MaxInvestmentPhilosophyTotal; // 10
    public int MaxInvestmentPhilosophyEach; // 5
}
```

---

### 3.10 RentArrearsConfigData

```csharp
public class RentArrearsConfigData
{
    public int MaxRentArrears; // 10
}
```

---

### 3.11 MissionDefinitionData

```csharp
public class MissionDefinitionData
{
    public string MissionId;
    public string DisplayName;
    public string DifficultyLabel;

    public MissionClearConditionData ClearCondition;
    public MissionSettlementFormulaData SettlementFormula;
}
```

미션 클리어 조건과 정산 공식은 분리한다.
난이도 표시는 디스플레이 전용이며 조건이나 보상 수치를 바꾸지 않는다.

---

### 3.12 MarketOfferData

```csharp
public class MarketOfferData
{
    public string OfferId;
    public MarketOfferRole Role;
    public int CashCostDelta;
    public InvestmentPhilosophyCost PhilosophyCostDelta;
    public int PermanentValueDelta;
    public int PermanentDividendDelta;
    public int ExtraBuyGrant;
    public ResourceReward PhilosophyReward;
}
```

결제 완화형은 결제 순간에만 적용하고 영구 흔적을 남기지 않는다.
성격 변환형은 영구 가치/배당 델타를 남길 수 있다.
템포형은 추가 매수권이나 투자 철학 보너스를 줄 수 있지만 비용과 영구 가치/배당을 바꾸지 않는다.

---

## 4. 런타임 상태

### 4.1 ResourceState

```csharp
public class ResourceState
{
    public int Cash;

    public int Reading;
    public int Meditation;
    public int Patience;

    public int Deal;
}
```

---

### 4.2 InvestmentPhilosophyMasteryState

```csharp
public class InvestmentPhilosophyMasteryState
{
    public int Reading;
    public int Meditation;
    public int Patience;
}
```

딜 1개를 소비하면 독서/명상/인내 중 하나의 마스터리가 1 오른다.
각 마스터리는 최대 5이며, 주식 매수 시 같은 종류 투자 철학 비용을 낮춘다.

---

### 4.3 DealRewardState

```csharp
public class DealRewardState
{
    public bool GrantedThreeStockSlots;
    public bool GrantedFiveStockSlots;
    public bool GrantedEightStockSlots;
    public bool GrantedFirstFoil;
}
```

딜 보상은 보유 주식 슬롯 3/5/8 최초 도달과 첫 호일 완성으로 지급한다.
각 보상은 한 런에서 한 번만 지급한다.

---

### 4.4 RunCalendarState

```csharp
public class RunCalendarState
{
    public int CurrentFiscalYear;
    public int CurrentQuarter;
    public int RemainingBusinessDays;
}
```

---

### 4.5 RunRevenueState

```csharp
public class RunRevenueState
{
    public int TotalRevenue;
    public int CurrentFiscalYearRevenue;
    public int CurrentQuarterCashFlow;

    public List<QuarterRevenueData> QuarterRevenueHistory;
}
```

소모형 현금 카드로 얻은 조달 현금은 여기에 포함하지 않는다.
미션 수익은 현금이 아니므로 별도 미션 상태에 기록한다.

---

### 4.6 StockRuntimeData

```csharp
public class StockRuntimeData
{
    public StockCardData BaseData;
    public StockRuntimeState RuntimeState;

    public bool IsFoil;
    public int? MarketSlotIndex;
    public int? PortfolioSlotIndex;
    public int? AcquiredOrder;

    public int PermanentValueDelta;
    public int PermanentDividendDelta;
}
```

---

### 4.7 MarketCardRuntimeData

```csharp
public class MarketCardRuntimeData
{
    public MarketCardType CardType;
    public StockRuntimeData Stock;
    public ConsumableResourceCardData ConsumableResource;
}
```

---

### 4.8 MarketSlot

```csharp
public class MarketSlot
{
    public int SlotIndex;
    public MarketCardRuntimeData Card;
    public bool IsReserved;
    public string CurrentMarketOfferId;
}
```

---

### 4.9 MarketTapeState

```csharp
public class MarketTapeState
{
    public List<MarketSlot> Slots;
}
```

---

### 4.10 MarketDeckState

```csharp
public class MarketDeckState
{
    public Queue<StockRuntimeData> StockDeck;
    public Queue<ConsumableResourceCardData> ConsumableResourceDeck;
    public List<ConsumableResourceCardData> ConsumableRecyclePool;
}
```

---

### 4.10.1 MissionRunState

```csharp
public class MissionRunState
{
    public List<MissionCandidateSlotState> CandidateSlots;
    public string ConfirmedMissionId;
    public int TotalMissionRevenue;
    public int CurrentQuarterMissionRevenue;
}
```

```csharp
public class MissionCandidateSlotState
{
    public int SlotIndex;
    public string MissionId;
    public bool HasUsedMulligan;
    public MissionState State;
}
```

---

### 4.10.2 MarketOfferQuarterState

```csharp
public class MarketOfferQuarterState
{
    public List<MarketOfferSlotState> OfferSlots;
}
```

```csharp
public class MarketOfferSlotState
{
    public int SlotIndex;
    public string OfferId;
    public bool IsConsumed;
}
```

---

### 4.11 PortfolioState

```csharp
public class PortfolioState
{
    public List<PortfolioSlot> Slots;
    public int NextAcquiredOrder;
}
```

```csharp
public class PortfolioSlot
{
    public int SlotIndex;
    public StockRuntimeData Stock;
}
```

---

### 4.12 PurchasePaymentState

```csharp
public class PurchasePaymentState
{
    public MarketCardRuntimeData SelectedCard;

    public List<PaymentSlotState> PaymentSlots;

    public int BaseCashCost;
    public int CashCostBeforeInflation;
    public int FinalCashCost;

    public bool IsValid;
    public string AppliedMarketOfferId;
}
```

---

### 4.13 PaymentSlotState

```csharp
public class PaymentSlotState
{
    public ResourceType RequiredResourceType;
    public bool IsFilled;
    public ResourceType? PlacedResourceType;
    public int SlotIndex;
}
```

---

### 4.14 RentArrearsState

```csharp
public class RentArrearsState
{
    public int CurrentRentArrears;
    public int MaxRentArrears;
}
```

---

## 5. 결과 데이터

### 5.1 QuarterEndResult

```csharp
public class QuarterEndResult
{
    public int FiscalYearIndex;
    public int QuarterIndex;

    public int QuarterCashFlow;
    public int MissionRevenue;
    public int QuarterTargetRevenue;
    public float AchievementRate;

    public int RentArrearsIncrease;
    public int CurrentRentArrears;

    public bool IsSuccess;
    public bool IsBankrupt;
}
```

---

### 5.2 FiscalYearSummaryData

```csharp
public class FiscalYearSummaryData
{
    public int FiscalYearIndex;

    public int CurrentFinalValue;
    public int FiscalYearRevenue;
    public int FiscalYearMissionRevenue;

    public List<QuarterRevenueData> QuarterRevenueList;

    public int OwnedStockCount;
    public int CurrentRentArrears;
}
```

---

### 5.3 FinalRunSummaryData

```csharp
public class FinalRunSummaryData
{
    public int FinalValue;
    public FinalRatingData FinalRating;

    public int TotalRevenue;
    public int TotalMissionRevenue;
    public int OwnedStockCount;
    public int RentArrears;

    public RentArrearsLevel RentArrearsLevel;
    public FinalCommentData Comment;
}
```

---

## 6. 기본 상수

```text
MarketSlotCount = 8
StockDrawWeight = 0.75
ConsumableResourceDrawWeight = 0.25

MaxPortfolioStockCount = 8
MaxReservedStockCount = 1

MaxInvestmentPhilosophyTotal = 10
MaxInvestmentPhilosophyEach = 5
MaxInvestmentPhilosophyMasteryEach = 5
MaxRentArrears = 10
```

---

## 7. 테이블 조정형 항목

```text
- 주식별 현금 비용
- 주식별 투자 철학 비용
- 주식별 기본 가치
- 주식별 기본 배당금
- 주식별 호일 가치
- 주식별 호일 배당금
- 주식별 산업 태그
- 주식별 덱 포함 장수 min~max
- 소모형 자원 카드별 비용/효과/등급
- 미션 정의/클리어 조건/정산 공식/난이도 표시
- Mr.Market 제안 풀/역할군/회계년도별 가중치
- 분기 평가 목표
- 최종 평가 가치 기준값
- 월세 밀림 단계 기준값
- 최종 코멘트 문구
- 시장 덱 드로우 가중치
```

---

## 8. 네이밍 주의사항

사용하지 않는 이름:

```text
AUM
Asset
ProfessionalResource
Research
Credit
Commodity
ManagementValue
IncomeCash
RedemptionPressure
CardDetail
GainLiquidity
ReservedSlot
MarketTapeZone
```

권장 이름:

```text
Stock
InvestmentPhilosophy
Reading
Meditation
Patience
Value
Dividend
Revenue
RentArrears
Bankruptcy
MarketSlot
PortfolioSlot
```

---

## 9. 구현 시 주의사항

```text
- 주식과 소모형 자원 카드는 서로 다른 덱을 가진다.
- 시장 슬롯은 8개 단일 리스트로 관리한다.
- 예약은 시장 슬롯의 IsReserved로 관리한다.
- 포트폴리오는 8칸 슬롯 구조로 관리한다.
- 호일은 IsFoil과 호일 수치로 표현한다.
- 소모형 자원 카드는 재순환 가능하다.
- 제거된 주식은 주식 덱으로 되돌리지 않는다.
- 조달 현금은 수익 상태에 누적하지 않는다.
- 월세 밀림은 증가 즉시 파산 여부를 검사한다.
```
