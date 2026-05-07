# 18. 회계년도 전환과 4Q 휴가

## 1. 목적

이 문서는 회계년도 전환 구조와 1·2회계년도 4Q 휴가 화면의 역할, 표시 정보, 처리 순서를 정의한다.

본 게임은 총 3개의 회계년도로 구성된다.

```text
1회계년도
2회계년도
3회계년도
```

1·2회계년도는 4Q를 플레이하지 않고 휴가 화면으로 처리한다.  
3회계년도는 마지막 해이므로 4Q까지 모두 플레이한다.

---

## 2. 회계년도 구성

전체 회계년도 구성은 다음과 같다.

```text
1회계년도
- 1Q: 플레이
- 2Q: 플레이
- 3Q: 플레이
- 4Q: 휴가

2회계년도
- 1Q: 플레이
- 2Q: 플레이
- 3Q: 플레이
- 4Q: 휴가

3회계년도
- 1Q: 플레이
- 2Q: 플레이
- 3Q: 플레이
- 4Q: 플레이
```

1·2회계년도의 4Q는 플레이 가능한 분기가 아니다.  
3회계년도의 4Q는 플레이 가능한 분기이다.

---

## 3. 4Q 휴가의 역할

4Q 휴가는 플레이 구간이 아니다.  
회계년도 전환을 위한 요약 화면이다.

4Q 휴가의 역할:

```text
- 한 해 운용 마무리 감각 제공
- 해당 회계년도 운용 결과 요약
- 다음 회계년도 진입 전 호흡 제공
```

4Q 휴가는 보상 화면이 아니다.

```text
보너스 없음
패널티 없음
선택지 없음
```

---

## 4. 4Q 휴가에서 하지 않는 것

4Q 휴가에서는 다음 처리를 하지 않는다.

```text
- 영업일 진행
- 자산 매수
- 카드 예약
- 유동성 확보
- 다음 영업일
- 분기 마감 시나리오
- 시장 테이프 진행
- 시장 테이프 갱신
- 보상 지급
- 패널티 적용
```

4Q 휴가는 순수하게 회계년도 요약과 전환을 담당한다.

---

## 5. 4Q 휴가 진입 조건

1·2회계년도 3Q 마감 후 4Q 휴가 화면으로 진입한다.

```text
1회계년도 3Q 마감
→ 1회계년도 4Q 휴가 화면
```

```text
2회계년도 3Q 마감
→ 2회계년도 4Q 휴가 화면
```

3회계년도에는 4Q 휴가가 없다.

```text
3회계년도 3Q 마감
→ 3회계년도 4Q 시작
```

3회계년도 4Q 마감 후에는 최종 정산으로 이동한다.

```text
3회계년도 4Q 마감
→ 최종 정산
```

---

## 6. 4Q 휴가 화면 표시 정보

4Q 휴가 화면에는 해당 회계년도의 운용 요약을 표시한다.

표시 항목:

```text
- 회계년도 / 4Q 휴가 타이틀
- 짧은 휴가 문구
- 현재 운용가치
- 올해 운용 수익 총합
- 분기별 운용 수익
- 보유 자산 수
- 환매 압력
- 다음 회계년도 시작 버튼
```

예시:

```text
1회계년도 4Q · 휴가

한 해 운용을 마치고 휴가를 떠납니다.

올해 운용 요약
- 현재 운용가치: 42
- 올해 운용 수익: 18
  · 1Q +4
  · 2Q +6
  · 3Q +8
- 보유 자산: 7장
- 환매 압력: 3 / 10

[2회계년도 시작]
```

---

## 7. 현재 운용가치

4Q 휴가 화면의 현재 운용가치는 해당 시점의 보유 자산 운용가치 합계이다.

```text
현재 운용가치
= 보유 자산 카드들의 운용가치 합계
```

운용가치에 포함하지 않는 것:

```text
- 현금
- 리서치
- 신용
- 원자재
- 딜
- 예약 카드
- 시장 카드
```

예약 카드는 매수 전까지 운용가치에 포함하지 않는다.

---

## 8. 올해 운용 수익

4Q 휴가 화면의 `올해 운용 수익`은 해당 회계년도 플레이 분기에서 발생한 운용 수익의 합계이다.

1·2회계년도는 1Q~3Q만 플레이하므로, 3개 분기의 운용 수익을 합산한다.

