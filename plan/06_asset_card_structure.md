# 06. 주식 카드 구조

## 1. 목적

이 문서는 주식 카드와 소모형 자원 카드의 데이터 구조, 런타임 상태, 포트폴리오/호일 규칙을 정의한다.

이 게임의 포트폴리오에는 주식만 들어간다.
채권, 원자재, 부동산 등 다른 자산군은 사용하지 않는다.

---

## 2. 카드 종류

시장에 등장하는 카드는 두 종류이다.

```text
1. 주식 카드
2. 소모형 자원 카드
```

주식 카드는 포트폴리오에 등록되는 카드이다.
소모형 자원 카드는 매수 시 효과를 발동하고 사라지는 카드이다.

---

## 3. 주식 카드 표시 정보

주식 카드는 다음 정보를 표시한다.

```text
- 이미지
- 이름
- 등급
- 가치
- 코스트
- 배당금
```

시장에 있을 때와 호버 확대 상태에서 제공하는 정보는 동일하다.
스케일만 다르다.

---

## 4. 소모형 자원 카드 표시 정보

소모형 자원 카드는 다음 정보를 표시한다.

```text
- 이미지
- 등급
- 제공 자원
- 코스트
```

소모형 자원 카드에는 이름을 표시하지 않는다.
소모형 자원 카드의 등급은 효율과 출현 빈도를 나타낸다.

소모형 자원 카드는 다음 값을 가지지 않는다.

```text
- 가치
- 배당금
- 호일 상태
- 포트폴리오 슬롯
```

---

## 5. 주식 기본 데이터

주식은 기본 상태와 호일 상태 수치를 모두 가진다.

```text
- 주식 ID
- 이름
- 이미지
- 등급
- 현금 비용
- 투자 철학 비용
- 기본 가치
- 기본 배당금
- 호일 가치
- 호일 배당금
- 덱 포함 장수 min
- 덱 포함 장수 max
```

호일 수치는 배율로 계산하지 않는다.
기획자가 직접 지정한다.

---

## 6. 가치

가치는 최종 평가의 기준이 되는 주식 카드 점수이다.

```text
최종 가치
= 보유 주식 가치 합계
```

포함:

```text
- 보유 중인 일반 주식의 기본 가치
- 보유 중인 호일 주식의 호일 가치
```

포함하지 않음:

```text
- 현금
- 독서 / 명상 / 인내
- 딜
- 시장에 있는 주식
- 예약된 주식
- 소모형 자원 카드
- 수익
```

---

## 7. 배당금과 수익

배당금은 보유 주식이 영업일 시작에 제공하는 현금이다.

```text
영업일 시작
→ 보유 주식 배당금 합산
→ 현금 증가
→ 분기 수익 증가
```

매수한 주식은 매수한 영업일에는 배당금을 지급하지 않는다.
다음 영업일부터 배당금을 지급한다.

```text
주식 매수
→ 해당 영업일 배당금 없음
→ 다음 영업일 시작부터 배당금 발생
```

동일 주식 2장을 보유 중이면 각 카드의 기본 배당금이 각각 발생한다.
호일로 합쳐진 뒤에는 호일 배당금만 발생한다.

---

## 8. 포트폴리오

포트폴리오는 최대 8칸이다.

```text
포트폴리오 최대 보유: 8주식
```

동일 주식 2장은 2칸을 차지한다.

```text
동일 주식 2장 보유
→ 포트폴리오 2칸 점유
→ 각 카드의 기본 가치 합산
→ 각 카드의 기본 배당금 발생
```

새 주식은 가장 왼쪽의 빈 포트폴리오 칸에 들어간다.

---

## 9. 포트폴리오 가득 참 처리

포트폴리오 8칸이 모두 점유된 상태에서 신규 주식 매수를 시도하면 실패한다.

```text
포트폴리오 8 / 8
동일 주식 2장 보유 상황이 아님
→ 매수 불가
→ "주식 매도가 필요합니다" 알림
→ 비용 소비 없음
→ 영업일 소비 없음
```

예외:

