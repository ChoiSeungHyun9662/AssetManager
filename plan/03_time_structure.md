# 03. 시간 구조

## 1. 목적

이 문서는 게임의 시간 단위와 진행 구조를 정의한다.

본 게임의 시간 구조는 다음 계층을 가진다.

```text
영업일 < 분기 < 회계년도
```

기존 게임 용어로는 다음과 같이 대응한다.

| 기존 개념 | 본 게임 표현 |
|---|---|
| 턴 | 영업일 |
| 스테이지 | 분기 |
| 런 챕터 / 연차 | 회계년도 |

유저-facing 텍스트에서는 `턴`, `스테이지`라는 표현을 사용하지 않는다.  
각각 `영업일`, `분기`로 표기한다.

---

## 2. 시간 단위 정의

### 2.1 영업일

영업일은 플레이어가 주요 행동 1회를 수행할 수 있는 최소 단위이다.

플레이어는 영업일 동안 보드 위 요소를 직접 조작한다.

```text
가능한 주요 행동:
- 자산 매수
- 카드 예약
- 유동성 확보
- 다음 영업일
```

행동이 확정되면 해당 영업일이 종료된다.

```text
행동 확정
→ 영업일 1일 소비
→ 다음 영업일 또는 분기 마감
```

---

### 2.2 분기

분기는 여러 영업일로 구성된 중간 진행 단위이다.

각 분기는 다음 요소를 가진다.

```text
- 회계년도 번호
- 분기 번호
- 분기당 영업일 수
- 분기 목표 운용 수익
- 분기 마감 시나리오 또는 시장 국면
- 분기 운용 수익 기록
```

분기 마지막 영업일이 종료되면 분기 마감으로 진입한다.

```text
마지막 영업일 종료
→ 분기 마감 정산
```

---

### 2.3 회계년도

회계년도는 여러 분기로 구성된 상위 진행 단위이다.

본 게임은 총 3개의 회계년도로 구성된다.

```text
1회계년도
2회계년도
3회계년도
```

1·2회계년도는 4Q가 휴가로 처리된다.  
3회계년도는 마지막 해이므로 4Q까지 모두 플레이한다.

---

## 3. 전체 회계년도 구성

전체 런의 회계년도 구성은 다음과 같다.

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

총 플레이 분기 수는 10개이다.

```text
1회계년도: 3개 분기
2회계년도: 3개 분기
3회계년도: 4개 분기

총 10개 분기
```

---

## 4. 분기당 영업일 수

분기당 영업일 수는 회계년도별로 다르다.

| 회계년도 | 플레이 분기 | 분기당 영업일 | 총 영업일 |
|---|---:|---:|---:|
| 1회계년도 | 1Q / 2Q / 3Q | 4영업일 | 12영업일 |
| 2회계년도 | 1Q / 2Q / 3Q | 4영업일 | 12영업일 |
| 3회계년도 | 1Q / 2Q / 3Q / 4Q | 5영업일 | 20영업일 |

전체 런의 총 플레이 영업일은 44일이다.

```text
12 + 12 + 20 = 44영업일
```

---

## 5. 회계년도별 세부 구조

### 5.1 1회계년도

```text
1회계년도
- 1Q: 4영업일
- 2Q: 4영업일
- 3Q: 4영업일
- 4Q: 휴가
```

1회계년도 3Q가 끝나면 4Q 휴가 화면을 표시한다.  
4Q 휴가 화면 이후 2회계년도 1Q로 넘어간다.

---

### 5.2 2회계년도

```text
2회계년도
- 1Q: 4영업일
- 2Q: 4영업일
- 3Q: 4영업일
- 4Q: 휴가
```

2회계년도 3Q가 끝나면 4Q 휴가 화면을 표시한다.  
4Q 휴가 화면 이후 3회계년도 1Q로 넘어간다.

---

### 5.3 3회계년도

```text
3회계년도
- 1Q: 5영업일
- 2Q: 5영업일
- 3Q: 5영업일
- 4Q: 5영업일
```

