# 11A. 인플레이션 비용 수정과 매수 표시

Status: done

## Parent

.scratch/mvp/PRD.md

## What to build

현재 분기의 인플레이션 값을 자산 매수 현금 비용에 반영한다. 시장 카드 매수와 예약 카드 매수는 같은 매수 결제 규칙을 사용하며, 딜 할인 뒤에 인플레이션을 적용한 최종 현금 비용을 카드 상세보기와 매수 가능 판정에 사용해야 한다. 인플레이션은 MVP에서는 시장 뉴스나 이벤트 시스템이 아니라 테이블로 조정 가능한 정수형 비용 수정값이다.

## Code work

- 인플레이션 기본값과 분기별 값을 정적 데이터에서 제공할 수 있게 한다.
- 현재 회계년도/분기에 해당하는 인플레이션 값을 런 상태 또는 결제 계산 경로에서 조회할 수 있게 한다.
- PurchasePayment의 최종 현금 비용 계산에 인플레이션을 반영한다.
- 계산 순서는 기본 현금 비용, 비용 수정 효과, 딜 할인, 인플레이션 적용, 최종 0 클램프로 고정한다.
- 인플레이션은 현금 비용에만 적용하고 전문 자원 비용 슬롯에는 적용하지 않는다.
- 예약 카드는 예약 시점이 아니라 실제 매수 시점의 인플레이션을 적용한다.
- 인플레이션 기본값 0에서는 기존 매수 테스트와 플레이 흐름이 그대로 유지되게 한다.
- 매수 비용 표시와 매수 가능 판정이 같은 최종 현금 비용을 사용하게 한다.

## Unity Editor work

- 카드 상세보기에서 인플레이션이 반영된 최종 현금 비용을 표시한다.
- 비용 표시가 딜 배치/회수 후 즉시 갱신되게 한다.
- 필요하면 인플레이션 값이 0이 아닐 때만 짧은 비용 breakdown을 표시한다.
- QA 또는 테스트용 데이터에서 인플레이션 +1 이상인 분기를 확인할 수 있게 한다.

## Verification

- EditMode에서 인플레이션 0일 때 기존 최종 현금 비용이 변하지 않는지 확인한다.
- EditMode에서 기본 현금 비용 5, 딜 2개, 인플레이션 +1이면 최종 현금 비용이 4인지 확인한다.
- EditMode에서 인플레이션 때문에 현금이 부족하면 매수 확정이 실패하고 자원, 카드, 시장/예약 상태가 변하지 않는지 확인한다.
- EditMode에서 예약 카드 매수도 현재 인플레이션을 적용하는지 확인한다.
- Play Mode에서 카드 상세보기의 최종 현금 비용 표시가 딜 배치와 인플레이션을 반영하는지 확인한다.

## Acceptance criteria

- [x] 인플레이션 기본값 0에서는 기존 매수 비용과 테스트가 유지된다.
- [x] 인플레이션은 현금 비용에만 적용되고 전문 자원 비용에는 적용되지 않는다.
- [x] 딜 할인 후 인플레이션이 적용된다.
- [x] 인플레이션이 반영된 최종 현금 비용으로 현금 부족 여부를 판정한다.
- [x] 시장 카드 매수와 예약 카드 매수 모두 인플레이션을 적용한다.
- [x] 예약 카드는 예약 시점이 아니라 실제 매수 시점의 인플레이션을 적용한다.
- [x] 카드 상세보기의 최종 현금 비용 표시가 인플레이션과 딜 배치 상태를 반영한다.
- [x] MVP QA smoke scenario에 인플레이션 매수 경로가 추가된다.

## Blocked by

- .scratch/mvp/issues/06-market-card-purchase-and-minimal-chip-payment.md
- .scratch/mvp/issues/10-reserved-card-persistence-and-purchase.md

## User stories covered

30, 38, 39, 59, 60

## Comments

- 2026-05-13: Added after MVP scope was updated to include inflation. This slice can be implemented after the current issue 11 work lands to avoid overlapping changes around run state and settlement flow, but it does not depend on quarter settlement behavior.
- 2026-05-13: Implemented table-driven quarter inflation. 1회계년도 2Q has +1 in MVP defaults for QA coverage; EditMode and PlayMode tests cover final cost, cash shortage, reserved purchase timing, and card detail display updates.
