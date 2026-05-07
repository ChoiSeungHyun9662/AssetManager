# 13. 유동성 확보 시스템

## 1. 목적

이 문서는 유동성 확보 시스템의 진입 방식, 화면 상태, 자원 선택 규칙, 취소 조건, 완료 조건, 전문 자원 한도 처리 규칙을 정의한다.

유동성 확보는 플레이어가 영업일 하나를 사용해 기본 자원을 확보하는 행동이다.

유동성 확보는 액션 선택창에서 고르는 방식이 아니다.  
플레이어는 시장 영역의 `자원통`을 클릭해 유동성 확보 화면으로 진입한다.

```text
시장 상태
→ 자원통 클릭
→ 유동성 확보 화면
```

유동성 확보 화면은 별도 팝업이 아니라 시장 영역을 대체하는 상태이다.

---

## 2. 유동성 확보의 핵심 성격

유동성 확보는 자원을 얻기 위한 기본 행동이다.

획득 가능한 기본 자원:

```text
- 현금
- 리서치
- 신용
- 원자재
```

획득 불가능한 자원:

```text
- 딜
```

딜은 예약 액션으로만 얻는다.

---

## 3. 유동성 확보 화면 진입

시장 상태에서 자원통을 클릭하면 유동성 확보 화면으로 진입한다.

```text
Market 상태
→ 자원통 클릭
→ GainLiquidity 상태
```

자원통 클릭 자체는 영업일을 소비하지 않는다.

```text
자원통 클릭
→ 유동성 확보 화면 진입
→ 아직 영업일 소비 없음
```

유동성 확보에서 첫 자원을 클릭하는 순간, 유동성 확보 행동이 확정된다.

```text
첫 자원 클릭
→ 자원 즉시 획득
→ 유동성 확보 행동 확정
```

---

## 4. 유동성 확보 화면 표시 정보

유동성 확보 화면은 시장 영역을 대체한다.

표시 요소:

```text
- 유동성 확보 제목
- 현금 버튼
- 리서치 버튼
- 신용 버튼
- 원자재 버튼
- 선택된 자원 표시
- 닫기 버튼
- 안내 메시지 영역
```

표시 예시:

```text
유동성 확보

[현금]
[리서치]
[신용]
[원자재]

선택한 자원:
-
[닫기]
```

---

## 5. 유동성 확보 닫기

유동성 확보 화면은 첫 자원 획득 전까지만 닫을 수 있다.

```text
자원통 클릭
→ 유동성 확보 화면 진입
→ 아직 자원 획득 없음
→ 닫기 가능
```

닫기 클릭 시 시장 상태로 복귀한다.

```text
닫기
→ 시장 상태 복귀
→ 영업일 소비 없음
```

자원을 하나라도 획득하면 닫기 버튼은 비활성화된다.

```text
자원 1개 클릭
→ 즉시 획득
→ 유동성 확보 행동 확정
→ 닫기 비활성화
```

자원 획득 후에는 유동성 확보가 완료될 때까지 진행한다.

---

## 6. 자원 클릭 즉시 획득

유동성 확보 화면에서 자원 버튼을 클릭하면 해당 자원을 즉시 획득한다.

```text
현금 클릭
→ 현금 +1 즉시 획득

리서치 클릭
→ 리서치 +1 즉시 획득

신용 클릭
→ 신용 +1 즉시 획득

원자재 클릭
→ 원자재 +1 즉시 획득
```

첫 자원 클릭부터는 해당 영업일의 행동이 유동성 확보로 확정된다.

```text
첫 자원 클릭
→ 이번 영업일 행동 = 유동성 확보
→ 취소 불가
→ 다른 기본 행동 불가
```

---

## 7. 유동성 확보 기본 완료 조건

유동성 확보는 기본적으로 다음 두 조건 중 하나를 만족하면 완료된다.

```text
1. 같은 기본 자원 2개 획득
2. 서로 다른 기본 자원 3개 획득
```

기본 자원은 다음 4개이다.

```text
- 현금
- 리서치
- 신용
- 원자재
```

딜은 포함하지 않는다.