3회계년도는 마지막 해이므로 4Q 휴가가 없다.  
3회계년도 4Q가 끝나면 최종 정산으로 이동한다.

---

## 6. 4Q 휴가

1·2회계년도의 4Q는 플레이하지 않는다.

4Q 휴가에서는 다음 행동이 발생하지 않는다.

```text
- 자산 매수
- 카드 예약
- 유동성 확보
- 다음 영업일
- 분기 마감 시나리오
- 시장 테이프 진행
- 보상
- 패널티
```

4Q 휴가의 역할은 회계년도 전환 연출과 요약이다.

```text
회계년도 3Q 마감
→ 4Q 휴가 화면
→ 해당 회계년도 요약 표시
→ 다음 회계년도 시작
```

4Q 휴가 화면에는 해당 회계년도의 운용 요약을 표시한다.

```text
- 현재 운용가치
- 올해 운용 수익 총합
- 분기별 운용 수익
- 보유 자산 수
- 환매 압력
```

보너스나 패널티는 없다.

---

## 7. 영업일 시작 처리

영업일 시작 시 다음 처리를 수행한다.

```text
영업일 시작
→ 보유 자산 인컴 발생
→ 시장 영역을 디폴트 시장 상태로 설정
→ 플레이어 입력 대기
```

보유 자산 인컴은 모든 영업일 시작에 발생한다.

```text
분기 첫 영업일
→ 인컴 발생

다음 영업일로 넘어간 경우
→ 다음 영업일 시작 시 인컴 발생
```

단, 다음 영업일이 존재하지 않는 경우에는 인컴이 발생하지 않는다.

```text
마지막 영업일 종료
→ 다음 영업일 없음
→ 인컴 없음
→ 분기 마감
```

---

## 8. 영업일 종료 처리

영업일은 주요 행동이 확정되면 종료된다.

영업일을 종료시키는 대표 입력은 다음과 같다.

```text
- 매수 확정
- 예약 버튼 클릭
- 유동성 확보 완료
- 다음 영업일 클릭
- 추가 매수권 사용 후 두 번째 매수 완료
- 추가 매수권 보유 중 다음 영업일 클릭
```

영업일 종료 시 처리 순서는 다음과 같다.

```text
영업일 종료
→ 남은 영업일 1 감소
→ 남은 영업일 확인
```

남은 영업일이 있으면 다음 영업일을 시작한다.

```text
남은 영업일 > 0
→ 다음 영업일 시작
```

남은 영업일이 없으면 분기 마감으로 이동한다.

```text
남은 영업일 == 0
→ 분기 마감
```

---

## 9. 분기 시작 처리

분기 시작 시 다음 요소를 설정한다.

```text
- 현재 회계년도
- 현재 분기
- 분기당 영업일 수
- 분기 목표 운용 수익
- 분기 마감 시나리오 또는 시장 국면
- 분기 운용 수익 초기화
```

시장 테이프 처리는 분기 시작 종류에 따라 다르다.

```text
회계년도 시작 시 1Q
→ 시장 테이프 갱신

같은 회계년도 내 다음 분기
→ 시장 테이프 진행
```

그 후 첫 영업일을 시작한다.

```text
분기 시작
→ 영업일 수 설정
→ 시장 테이프 처리
→ 1영업일 시작
→ 인컴 발생
```

---

## 10. 분기 마감 처리

분기 마지막 영업일이 종료되면 분기 마감이 발생한다.

분기 마감 처리 순서는 다음과 같다.

```text
분기 마지막 영업일 종료
→ 태그별 정산 애니메이션 순차 재생
→ 분기 마감 정산 수익 반영
→ 분기 운용 수익 확정
→ 분기 목표 달성률 계산
→ 목표 미달 시 환매 압력 증가량 계산
→ 환매 압력 증가 적용
→ 환매 압력 한도 검사
→ 실패하지 않았으면 분기 정산 UI 표시
→ 다음 일정 진행
```

분기 성공 시 별도 보상은 없다.

