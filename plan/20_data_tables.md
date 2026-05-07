# 20. 데이터 테이블 구조

## 1. 목적

이 문서는 MVP 구현에 필요한 주요 데이터 테이블과 런타임 상태 구조를 정리한다.

본 문서의 목적은 다음과 같다.

```text
- Codex 또는 개발자가 구현할 데이터 구조를 빠르게 파악할 수 있게 한다.
- 카드, 자원, 시장, 분기, 평가, 코멘트 데이터를 테이블화한다.
- 밸런스 조정이 필요한 수치를 코드 하드코딩이 아니라 데이터로 관리한다.
- 런타임 상태와 정적 데이터를 구분한다.
```

이 문서는 최종 코드 구조를 강제하지 않는다.  
다만 Unity 구현 시 ScriptableObject, JSON, CSV, Addressables, 자체 데이터 테이블 등 어떤 방식으로 구현하더라도 동일한 필드 의미를 유지해야 한다.

---

## 2. 데이터 구분

데이터는 크게 두 종류로 나눈다.

```text
1. 정적 데이터
2. 런타임 상태 데이터
```

---

### 2.1 정적 데이터

정적 데이터는 게임 시작 전 정의되어 있으며, 런 중 원칙적으로 변경되지 않는 데이터이다.

예시:

```text
- 자산 카드 데이터
- 태그 데이터
- 분기 목표 데이터
- 최종 평가 기준 데이터
- 환매 압력 단계 데이터
- 운용 코멘트 데이터
```

정적 데이터는 ScriptableObject 또는 외부 테이블로 관리할 수 있다.

---

### 2.2 런타임 상태 데이터

런타임 상태 데이터는 플레이 중 변하는 데이터이다.

예시:

```text
- 현재 현금
- 현재 전문 자원
- 현재 딜
- 현재 회계년도
- 현재 분기
- 남은 영업일
- 시장 테이프 상태
- 예약 슬롯 상태
- 보유 자산 목록
- 현재 환매 압력
- 현재 분기 운용 수익
```

런타임 상태 데이터는 세이브/로드 대상이 될 수 있다.

---

## 3. 주요 enum

### 3.1 ResourceType

```csharp
public enum ResourceType
{
    Cash,
    Research,
    Credit,
    Commodity,
    Deal
}
```

의미:

| 값 | 의미 |
|---|---|
| Cash | 현금 |
| Research | 리서치 |
| Credit | 신용 |
| Commodity | 원자재 |
| Deal | 딜 칩 |

---

### 3.2 AssetRarity

```csharp
public enum AssetRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
```

희귀도 명칭은 최종 UI 톤에 맞춰 변경할 수 있다.

---

### 3.3 TagType

```csharp
public enum TagType
{
    AssetClass,
    General
}
```

의미:

| 값 | 의미 |
|---|---|
| AssetClass | 자산군 태그 |
| General | 일반 태그 |

---

### 3.4 PurchaseSource

```csharp
public enum PurchaseSource
{
    MarketTape,
    Reserved
}
```

의미:

| 값 | 의미 |
|---|---|
| MarketTape | 시장 테이프에서 매수 |
| Reserved | 예약 카드에서 매수 |

예약 카드 매수도 일반 자산 매수로 판정하지만, 구매 출처는 `Reserved`로 남긴다.

---

### 3.5 AssetCardRuntimeState

```csharp
public enum AssetCardRuntimeState
{
    Deck,
    MarketTape,
    Reserved,
    Owned,
    Removed
}
```

의미:

| 값 | 의미 |
|---|---|
| Deck | 아직 시장에 나오지 않은 카드 |
| MarketTape | 시장 테이프에 있는 카드 |
| Reserved | 예약 슬롯에 있는 카드 |
| Owned | 매수 완료된 보유 자산 |
| Removed | 시장에서 제거된 카드 |

---

### 3.6 MarketAreaState

```csharp
public enum MarketAreaState
{
    Market,
    CardDetail,
    GainLiquidity
}
```

의미:

| 값 | 의미 |
|---|---|
| Market | 기본 시장 상태 |
| CardDetail | 카드 상세보기 |
| GainLiquidity | 유동성 확보 |

---

### 3.7 MarketTapeZone

권장 명칭:

```csharp
public enum MarketTapeZone
{
    SellImminent,
    Current,
    Upcoming
}
```

의미:

