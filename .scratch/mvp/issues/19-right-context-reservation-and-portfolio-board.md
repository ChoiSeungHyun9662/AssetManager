# 19. Right Context 예약 구역과 포트폴리오 보드

Status: ready-for-agent

## Parent

.scratch/mvp/UI_GAMEPLAY_SPEC.md

## What to build

Right Context를 선택 카드 상세 패널이 아니라 항상 보이는 플레이어 보드 상태판으로 정리한다. 상단에는 예약 구역 3칸이 있고, 예약 카드가 있으면 미니 카드로 보인다. 하단에는 포트폴리오 요약이 있어 보유 자산 수, 현재 운용가치, 분기 운용 수익이 먼저 읽힌다. 구매 영향 미리보기는 Right Context에 넣지 않는다.

## Acceptance criteria

- [ ] Right Context는 시장, 카드 상세보기, 자원 확보, 분기 마감 흐름에서 기본적으로 유지된다.
- [ ] 예약 구역은 3칸으로 보이며, 빈 자리는 빈 카드 슬롯처럼 읽힌다.
- [ ] 예약 카드가 있으면 작지만 카드 실물처럼 보이는 미니 카드로 표시된다.
- [ ] 예약 카드 미니 카드는 기존 예약 카드 상세보기 진입 흐름을 유지한다.
- [ ] 포트폴리오 요약은 보유 자산 수, 현재 운용가치, 분기 운용 수익을 표시한다.
- [ ] 보유 자산 목록을 표시하더라도 요약 숫자보다 강하게 보이지 않는다.
- [ ] 선택 중인 카드의 구매 영향 미리보기는 Right Context에 표시되지 않는다.
- [ ] Right Context는 카드 상세보기 진입/종료 때문에 내용 구조가 흔들리지 않는다.

## Blocked by

- .scratch/mvp/issues/15-resource-object-assets-and-gain-liquidity-ui.md
- .scratch/mvp/issues/16-market-asset-card-icon-language.md

## User stories covered

20, 23, 24, 25, 32, 38, 39

