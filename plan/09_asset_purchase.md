# 09. 자산 매수 시스템

## 1. 목적

이 문서는 자산 매수 시스템의 규칙, 결제 흐름, 처리 순서, 예외 처리를 정의한다.

자산 매수는 플레이어가 시장 카드 또는 예약 카드를 실제 보유 자산으로 전환하는 행동이다.
매수 확정 시 영업일을 소비한다.

```text
카드 상세보기
→ 칩 배치
→ 매수 확정
→ 영업일 소비
```

카드 상세보기 진입, 칩 배치, 칩 회수, 닫기는 영업일을 소비하지 않는다.
영업일을 소비하는 시점은 `매수 확정`이다.

---

## 2. 매수 대상

매수 가능한 대상은 다음 두 종류이다.

```text
- 시장 카드
- 예약 카드
```

시장 카드와 예약 카드는 모두 자산 매수로 판정한다.

```text
시장 카드 매수
= 자산 매수

예약 카드 매수
= 자산 매수
```

단, 매수 출처는 별도로 기록한다.

```csharp
public enum PurchaseSource
{
    MarketTape,
    Reserved
}
```

매수 출처는 일부 규칙 판정이나 시장 처리에 사용한다.

---

## 3. 매수 진입

자산 매수는 카드 상세보기 상태에서 진행한다.

```text
시장 카드 클릭
→ 카드 상세보기
→ 매수 결제 대기 상태

예약 카드 클릭
→ 카드 상세보기
→ 매수 결제 대기 상태
```

카드 상세보기는 시장 영역을 대체한다.

카드 상세보기 진입 자체는 영업일을 소비하지 않는다.

```text
카드 보기 = 무료
```

---

## 4. 매수 결제 구성 요소

자산 매수에는 다음 비용이 사용된다.

```text
- 현금 비용
- 전문 자원 비용
- 딜
```

현금 비용은 숫자로 표시한다.

```text
현금 5
```

전문 자원 비용은 슬롯으로 표시한다.

```text
리서치 비용 2
→ 리서치 슬롯 2개

신용 비용 1
→ 신용 슬롯 1개
```

딜은 전문 자원 슬롯에 배치할 수 있는 예약 보상 자원이다.

---

## 5. 현금 비용

현금 비용은 카드 데이터의 기본 비용이다.

```csharp
public int CashCost;
```

최종 현금 비용은 다음 순서로 계산한다.

```text
기본 현금 비용
→ 비용 수정 효과 합산
→ 딜 할인 적용
→ 인플레이션 적용
→ 최종 현금 비용
```

딜 할인은 인플레이션 적용 전에 반영한다.

```text
딜 1개 사용
→ 기본 현금 비용 -1
```

최종 현금 비용은 0보다 작아질 수 없다.

```text
최종 현금 비용 최소값: 0
```

---

## 6. 전문 자원 비용

전문 자원 비용은 다음 3종이다.

```text
- 리서치
- 신용
- 원자재
```

전문 자원 비용은 비용 슬롯으로 표시한다.

예시:

```text
리서치 2
신용 1
원자재 0
```

표시:

```text
리서치 슬롯 2개
신용 슬롯 1개
```

각 슬롯은 다음 중 하나의 상태를 가진다.

```text
- 비어 있음
- 해당 전문 자원 칩 배치됨
- 딜 배치됨
```

모든 전문 자원 슬롯이 채워져야 매수 가능하다.

---

## 7. 딜의 결제 역할

딜은 전문 자원 슬롯을 대체할 수 있다.

```text
딜
→ 리서치 슬롯 배치 가능
→ 신용 슬롯 배치 가능
→ 원자재 슬롯 배치 가능
```

딜이 배치된 슬롯은 결제 완료 슬롯으로 본다.

```text
딜이 배치된 슬롯
= 해당 전문 자원 비용 결제 완료
```

딜은 추가로 현금 비용을 낮춘다.

```text
투여한 딜 수만큼 기본 현금 비용 감소
```

예시:

```text
기본 현금 비용 5
딜 2개 사용
→ 딜 할인 후 기본 현금 비용 3
```

---

## 8. 매수 가능 조건

매수 버튼은 다음 조건을 모두 만족할 때 활성화된다.

```text
1. 선택 카드가 유효하다.
2. 현재 카드 상세보기 상태이다.
3. 모든 전문 자원 비용 슬롯이 채워져 있다.
4. 최종 현금 비용을 지불할 수 있다.
5. 현재 행동 상태에서 매수가 허용된다.
```

전문 자원 비용이 없는 카드라면 3번 조건은 자동 충족으로 본다.

```text
전문 자원 비용 없음
→ 전문 자원 슬롯 조건 충족
```

현금 비용이 0이라면 현금 조건은 자동 충족으로 본다.

