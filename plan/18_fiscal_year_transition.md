# 18. 회계연도 전환과 4Q 휴가

## 1. 목적

이 문서는 회계연도 전환 구조와 1, 2회계연도 4Q 휴가 화면의 역할, 표시 정보, 처리 순서를 정의한다.

게임은 총 3개의 회계연도로 구성된다.

```text
1회계연도
2회계연도
3회계연도
```

1, 2회계연도는 4Q를 플레이하지 않고 휴가 화면으로 처리한다.
3회계연도는 마지막 회계연도이므로 4Q까지 플레이한다.

---

## 2. 회계연도 구성

전체 회계연도 구성은 다음과 같다.

```text
1회계연도
- 1Q: 플레이
- 2Q: 플레이
- 3Q: 플레이
- 4Q: 휴가

2회계연도
- 1Q: 플레이
- 2Q: 플레이
- 3Q: 플레이
- 4Q: 휴가

3회계연도
- 1Q: 플레이
- 2Q: 플레이
- 3Q: 플레이
- 4Q: 플레이
```

1, 2회계연도의 4Q는 플레이 가능한 분기가 아니다.
3회계연도의 4Q는 플레이 가능한 분기다.

---

## 3. 4Q 휴가의 역할

4Q 휴가는 플레이 구간이 아니라 회계연도 전환을 위한 요약 화면이다.

역할:

```text
- 이번 회계연도 성과 마무리
- 회계연도 수익 요약
- 회계연도 미션 수익 요약
- 현재 포트폴리오 상태 요약
- 다음 회계연도 진입 전 완충 제공
```

4Q 휴가는 보상 화면이 아니다.

```text
보너스 없음
패널티 없음
선택지 없음
```

---

## 4. 4Q 휴가에서 하지 않는 처리

4Q 휴가에서는 다음 처리를 하지 않는다.

```text
- 영업일 진행
- 주식 매수
- 주식 예약
- 주식 매도
- 소모형 자원 카드 매수
- 다음 영업일
- 분기 마감 시나리오
- 시장 테이프 진행
- 시장 테이프 갱신
- 배당금 지급
- 보상 지급
- 패널티 적용
```

4Q 휴가는 순수하게 회계연도 요약과 전환을 담당한다.

---

## 5. 진입 조건

1, 2회계연도 3Q 마감 뒤 4Q 휴가 화면으로 진입한다.

```text
1회계연도 3Q 마감
-> 1회계연도 4Q 휴가 화면
```

```text
2회계연도 3Q 마감
-> 2회계연도 4Q 휴가 화면
```

3회계연도에는 4Q 휴가가 없다.

```text
3회계연도 3Q 마감
-> 3회계연도 4Q 시작
```

3회계연도 4Q 마감 뒤에는 최종 정산으로 이동한다.

```text
3회계연도 4Q 마감
-> 최종 정산
```

---

## 6. 표시 정보

4Q 휴가 화면에는 해당 회계연도의 요약 정보를 표시한다.

표시 항목:

```text
- 회계연도 / 4Q 휴가 타이틀
- 짧은 휴가 문구
- 현재 가치
- 올해 수익 총합
- 올해 미션 수익 총합
- 분기별 수익
- 보유 주식 수
- 월세 밀림
- 다음 회계연도 시작 버튼
```

예시:

```text
1회계연도 4Q 휴가

올해의 투자를 마치고 잠깐 숨을 고릅니다.

올해 수익 요약
- 현재 가치: 42
- 올해 수익: 18
  - 1Q +4
  - 2Q +6
  - 3Q +8
- 올해 미션 수익: +5
- 보유 주식: 7 / 8
- 월세 밀림: 3 / 10

[2회계연도 시작]
```

---

## 7. 현재 가치

4Q 휴가 화면의 현재 가치는 해당 시점에 보유한 주식 가치의 합계다.

```text
현재 가치 = 보유 주식의 가치 합계
```

가치에 포함되는 것:

```text
- 보유 중인 일반 주식의 가치
- 보유 중인 호일 주식의 가치
```

가치에 포함되지 않는 것:

```text
- 현금
- 독서
- 명상
- 인내
- 딜
- 시장 카드
- 예약된 주식
```

예약된 주식은 아직 보유 주식이 아니므로 가치에 포함하지 않는다.

---

## 8. 올해 수익

올해 수익은 해당 회계연도에서 플레이한 분기의 수익 합계다.

1, 2회계연도는 1Q부터 3Q까지만 플레이하므로 3개 분기의 수익을 합산한다.

```text
1회계연도 수익
= 1Q 수익
+ 2Q 수익
+ 3Q 수익
```

```text
2회계연도 수익
= 1Q 수익
+ 2Q 수익
+ 3Q 수익
```

4Q는 휴가이므로 수익이 없다.

올해 수익에 포함되는 항목:

```text
- 배당금
- 주식 매도 수익
- 그 밖에 수익으로 명시된 현금
```

미션 수익은 현금이 아니므로 올해 수익에 더하지 않는다.
필요하면 별도 `올해 미션 수익` 항목으로 표시한다.

올해 수익에 포함되지 않는 항목:

```text
- 소모형 자원 카드로 획득한 현금
- 시작 현금
- 단순 환급 또는 비용 취소
```