---

## 8. 같은 기본 자원 2개

같은 기본 자원을 2번 선택하면 유동성 확보가 완료된다.

예시:

```text
현금 클릭
→ 현금 +1

현금 클릭
→ 현금 +1

결과:
현금 +2
→ 유동성 확보 완료
→ 영업일 종료
```

다른 예시:

```text
원자재 클릭
→ 원자재 +1

원자재 클릭
→ 원자재 +1

결과:
원자재 +2
→ 유동성 확보 완료
→ 영업일 종료
```

---

## 9. 서로 다른 기본 자원 3개

서로 다른 기본 자원 3개를 선택하면 유동성 확보가 완료된다.

예시:

```text
리서치 클릭
→ 리서치 +1

신용 클릭
→ 신용 +1

원자재 클릭
→ 원자재 +1

결과:
리서치 +1
신용 +1
원자재 +1
→ 유동성 확보 완료
→ 영업일 종료
```

현금을 포함할 수도 있다.

```text
현금 클릭
→ 현금 +1

리서치 클릭
→ 리서치 +1

신용 클릭
→ 신용 +1

결과:
현금 +1
리서치 +1
신용 +1
→ 유동성 확보 완료
→ 영업일 종료
```

---

## 10. 유효 선택 제한

유동성 확보 중에는 현재 선택 상태에서 유효한 자원만 활성화한다.

잘못된 조합이 만들어지지 않도록 버튼 상태를 제한한다.

예시:

```text
리서치 선택
→ 두 번째 선택은 리서치 또는 다른 기본 자원 가능
```

리서치와 신용을 선택한 상태:

```text
선택: 리서치 + 신용

가능한 다음 선택:
- 현금
- 원자재

불가능:
- 리서치
- 신용
```

이유:

```text
리서치 + 신용 + 리서치
→ 같은 자원 2개 아님
→ 서로 다른 자원 3개도 아님
```

비활성화된 자원을 클릭하면 흔들림 피드백을 주고 선택 변화는 없다.

```text
비활성 자원 클릭
→ 흔들림
→ 자원 획득 없음
→ 선택 상태 변화 없음
```

---

## 11. 전문 자원 한도

전문 자원은 합산 보유 한도를 가진다.

```text
리서치 + 신용 + 원자재 <= 10
```

현금은 전문 자원 한도에 포함되지 않는다.

딜도 전문 자원 한도에 포함되지 않는다.

```text
전문 자원 한도 대상:
- 리서치
- 신용
- 원자재

전문 자원 한도 제외:
- 현금
- 딜
```

---

## 12. 전문 자원 한도에 따른 버튼 비활성화

유동성 확보 화면에서는 전문 자원 한도를 넘는 선택을 애초에 막는다.

전문 자원 합계가 10이면 전문 자원 버튼은 비활성화된다.

```text
리서치 + 신용 + 원자재 = 10 / 10
→ 리서치 비활성화
→ 신용 비활성화
→ 원자재 비활성화
→ 현금 활성화
```

이 상태에서도 유동성 확보는 가능하다.  
현금 선택이 가능하기 때문이다.

```text
전문 자원 10 / 10
→ 자원통 클릭 가능
→ 유동성 확보 화면 진입 가능
→ 현금만 선택 가능
```

---

## 13. 전문 자원 10/10에서 유동성 확보

전문 자원이 이미 10개인 상태에서 유동성 확보를 시작하면 현금만 선택할 수 있다.

```text
전문 자원 10 / 10

유동성 확보 시작
→ 현금 활성화
→ 리서치 비활성화
→ 신용 비활성화
→ 원자재 비활성화
```

이 경우 가능한 완료 방식은 현금 +2이다.

```text
현금 클릭
→ 현금 +1

현금 클릭
→ 현금 +1

결과:
현금 +2
→ 유동성 확보 완료
→ 영업일 종료
```

---

## 14. 전문 자원 9/10에서 유동성 확보

전문 자원이 9개인 상태에서는 전문 자원 하나를 더 선택할 수 있다.

예시:

