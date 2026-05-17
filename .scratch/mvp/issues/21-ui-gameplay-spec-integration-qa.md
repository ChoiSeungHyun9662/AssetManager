# 21. UI_GAMEPLAY_SPEC 통합 QA와 첫 애셋 적용 검수

Status: superseded

## Superseded by

- `.scratch/stock-rules-overhaul/issues/13-stock-overhaul-integration-qa.md`

Do not run this QA issue as written. The validation target is no longer the old UI_GAMEPLAY_SPEC flow with card detail, Payment Pot, 3-zone market, and GainLiquidity. Use the stock overhaul integration QA issue instead.

## Parent

.scratch/mvp/UI_GAMEPLAY_SPEC.md

## What to build

UI_GAMEPLAY_SPEC 적용 slice들이 Unity Play Mode에서 하나의 플레이 흐름으로 잘 이어지는지 검수한다. 자동 테스트가 잡을 수 있는 규칙 회귀는 기존 테스트 흐름으로 확인하고, 화면의 읽힘, 작은 크기에서의 애셋 구분, 조작감, 카드 상세보기와 Payment Pot의 물리적 결제 느낌은 사람이 확인한다.

우선 검수 대상 애셋:

- Icon_Cash
- Icon_Research
- Icon_Credit
- Icon_Commodity
- Icon_Deal
- Cash_BillStack_Normal
- Chip_Research_Normal
- Chip_Credit_Normal
- Chip_Commodity_Normal
- Chip_Deal_Normal
- PaymentPot_Background_Default

## Acceptance criteria

- [ ] Play Mode에서 시장 테이프의 매도 임박, 현재 시장, 예비 시장 구분이 읽힌다.
- [ ] Play Mode에서 Bottom Chip Tray의 현금, 리서치, 신용, 원자재, 딜이 작은 크기에서도 구분된다.
- [ ] Play Mode에서 자원 확보 선택과 Bottom Chip Tray 반영 흐름이 자연스럽게 이어진다.
- [ ] Play Mode에서 매수 가능한 카드 상세보기의 Large Card, Payment Pot, Actions 구조가 읽힌다.
- [ ] Play Mode에서 Payment Pot의 비용 슬롯, 칩 배치, 딜 대체, Buy 활성 상태를 확인한다.
- [ ] Play Mode에서 예비 시장 카드 상세보기는 미리보기 전용으로 읽히고 Buy/예약이 보이지 않는다.
- [ ] Play Mode에서 Right Context가 고정 플레이어 보드로 유지되는지 확인한다.
- [ ] Play Mode에서 분기 마감 결과판이 공통 프레임 안에서 읽히는지 확인한다.
- [ ] 대상 애셋 누락 시 placeholder가 유지되고 누락 이름을 확인할 수 있다.
- [ ] 최종 검수 결과와 남은 시각/상호작용 리스크가 이슈 코멘트에 기록된다.

## Blocked by

- .scratch/mvp/issues/15-resource-object-assets-and-gain-liquidity-ui.md
- .scratch/mvp/issues/16-market-asset-card-icon-language.md
- .scratch/mvp/issues/17-payment-pot-background-and-cost-slot-visuals.md
- .scratch/mvp/issues/18-card-detail-action-board-and-preview-state.md
- .scratch/mvp/issues/19-right-context-reservation-and-portfolio-board.md
- .scratch/mvp/issues/20-quarter-result-board-in-shared-frame.md

## User stories covered

7, 13, 15, 22, 26, 29, 30, 40, 49, 52