```text
1회계년도 운용 수익
= 1Q 운용 수익
+ 2Q 운용 수익
+ 3Q 운용 수익
```

```text
2회계년도 운용 수익
= 1Q 운용 수익
+ 2Q 운용 수익
+ 3Q 운용 수익
```

4Q는 휴가이므로 운용 수익이 없다.

---

## 9. 운용 수익 포함/제외 기준

올해 운용 수익에는 운용 성과 현금만 포함한다.

포함:

```text
- 자산 인컴
- 분기 마감 정산 수익
- 기타 운용 성과로 분류된 현금
```

제외:

```text
- 유동성 확보로 얻은 현금
```

유동성 확보 현금은 조달 현금이다.

```text
유동성 확보 현금
= 조달 현금
= 올해 운용 수익에 포함하지 않음
```

---

## 10. 분기별 운용 수익 표시

4Q 휴가 화면에는 올해 운용 수익 총합과 함께 분기별 운용 수익을 표시한다.

예시:

```text
올해 운용 수익: 18
- 1Q +4
- 2Q +6
- 3Q +8
```

1·2회계년도는 플레이 분기가 3개이므로 1Q~3Q만 표시한다.

```text
4Q
→ 휴가
→ 운용 수익 표시 없음
```

분기별 운용 수익은 해당 분기 마감 시 확정된 값을 사용한다.

---

## 11. 보유 자산 수

보유 자산 수는 현재 플레이어가 매수 완료한 자산 카드 수이다.

```text
보유 자산 수
= Owned 상태의 자산 카드 수
```

예약 카드는 보유 자산 수에 포함하지 않는다.

```text
예약 카드
→ 보유 자산 수 제외
```

---

## 12. 환매 압력 표시

4Q 휴가 화면에는 현재 환매 압력을 표시한다.

예시:

```text
환매 압력: 3 / 10
```

환매 압력이 10 이상이면 이미 런 실패가 발생했어야 하므로, 4Q 휴가 화면에 진입할 수 없다.

```text
환매 압력 10 이상
→ 런 실패
→ 휴가 화면 진입 없음
```

---

## 13. 휴가 화면 버튼

1회계년도 4Q 휴가 화면 버튼:

```text
[2회계년도 시작]
```

2회계년도 4Q 휴가 화면 버튼:

```text
[3회계년도 시작]
```

버튼 클릭 시 다음 회계년도를 시작한다.

```text
휴가 화면 버튼 클릭
→ 다음 회계년도 시작
```

---

## 14. 휴가 이후 회계년도 시작

휴가 화면에서 다음 회계년도 시작 버튼을 누르면 다음 처리를 수행한다.

```text
1. 현재 회계년도 +1
2. 현재 분기 = 1Q
3. 시장 테이프 갱신
4. 1Q 시작
5. 1영업일 시작
6. 보유 자산 인컴 발생
```

시장 테이프는 진행이 아니라 갱신한다.

```text
회계년도 시작
→ 시장 테이프 갱신
```

시장 테이프 갱신은 기존 시장 테이프 카드들을 모두 제거하고 새 카드로 재구성하는 처리이다.

---

## 15. 회계년도 시작 시 시장 테이프 갱신

회계년도 시작 시에는 시장 테이프를 갱신한다.

```text
기존 시장 테이프 카드 전부 제거
→ 매도 임박 슬롯 새 카드로 채움
→ 현재 시장 슬롯 새 카드로 채움
→ 예비 시장 슬롯 새 카드로 채움
```

적용 시점:

```text
1회계년도 시작
2회계년도 시작
3회계년도 시작
```

예약 카드는 시장 테이프 갱신의 영향을 받지 않는다.

```text
시장 테이프 갱신
→ 예약 슬롯 유지
```

---

## 16. 예약 카드 유지

예약 카드는 회계년도 전환 후에도 유지된다.

```text
1회계년도 4Q 휴가
→ 2회계년도 시작
→ 예약 카드 유지
```

```text
2회계년도 4Q 휴가
→ 3회계년도 시작
→ 예약 카드 유지
```

예약 카드는 장기 보관 가능한 카드이다.  
플레이어가 직접 매수하기 전까지 사라지지 않는다.

---

## 17. 회계년도 운용 수익 초기화

