# 18. 카드 상세보기 액션 보드와 예비 시장 미리보기

Status: ready-for-agent

## Parent

.scratch/mvp/UI_GAMEPLAY_SPEC.md

## What to build

카드 상세보기를 Large Card, Payment Pot, Actions의 3분할 보드로 개선한다. 매도 임박과 현재 시장 카드는 매수 가능한 상세보기로 열리고, 예비 시장 카드는 미래 정보 미리보기로 열린다. 매수 가능한 상세보기에는 Buy, 예약, Back, Market Peek, 구매 영향 스트립이 보인다. 예비 시장 상세보기에는 Buy, 예약, Payment Pot이 보이지 않고 큰 카드 미리보기와 Close, Market Peek만 보인다.

Market Peek은 토글이 아니라 누르고 있는 동안만 상세보기를 숨겨 뒤의 시장 테이프를 보는 상호작용으로 유지한다. Market Peek 중에는 뒤의 시장 카드가 조작되지 않아야 한다.

## Acceptance criteria

- [ ] 카드 상세보기에서도 Top Status Bar, Right Context, Bottom Chip Tray가 유지된다.
- [ ] 매수 가능한 카드 상세보기는 Large Card, Payment Pot, Actions가 한 시장 영역 안에서 함께 보인다.
- [ ] Buy 버튼은 큰 주 액션으로 보이며 최종 현금 비용을 현금 아이콘 또는 지폐 언어와 함께 표시한다.
- [ ] 예약 버튼은 환매 압력 +1과 딜 +1을 아이콘+숫자 중심으로 표시한다.
- [ ] 구매 영향 스트립은 Right Context가 아니라 Actions 주변에 작게 표시된다.
- [ ] 구매 영향 스트립은 현금 변화, 분기 운용 수익 증가, 운용가치 증가만 보여준다.
- [ ] 예비 시장 카드 상세보기에서는 Buy, 예약, Payment Pot이 숨겨진다.
- [ ] 예비 시장 카드 상세보기에서는 큰 카드 미리보기와 Close, Market Peek만 제공된다.
- [ ] Market Peek을 누르는 동안 시장 테이프가 보이고, 손을 떼면 카드 상세보기가 복귀한다.
- [ ] Market Peek 중에는 시장 카드 클릭이나 다른 행동 확정이 일어나지 않는다.

## Blocked by

- .scratch/mvp/issues/16-market-asset-card-icon-language.md
- .scratch/mvp/issues/17-payment-pot-background-and-cost-slot-visuals.md

## User stories covered

7, 8, 9, 10, 32, 33, 34, 37