```text
분기 목표 달성
→ 환매 압력 증가 없음
→ 별도 보상 없음
→ 다음 일정 진행
```

분기 실패 시에는 달성률에 따라 환매 압력이 증가한다.

```text
운용 수익이 목표의 50% 미만
→ 환매 압력 +3

운용 수익이 목표의 75% 미만
→ 환매 압력 +2

운용 수익이 목표의 100% 미만
→ 환매 압력 +1
```

환매 압력이 10 이상이면 즉시 런 실패가 발생한다.

---

## 11. 회계년도 시작 처리

회계년도 시작 시 다음 처리를 수행한다.

```text
회계년도 시작
→ 현재 회계년도 설정
→ 현재 분기를 1Q로 설정
→ 시장 테이프 갱신
→ 1Q 시작
```

시장 테이프 갱신은 시장 전체를 새 카드로 재구성하는 처리이다.

```text
기존 시장 테이프 카드 전부 제거
→ 매도 임박 / 현재 시장 / 예비 시장 슬롯을 새 카드로 전부 구성
```

예약 카드는 회계년도 전환 시에도 유지된다.

```text
시장 테이프는 갱신됨
예약 슬롯은 유지됨
```

---

## 12. 다음 분기 시작 처리

같은 회계년도 내에서 다음 분기로 넘어갈 때는 시장 테이프를 진행한다.

```text
분기 마감
→ 다음 분기 시작
→ 시장 테이프 진행
```

시장 테이프 진행은 다음 처리이다.

```text
1. 매도 임박 카드 제거
2. 현재 시장 카드 → 매도 임박으로 이동
3. 예비 시장 카드 → 현재 시장으로 이동
4. 예비 시장 빈 슬롯 새 카드로 보충
```

예약 카드는 분기 전환 시에도 유지된다.

```text
시장 테이프는 진행됨
예약 슬롯은 유지됨
```

---

## 13. 휴가 이후 회계년도 전환

1·2회계년도의 3Q가 끝나면 4Q 휴가 화면을 표시한다.

```text
1회계년도 3Q 마감
→ 1회계년도 4Q 휴가 화면
→ 계속
→ 2회계년도 시작
```

```text
2회계년도 3Q 마감
→ 2회계년도 4Q 휴가 화면
→ 계속
→ 3회계년도 시작
```

휴가 화면에서 계속을 누르면 다음 회계년도가 시작된다.

```text
휴가 화면 계속
→ 다음 회계년도 시작
→ 시장 테이프 갱신
→ 1Q 시작
```

---

## 14. 최종 정산 진입

3회계년도 4Q 마감 후, 환매 압력으로 인한 실패가 발생하지 않았다면 최종 정산으로 이동한다.

```text
3회계년도 4Q 마감
→ 최종 정산
```

최종 정산에서는 다음 정보를 표시한다.

```text
- 최종 운용가치
- 최종 평가
- 총 운용 수익
- 보유 자산 수
- 환매 압력
- 운용 코멘트
```

---

## 15. 런 실패 진입

환매 압력이 10 이상이 되면 즉시 런 실패가 발생한다.

```text
환매 압력 >= 10
→ 대규모 환매 발생
→ 런 실패
```

환매 압력 한도 검사는 환매 압력이 증가하는 즉시 수행한다.

```text
예약 액션으로 환매 압력 증가
→ 즉시 한도 검사

분기 마감으로 환매 압력 증가
→ 즉시 한도 검사
```

런 실패가 발생하면 이후 분기 또는 회계년도 진행은 중단된다.

---

## 16. 시간 구조 예시

### 16.1 1회계년도 전체 흐름

```text
1회계년도 시작
→ 시장 테이프 갱신
→ 1Q 시작
→ 4영업일 진행
→ 1Q 마감
→ 2Q 시작
→ 시장 테이프 진행
→ 4영업일 진행
→ 2Q 마감
→ 3Q 시작
→ 시장 테이프 진행
→ 4영업일 진행
→ 3Q 마감
→ 4Q 휴가 화면
→ 2회계년도 시작
```