회계년도가 시작되면 해당 회계년도의 운용 수익 기록을 0으로 초기화한다.

```text
새 회계년도 시작
→ CurrentFiscalYearEarnedCash = 0
```

총 운용 수익은 초기화하지 않는다.

```text
TotalEarnedCash
→ 런 전체 누적
→ 유지
```

분기 운용 수익은 각 분기 시작 시 초기화한다.

```text
새 분기 시작
→ CurrentQuarterEarnedCash = 0
```

---

## 18. 휴가 화면 데이터

4Q 휴가 화면에 필요한 데이터 구조 예시:

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

분기별 운용 수익 데이터:

```csharp
public class QuarterEarnedCashData
{
    public int FiscalYearIndex;
    public int QuarterIndex;
    public int EarnedCash;
}
```

---

## 19. 휴가 화면 데이터 생성

```csharp
FiscalYearSummaryData BuildFiscalYearSummary()
{
    return new FiscalYearSummaryData
    {
        FiscalYearIndex = CurrentFiscalYear,

        CurrentManagementValue = CalculateTotalManagementValue(),
        FiscalYearEarnedCash = CurrentFiscalYearEarnedCash,

        QuarterEarnedCashList =
            GetQuarterEarnedCashListForFiscalYear(CurrentFiscalYear),

        OwnedAssetCount = OwnedAssets.Count,
        CurrentRedemptionPressure = CurrentRedemptionPressure
    };
}
```

---

## 20. 휴가 화면 진입 예시

```csharp
void ShowVacationQuarterScreen()
{
    FiscalYearSummaryData summary = BuildFiscalYearSummary();

    CurrentGamePhase = GamePhase.VacationQuarter;

    VacationQuarterView.Show(summary);
}
```

---

## 21. 휴가 이후 계속 버튼 예시

```csharp
void OnVacationContinueClicked()
{
    int nextFiscalYear = CurrentFiscalYear + 1;

    StartFiscalYear(nextFiscalYear);
}
```

---

## 22. 회계년도 시작 함수 예시

```csharp
void StartFiscalYear(int fiscalYear)
{
    CurrentFiscalYear = fiscalYear;
    CurrentQuarter = 1;

    CurrentFiscalYearEarnedCash = 0;

    RefreshMarketTape();

    StartQuarter(CurrentFiscalYear, CurrentQuarter);
}
```

---

## 23. 분기 이후 진행 예시

```csharp
void ProceedAfterQuarterEnd()
{
    if (CurrentFiscalYear == 3 && CurrentQuarter == 4)
    {
        ShowFinalSettlement();
        return;
    }

    int nextQuarter = CurrentQuarter + 1;

    if (IsVacationQuarter(CurrentFiscalYear, nextQuarter))
    {
        ShowVacationQuarterScreen();
        return;
    }

    CurrentQuarter = nextQuarter;

    AdvanceMarketTape();

    StartQuarter(CurrentFiscalYear, CurrentQuarter);
}
```

---

## 24. 휴가 분기 판정

```csharp
bool IsVacationQuarter(int fiscalYear, int quarter)
{
    return (fiscalYear == 1 || fiscalYear == 2)
        && quarter == 4;
}
```

3회계년도 4Q는 휴가가 아니다.

```text
IsVacationQuarter(3, 4)
→ false
```

---

## 25. 구현 시 주의사항

```text
- 1·2회계년도 4Q는 플레이하지 않는다.
- 1·2회계년도 4Q는 휴가 화면으로 처리한다.
- 4Q 휴가에서는 보상이나 패널티를 주지 않는다.
- 4Q 휴가에서는 시장 테이프 진행/갱신을 하지 않는다.
- 휴가 이후 다음 회계년도 시작 시 시장 테이프를 갱신한다.
- 회계년도 시작 시 시장 테이프는 진행이 아니라 갱신이다.
- 예약 카드는 회계년도 전환 후에도 유지된다.
- 휴가 화면의 올해 운용 수익은 운용 성과 현금만 포함한다.
- 유동성 확보 현금은 올해 운용 수익에 포함하지 않는다.
- 휴가 화면에는 총합과 분기별 운용 수익을 함께 표시한다.
- 3회계년도 4Q 이후에는 휴가가 아니라 최종 정산으로 이동한다.
