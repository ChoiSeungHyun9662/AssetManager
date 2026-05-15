# UI_INTEGRATION_QA_v0.1

상태: Unity 적용 및 QA 기준 문서  
대상: Codex, Unity 구현자, 솔로 개발자  
기준 문서: UI_GAMEPLAY_SPEC_v0.2.md, UI_ASSET_BRIEF_v0.1.md  
범위: 에셋 임포트, 파일명 매핑, 구현 우선순위, 검증 체크리스트

---

## 1. Purpose

이 문서는 Recraft / Layer 등에서 생성한 UI 에셋을 Unity에 적용할 때, Codex가 어떤 순서로 연결하고 무엇을 검증해야 하는지 정의한다.

목표는 예쁜 화면이 아니라 다음을 보장하는 것이다.

- 정보가 읽힌다.
- 조작 가능한 대상이 명확하다.
- 칩 드래그와 Payment Pot 스냅이 자연스럽다.
- Buy / Reserve / Back / Market Peek의 상태가 헷갈리지 않는다.
- 아트가 게임 규칙을 가리지 않는다.

---

## 2. Import Rules

### General

- 모든 UI 에셋은 Unity UI에서 사용할 수 있어야 한다.
- 투명 배경이 필요한 에셋은 PNG 또는 SVG로 관리한다.
- 상태별 에셋은 파일명으로 상태를 구분한다.
- 에셋이 누락되면 placeholder를 유지하고 경고 로그를 남긴다.

### Recommended Formats

| Asset Type | Preferred Format | Notes |
|---|---|---|
| Icons | SVG or high-res PNG | 작은 크기에서 선명해야 함 |
| Chips | Transparent PNG | 물성 있는 렌더링이면 PNG 권장 |
| Card Frames | PNG | 필요 시 9-slice 검토 |
| Panels | 9-slice PNG | 크기 변경이 잦은 패널 |
| Buttons | State-based PNG | Enabled / Disabled / Hovered / Pressed |
| Gauges | Fillable UI Image or segmented PNG | Unity fillAmount 사용 가능하면 우선 |

---

## 3. Suggested Unity Folder Structure

```text
Assets/
  Art/
    UI/
      Chips/
      Cash/
      Cards/
      PaymentPot/
      Buttons/
      Panels/
      Gauges/
      Icons/
      QuarterResult/
```

---

## 4. Asset Mapping

### ResourceChip

```text
ResourceChip.Research.Normal        -> Chip_Research_Normal
ResourceChip.Research.Hovered       -> Chip_Research_Hovered
ResourceChip.Research.Selected      -> Chip_Research_Selected
ResourceChip.Research.Disabled      -> Chip_Research_Disabled
ResourceChip.Research.InPaymentPot  -> Chip_Research_InPaymentPot

ResourceChip.Credit.Normal          -> Chip_Credit_Normal
ResourceChip.Credit.Hovered         -> Chip_Credit_Hovered
ResourceChip.Credit.Selected        -> Chip_Credit_Selected
ResourceChip.Credit.Disabled        -> Chip_Credit_Disabled
ResourceChip.Credit.InPaymentPot    -> Chip_Credit_InPaymentPot

ResourceChip.Commodity.Normal       -> Chip_Commodity_Normal
ResourceChip.Commodity.Hovered      -> Chip_Commodity_Hovered
ResourceChip.Commodity.Selected     -> Chip_Commodity_Selected
ResourceChip.Commodity.Disabled     -> Chip_Commodity_Disabled
ResourceChip.Commodity.InPaymentPot -> Chip_Commodity_InPaymentPot

ResourceChip.Deal.Normal            -> Chip_Deal_Normal
ResourceChip.Deal.Hovered           -> Chip_Deal_Hovered
ResourceChip.Deal.Selected          -> Chip_Deal_Selected
ResourceChip.Deal.Disabled          -> Chip_Deal_Disabled
ResourceChip.Deal.InPaymentPot      -> Chip_Deal_InPaymentPot
ResourceChip.Deal.AsSubstitute      -> Chip_Deal_AsSubstitute
```

### Cash

```text
Cash.Normal      -> Cash_BillStack_Normal
Cash.Disabled    -> Cash_BillStack_Disabled
Cash.SmallIcon   -> Cash_BillStack_SmallIcon
Cash.CostBadge   -> Cash_CostBadge
```

### Payment Pot / Slot

```text
PaymentPot.Default       -> PaymentPot_Background_Default
PaymentPot.Highlighted   -> PaymentPot_Background_Highlighted
PaymentPot.Invalid       -> PaymentPot_Background_Invalid
PaymentPot.CostComplete  -> PaymentPot_Background_CostComplete

PaymentSlot.Empty            -> PaymentSlot_Empty
PaymentSlot.ValidHover       -> PaymentSlot_ValidHover
PaymentSlot.Filled           -> PaymentSlot_Filled
PaymentSlot.Invalid          -> PaymentSlot_Invalid
PaymentSlot.DealSubstituted  -> PaymentSlot_DealSubstituted
PaymentSlot.GhostRequirement -> PaymentSlot_GhostRequirement
```

