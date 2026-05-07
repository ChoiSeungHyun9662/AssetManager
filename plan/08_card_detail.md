# 08. 카드 상세보기

## 1. 목적

이 문서는 카드 상세보기 화면의 역할, 표시 정보, 진입 조건, 닫기 처리, 매수/예약 입력 규칙을 정의한다.

카드 상세보기는 시장 카드 또는 예약 카드를 클릭했을 때 진입하는 화면이다.

카드 상세보기는 별도 팝업이 아니라, 기존 시장 테이프를 표시하던 `시장 영역`을 대체하는 상태이다.

```text
시장 영역 상태:
- Market
- CardDetail
- GainLiquidity
```

카드 상세보기 상태는 `CardDetail`에 해당한다.

---

## 2. 카드 상세보기의 역할

카드 상세보기는 플레이어가 특정 자산 카드의 정보를 검토하고, 해당 카드에 대해 매수 또는 예약을 결정하는 화면이다.

카드 상세보기에서 가능한 주요 행동은 다음과 같다.

```text
- 카드 정보 확인
- 카드 상세보기 닫기
- 자산 매수 준비
- 자산 매수 확정
- 시장 카드 예약
```

카드 상세보기 진입 자체는 영업일을 소비하지 않는다.

```text
카드 클릭
→ 카드 상세보기 진입
→ 영업일 소비 없음
```

영업일을 소비하는 시점은 다음 중 하나이다.

```text
- 매수 확정
- 예약 버튼 클릭
```

---

## 3. 진입 조건

카드 상세보기는 다음 입력으로 진입한다.

```text
시장 카드 클릭
→ 카드 상세보기

예약 카드 클릭
→ 카드 상세보기
```

카드 상세보기 진입 가능 상태는 시장 영역이 `Market` 상태일 때이다.

```text
Market 상태
→ 시장 카드 클릭 가능
→ 예약 카드 클릭 가능
```

다른 시장 영역 상태에서는 카드 상세보기 진입을 허용하지 않는다.

```text
CardDetail 상태
→ 다른 카드 클릭 불가

GainLiquidity 상태
→ 카드 클릭 불가
```

---

## 4. 카드 상세보기 표시 정보

카드 상세보기에는 다음 정보를 표시한다.

```text
- 카드 이미지
- 카드 이름
- 카드 설명
- 운용가치
- 인컴
- 현금 비용
- 전문 자원 비용
- 태그
- 희귀도
- 닫기 버튼
- 매수 버튼
- 자동 배치 버튼
- 비용 슬롯
- 예약 버튼
```

단, 카드의 출처에 따라 표시 정보가 달라진다.

```text
시장 카드 상세보기
→ 예약 버튼 표시

예약 카드 상세보기
→ 예약 버튼 숨김
```

---

## 5. 카드 출처

카드 상세보기에서 선택된 카드는 구매 출처를 가진다.

```csharp
public enum PurchaseSource
{
    MarketTape,
    Reserved
}
```

구매 출처에 따라 처리 규칙이 다르다.

| 구매 출처 | 매수 가능 | 예약 가능 | 매수 후 시장 처리 |
|---|---:|---:|---|
| MarketTape | O | O | 산 자리 보충 |
| Reserved | O | X | 예약 슬롯 비움 |

예약 카드 매수도 일반 자산 매수로 판정한다.

```text
예약 카드 매수
= 자산 매수
= OnBuyAsset 발동 가능
```

단, 구매 출처는 `Reserved`로 기록한다.

---

## 6. 카드 상세보기 상태 진입 처리

시장 카드 또는 예약 카드를 클릭하면 다음 처리를 수행한다.

```text
1. 선택 카드 저장
2. 구매 출처 기록
3. 카드 상세 정보 UI 구성
4. 매수 결제 상태 초기화
5. 비용 슬롯 구성
6. 매수 버튼 상태 갱신
7. 예약 버튼 표시 여부 갱신
8. 시장 영역을 CardDetail 상태로 전환
```

구현 예시:

```csharp
void OpenCardDetail(AssetCardRuntimeData card)
{
    SelectedCard = card;
    CurrentPurchaseSource = ResolvePurchaseSource(card);

    ClearPaymentState();
    BuildCardDetailView(card);
    BuildPurchasePaymentState(card);

    UpdateBuyButton();
    UpdateReserveButton();

    SetMarketAreaState(MarketAreaState.CardDetail);
}
```

