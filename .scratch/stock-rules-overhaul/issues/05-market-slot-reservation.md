# 05. 시장 슬롯 예약

Status: done

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

예약 구역을 제거하고, 예약을 시장 슬롯의 주식 잠금 상태로 전환한다. 플레이어가 주식을 예약하면 해당 시장 슬롯의 주식은 진행, 당김, 갱신의 영향을 받지 않는다. 예약 액션은 딜을 지급하고 월세 밀림을 증가시키며, 파산 여부를 즉시 확인한 뒤 영업일을 소비한다.

소모형 자원 카드는 예약할 수 없다. 예약된 주식을 구매하면 예약이 해제되고 해당 슬롯은 비며, 시장 테이프 당김으로 메운다.

## Existing implementation conflicts

- `ReservationState`와 `ReservationView`는 별도 예약 구역 3칸을 가진다.
- `ReservationAction`은 시장 카드를 예약 구역으로 이동시키고, 예약한 세로줄만 시장 테이프 진행처럼 처리한다.
- `PurchasePayment`는 예약 카드 구매를 `Reserved` source로 별도 처리하고 시장 테이프에는 영향을 주지 않는다.
- `RedemptionPressure`가 예약 패널티를 담당한다.

## Refactor approach

- 예약 상태를 별도 컬렉션이 아니라 시장 슬롯의 잠금 상태로 이동한다.
- 예약 개수는 시장 슬롯에서 계산하고, 예약 UI는 별도 보관소가 아니라 시장 카드 자체의 상태로 표시한다.
- 예약 구매는 "시장 슬롯에서 구매"로 취급하되, 구매 후 해당 슬롯을 비우고 시장 테이프 당김을 적용한다.
- 예약 패널티는 월세 밀림으로 연결하되, 실제 월세 밀림 용어 전환은 별도 이슈와 맞춘다.

## Acceptance criteria

- [x] 예약은 별도 예약 구역이 아니라 시장 슬롯 상태로 관리된다.
- [x] 주식 카드만 예약할 수 있고 소모형 자원 카드는 예약할 수 없다.
- [x] 동시에 최대 3개의 주식을 예약할 수 있다.
- [x] 예약된 주식은 시장 테이프 진행, 갱신, 당김 중에도 같은 슬롯에 남는다.
- [x] 예약 액션은 딜 +1을 시도하고, 월세 밀림 +1을 적용하고, 파산 여부를 확인한다. 구현은 아직 기존 `RedemptionPressure` 명칭을 사용하며, 월세 밀림 용어 전환은 별도 이슈와 맞춘다.
- [x] 딜이 이미 최대치여도 예약은 성공하고 초과 딜만 버려진다.
- [x] 예약 액션은 영업일을 소비하지만 즉시 시장 테이프를 진행하지 않는다.
- [x] 예약된 주식을 구매하면 예약 상태가 해제되고, 시장 슬롯 빈칸은 시장 테이프 당김으로 메워진다.

## Implementation notes

- `ReservationAction` no longer appends newly reserved cards to `ReservationState.ReservedCards`; reservation count/capacity checks are based on `MarketTapeSlotState.IsReserved`.
- Reserving a stock replaces the visible market slot card with a reserved runtime card in the same slot, grants Deal through `ResourceLedger`, increases pressure through `RedemptionPressure`, and consumes a business day through a reservation-specific no-market-advance path.
- `CardDetailState` hides Reserve for reserved cards, consumable resource cards, preview cards, and extra-buy purchases.
- `MarketAreaFlow.OpenReservedCardDetail` now opens reserved cards as `PurchaseSource.MarketTape`; reserved purchases are treated as market-slot purchases.
- `PurchasePayment` finds the purchased card by market slot and uses the market tape pull path after the slot is emptied.
- `ReservationView` is hidden because reservation is displayed on the market card itself instead of in a separate holding area.
- `docs/agents/class-inventory.md` was updated for stock overhaul issue 05.

## Verification notes

- TDD RED/GREEN covered reservation slot locking, stock-only reservation, 3-reservation capacity, no immediate tape advance on reservation, Deal overflow success, pressure/failure check, and reserved-slot purchase pull behavior.
- `dotnet build "Asset Manager/AssetManager.Tests.EditMode.csproj" --no-restore` passed after the code/test edits.
- `dotnet build "Asset Manager/AssetManager.Tests.PlayMode.csproj" --no-restore` passed after the UI test edits.
- Unity EditMode passed: 87 total, 87 passed, 0 failed (`editmode-20260517-211106-results.xml`).
- Unity PlayMode passed: 33 total, 33 passed, 0 failed (`playmode-20260517-211031-results.xml`).
- Unity manual scene verification was not run.

## Follow-up

- After issue `11-rent-arrears-and-bankruptcy.md`, rename the pressure-facing reservation penalty language from `RedemptionPressure` to 월세 밀림 where appropriate.

## Blocked by

- `01-stock-data-and-investment-philosophy-resources.md`
- `04-market-tape-1x8-progress-refresh-pull.md`