---

### 16.2 3회계년도 전체 흐름

```text
3회계년도 시작
→ 시장 테이프 갱신
→ 1Q 시작
→ 5영업일 진행
→ 1Q 마감
→ 2Q 시작
→ 시장 테이프 진행
→ 5영업일 진행
→ 2Q 마감
→ 3Q 시작
→ 시장 테이프 진행
→ 5영업일 진행
→ 3Q 마감
→ 4Q 시작
→ 시장 테이프 진행
→ 5영업일 진행
→ 4Q 마감
→ 최종 정산
```

---

## 17. 구현 데이터 제안

### 17.1 RunCalendarState

```csharp
public class RunCalendarState
{
    public int CurrentFiscalYear;
    public int CurrentQuarter;
    public int RemainingBusinessDays;
}
```

---

### 17.2 FiscalYearData

```csharp
public class FiscalYearData
{
    public int FiscalYearIndex;
    public List<QuarterData> Quarters;
}
```

---

### 17.3 QuarterData

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

---

## 18. 구현 함수 예시

### 18.1 분기당 영업일 수

```csharp
int GetBusinessDaysPerQuarter(int fiscalYear)
{
    switch (fiscalYear)
    {
        case 1:
        case 2:
            return 4;

        case 3:
            return 5;

        default:
            return 4;
    }
}
```

---

### 18.2 플레이 가능한 분기 수

```csharp
int GetPlayableQuarterCount(int fiscalYear)
{
    switch (fiscalYear)
    {
        case 1:
        case 2:
            return 3;

        case 3:
            return 4;

        default:
            return 3;
    }
}
```

---

### 18.3 휴가 분기 판정

```csharp
bool IsVacationQuarter(int fiscalYear, int quarter)
{
    return (fiscalYear == 1 || fiscalYear == 2)
        && quarter == 4;
}
```

---

### 18.4 회계년도 시작

```csharp
void StartFiscalYear(int fiscalYear)
{
    CurrentFiscalYear = fiscalYear;
    CurrentQuarter = 1;

    RefreshMarketTape();

    StartQuarter(CurrentFiscalYear, CurrentQuarter);
}
```

---

### 18.5 분기 시작

```csharp
void StartQuarter(int fiscalYear, int quarter)
{
    CurrentFiscalYear = fiscalYear;
    CurrentQuarter = quarter;

    RemainingBusinessDays = GetBusinessDaysPerQuarter(fiscalYear);

    CurrentQuarterEarnedCash = 0;
    CurrentQuarterTargetCash = GetQuarterTargetCash(fiscalYear, quarter);

    StartBusinessDay();
}
```

---

### 18.6 영업일 종료

```csharp
void EndBusinessDay()
{
    RemainingBusinessDays--;

    if (RemainingBusinessDays <= 0)
    {
        ResolveQuarterEnd();
        return;
    }

    StartBusinessDay();
}
```

---

### 18.7 다음 일정 진행

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

### 18.8 휴가 이후 계속

```csharp
void OnVacationContinueClicked()
{
    int nextFiscalYear = CurrentFiscalYear + 1;

    StartFiscalYear(nextFiscalYear);
}
```

---

## 19. 구현 시 주의사항

```text
- 회계년도 시작 시에는 시장 테이프 갱신을 사용한다.
- 같은 회계년도 안에서 다음 분기 시작 시에는 시장 테이프 진행을 사용한다.
- 1·2회계년도 4Q는 플레이하지 않는다.
- 3회계년도 4Q는 플레이한다.
- 분기 첫 영업일에도 인컴은 정상 발생한다.
- 마지막 영업일 종료 후에는 다음 영업일 인컴이 발생하지 않고 분기 마감으로 간다.
- 예약 카드는 분기와 회계년도 전환 후에도 유지한다.
- 시장 테이프는 분기/회계년도 전환에 따라 바뀌지만, 예약 슬롯은 유지된다.
```
