# 16. 분기 마감 시스템

## 1. 목적

이 문서는 분기 마지막 영업일 종료 후 진행되는 분기 마감 처리 흐름을 정의한다.

분기 마감은 해당 분기의 운용 성과를 확정하고, 목표 달성 여부에 따라 환매 압력을 처리하는 구간이다.

분기 마감의 핵심 목적은 다음과 같다.

```text
- 보유 자산의 태그별 정산 수익 계산
- 분기 운용 수익 확정
- 분기 목표 달성률 계산
- 목표 미달 시 환매 압력 증가
- 환매 압력 한도 도달 여부 검사
- 분기 정산 UI 표시
- 다음 분기 / 휴가 / 최종 정산으로 진행
```

---

## 2. 분기 마감 진입 조건

분기 마감은 해당 분기의 마지막 영업일이 종료되었을 때 발생한다.

```text
마지막 영업일 종료
→ 남은 영업일 0
→ 분기 마감 진입
```

마지막 영업일 종료 후에는 다음 영업일 시작 운용 수익이 발생하지 않는다.

```text
마지막 영업일 종료
→ 다음 영업일 없음
→ 운용 수익 없음
→ 분기 마감
```

---

## 3. 분기 마감 전체 흐름

분기 마감 처리 순서는 다음과 같다.

```text
1. 분기 마감 진입
2. 태그별 정산 애니메이션 순차 재생
3. 분기 마감 정산 수익 반영
4. 분기 운용 수익 확정
5. 분기 목표 달성률 계산
6. 환매 압력 증가량 계산
7. 환매 압력 증가 적용
8. 환매 압력 한도 검사
9. 실패하지 않았으면 분기 정산 UI 표시
10. 플레이어가 다음 진행 입력
11. 다음 분기 / 휴가 / 회계년도 / 최종 정산으로 이동
```

중요한 점은 정산 수익이 목표 달성률 계산 전에 반영되어야 한다는 것이다.

```text
분기 마감 정산 수익
→ 분기 운용 수익에 포함
→ 목표 달성률 계산에 반영
```

---

## 4. 분기 운용 수익

분기 운용 수익은 해당 분기 동안 발생한 운용 성과 현금이다.

포함 대상:

```text
- 보유 자산의 영업일 시작 현금
- 분기 마감 정산 수익
- 기타 운용 성과로 분류된 현금
```

제외 대상:

```text
- 자원 확보로 얻은 현금
```

자원 확보로 얻은 현금은 조달 현금이다.

```text
조달 현금
= 조달 현금
= 분기 운용 수익에 포함하지 않음
```

---

## 5. 태그별 정산 애니메이션

분기 마감 시 보유 자산의 태그를 기준으로 정산 애니메이션을 순차 재생한다.

```text
분기 마지막 영업일 종료
→ 태그별 정산 애니메이션 순차 재생
```

태그별 정산은 분기 마감 정산 수익을 시각적으로 보여주는 단계이다.

예시:

```text
성장 태그 정산
→ +3 현금

운용 수익 태그 정산
→ +2 현금

방어 태그 정산
→ +1 현금
```

각 정산 애니메이션은 순차적으로 재생한다.

```text
성장 정산
→ 운용 수익 정산
→ 방어 정산
→ 정산 UI 표시
```

---

## 6. 정산 애니메이션 스킵 규칙

정산 애니메이션은 플레이어 입력으로 스킵할 수 있다.

다만 하나의 입력은 하나의 태그별 정산 애니메이션만 스킵한다.

```text
클릭 1회
→ 현재 재생 중인 태그 정산 애니메이션만 스킵
→ 다음 태그 정산 애니메이션으로 이동
```

모든 정산 애니메이션을 한 번에 스킵하지 않는다.

```text
클릭 1회
≠ 전체 정산 스킵
```

예시:

```text
정산 애니메이션 5개 대기

클릭 1회
→ 1번째 애니메이션만 스킵

클릭 2회
→ 2번째 애니메이션만 스킵
```

---

## 7. 분기 마감 정산 수익 반영

태그별 정산 애니메이션이 끝나면 정산 수익을 반영한다.

```text
태그별 정산 애니메이션 완료
→ 정산 수익 반영
```

정산 수익은 운용 수익으로 기록한다.

```text
분기 마감 정산 수익
→ CurrentCash 증가
→ CurrentQuarterEarnedCash 증가
→ CurrentFiscalYearEarnedCash 증가
→ TotalEarnedCash 증가
```

정산 수익은 조달 현금과 다르다.

```text
분기 마감 정산 수익
= 운용 성과 현금
```

---

## 8. 분기 목표

각 분기는 목표 운용 수익을 가진다.

```text
분기 목표
= 해당 분기에서 달성해야 하는 운용 수익 기준
```

분기 목표는 현재 현금 보유량 기준이 아니다.

