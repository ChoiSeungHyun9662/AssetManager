# 06. 자산 카드 구조

## 1. 목적

이 문서는 자산 카드의 데이터 구조와 게임 내 상태 변화를 정의한다.

자산 카드는 본 게임의 핵심 오브젝트이다.  
플레이어는 시장 카드 또는 예약 카드를 매수해 자산을 보유하고, 보유 자산의 인컴과 분기 마감 정산을 통해 운용 수익을 만든다.

자산 카드는 최종 평가의 기준이 되는 `운용가치`를 가진다.

```text
최종 운용가치
= 보유 자산 카드들의 운용가치 합계
```

현금, 리서치, 신용, 원자재, 딜은 운용가치에 포함되지 않는다.

---

## 2. 자산 카드의 역할

자산 카드는 다음 역할을 가진다.

```text
- 매수 대상
- 예약 대상
- 보유 자산
- 인컴 발생원
- 분기 마감 정산 대상
- 최종 운용가치 제공원
- 태그 기반 효과 조건
```

플레이어는 제한된 영업일과 자원을 사용해 자산 카드를 매수한다.  
매수한 카드는 포트폴리오에 추가되며, 이후 영업일 시작과 분기 마감에서 효과를 발휘할 수 있다.

---

## 3. 자산 카드 기본 정보

자산 카드는 최소한 다음 정보를 가진다.

```text
- 카드 ID
- 카드 이름
- 카드 이미지
- 카드 설명
- 자산군 태그
- 일반 태그
- 희귀도
- 현금 비용
- 전문 자원 비용
- 운용가치
- 인컴
```

구현상 추천 필드는 다음과 같다.

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

---

## 4. 카드 ID

카드 ID는 데이터 테이블에서 카드를 식별하기 위한 고유값이다.

```text
예:
asset_growth_tech_001
asset_bond_safe_001
asset_commodity_oil_001
```

카드 ID는 런 중 동일 카드의 중복 생성 여부, 보유 상태, 시장 슬롯 상태 등을 추적하는 기준이 된다.

---

## 5. 카드 이름

카드 이름은 유저-facing 텍스트이다.

예시:

```text
성장 기술주
국채 ETF
원자재 펀드
리츠 포트폴리오
사모 크레딧
```

카드 이름은 카드 상세보기와 보유 자산 목록에서 사용한다.

---

## 6. 카드 이미지

카드 이미지는 카드의 시각적 정체성을 표현한다.

MVP에서는 임시 이미지 또는 플레이스홀더를 사용할 수 있다.  
구현상 카드 데이터에는 이미지 리소스 경로를 둔다.

```csharp
public string ImageResourcePath;
```

카드 상세보기에서는 카드 이미지를 크게 표시한다.  
시장 테이프에서는 카드 이미지 또는 축약 카드 UI를 사용한다.

---

## 7. 희귀도

카드는 희귀도를 가진다.

희귀도는 다음 용도로 사용된다.

```text
- 카드의 가치 체감
- 카드 정렬
- 카드 테이블 밸런싱
- 카드 UI 연출
```

희귀도 enum 예시:

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

희귀도 단계명은 최종 UI 톤에 맞춰 변경할 수 있다.

---

## 8. 운용가치

운용가치는 자산 카드가 가진 고유 점수이다.

```text
운용가치
= 스플렌더의 승점과 같은 개념
```

최종 평가는 보유 자산 카드들의 운용가치 합계로 산정한다.

```text
최종 운용가치
= 보유 자산 카드 운용가치 합계
```

운용가치에 포함되지 않는 것:

```text
- 현금
- 리서치
- 신용
- 원자재
- 딜
- 예약 카드
- 시장 카드
```

예약 카드는 아직 매수한 자산이 아니므로 운용가치에 포함하지 않는다.  
시장 카드도 당연히 운용가치에 포함하지 않는다.

---

## 9. 인컴

자산 카드는 영업일 시작 시 현금 인컴을 제공할 수 있다.

```text
영업일 시작
→ 보유 자산 인컴 발생
```

인컴으로 얻은 현금은 운용 수익으로 기록한다.

```text
자산 인컴
→ CurrentCash 증가
→ CurrentQuarterEarnedCash 증가
→ CurrentFiscalYearEarnedCash 증가
→ TotalEarnedCash 증가
```

