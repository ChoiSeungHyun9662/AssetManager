# 15. 영업일 흐름

## 1. 목적

이 문서는 영업일의 시작, 플레이어 입력, 행동 확정, 영업일 종료, 다음 영업일 또는 분기 마감으로 이어지는 흐름을 정의한다.

영업일은 본 게임에서 플레이어가 주요 행동 1회를 수행할 수 있는 최소 시간 단위이다.

```text
영업일
= 기존 턴에 해당하는 개념
= 유저-facing 표현은 "영업일"로 통일
```

플레이어는 액션 선택창을 거치지 않고, 보드 위 요소를 직접 조작한다.  
행동이 확정되면 영업일 1일을 소비한다.

---

## 2. 영업일 기본 구조

한 영업일은 다음 순서로 진행된다.

```text
영업일 시작
→ 보유 자산 인컴 발생
→ 시장 디폴트 상태 진입
→ 플레이어 직접 조작
→ 행동 확정
→ 영업일 종료
```

영업일이 종료되면 남은 영업일 수를 확인한다.

```text
남은 영업일 있음
→ 다음 영업일 시작

남은 영업일 없음
→ 분기 마감
```

---

## 3. 영업일 시작

영업일 시작 시 다음 처리를 수행한다.

```text
1. 영업일 시작 상태 진입
2. 보유 자산 인컴 발생
3. 시장 영역을 Market 상태로 설정
4. 다음 영업일 버튼 상태 갱신
5. 플레이어 입력 대기 상태 진입
```

보유 자산 인컴은 영업일 시작마다 발생한다.

```text
영업일 시작
→ 보유 자산 인컴 발생
```

분기 첫 영업일에도 인컴은 정상 발생한다.

```text
분기 시작
→ 1영업일 시작
→ 보유 자산 인컴 발생
```

---

## 4. 인컴 발생 규칙

보유 자산의 인컴은 `운용 수익`으로 기록한다.

```text
보유 자산 인컴
→ CurrentCash 증가
→ CurrentQuarterEarnedCash 증가
→ CurrentFiscalYearEarnedCash 증가
→ TotalEarnedCash 증가
```

예약 카드는 인컴을 발생시키지 않는다.

```text
예약 카드
→ 인컴 없음
```

시장 카드도 인컴을 발생시키지 않는다.

```text
시장 카드
→ 인컴 없음
```

매수한 자산은 매수한 즉시 인컴을 발생시키지 않는다.

```text
자산 매수
→ 해당 영업일에는 인컴 없음
→ 다음 영업일 시작부터 인컴 발생
```

---

## 5. 플레이어 입력 대기 상태

인컴 처리가 끝나면 플레이어 입력 대기 상태로 진입한다.

```text
영업일 시작 처리 완료
→ 시장 영역 Market 상태
→ 플레이어 조작 가능
```

시장 상태에서 가능한 주요 입력은 다음과 같다.

```text
- 시장 카드 클릭
- 예약 카드 클릭
- 자원통 클릭
- 다음 영업일 클릭
```

이 입력들이 모두 즉시 영업일을 소비하는 것은 아니다.  
영업일 소비는 행동 확정 시점에 발생한다.

---

## 6. 행동 확정 원칙

핵심 원칙은 다음이다.

```text
검토는 무료.
확정은 영업일 소비.
```

무료 검토 예시:

```text
시장 카드 클릭
→ 카드 상세보기
→ 영업일 소비 없음

예약 카드 클릭
→ 카드 상세보기
→ 영업일 소비 없음

자원통 클릭
→ 유동성 확보 화면
→ 첫 자원 선택 전까지 영업일 소비 없음
```

영업일을 소비하는 확정 행동:

```text
- 매수 확정
- 예약 버튼 클릭
- 유동성 확보에서 첫 자원 획득 후 완료
- 다음 영업일 클릭
```

---

## 7. 영업일을 소비하는 행동

영업일을 소비하는 행동은 다음과 같다.

### 7.1 자산 매수 확정

```text
카드 상세보기
→ 매수 버튼 클릭
→ 매수 조건 검증
→ 자원 소비
→ 자산 획득
→ 영업일 소비
```

추가 매수권이 없는 경우 매수 완료 후 영업일을 종료한다.

```text
매수 완료
→ 추가 매수권 없음
→ 영업일 종료
```

추가 매수권이 발생한 경우에는 영업일을 바로 종료하지 않고 추가 매수 선택 상태로 들어간다.

```text
매수 완료
→ 추가 매수권 발생
→ 추가 매수 선택 상태
```