```text
현재 현금 >= 분기 목표
→ 성공 판정 아님
```

분기 성공 여부는 분기 운용 수익 기준이다.

```text
분기 운용 수익 >= 분기 목표
→ 분기 성공
```

---

## 9. 분기 성공 판정

분기 운용 수익이 분기 목표 이상이면 성공이다.

```text
CurrentQuarterEarnedCash >= CurrentQuarterTargetCash
→ 분기 성공
```

분기 성공 시 처리:

```text
- 환매 압력 증가 없음
- 별도 보상 없음
- 다음 일정 진행
```

성공 보상은 따로 없다.

```text
분기 성공
= 환매 압력 증가를 피하는 것 자체가 보상
```

---

## 10. 분기 실패 판정

분기 운용 수익이 분기 목표보다 낮으면 실패이다.

```text
CurrentQuarterEarnedCash < CurrentQuarterTargetCash
→ 분기 실패
```

분기 실패 시에는 목표 달성률에 따라 환매 압력이 증가한다.

```text
목표의 50% 미만
→ 환매 압력 +3

목표의 75% 미만
→ 환매 압력 +2

목표의 100% 미만
→ 환매 압력 +1
```

분기 실패가 즉시 런 실패를 의미하지는 않는다.
다만 환매 압력이 10 이상이 되면 런 실패가 발생한다.

---

## 11. 목표 달성률 계산

목표 달성률은 다음 방식으로 계산한다.

```text
목표 달성률 = 분기 운용 수익 / 분기 목표
```

예시:

```text
분기 목표: 20
분기 운용 수익: 14

달성률 = 14 / 20 = 70%
```

분기 목표가 0 이하인 특수 상황은 발생하지 않도록 데이터에서 관리한다.
안전 코드에서는 목표가 0 이하일 경우 성공으로 처리해 0 나누기 오류를 방지한다.

---

## 12. 환매 압력 증가량 계산

환매 압력 증가량은 목표 달성률로 결정한다.

```text
운용 수익 >= 목표
→ +0

운용 수익 < 목표
그리고 달성률 >= 75%
→ +1

달성률 < 75%
그리고 달성률 >= 50%
→ +2

달성률 < 50%
→ +3
```

표로 정리하면 다음과 같다.

| 달성률 | 환매 압력 증가 |
|---:|---:|
| 100% 이상 | +0 |
| 75% 이상 100% 미만 | +1 |
| 50% 이상 75% 미만 | +2 |
| 50% 미만 | +3 |

---

## 13. 환매 압력 한도 검사

분기 마감에서 환매 압력이 증가하면 즉시 한도 검사를 한다.

```text
환매 압력 증가
→ 즉시 한도 검사
```

환매 압력 한도는 10이다.

```text
환매 압력 >= 10
→ 대규모 환매 발생
→ 런 실패
```

분기 마감으로 환매 압력이 10 이상이 되면 다음 분기로 넘어가지 않는다.

```text
분기 마감
→ 환매 압력 증가
→ 환매 압력 10 도달
→ 런 실패
→ 다음 분기 진행 없음
```

---

## 14. 분기 정산 UI

환매 압력 한도 검사 후, 런 실패가 발생하지 않았다면 분기 정산 UI를 표시한다.

분기 정산 UI에는 다음 정보를 표시한다.

```text
- 분기 운용 수익
- 분기 목표
- 목표 달성률
- 환매 압력 증가량
- 현재 환매 압력
```

성공 예시:

```text
분기 마감

분기 운용 수익: 24
분기 목표: 20
달성률: 120%

환매 압력 증가: 없음
현재 환매 압력: 4 / 10
```

실패 예시:

```text
분기 마감

분기 운용 수익: 14
분기 목표: 20
달성률: 70%

환매 압력 증가: +2
현재 환매 압력: 6 / 10
```

---

## 15. 분기 정산 UI 이후 진행

분기 정산 UI에서 다음 진행 버튼을 누르면 현재 위치에 따라 다음 일정으로 이동한다.

```text
1·2회계년도 1Q / 2Q 마감
→ 다음 분기 시작

1·2회계년도 3Q 마감
→ 4Q 휴가 화면

3회계년도 1Q / 2Q / 3Q 마감
→ 다음 분기 시작

3회계년도 4Q 마감
→ 최종 정산
```

다음 분기 시작 시에는 시장 테이프를 진행한다.

```text
같은 회계년도 내 다음 분기 시작
→ 시장 테이프 진행
```

다음 회계년도 시작 시에는 시장 테이프를 갱신한다.

```text
회계년도 시작
→ 시장 테이프 갱신
```

---

## 16. 회계년도 요약과 분기 수익

1·2회계년도의 4Q 휴가 화면에서는 해당 회계년도의 운용 수익 요약을 표시한다.

회계년도 운용 수익은 해당 회계년도 플레이 분기의 운용 수익 합산이다.

