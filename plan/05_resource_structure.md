# 05. 자원 구조

## 1. 목적

이 문서는 게임에서 사용하는 자원의 종류, 역할, 보유 한도, 획득/소비 규칙을 정의한다.

본 게임의 자원은 크게 다음으로 나뉜다.

```text
- 현금
- 전문 자원
  - 리서치
  - 신용
  - 원자재
- 딜
```

각 자원은 자산 매수, 자원 확보, 예약, 분기 운영에 사용된다.

---

## 2. 자원 분류

### 2.1 기본 자원

자원 확보로 선택할 수 있는 기본 자원은 다음 4개이다.

```text
- 현금
- 리서치
- 신용
- 원자재
```

딜은 자원 확보로 획득할 수 없다.

```text
딜은 자원 확보 대상이 아니다.
```

---

### 2.2 전문 자원

전문 자원은 자산 매수 시 특정 비용 슬롯에 배치하는 칩형 자원이다.

전문 자원은 다음 3개이다.

```text
- 리서치
- 신용
- 원자재
```

전문 자원은 합산 보유 한도를 가진다.

```text
리서치 + 신용 + 원자재 <= 10
```

---

### 2.3 딜

딜은 예약을 통해 얻는 특수 자원이다.

딜은 전문 자원 비용 슬롯을 대체할 수 있으며, 추가로 현금 비용을 낮춘다.

```text
딜 1개
= 전문 자원 비용 슬롯 1칸 대체
+ 기본 현금 비용 1 감소
```

딜은 최대 3개까지 보유할 수 있다.

```text
딜 최대 보유량: 3개
```

---

## 3. 자원별 역할

| 자원 | 역할 | 보유 한도 |
|---|---|---:|
| 현금 | 자산 매수의 기본 비용 지불 | 없음 |
| 리서치 | 리서치 비용 슬롯 지불 | 전문 자원 합계 10 |
| 신용 | 신용 비용 슬롯 지불 | 전문 자원 합계 10 |
| 원자재 | 원자재 비용 슬롯 지불 | 전문 자원 합계 10 |
| 딜 | 전문 자원 비용 대체 및 현금 비용 감소 | 3 |

---

## 4. 현금

현금은 자산 매수에 사용하는 기본 결제 자원이다.

현금은 다음 방식으로 증가할 수 있다.

```text
- 자원 확보로 현금 선택
- 보유 자산의 영업일 시작 현금
- 분기 마감 정산 수익
- 기타 운용 성과 효과
```

현금은 증가 방식에 따라 성격이 다르다.

```text
자원 확보로 얻은 현금
= 조달 현금

보유 자산의 영업일 시작 현금 / 분기 마감 정산 수익 / 기타 운용 성과 현금
= 운용 수익
```

운용 수익은 분기 목표 달성, 회계년도 요약, 최종 정산에 사용된다.
조달 현금은 운용 수익에 포함하지 않는다.

---

## 5. 조달 현금과 운용 수익

현금이 늘었다고 모두 운용 수익으로 기록하지 않는다.

### 5.1 조달 현금

조달 현금은 플레이어가 자원 확보를 통해 확보한 현금이다.

```text
자원 확보
→ 현금 선택
→ CurrentCash 증가
→ CurrentQuarterEarnedCash 증가 없음
```

조달 현금은 생존과 매수를 위한 자금 확보 수단이다.
성과 현금으로 취급하지 않는다.

---

### 5.2 운용 수익

운용 수익은 자산 운용 결과로 발생한 현금이다.

포함 대상:

```text
- 보유 자산의 영업일 시작 현금
- 분기 마감 정산 수익
- 기타 운용 성과로 분류된 현금
```

운용 수익은 다음 지표에 반영된다.

```text
- 분기 운용 수익
- 회계년도 운용 수익
- 총 운용 수익
```

---

## 6. 전문 자원

전문 자원은 자산 매수 시 전문 비용 슬롯에 직접 배치하는 칩형 자원이다.

전문 자원은 다음 3개이다.

```text
리서치
신용
원자재
```

각 전문 자원은 특정 비용 슬롯과 매칭된다.

```text
리서치 칩
→ 리서치 비용 슬롯에 배치 가능

신용 칩
→ 신용 비용 슬롯에 배치 가능

원자재 칩
→ 원자재 비용 슬롯에 배치 가능
```

딜은 예외적으로 모든 전문 자원 비용 슬롯에 배치할 수 있다.

---

## 7. 전문 자원 보유 한도

전문 자원은 개별 한도가 아니라 합산 한도를 가진다.

```text
리서치 + 신용 + 원자재 <= 10
```

예시:

```text
리서치 4
신용 3
원자재 3
합계 10 / 10
```

이 상태에서는 전문 자원을 추가로 획득할 수 없다.
단, 현금과 딜은 전문 자원 합계에 포함되지 않는다.

---

## 8. 전문 자원 한도 초과 처리