### Asset Card

```text
AssetCard.Frame.Large          -> AssetCard_Frame_Large
AssetCard.Frame.Market         -> AssetCard_Frame_Market
AssetCard.Frame.PreviewSmall   -> AssetCard_Frame_PreviewSmall
AssetCard.Frame.MiniReserved   -> AssetCard_Frame_MiniReserved
AssetCard.Frame.MiniOwned      -> AssetCard_Frame_MiniOwned
AssetCard.Frame.Hovered        -> AssetCard_Frame_Hovered
AssetCard.Frame.Selected       -> AssetCard_Frame_Selected
AssetCard.Frame.Affordable     -> AssetCard_Frame_Affordable
AssetCard.Frame.NotAffordable  -> AssetCard_Frame_NotAffordable
AssetCard.Frame.Reserved       -> AssetCard_Frame_Reserved

AssetCard.NameRibbon           -> AssetCard_NameRibbon
AssetCard.TagBadge             -> AssetCard_TagBadge
AssetCard.CashBadge            -> AssetCard_CashBadge
AssetCard.IncomeBadge          -> AssetCard_IncomeBadge
AssetCard.LongTermValueBadge   -> AssetCard_LongTermValueBadge
```

### Buttons

```text
Button.Buy.Enabled             -> Button_Buy_Enabled
Button.Buy.Disabled            -> Button_Buy_Disabled
Button.Buy.InsufficientCash    -> Button_Buy_InsufficientCash
Button.Buy.Hovered             -> Button_Buy_Hovered
Button.Buy.Pressed             -> Button_Buy_Pressed

Button.Reserve.Normal          -> Button_Reserve_Normal
Button.Reserve.Hovered         -> Button_Reserve_Hovered
Button.Reserve.Pressed         -> Button_Reserve_Pressed

Button.Back.Normal             -> Button_Back_Normal
Button.Back.Hovered            -> Button_Back_Hovered

Button.MarketPeek.Normal       -> Button_MarketPeek_Normal
Button.MarketPeek.Held         -> Button_MarketPeek_Held

Button.Continue.Normal         -> Button_Continue_Normal
Button.Continue.Hovered        -> Button_Continue_Hovered
```

---

## 5. Implementation Priority For Codex

### Priority 0: Keep Existing MVP Functional

- 새 에셋 적용 중에도 기존 MVP 룰이 깨지면 안 된다.
- 에셋이 없어도 placeholder로 플레이 가능해야 한다.

### Priority 1: Resource Objects

- Cash bill stack 표시
- Research / Credit / Commodity / Deal 칩 표시
- Bottom Chip Tray에서 칩 상태 표시
- Disabled 상태 처리

### Priority 2: Payment Pot / Slot

- Payment Pot 배경 적용
- Payment Slot 상태 에셋 적용
- Valid / Invalid / Filled / DealSubstituted 상태 연결
- 칩 드래그, 스냅, 회수 기존 기능 유지

### Priority 3: Buttons

- Buy / Reserve / Back / Market Peek / Continue 버튼 상태 에셋 연결
- Buy 활성/비활성 조건 시각화
- InsufficientCash 상태 표시

### Priority 4: Asset Card

- 카드 프레임 적용
- Cash badge / resource cost / income / long-term value / tag 위치 확인
- Market / Large / Small / Mini 사이즈별 가독성 확인

### Priority 5: Persistent Panels

- Top Status Bar
- Right Context
- Bottom Chip Tray
- Gauges

### Priority 6: Quarter Result

- Quarter Result Board
- Target Success / Failed
- Pressure Increase
- Continue highlight

---

## 6. QA Checklist: Global Readability

- [ ] 현금과 전문자원이 명확히 다른 형태로 보이는가?
- [ ] 리서치 / 신용 / 원자재 / 딜 칩이 작은 크기에서도 구분되는가?
- [ ] 색상 없이 심볼만 봐도 자원 구분이 가능한가?
- [ ] Top Status Bar가 자원 영역처럼 보이지 않는가?
- [ ] Bottom Chip Tray가 플레이어의 물리적 자원 영역처럼 보이는가?
- [ ] Right Context가 선택 카드 상세 패널처럼 보이지 않는가?
- [ ] 카드 일러스트가 비용/수익/가치 정보를 방해하지 않는가?
- [ ] 전체 화면이 증권 HTS처럼 차갑지 않은가?
- [ ] 전체 화면이 카지노처럼 과하게 금색/화려하지 않은가?

---

## 7. QA Checklist: Market State

- [ ] 매도 임박 / 현재 시장 / 예비 시장 3열이 즉시 구분되는가?
- [ ] 매도 임박과 현재 시장 카드는 클릭 가능해 보이는가?
- [ ] 예비 시장 카드는 살 수 없는 미래 정보처럼 보이는가?
- [ ] 예비 시장 카드는 Buy / Reserve가 불가능하다는 점이 명확한가?
- [ ] 시장 흐름이 예비 → 현재 → 매도 임박 → 제거로 느껴지는가?
- [ ] 열 제목 외 설명문 없이도 열 역할이 대략 전달되는가?