---

### 7.2 예약 버튼 클릭

예약 버튼 클릭은 즉시 예약 액션이다.

```text
시장 카드 상세보기
→ 예약 버튼 클릭
→ 카드 예약
→ 딜 +1
→ 환매 압력 +1
→ 시장 테이프 진행
→ 영업일 종료
```

예약 확인 상태는 없다.

---

### 7.3 유동성 확보 완료

유동성 확보는 자원통을 클릭해 진입한다.

```text
자원통 클릭
→ 유동성 확보 화면
→ 아직 영업일 소비 없음
```

첫 자원 클릭 순간 유동성 확보 행동이 확정된다.

```text
첫 자원 클릭
→ 자원 즉시 획득
→ 유동성 확보 행동 확정
```

유동성 확보는 다음 조건 중 하나로 완료된다.

```text
- 같은 기본 자원 2개 획득
- 서로 다른 기본 자원 3개 획득
- 전문 자원 한도 때문에 더 이상 유효 선택지가 없음
```

완료 시 영업일을 종료한다.

---

### 7.4 다음 영업일

다음 영업일 버튼은 현재 영업일을 종료하는 입력이다.

```text
다음 영업일 클릭
→ 현재 영업일 종료
```

다음 영업일 버튼은 시장 영역이 Market 상태일 때만 활성화된다.

```text
Market
→ 다음 영업일 활성화

CardDetail
→ 다음 영업일 비활성화

GainLiquidity
→ 다음 영업일 비활성화
```

---

## 8. 영업일을 소비하지 않는 행동

다음 행동은 영업일을 소비하지 않는다.

```text
- 시장 카드 클릭
- 예약 카드 클릭
- 카드 상세보기 닫기
- 카드 상세보기에서 칩 배치
- 카드 상세보기에서 칩 회수
- 카드 상세보기에서 자동 배치
- 유동성 확보 화면 진입
- 유동성 확보 첫 자원 획득 전 닫기
```

단, 카드 상세보기에서 예약 버튼을 클릭하면 즉시 영업일을 소비한다.  
또한 유동성 확보에서 첫 자원을 획득하면 행동이 확정된다.

---

## 9. 시장 영역 상태와 영업일 흐름

시장 영역 상태는 영업일 중 플레이어 입력 가능 범위를 결정한다.

```csharp
public enum MarketAreaState
{
    Market,
    CardDetail,
    GainLiquidity
}
```

상태별 의미:

| 상태 | 의미 |
|---|---|
| Market | 시장 기본 상태. 카드, 자원통, 다음 영업일 조작 가능 |
| CardDetail | 카드 정보와 매수/예약 UI 표시 |
| GainLiquidity | 유동성 확보 자원 선택 UI 표시 |

상태별 다음 영업일 가능 여부:

| 상태 | 다음 영업일 |
|---|---:|
| Market | 가능 |
| CardDetail | 불가 |
| GainLiquidity | 불가 |

---

## 10. CardDetail 상태의 영업일 흐름

카드 상세보기 상태는 검토 상태이다.

```text
카드 클릭
→ CardDetail
→ 영업일 소비 없음
```

닫기:

```text
CardDetail
→ 닫기
→ Market
→ 영업일 소비 없음
```

매수 확정:

```text
CardDetail
→ 매수 확정
→ 영업일 소비
```

예약 버튼:

```text
CardDetail
→ 예약 버튼 클릭
→ 예약 액션 실행
→ 영업일 소비
```

카드 상세보기 상태에서는 다음 영업일 버튼이 비활성화된다.

---

## 11. GainLiquidity 상태의 영업일 흐름

유동성 확보 상태는 첫 자원 획득 전까지는 검토 상태이다.

```text
자원통 클릭
→ GainLiquidity
→ 영업일 소비 없음
```

첫 자원 획득 전 닫기:

```text
GainLiquidity
→ 닫기
→ Market
→ 영업일 소비 없음
```

첫 자원 획득:

```text
GainLiquidity
→ 자원 클릭
→ 자원 즉시 획득
→ 유동성 확보 행동 확정
→ 닫기 비활성화
```

이후 조합 완성 또는 선택지 소멸 시 영업일 종료:

```text
유동성 확보 완료
→ 영업일 종료
```

GainLiquidity 상태에서는 다음 영업일 버튼이 비활성화된다.

---

## 12. 영업일 종료 처리

영업일 종료 시 다음 순서로 처리한다.