전문 자원 합계가 10을 초과하는 경우, 기존 보유 자원은 건드리지 않는다.

```text
기존 보유 자원은 유지한다.
신규 획득분 중 초과분만 폐기한다.
```

플레이어에게 버릴 자원을 선택하게 하지 않는다.

예시:

```text
현재 보유:
리서치 4
신용 3
원자재 3
합계 10 / 10

보상으로 리서치 +1 획득 시도
→ 리서치 +1 폐기
→ 보유량 변화 없음
```

다른 예시:

```text
현재 보유:
리서치 3
신용 3
원자재 3
합계 9 / 10

보상으로 원자재 +2 획득 시도
→ 원자재 +1 획득
→ 원자재 +1 폐기
→ 최종 합계 10 / 10
```

---

## 9. 전문 자원 초과 폐기 메시지

전문 자원 한도 초과로 폐기되는 경우, 폐기된 자원명을 메시지에 포함한다.

문구 형식:

```text
자원칩 최대 보유: [자원명] +[수량] 폐기
```

예시:

```text
자원칩 최대 보유: 리서치 +1 폐기
```

```text
자원칩 최대 보유: 신용 +2 폐기
```

여러 자원이 동시에 폐기되면 한 메시지에 묶어서 표시한다.

```text
자원칩 최대 보유: 리서치 +1, 신용 +1 폐기
```

---

## 10. 자원 확보에서의 전문 자원 제한

자원 확보 화면에서는 전문 자원 한도를 넘는 선택을 애초에 막는다.

전문 자원 합계가 10이면 전문 자원 버튼을 비활성화한다.

```text
리서치 + 신용 + 원자재 = 10 / 10
→ 리서치 비활성화
→ 신용 비활성화
→ 원자재 비활성화
→ 현금만 활성화
```

전문 자원 합계가 9인 상태에서 전문 자원을 하나 얻으면 합계가 10이 된다.

```text
전문 자원 9 / 10
→ 리서치 클릭
→ 리서치 +1
→ 전문 자원 10 / 10
→ 리서치 / 신용 / 원자재 비활성화
→ 현금만 활성화
```

이때 현금이 유효 선택지로 남아 있으면 계속 진행한다.
현재 선택 상태에서 더 이상 유효 선택지가 없으면 자원 확보를 자동 종료한다.

자동 종료 메시지:

```text
자원칩 최대 한도 도달
```

---

## 11. 딜

딜은 예약 액션을 통해 얻는 특수 자원이다.

예약 액션:

```text
시장 카드 예약
→ 딜 +1
→ 환매 압력 +1
→ 시장 테이프 진행
→ 영업일 종료
```

딜은 최대 3개까지 보유한다.

```text
딜 최대 보유량: 3개
```

딜은 전문 자원 합계 10개 제한에 포함되지 않는다.

---

## 12. 딜의 매수 기능

딜은 자산 매수 시 전문 자원 비용 슬롯을 대체할 수 있다.

```text
리서치 슬롯
→ 딜 배치 가능

신용 슬롯
→ 딜 배치 가능

원자재 슬롯
→ 딜 배치 가능
```

딜이 올라간 슬롯은 결제 완료 슬롯으로 본다.

```text
딜이 올라간 전문 자원 슬롯
= 이미 결제된 슬롯
= 빈 슬롯 아님
= 자동 배치 대상 아님
```

딜 1개는 추가로 기본 현금 비용을 1 낮춘다.

```text
투여한 딜 수만큼 기본 현금 비용 감소
```

딜 할인은 인플레이션 적용 전에 반영한다.

```text
기본 현금 비용
→ 딜 할인 적용
→ 인플레이션 적용
→ 최종 현금 비용
```

---

## 13. 딜 보유 한도 초과 처리

딜이 이미 3개인 상태에서도 예약 액션은 가능하다.

```text
딜 3 / 3
→ 예약 가능
```

단, 예약으로 얻는 추가 딜은 한도를 초과하므로 폐기한다.

```text
딜 3 / 3
→ 예약 실행
→ 딜 +1 시도
→ 추가 딜 폐기
```

이때 안내 메시지를 표시한다.

```text
딜 최대 보유: 추가 딜 폐기
```

딜 한도는 예약 가능 여부를 막지 않는다.
예약 버튼 비활성화 조건에는 딜 보유량을 포함하지 않는다.

---

## 14. 딜 한도 초과 메시지

딜 한도 초과로 추가 딜이 폐기될 때는 다음 메시지를 표시한다.

```text
딜 최대 보유: 추가 딜 폐기
```

표시 상황:

```text
현재 딜 3 / 3
→ 시장 카드 예약
→ 예약은 정상 실행
→ 추가 딜만 폐기
→ 메시지 표시
```

딜이 0~2개인 상태에서 예약한 경우에는 일반 딜 획득 연출만 표시하면 된다.

---