인컴은 매수 즉시 발생하지 않는다.

```text
자산 매수
→ 해당 영업일에는 인컴 없음
→ 다음 영업일 시작부터 인컴 발생
```

구현 필드:

```csharp
public int IncomeCash;
```

인컴이 없는 자산은 0으로 둔다.

---

## 10. 현금 비용

현금 비용은 자산 매수 시 지불해야 하는 기본 현금 비용이다.

```csharp
public int CashCost;
```

현금 비용은 매수 확정 시 차감된다.

```text
매수 확정
→ 최종 현금 비용 계산
→ CurrentCash 차감
```

최종 현금 비용은 딜 칩과 인플레이션 규칙을 반영해 계산한다.

```text
기본 현금 비용
→ 딜 할인 적용
→ 인플레이션 적용
→ 최종 현금 비용
```

딜 칩 1개는 기본 현금 비용을 1 낮춘다.

---

## 11. 전문 자원 비용

자산 카드는 전문 자원 비용을 가질 수 있다.

전문 자원 비용은 다음 3종으로 구성된다.

```text
- 리서치 비용
- 신용 비용
- 원자재 비용
```

구현 구조 예시:

```csharp
public class ProfessionalResourceCost
{
    public int Research;
    public int Credit;
    public int Commodity;
}
```

전문 자원 비용이 없는 자산은 세 값이 모두 0이다.

```text
리서치 0
신용 0
원자재 0
```

전문 자원 비용이 존재하는 자산은 카드 상세보기에서 비용 슬롯을 표시한다.

```text
리서치 2
신용 1
→ 리서치 슬롯 2개
→ 신용 슬롯 1개
```

---

## 12. 비용 슬롯

전문 자원 비용은 카드 상세보기에서 슬롯으로 표현한다.

```text
리서치 비용 2
→ 리서치 슬롯 2개

신용 비용 1
→ 신용 슬롯 1개
```

각 슬롯은 다음 상태를 가진다.

```text
- 비어 있음
- 해당 전문 자원 칩 배치됨
- 딜 칩 배치됨
```

슬롯에 딜 칩이 배치되면 해당 슬롯은 결제 완료로 본다.

```text
딜 칩이 배치된 슬롯
= 결제 완료
```

---

## 13. 태그 구조

자산 카드는 태그를 가진다.

태그는 크게 다음 두 종류로 나눈다.

```text
- 자산군 태그
- 일반 태그
```

자산군 태그는 보유 자산을 큰 카테고리로 분류하는 용도이다.

예시:

```text
주식
채권
원자재
부동산
대체투자
```

일반 태그는 정산, 효과, 시장 국면, 시너지 등에 사용한다.

예시:

```text
성장
방어
인컴
변동성
친환경
기술
금리민감
```

구현상 태그는 ID로 관리한다.

```csharp
public List<string> AssetClassTagIds;
public List<string> TagIds;
```

---

## 14. 태그 데이터

태그는 별도 데이터 테이블로 관리한다.

```csharp
public class TagData
{
    public string Id;
    public string DisplayName;
    public TagType Type;
    public int SortOrder;
}
```

태그 타입:

```csharp
public enum TagType
{
    AssetClass,
    General
}
```

태그 정렬은 `sort_order`를 기준으로 한다.

```text
sort_order 오름차순
→ 동률이면 id 오름차순
```

---

## 15. 자산군 태그 정렬

보유 자산 요약에서 자산군 태그는 태그 정의 테이블의 정렬값을 따른다.

정렬 규칙:

```text
1. 태그 테이블의 sort_order 오름차순
2. sort_order가 같으면 id 오름차순
```

예시:

```text
주식
채권
원자재
부동산
대체투자
```

이 순서는 하드코딩하지 않고 태그 테이블에서 관리한다.

---

## 16. 보유 자산 정렬

보유 자산은 다음 기준으로 정렬한다.

```text
1. 자산군 태그별로 묶는다.
2. 각 묶음 내부에서는 희귀도 순으로 정렬한다.
3. 희귀도가 같으면 먼저 산 자산을 위로 올린다.
```

즉:

```text
자산군 태그
→ 희귀도
→ 획득 순서
```

획득 순서는 자산을 매수한 시점에 부여한다.

```csharp
public int AcquiredOrder;
```

획득 순서는 오름차순으로 정렬한다.

