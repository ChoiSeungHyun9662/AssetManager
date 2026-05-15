# 17. Payment Pot 배경과 비용 슬롯 결제 시각화

Status: ready-for-agent

## Parent

.scratch/mvp/UI_GAMEPLAY_SPEC.md

## What to build

카드 상세보기의 결제 영역을 전문 자원 비용을 지불하는 물리적 Payment Pot으로 보이게 만든다. Payment Pot에는 현금 슬롯을 만들지 않고, 자산 카드가 요구하는 전문 자원 비용 슬롯만 표시한다. 리서치, 신용, 원자재 칩은 맞는 비용 슬롯에 놓을 수 있고, 딜은 어떤 전문 자원 비용 슬롯도 대체할 수 있어야 한다. 딜이 놓이면 Buy 버튼의 최종 현금 비용 감소가 즉시 읽혀야 한다.

우선 적용 대상 애셋:

- PaymentPot_Background_Default
- Icon_Research
- Icon_Credit
- Icon_Commodity
- Icon_Deal
- Chip_Research_Normal
- Chip_Credit_Normal
- Chip_Commodity_Normal
- Chip_Deal_Normal

애셋이 아직 없으면 기존 placeholder를 유지하되, 누락된 애셋 이름을 확인할 수 있는 경고를 남긴다.

## Acceptance criteria

- [ ] 매수 가능한 카드 상세보기에서 Payment Pot 배경이 결제 테이블처럼 보인다.
- [ ] Payment Pot에는 전문 자원 비용 슬롯만 표시되고 현금 슬롯은 표시되지 않는다.
- [ ] 빈 비용 슬롯은 요구 전문 자원을 흐릿한 아이콘 또는 placeholder로 보여준다.
- [ ] 맞는 전문 자원 칩은 호환 비용 슬롯에 배치할 수 있다.
- [ ] 딜은 어떤 전문 자원 비용 슬롯에도 배치할 수 있다.
- [ ] 호환되지 않는 칩 배치는 거부되거나 원래 위치로 돌아간다.
- [ ] 배치된 칩은 다시 회수할 수 있고, 회수 후 Bottom Chip Tray에 돌아온 것처럼 보인다.
- [ ] Payment Pot에 놓인 딜 1개마다 Buy 버튼의 최종 현금 비용이 1 감소해 표시된다.
- [ ] 모든 비용 슬롯이 채워지고 최종 현금 비용을 낼 수 있을 때만 Buy가 활성 상태로 보인다.

## Blocked by

- .scratch/mvp/issues/15-resource-object-assets-and-gain-liquidity-ui.md

## User stories covered

9, 26, 27, 28, 29, 30, 31, 59