## 15. 자원 획득 함수 구분

현금은 증가 원인에 따라 기록 방식이 다르다.

구현상 다음 함수를 분리하는 것을 권장한다.

```csharp
void AddFundingCash(int amount)
{
    CurrentCash += amount;
    // 운용 수익에는 포함하지 않음
}
```

```csharp
void AddPerformanceCash(int amount)
{
    CurrentCash += amount;

    CurrentQuarterEarnedCash += amount;
    CurrentFiscalYearEarnedCash += amount;
    TotalEarnedCash += amount;
}
```

전문 자원은 한도 처리를 포함해 추가한다.

```csharp
void AddProfessionalResource(ResourceType resourceType, int amount)
{
    // 리서치 / 신용 / 원자재만 허용
    // 전문 자원 합계 10을 넘는 신규 획득분은 폐기
}
```

딜은 별도 한도를 가진다.

```csharp
void AddDeal(int amount)
{
    // 딜 최대 3
    // 초과분은 폐기
}
```

---

## 16. ResourceType 제안

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

각 자원의 표시 이름은 별도 로컬라이즈 테이블 또는 display name 함수로 관리한다.

```csharp
string GetResourceDisplayName(ResourceType resourceType)
{
    switch (resourceType)
    {
        case ResourceType.Cash:
            return "현금";
        case ResourceType.Research:
            return "리서치";
        case ResourceType.Credit:
            return "신용";
        case ResourceType.Commodity:
            return "원자재";
        case ResourceType.Deal:
            return "딜";
        default:
            return "";
    }
}
```

---

## 17. ResourceState 제안

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

전문 자원 합계를 계산하는 함수:

```csharp
int GetProfessionalResourceTotal()
{
    return Research + Credit + Commodity;
}
```

한도 상수:

```csharp
const int MaxProfessionalResourceTotal = 10;
const int MaxDeal = 3;
```

---

## 18. 전문 자원 추가 처리 예시

```csharp
List<DiscardedResourceData> AddProfessionalResource(
    ResourceType resourceType,
    int amount
)
{
    if (!IsProfessionalResource(resourceType))
        return new List<DiscardedResourceData>();

    int currentTotal = GetProfessionalResourceTotal();
    int remainingCapacity = MaxProfessionalResourceTotal - currentTotal;

    int gainAmount = Math.Max(0, Math.Min(amount, remainingCapacity));
    int discardAmount = amount - gainAmount;

    ApplyProfessionalResourceGain(resourceType, gainAmount);

    var discarded = new List<DiscardedResourceData>();

    if (discardAmount > 0)
    {
        discarded.Add(new DiscardedResourceData
        {
            ResourceType = resourceType,
            Amount = discardAmount
        });
    }

    return discarded;
}
```

---

## 19. 딜 추가 처리 예시

```csharp
void AddDealFromReservation()
{
    if (CurrentDeal >= MaxDeal)
    {
        ShowShortFeedback("딜 최대 보유: 추가 딜 폐기");
        return;
    }

    CurrentDeal += 1;
}
```

---

## 20. 자원 사용 처리

자산 매수 시 자원은 매수 확정 단계에서만 소비한다.

```text
카드 상세보기 진입
→ 칩 배치
→ 아직 자원 소비 아님

매수 확정
→ 결제 검증
→ 전문 자원 칩 / 딜 소비
→ 현금 차감
```

결제 대기 중 취소하면 올려둔 칩은 전부 원래 보유 상태로 돌아간다.

```text
매수 취소
→ 배치 칩 전부 회수
→ 자원 소비 없음
```

---

## 21. 자원 표시 원칙

현금은 숫자로 표시한다.

```text
현금 8
```

전문 자원과 딜은 칩 리소스를 활용해 표시한다.

```text
리서치 칩
신용 칩
원자재 칩
딜
```

다만 보유 자산군이나 태그 요약은 칩 표시 대상이 아니다.
해당 정보는 숫자 텍스트로 표시한다.

```text
주식 5
채권 3
성장 4
방어 2
```

칩으로 표현하는 대상은 전문 자원 및 딜이다.

---

## 22. 구현 시 주의사항

```text
- 현금은 전문 자원 한도에 포함되지 않는다.
- 딜은 전문 자원 한도에 포함되지 않는다.
- 전문 자원 한도는 리서치 + 신용 + 원자재 합계 10이다.
- 전문 자원 한도 초과 시 기존 보유 자원을 버리지 않는다.
- 초과분은 신규 획득분에서만 폐기한다.
- 딜은 최대 3개이다.
- 딜이 3개여도 예약은 가능하다.
- 딜 초과분은 폐기하고 메시지를 표시한다.
- 조달 현금은 운용 수익에 포함하지 않는다.
- 보유 자산의 영업일 시작 현금과 분기 마감 정산 수익은 운용 수익에 포함한다.
- 자원 소비는 매수 확정 시점에만 발생한다.
```