```text
1. 현재 영업일 행동 상태 정리
2. 시장 영역 상태 정리
3. 남은 영업일 1 감소
4. 남은 영업일 확인
5. 다음 영업일 또는 분기 마감으로 이동
```

구체적 흐름:

```text
EndBusinessDay()
→ RemainingBusinessDays--
→ if RemainingBusinessDays > 0
      StartBusinessDay()
  else
      ResolveQuarterEnd()
```

---

## 13. 남은 영업일이 있는 경우

남은 영업일이 있으면 다음 영업일을 시작한다.

```text
영업일 종료
→ 남은 영업일 > 0
→ 다음 영업일 시작
→ 보유 자산 인컴 발생
→ 시장 상태 진입
```

다음 영업일 시작 시 시장 테이프는 자동 진행되지 않는다.

```text
다음 영업일
→ 시장 테이프 변화 없음
```

시장 테이프가 진행되는 상황은 별도이다.

```text
시장 테이프 진행:
- 예약 액션 실행 후
- 같은 회계년도 내 다음 분기 시작
```

---

## 14. 남은 영업일이 없는 경우

남은 영업일이 없으면 분기 마감으로 진입한다.

```text
영업일 종료
→ 남은 영업일 == 0
→ 분기 마감
```

이 경우 다음 영업일 시작 인컴은 발생하지 않는다.

```text
마지막 영업일 종료
→ 다음 영업일 없음
→ 인컴 없음
→ 분기 마감
```

분기 마감에서는 태그별 정산 애니메이션과 분기 목표 판정이 진행된다.

---

## 15. 다음 영업일 버튼

다음 영업일 버튼은 명시적으로 현재 영업일을 포기하고 다음 날로 넘어가는 입력이다.

버튼 문구:

```text
다음 영업일
```

처리:

```text
다음 영업일 클릭
→ 영업일 종료
```

다음 영업일 버튼은 다음 상황에서 활성화된다.

```text
- 시장 영역이 Market 상태
- 현재 플레이어 입력 대기 상태
```

다음 상황에서는 비활성화된다.

```text
- CardDetail 상태
- GainLiquidity 상태
- 자동 배치 애니메이션 중
- 분기 마감 처리 중
- 최종 정산 화면
- 런 실패 화면
```

추가 매수권 상태에서는 다음 영업일 버튼을 사용할 수 있다.

```text
추가 매수권 보유 중
→ 다음 영업일 클릭 가능
→ 추가 매수권 포기
→ 영업일 종료
```

---

## 16. 추가 매수권 상태의 영업일 흐름

자산 매수 후 추가 매수권이 발생하면 영업일을 즉시 종료하지 않는다.

```text
첫 번째 자산 매수
→ 추가 매수권 발생
→ 추가 매수 선택 상태
```

추가 매수권 상태에서 가능한 행동:

```text
- 시장 카드 매수
- 예약 카드 매수
- 다음 영업일
```

불가능한 행동:

```text
- 유동성 확보
- 카드 예약
```

추가 매수권을 사용해 자산을 매수하면 영업일을 종료한다.

```text
추가 매수
→ 자산 매수 확정
→ 추가 매수권 소모
→ 영업일 종료
```

다음 영업일을 누르면 추가 매수권을 포기하고 영업일을 종료한다.

```text
추가 매수권 보유
→ 다음 영업일
→ 추가 매수권 폐기
→ 영업일 종료
```

---

## 17. 환매 압력에 의한 영업일 중단

예약 액션으로 환매 압력이 증가할 수 있다.

```text
예약 액션
→ 환매 압력 +1
→ 환매 압력 한도 검사
```

환매 압력이 10 이상이 되면 즉시 런 실패가 발생한다.

```text
환매 압력 >= 10
→ 대규모 환매 발생
→ 런 실패
```

이 경우 이후 영업일 종료나 다음 영업일 처리는 진행하지 않는다.

```text
예약으로 환매 압력 10 도달
→ 런 실패
→ EndBusinessDay 진행하지 않음
```

---

## 18. 행동 상태 초기화

영업일 종료 시 다음 상태를 초기화한다.

```text
- 카드 상세보기 선택 카드
- 매수 결제 상태
- 유동성 확보 상태
- 자동 배치 상태
- 매수 입력 큐
- 추가 매수권 상태
```

단, 다음 항목은 유지한다.

```text
- 보유 자산
- 예약 카드
- 보유 자원
- 현금
- 환매 압력
- 시장 테이프
- 분기 운용 수익
```

시장 영역은 다음 영업일 시작 시 Market 상태가 된다.

---

## 19. 영업일 상태 데이터

