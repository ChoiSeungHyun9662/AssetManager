# 04. 1x8 시장 테이프 진행, 갱신, 당김

Status: ready-for-agent

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

기존 3구역 시장을 1x8 시장 테이프로 전환한다. 시장 카드는 오른쪽에서 등장해 왼쪽으로 이동하고, 가장 왼쪽 방향으로 사라진다. 새 영업일 시작 시 시장 테이프가 진행되고, 새 분기 시작 시 시장 테이프가 갱신된다. 분기 첫 영업일에는 갱신만 하고 진행은 하지 않는다.

시장에 빈칸이 생기면 모든 경우를 시장 테이프 당김으로 처리한다. 당김은 빈칸 오른쪽 영역의 카드만 왼쪽으로 압축하고, 새 카드는 가장 오른쪽의 비예약 빈칸에 공급한다.

## Existing implementation conflicts

- `MarketTapeState`는 매도 임박/현재 시장/예비 시장 3개 리스트를 가진다.
- `MarketTapeZone`과 `MarketTape.Advance`, `Refresh`, `RefillSlot`, `AdvanceSlotAt`는 3구역 세로줄 이동을 전제로 한다.
- `MarketTapeView`와 `ProjectShell`는 3개 시장 패널과 예비 시장 미리보기 크기를 만든다.
- `BusinessDayFlow`, `PurchasePayment`, `ReservationAction`은 구매/예약 후 특정 column advance를 호출한다.

## Refactor approach

- 시장 상태를 8개 순서 슬롯 리스트로 단순화하고, 슬롯별 카드와 예약 여부를 함께 다룰 수 있게 준비한다.
- `Advance`, `Refresh`, `PullFromEmptySlot`을 1x8 기준 순수 규칙으로 다시 정의한다.
- 구매/예약/호일 제거 후에는 column advance가 아니라 빈칸 기준 당김을 호출하게 한다.
- UI는 3구역 패널 대신 8칸 테이프를 렌더링하도록 바꾸고, 기존 3구역 테스트는 폐기하거나 새 1x8 테스트로 교체한다.

## Acceptance criteria

- [ ] 시장은 1x8 슬롯으로 표시되고 왼쪽 1번 슬롯이 이탈 방향, 오른쪽 8번 슬롯이 진입 방향이다.
- [ ] 일반 영업일 시작 시 가장 왼쪽의 비예약 카드가 사라지고 나머지 비예약 카드가 왼쪽으로 진행한다.
- [ ] 새 분기 시작 시 시장 테이프는 갱신되고, 그 분기 첫 영업일에는 추가 진행하지 않는다.
- [ ] 갱신은 예약되지 않은 시장 슬롯만 새 카드로 교체한다.
- [ ] 시장 카드 구매, 소모형 자원 카드 구매, 예약 주식 구매, 호일 제거 등으로 생긴 빈칸은 모두 시장 테이프 당김으로 처리한다.
- [ ] 복수 빈칸이 생긴 경우 가장 왼쪽 빈칸부터 순서대로 당김을 적용한다.
- [ ] 당김/진행/갱신 시 새 카드 공급은 분리 덱 공급 규칙을 사용한다.

## Blocked by

- `03-split-market-decks-and-draw-rules.md`
