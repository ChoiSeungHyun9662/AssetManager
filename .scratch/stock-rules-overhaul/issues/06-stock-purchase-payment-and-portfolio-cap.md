# 06. 주식 매수 결제와 8칸 포트폴리오 제한

Status: completed

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

기존 시장 카드 매수와 칩 결제 기반을 새 주식 매수 규칙에 맞춘다. 주식 매수는 현금과 투자 철학 비용을 검증하고, 딜은 주식 매수에서만 투자 철학 비용 슬롯 대체와 현금 할인에 사용할 수 있다. 포트폴리오는 최대 8칸이며, 새 주식 구매가 8칸을 초과하면 비용과 영업일을 소비하지 않고 막는다.

이 이슈는 아직 호일 합성의 전체 제거 규칙을 완성하지 않아도 된다. 다만 포트폴리오가 가득 찬 상태에서 세 번째 동일 주식 구매가 호일로 이어질 수 있도록 막지 않는 길은 준비해야 한다.

## Existing implementation conflicts

- `PurchasePayment`는 카드 상세보기의 비용 슬롯 상태를 기준으로 자산 매수를 확정한다.
- `OwnedAssetState`는 보유 자산 목록과 요약 중심이며, 8칸 슬롯 제한과 빈칸 개념이 없다.
- 구매 성공 시 `MarketTape.AdvanceSlotAt`으로 구매 column을 보충한다.
- 소모형 자원 카드 구매와 주식 구매가 아직 같은 시장 구매 표면에서 분리되어 있지 않다.

## Refactor approach

- 구매 검증을 카드 상세보기 의존 상태에서 분리해, 시장 카드 타입과 결제 선택을 입력으로 받는 규칙으로 정리한다.
- 포트폴리오를 8칸 슬롯 구조로 만들고, 구매 전 수용 가능 여부를 비용 소비보다 먼저 검증한다.
- 딜 할인과 인플레이션 적용 순서는 기존 구현을 유지하되, 주식 카드에만 허용한다.
- 구매 후 시장 보충은 column advance가 아니라 시장 테이프 당김으로 연결한다.

## Acceptance criteria

- [ ] 주식 매수는 현금 비용과 독서/명상/인내 비용을 검증한다.
- [ ] 딜은 주식 매수에서만 사용할 수 있으며, 소모형 자원 카드 매수에는 사용할 수 없다.
- [ ] 딜은 투자 철학 비용 슬롯 하나를 대체하고 기본 현금 비용을 1 낮춘다.
- [ ] 딜 할인 뒤 인플레이션을 적용한 최종 현금 비용으로 구매 가능 여부를 판단한다.
- [ ] 포트폴리오는 최대 8칸의 주식 슬롯을 가진다.
- [ ] 8칸이 모두 찬 상태에서 호일로 이어지지 않는 신규 주식 구매는 막고 "주식 매도가 필요합니다" 알림을 낸다.
- [ ] 막힌 구매는 비용을 소비하지 않고 영업일도 소비하지 않는다.
- [ ] 구매로 비워진 시장 슬롯은 시장 테이프 당김으로 메워진다.

## Blocked by

- `01-stock-data-and-investment-philosophy-resources.md`
- `04-market-tape-1x8-progress-refresh-pull.md`

## TDD completion report

### Assumptions and non-goals

- Portfolio capacity means 8 owned stock slots; consumable resource cards are paid for and consumed without entering the portfolio.
- A third purchase of an already-owned stock must not be blocked by the 8-slot cap, because a later foil-combination issue will consume that path.
- Selling stocks and full foil-combination resolution are out of scope for this issue.
- Existing MVP tests that assumed the old 3-zone market tape were treated as stale when they conflicted with the stock-overhaul PRD.

### Behaviors protected

