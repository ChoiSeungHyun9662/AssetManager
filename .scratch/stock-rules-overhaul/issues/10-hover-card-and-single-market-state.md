# 10. 카드 호버 확대와 단일 시장 상태

Status: done

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

카드 상세보기 화면과 GainLiquidity 상태를 제거하고, 시장 영역을 단일 상태로 유지한다. 시장 카드에 마우스를 올리면 같은 정보를 더 큰 스케일로 보여주는 호버 확대 카드를 표시한다. 시장에 있을 때와 호버 확대 시 제공 정보는 동일하고, 스케일만 달라야 한다.

주식 카드와 소모형 자원 카드는 서로 다른 표시 정보를 가진다. 기존 상세보기 액션 보드, 예비 시장 미리보기, Payment Pot 전제는 새 흐름에서 사용하지 않는다.

## Existing implementation conflicts

- `MarketAreaState`는 Market, CardDetail, GainLiquidity 상태를 가진다.
- `MarketAreaFlow`, `CardDetailState`, `CardDetailView`는 클릭으로 카드 상세보기 화면을 여는 흐름을 담당한다.
- `LiquidityActionView`는 중앙 은행/GainLiquidity 화면을 제공한다.
- `ProjectShell`, `MainGameShellBootstrap`, PlayMode 테스트는 CardDetail 패널, Payment Pot, 예비 시장 미리보기, GainLiquidity UI가 존재한다고 기대한다.

## Refactor approach

- 시장 영역은 단일 상태로 유지하고, 카드 검사 정보는 hover-only presentation으로 이동한다.
- 구매/예약 액션은 별도 상세보기 화면이 아니라 시장 카드 주변 또는 카드 자체의 액션 표면에서 발생하게 한다.
- CardDetail/GainLiquidity UI 생성과 이벤트 연결은 새 흐름에서 끊고, 필요한 경우 폐기 확인 테스트를 추가한다.
- 주식 카드와 소모형 자원 카드 표시 모델을 분리해 같은 hover 컴포넌트가 타입별 필드를 렌더링하게 한다.

## Acceptance criteria

- [x] 시장 영역은 Market/CardDetail/GainLiquidity 같은 복수 상태 전환을 사용하지 않고 단일 시장 상태로 동작한다.
- [x] 카드 상세보기 화면 진입/닫기 흐름은 새 플레이 경로에서 제거된다.
- [x] GainLiquidity 화면 진입/닫기 흐름은 새 플레이 경로에서 제거된다.
- [x] 주식 카드는 이미지, 이름, 등급, 가치, 코스트, 배당금을 표시한다.
- [x] 소모형 자원 카드는 이미지, 등급, 제공 자원, 코스트를 표시하고 이름은 표시하지 않는다.
- [x] 카드 호버 확대는 시장 카드와 동일한 정보를 더 큰 스케일로 보여준다.
- [x] 호버 확대는 영업일, 자원, 카드 상태를 변경하지 않는다.
- [x] 낡은 카드 상세보기 액션 보드와 Payment Pot 구현이 새 UI에서 노출되지 않는다.

## Implementation notes

- Completed on 2026-05-20.
- Market card selection now keeps `BusinessDay.MarketArea` at `Market`; `CardDetailState` remains as a compatibility purchase/reservation working model, not a screen state.
- Extra-buy purchase selection also stays in the single market state.
- `PurchasePayment` and `ReservationAction` now validate against the market state instead of requiring `CardDetail`.
- `MarketTapeView` owns a hover-only enlarged card panel. The hover text uses the same formatted card information as the market button and pointer hover does not mutate run state.
- `CardDetailView` no longer becomes visible in the normal market path. Its hidden child controls remain wired as a compatibility action surface for the current tests until a later UI issue replaces them with card-local controls.
- GainLiquidity remains disconnected from the new play flow.

## TDD log

- RED: `MarketAreaFlowTests` failed while market card selection still entered `CardDetail`.
- GREEN: `AssetManager.Tests.EditMode` passed after single-market-state rule changes.
- RED: `MainGameShellBootstrapMarketCardHoverShowsLargerSameCardWithoutStateChange` failed while the hover card panel was absent.
- GREEN: `AssetManager.Tests.PlayMode` passed after hover card wiring and updated single-market-state UI expectations.

## Verification

- EditMode: `AssetManager.Tests.EditMode` 100/100 passed via `scripts/Run-UnityBatchmode.ps1`.
- PlayMode: `AssetManager.Tests.PlayMode` 36/36 passed via `scripts/Run-UnityBatchmode.ps1`.
- Unity manual check: not run separately; PlayMode covers market card click, hover enter/exit, state preservation, hidden CardDetail panel, reservation, purchase, and existing GainLiquidity absence.
- Remaining risk: market card buy/reserve controls are still exposed through hidden compatibility controls in tests rather than a finished card-local action surface.

## Blocked by

- `02-consumable-resource-cards-replace-central-bank.md`
- `04-market-tape-1x8-progress-refresh-pull.md`
- `06-stock-purchase-payment-and-portfolio-cap.md`