---

## 9. 매수 불가 조건

다음 중 하나라도 해당하면 매수할 수 없다.

```text
- 선택 카드가 없음
- 카드 상세보기 상태가 아님
- 전문 자원 슬롯이 비어 있음
- 최종 현금 비용보다 현재 현금이 부족함
- 추가 매수권 상태에서 매수 외 행동을 시도함
- 이미 보유 중인 카드임
- 카드가 현재 유효한 시장/예약 위치에 없음
```

매수 불가 시 자원은 소비하지 않는다.

```text
매수 검증 실패
→ 자원 소비 없음
→ 카드 상태 변화 없음
→ UI 유지
```

---

## 10. 결제 검증

매수 버튼 클릭 시 가장 먼저 최종 결제 조건을 검증한다.

검증 항목:

```text
- 선택 카드가 여전히 유효한가
- 매수 출처가 유효한가
- 카드가 아직 해당 위치에 존재하는가
- 모든 전문 자원 슬롯이 채워졌는가
- 딜로 대체된 슬롯도 결제 완료로 볼 수 있는가
- 최종 현금 비용을 낼 수 있는가
- 현재 매수 가능한 상태인가
```

검증 실패 시 즉시 중단한다.

```text
검증 실패
→ 자원 소비 없음
→ 현금 차감 없음
→ 자산 획득 없음
→ 시장 보충 없음
→ 예약 구역 변화 없음
→ 카드 상세보기 유지
```

---

## 11. 매수 확정 처리 순서

매수 확정 처리 순서는 다음과 같다.

```text
1. 최종 결제 조건 검증
2. 전문 자원 칩 소비
3. 딜 소비
4. 현금 차감
5. 자산을 포트폴리오에 추가
6. 매수 출처에 따른 카드 위치 처리
7. 매수 관련 규칙 발동
8. 결제 상태 초기화
9. 카드 상세보기 종료
10. 추가 매수권 처리 또는 영업일 종료
```

핵심 원칙:

```text
자원 소비와 자산 획득이 먼저.
UI 종료는 마지막.
```

검증이 실패하면 자원은 건드리지 않는다.

---

## 12. 시장 카드 매수

시장 카드 매수는 매수 출처가 `MarketTape`인 매수이다.

처리:

```text
시장 카드 매수
→ 결제 검증
→ 자원 소비
→ 현금 차감
→ 자산을 포트폴리오에 추가
→ 산 시장 슬롯을 새 카드로 보충
→ 시장 테이프 진행 없음
→ 매수 규칙 발동
→ 추가 매수권 없으면 영업일 종료
```

시장 카드 매수는 산 자리만 보충한다.

```text
현재 시장 [A] [B] [C]

B 매수
→ B는 보유 자산으로 이동
→ B 자리 새 카드 D 보충

현재 시장 [A] [D] [C]
```

시장 카드 매수는 시장 테이프를 진행시키지 않는다.

---

## 13. 예약 카드 매수

예약 카드 매수는 매수 출처가 `Reserved`인 매수이다.

처리:

```text
예약 카드 매수
→ 결제 검증
→ 자원 소비
→ 현금 차감
→ 예약 카드를 포트폴리오에 추가
→ 해당 예약 구역 비움
→ 시장 테이프 변화 없음
→ 매수 규칙 발동
→ 추가 매수권 없으면 영업일 종료
```

예약 카드 매수는 시장 테이프에 영향을 주지 않는다.

```text
예약 카드 매수
→ 시장 슬롯 보충 없음
→ 시장 테이프 진행 없음
```

예약 카드도 일반 자산 매수로 판정한다.

```text
예약 카드 매수
= OnBuyAsset 발동
= OnBuyAssetWithTag 발동 가능
```

단, 매수 출처는 `Reserved`로 남긴다.

---

## 14. 자산 획득 처리

매수 확정 후 카드는 보유 자산 상태가 된다.

처리:

```text
- 카드 상태를 Owned로 변경
- 보유 자산 목록에 추가
- 획득 순서 번호 부여
- 운용가치 합계에 포함
- 다음 영업일부터 운용 수익 발생 대상이 됨
```

매수 즉시 운용 수익은 발생하지 않는다.

```text
매수한 영업일
→ 운용 수익 없음

다음 영업일 시작
→ 운용 수익 발생
```

---

## 15. 현금 차감

현금은 매수 확정 시 최종 현금 비용만큼 차감한다.

```text
CurrentCash -= FinalCashCost
```

최종 현금 비용은 다음을 반영한다.

```text
- 기본 현금 비용
- 비용 수정 효과
- 딜 할인
- 인플레이션
```

현금이 부족하면 매수 확정은 실패한다.

---

## 16. 전문 자원 소비

전문 자원 칩은 매수 확정 시 소비한다.