| 값 | 의미 |
|---|---|
| SellImminent | 매도 임박 |
| Current | 현재 시장 |
| Upcoming | 예비 시장 |

`Upcoming`은 예비 시장을 뜻한다.  
예약 슬롯과 혼동하지 않기 위해 `ReserveMarket` 같은 명칭은 피하는 것이 좋다.

---

### 3.8 RedemptionPressureLevel

```csharp
public enum RedemptionPressureLevel
{
    Low,
    Medium,
    High
}
```

최종 운용 코멘트 산정에 사용한다.

---

### 3.9 RunState

```csharp
public enum RunState
{
    NotStarted,
    Playing,
    Failed,
    Completed
}
```

---

### 3.10 BusinessDayPhase

```csharp
public enum BusinessDayPhase
{
    Starting,
    WaitingForPlayerAction,
    ResolvingAction,
    Ended
}
```

---

## 4. 정적 데이터 테이블

---

## 4.1 AssetCardData

자산 카드 기본 데이터이다.

```csharp
public class AssetCardData
{
    public string Id;
    public string DisplayName;
    public string Description;

    public string ImageResourcePath;

    public AssetRarity Rarity;

    public int CashCost;
    public ProfessionalResourceCost ProfessionalCost;

    public int ManagementValue;
    public int IncomeCash;

    public List<string> AssetClassTagIds;
    public List<string> TagIds;
}
```

필드 설명:

| 필드 | 의미 |
|---|---|
| Id | 카드 고유 ID |
| DisplayName | 카드 이름 |
| Description | 카드 설명 |
| ImageResourcePath | 카드 이미지 리소스 경로 |
| Rarity | 희귀도 |
| CashCost | 기본 현금 비용 |
| ProfessionalCost | 전문 자원 비용 |
| ManagementValue | 운용가치 |
| IncomeCash | 영업일 시작 인컴 |
| AssetClassTagIds | 자산군 태그 ID 목록 |
| TagIds | 일반 태그 ID 목록 |

주의:

```text
AUM 필드는 사용하지 않는다.
카드 점수 개념은 ManagementValue / 운용가치로 통일한다.
```

---

## 4.2 ProfessionalResourceCost

자산 카드의 전문 자원 비용이다.

```csharp
public class ProfessionalResourceCost
{
    public int Research;
    public int Credit;
    public int Commodity;
}
```

전문 자원 비용이 없는 카드는 모든 값을 0으로 둔다.

```text
Research = 0
Credit = 0
Commodity = 0
```

---

## 4.3 TagData

태그 데이터이다.

```csharp
public class TagData
{
    public string Id;
    public string DisplayName;
    public TagType Type;
    public int SortOrder;
}
```

필드 설명:

| 필드 | 의미 |
|---|---|
| Id | 태그 고유 ID |
| DisplayName | 표시 이름 |
| Type | 자산군 태그 또는 일반 태그 |
| SortOrder | 표시 정렬 순서 |

자산군 태그 정렬은 `SortOrder`를 기준으로 한다.

```text
SortOrder 오름차순
→ 동률이면 Id 오름차순
```

---

## 4.4 QuarterData

분기 데이터이다.

```csharp
public class QuarterData
{
    public int FiscalYearIndex;
    public int QuarterIndex;

    public bool IsPlayable;
    public bool IsVacationQuarter;

    public int BusinessDayCount;
    public int TargetEarnedCash;
}
```

필드 설명:

| 필드 | 의미 |
|---|---|
| FiscalYearIndex | 회계년도 번호 |
| QuarterIndex | 분기 번호 |
| IsPlayable | 플레이 가능한 분기인지 |
| IsVacationQuarter | 휴가 분기인지 |
| BusinessDayCount | 분기당 영업일 수 |
| TargetEarnedCash | 분기 목표 운용 수익 |

예시:

```text
1회계년도 1Q
IsPlayable: true
BusinessDayCount: 4

1회계년도 4Q
IsPlayable: false
IsVacationQuarter: true

3회계년도 4Q
IsPlayable: true
BusinessDayCount: 5
```

---

## 4.5 FiscalYearData

회계년도 데이터이다.

```csharp
public class FiscalYearData
{
    public int FiscalYearIndex;
    public List<QuarterData> Quarters;
}
```

---

## 4.6 FinalRatingData

최종 평가 등급 데이터이다.

```csharp
public class FinalRatingData
{
    public string Id;
    public string Grade;
    public string Title;
    public int MinManagementValue;
}
```

