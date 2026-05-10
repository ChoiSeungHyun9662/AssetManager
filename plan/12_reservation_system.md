# 12. 예약 시스템

## 1. 목적

이 문서는 카드 예약 시스템의 규칙, UI 조건, 처리 순서, 시장 테이프와의 관계, 딜/환매 압력 처리 규칙을 정의한다.

예약은 시장 카드 1장을 시장에서 빼내 예약 구역에 장기 보관하는 행동이다.
예약은 좋은 카드를 나중에 매수하기 위해 보관하는 수단이며, 동시에 딜을 얻는 주요 수단이다.

예약은 강력한 행동이므로 다음 대가를 가진다.

```text
- 영업일 1일 소비
- 환매 압력 +1
- 시장 테이프 진행
```

---

## 2. 예약의 핵심 성격

예약은 다음 효과를 가진다.

```text
시장 카드 1장을 예약 구역으로 이동
→ 딜 +1
→ 환매 압력 +1
→ 예약한 자리 새 카드 보충
→ 시장 테이프 진행
→ 영업일 종료
```

예약은 자산 매수가 아니다.

```text
예약
≠ 자산 매수
```

예약한 카드는 보유 자산이 아니므로, 운용가치에 포함되지 않는다.

```text
예약 카드
→ 운용가치 합산 대상 아님
→ 운용 수익 발생 대상 아님
→ 분기 마감 정산 대상 아님
```

예약 카드는 실제로 매수해야 보유 자산이 된다.

---

## 3. 예약 가능 대상

예약은 시장 카드에만 가능하다.

```text
시장 카드
→ 예약 가능

예약 카드
→ 예약 불가
```

카드 상세보기에서 예약 버튼은 시장 카드 상세보기에서만 표시한다.

```text
시장 카드 상세보기
→ 예약 버튼 표시

예약 카드 상세보기
→ 예약 버튼 숨김
```

이미 예약된 카드를 다시 예약할 수 없다.

---

## 4. 예약 구역

예약 구역은 최대 3개이다.

```text
예약 구역 수: 3개
예약 카드 최대 보유 수: 3장
```

예약 구역은 분기와 회계년도 전환 후에도 유지된다.

```text
분기 전환
→ 예약 카드 유지

회계년도 전환
→ 예약 카드 유지
```

예약 구역이 가득 찬 경우 더 이상 새 카드를 예약할 수 없다.

```text
예약 구역 3 / 3
→ 예약 버튼 비활성화
```

---

## 5. 예약 버튼 표시 조건

예약 버튼은 다음 조건을 만족할 때 표시한다.

```text
- 카드 상세보기 상태이다.
- 선택 카드의 출처가 시장 테이프이다.
- 현재 추가 매수권 상태가 아니다.
```

예약 카드 상세보기에서는 예약 버튼을 표시하지 않는다.

```text
예약 카드 상세보기
→ 예약 버튼 숨김
```

추가 매수권 상태에서도 예약 버튼은 숨기거나 비활성화한다.

```text
추가 매수권 상태
→ 예약 불가
```

추가 매수권은 자산 매수 전용 권리이기 때문이다.

---

## 6. 예약 버튼 활성 조건

예약 버튼이 표시되더라도, 예약 구역이 가득 차 있으면 비활성화한다.

```text
예약 구역 여유 있음
→ 예약 버튼 활성화

예약 구역 3 / 3
→ 예약 버튼 비활성화
```

비활성 사유 문구:

```text
예약 구역이 가득 찼습니다.
```

또는 축약 문구:

```text
예약 구역 가득 참
```

예약 구역이 가득 찬 상태에서는 예약 버튼을 눌러도 예약 액션이 실행되지 않는다.
영업일도 소비하지 않는다.

---

## 7. 딜 보유량과 예약 가능 여부

딜이 이미 최대치여도 예약은 가능하다.

```text
딜 3 / 3
→ 예약 가능
```

딜 최대 보유량은 예약 가능 여부를 막지 않는다.

```text
예약 버튼 비활성화 조건 아님:
- 딜 3 / 3
```

딜이 최대치인 상태에서 예약하면 추가 딜만 폐기한다.

```text
딜 3 / 3
→ 예약 실행
→ 딜 +1 시도
→ 추가 딜 폐기
→ 메시지 표시
```

메시지:

```text
딜 최대 보유: 추가 딜 폐기
```

---

## 8. 예약 액션 실행 타이밍

예약 버튼 클릭은 즉시 예약 액션이다.

예약 확인 상태는 존재하지 않는다.