---

## 7. 카드 상세보기 닫기

카드 상세보기 상태에서 닫기 버튼을 누르면 항상 시장 상태로 복귀한다.

```text
카드 상세보기
→ 닫기
→ 시장 상태
```

닫기는 영업일을 소비하지 않는다.

```text
카드 보기 = 무료
카드 닫기 = 무료
```

닫기 처리 시 선택 카드와 결제 대기 상태를 정리한다.

```text
닫기
→ 선택 카드 해제
→ 결제 상태 초기화
→ 비용 슬롯 초기화
→ 시장 상태 복귀
```

---

## 8. 칩이 올라간 상태에서 닫기

카드 상세보기에서 비용 슬롯에 칩이 올라간 상태로 닫기를 누를 수 있다.

이 경우 매수 취소와 동일하게 처리한다.

```text
닫기
→ 올려둔 칩 전부 회수
→ 이동 중인 칩 즉시 회수
→ 매수 큐 제거
→ 결제 상태 초기화
→ 선택 카드 해제
→ 시장 상태 복귀
→ 영업일 소비 없음
```

자원은 소비되지 않는다.

```text
매수 확정 전 닫기
→ 자원 소비 없음
```

구현 예시:

```csharp
void CloseCardDetail()
{
    if (HasAnyPlacedChip(CurrentPaymentState))
    {
        ReturnAllPlacedChipsToTray();
    }

    if (IsAutoPlaceAnimating)
    {
        StopAutoPlaceAnimation();
        SnapAllMovingChipsToTray();
    }

    HasQueuedBuyInput = false;

    ClearPaymentState();
    SelectedCard = null;

    SetMarketAreaState(MarketAreaState.Market);
}
```

---

## 9. 매수 UI

카드 상세보기에는 매수 관련 UI를 표시한다.

표시 요소:

```text
- 현금 비용
- 전문 자원 비용 슬롯
- 딜 칩 배치 가능 슬롯
- 자동 배치 버튼
- 매수 버튼
- 취소/닫기 입력
```

전문 자원 비용이 없는 순수 현금 자산의 경우 비용 슬롯은 표시하지 않는다.

```text
전문 자원 비용 없음
→ 비용 슬롯 없음
→ 매수 버튼만 표시
```

전문 자원 비용이 있는 경우 비용 슬롯을 표시한다.

```text
리서치 비용 2
신용 비용 1
→ 리서치 슬롯 2개
→ 신용 슬롯 1개
```

---

## 10. 비용 슬롯

전문 자원 비용은 비용 슬롯으로 표시한다.

각 슬롯은 다음 상태를 가진다.

```text
- 비어 있음
- 해당 전문 자원 칩 배치됨
- 딜 칩 배치됨
```

슬롯에 해당 전문 자원 칩 또는 딜 칩이 배치되면 결제 완료 상태로 본다.

```text
슬롯이 채워짐
→ 해당 전문 자원 비용 결제 완료
```

딜 칩은 모든 전문 자원 슬롯에 배치 가능하다.

```text
딜 칩
→ 리서치 슬롯 가능
→ 신용 슬롯 가능
→ 원자재 슬롯 가능
```

---

## 11. 매수 버튼

매수 버튼은 현재 결제 상태를 기준으로 활성/비활성화된다.

매수 가능 조건:

```text
- 선택 카드가 유효함
- 모든 전문 자원 비용 슬롯이 채워짐
- 최종 현금 비용을 지불할 수 있음
- 현재 매수 가능한 행동 상태임
```

매수 불가 시 버튼은 비활성화하거나 불가 피드백을 제공한다.

```text
전문 자원 슬롯 미충족
→ 매수 불가

현금 부족
→ 매수 불가
```

매수 버튼의 정확한 위치와 크기는 Unity 구현 단계에서 정한다.

---

## 12. 매수 확정 처리

매수 버튼을 클릭하면 매수 확정을 시도한다.

처리 순서:

```text
1. 최종 결제 조건 검증
2. 전문 자원 칩 소비
3. 딜 칩 소비
4. 현금 차감
5. 자산을 포트폴리오에 추가
6. 구매 출처에 따른 카드 제거 처리
7. 매수 관련 규칙 발동
8. 결제 상태 초기화
9. 카드 상세보기 종료
10. 영업일 종료 또는 추가 매수 상태 진입
```