구현상 영업일 진행 상태는 다음과 같이 관리할 수 있다.

```csharp
public enum BusinessDayPhase
{
    Starting,
    WaitingForPlayerAction,
    ResolvingAction,
    Ended
}
```

영업일 관련 상태:

```csharp
public class BusinessDayState
{
    public int RemainingBusinessDays;
    public BusinessDayPhase Phase;

    public bool HasExtraBuyAction;
    public bool IsWaitingForExtraBuyDecision;
    public bool IsExtraBuyPurchase;
}
```

---

## 20. 영업일 시작 함수 예시

```csharp
void StartBusinessDay()
{
    CurrentBusinessDayState.Phase = BusinessDayPhase.Starting;

    ClearTransientActionStates();

    ApplyOwnedAssetIncome();

    SetMarketAreaState(MarketAreaState.Market);

    CurrentBusinessDayState.Phase =
        BusinessDayPhase.WaitingForPlayerAction;

    UpdateAllActionButtons();
}
```

---

## 21. 영업일 종료 함수 예시

```csharp
void EndBusinessDay()
{
    if (CurrentRunState == RunState.Failed)
        return;

    CurrentBusinessDayState.Phase = BusinessDayPhase.Ended;

    ClearTransientActionStates();

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

## 22. 다음 영업일 버튼 함수 예시

```csharp
void OnNextBusinessDayClicked()
{
    if (!CanGoNextBusinessDay())
    {
        PlayInvalidActionFeedback();
        return;
    }

    if (CurrentBusinessDayState.IsWaitingForExtraBuyDecision)
    {
        ClearExtraBuyActionIfAny();
    }

    EndBusinessDay();
}
```

---

## 23. 다음 영업일 가능 판정 예시

```csharp
bool CanGoNextBusinessDay()
{
    if (CurrentRunState != RunState.Playing)
        return false;

    if (CurrentBusinessDayState.Phase
        != BusinessDayPhase.WaitingForPlayerAction)
        return false;

    if (CurrentMarketAreaState != MarketAreaState.Market)
        return false;

    return true;
}
```

---

## 24. 일시 상태 초기화 예시

```csharp
void ClearTransientActionStates()
{
    ClearCardDetailState();
    ClearPaymentState();
    ClearLiquidityActionState();

    HasQueuedBuyInput = false;
    IsAutoPlaceAnimating = false;

    // 추가 매수권은 영업일 종료 시 사라져야 하므로 여기서 정리 가능
    ClearExtraBuyActionIfAny();
}
```

단, 추가 매수 선택 상태로 진입해야 하는 경우에는 `EndBusinessDay()`를 호출하지 않으므로, 추가 매수권 상태를 유지한다.

---

## 25. 처리 순서 요약

일반 영업일 흐름:

```text
StartBusinessDay
→ ApplyOwnedAssetIncome
→ Market 상태
→ 플레이어 조작
→ 행동 확정
→ EndBusinessDay
→ 남은 영업일 확인
```

시장 카드 매수:

```text
카드 상세보기
→ 매수 확정
→ 시장 슬롯 보충
→ 추가 매수권 없으면 EndBusinessDay
```

예약:

```text
카드 상세보기
→ 예약 버튼 클릭
→ 예약 처리
→ 환매 압력 검사
→ 실패하지 않으면 EndBusinessDay
```

유동성 확보:

```text
자원통
→ 유동성 확보 화면
→ 자원 선택
→ 조합 완료 또는 자동 종료
→ EndBusinessDay
```

다음 영업일:

```text
Market 상태
→ 다음 영업일 클릭
→ EndBusinessDay
```

---

## 26. 구현 시 주의사항

```text
- 영업일 시작마다 보유 자산 인컴이 발생한다.
- 분기 첫 영업일에도 인컴은 발생한다.
- 마지막 영업일 종료 후에는 인컴 없이 분기 마감으로 간다.
- 카드 보기와 닫기는 영업일을 소비하지 않는다.
- 유동성 확보 진입은 영업일을 소비하지 않는다.
- 유동성 확보 첫 자원 클릭은 행동 확정이다.
- 매수 확정은 영업일을 소비한다.
- 예약 버튼 클릭은 영업일을 소비한다.
- 다음 영업일 버튼은 Market 상태에서만 활성화된다.
- 다음 영업일은 시장 테이프를 진행시키지 않는다.
- 추가 매수권 상태에서는 유동성 확보와 예약을 막는다.
- 환매 압력 10 도달 시 영업일 진행보다 런 실패가 우선한다.
```