```text
전문 자원 9 / 10

리서치 클릭
→ 리서치 +1
→ 전문 자원 10 / 10
→ 리서치 / 신용 / 원자재 비활성화
→ 현금만 활성화
```

이때 즉시 종료하지 않는다.  
현금이 아직 유효 선택지로 남아 있기 때문이다.

```text
전문 자원 10 / 10 도달
→ 현금 선택 가능
→ 계속 진행
```

이후 현금을 선택한다.

```text
현금 클릭
→ 현금 +1
→ 선택 상태: 리서치 + 현금
```

이 상태에서는 더 이상 유효한 선택지가 없으므로 자동 종료한다.

```text
더 이상 유효 선택지 없음
→ 자원칩 최대 한도 도달 메시지
→ 영업일 종료
```

결과:

```text
리서치 +1
현금 +1
→ 유동성 확보 종료
```

---

## 15. 선택지 소멸 자동 종료

유동성 확보는 조합이 정상 완성되지 않았더라도, 현재 상태에서 더 이상 유효한 자원 선택지가 없으면 자동으로 종료한다.

```text
현재 선택 상태
→ 선택 가능한 유효 자원 없음
→ 자동 종료
→ 영업일 종료
```

이 자동 종료는 주로 전문 자원 한도 때문에 발생한다.

대표 예시:

```text
전문 자원 9 / 10
→ 전문 자원 1개 획득
→ 전문 자원 10 / 10
→ 현금 1개 획득
→ 더 이상 유효 선택 없음
→ 자동 종료
```

이 경우 안내 메시지를 표시한다.

```text
자원칩 최대 한도 도달
```

---

## 16. 자동 종료 메시지

전문 자원 한도 때문에 더 이상 유효 선택지가 없어 자동 종료될 때는 다음 메시지를 표시한다.

```text
자원칩 최대 한도 도달
```

표시 상황:

```text
유동성 확보 중
→ 아직 같은 자원 2개 / 서로 다른 자원 3개 조합이 완성되지 않음
→ 전문 자원 한도 때문에 더 이상 유효 선택지가 없음
→ 자동 종료
→ 메시지 표시
```

정상 조합 완성으로 종료되는 경우에는 이 메시지를 표시하지 않는다.

```text
현금 +2
→ 정상 완료
→ 메시지 없음

리서치 + 신용 + 원자재
→ 정상 완료
→ 메시지 없음
```

---

## 17. 자원 획득 후 다른 행동 차단

유동성 확보에서 자원을 하나라도 획득한 뒤에는 다른 기본 행동을 할 수 없다.

```text
첫 자원 획득
→ 유동성 확보 행동 확정
→ 다른 행동 불가
```

불가능한 행동:

```text
- 자산 매수
- 카드 예약
- 다음 영업일
- 다른 카드 상세보기 진입
```

이 상태에서는 유동성 확보를 끝까지 진행해야 한다.

```text
유동성 확보 완료
또는
선택지 소멸 자동 종료
→ 영업일 종료
```

---

## 18. 자원 획득 전 다른 행동

유동성 확보 화면에 진입했지만 아직 자원을 획득하지 않은 상태에서는 닫기를 통해 시장 상태로 복귀할 수 있다.

```text
유동성 확보 진입
→ 자원 획득 전
→ 닫기 가능
→ 시장 상태 복귀
→ 영업일 소비 없음
```

다음 영업일 버튼은 유동성 확보 상태에서 비활성화된다.

```text
GainLiquidity 상태
→ 다음 영업일 비활성화
```

카드를 보고 싶다면 먼저 닫기를 눌러 시장 상태로 돌아가야 한다.

---

## 19. 유동성 확보와 운용 수익

유동성 확보로 얻은 현금은 운용 수익이 아니다.

```text
유동성 확보 현금
= 조달 현금
```

현금 선택 시 처리:

```text
현금 +1
→ CurrentCash 증가
→ CurrentQuarterEarnedCash 증가 없음
→ CurrentFiscalYearEarnedCash 증가 없음
→ TotalEarnedCash 증가 없음
```

전문 자원 선택 시 해당 자원을 증가시킨다.