```text
리서치 칩이 리서치 슬롯에 배치됨
→ 매수 확정 시 리서치 -1
```

카드 상세보기에서 슬롯에 올려둔 상태는 아직 소비가 아니다.

```text
칩 배치
→ 소비 아님

매수 확정
→ 소비
```

닫기나 취소 시 올려둔 칩은 회수된다.

```text
닫기 / 취소
→ 칩 회수
→ 자원 소비 없음
```

---

## 17. 딜 소비

딜도 매수 확정 시 소비한다.

```text
딜이 비용 슬롯에 배치됨
→ 매수 확정 시 딜 -1
```

딜은 전문 자원 슬롯 대체와 현금 할인 효과를 동시에 가진다.

```text
딜 1개
→ 슬롯 1칸 결제
→ 기본 현금 비용 -1
```

딜도 확정 전에는 소비되지 않는다.

```text
딜 배치
→ 소비 아님

매수 확정
→ 소비
```

---

## 18. 카드 상세보기 종료

매수 확정 처리가 끝나면 카드 상세보기 상태를 종료한다.

```text
매수 확정 완료
→ 결제 상태 초기화
→ 선택 카드 해제
→ 카드 상세보기 종료
→ 시장 상태 복귀
```

단, 이후 바로 영업일 종료 또는 추가 매수 상태로 이어질 수 있다.

추가 매수권이 없는 경우:

```text
매수 완료
→ 시장 상태 복귀
→ 영업일 종료
```

추가 매수권이 있는 경우:

```text
매수 완료
→ 시장 상태 복귀
→ 추가 매수 선택 상태
```

---

## 19. 매수 관련 규칙 발동

자산 매수 후 매수 관련 규칙을 발동한다.

대표 조건:

```text
- OnBuyAsset
- OnBuyAssetWithTag
- OnBuyFromSource
```

예약 카드 매수도 일반 자산 매수로 판정한다.

```text
예약 카드 매수
→ OnBuyAsset 발동
```

태그가 있는 자산을 매수하면 태그 기반 매수 규칙도 발동할 수 있다.

```text
성장 태그 자산 매수
→ OnBuyAssetWithTag: 성장
```

매수 출처는 별도로 기록해 출처 기반 규칙에 사용할 수 있다.

```text
PurchaseSource.MarketTape
PurchaseSource.Reserved
```

---

## 20. 추가 매수권과 매수

매수 관련 규칙에 의해 추가 매수권이 발생할 수 있다.

추가 매수권은 자산 매수 전용이다.

```text
추가 매수권
→ 시장 카드 매수 가능
→ 예약 카드 매수 가능
→ 자원 확보 불가
→ 예약 불가
```

추가 매수권은 중첩되지 않는다.

```text
추가 매수권 여러 번 발생
→ 최대 1개만 보유
```

추가 매수로 산 두 번째 자산도 매수 관련 규칙은 발동한다.

단, `GrantExtraBuyAction` 효과만 무시한다.

```text
추가 매수로 자산 매수
→ OnBuyAsset 발동
→ OnBuyAssetWithTag 발동
→ GrantExtraBuyAction만 무시
→ 영업일 종료
```

---

## 21. 첫 번째 매수 후 시장 보충과 추가 매수

시장 카드 매수 후에는 산 자리를 즉시 보충한다.

이후 추가 매수권이 발생하면, 보충된 새 카드도 추가 매수 대상으로 선택할 수 있다.

```text
현재 시장:
[A] [B] [C]

A 매수
→ A 자리 새 카드 D 보충

추가 매수권 발생
→ D, B, C 모두 추가 매수 후보
```

예약 카드 매수로 추가 매수권이 발생한 경우 시장 테이프는 변하지 않는다.

```text
예약 카드 매수
→ 예약 구역 비움
→ 시장 변화 없음
→ 추가 매수권 발생 가능
```

---

## 22. 매수 취소

매수 확정 전에는 닫기 또는 취소를 통해 매수 대기 상태를 해제할 수 있다.

취소 처리:

```text
- 올려둔 칩 전부 회수
- 이동 중인 칩 즉시 트레이 복귀
- 매수 큐 제거
- 선택 카드 해제
- 결제 상태 해제
- 카드 상세보기 종료
- 시장 상태 복귀
- 영업일 소비 없음
```

카드 상세보기의 닫기 버튼도 매수 취소와 동일한 정리 처리를 수행한다.

---

## 23. 매수 확정 중 오류 처리

매수 확정 처리 중에는 가능한 한 검증을 먼저 끝낸다.

```text
검증 완료 전
→ 자원 소비 금지
→ 카드 상태 변경 금지
```

검증 완료 후 처리 도중 오류가 발생하지 않도록, 다음 순서로 트랜잭션처럼 처리하는 것이 좋다.