```text
먼저 산 자산
→ 위쪽 표시
```

---

## 17. 카드 출처

카드는 현재 위치에 따라 출처를 가진다.

```csharp
public enum PurchaseSource
{
    MarketTape,
    Reserved
}
```

시장 카드와 예약 카드는 모두 매수할 수 있다.  
예약 카드 매수도 일반 자산 매수로 판정한다.

```text
예약 카드 매수
= 자산 매수
= OnBuyAsset 발동
```

단, 구매 출처는 별도로 남긴다.

```text
시장 카드 매수
→ PurchaseSource.MarketTape

예약 카드 매수
→ PurchaseSource.Reserved
```

구매 출처는 규칙 판정에 사용할 수 있다.

---

## 18. 카드 상태

자산 카드는 런 중 다음 상태 중 하나에 있을 수 있다.

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

상태 의미:

| 상태 | 의미 |
|---|---|
| Deck | 아직 시장에 나오지 않은 카드 |
| MarketTape | 시장 테이프에 표시 중인 카드 |
| Reserved | 예약 슬롯에 보관 중인 카드 |
| Owned | 플레이어가 매수 완료한 카드 |
| Removed | 시장에서 제거되었거나 더 이상 사용하지 않는 카드 |

---

## 19. 시장 카드

시장 테이프에 표시된 카드를 시장 카드라고 한다.

시장 카드의 가능 행동:

```text
- 카드 상세보기
- 매수
- 예약
```

시장 카드를 매수하면 해당 시장 슬롯만 새 카드로 보충한다.

```text
시장 카드 매수
→ 산 자리만 새 카드로 보충
→ 시장 테이프 진행 없음
```

시장 카드를 예약하면 다음 순서로 처리한다.

```text
시장 카드 예약
→ 예약 슬롯으로 이동
→ 예약한 자리 새 카드 보충
→ 시장 테이프 진행
→ 딜 +1
→ 환매 압력 +1
→ 영업일 종료
```

---

## 20. 예약 카드

예약 카드는 시장에서 빼내 예약 슬롯에 장기 보관한 카드이다.

예약 카드의 가능 행동:

```text
- 카드 상세보기
- 매수
```

예약 카드에는 예약 버튼을 표시하지 않는다.

```text
예약 카드 상세보기
→ 예약 버튼 숨김
```

예약 카드를 매수하면 예약 슬롯만 비운다.

```text
예약 카드 매수
→ 예약 슬롯 비움
→ 시장 테이프 변화 없음
```

예약 카드는 분기와 회계년도 전환 후에도 유지된다.

```text
분기 전환
→ 예약 카드 유지

회계년도 전환
→ 예약 카드 유지
```

예약 카드는 매수 전까지 운용가치에 포함되지 않는다.

---

## 21. 보유 자산

매수 완료된 카드는 보유 자산이 된다.

보유 자산의 역할:

```text
- 운용가치 합산
- 영업일 시작 인컴 발생
- 분기 마감 정산 대상
- 태그 기반 효과 조건
```

보유 자산은 시장에 다시 등장하지 않는다.

```text
보유 중인 자산
→ 시장 재등장 불가
```

MVP에서는 보유 자산 매각 기능은 다루지 않는다.

---

## 22. 카드 상세보기 표시 정보

카드 상세보기에는 다음 정보를 표시한다.

```text
- 카드 이미지
- 카드 이름
- 카드 설명
- 운용가치
- 인컴
- 현금 비용
- 전문 자원 비용
- 태그
- 희귀도
- 닫기 버튼
- 매수 버튼
- 예약 버튼
```

예약 버튼은 시장 카드 상세보기에서만 표시한다.

카드 상세보기는 시장 영역을 대체한다.

---

## 23. 카드 매수 처리와 카드 데이터

매수 확정 시 다음 정보를 사용한다.

```text
- CashCost
- ProfessionalCost
- ManagementValue
- IncomeCash
- TagIds
- AssetClassTagIds
- PurchaseSource
```

매수 확정 후 처리:

```text
자원 소비
→ 현금 차감
→ 카드 상태 Owned로 변경
→ 보유 자산 목록에 추가
→ AcquiredOrder 부여
→ 시장 카드면 산 자리 보충
→ 예약 카드면 예약 슬롯 비움
```

---

