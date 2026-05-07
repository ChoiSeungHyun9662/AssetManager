# 14. 추가 매수권

## 1. 목적

이 문서는 추가 매수권의 발생 조건, 사용 범위, 상태 제한, 중첩 처리, 영업일 종료 규칙을 정의한다.

추가 매수권은 특정 자산 매수 효과나 규칙에 의해 발생하는 임시 권리이다.  
추가 매수권을 보유하면 같은 영업일 안에서 자산을 한 번 더 매수할 수 있다.

```text
자산 매수
→ 특정 규칙으로 추가 매수권 발생
→ 같은 영업일 안에서 자산 1장 추가 매수 가능
```

추가 매수권은 자산 매수 전용이다.

```text
추가 매수권으로 가능한 행동:
- 시장 카드 매수
- 예약 카드 매수

추가 매수권으로 불가능한 행동:
- 유동성 확보
- 카드 예약
```

---

## 2. 핵심 규칙

추가 매수권의 핵심 규칙은 다음과 같다.

```text
- 자산 매수 전용이다.
- 시장 카드와 예약 카드 모두 매수 가능하다.
- 다음 영업일로 이월되지 않는다.
- 중첩되지 않는다.
- 추가 매수권을 사용한 매수 후에는 즉시 영업일을 종료한다.
- 추가 매수로도 일반 매수 규칙은 발동한다.
- 단, 추가 매수로 발생한 GrantExtraBuyAction 효과는 무시한다.
```

---

## 3. 발생 조건

추가 매수권은 자산 매수 후 특정 카드 효과 또는 규칙에 의해 발생한다.

예시:

```text
자산 A 매수
→ OnBuyAsset 효과 발동
→ 추가 매수권 1개 부여
```

정확한 발생 조건은 카드 효과 데이터에서 정의한다.

```text
EffectType: GrantExtraBuyAction
```

추가 매수권은 자산 매수 처리 중 매수 관련 규칙을 해석하는 단계에서 발생할 수 있다.

```text
매수 확정
→ 자산 획득
→ 매수 관련 규칙 발동
→ 추가 매수권 발생 여부 확인
```

---

## 4. 추가 매수권 상태

추가 매수권이 발생하면 즉시 영업일을 종료하지 않고, 추가 매수 선택 상태로 들어간다.

```text
첫 번째 자산 매수
→ 추가 매수권 발생
→ 영업일 유지
→ 추가 매수 선택 상태 진입
```

추가 매수 선택 상태에서는 행동 선택 범위가 제한된다.

가능:

```text
- 시장 카드 매수
- 예약 카드 매수
- 다음 영업일
```

불가능:

```text
- 시장 카드 예약
- 유동성 확보
```

---

## 5. 추가 매수권 보유 중 UI 상태

추가 매수권 보유 중에도 시장 영역의 기본 구조는 유지한다.

```text
시장 영역 상태:
- Market
- CardDetail
```

단, 유동성 확보 진입은 막는다.

```text
추가 매수권 보유 중
→ 자원통 비활성화
또는
→ 자원통 클릭 시 불가 피드백
```

시장 카드 상세보기에서는 예약 버튼을 숨기거나 비활성화한다.

권장 처리:

```text
추가 매수권 보유 중 시장 카드 상세보기
→ 예약 버튼 숨김
```

이유:

```text
추가 매수권은 자산 매수 전용이다.
예약은 자산 매수가 아니다.
```

다음 영업일 버튼은 사용할 수 있다.

```text
추가 매수권 보유 중
→ 다음 영업일 가능
→ 추가 매수권 포기
→ 영업일 종료
```

---

## 6. 추가 매수 가능 대상

추가 매수권으로 매수 가능한 대상은 일반 자산 매수와 동일하다.

```text
- 시장 카드
- 예약 카드
```

시장 카드 추가 매수:

```text
시장 카드 클릭
→ 카드 상세보기
→ 매수 확정
→ 시장 카드 매수 처리
→ 산 자리 보충
→ 영업일 종료
```

예약 카드 추가 매수:

```text
예약 카드 클릭
→ 카드 상세보기
→ 매수 확정
→ 예약 카드 매수 처리
→ 예약 슬롯 비움
→ 영업일 종료
```

---

## 7. 추가 매수권 중첩 금지

추가 매수권은 중첩되지 않는다.

```text
추가 매수권 최대 보유량: 1
```

이미 추가 매수권을 보유한 상태에서 추가 매수권을 다시 얻으려 해도 추가로 누적하지 않는다.

```text
추가 매수권 보유 중
→ GrantExtraBuyAction 발생
→ 추가 매수권 추가 부여 없음
```

특히 추가 매수로 산 카드가 다시 추가 매수권을 주는 효과를 가지고 있더라도, 해당 효과는 무시한다.

```text
추가 매수로 카드 매수
→ OnBuyAsset 발동
→ OnBuyAssetWithTag 발동
→ GrantExtraBuyAction만 무시
→ 영업일 종료
```

---

## 8. 다음 영업일로 이월 금지

추가 매수권은 다음 영업일로 이월되지 않는다.