- Full portfolio blocks a new stock purchase before chips, cash, or business days are consumed.
- Full portfolio does not block consumable resource card purchase.
- Owned assets expose 8-slot capacity and full/open-slot state through the public `OwnedAssetState` surface.
- Market and reserved purchases replenish the 1x8 market tape by pulling from the purchased slot, not by column-advance semantics.
- Long schedule refresh preserves reserved slots while replacing non-reserved visible cards so consumable resource cards can recycle.
- PlayMode market and purchase flows use the active scene UI root and current 1x8 market-row behavior.

### RED/GREEN/REFACTOR log

- RED: added `OwnedAssetState` behavior for 8 stock slots and full state; initial compile/test signal exposed missing capacity API. GREEN: added `MaxStockSlots`, `OpenStockSlots`, `IsPortfolioFull`, `CanAcceptStockPurchase`, and third-copy foil-path allowance.
- RED: added purchase validation behavior for full portfolio blocking new stock buys before spending. GREEN: `PurchasePayment` checks portfolio capacity before payment slot, cash, and business-day validation.
- RED: added consumable resource purchase behavior under full portfolio. GREEN: capacity validation applies only to stock cards.
- RED: final EditMode revealed long schedule refresh could exhaust the market deck. GREEN: `MarketTape.Refresh(run)` now removes non-reserved visible cards before refresh, preserves reserved slots, and allows removed consumables to recycle.
- RED: PlayMode exposed stale UI-root lookup and old 3-zone assumptions. GREEN: PlayMode helpers now isolate/find the active scene UI root and assertions follow the 1x8 market-row behavior.
- REFACTOR: kept `AdvanceSlotAt` as a compatibility wrapper and routed new purchase behavior through `PullFromSlotAt` to make the public purchase intent explicit without a broad rename.

### Automated tests

- Passed: EditMode `AssetManager.Tests.EditMode`
  - Result: `.scratch/test-results/editmode-20260517-211106-results.xml`
- Passed: PlayMode `AssetManager.Tests.PlayMode`
  - Result: `.scratch/test-results/playmode-20260517-211031-results.xml`

### Unity manual checks

- Not run manually. UI-visible behavior was covered by PlayMode tests for market row rendering, card detail purchase, reservation/failure flows, and smoke scenarios.

### Files touched

- `Asset Manager/Assets/_AssetManager/Scripts/Runtime/RunModels.cs`: portfolio-cap helpers on `OwnedAssetState`.
- `Asset Manager/Assets/_AssetManager/Scripts/Runtime/PurchasePayment.cs`: stock-cap validation and direct 1x8 slot-pull purchase replenishment.
- `Asset Manager/Assets/_AssetManager/Scripts/Runtime/MarketTape.cs`: `PullFromSlotAt` public API and schedule refresh preservation/recycling behavior.
- `Asset Manager/Assets/_AssetManager/Tests/EditMode/OwnedAssetStateTests.cs`: 8-slot capacity behavior.
- `Asset Manager/Assets/_AssetManager/Tests/EditMode/PurchasePaymentTests.cs`: full-portfolio stock/consumable purchase behavior and 1x8 purchase replenishment expectations.
- `Asset Manager/Assets/_AssetManager/Tests/EditMode/BusinessDayFlowTests.cs`: schedule refresh expectation updated to stock-only non-reuse while consumables may recycle.
- `Asset Manager/Assets/_AssetManager/Tests/PlayMode/MainGameShellBootstrapTests.cs`: active-scene UI isolation and 1x8 market-row assertions.
- `Asset Manager/Assets/_AssetManager/Tests/PlayMode/MvpSmokeScenarioTests.cs`: active-scene UI isolation for smoke flows.
- `docs/agents/class-inventory.md`: class/responsibility inventory updated for issue 06.
- `.scratch/stock-rules-overhaul/issues/06-stock-purchase-payment-and-portfolio-cap.md`: this completion report.

### Remaining risk

- Issue 05 was being developed concurrently, and the working tree contains unrelated issue-05 and scene changes. Those were not reverted.
- Foil-combination behavior is intentionally deferred; this issue only leaves the third-copy purchase path open.
- Manual Unity scene inspection was not run, so visual polish outside PlayMode assertions remains unverified.