---

## 9. 보유 주식 수

보유 주식 수는 현재 포트폴리오 슬롯을 점유한 주식 수다.

```text
보유 주식 수 = 포트폴리오에서 점유된 슬롯 수
```

일반 주식 1장은 1칸을 차지한다.
호일 주식 1장은 1칸을 차지한다.

```text
보유 주식: 7 / 8
```

예약된 주식은 보유 주식 수에 포함하지 않는다.

---

## 10. 월세 밀림 표시

4Q 휴가 화면에는 현재 월세 밀림을 표시한다.

```text
월세 밀림: 3 / 10
```

월세 밀림이 10 이상이면 이미 파산이 발생했으므로 4Q 휴가 화면에 진입할 수 없다.

```text
월세 밀림 10 이상
-> 파산
-> 휴가 화면 진입 없음
```

---

## 11. 다음 회계연도 시작

휴가 화면에서 다음 회계연도 시작 버튼을 누르면 다음 처리를 수행한다.

```text
1. 현재 회계연도 +1
2. 현재 분기 = 1Q
3. 회계연도 수익 초기화
4. 시장 테이프 갱신
5. 1Q 시작
6. 첫 영업일 시작
7. 보유 주식의 배당금 지급
```

새 회계연도 시작 시 시장 테이프는 진행이 아니라 갱신이다.

```text
회계연도 시작
-> 시장 테이프 갱신
```

갱신은 기존 시장 카드 중 예약되지 않은 카드를 교체하는 처리다.
예약된 주식은 갱신의 영향을 받지 않는다.

---

## 12. 시장 테이프 갱신

회계연도 시작 시 시장 테이프를 갱신한다.

```text
기존 시장 카드 확인
-> 예약된 주식은 유지
-> 예약되지 않은 슬롯은 새 카드로 교체
```

갱신 시 75% 확률로 주식 덱, 25% 확률로 소모형 자원 덱에서 카드를 뽑는다.
선택한 덱이 비어 있으면 반대 덱으로 fallback한다.

---

## 13. 수익 초기화

회계연도가 시작되면 해당 회계연도의 수익 기록을 0으로 초기화한다.

```text
새 회계연도 시작
-> CurrentFiscalYearRevenue = 0
```

총 수익은 초기화하지 않는다.

```text
TotalRevenue
-> 전체 런 누적
-> 유지
```

분기 현금 흐름과 미션 수익은 각 분기 시작 시 초기화한다.

```text
새 분기 시작
-> CurrentQuarterRevenue = 0
```

---

## 14. 상태 데이터 예시

```csharp
public class FiscalYearSummaryData
{
    public int FiscalYearIndex;

    public int CurrentValue;
    public int FiscalYearRevenue;
    public int FiscalYearMissionRevenue;

    public List<QuarterRevenueData> QuarterRevenueList;

    public int OwnedStockSlotCount;
    public int MaxStockSlotCount;
    public int CurrentRentArrears;
}
```

분기별 수익 데이터:

```csharp
public class QuarterRevenueData
{
    public int FiscalYearIndex;
    public int QuarterIndex;
    public int Revenue;
}
```

---

## 15. 처리 예시

```csharp
FiscalYearSummaryData BuildFiscalYearSummary()
{
    return new FiscalYearSummaryData
    {
        FiscalYearIndex = CurrentFiscalYear,

        CurrentValue = CalculateTotalStockValue(),
        FiscalYearRevenue = CurrentFiscalYearRevenue,

        QuarterRevenueList =
            GetQuarterRevenueListForFiscalYear(CurrentFiscalYear),

        OwnedStockSlotCount = Portfolio.OccupiedSlotCount,
        MaxStockSlotCount = Portfolio.MaxSlotCount,
        CurrentRentArrears = CurrentRentArrears
    };
}
```

```csharp
void ShowVacationQuarterScreen()
{
    FiscalYearSummaryData summary = BuildFiscalYearSummary();

    CurrentGamePhase = GamePhase.VacationQuarter;

    VacationQuarterView.Show(summary);
}
```

```csharp
void StartFiscalYear(int fiscalYear)
{
    CurrentFiscalYear = fiscalYear;
    CurrentQuarter = 1;

    CurrentFiscalYearRevenue = 0;

    RefreshMarketTape();

    StartQuarter(CurrentFiscalYear, CurrentQuarter);
}
```

---

## 16. 구현 시 주의사항

```text
- 1, 2회계연도의 4Q는 플레이하지 않는다.
- 1, 2회계연도의 4Q는 휴가 화면으로 처리한다.
- 4Q 휴가에서 보상이나 패널티를 주지 않는다.
- 4Q 휴가에서 시장 테이프를 진행하거나 갱신하지 않는다.
- 휴가 이후 다음 회계연도 시작 시 시장 테이프를 갱신한다.
- 시장 테이프 갱신은 예약된 주식을 유지한다.
- 휴가 화면의 올해 수익은 수익으로 분류된 현금만 포함한다.
- 소모형 자원 카드로 획득한 현금은 올해 수익에 포함하지 않는다.
- 3회계연도 4Q 이후에는 휴가가 아니라 최종 정산으로 이동한다.
```
