# 08. 주식 매도와 수익 추적 전환

Status: done

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

포트폴리오의 보유 주식을 매도할 수 있게 하고, 매도 수익을 분기 목표 판정에 쓰이는 수익에 포함한다. 주식 매도는 영업일을 소비하지 않으며 하루에 여러 번 가능하다. 일반 주식은 현금 1 x 인플레이션, 호일 주식은 현금 3 x 인플레이션을 지급한다.

기존 운용 수익 추적은 새 수익 개념으로 전환한다. 수익에는 배당금, 주식 매도 수익, 분기 마감 정산 수익이 포함되고, 소모형 자원 카드로 얻은 현금은 포함되지 않는다.

## Existing implementation conflicts

- 현재 보유 자산은 매수와 영업일 시작 수익 중심이며, 매도 액션이 없다.
- `OwnedAssetState`는 보유 목록에서 카드를 제거하고 빈 포트폴리오 슬롯을 유지하는 흐름을 갖지 않는다.
- `RunPerformanceState`와 `ResourceLedger`는 운용 수익/조달 현금 분리를 갖지만, 주식 매도 수익은 별도 경로가 없다.
- `BusinessDayFlow`는 주요 액션이 영업일을 소비한다는 전제에 가깝다.

## Refactor approach

- 포트폴리오 슬롯에서 주식을 제거하는 매도 규칙을 추가하고, 이 액션은 영업일을 소비하지 않게 한다.
- 매도 보상은 인플레이션을 적용한 현금 지급과 동시에 수익 카운터에 반영한다.
- 매도된 주식은 주식 덱 복귀가 아니라 제거 상태로 기록해 공급 규칙과 연결한다.
- 기존 수익 추적 명칭은 배당금/수익으로 정리하고, 소모형 자원 카드 현금은 funding cash 경로로 유지한다.

## Acceptance criteria

- [x] 보유 일반 주식을 매도하면 현금 1 x 인플레이션을 얻는다.
- [x] 보유 호일 주식을 매도하면 현금 3 x 인플레이션을 얻는다.
- [x] 주식 매도는 영업일을 소비하지 않고 하루에 여러 번 수행할 수 있다.
- [x] 매도한 주식은 포트폴리오에서 제거되고 이번 게임의 주식 덱에 돌아오지 않는다.
- [x] 매도 수익은 현재 분기 수익, 현재 회계연도 수익, 총 수익에 포함된다.
- [x] 배당금은 영업일 시작 시 보유 주식에서 발생하고 수익에 포함된다.
- [x] 소모형 자원 카드로 얻은 현금은 수익에 포함되지 않는다.
- [x] 분기 목표 판정은 현재 현금이 아니라 현재 분기 수익을 기준으로 한다.
- [x] 보유 주식 Card Button 호버 시 해당 Sell Button이 표시된다.
- [x] Card Button과 Sell Button 사이로 커서를 이동하는 동안 Sell Button이 유지된다.
- [x] Card Button과 Sell Button을 모두 벗어나면 Sell Button이 숨겨진다.
- [x] Card Button 클릭은 Sell Button 표시 조건이 아니다.

## Implementation notes

- Added `StockSaleAction` as the public rule service for portfolio slot sales.
- Selling a stock leaves the portfolio slot empty, marks that runtime card `Removed`, pays inflation-adjusted sale cash through the revenue path, and does not consume a business day.
- Owned stock cards now use a hover sale affordance: hovering a card's Card Button reveals that card's child Sell Button, moving between the Card Button and Sell Button keeps it visible, leaving both hides it, and clicking the Sell Button confirms the sale.
- The visible portfolio row is compressed from `OwnedAssetState.StockSlots` by skipping empty slots; each displayed card keeps its original stock slot index so sale removes the correct internal slot.
- Any explicit non-sale UI interaction clears the pending sale button. Empty slots are not rendered, so they have no click target.
- Visible revenue labels now use 수익/배당금 language while keeping existing internal `EarnedCash` compatibility names.

## Verification

- RED: `StockSaleActionTests` first failed because `StockSaleAction` did not exist.
- GREEN: `StockSaleActionTests` passed after adding normal/foil sale behavior.
- RED: `MainGameShellBootstrapCardDetailPaymentPlacesRecoversAndConfirmsMarketPurchase` failed until card click no longer sold immediately and instead used a child sell button.
- GREEN: PlayMode passed after wiring owned stock cards to reveal per-card sell buttons on hover, keep them visible across Card Button/Sell Button pointer transitions, hide them after leaving both targets, confirm sale through the child button, clear pending sale on non-sale interactions, and preserve correct `StockSlots` sale indices after compressed display.

## Post-completion correction

2026-05-19 UX decision supersedes the previous click-to-toggle sale affordance:

- `Owned Stock Card 1~8 Card Button` hover reveals `Owned Stock Card 1~8 Sell Button`.
- Moving the pointer from Card Button to Sell Button, or from Sell Button back to Card Button, keeps the Sell Button visible.
- Leaving both Card Button and Sell Button hides the Sell Button.
- Card Button click no longer reveals or toggles the Sell Button.

## Blocked by

- `06-stock-purchase-payment-and-portfolio-cap.md`
- `07-foil-merge-and-stock-removal.md`