예시:

```text
id: rating_d
grade: D
title: 생존한 운용자
min_management_value: 0

id: rating_c
grade: C
title: 신중한 운용자
min_management_value: 50

id: rating_b
grade: B
title: 유능한 펀드매니저
min_management_value: 100

id: rating_a
grade: A
title: 스타 펀드매니저
min_management_value: 150

id: rating_s
grade: S
title: 전설적인 펀드매니저
min_management_value: 200
```

기준값은 테이블 조정형이다.

---

## 4.7 RedemptionPressureLevelData

환매 압력 단계 데이터이다.

```csharp
public class RedemptionPressureLevelData
{
    public RedemptionPressureLevel Level;
    public int MinRedemptionPressure;
}
```

초기값:

```text
Low
min_redemption_pressure: 0

Medium
min_redemption_pressure: 3

High
min_redemption_pressure: 6
```

이 기준은 최종 운용 코멘트 산정에 사용한다.  
기준값은 테이블 조정형이다.

---

## 4.8 FinalManagementCommentData

최종 운용 코멘트 데이터이다.

```csharp
public class FinalManagementCommentData
{
    public string Id;

    public string FinalRatingGrade;
    public RedemptionPressureLevel RedemptionPressureLevel;

    public string Comment;
}
```

예시:

```text
id: comment_a_high
final_rating_grade: A
redemption_pressure_level: High
comment: 높은 환매 압력을 감수하며 스타급 성과를 낸 공격적인 운용입니다.
```

총 15개 조합을 준비할 수 있다.

```text
D / C / B / A / S
×
Low / Medium / High
=
15개
```

---

## 4.9 MarketConfigData

시장 테이프 구성값이다.

```csharp
public class MarketConfigData
{
    public int SlotCountPerZone;
}
```

MVP에서는 각 구역의 슬롯 수를 동일하게 둔다.

```text
매도 임박 N장
현재 시장 N장
예비 시장 N장
```

N은 데이터에서 조정 가능하게 둔다.

---

## 4.10 ResourceConfigData

자원 관련 상수값이다.

```csharp
public class ResourceConfigData
{
    public int MaxProfessionalResourceTotal;
    public int MaxDeal;
}
```

초기값:

```text
MaxProfessionalResourceTotal = 10
MaxDeal = 3
```

---

## 4.11 RedemptionPressureConfigData

환매 압력 관련 상수값이다.

```csharp
public class RedemptionPressureConfigData
{
    public int MaxRedemptionPressure;
}
```

초기값:

```text
MaxRedemptionPressure = 10
```

---

## 5. 런타임 상태 데이터

---

## 5.1 ResourceState

현재 보유 자원 상태이다.

```csharp
public class ResourceState
{
    public int Cash;

    public int Research;
    public int Credit;
    public int Commodity;

    public int Deal;
}
```

전문 자원 합계:

```csharp
int GetProfessionalResourceTotal()
{
    return Research + Credit + Commodity;
}
```

---

## 5.2 RunCalendarState

현재 시간 진행 상태이다.

```csharp
public class RunCalendarState
{
    public int CurrentFiscalYear;
    public int CurrentQuarter;
    public int RemainingBusinessDays;
}
```

---

## 5.3 RunPerformanceState

운용 수익 기록 상태이다.

```csharp
public class RunPerformanceState
{
    public int TotalEarnedCash;
    public int CurrentFiscalYearEarnedCash;
    public int CurrentQuarterEarnedCash;

    public List<QuarterEarnedCashData> QuarterEarnedCashHistory;
}
```

유동성 확보 현금은 여기에 포함하지 않는다.

---

## 5.4 QuarterEarnedCashData

분기별 운용 수익 기록이다.

```csharp
public class QuarterEarnedCashData
{
    public int FiscalYearIndex;
    public int QuarterIndex;
    public int EarnedCash;
}
```

4Q 휴가 화면과 최종 기록에 사용한다.

---

## 5.5 AssetCardRuntimeData

런 중 자산 카드의 현재 상태이다.

```csharp
public class AssetCardRuntimeData
{
    public AssetCardData BaseData;

    public AssetCardRuntimeState RuntimeState;

    public PurchaseSource? PurchaseSource;

    public MarketTapeZone? MarketTapeZone;
    public int? MarketSlotIndex;

    public int? ReservedSlotIndex;

    public int? AcquiredOrder;
}
```

필드 설명:

| 필드 | 의미 |
|---|---|
| BaseData | 정적 카드 데이터 |
| RuntimeState | 현재 카드 상태 |
| PurchaseSource | 매수 출처 |
| MarketTapeZone | 시장 테이프 구역 |
| MarketSlotIndex | 시장 슬롯 인덱스 |
| ReservedSlotIndex | 예약 슬롯 인덱스 |
| AcquiredOrder | 매수 순서 |

---

## 5.6 MarketTapeSlot

시장 테이프 슬롯이다.

```csharp
public class MarketTapeSlot
{
    public MarketTapeZone Zone;
    public int SlotIndex;
    public AssetCardRuntimeData Card;
}
```

---

## 5.7 MarketTapeState

시장 테이프 전체 상태이다.

```csharp
public class MarketTapeState
{
    public List<MarketTapeSlot> SellImminentSlots;
    public List<MarketTapeSlot> CurrentSlots;
    public List<MarketTapeSlot> UpcomingSlots;
}
```

---

## 5.8 ReservedAssetSlot

예약 슬롯이다.

```csharp
public class ReservedAssetSlot
{
    public int SlotIndex;
    public AssetCardRuntimeData ReservedAsset;
}
```

---

## 5.9 ReservationState

예약 슬롯 전체 상태이다.

```csharp
public class ReservationState
{
    public List<ReservedAssetSlot> ReservedSlots;
}
```

예약 슬롯 수:

```text
3개
```

---

## 5.10 OwnedAssetState

보유 자산 상태는 리스트로 관리할 수 있다.

```csharp
public class OwnedAssetState
{
    public List<AssetCardRuntimeData> OwnedAssets;
    public int NextAcquiredOrder;
}
```

자산 매수 시:

```text
AcquiredOrder = NextAcquiredOrder
NextAcquiredOrder += 1
```

---

## 5.11 CardDetailState

카드 상세보기 상태이다.

```csharp
public class CardDetailState
{
    public AssetCardRuntimeData SelectedCard;
    public PurchaseSource PurchaseSource;

    public PurchasePaymentState PaymentState;

    public bool IsOpenedDuringExtraBuy;
}
```

---

## 5.12 PurchasePaymentState

매수 결제 대기 상태이다.

```csharp
public class PurchasePaymentState
{
    public AssetCardRuntimeData SelectedAsset;

    public List<PaymentSlotState> PaymentSlots;

    public int BaseCashCost;
    public int UsedDealCount;
    public int CashCostBeforeInflation;
    public int FinalCashCost;

    public bool IsValid;
}
```

---

## 5.13 PaymentSlotState

전문 자원 비용 슬롯 상태이다.

```csharp
public class PaymentSlotState
{
    public ResourceType RequiredResourceType;

    public bool IsFilled;

    public ResourceType? PlacedResourceType;

    public bool IsFilledByDeal;

    public int SlotIndex;
}
```

---

## 5.14 PurchaseContext

매수 확정 처리 컨텍스트이다.

```csharp
public class PurchaseContext
{
    public AssetCardRuntimeData PurchasedAsset;

    public PurchaseSource PurchaseSource;

    public MarketTapeZone? SourceMarketTapeZone;
    public int? SourceSlotIndex;

    public int? SourceReservedSlotIndex;

    public bool IsExtraBuyAction;

    public int UsedDealCount;
    public int FinalCashCost;
}
```

---

## 5.15 LiquidityActionState

유동성 확보 진행 상태이다.

```csharp
public class LiquidityActionState
{
    public List<ResourceType> SelectedResources = new();

    public bool HasGainedAnyResource;
}
```

---

## 5.16 BusinessDayState

영업일 진행 상태이다.

```csharp
public class BusinessDayState
{
    public BusinessDayPhase Phase;

    public bool HasExtraBuyAction;
    public bool IsWaitingForExtraBuyDecision;
    public bool IsExtraBuyPurchase;
}
```

---

## 5.17 RedemptionPressureState

환매 압력 상태이다.

```csharp
public class RedemptionPressureState
{
    public int CurrentRedemptionPressure;
    public int MaxRedemptionPressure;
}
```

초기 한도:

```text
MaxRedemptionPressure = 10
```

---

## 6. 결과 화면 데이터

---

## 6.1 QuarterEndResult

분기 마감 결과 데이터이다.