```text
1. 검증
2. 계산값 확정
3. 자원 소비
4. 카드 상태 변경
5. 시장/예약 구역 변경
6. 규칙 발동
7. UI 종료
```

구현상 중간 실패를 줄이기 위해 `PurchaseContext`를 먼저 구성한다.

---

## 24. PurchaseContext

매수 처리에는 구매 컨텍스트를 사용한다.

```csharp
public class PurchaseContext
{
    public AssetCardRuntimeData PurchasedAsset;

    public PurchaseSource PurchaseSource;

    public MarketTapeZone? SourceMarketTapeZone;
    public int? SourceSlotIndex;

    public int? SourceReservedSlotIndex;

    public bool IsExtraBuyAction;

    public int UsedDealCount;
    public int FinalCashCost;
}
```

용도:

```text
- 규칙 발동
- 매수 출처 판정
- 시장 슬롯 보충
- 예약 구역 비움
- 추가 매수 판정
```

---

## 25. PurchasePaymentState

카드 상세보기에서 결제 대기 상태를 관리한다.

```csharp
public class PurchasePaymentState
{
    public AssetCardRuntimeData SelectedAsset;

    public List<PaymentSlotState> PaymentSlots;

    public int BaseCashCost;
    public int DealDiscount;
    public int FinalCashCost;

    public bool IsValid;
}
```

---

## 26. PaymentSlotState

전문 자원 비용 슬롯 상태를 관리한다.

```csharp
public class PaymentSlotState
{
    public ResourceType RequiredResourceType;

    public bool IsFilled;

    public ResourceType? PlacedResourceType;

    public bool IsFilledByDeal;
}
```

슬롯에 배치 가능한 자원:

```text
RequiredResourceType와 같은 전문 자원
또는
딜
```

---

## 27. 매수 가능 판정 예시

```csharp
bool CanConfirmPurchase(PurchasePaymentState state)
{
    if (state == null)
        return false;

    if (state.SelectedAsset == null)
        return false;

    if (!IsSelectedAssetStillValid(state.SelectedAsset))
        return false;

    if (!AreAllPaymentSlotsFilled(state))
        return false;

    if (CurrentCash < state.FinalCashCost)
        return false;

    if (!CanBuyAssetInCurrentActionState())
        return false;

    return true;
}
```

---

## 28. 매수 확정 예시

```csharp
void ConfirmPurchase()
{
    if (!CanConfirmPurchase(CurrentPaymentState))
    {
        ShowPurchaseBlockedFeedback();
        return;
    }

    PurchaseContext context =
        BuildPurchaseContext(CurrentPaymentState);

    ConsumePlacedProfessionalChips(CurrentPaymentState);
    ConsumePlacedDealChips(CurrentPaymentState);

    CurrentCash -= context.FinalCashCost;

    AddAssetToPortfolio(context.PurchasedAsset);

    if (context.PurchaseSource == PurchaseSource.MarketTape)
    {
        RemoveAssetFromMarketSlot(context);
        RefillPurchasedMarketSlot(context);
    }
    else if (context.PurchaseSource == PurchaseSource.Reserved)
    {
        RemoveAssetFromReservedSlot(context);
    }

    ResolvePurchaseRules(context);

    FinishPurchaseFlow(context);
}
```

---

## 29. FinishPurchaseFlow 예시

```csharp
void FinishPurchaseFlow(PurchaseContext context)
{
    ClearPaymentState();
    ClearCardDetailState();

    SetMarketAreaState(MarketAreaState.Market);

    if (context.IsExtraBuyAction)
    {
        ClearExtraBuyActionIfAny();
        EndBusinessDay();
        return;
    }

    if (ShouldEnterExtraBuyDecisionState(context))
    {
        EnterExtraBuyDecisionState();
        return;
    }

    EndBusinessDay();
}
```

---

## 30. 구현 시 주의사항

```text
- 카드 상세보기 진입은 영업일을 소비하지 않는다.
- 매수 확정 시 영업일을 소비한다.
- 매수 확정 전에는 자원을 소비하지 않는다.
- 검증 실패 시 자원, 카드, 시장 상태를 변경하지 않는다.
- 시장 카드 매수는 산 자리만 보충한다.
- 예약 카드 매수는 예약 구역만 비운다.
- 예약 카드 매수도 일반 자산 매수로 판정한다.
- 매수 출처는 PurchaseSource로 남긴다.
- 매수 후 보유 자산의 영업일 시작 현금은 다음 영업일부터 발생한다.
- 딜 할인은 인플레이션 적용 전에 반영한다.
- 추가 매수로도 매수 규칙은 발동하되, GrantExtraBuyAction만 무시한다.
- 추가 매수 후에는 즉시 영업일을 종료한다.
```