시장 카드 매수:

```text
시장 카드 매수
→ 산 자리만 새 카드로 보충
→ 시장 테이프 진행 없음
```

예약 카드 매수:

```text
예약 카드 매수
→ 예약 슬롯만 비움
→ 시장 테이프 변화 없음
```

---

## 13. 예약 버튼 표시 조건

예약 버튼은 시장 카드 상세보기에서만 표시한다.

```text
시장 카드 상세보기
→ 예약 버튼 표시

예약 카드 상세보기
→ 예약 버튼 숨김
```

이미 예약된 카드는 다시 예약할 수 없다.

```text
예약 카드
→ 예약 버튼 없음
```

---

## 14. 예약 버튼 활성 조건

예약 버튼이 표시되더라도, 예약 슬롯이 가득 차 있으면 비활성화한다.

```text
예약 슬롯 여유 있음
→ 예약 버튼 활성화

예약 슬롯 3 / 3
→ 예약 버튼 비활성화
→ 사유 표시
```

사유 문구:

```text
예약 슬롯이 가득 찼습니다.
```

또는 축약 문구:

```text
예약 슬롯 가득 참
```

예약 슬롯이 가득 찬 상태에서는 예약 버튼을 눌러도 영업일을 소비하지 않는다.

```text
예약 불가
→ 영업일 소비 없음
```

---

## 15. 예약 버튼 클릭 처리

예약 버튼 클릭은 즉시 예약 액션이다.

예약 확인 상태는 존재하지 않는다.

```text
예약 버튼 클릭
→ 예약 액션 실행
→ 영업일 소비
```

예약 액션 처리 순서:

```text
1. 비용 슬롯에 올려둔 칩이 있다면 자동 회수
2. 매수 결제 상태 초기화
3. 선택한 시장 카드를 예약 슬롯으로 이동
4. 예약으로 비워진 시장 슬롯에 새 카드 1장 보충
5. 시장 테이프 진행
6. 딜 +1
7. 환매 압력 +1
8. 카드 상세보기 종료
9. 시장 상태 복귀
10. 영업일 종료
```

---

## 16. 칩이 올라간 상태에서 예약 버튼 클릭

카드 상세보기에는 매수 UI와 예약 버튼이 함께 존재한다.

플레이어가 비용 슬롯에 칩을 일부 올려둔 상태에서 예약 버튼을 클릭할 수 있다.

이 경우 예약 버튼은 비활성화하지 않는다.  
예약 버튼 클릭 시 매수 대기 상태를 자동 정리한 뒤 예약 액션을 실행한다.

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
→ 배치 칩 회수
→ 자원 소비 없음
```

---

## 17. 예약 슬롯 가득 참과 칩 배치

칩이 올라간 상태라도 예약 슬롯이 가득 차 있으면 예약 버튼은 비활성화한다.

```text
칩 배치됨
예약 슬롯 3 / 3
→ 예약 버튼 비활성화
→ 칩은 그대로 유지
→ 매수 결제 상태 유지
```

예약이 실제로 실행될 수 있을 때만 칩 자동 회수가 일어난다.

```text
예약 버튼 활성화 상태에서 클릭
→ 칩 자동 회수
→ 예약 실행
```

---

## 18. 딜 한도와 예약 버튼

딜 칩이 이미 최대치여도 예약 버튼은 비활성화하지 않는다.

```text
딜 3 / 3
→ 예약 가능
```

예약으로 얻는 추가 딜만 폐기한다.

```text
딜 3 / 3
→ 예약 실행
→ 딜 +1 시도
→ 추가 딜 폐기
→ "딜 칩 최대 보유: 추가 딜 폐기"
```

예약 버튼 비활성화 조건에는 딜 보유량을 포함하지 않는다.

---

## 19. 카드 상세보기와 다음 영업일

카드 상세보기 상태에서는 다음 영업일 버튼을 비활성화한다.

```text
CardDetail 상태
→ 다음 영업일 비활성화
```

다음 영업일로 넘기고 싶다면 먼저 카드 상세보기를 닫아야 한다.

```text
카드 상세보기
→ 닫기
→ 시장 상태
→ 다음 영업일 가능
```

이 규칙은 실수로 영업일을 소비하는 상황을 방지한다.

---

## 20. 카드 상세보기와 추가 매수권

추가 매수권 보유 중에도 카드 상세보기로 진입할 수 있다.

추가 매수권 상태에서 가능한 행동:

```text
- 시장 카드 매수
- 예약 카드 매수
- 다음 영업일
```

불가능한 행동:

```text
- 예약
- 유동성 확보
```

따라서 추가 매수권 상태에서 시장 카드 상세보기에 들어가더라도 예약 버튼은 표시하지 않거나 비활성화한다.

권장 처리:

```text
추가 매수권 상태
→ 시장 카드 상세보기
→ 예약 버튼 숨김
```

이유:

```text
추가 매수권은 자산 매수 전용 권리이다.
예약은 자산 매수가 아니다.
```

추가 매수권 상태에서 매수 확정하면 두 번째 매수로 처리하고, 즉시 영업일을 종료한다.

---

## 21. 카드 상세보기 상태 데이터

카드 상세보기에는 다음 상태 데이터가 필요하다.

```csharp
public class CardDetailState
{
    public AssetCardRuntimeData SelectedCard;
    public PurchaseSource PurchaseSource;

