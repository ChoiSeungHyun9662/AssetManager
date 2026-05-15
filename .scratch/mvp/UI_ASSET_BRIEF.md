# UI_ASSET_BRIEF_v0.1

상태: AI 에셋 제작 요청서  
대상 도구: Recraft 우선, Layer 보조, Scenario 보류  
기준 문서: UI_GAMEPLAY_SPEC_v0.2.md  
범위: UI 아이콘, 칩, 배지, 슬롯, 버튼, 카드 프레임, 패널 스킨, 게이지, 제작 프롬프트  
비범위: Unity 구현 로직, 게임 밸런스, 애니메이션 타이밍

---

## 1. Tool Strategy

### Recommended Order

Recraft:
- 최종 UI 전체 제작 도구가 아니다.
- 먼저 Symbol Kit를 만든다.
- 자원 심볼, 비용/수익/가치 아이콘, 게이지 아이콘을 확정한다.

Layer:
- Recraft Symbol Kit를 바탕으로 최종 물성 에셋을 만든다.
- 칩, 지폐 다발, 카드 프레임, Payment Pot, 패널 스킨을 만든다.

Unity:
- Symbol Kit와 Material UI Kit가 섞였을 때 톤이 맞는지 검증한다.

### Current Recommendation

Recraft를 1차 도구로 사용한다.  
단, Recraft로 전체 화면이나 완성형 게임 UI를 한 번에 만들려고 하지 않는다.

Recraft의 역할은 이 게임의 작은 UI 언어를 먼저 확정하는 것이다.

- 자원들이 어떻게 생겼는가
- 칩 심볼이 어떻게 구분되는가
- 비용/수익/가치/압력 아이콘이 어떻게 읽히는가
- Payment Slot 상태가 어떻게 구분되는가

Layer는 Recraft 결과물이 너무 평면적이거나 게임 오브젝트 느낌이 부족할 때 추가한다.

---

## 2. Art Direction

### Core Direction

- Premium board game table
- Asset management theme
- Tactile chips
- Clear card decision
- Financial tension
- Calm reward feedback

### Visual Goal

플레이어가 고급 보드게임 테이블 위에서 자산 카드를 고르고, 칩을 던져 결제하며, 제한된 영업일 안에 수익을 만드는 느낌.

### Must Avoid

- 실제 증권 HTS처럼 차갑고 복잡한 화면
- 카지노처럼 과한 금색 장식
- 모바일 방치형 게임처럼 과도한 재화 UI
- 카드 일러스트가 정보 가독성을 잡아먹는 구조
- 보드게임 오브젝트 대신 줄글 패널이 화면을 지배하는 구조
- 작은 크기에서 서로 구분되지 않는 아이콘

### Priority

1. 비용 / 자원 가독성
2. 클릭 가능한 대상 명확성
3. 칩 조작의 촉각감
4. 카드 비교 편의성
5. 행동 결과 피드백
6. 보상 연출
7. 장식성

---

## 3. Naming Rule

파일명은 다음 규칙을 따른다.

```text
Component_Type_State
```

Examples:

```text
Chip_Research_Normal
Chip_Research_Selected
Chip_Deal_InPaymentPot
Cash_BillStack_Normal
PaymentSlot_Empty
PaymentSlot_Filled
Button_Buy_Disabled
AssetCard_Frame_Large
Gauge_RedemptionPressure_Normal
```

### State Names

공통 상태는 다음 이름을 우선 사용한다.

- Normal
- Hovered
- Selected
- Disabled
- Filled
- Invalid
- Highlighted
- InPaymentPot
- CostComplete
- InsufficientCash

---

## 4. Priority 0: Core Resource Objects

가장 먼저 제작한다.  
게임의 조작감과 비용 언어를 결정하는 핵심 에셋이다.

### Cash

현금은 칩이 아니라 지폐 다발이다.

Required Assets:

- Cash_BillStack_Normal
- Cash_BillStack_Disabled
- Cash_BillStack_SmallIcon
- Cash_CostBadge

Production Notes:

- 전문자원 칩과 확실히 다른 형태여야 한다.
- 드래그 가능한 칩처럼 보이면 안 된다.
- 구매력/현금 비용을 나타내는 시각 언어로 사용한다.
- 너무 현실적인 지폐보다는 보드게임 토큰화된 지폐 다발이 좋다.

### Research Chip

Required Assets:

- Chip_Research_Normal
- Chip_Research_Hovered
- Chip_Research_Selected
- Chip_Research_Disabled
- Chip_Research_InPaymentPot
- Icon_Research

### Credit Chip

Required Assets:

- Chip_Credit_Normal
- Chip_Credit_Hovered
- Chip_Credit_Selected
- Chip_Credit_Disabled
- Chip_Credit_InPaymentPot
- Icon_Credit

### Commodity Chip

Required Assets:

- Chip_Commodity_Normal
- Chip_Commodity_Hovered
- Chip_Commodity_Selected
- Chip_Commodity_Disabled
- Chip_Commodity_InPaymentPot
- Icon_Commodity

### Deal Chip

Required Assets:

- Chip_Deal_Normal
- Chip_Deal_Hovered
- Chip_Deal_Selected
- Chip_Deal_Disabled
- Chip_Deal_InPaymentPot
- Chip_Deal_AsSubstitute
- Icon_Deal

Production Notes For All Chips:

- 각 칩은 색상뿐 아니라 심볼로도 구분되어야 한다.
- 작은 크기에서도 구분 가능해야 한다.
- 카지노칩처럼 너무 화려하면 안 된다.
- 고급 보드게임 토큰 느낌을 유지한다.
- 투명 배경 PNG 또는 SVG로 출력한다.
- Unity UI에서 64px~128px 크기에서도 읽혀야 한다.

---

## 5. Priority 1: Payment Pot / Payment Slot

Payment Pot은 전문자원 비용을 지불하는 물리적 결제 테이블이다.

### Required Assets

- PaymentPot_Background_Default
- PaymentPot_Background_Highlighted
- PaymentPot_Background_Invalid
- PaymentPot_Background_CostComplete
- PaymentSlot_Empty
- PaymentSlot_ValidHover
- PaymentSlot_Filled
- PaymentSlot_Invalid
- PaymentSlot_DealSubstituted
- PaymentSlot_GhostRequirement
- PaymentSlot_SnapGlow

### Production Notes

- Payment Pot은 하나의 넓은 결제 테이블처럼 보여야 한다.
- 현금 슬롯은 만들지 않는다.
- Payment Slot은 전문자원 요구 슬롯만 표현한다.
- 빈 슬롯에는 흐릿한 요구 자원 아이콘이 보인다.
- 딜 칩 대체 슬롯은 일반 Filled 상태와 구분되어야 한다.
- Invalid 상태는 명확하지만 과하게 벌칙처럼 보이면 안 된다.

---

## 6. Priority 2: Asset Card UI

자산 카드는 하스스톤형 세로 카드처럼 명확한 정체성을 가져야 한다.  
그러나 카드 안의 텍스트는 최소화한다.

### Required Assets

- AssetCard_Frame_Large
- AssetCard_Frame_Market
- AssetCard_Frame_PreviewSmall
- AssetCard_Frame_MiniReserved
- AssetCard_Frame_MiniOwned
- AssetCard_Frame_Hovered
- AssetCard_Frame_Selected
- AssetCard_Frame_Affordable
- AssetCard_Frame_NotAffordable
- AssetCard_Frame_Reserved

### Card Parts

- AssetCard_NameRibbon
- AssetCard_RarityRibbon_Common
- AssetCard_RarityRibbon_Rare
- AssetCard_RarityRibbon_Epic
- AssetCard_TagBadge
- AssetCard_CashBadge
- AssetCard_IncomeBadge
- AssetCard_LongTermValueBadge
- AssetCard_ResourceCostStackBackground
- AssetCard_ImageMask

### Production Notes

- 카드 프레임은 하스스톤처럼 강한 정체성을 가지되, 판타지 장식으로 과해지면 안 된다.
- 금융/자산 운용 테마를 가진 보드게임 카드 느낌이 우선이다.
- 비용, 수익, 장기 가치, 태그가 일러스트에 묻히지 않아야 한다.
- 카드 이름과 태그명만 텍스트로 들어간다.
- 효과문 줄글은 카드에 넣지 않는다.

---

## 7. Priority 3: Action Buttons

### Required Assets

- Button_Buy_Enabled
- Button_Buy_Disabled
- Button_Buy_InsufficientCash
- Button_Buy_Hovered
- Button_Buy_Pressed
- Button_Reserve_Normal
- Button_Reserve_Hovered
- Button_Reserve_Pressed
- Button_Back_Normal
- Button_Back_Hovered
- Button_MarketPeek_Normal
- Button_MarketPeek_Held
- Button_Continue_Normal
- Button_Continue_Hovered

### Button Icon Assets

- Icon_Buy
- Icon_Reserve
- Icon_Back
- Icon_MarketPeek
- Icon_Continue

### Production Notes

- Buy와 Reserve는 큰 원형 버튼이다.
- Buy는 현금 비용을 함께 표시할 수 있어야 한다.
- Reserve는 환매 압력 +1 / 딜 +1을 아이콘으로 표현해야 한다.
- Disabled 상태는 명확해야 한다.
- Market Peek은 press-and-hold 성격이 드러나야 한다.

