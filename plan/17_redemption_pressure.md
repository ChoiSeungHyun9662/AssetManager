# 17. 환매 압력

## 1. 목적

이 문서는 환매 압력 시스템의 역할, 증가 요인, 한도, 런 실패 판정, 최종 정산 코멘트와의 관계를 정의한다.

환매 압력은 플레이어의 누적 리스크 수치이다.
환매 압력이 한도에 도달하면 대규모 환매가 발생해 런 실패가 된다.

```text
환매 압력 >= 10
→ 대규모 환매 발생
→ 런 실패
```

환매 압력은 단순 점수나 결과 화면용 지표가 아니라, 런을 위협하는 실제 실패 조건이다.

---

## 2. 환매 압력의 역할

환매 압력은 다음 역할을 가진다.

```text
- 무리한 예약 행동에 대한 리스크
- 분기 목표 미달에 대한 누적 페널티
- 런 실패 조건
- 최종 정산 운용 코멘트의 보조 지표
```

플레이어는 높은 운용가치를 노리면서도 환매 압력이 10에 도달하지 않도록 관리해야 한다.

---

## 3. 환매 압력 한도

환매 압력 한도는 10이다.

```text
환매 압력 한도: 10
```

환매 압력이 10 이상이 되면 즉시 런 실패가 발생한다.

```text
환매 압력 0~9
→ 계속 진행

환매 압력 10 이상
→ 런 실패
```

구현 상수 예시:

```csharp
const int MaxRedemptionPressure = 10;
```

---

## 4. 환매 압력 증가 요인

환매 압력은 다음 상황에서 증가한다.

```text
1. 시장 카드 예약
2. 분기 운용 수익 목표 미달
```

각 증가 요인은 즉시 환매 압력 한도 검사를 수행한다.

```text
환매 압력 증가
→ 즉시 한도 검사
```

---

## 5. 예약으로 인한 환매 압력 증가

시장 카드를 예약하면 환매 압력이 1 증가한다.

```text
시장 카드 예약
→ 환매 압력 +1
```

예약 처리 중 환매 압력 증가는 다음 흐름에 포함된다.

```text
예약 버튼 클릭
→ 카드 예약
→ 예약한 자리 보충
→ 시장 테이프 진행
→ 딜 +1
→ 환매 압력 +1
→ 한도 검사
→ 실패하지 않으면 영업일 종료
```

예약으로 환매 압력이 10 이상이 되면 즉시 런 실패가 발생한다.

예시:

```text
현재 환매 압력: 9

시장 카드 예약
→ 환매 압력 +1
→ 환매 압력 10
→ 대규모 환매 발생
→ 런 실패
```

이 경우 영업일 종료보다 런 실패가 우선한다.

---

## 6. 분기 목표 미달로 인한 환매 압력 증가

분기 운용 수익이 분기 목표에 미달하면, 목표 달성률에 따라 환매 압력이 증가한다.

분기 운용 수익 기준:

```text
분기 운용 수익
= 보유 자산의 영업일 시작 현금
+ 분기 마감 정산 수익
+ 기타 운용 성과 현금
```

제외:

```text
자원 확보로 얻은 현금
```

증가량:

```text
운용 수익이 목표의 50% 미만
→ 환매 압력 +3

운용 수익이 목표의 75% 미만
→ 환매 압력 +2

운용 수익이 목표의 100% 미만
→ 환매 압력 +1

운용 수익이 목표의 100% 이상
→ 환매 압력 증가 없음
```

표로 정리하면 다음과 같다.

| 목표 달성률 | 환매 압력 증가 |
|---:|---:|
| 100% 이상 | +0 |
| 75% 이상 100% 미만 | +1 |
| 50% 이상 75% 미만 | +2 |
| 50% 미만 | +3 |

---

## 7. 분기 목표 미달 예시

분기 목표가 20인 경우:

| 분기 운용 수익 | 목표 달성률 | 결과 |
|---:|---:|---:|
| 20 이상 | 100% 이상 | 성공, 환매 압력 +0 |
| 15~19 | 75% 이상 100% 미만 | 환매 압력 +1 |
| 10~14 | 50% 이상 75% 미만 | 환매 압력 +2 |
| 0~9 | 50% 미만 | 환매 압력 +3 |