## 24. 카드 예약 처리와 카드 데이터

예약은 시장 카드에만 가능하다.

예약 처리:

```text
시장 카드 선택
→ 예약 버튼 클릭
→ 카드 상태 Reserved로 변경
→ 예약 슬롯에 배치
→ 예약한 시장 슬롯 새 카드로 보충
→ 시장 테이프 진행
→ 딜 +1
→ 환매 압력 +1
→ 영업일 종료
```

예약 슬롯이 가득 차면 예약 버튼은 비활성화한다.

```text
예약 슬롯 3 / 3
→ 예약 버튼 비활성화
```

---

## 25. 운용가치와 최종 평가

최종 평가 기준은 운용가치이다.

```text
최종 운용가치
= 보유 자산 카드들의 ManagementValue 합계
```

최종 평가 테이블은 다음 필드를 사용한다.

```csharp
public class FinalRatingData
{
    public string Id;
    public string Grade;
    public string Title;
    public int MinManagementValue;
}
```

초기 임시 기준:

| 등급 | 칭호 | 임시 운용가치 기준 |
|---|---|---:|
| D | 생존한 운용자 | 0+ |
| C | 신중한 운용자 | 50+ |
| B | 유능한 펀드매니저 | 100+ |
| A | 스타 펀드매니저 | 150+ |
| S | 전설적인 펀드매니저 | 200+ |

기준값은 밸런스 테이블에서 조정 가능하게 둔다.

---

## 26. 데이터 구조 예시

### 26.1 AssetCardData

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

---

### 26.2 AssetCardRuntimeData

```csharp
public class AssetCardRuntimeData
{
    public AssetCardData BaseData;

    public AssetCardRuntimeState RuntimeState;

    public PurchaseSource? PurchaseSource;

    public int? MarketTapeZoneIndex;
    public int? MarketSlotIndex;
    public int? ReservedSlotIndex;

    public int? AcquiredOrder;
}
```

---

### 26.3 ProfessionalResourceCost

```csharp
public class ProfessionalResourceCost
{
    public int Research;
    public int Credit;
    public int Commodity;
}
```

---

### 26.4 TagData

```csharp
public class TagData
{
    public string Id;
    public string DisplayName;
    public TagType Type;
    public int SortOrder;
}
```

---

### 26.5 TagType

```csharp
public enum TagType
{
    AssetClass,
    General
}
```

---

## 27. 구현 함수 예시

### 27.1 운용가치 계산

```csharp
int CalculateTotalManagementValue()
{
    return OwnedAssets.Sum(asset =>
        asset.BaseData.ManagementValue
    );
}
```

---

### 27.2 보유 자산 수

```csharp
int GetOwnedAssetCount()
{
    return OwnedAssets.Count;
}
```

---

### 27.3 보유 자산 인컴 적용

```csharp
void ApplyOwnedAssetIncome()
{
    int totalIncome = OwnedAssets.Sum(asset =>
        asset.BaseData.IncomeCash
    );

    AddPerformanceCash(totalIncome);
}
```

---

### 27.4 보유 자산 정렬

```csharp
IEnumerable<AssetCardRuntimeData> SortOwnedAssets(
    IEnumerable<AssetCardRuntimeData> assets
)
{
    return assets
        .OrderBy(asset => GetPrimaryAssetClassSortOrder(asset))
        .ThenBy(asset => GetRaritySortOrder(asset.BaseData.Rarity))
        .ThenBy(asset => asset.AcquiredOrder);
}
```

---

## 28. 구현 시 주의사항

```text
- AUM이라는 용어를 사용하지 않는다.
- 카드 점수 개념은 운용가치로 통일한다.
- 운용가치는 현금과 무관하다.
- 예약 카드는 운용가치에 포함하지 않는다.
- 시장 카드는 운용가치에 포함하지 않는다.
- 보유 자산만 운용가치 합산 대상이다.
- 예약 카드 매수도 일반 자산 매수로 판정한다.
- 구매 출처는 별도로 기록한다.
- 보유 자산은 시장에 다시 등장하지 않는다.
- 자산군 태그 정렬은 태그 테이블의 sort_order를 따른다.
- sort_order 동률이면 id 오름차순으로 정렬한다.
- 보유 자산 내부 정렬은 자산군 태그 → 희귀도 → 획득 순서이다.
```