---

## 8. Priority 4: Persistent UI Panels And Gauges

### Top Status Bar

Required Assets:

- TopStatusBar_Background
- Icon_FiscalYear
- Icon_Quarter
- Icon_BusinessDay
- Gauge_QuarterTarget_Background
- Gauge_QuarterTarget_Fill
- Gauge_RedemptionPressure_Background
- Gauge_RedemptionPressure_Fill
- Icon_QuarterTarget
- Icon_RedemptionPressure
- Icon_CurrentProfit

### Right Context

Required Assets:

- RightContext_Background
- ReservedCardSlot_Empty
- OwnedAssetSummary_Background
- Icon_OwnedCount
- Icon_TotalQuarterIncome
- Icon_TotalLongTermValue

### Bottom Chip Tray

Required Assets:

- BottomChipTray_Background
- ChipArea_Background_Cash
- ChipArea_Background_Research
- ChipArea_Background_Credit
- ChipArea_Background_Commodity
- ChipArea_Background_Deal

### Quarter Result

Required Assets:

- QuarterResult_Board
- QuarterResult_TargetSuccess
- QuarterResult_TargetFailed
- QuarterResult_PressureIncrease
- QuarterResult_ContinueHighlight

---

## 9. Recraft Prompt: Core Resource Chips

```text
Create a consistent set of premium tabletop board-game resource objects for a PC strategy card game about asset management.

Objects:
1. Cash bill stack
2. Research chip
3. Credit chip
4. Commodity chip
5. Deal chip

Style:
Premium board game table, tactile physical tokens, asset management theme, clean readable symbols, calm financial tension, not flashy casino, not cold stock trading app, not mobile gacha currency clutter.

Requirements:
- Transparent background.
- Each resource must be distinguishable by both color and symbol.
- Cash must look like a bill stack, not a chip.
- Research, Credit, Commodity, and Deal must look like physical chips/tokens.
- Deal should feel like a wild substitute token.
- Must remain readable at small UI sizes.
- Consistent visual language across all objects.
- Create normal, hovered, selected, disabled, and in-payment-pot variants.
```

---

## 10. Recraft Prompt: Card Badges And Icons

```text
Create a set of clean UI badge icons for a premium board-game style asset management card game.

Badges:
- Cash cost badge
- Quarter income badge
- Long-term value badge
- Tag badge
- Rarity ribbon
- Redemption pressure icon
- Quarter target icon
- Business day icon
- Current profit icon

Style:
Premium tabletop board game UI, readable financial symbols, subtle metallic or paper texture, not realistic stock trading software, not fantasy RPG, not flashy casino.

Requirements:
- Transparent background.
- Consistent shape language.
- Readable at small size.
- Icons should work on dark tabletop UI background.
- Avoid detailed text inside the icon.
```

---

## 11. Recraft Prompt: Payment Slots

```text
Create UI assets for payment slots in a premium tabletop strategy card game.

Slot states:
- Empty
- Valid hover
- Filled
- Invalid
- Deal substituted
- Ghost requirement

Style:
Premium board game payment table, tactile chip slots, subtle glow for valid state, clear but not aggressive invalid state, readable at small size.

Rules:
- Slots represent professional resource requirements only.
- Cash is not represented as a slot.
- Deal substituted state must look different from normal filled state.
- Empty slot should show a faint ghost icon of the required resource.
- Transparent background where appropriate.
```

---

## 12. Layer Prompt: Card Frame And Payment Table

Use Layer only if Recraft outputs are too flat or not game-like enough.

```text
Create game UI assets for a premium tabletop asset-management card game.

Assets:
- Vertical asset card frame, large version
- Market card frame, medium version
- Preview card frame, small version
- Payment table / Payment Pot background
- Top status bar panel
- Right context panel
- Bottom chip tray panel

Style:
Premium board game table, asset management, tactile chips, clean readable financial UI, subtle paper/metal/tabletop material, not cold stock trading terminal, not flashy casino, not fantasy RPG.

Requirements:
- Unity UI friendly.
- Transparent background where needed.
- 9-slice compatible panels if possible.
- Leave clean interior space for text, icons, and card art.
- Visual detail must not reduce readability.
```

---

## 13. Scenario Usage Rule

Do not use Scenario at the current UI asset stage.

Use Scenario later only if all conditions are met:

- 카드 일러스트가 50장 이상 필요하다.
- 카드 아트 스타일이 이미 확정되었다.
- 10~50개 이상의 스타일 레퍼런스를 확보했다.
- 동일한 스타일의 자산 카드 이미지를 대량으로 생성해야 한다.