```text
포트폴리오 8 / 8
동일 주식 2장을 이미 보유
3번째 동일 주식 매수 시도
→ 비용 지불 가능하면 매수 허용
→ 즉시 호일 처리
→ 포트폴리오 점유 8칸 → 7칸
→ 영업일 종료
```

---

## 10. 호일 규칙

동일 주식 3장을 보유하면 즉시 1장 호일 주식으로 합쳐진다.

```text
동일 주식 3장 보유
→ 가장 먼저 보유한 동일 주식 칸이 호일 주식으로 변함
→ 나머지 동일 주식 2장은 제거
→ 포트폴리오 빈칸 2개는 그대로 빈칸으로 남김
```

호일 주식:

```text
- 등급은 기본 주식과 동일
- 가치는 호일 가치 사용
- 배당금은 호일 배당금 사용
- 무지개 호일 테두리로 표시
```

호일 완성 시 같은 종목은 이번 게임에서 더 이상 등장하지 않는다.

제거 대상:

```text
- 주식 덱의 같은 종목
- 시장의 같은 종목
- 예약된 같은 종목
```

시장 또는 예약된 같은 종목 제거로 생긴 빈칸은 시장 테이프 당김으로 처리한다.

---

## 11. 주식 종목 장수

각 주식 종목은 덱에 여러 장 포함될 수 있다.
종목별 포함 장수는 데이터 테이블의 min~max 값으로 컨트롤한다.

```text
StockId: calm_reader
MinCopiesInDeck: 2
MaxCopiesInDeck: 5
```

호일 주식은 별도 카드로 덱에 존재하지 않는다.
호일 주식은 동일 종목 3장 보유의 결과 상태이다.

---

## 12. 주식 매도

주식 매도는 영업일을 소비하지 않는다.
하루 횟수 제한이 없다.

매도 보상:

```text
일반 주식 매도
→ 현금 1 * 인플레이션 획득

호일 주식 매도
→ 현금 3 * 인플레이션 획득
```

매도 수익은 분기 수익에 포함한다.

매도한 주식은 이번 게임에서 제거된다.
덱/시장에 다시 등장하지 않는다.

---

## 13. 카드 상태

추천 런타임 상태:

```csharp
public enum MarketCardType
{
    Stock,
    ConsumableResource
}
```

```csharp
public enum StockRuntimeState
{
    StockDeck,
    Market,
    Owned,
    Removed
}
```

예약은 별도 상태나 구역이 아니라 시장 슬롯의 잠금 상태로 관리한다.

```csharp
public class MarketSlot
{
    public int SlotIndex;
    public MarketCardRuntimeData Card;
    public bool IsReserved;
}
```

---

## 14. 데이터 구조 제안

```csharp
public class InvestmentPhilosophyCost
{
    public int Reading;
    public int Meditation;
    public int Patience;
}
```

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

    public int MinCopiesInDeck;
    public int MaxCopiesInDeck;
}
```

```csharp
public class StockRuntimeData
{
    public StockCardData BaseData;
    public StockRuntimeState RuntimeState;

    public bool IsFoil;
    public int? MarketSlotIndex;
    public int? PortfolioSlotIndex;
    public int? AcquiredOrder;
}
```

---

## 15. 구현 시 주의사항

```text
- 포트폴리오에는 주식만 들어간다.
- 자산이라는 용어 대신 주식을 사용한다.
- 운용가치 대신 가치를 사용한다.
- 운용 수익은 배당금/수익으로 구분한다.
- 포트폴리오는 최대 8칸이다.
- 포트폴리오가 가득 찬 경우 기본적으로 신규 매수는 불가능하다.
- 동일 주식 2장 보유 상태에서 3번째 동일 주식 매수는 호일 예외로 허용한다.
- 동일 주식 3장은 즉시 1장 호일로 합쳐진다.
- 호일 수치는 기획자가 직접 지정한다.
- 호일 완성 시 같은 종목의 덱/시장/예약 주식을 제거한다.
- 소모형 자원 카드는 포트폴리오에 들어가지 않는다.
- 주식 매도는 영업일을 소비하지 않고 수익을 발생시킨다.
```