```text
카드 상세보기
→ 예약 버튼 클릭
→ 예약 액션 즉시 실행
```

예약 버튼 클릭 시 영업일을 소비한다.

```text
예약 버튼 클릭
→ 영업일 1일 소비
```

단, 예약 버튼이 비활성화된 상태에서는 실행되지 않으며 영업일을 소비하지 않는다.

---

## 9. 비용 슬롯에 칩이 올라간 상태에서 예약

카드 상세보기에는 매수 UI와 예약 버튼이 함께 존재한다.

플레이어가 비용 슬롯에 칩을 일부 올려둔 상태에서 예약 버튼을 누를 수 있다.

이 경우 예약 버튼은 비활성화하지 않는다.
예약 버튼을 클릭하면 올려둔 칩을 자동 회수한 뒤 예약을 실행한다.

```text
비용 슬롯에 칩 배치
→ 예약 버튼 클릭
→ 올려둔 칩 전부 자동 회수
→ 매수 결제 상태 초기화
→ 예약 액션 실행
```

자원은 소비되지 않는다.

```text
예약 버튼 클릭
→ 매수 확정 아님
→ 자원 소비 없음
```

예약 구역이 가득 찬 경우에는 예약 버튼이 비활성화되므로, 칩 자동 회수도 발생하지 않는다.

```text
예약 구역 3 / 3
→ 예약 버튼 비활성화
→ 칩은 그대로 유지
→ 매수 결제 상태 유지
```

---

## 10. 예약 액션 처리 순서

예약 액션의 처리 순서는 다음과 같다.

```text
1. 예약 가능 여부 검증
2. 비용 슬롯에 올려둔 칩이 있다면 전부 자동 회수
3. 매수 결제 상태 초기화
4. 선택한 시장 카드를 예약 구역으로 이동
5. 예약으로 비워진 시장 슬롯에 새 카드 1장 보충
6. 시장 테이프 진행
7. 딜 +1 처리
8. 환매 압력 +1 처리
9. 환매 압력 한도 검사
10. 실패하지 않았다면 카드 상세보기 종료
11. 시장 상태 복귀
12. 영업일 종료
```

중요한 순서:

```text
예약한 자리 보충
→ 시장 테이프 진행
```

예약으로 빈 자리를 먼저 새 카드로 채운 뒤, 시장 테이프가 한 단계 진행된다.

---

## 11. 예약 후 시장 테이프 처리

예약 액션 후에는 시장 테이프가 진행된다.

단, 예약한 자리를 먼저 새 카드로 보충한다.

```text
시장 카드 예약
→ 예약한 자리 새 카드 보충
→ 시장 테이프 진행
```

시장 테이프 진행은 다음 처리이다.

```text
1. 매도 임박 카드 제거
2. 현재 시장 카드 → 매도 임박으로 이동
3. 예비 시장 카드 → 현재 시장으로 이동
4. 예비 시장 빈 슬롯 전부 새 카드로 보충
```

---

## 12. 예약 처리 예시

예약 전 시장 상태:

```text
매도 임박: [A] [B] [C]
현재 시장: [D] [E] [F]
예비 시장: [G] [H] [I]
```

플레이어가 현재 시장의 `E`를 예약한다.

1단계: E가 예약 구역으로 이동한다.

```text
예약 구역: [E]
```

2단계: E가 빠진 자리에 새 카드 J를 보충한다.

```text
매도 임박: [A] [B] [C]
현재 시장: [D] [J] [F]
예비 시장: [G] [H] [I]
```

3단계: 시장 테이프가 진행된다.

```text
매도 임박 [A] [B] [C] 제거
현재 시장 [D] [J] [F] → 매도 임박
예비 시장 [G] [H] [I] → 현재 시장
예비 시장 빈 슬롯 → 새 카드 보충
```

최종 상태:

```text
매도 임박: [D] [J] [F]
현재 시장: [G] [H] [I]
예비 시장: [K] [L] [M]
예약 구역: [E]
```

---

## 13. 예약 카드 유지

예약 카드는 분기와 회계년도가 바뀌어도 유지된다.

```text
예약 카드
→ 영업일 종료 후 유지
→ 분기 마감 후 유지
→ 다음 분기 시작 후 유지
→ 4Q 휴가 후 유지
→ 다음 회계년도 시작 후 유지
```

시장 테이프 진행과 갱신은 예약 구역에 영향을 주지 않는다.

```text
시장 테이프 진행
→ 예약 구역 유지

시장 테이프 갱신
→ 예약 구역 유지
```