```csharp
public class QuarterEndResult
{
    public int FiscalYearIndex;
    public int QuarterIndex;

    public int QuarterEarnedCash;
    public int QuarterTargetCash;
    public float AchievementRate;

    public int QuarterSettlementIncome;
    public int RedemptionPressureIncrease;
    public int CurrentRedemptionPressure;

    public bool IsSuccess;
    public bool IsRunFailed;
}
```

---

## 6.2 FiscalYearSummaryData

4Q 휴가 화면에 표시할 회계년도 요약 데이터이다.

```csharp
public class FiscalYearSummaryData
{
    public int FiscalYearIndex;

    public int CurrentManagementValue;
    public int FiscalYearEarnedCash;

    public List<QuarterEarnedCashData> QuarterEarnedCashList;

    public int OwnedAssetCount;
    public int CurrentRedemptionPressure;
}
```

---

## 6.3 FinalRunSummaryData

최종 정산 화면 데이터이다.

```csharp
public class FinalRunSummaryData
{
    public int FinalManagementValue;
    public FinalRatingData FinalRating;

    public int TotalEarnedCash;
    public int OwnedAssetCount;
    public int RedemptionPressure;

    public RedemptionPressureLevel RedemptionPressureLevel;
    public FinalManagementCommentData ManagementComment;
}
```

---

## 7. 상수 기본값

MVP 기준 기본값은 다음과 같다.

```text
MaxProfessionalResourceTotal = 10
MaxDeal = 3
MaxReservedAssetCount = 3
MaxRedemptionPressure = 10
```

시간 구조:

```text
1회계년도 분기당 영업일 = 4
2회계년도 분기당 영업일 = 4
3회계년도 분기당 영업일 = 5
```

플레이 분기:

```text
1회계년도: 1Q~3Q
2회계년도: 1Q~3Q
3회계년도: 1Q~4Q
```

---

## 8. 테이블 조정형 항목

다음 항목은 하드코딩하지 않고 데이터 테이블로 조정 가능하게 둔다.

```text
- 카드별 현금 비용
- 카드별 전문 자원 비용
- 카드별 운용가치
- 카드별 인컴
- 카드별 태그
- 카드별 희귀도
- 분기 목표 운용 수익
- 최종 평가 운용가치 기준값
- 환매 압력 단계 기준값
- 운용 코멘트 문구
- 시장 테이프 슬롯 수
```

---

## 9. 저장 대상 후보

런 저장 기능을 고려할 경우 다음 데이터는 저장 대상이다.

```text
- RunState
- RunCalendarState
- ResourceState
- RunPerformanceState
- RedemptionPressureState
- MarketTapeState
- ReservationState
- OwnedAssetState
- AssetCardRuntimeData 목록
- BusinessDayState
```

현재 열려 있는 카드 상세보기나 유동성 확보 중간 상태를 저장할지 여부는 별도 저장 정책에서 결정한다.  
MVP에서는 영업일 입력 대기 상태에서만 저장을 허용하는 방식이 단순하다.

---

## 10. 데이터 네이밍 주의사항

```text
- AUM이라는 필드명은 사용하지 않는다.
- 운용가치는 ManagementValue를 사용한다.
- 최종 운용가치는 FinalManagementValue를 사용한다.
- 평가 기준은 MinManagementValue를 사용한다.
- 턴이라는 유저-facing 표현은 사용하지 않는다.
- 내부 코드에서 Turn을 쓰더라도 UI에는 영업일로 표시한다.
```

금지 예시:

```csharp
public int Aum;
public int TotalAum;
public int MinAum;
```

권장 예시:

```csharp
public int ManagementValue;
public int TotalManagementValue;
public int FinalManagementValue;
public int MinManagementValue;
```

---

## 11. 구현 시 주의사항

```text
- 정적 데이터와 런타임 상태 데이터를 구분한다.
- 밸런스 수치는 테이블 조정형으로 둔다.
- 카드 운용가치 필드명은 ManagementValue로 통일한다.
- AUM이라는 명칭은 사용하지 않는다.
- 유동성 확보 현금은 운용 수익 상태에 누적하지 않는다.
- 예약 카드와 보유 자산을 명확히 구분한다.
- 시장 테이프와 예약 슬롯은 별도 상태로 관리한다.
- 환매 압력 한도는 10이지만 ConfigData로 관리할 수 있다.
- 최종 평가는 FinalRatingTable에서 판정한다.
- 운용 코멘트는 FinalRatingGrade × RedemptionPressureLevel로 판정한다.
