# 10. 예약 카드 유지와 예약 카드 매수

Status: done

## Parent

.scratch/mvp/PRD.md

## What to build

예약 카드가 영업일, 분기, 회계년도 전환, 시장 테이프 진행과 갱신 이후에도 유지되게 하고, 예약 카드 상세보기에서 매수할 수 있게 한다. 예약 카드 매수는 일반 자산 매수로 판정하되 매수 출처는 Reserved로 기록하고, 시장 테이프에는 영향을 주지 않아야 한다.

## Code work

- 예약 카드 클릭을 CardDetail 진입과 연결한다.
- 예약 카드 상세보기에서는 예약 버튼을 숨긴다.
- 예약 카드 매수는 PurchasePayment의 일반 매수 흐름을 사용한다.
- 예약 카드 매수 시 매수 출처를 Reserved로 기록한다.
- 매수 확정 후 해당 예약 구역만 비운다.
- 예약 카드 매수 후 시장 테이프 진행이나 시장 슬롯 보충은 하지 않는다.
- 예약 카드는 매수 전까지 운용가치, 운용 수익, 분기 정산에 포함하지 않는다.
- 예약 구역은 영업일 종료, 분기 전환, 회계년도 전환, 시장 테이프 진행, 시장 테이프 갱신 이후에도 유지된다.

## Unity Editor work

- 예약 구역에 표시된 카드 요약 UI를 클릭 가능하게 연결한다.
- 예약 카드 상세보기에서는 매수 UI를 표시하고 예약 버튼을 숨긴다.
- 예약 카드 매수 후 예약 구역 UI가 빈 상태로 갱신되게 연결한다.
- 분기 전환, 휴가, 회계년도 전환 이후 예약 구역 UI가 유지되는지 확인할 수 있게 표시한다.

## Verification

- Play Mode에서 시장 카드를 예약하고 다음 영업일, 다음 분기, 회계년도 전환을 지나도 예약 구역에 카드가 남아 있다.
- 예약 카드를 클릭하면 카드 상세보기로 들어간다.
- 예약 카드 상세보기에는 예약 버튼이 없다.
- 예약 카드 매수 후 카드가 보유 자산으로 이동하고 예약 구역만 비워진다.
- 예약 카드 매수로 시장 테이프는 바뀌지 않는다.

## Acceptance criteria

- [x] 예약 카드는 분기와 회계년도 전환 후에도 유지된다.
- [x] 시장 테이프 진행과 갱신은 예약 구역에 영향을 주지 않는다.
- [x] 예약 카드 클릭은 CardDetail 상태로 진입한다.
- [x] 예약 카드 상세보기에서는 예약 버튼을 숨긴다.
- [x] 예약 카드 매수는 일반 자산 매수로 판정된다.
- [x] 예약 카드 매수의 PurchaseSource는 Reserved로 기록된다.
- [x] 예약 카드 매수 후 해당 예약 구역만 비워진다.
- [x] 예약 카드 매수는 시장 테이프를 진행시키지 않는다.

## Blocked by

- .scratch/mvp/issues/09-market-card-reservation-deal-and-redemption-pressure.md

## User stories covered

20, 23, 24, 25, 38, 39

## Comments

- 2026-05-12: Implemented with TDD.
  - EditMode: `editmode-20260512-233954-results.xml` passed 46/46.
  - PlayMode: `playmode-20260512-234026-results.xml` passed 15/15.
  - Manual Unity scene check not run separately; PlayMode covers reservation click, card detail, hidden reserve button, purchase, reservation UI refresh, and unchanged market tape.