```text
리서치 +1
신용 +1
원자재 +1
```

딜은 유동성 확보로 얻을 수 없다.

---

## 20. 유동성 확보 상태 데이터

구현상 유동성 확보 상태는 다음 데이터를 가진다.

```csharp
public class LiquidityActionState
{
    public List<ResourceType> SelectedResources = new();

    public bool HasGainedAnyResource;
}
```

`SelectedResources`는 이번 유동성 확보에서 이미 획득한 자원 순서를 기록한다.

`HasGainedAnyResource`는 닫기 가능 여부와 행동 확정 여부를 판단한다.

---

## 21. 유효 선택 판정

유동성 확보에서 자원을 선택할 수 있는지 판정한다.

```csharp
bool CanSelectLiquidityResource(ResourceType clickedResource)
{
    if (!IsBasicLiquidityResource(clickedResource))
        return false;

    if (IsProfessionalResource(clickedResource)
        && GetProfessionalResourceTotal() >= MaxProfessionalResourceTotal)
        return false;

    return IsValidByLiquidityCombinationRule(clickedResource);
}
```

기본 자원 판정:

```csharp
bool IsBasicLiquidityResource(ResourceType resourceType)
{
    return resourceType == ResourceType.Cash
        || resourceType == ResourceType.Research
        || resourceType == ResourceType.Credit
        || resourceType == ResourceType.Commodity;
}
```

전문 자원 판정:

```csharp
bool IsProfessionalResource(ResourceType resourceType)
{
    return resourceType == ResourceType.Research
        || resourceType == ResourceType.Credit
        || resourceType == ResourceType.Commodity;
}
```

---

## 22. 조합 규칙 판정

유동성 확보의 선택 상태에 따라 다음 선택이 유효한지 판정한다.

기본 규칙:

```text
- 같은 자원 2개
- 서로 다른 자원 3개
```

판정 예시:

```csharp
bool IsValidByLiquidityCombinationRule(ResourceType clickedResource)
{
    List<ResourceType> selected = LiquidityActionState.SelectedResources;

    if (selected.Count == 0)
        return true;

    if (selected.Count == 1)
        return true;

    if (selected.Count == 2)
    {
        bool twoSame = selected[0] == selected[1];
        bool twoDifferent = selected[0] != selected[1];

        if (twoSame)
            return false;

        if (twoDifferent)
        {
            return !selected.Contains(clickedResource);
        }
    }

    return false;
}
```

---

## 23. 완료 판정

유동성 확보가 정상 완료되었는지 판정한다.

```csharp
bool IsCompletedLiquiditySelection()
{
    List<ResourceType> selected = LiquidityActionState.SelectedResources;

    if (selected.Count == 2)
    {
        return selected[0] == selected[1];
    }

    if (selected.Count == 3)
    {
        return selected.Distinct().Count() == 3;
    }

    return false;
}
```

---

## 24. 유효 다음 선택 존재 여부

현재 상태에서 더 선택 가능한 자원이 있는지 확인한다.

```csharp
bool HasAnyValidNextLiquiditySelection()
{
    ResourceType[] candidates =
    {
        ResourceType.Cash,
        ResourceType.Research,
        ResourceType.Credit,
        ResourceType.Commodity
    };

    return candidates.Any(resource => CanSelectLiquidityResource(resource));
}
```

이 함수가 false이면 유동성 확보를 자동 종료한다.

```text
HasAnyValidNextLiquiditySelection == false
→ 자동 종료
```

---

## 25. 자원 클릭 처리 예시

```csharp
void OnLiquidityResourceClicked(ResourceType resourceType)
{
    if (!CanSelectLiquidityResource(resourceType))
    {
        PlayInvalidResourceShake(resourceType);
        return;
    }

    GainLiquidityResourceImmediately(resourceType);

    LiquidityActionState.SelectedResources.Add(resourceType);
    LiquidityActionState.HasGainedAnyResource = true;

    UpdateLiquidityCloseButton();
    UpdateLiquidityResourceButtons();
    UpdateSelectedResourcePreview();

    if (IsCompletedLiquiditySelection())
    {
        FinishLiquidityAction();
        return;
    }

    if (!HasAnyValidNextLiquiditySelection())
    {
        ShowShortFeedback("자원칩 최대 한도 도달");
        FinishLiquidityAction();
        return;
    }
}
```