예약 카드는 플레이어가 매수할 때까지 남아 있다.

---

## 14. 예약 카드 매수

예약 카드는 카드 상세보기에서 매수할 수 있다.

```text
예약 카드 클릭
→ 카드 상세보기
→ 매수 결제
→ 매수 확정
```

예약 카드 매수는 일반 자산 매수로 판정한다.

```text
예약 카드 매수
= 자산 매수
= OnBuyAsset 발동
```

단, 매수 출처는 `Reserved`로 기록한다.

```text
PurchaseSource.Reserved
```

예약 카드 매수 후에는 해당 예약 구역만 비운다.

```text
예약 카드 매수
→ 예약 구역 비움
→ 시장 테이프 변화 없음
```

예약 카드 매수는 시장 테이프를 진행시키지 않는다.

---

## 15. 예약과 딜

예약은 딜을 획득하는 주요 수단이다.

```text
시장 카드 예약
→ 딜 +1
```

딜은 최대 3개까지 보유한다.

```text
딜 최대 보유량: 3개
```

딜이 3개인 상태에서 예약하면 추가 딜은 폐기된다.

```text
딜 3 / 3
→ 예약 실행
→ 딜 +1 폐기
```

메시지:

```text
딜 최대 보유: 추가 딜 폐기
```

---

## 16. 예약과 환매 압력

예약은 환매 압력을 증가시킨다.

```text
시장 카드 예약
→ 환매 압력 +1
```

환매 압력은 증가 즉시 한도 검사를 한다.

```text
환매 압력 증가
→ 환매 압력 10 이상인지 검사
→ 10 이상이면 런 실패
```

예약으로 환매 압력이 10 이상이 되면 즉시 런 실패가 발생한다.

```text
현재 환매 압력 9
→ 예약
→ 환매 압력 +1
→ 환매 압력 10
→ 대규모 환매 발생
→ 런 실패
```

이 경우 이후 영업일 종료 처리보다 런 실패가 우선한다.

---

## 17. 예약과 추가 매수권

추가 매수권 상태에서는 예약을 할 수 없다.

추가 매수권은 자산 매수 전용 권리이다.

```text
추가 매수권 보유 중 가능:
- 시장 카드 매수
- 예약 카드 매수
- 다음 영업일

추가 매수권 보유 중 불가:
- 시장 카드 예약
- 자원 확보
```

따라서 추가 매수권 상태에서 시장 카드 상세보기로 들어가면 예약 버튼은 숨기거나 비활성화한다.

권장 처리:

```text
추가 매수권 상태
→ 예약 버튼 숨김
```

---

## 18. 예약과 운용가치

예약 카드는 보유 자산이 아니므로 운용가치에 포함되지 않는다.

```text
예약 카드 운용가치
→ 최종 운용가치에 포함하지 않음
```

예약 카드가 운용가치에 포함되는 시점은 매수 확정 후이다.

```text
예약 카드 매수 확정
→ 보유 자산으로 이동
→ 운용가치 합산 대상
```

예약 카드도 운용 수익을 발생시키지 않는다.

```text
예약 카드
→ 운용 수익 없음
```

---

## 19. 예약과 분기 마감

예약 카드는 분기 마감 정산 대상이 아니다.

```text
예약 카드
→ 분기 마감 정산 대상 아님
```

분기 마감 정산은 보유 자산만 대상으로 한다.

```text
보유 자산
→ 정산 대상

예약 카드
→ 정산 대상 아님
```

예약 카드가 분기 마감 후 사라지지도 않는다.

```text
분기 마감
→ 예약 카드 유지
```

---

## 20. 예약 구역 UI

예약 구역 UI는 3개의 슬롯을 표시한다.

```text
예약 구역 1
예약 구역 2
예약 구역 3
```

빈 슬롯은 빈 상태로 표시한다.

```text
빈 예약 구역
→ 카드 없음
```

예약된 카드가 있는 슬롯은 카드 축약 UI를 표시한다.

```text
예약 구역에 카드 있음
→ 카드 클릭 가능
→ 카드 상세보기 진입
```

예약 구역이 모두 차 있으면 시장 카드 상세보기의 예약 버튼은 비활성화된다.

---

## 21. 예약 시스템 데이터 구조

### 21.1 ReservedAssetSlot

```csharp
public class ReservedAssetSlot
{
    public int SlotIndex;
    public AssetCardRuntimeData ReservedAsset;
}
```

---

### 21.2 ReservationState

```csharp
public class ReservationState
{
    public List<ReservedAssetSlot> ReservedSlots;
}
```

---

