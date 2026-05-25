# 16. 분기 마감 시스템

## 1. 목적

이 문서는 분기의 마지막 영업일이 끝난 뒤 처리되는 분기 마감 흐름을 정의한다.

분기 마감의 역할은 다음과 같다.

```text
- 이번 분기 현금 흐름 확정
- 확정 미션의 미션 수익 계산
- 분기 목표 달성률 계산
- 목표 미달 시 월세 밀림 증가
- 월세 밀림 한도 도달 여부 확인
- 분기 마감 요약 표시
- 다음 분기, 휴가, 또는 최종 정산으로 진행
```

분기 마감은 플레이어가 이번 분기 동안 얼마나 안정적으로 수익을 만들었는지 확인하는 구간이다.

---

## 2. 진입 조건

분기 마감은 현재 분기의 마지막 영업일이 종료되면 발생한다.

```text
마지막 영업일 종료
-> 남은 영업일 0
-> 분기 마감 진입
```

마지막 영업일 종료 뒤에는 새 영업일 시작 처리를 하지 않는다.

```text
마지막 영업일 종료
-> 다음 영업일 없음
-> 배당금 발생 없음
-> 시장 테이프 진행 없음
-> 분기 마감
```

---

## 3. 분기 목표 판정값

v3 기준 분기 목표 판정값은 해당 분기의 현금 흐름과 미션 수익의 합계다.

현금 흐름에 포함되는 항목:

```text
- 보유 주식의 영업일 시작 배당금
- 주식 매도 현금
```

미션 수익:

```text
- 확정 미션이 분기말 현재 포트폴리오 기준으로 만든 평가 수익
- 현금에는 더하지 않음
- 월세 밀림 판정에는 항상 포함
```

분기 목표 판정값에 포함되지 않는 항목:

```text
- 소모형 자원 카드로 획득한 현금
- 시작 현금
- 단순 환급 또는 비용 취소
```

소모형 자원 카드로 획득한 현금은 투자 재원이며, 분기 목표 달성에 직접 반영되는 수익이 아니다.

---

## 4. 분기 마감 순서

분기 마감 처리 순서는 다음과 같다.

```text
1. 분기 마감 진입
2. 보유 주식 및 확정 미션 정산 연출 표시
3. 확정 미션이 있으면 미션 수익 계산
4. 최종 현금 흐름과 미션 수익 확정
5. 분기 목표 달성률 계산
6. 월세 밀림 증가량 계산
7. 월세 밀림 증가 적용
8. 파산 여부 확인
9. 파산하지 않았다면 분기 마감 요약 표시
10. 플레이어가 다음 진행 입력
11. 다음 분기 / 휴가 / 최종 정산으로 이동
```

미션 수익은 달성률 계산 전에 먼저 확정한다.

```text
미션 수익 계산
-> 현금 흐름 + 미션 수익 확정
-> 분기 목표 달성률 계산
```

---

## 5. 미션 수익

분기 마감에서는 확정 미션의 미션 수익을 별도 행으로 보여준다.

미션 수익은 현금이 아니다.
현재 현금, 현재 분기 현금 흐름, 회계연도 수익, 총 수익에 더하지 않는다.

```text
미션 수익 +N
→ 분기 목표 판정값에 포함
→ 현금 잔고에는 미반영
```

미션 수익의 구체 계산식은 미션 데이터 테이블에서 조정한다.

---

## 6. 분기 목표

각 분기는 목표 수익을 가진다.

```text
분기 목표
= 해당 분기에서 달성해야 하는 수익 기준
```

분기 성공 여부는 현재 보유 현금이 아니라 현금 흐름과 미션 수익의 합으로 판정한다.

```text
현재 현금 >= 분기 목표
-> 성공 판정 아님

현금 흐름 + 미션 수익 >= 분기 목표
-> 분기 성공
```

---

## 7. 목표 달성률

목표 달성률은 다음 방식으로 계산한다.

```text
목표 달성률 = (현금 흐름 + 미션 수익) / 분기 목표
```

예시:

```text
분기 목표: 20
현금 흐름: 11
미션 수익: 3

목표 달성률 = 14 / 20 = 70%
```

분기 목표가 0 이하인 데이터는 만들지 않는 것을 원칙으로 한다.
방어 코드에서는 목표가 0 이하일 경우 성공으로 처리해 0 나누기 오류를 막는다.

---

## 8. 성공 판정

현금 흐름과 미션 수익의 합이 분기 목표 이상이면 성공이다.

```text
CurrentQuarterCashFlow + CurrentQuarterMissionRevenue >= CurrentQuarterTargetRevenue
-> 분기 성공
```

성공 시 처리:

```text
- 월세 밀림 증가 없음
- 별도 보상 없음
- 다음 일정 진행
```

분기 성공은 월세가 더 밀리지 않는 것 자체가 보상이다.

---

## 9. 실패 판정과 월세 밀림 증가

현금 흐름과 미션 수익의 합이 분기 목표보다 낮으면 실패다.

```text
CurrentQuarterCashFlow + CurrentQuarterMissionRevenue < CurrentQuarterTargetRevenue
-> 분기 실패
```