---

## 8. QA Checklist: Card Detail State

- [ ] Large Card Showcase가 하스스톤형 세로 카드처럼 명확히 보이는가?
- [ ] Card Detail이 줄글 상세 패널처럼 보이지 않는가?
- [ ] Payment Pot이 전문자원 결제 영역으로 명확히 보이는가?
- [ ] 현금 슬롯이 없다는 점이 자연스러운가?
- [ ] Buy 버튼에 최종 현금 비용이 명확히 표시되는가?
- [ ] Reserve 버튼에 환매 압력 +1 / 딜 +1이 아이콘으로 보이는가?
- [ ] 구매 영향 스트립이 Right Context가 아니라 Actions 근처에 있는가?
- [ ] Back / Close가 보조 액션으로 보이는가?
- [ ] Market Peek이 press-and-hold 성격으로 이해되는가?

---

## 9. QA Checklist: Payment Pot

- [ ] 유효한 칩을 슬롯에 드롭하면 자연스럽게 스냅되는가?
- [ ] 유효하지 않은 칩은 명확히 거부되거나 되돌아가는가?
- [ ] 빈 슬롯에 요구 자원 고스트 아이콘이 보이는가?
- [ ] 딜 칩이 어떤 전문자원 슬롯에도 들어갈 수 있다는 점이 보이는가?
- [ ] 딜로 대체한 슬롯이 일반 Filled 상태와 구분되는가?
- [ ] 딜 1개당 현금 비용 -1이 Buy 버튼에 즉시 반영되는가?
- [ ] 슬롯에 놓은 칩을 회수할 수 있다는 점이 자연스러운가?
- [ ] 비용이 모두 충족되기 전에는 Buy가 비활성인가?
- [ ] 현금이 부족하면 Buy가 InsufficientCash 상태로 보이는가?

---

## 10. QA Checklist: Bottom Chip Tray

- [ ] 현금 지폐 다발은 드래그 가능한 칩처럼 보이지 않는가?
- [ ] 전문자원 칩은 드래그 가능한 물체처럼 보이는가?
- [ ] 같은 자원 칩이 여러 개 있을 때 겹침이 자연스러운가?
- [ ] 칩이 2줄까지 늘어나도 영역을 침범하지 않는가?
- [ ] Payment Pot이 비활성일 때 칩 드래그가 불가능하다는 점이 보이는가?
- [ ] Payment Pot이 활성일 때 드래그 가능한 상태가 명확한가?

---

## 11. QA Checklist: Gain Liquidity State

- [ ] 자원 선택 화면이 카드 선택 화면처럼 보이지 않는가?
- [ ] 현금 / 리서치 / 신용 / 원자재 선택지가 오브젝트처럼 보이는가?
- [ ] 딜이 선택지에 없다는 점이 명확한가?
- [ ] 선택 즉시 Bottom Chip Tray에 반영되는가?
- [ ] 남은 선택 횟수가 즉시 줄어드는가?
- [ ] 상한에 걸린 자원은 비활성처럼 보이는가?

---

## 12. QA Checklist: Quarter Result State

- [ ] Center Main Board가 분기 결과에 집중하는가?
- [ ] 분기 수익과 목표 비교가 가장 크게 보이는가?
- [ ] 목표 미달 시 환매 압력 증가가 명확한가?
- [ ] Continue가 주 액션으로 보이는가?
- [ ] Right Context와 Bottom Chip Tray는 유지되지만 낮은 강조 상태인가?

---

## 13. Common Failure Cases And Fixes

### Problem: UI looks like a stock trading terminal.

Fix:
- 패널을 더 보드게임 오브젝트처럼 만든다.
- 줄글과 표를 줄인다.
- 카드/칩/토큰의 물성을 강화한다.

### Problem: UI looks too much like a casino.

Fix:
- 금색 장식과 강한 반짝임을 줄인다.
- 금융/자산 운용 심볼을 강화한다.
- 보상 연출보다 정보 가독성을 우선한다.

### Problem: Chips look good but are hard to distinguish.

Fix:
- 색상 차이만 쓰지 말고 심볼과 실루엣을 다르게 한다.
- 작은 크기 테스트를 먼저 한다.
- Deal은 와일드 토큰이라는 정체성을 더 강하게 만든다.

### Problem: Card art dominates card information.

Fix:
- 일러스트 영역을 줄인다.
- 비용 / 수익 / 장기 가치 배지를 더 강하게 만든다.
- 카드 프레임 장식을 줄인다.

### Problem: Buy state is unclear.

Fix:
- Buy Enabled / Disabled / InsufficientCash 상태를 별도 에셋으로 분리한다.
- Payment Pot CostComplete와 Buy Enabled를 동시에 강조한다.
- 현금 비용 표시를 Buy 버튼 내부 또는 바로 아래에 둔다.

### Problem: Right Context becomes too noisy.

Fix:
- 예약 카드는 미니 카드 중심으로 유지한다.
- 보유 자산은 개별 카드보다 요약 숫자 중심으로 유지한다.
- 구매 영향은 Right Context가 아니라 Actions 근처에 둔다.

