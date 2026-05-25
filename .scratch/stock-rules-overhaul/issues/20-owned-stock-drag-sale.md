# 20. 보유 주식 드래그 매도

Status: ready-for-agent

## Parent

- `.scratch/stock-rules-overhaul/PRD_20260520_feedback_overhaul.md`

## What to build

보유 주식 매도를 기존 호버 Sell Button 방식에서 드래그 앤 드롭 방식으로 바꾼다. 새 플레이 경로에서 보유 주식 호버는 매도 버튼을 노출하지 않는다. 매도 실행 영역은 항상 보이는 붉은 배경의 `$` 영역이다.

플레이어가 보유 주식 카드를 드래그하면 원래 포트폴리오 카드는 잠시 숨기고, 시장 호버 패널과 같은 카드 상세 패널을 포인터에 붙여 따라다니게 한다. 이 상세 패널은 보유 카드 정보만 보여주고 매수 비용은 표시하지 않는다. 드래그 중 매도 실행 영역은 해당 주식 매도 시 획득하는 현금 `+N`을 보여준다. 드롭 판정은 포인터 위치 기준이다. 포인터가 매도 실행 영역 안이면 원래 포트폴리오 `StockSlots` 인덱스의 주식을 매도하고, 밖이면 취소해서 원래 카드를 다시 표시한다.

## Acceptance criteria

- [x] 새 플레이 경로에서 보유 주식 호버는 Sell Button을 노출하지 않는다.
- [x] 매도 실행 영역은 항상 보이며 붉은 배경과 `$` 표시를 가진다.
- [x] 보유 주식 드래그 시작 시 원래 포트폴리오 카드는 잠시 숨겨진다.
- [x] 보유 주식 드래그 중 카드 상세 패널이 포인터를 따라다닌다.
- [x] 드래그 상세 패널의 좌측 하단 모서리는 포인터 위치에 맞춰진다.
- [x] 드래그 상세 패널은 시장 호버 패널과 같은 카드 상세 표시를 재사용하되 보유 카드의 매수 비용은 표시하지 않는다.
- [x] 보유 주식 드래그 중 매도 실행 영역은 매도 시 획득 현금 `+N`을 표시한다.
- [x] 포인터가 매도 실행 영역 안에서 드롭되면 해당 주식이 매도된다.
- [x] 매도는 드래그한 보유 주식의 원래 `StockSlots` 인덱스를 기준으로 처리한다.
- [x] 포인터가 매도 실행 영역 밖에서 드롭되면 매도는 취소되고 상세 패널은 닫히며 원래 포트폴리오 카드가 다시 표시된다.
- [x] 관련 EditMode 테스트가 기존 주식 매도 규칙이 드래그 원본 슬롯 인덱스 기준으로 호출될 수 있음을 검증한다.
- [x] 관련 PlayMode 테스트가 호버 Sell Button 미노출, `$` 매도 영역, 드래그 패널, `+N` 표시, 영역 안 드롭 매도, 영역 밖 드롭 취소를 검증한다.

## Blocked by

- `.scratch/stock-rules-overhaul/issues/16-purchase-confirmation-modal.md`

## Implementation notes

- Replaced the issue 08 hover-revealed owned stock Sell Button path with drag/drop sale. The child Sell Button objects remain in the shell for scene compatibility, but the new play path keeps them inactive and non-interactable.
- Added a persistent red portfolio sale drop zone. It shows `$` normally and changes to `+N` while an owned stock is dragged.
- Owned stock drag hides the source card visually through a `CanvasGroup`, shows a pointer-following detail panel with its bottom-left corner at the pointer, and formats owned-card value/dividend without purchase costs.
- Drop handling uses the pointer position against the sale drop zone. Inside sells through `StockSaleAction.ConfirmSale` using the dragged card's original `StockSlots` index; outside cancels and restores the original portfolio card.

## TDD log

- RED: added `SellingOwnedStockUsesOriginalPortfolioSlotIndex`; EditMode initially passed because issue 08 already had the needed slot-index rule surface.
- RED: updated PlayMode expectations for hover Sell Button removal, persistent `$` drop zone, owned-card drag detail, `+N`, inside-drop sale, and outside-drop cancel; Unity compile failed on missing shell constants.
- GREEN: added shell sale-zone/detail-panel objects and wired `PortfolioSummaryView` drag/drop behavior through the existing stock sale rule.

## Verification

- EditMode: `AssetManager.Tests.EditMode` passed, 120/120.
- PlayMode: `AssetManager.Tests.PlayMode` passed, 60/60.
- Unity manual check: not run; behavior was verified through PlayMode pointer event tests.
- Remaining risk: visual polish of the new sale zone and drag detail panel still needs human QA in the real scene.