### 21.3 예약 구역 수 상수

```csharp
const int MaxReservedAssetCount = 3;
```

---

## 22. 예약 가능 판정

```csharp
bool CanReserveSelectedCard()
{
    if (SelectedCard == null)
        return false;

    if (CurrentPurchaseSource != PurchaseSource.MarketTape)
        return false;

    if (CurrentTurnActionState.IsWaitingForExtraBuyDecision)
        return false;

    if (IsReservedSlotFull())
        return false;

    return true;
}
```

---

## 23. 예약 버튼 표시 판정

```csharp
bool ShouldShowReserveButton()
{
    if (SelectedCard == null)
        return false;

    if (CurrentPurchaseSource != PurchaseSource.MarketTape)
        return false;

    if (CurrentTurnActionState.IsWaitingForExtraBuyDecision)
        return false;

    return true;
}
```

---

## 24. 예약 구역 가득 참 판정

```csharp
bool IsReservedSlotFull()
{
    return ReservedSlots.All(slot => slot.ReservedAsset != null);
}
```

빈 예약 구역 찾기:

```csharp
ReservedAssetSlot GetFirstEmptyReservedSlot()
{
    return ReservedSlots
        .OrderBy(slot => slot.SlotIndex)
        .FirstOrDefault(slot => slot.ReservedAsset == null);
}
```

---

## 25. 예약 실행 예시

```csharp
void OnReserveButtonClicked()
{
    if (!CanReserveSelectedCard())
    {
        ShowReserveBlockedFeedback();
        return;
    }

    if (HasAnyPlacedChip(CurrentPaymentState))
    {
        ReturnAllPlacedChipsToTray();
        ClearPaymentState();
    }

    ConfirmReserveSelectedCard();
}
```

```csharp
void ConfirmReserveSelectedCard()
{
    MarketTapeSlot sourceSlot = GetSelectedCardMarketSlot();
    ReservedAssetSlot targetReservedSlot = GetFirstEmptyReservedSlot();

    MoveSelectedCardToReservedSlot(targetReservedSlot);

    RefillMarketSlot(sourceSlot);

    AdvanceMarketTape();

    AddDealFromReservation();

    AddRedemptionPressure(1);

    if (IsRunFailedByRedemption())
        return;

    ClearCardDetailState();
    SetMarketAreaState(MarketAreaState.Market);

    EndBusinessDay();
}
```

---

## 26. 예약 카드 매수 후 슬롯 비우기

```csharp
void RemoveAssetFromReservedSlot(PurchaseContext context)
{
    ReservedAssetSlot slot =
        ReservedSlots.FirstOrDefault(s =>
            s.SlotIndex == context.SourceReservedSlotIndex
        );

    if (slot == null)
        return;

    slot.ReservedAsset = null;
}
```

---

## 27. 예약 버튼 상태 갱신 예시

```csharp
void UpdateReserveButton()
{
    if (!ShouldShowReserveButton())
    {
        ReserveButton.Hide();
        return;
    }

    ReserveButton.Show();

    if (IsReservedSlotFull())
    {
        ReserveButton.SetInteractable(false);
        ReserveButton.SetText("예약 구역 가득 참");
        ReserveButton.SetTooltip("예약 구역이 가득 찼습니다.");
        return;
    }

    ReserveButton.SetInteractable(true);
    ReserveButton.SetText("예약");
    ReserveButton.SetTooltip("");
}
```

---

## 28. 구현 시 주의사항

```text
- 예약은 시장 카드에만 가능하다.
- 예약 카드에는 예약 버튼을 표시하지 않는다.
- 예약 버튼 클릭은 즉시 예약 액션이다.
- 예약 확인 상태는 없다.
- 예약 구역은 최대 3개이다.
- 예약 구역이 가득 차면 예약 버튼을 비활성화한다.
- 딜이 3개여도 예약은 가능하다.
- 딜 한도 초과 시 추가 딜만 폐기한다.
- 예약은 환매 압력 +1을 발생시킨다.
- 환매 압력 증가 후 즉시 한도 검사를 한다.
- 예약으로 환매 압력이 10 이상이 되면 즉시 런 실패다.
- 예약한 자리는 먼저 새 카드로 보충한 뒤 시장 테이프를 진행한다.
- 예약 카드는 분기/회계년도 전환 후에도 유지한다.
- 예약 카드는 운용가치, 운용 수익, 분기 마감 정산에 포함하지 않는다.
- 예약 카드 매수는 일반 자산 매수로 판정하되 매수 출처는 Reserved로 기록한다.
```