분기 실패 시 목표 달성률에 따라 월세 밀림이 증가한다.

```text
수익 >= 목표
-> +0

수익 < 목표
그리고 달성률 >= 75%
-> +1

달성률 < 75%
그리고 달성률 >= 50%
-> +2

달성률 < 50%
-> +3
```

표로 정리하면 다음과 같다.

| 목표 달성률 | 월세 밀림 증가 |
|---:|---:|
| 100% 이상 | +0 |
| 75% 이상 100% 미만 | +1 |
| 50% 이상 75% 미만 | +2 |
| 50% 미만 | +3 |

---

## 10. 파산 판정

분기 마감에서 월세 밀림이 증가하면 즉시 한도를 확인한다.

```text
월세 밀림 증가
-> 즉시 한도 확인
```

월세 밀림 한도는 10이다.

```text
월세 밀림 >= 10
-> 파산
-> 게임 오버
```

파산이 발생하면 분기 마감 요약을 표시하지 않고 파산 화면으로 이동한다.

---

## 11. 분기 마감 요약 UI

파산하지 않았다면 분기 마감 요약 UI를 표시한다.

표시 정보:

```text
- 현금 흐름
- 미션 수익
- 분기 목표
- 목표 달성률
- 월세 밀림 증가량
- 현재 월세 밀림
```

성공 예시:

```text
분기 마감

현금 흐름: 21
미션 수익: +3
분기 목표: 20
달성률: 120%

월세 밀림 증가: 없음
현재 월세 밀림: 4 / 10
```

실패 예시:

```text
분기 마감

현금 흐름: 11
미션 수익: +3
분기 목표: 20
달성률: 70%

월세 밀림 증가: +2
현재 월세 밀림: 6 / 10
```

---

## 12. 다음 진행

분기 마감 요약에서 계속 버튼을 누르면 현재 위치에 따라 다음 일정으로 이동한다.

```text
1, 2회계연도 1Q / 2Q 마감
-> 다음 분기 시작

1, 2회계연도 3Q 마감
-> 4Q 휴가 화면

3회계연도 1Q / 2Q / 3Q 마감
-> 다음 분기 시작

3회계연도 4Q 마감
-> 최종 정산
```

새 분기가 시작되면 시장 테이프를 갱신한다.
새 분기의 첫 영업일은 시장 테이프 진행을 하지 않고 시작한다.

---

## 13. 상태 데이터 예시

```csharp
public class QuarterEndResult
{
    public int FiscalYearIndex;
    public int QuarterIndex;

    public int QuarterCashFlow;
    public int MissionRevenue;
    public int QuarterTarget;
    public float AchievementRate;

    public int RentArrearsIncrease;
    public int CurrentRentArrears;

    public bool IsSuccess;
    public bool IsBankrupt;
}
```

현금 흐름 상태 예시:

```csharp
public class RunRevenueState
{
    public int TotalRevenue;
    public int CurrentFiscalYearRevenue;
    public int CurrentQuarterCashFlow;
    public int CurrentQuarterMissionRevenue;

    public List<QuarterRevenueData> QuarterRevenueHistory;
}
```

분기 시작 시 현재 분기 현금 흐름과 미션 수익을 초기화한다.

```csharp
void StartQuarter()
{
    CurrentQuarterCashFlow = 0;
    CurrentQuarterMissionRevenue = 0;
}
```

---

## 14. 처리 예시

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
    int missionRevenue = CalculateMissionRevenue();

    QuarterEndResult result = BuildQuarterEndResult(missionRevenue);

    int rentArrearsIncrease =
        GetRentArrearsIncreaseFromQuarterResult(
            result.QuarterCashFlow + result.MissionRevenue,
            result.QuarterTarget
        );

    result.RentArrearsIncrease = rentArrearsIncrease;

    if (rentArrearsIncrease > 0)
    {
        AddRentArrears(rentArrearsIncrease);

        result.CurrentRentArrears = CurrentRentArrears;

        if (IsBankruptByRentArrears())
        {
            result.IsBankrupt = true;
            TriggerBankruptcy();
            return;
        }
    }

    ShowQuarterEndSummary(result);
}
```

---

## 15. 구현 시 주의사항

```text
- 분기 마감은 마지막 영업일 종료 뒤 발생한다.
- 마지막 영업일 종료 뒤에는 배당금을 지급하지 않는다.
- 미션 수익은 분기 목표 판정에 포함한다.
- 미션 수익은 현금에 더하지 않는다.
- 소모형 자원 카드로 얻은 현금은 현금 흐름에 포함하지 않는다.
- 목표 달성률 계산 전에 미션 수익을 먼저 확정한다.
- 분기 성공 시 별도 보상은 없다.
- 분기 실패 시 달성률에 따라 월세 밀림을 증가시킨다.
- 월세 밀림이 10 이상이면 즉시 파산한다.
- 파산하면 다음 분기, 휴가, 최종 정산으로 진행하지 않는다.
- 새 분기 시작 시 시장 테이프는 진행이 아니라 갱신된다.
```