```text
1회계년도 운용 수익
= 1Q 운용 수익
+ 2Q 운용 수익
+ 3Q 운용 수익
```

4Q는 휴가이므로 운용 수익이 없다.

휴가 화면에는 분기별 운용 수익도 함께 표시한다.

```text
올해 운용 수익: 18
- 1Q +4
- 2Q +6
- 3Q +8
```

---

## 17. 분기 마감 상태 데이터

분기 마감 처리를 위한 데이터 예시:

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

## 18. 분기 운용 수익 상태

운용 수익은 다음 상태로 관리한다.

```csharp
public class RunPerformanceState
{
    public int TotalEarnedCash;
    public int CurrentFiscalYearEarnedCash;
    public int CurrentQuarterEarnedCash;

    public List<QuarterEarnedCashData> QuarterEarnedCashHistory;
}
```

분기 시작 시 `CurrentQuarterEarnedCash`를 0으로 초기화한다.

```csharp
void StartQuarter()
{
    CurrentQuarterEarnedCash = 0;
}
```

---

## 19. 분기 마감 함수 예시

```csharp
void ResolveQuarterEnd()
{
    CurrentGamePhase = GamePhase.QuarterEnd;

    PlayQuarterSettlementAnimations(
        onComplete: ResolveQuarterEndAfterAnimations
    );
}
```

```csharp
void ResolveQuarterEndAfterAnimations()
{
    int settlementIncome = CalculateQuarterSettlementIncome();

    AddPerformanceCash(settlementIncome);

    QuarterEndResult result = BuildQuarterEndResult(settlementIncome);

    int pressureIncrease =
        GetRedemptionPressureIncreaseFromQuarterResult(
            result.QuarterEarnedCash,
            result.QuarterTargetCash
        );

    result.RedemptionPressureIncrease = pressureIncrease;

    if (pressureIncrease > 0)
    {
        AddRedemptionPressure(pressureIncrease);

        result.CurrentRedemptionPressure = CurrentRedemptionPressure;

        if (IsRunFailedByRedemption())
        {
            result.IsRunFailed = true;
            TriggerRunFailureByRedemption();
            return;
        }
    }

    ShowQuarterEndSummary(result);
}
```

---

## 20. 환매 압력 증가량 함수 예시

```csharp
int GetRedemptionPressureIncreaseFromQuarterResult(
    int quarterEarnedCash,
    int quarterTargetCash
)
{
    if (quarterTargetCash <= 0)
        return 0;

    if (quarterEarnedCash >= quarterTargetCash)
        return 0;

    float achievementRate =
        (float)quarterEarnedCash / quarterTargetCash;

    if (achievementRate < 0.5f)
        return 3;

    if (achievementRate < 0.75f)
        return 2;

    return 1;
}
```

---

## 21. 달성률 표시 함수 예시

```csharp
int GetAchievementPercent(int earnedCash, int targetCash)
{
    if (targetCash <= 0)
        return 100;

    return Mathf.FloorToInt(
        ((float)earnedCash / targetCash) * 100f
    );
}
```

---

## 22. 정산 애니메이션 스킵 예시

```csharp
void OnQuarterSettlementSkipInput()
{
    if (!IsPlayingSettlementAnimation)
        return;

    SkipCurrentSettlementAnimationOnly();
}
```

```csharp
void SkipCurrentSettlementAnimationOnly()
{
    CompleteCurrentTagSettlementAnimationImmediately();

    PlayNextTagSettlementAnimationOrFinish();
}
```

---

## 23. 다음 진행 버튼 예시

```csharp
void OnQuarterEndContinueClicked()
{
    if (CurrentRunState == RunState.Failed)
        return;

    ProceedAfterQuarterEnd();
}
```

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

## 24. 구현 시 주의사항

```text
- 분기 마감은 마지막 영업일 종료 후 발생한다.
- 마지막 영업일 종료 후 다음 영업일 운용 수익은 발생하지 않는다.
- 분기 마감 정산 수익은 운용 수익에 포함한다.
- 조달 현금은 분기 운용 수익에 포함하지 않는다.
- 목표 달성률 계산 전에 정산 수익을 먼저 반영한다.
- 분기 성공 시 별도 보상은 없다.
- 분기 실패 시 달성률에 따라 환매 압력이 증가한다.
- 환매 압력 증가 후 즉시 한도 검사를 한다.
- 환매 압력 10 이상이면 즉시 런 실패다.
- 런 실패가 발생하면 분기 정산 UI 대신 실패 화면으로 이동한다.
- 정산 애니메이션 스킵 입력 1회는 현재 태그 애니메이션 1개만 스킵한다.
- 1·2회계년도 3Q 이후에는 4Q 휴가 화면으로 이동한다.
- 3회계년도 4Q 이후에는 최종 정산으로 이동한다.
```