---

## 26. 자원 즉시 획득 처리

```csharp
void GainLiquidityResourceImmediately(ResourceType resourceType)
{
    switch (resourceType)
    {
        case ResourceType.Cash:
            AddFundingCash(1);
            break;

        case ResourceType.Research:
        case ResourceType.Credit:
        case ResourceType.Commodity:
            AddProfessionalResource(resourceType, 1);
            break;

        case ResourceType.Deal:
            // 유동성 확보로 딜은 획득할 수 없음
            break;
    }
}
```

유동성 확보에서 전문 자원 버튼은 한도를 넘지 않게 비활성화되므로, 일반적으로 초과 폐기가 발생하지 않는다.  
다만 안전성을 위해 `AddProfessionalResource`는 기존 초과 폐기 규칙을 그대로 사용한다.

---

## 27. 유동성 확보 종료

유동성 확보가 완료되면 다음 처리를 한다.

```text
1. 유동성 확보 상태 정리
2. 시장 영역을 Market 상태로 복귀
3. 영업일 종료
```

예시:

```csharp
void FinishLiquidityAction()
{
    ClearLiquidityActionState();

    SetMarketAreaState(MarketAreaState.Market);

    EndBusinessDay();
}
```

---

## 28. 닫기 처리 예시

```csharp
bool CanCloseLiquidityView()
{
    return CurrentMarketAreaState == MarketAreaState.GainLiquidity
        && !LiquidityActionState.HasGainedAnyResource;
}
```

```csharp
void OnCloseLiquidityClicked()
{
    if (!CanCloseLiquidityView())
    {
        PlayInvalidCloseFeedback();
        return;
    }

    ClearLiquidityActionState();

    SetMarketAreaState(MarketAreaState.Market);
}
```

---

## 29. 버튼 상태 갱신

유동성 확보 화면에서는 자원 버튼 상태를 매 선택 후 갱신한다.

```csharp
void UpdateLiquidityResourceButtons()
{
    foreach (ResourceType resourceType in BasicLiquidityResources)
    {
        bool canSelect = CanSelectLiquidityResource(resourceType);

        SetLiquidityResourceButtonInteractable(resourceType, canSelect);
    }
}
```

비활성화된 버튼은 클릭 시 흔들림 피드백을 줄 수 있다.  
다만 UI 프레임워크상 비활성 버튼이 클릭 이벤트를 받지 못한다면, 상위 투명 입력 영역을 두거나 시각 피드백을 생략할 수 있다.

---

## 30. 구현 시 주의사항

```text
- 유동성 확보는 자원통 클릭으로 진입한다.
- 유동성 확보 화면은 시장 영역을 대체한다.
- 자원통 클릭 자체는 영업일을 소비하지 않는다.
- 첫 자원 클릭 순간 유동성 확보 행동이 확정된다.
- 첫 자원 획득 전에는 닫기 가능하다.
- 자원 1개 이상 획득 후에는 닫기 불가다.
- 유동성 확보로 딜은 얻을 수 없다.
- 현금은 유동성 확보로 얻을 수 있다.
- 유동성 확보 현금은 운용 수익에 포함하지 않는다.
- 기본 완료 조건은 같은 자원 2개 또는 서로 다른 자원 3개다.
- 전문 자원 합계가 10이면 전문 자원 버튼은 비활성화한다.
- 전문 자원 10/10이어도 유동성 확보는 가능하며, 현금만 선택 가능하다.
- 전문 자원 9/10에서 전문 자원 1개 획득 후 현금 선택이 가능하다.
- 더 이상 유효 선택지가 없으면 자동 종료한다.
- 한도 때문에 자동 종료될 때는 `자원칩 최대 한도 도달` 메시지를 표시한다.
- 유동성 확보 중 다음 영업일 버튼은 비활성화한다.
```
