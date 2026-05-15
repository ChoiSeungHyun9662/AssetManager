# 20. 공통 프레임 안의 분기 마감 결과판

Status: ready-for-agent

## Parent

.scratch/mvp/UI_GAMEPLAY_SPEC.md

## What to build

분기 마감 결과를 별도 화면처럼 끊지 않고 공통 UI 프레임 안에서 보여준다. Top Status Bar, Right Context, Bottom Chip Tray는 유지하고, Center Main Board만 분기 결과판으로 바꾼다. 결과판은 분기 운용 수익과 분기 목표 운용 수익의 비교, 목표 달성률, 환매 압력 변화, Continue 액션을 중심으로 읽혀야 한다.

Bottom Chip Tray는 결과 확인 중 조작 대상이 아니므로 낮은 강조 또는 조작 불가 상태로 보인다.

## Acceptance criteria

- [ ] 분기 마감 결과 상태에서도 Top Status Bar가 유지된다.
- [ ] 분기 마감 결과 상태에서도 Right Context가 유지된다.
- [ ] 분기 마감 결과 상태에서도 Bottom Chip Tray가 유지되지만 조작 가능 대상으로 강조되지 않는다.
- [ ] Center Main Board는 분기 결과판으로 전환된다.
- [ ] 분기 운용 수익과 분기 목표 운용 수익 비교가 결과판의 핵심 정보로 보인다.
- [ ] 목표 달성률 또는 목표 게이지 결과가 한눈에 읽힌다.
- [ ] 목표 미달 시 환매 압력 증가가 명확히 표시된다.
- [ ] Continue는 결과판의 주 액션으로 보이고 기존 진행 흐름을 유지한다.

## Blocked by

- .scratch/mvp/issues/15-resource-object-assets-and-gain-liquidity-ui.md
- .scratch/mvp/issues/19-right-context-reservation-and-portfolio-board.md

## User stories covered

49, 50, 51, 52, 53