    public PurchasePaymentState PaymentState;

    public bool IsOpenedDuringExtraBuy;
}
```

---

## 22. 예약 버튼 표시 함수 예시

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

## 23. 예약 버튼 활성 함수 예시

```csharp
bool CanReserveSelectedCard()
{
    if (!ShouldShowReserveButton())
        return false;

    if (IsReservedSlotFull())
        return false;

    return true;
}
```

---

## 24. 예약 버튼 갱신 예시

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
        ReserveButton.SetText("예약 슬롯 가득 참");
        ReserveButton.SetTooltip("예약 슬롯이 가득 찼습니다.");
        return;
    }

    ReserveButton.SetInteractable(true);
    ReserveButton.SetText("예약");
    ReserveButton.SetTooltip("");
}
```

---

## 25. 카드 상세보기 닫기 함수 예시

```csharp
void OnCardDetailCloseClicked()
{
    CloseCardDetail();
}
```

```csharp
void CloseCardDetail()
{
    if (IsAutoPlaceAnimating)
    {
        StopAutoPlaceAnimation();
        SnapAllMovingChipsToTray();
    }

    if (HasAnyPlacedChip(CurrentPaymentState))
    {
        ReturnAllPlacedChipsToTray();
    }

    HasQueuedBuyInput = false;

    ClearPaymentState();
    SelectedCard = null;

    SetMarketAreaState(MarketAreaState.Market);
}
```

---

## 26. 예약 실행 함수 예시

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

    MoveSelectedCardToReservedSlot();

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

## 27. 매수 확정 함수 예시

```csharp
void OnBuyButtonClicked()
{
    if (!CanConfirmPurchase(CurrentPaymentState))
    {
        ShowPurchaseBlockedFeedback();
        return;
    }

    ConfirmPurchase();
}
```

```csharp
void ConfirmPurchase()
{
    PurchaseContext context = BuildPurchaseContext(SelectedCard);

    ConsumePlacedProfessionalChips();
    ConsumePlacedDealChips();

    CurrentCash -= CurrentPaymentState.FinalCashCost;

    AddAssetToPortfolio(SelectedCard);

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

## 28. 구현 시 주의사항

```text
- 카드 상세보기는 시장 영역을 대체하는 상태이다.
- 카드 클릭은 영업일을 소비하지 않는다.
- 닫기는 항상 시장 상태로 복귀하며 영업일을 소비하지 않는다.
- 닫기 시 올려둔 칩은 모두 회수한다.
- 예약 버튼은 시장 카드 상세보기에서만 표시한다.
- 예약 카드 상세보기에서는 예약 버튼을 숨긴다.
- 예약 확인 상태는 없다.
- 예약 버튼 클릭은 즉시 예약 액션이다.
- 예약 슬롯이 가득 찬 경우 예약 버튼은 비활성화한다.
- 딜이 3개여도 예약 버튼은 활성화할 수 있다.
- 딜 초과분은 폐기하고 메시지를 표시한다.
- 비용 슬롯에 칩이 올라간 상태에서 예약 버튼을 누르면 칩을 자동 회수한 뒤 예약한다.
- 추가 매수권 상태에서는 예약 버튼을 숨기거나 비활성화한다.
- 카드 상세보기 상태에서는 다음 영업일 버튼을 비활성화한다.
```