```text
추가 매수권 보유 중
→ 다음 영업일 클릭
→ 추가 매수권 폐기
→ 영업일 종료
```

분기 마감으로 넘어가는 경우에도 추가 매수권은 사라진다.

```text
추가 매수권 보유 중 마지막 영업일
→ 다음 영업일 클릭
→ 추가 매수권 폐기
→ 분기 마감
```

---

## 9. 추가 매수권 사용 후 영업일 종료

추가 매수권을 사용해 두 번째 자산을 매수하면 즉시 영업일을 종료한다.

```text
추가 매수권 보유
→ 자산 매수 확정
→ 추가 매수권 소모
→ 영업일 종료
```

추가 매수 후에는 다시 추가 매수 선택 상태로 들어가지 않는다.

```text
추가 매수 후 GrantExtraBuyAction 발생
→ 무시
→ 영업일 종료
```

---

## 10. 첫 번째 매수와 시장 보충

첫 번째 매수가 시장 카드 매수인 경우, 산 자리는 즉시 보충한다.

```text
시장 카드 A 매수
→ A 자리 새 카드 B 보충
```

이후 추가 매수권이 발생하면 새로 보충된 카드도 추가 매수 대상으로 선택할 수 있다.

```text
A 매수
→ A 자리 B 보충
→ 추가 매수권 발생
→ B도 추가 매수 후보
```

이 규칙은 시장을 자연스럽게 유지하고, 추가 매수권의 선택지를 넓혀준다.

---

## 11. 첫 번째 매수가 예약 카드인 경우

첫 번째 매수가 예약 카드라면 해당 예약 슬롯만 비운다.

```text
예약 카드 A 매수
→ 예약 슬롯 비움
→ 시장 테이프 변화 없음
```

이후 추가 매수권이 발생하면 시장 카드 또는 다른 예약 카드를 매수할 수 있다.

```text
예약 카드 A 매수
→ 추가 매수권 발생
→ 시장 카드 매수 가능
→ 예약 카드 매수 가능
```

---

## 12. 추가 매수 중 예약 불가

추가 매수권 상태에서 시장 카드를 상세보기로 열더라도 예약은 불가능하다.

```text
추가 매수권 상태
→ 시장 카드 상세보기
→ 예약 버튼 숨김
```

또는 UI 사정상 버튼을 유지해야 한다면 비활성화하고 사유를 표시한다.

```text
예약 불가: 추가 매수권은 자산 매수 전용입니다.
```

권장안은 버튼 숨김이다.

---

## 13. 추가 매수 중 유동성 확보 불가

추가 매수권 상태에서는 유동성 확보를 할 수 없다.

```text
추가 매수권 상태
→ 자원통 클릭 불가
```

자원통은 비활성화하거나 클릭 시 짧은 피드백만 준다.

```text
추가 매수권은 자산 매수에만 사용할 수 있습니다.
```

MVP에서는 메시지 없이 비활성화만 해도 된다.

---

## 14. 추가 매수권 포기

플레이어는 추가 매수권을 사용하지 않고 다음 영업일로 넘어갈 수 있다.

```text
추가 매수권 보유
→ 다음 영업일 클릭
→ 추가 매수권 폐기
→ 영업일 종료
```

이때 별도 패널티는 없다.

```text
추가 매수권 포기
→ 추가 패널티 없음
```

---

## 15. 추가 매수권 상태에서 카드 상세보기 닫기

추가 매수권 상태에서 카드를 상세보기로 열었다가 닫을 수 있다.

```text
추가 매수권 상태
→ 카드 상세보기
→ 닫기
→ 시장 상태 복귀
→ 추가 매수권 유지
```

닫기는 영업일을 소비하지 않는다.

```text
카드 보기 = 무료
닫기 = 무료
```

카드 상세보기 닫기 시 결제 대기 중인 칩은 모두 회수한다.

---

## 16. 추가 매수권과 매수 규칙 발동

추가 매수로 산 자산도 일반 매수 규칙을 발동한다.

발동 가능:

```text
- OnBuyAsset
- OnBuyAssetWithTag
- OnBuyFromSource
```

무시 대상:

```text
- GrantExtraBuyAction
```

즉, 추가 매수로 산 카드가 또 추가 매수권을 주더라도 추가 매수권은 발생하지 않는다.

```text
추가 매수로 카드 매수
→ GrantExtraBuyAction 발생 시도
→ 무시
→ 영업일 종료
```

---

## 17. 추가 매수권 발생 처리 순서

첫 번째 매수 후 처리 순서:

```text
1. 매수 확정
2. 자원 소비
3. 자산 획득
4. 시장 슬롯 보충 또는 예약 슬롯 비움
5. 매수 관련 규칙 발동
6. 추가 매수권 발생 여부 확인
7. 추가 매수권이 있으면 추가 매수 선택 상태 진입
8. 추가 매수권이 없으면 영업일 종료
```

---

## 18. 추가 매수권 사용 처리 순서

추가 매수권 사용 시 처리 순서:

```text
1. 추가 매수 대상 카드 상세보기
2. 결제 상태 구성
3. 매수 확정
4. 자원 소비
5. 자산 획득
6. 시장 슬롯 보충 또는 예약 슬롯 비움
7. 매수 관련 규칙 발동
8. GrantExtraBuyAction은 무시
9. 추가 매수권 소모
10. 영업일 종료
```

---

## 19. 상태 데이터

추가 매수권 상태를 관리하기 위한 데이터 예시:

```csharp
public class TurnActionState
{
    public bool HasExtraBuyAction;
    public bool IsWaitingForExtraBuyDecision;
    public bool IsResolvingExtraBuyAction;
}
```

또는 영업일 단위 상태에 포함할 수 있다.

```csharp
public class BusinessDayActionState
{
    public bool HasExtraBuyAction;
    public bool IsWaitingForExtraBuyDecision;
    public bool IsExtraBuyPurchase;
}
```

용어는 내부 구현에서 `Turn`을 사용해도 되지만, 유저-facing 텍스트는 `영업일`을 사용한다.

---

## 20. 추가 매수권 부여 예시

```csharp
void GrantExtraBuyAction()
{
    if (CurrentBusinessDayActionState.IsExtraBuyPurchase)
        return;

    if (CurrentBusinessDayActionState.HasExtraBuyAction)
        return;

    CurrentBusinessDayActionState.HasExtraBuyAction = true;
}
```

---

## 21. 추가 매수 선택 상태 진입 예시

```csharp
void EnterExtraBuyDecisionState()
{
    CurrentBusinessDayActionState.IsWaitingForExtraBuyDecision = true;

    SetMarketAreaState(MarketAreaState.Market);

    UpdateAvailableActionsForExtraBuy();
}
```

---

## 22. 추가 매수 상태에서 입력 제한

```csharp
bool CanUseLiquidityDuringExtraBuy()
{
    return false;
}
```

```csharp
bool CanReserveDuringExtraBuy()
{
    return false;
}
```

```csharp
bool CanBuyAssetDuringExtraBuy()
{
    return CurrentBusinessDayActionState.IsWaitingForExtraBuyDecision;
}
```

---

## 23. 추가 매수 확정 예시

```csharp
void ConfirmExtraBuyPurchase()
{
    CurrentBusinessDayActionState.IsExtraBuyPurchase = true;

    ConfirmPurchase();

    CurrentBusinessDayActionState.HasExtraBuyAction = false;
    CurrentBusinessDayActionState.IsWaitingForExtraBuyDecision = false;
    CurrentBusinessDayActionState.IsExtraBuyPurchase = false;

    EndBusinessDay();
}
```

실제 구현에서는 `ConfirmPurchase()` 내부에서 중복으로 `EndBusinessDay()`가 호출되지 않도록 `PurchaseContext.IsExtraBuyAction`을 사용한다.

---

## 24. 추가 매수권 포기 예시

```csharp
void OnNextBusinessDayClickedDuringExtraBuy()
{
    ClearExtraBuyActionIfAny();

    EndBusinessDay();
}
```

```csharp
void ClearExtraBuyActionIfAny()
{
    CurrentBusinessDayActionState.HasExtraBuyAction = false;
    CurrentBusinessDayActionState.IsWaitingForExtraBuyDecision = false;
    CurrentBusinessDayActionState.IsExtraBuyPurchase = false;
}
```

---

## 25. 매수 규칙 처리 시 GrantExtraBuyAction 무시

```csharp
void ResolvePurchaseRule(
    PurchaseRule rule,
    PurchaseContext context
)
{
    if (context.IsExtraBuyAction
        && rule.EffectType == EffectType.GrantExtraBuyAction)
    {
        return;
    }

    ApplyPurchaseRule(rule, context);
}
```

---

## 26. UI 표시 제안

추가 매수권 상태에서는 상단 또는 시장 영역에 짧은 안내를 표시한다.

예시:

```text
추가 매수 가능
자산 1장을 더 매수하거나 다음 영업일로 넘어갈 수 있습니다.
```

버튼/입력 상태:

```text
시장 카드 클릭 가능
예약 카드 클릭 가능
자원통 비활성화
시장 카드 상세보기 예약 버튼 숨김
다음 영업일 가능
```

---

## 27. 구현 시 주의사항

```text
- 추가 매수권은 자산 매수 전용이다.
- 추가 매수권으로 유동성 확보는 불가능하다.
- 추가 매수권으로 카드 예약은 불가능하다.
- 추가 매수권으로 시장 카드와 예약 카드는 매수 가능하다.
- 추가 매수권은 다음 영업일로 이월되지 않는다.
- 추가 매수권은 중첩되지 않는다.
- 추가 매수로도 일반 매수 규칙은 발동한다.
- 추가 매수로 발생한 GrantExtraBuyAction은 무시한다.
- 추가 매수 후에는 즉시 영업일을 종료한다.
- 추가 매수권을 사용하지 않고 다음 영업일로 넘길 수 있다.
- 다음 영업일로 넘기면 추가 매수권은 폐기된다.
- 첫 번째 시장 카드 매수 후 보충된 새 카드도 추가 매수 대상이 될 수 있다.
```