예시:

```text
현재 환매 압력: 8
분기 목표: 20
분기 운용 수익: 11

목표 달성률: 55%
→ 환매 압력 +2
→ 최종 환매 압력 10
→ 대규모 환매 발생
→ 런 실패
```

---

## 8. 즉시 한도 검사

환매 압력은 증가하는 즉시 한도 도달 여부를 검사한다.

```text
환매 압력 증가
→ 즉시 검사
→ 10 이상이면 런 실패
```

적용 지점:

```text
- 예약 액션 직후
- 분기 마감 목표 미달 처리 직후
```

환매 압력이 10 이상인 상태로 다음 처리가 계속 진행되면 안 된다.

```text
환매 압력 10 이상
→ 즉시 런 실패
→ 다음 영업일 / 다음 분기 / 최종 정산 진행 없음
```

---

## 9. 런 실패 처리

환매 압력으로 인한 런 실패 사유는 다음 문구를 사용한다.

```text
대규모 환매 발생
```

실패 흐름:

```text
환매 압력 10 이상
→ 현재 진행 중인 처리 중단
→ RunState.Failed
→ 런 실패 화면 표시
```

런 실패가 발생하면 다음 흐름으로 이동하지 않는다.

```text
- 다음 영업일 시작 없음
- 다음 분기 시작 없음
- 4Q 휴가 없음
- 최종 정산 없음
```

---

## 10. 환매 압력과 분기 정산 UI

분기 마감에서 환매 압력이 증가했지만 한도에 도달하지 않았다면, 분기 정산 UI에 증가량과 현재값을 표시한다.

실패하지 않은 경우 예시:

```text
분기 운용 수익: 14
분기 목표: 20
달성률: 70%

환매 압력 증가: +2
현재 환매 압력: 6 / 10
```

성공한 경우 예시:

```text
분기 운용 수익: 24
분기 목표: 20
달성률: 120%

환매 압력 증가: 없음
현재 환매 압력: 4 / 10
```

환매 압력이 10 이상이 되어 런 실패가 발생하면 분기 정산 UI 대신 실패 화면으로 이동한다.

---

## 11. 환매 압력과 예약 UI

시장 카드 상세보기에서 예약 버튼은 환매 압력을 증가시키는 행동이다.

예약 버튼 옆이나 툴팁에 리스크를 표시할 수 있다.

예시:

```text
예약
딜 +1, 환매 압력 +1
```

또는 툴팁:

```text
이 카드를 예약 구역에 보관합니다.
딜을 1개 얻고, 환매 압력이 1 증가합니다.
시장 테이프가 진행됩니다.
```

MVP에서는 버튼 옆 간단 표기만으로 충분하다.

---

## 12. 환매 압력과 최종 정산

최종 정산에서 환매 압력은 최종 평가 등급을 낮추지 않는다.

```text
최종 평가
= 최종 운용가치 기준
```

환매 압력은 별도의 운용 코멘트에 사용한다.

```text
운용 코멘트
= 최종 평가 등급 × 환매 압력 단계
```

즉, 환매 압력은 최종 등급 계산에는 반영하지 않지만, 플레이 스타일을 해석하는 데 사용한다.

---

## 13. 환매 압력 단계

최종 운용 코멘트를 위해 환매 압력을 3단계로 구분한다.

초기 임시값:

```text
낮음: 0~2
중간: 3~5
높음: 6+
```

이 기준은 테이블 조정형으로 둔다.

```text
RedemptionPressureLevelTable에서 조정 가능
```

단계 enum 예시:

```csharp
public enum RedemptionPressureLevel
{
    Low,
    Medium,
    High
}
```

---

## 14. 운용 코멘트와의 관계

최종 정산의 운용 코멘트는 다음 조합으로 결정한다.

```text
최종 평가 등급 5단계
×
환매 압력 단계 3단계
```

최종 평가 등급:

```text
D / C / B / A / S
```

환매 압력 단계:

```text
Low / Medium / High
```

총 15개 코멘트를 준비할 수 있다.

예시:

```text
S + Low
→ 압도적인 성과와 안정성을 모두 달성한 운용입니다.

A + High
→ 높은 환매 압력을 감수하며 스타급 성과를 낸 공격적인 운용입니다.

C + High
→ 리스크는 컸지만 성과로 충분히 이어지지 못한 운용입니다.
```

---

## 15. 환매 압력 증가 함수

환매 압력 증가 함수는 증가량을 더한 뒤 즉시 한도 검사를 수행한다.

```csharp
void AddRedemptionPressure(int amount)
{
    if (amount <= 0)
        return;

    CurrentRedemptionPressure += amount;

    if (CurrentRedemptionPressure >= MaxRedemptionPressure)
    {
        TriggerRunFailureByRedemption();
        return;
    }
}
```

환매 압력 증가 후에는 호출부에서 런 상태를 확인해야 한다.

```csharp
AddRedemptionPressure(1);

if (CurrentRunState == RunState.Failed)
    return;
```

---

## 16. 런 실패 함수

```csharp
void TriggerRunFailureByRedemption()
{
    CurrentRunState = RunState.Failed;

    ShowRunFailureScreen("대규모 환매 발생");
}
```

실패 화면에서는 최소한 다음 정보를 표시한다.

```text
- 실패 사유: 대규모 환매 발생
- 최종 도달 회계년도 / 분기
- 최종 운용가치
- 총 운용 수익
- 환매 압력
```

최종 정산과 동일한 평가 화면은 표시하지 않는다.
실패 화면은 별도 처리한다.

---

## 17. 분기 목표 미달 증가량 함수

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

## 18. 환매 압력 단계 판정 함수

```csharp
RedemptionPressureLevel GetRedemptionPressureLevel(
    int redemptionPressure
)
{
    return RedemptionPressureLevelTable
        .Where(data => redemptionPressure >= data.MinRedemptionPressure)
        .OrderByDescending(data => data.MinRedemptionPressure)
        .First()
        .Level;
}
```

초기 데이터 예시:

```text
Low
min_redemption_pressure: 0

Medium
min_redemption_pressure: 3

High
min_redemption_pressure: 6
```

---

## 19. 데이터 구조

### 19.1 RedemptionPressureState

```csharp
public class RedemptionPressureState
{
    public int CurrentRedemptionPressure;
    public int MaxRedemptionPressure;
}
```

---

### 19.2 RedemptionPressureLevelData

```csharp
public class RedemptionPressureLevelData
{
    public RedemptionPressureLevel Level;
    public int MinRedemptionPressure;
}
```

---

### 19.3 FinalManagementCommentData

```csharp
public class FinalManagementCommentData
{
    public string Id;

    public string FinalRatingGrade;
    public RedemptionPressureLevel RedemptionPressureLevel;

    public string Comment;
}
```

---

## 20. 처리 우선순위

환매 압력 한도 도달은 다른 진행보다 우선한다.

예약 액션 중:

```text
예약 실행
→ 환매 압력 +1
→ 10 이상이면 런 실패
→ 실패 시 영업일 종료 진행 안 함
```

분기 마감 중:

```text
분기 목표 미달
→ 환매 압력 +N
→ 10 이상이면 런 실패
→ 실패 시 다음 분기/휴가/최종 정산 진행 안 함
```

핵심 원칙:

```text
환매 압력은 10 이상인 상태로 방치되지 않는다.
도달 즉시 런 실패다.
```

---

## 21. 구현 시 주의사항

```text
- 환매 압력 한도는 10이다.
- 환매 압력 10 이상이면 즉시 런 실패다.
- 예약 액션은 환매 압력 +1을 발생시킨다.
- 분기 목표 미달은 달성률에 따라 환매 압력 +1/+2/+3을 발생시킨다.
- 자원 확보는 환매 압력을 증가시키지 않는다.
- 시장 카드 매수는 환매 압력을 증가시키지 않는다.
- 예약 카드 매수는 환매 압력을 증가시키지 않는다.
- 환매 압력 증가 후 즉시 한도 검사를 한다.
- 런 실패 시 이후 영업일/분기/회계년도 진행을 중단한다.
- 최종 평가 등급은 환매 압력으로 낮아지지 않는다.
- 환매 압력은 최종 운용 코멘트에 사용한다.
- 환매 압력 단계 기준은 테이블 조정형으로 둔다.
