# 09. 시장 카드 예약, 딜, 환매 압력

Status: done

## Parent

.scratch/mvp/PRD.md

## What to build

시장 카드 상세보기에서 예약 버튼을 눌러 카드를 예약 구역으로 이동시키고, 딜 +1, 환매 압력 +1, 예약한 세로줄만 시장 테이프 진행처럼 처리, 영업일 종료를 처리한다. 예약은 확인 상태 없이 즉시 실행되며, 예약 구역이 가득 찼을 때는 실행되지 않고 영업일도 소비하지 않아야 한다.

## Code work

- Reservation module을 구현한다.
- 예약 구역 3개를 관리한다.
- 예약 가능 대상은 시장 카드로 제한한다.
- 예약 버튼 표시 조건은 시장 카드 상세보기, 추가 매수권 상태 아님, 선택 카드 유효함으로 둔다.
- 예약 버튼 활성 조건은 예약 구역 여유 있음으로 둔다.
- 딜이 3/3이어도 예약은 가능하게 한다.
- 예약 실행 시 비용 슬롯에 올린 칩이 있으면 자동 회수하고 결제 상태를 초기화한다.
- 선택한 시장 카드를 Reserved 상태로 바꾸고 첫 빈 예약 구역으로 이동한다.
- 예약한 세로줄만 시장 테이프 진행처럼 처리하고 전체 시장 테이프는 진행하지 않는다.
- 딜 +1을 시도하고 3개 초과 시 추가 딜만 폐기한다.
- 환매 압력 +1을 적용하고 즉시 한도 검사를 수행한다.
- 실패하지 않으면 CardDetail을 닫고 Market 상태로 돌아가 영업일을 종료한다.

## Unity Editor work

- 예약 구역 3개 UI를 만든다.
- CardDetail Panel의 예약 버튼을 시장 카드 상세보기에서만 보이게 연결한다.
- 예약 구역이 가득 차면 예약 버튼을 비활성화하고 예약 구역 가득 참 메시지를 표시한다.
- 딜 초과 폐기 메시지와 환매 압력 증가 표시를 메시지 영역 또는 HUD에 연결한다.
- 예약 후 예약 구역에 카드 요약 UI가 표시되게 연결한다.
- 예약으로 인한 해당 세로줄 시장 테이프 진행이 화면에 반영되게 연결한다.

## Verification

- Play Mode에서 시장 카드 상세보기의 예약 버튼을 누르면 예약 구역에 카드가 들어간다.
- 예약 시 딜이 1 증가하고 환매 압력이 1 증가한다.
- 딜 3/3 상태에서도 예약은 실행되고 추가 딜 폐기 메시지가 표시된다.
- 예약 구역 3/3 상태에서는 예약 버튼이 비활성화되고 클릭해도 영업일이 소비되지 않는다.
- 예약 후 시장 테이프는 예약한 세로줄만 진행된다.
- 환매 압력 9에서 예약하면 즉시 실패 화면으로 이동한다.

## Acceptance criteria

- [x] 예약은 시장 카드에만 가능하다.
- [x] 예약 카드 상세보기에서는 예약 버튼을 표시하지 않는다.
- [x] 예약 확인 상태 없이 버튼 클릭 즉시 예약을 실행한다.
- [x] 예약 구역은 최대 3개다.
- [x] 예약 구역이 가득 차면 예약은 실행되지 않고 영업일도 소비하지 않는다.
- [x] 딜 3/3이어도 예약 가능하며 추가 딜만 폐기된다.
- [x] 예약은 환매 압력 +1을 발생시키고 즉시 한도 검사를 한다.
- [x] 예약한 세로줄만 시장 테이프 진행처럼 처리하고 전체 시장 테이프는 진행하지 않는다.

## Blocked by

- .scratch/mvp/issues/06-market-card-purchase-and-minimal-chip-payment.md
- .scratch/mvp/issues/07-owned-assets-income-and-performance-tracking.md

## User stories covered

10, 17, 32, 33, 34, 35, 36, 37, 51, 52

## Comments

- 2026-05-12: Implemented with TDD. Added `ReservationAction` for reservation validation/confirmation, `ReservationView` for the 3-slot reservation area UI, CardDetail reserve-button wiring, and PlayMode coverage for reservation button flow, full reservation-area blocking, and 환매 압력 failure UI. Verification: RED compile failure for missing `ReservationAction`; final EditMode 43/43 passed; final PlayMode 14/14 passed.
