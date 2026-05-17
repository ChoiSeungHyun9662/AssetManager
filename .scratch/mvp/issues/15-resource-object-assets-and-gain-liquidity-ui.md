# 15. 자원 오브젝트 애셋과 자원 확보 UI

Status: done

## Parent

.scratch/mvp/UI_GAMEPLAY_SPEC.md

## What to build

플레이어가 조작하는 자원이 보드게임 테이블 위 물리 오브젝트처럼 읽히도록 Bottom Chip Tray와 자원 확보 화면을 함께 개선한다. 현금은 칩이 아니라 지폐 다발로 보이고, 리서치, 신용, 원자재, 딜은 개별 칩으로 보인다. 자원 확보 상태에서도 자산 카드 프레임을 쓰지 않고 현금/전문 자원 오브젝트를 직접 선택하는 화면처럼 느껴져야 한다.

우선 적용 대상 애셋:

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

애셋이 아직 없으면 기존 placeholder를 유지하되, 누락된 애셋 이름을 확인할 수 있는 경고를 남긴다.

## Acceptance criteria

- [ ] Top Status Bar에는 현금, 리서치, 신용, 원자재, 딜이 표시되지 않는다.
- [ ] Bottom Chip Tray는 현금을 지폐 다발과 숫자로 표시하고, 현금은 드래그 가능한 칩처럼 보이지 않는다.
- [ ] Bottom Chip Tray는 리서치, 신용, 원자재, 딜을 보유량만큼 개별 칩으로 표시한다.
- [ ] 같은 자원 칩은 약간 겹쳐 배치되며, 많아져도 트레이 밖이나 다른 자원 영역을 침범하지 않는다.
- [ ] 자원 확보 화면은 현금, 리서치, 신용, 원자재를 자산 카드가 아닌 자원 오브젝트 선택지로 표시한다.
- [ ] 자원 확보 선택은 기존 규칙대로 즉시 확정되고 Bottom Chip Tray와 남은 선택 횟수에 반영된다.
- [ ] 전문 자원 한도에 걸린 자원은 선택 불가 또는 비활성 상태로 읽힌다.
- [ ] 딜은 자원 확보 선택지에 나타나지 않는다.
- [ ] 대상 애셋이 누락되어도 플레이 가능한 placeholder UI가 유지된다.

## Blocked by

None - can start immediately

## User stories covered

2, 11, 13, 26, 40, 41, 42, 43, 44
