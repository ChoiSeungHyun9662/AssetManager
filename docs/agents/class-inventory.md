# Class Inventory

Living map of implemented production classes for Asset Manager. Keep this as a quick orientation document, not full API documentation.

Last reviewed: 2026-05-25
Covered implementation slices: issues 00-18, stock overhaul issues 01-20

## Update Workflow

Use this workflow whenever production code under `Asset Manager/Assets/_AssetManager/Scripts` changes.

1. Before changing code, read `CONTEXT.md` and this file.
2. If a class, struct, enum, serialized data shape, or major responsibility is added, removed, renamed, or moved, update this file in the same turn.
3. Keep entries grouped by role: shell/editor setup, run data, run rules, market rules, and UI.
4. Use the domain terms from `CONTEXT.md`. If code has a temporary implementation term, describe the domain meaning first and mention the code name second.
5. After updating, compare this file against the source declarations with a class/enum search so the inventory does not drift.
6. If behavior changed, verify through the relevant EditMode or PlayMode tests. Documentation-only inventory updates do not need Unity test runs.

Success criteria for future agents:

- A reader can tell what each production class is for without opening the source.
- A reader can see which issue slice introduced or shaped the current subsystem.
- The document stays short enough to scan before implementation work.

## Issue 00-03 Zoom-Out

Issues 00-03 establish a vertical slice from Unity launch to visible market card flow:

- **00 - Unity project shell**: creates the Unity project root, Bootstrap and MainGame scenes, Game Root, UI Root, setup menu commands, and the first EditMode/PlayMode verification path.
- **01 - 런 bootstrap and data seed**: introduces static data versus runtime state, starts a new 런 at 1회계년도 1Q, initializes resources and 환매 압력, and shows the status HUD.
- **02 - calendar and 영업일 loop**: defines the MVP schedule: 10 playable 분기, 44 playable 영업일, 1/2회계년도 4Q 휴가, and final settlement routing after 3회계년도 4Q.
- **03 - 시장 테이프**: displays 매도 임박, 현재 시장, and 예비 시장; separates 시장 테이프 갱신 from 시장 테이프 진행; prevents visible/owned/reserved/removed card duplication.
- **04 - 시장 영역 and 카드 상세보기**: adds single 시장 영역 transitions, CardDetail transient state, clickable market cards, a CardDetail replacement panel, close handling, and 다음 영업일 gating.
- **05 - 자원 원장 and 보유 자원 UI**: adds ResourceLedger as the public 자원 rule service, separates 조달 현금 from 운용 수익 counters, applies investment philosophy type caps, and displays 보유 자원 with short cap messages.
- **06 - 자산 매수 and 비용 슬롯 결제**: adds PurchasePayment as the public 자산 매수 rule service, creates 비용 슬롯 from 전문 자원 costs, supports 전문 자원/딜 tentative placement and recovery, confirms 시장 카드 purchases, advances only the purchased 시장 테이프 column, and consumes a 영업일.
- **07 - 보유 자산 income and 포트폴리오 summary**: connects OwnedAssetState calculations, AcquiredOrder on owned cards, business-day-start 운용 수익 through ResourceLedger, and a portfolio summary UI for 보유 자산 수, 현재 운용가치, and 분기 운용 수익.
- **08 - 자원 확보 action**: adds LiquidityAction as the public 자원 확보 rule service, opens GainLiquidity from the 중앙 은행, applies 조달 현금 and 전문 자원 choices, completes on two matching or three different basic resources, blocks 딜 and professional-resource cap overflow, and connects the GainLiquidity UI.
- **09 - 예약 action**: adds ReservationAction as the public 예약 rule service, moves 시장 카드 into the 예약 구역, grants 딜, increases 환매 압력, advances only the reserved market-tape column, and connects the 예약 구역 UI.
- **10 - 예약 카드 유지 and 매수**: keeps 예약 카드 across calendar and 시장 테이프 transitions, opens 예약 카드 상세보기 from the 예약 구역, buys reserved cards through PurchasePayment with `Reserved` source, clears only the purchased reservation slot, and leaves 시장 테이프 unchanged.
- **11 - 분기 마감 and 월세 밀림 실패**: adds QuarterSettlement and RentArrears rule modules, applies 정산 수익 before 목표 달성률, excludes 조달 현금 from 분기 수익, stores QuarterEndResult, and switches to 파산 when 월세 밀림 reaches 10.
- **11A - 인플레이션 비용 수정**: adds table-driven quarter inflation as an integer cash-cost modifier, applies it after deal discounts in PurchasePayment, and shows the same final cash cost in 카드 상세보기.
- **12 - 4Q 휴가 and 최종 정산**: adds fiscal-year summary and final settlement rule modules, stores completed 분기 수익 records, displays 4Q 휴가 summaries for 1/2회계년도, and displays 최종 가치-based 최종 정산 after 3회계년도 4Q.

- **13 - 추가 매수권 지원**: adds ExtraBuyAction, stores extra-buy ownership/choice/purchase state on BusinessDayState, lets GrantExtraBuyAction cards keep the same 영업일 open for one extra eligible market purchase, blocks 예약 and 자원 확보 during that state, and clears the right after use or forfeiture.

Current runtime flow:

1. `BootstrapSceneLoader` loads `MainGame`.
2. `MainGameShellBootstrap.Awake` ensures the scene shell through `ProjectShell`.
3. `RunBootstrapper.CreateNewRun` builds `RunSessionState` from `RunStaticDataSet`.
4. `MarketTape.Refresh` fills the first market tape.
5. UI components render the current `RunSessionState`.
6. Buttons call `BusinessDayFlow`, `MarketAreaFlow`, `PurchasePayment`, `ReservationAction`, `MarketTape`, or `ResourceLedger`, then all UI is refreshed from the new state. The old `LiquidityAction` rule still exists for compatibility but is no longer connected to the 플레이 flow.

## Issue 13 Inventory Notes

- `AssetCardData` now includes an optional GrantExtraBuyAction flag for cards that can create an extra buy right after purchase.
- `BusinessDayState` now carries the extra buy right state: has right, awaiting choice, or buying with the right.
- `ExtraBuyAction` (`Runtime/ExtraBuyAction.cs`) is the public rule service for entering extra-buy choice, validating eligible extra-buy candidates, opening extra-buy purchases, and returning from card detail to the pending choice.
- `PurchasePayment` now routes first purchases that grant an extra buy into `ExtraBuyAction` instead of consuming the 영업일 immediately, while purchases made with the right consume it and ignore nested GrantExtraBuyAction grants.
- `RunStatusFormatter`, `RunProgressControls`, `LiquidityActionView`, `MarketTapeDevControls`, and `ResourceDevControls` expose or respect the pending extra-buy state so only market/reserved card purchase or forfeiture remains available.

## Issue 15 Inventory Notes

- `RunStatusFormatter` now keeps player resources out of the top status bar; resource counts live in the bottom chip tray.
- `ResourceHud` now renders the bottom tray as separate resource object lanes: cash as `<value>$`, investment philosophy holdings as large integers with optional small mastery `+N`, chip stacks, Deal chip stack, plus short message text.
- `ProjectShell` creates those tray lanes and image placeholders only when they are missing, preserving existing Editor-authored RectTransform layout on later bootstraps; it also removes legacy root-level Market Tape zone duplicates so the zone panels live under `Market Area Market Panel`, removes legacy Market Tape zone title Text objects, and removes legacy Reservation/Central Bank overlays.

## Issue 16 Inventory Notes

- `MarketTapeView` now renders market asset cards with compact icon/chip tokens for cash, professional resources, income, and management value instead of prose labels.
- `ProjectShell` keeps the market tape as three columns while giving 예비 시장 cards smaller preview-card geometry.
- `MarketAreaFlow`, `CardDetailState`, and `CardDetailView` now distinguish 예비 시장 previews from transaction card details, hiding Buy/Reserve and payment controls for preview cards.

## Issue 17 Inventory Notes

- `ProjectShell` now creates a named Payment Pot background in 카드 상세보기, loads `PaymentPot_Background_Default` when available, and keeps the placeholder/warning path for missing payment pot and resource object sprites.
- `CardDetailView` now treats buyable transaction payment as a Payment Pot surface: only 전문 자원 비용 슬롯 are shown in the pot, preview details hide the pot, and the existing chip placement/recovery buttons continue to update final cash cost and Buy availability.

## Issue 18 Inventory Notes

- `ReservationAction` now treats 예약 as a non-consuming single-stock market-slot toggle: reserving one stock automatically releases any previous reserved slot, does not grant 딜, does not increase 월세 밀림, and leaves 영업일 unchanged.
- `MarketTapeView` now creates stock-only card-local 예약/해제 buttons for current-market slots, keeps those buttons visible while hovering between the card and button, lowers reserved cards, and uses the lowered card position for hover and drag restoration.
- `MainGameShellBootstrap` wires those card-local 예약/해제 buttons directly to `ReservationAction`, keeping button clicks separate from card purchase selection.

## Issue 19 Inventory Notes

- `DealRewardState` stores run-level one-time Deal reward flags for first reaching 3/5/8 occupied portfolio stock slots and first foil creation.
- `DealRewardAction` evaluates Deal rewards after confirmed stock purchases finish ownership, foil merge, and market cleanup; duplicate stock ids count by occupied portfolio slots, and already-granted thresholds are not paid again after sale or merge.
- `DealMasteryAction` converts one Deal into one Reading/Meditation/Patience mastery, rejects empty Deal stacks, and rejects mastery at 5 without consuming the Deal.
- `ResourceHud` now owns Deal chip hover/drag presentation: guide text, temporary hidden source chip, pointer-following Deal image, and guide panel with its bottom-right corner at the pointer.
- `MainGameShellBootstrap` wires Deal drops from the resource HUD into `DealMasteryAction` and keeps purchase confirmation blocking Deal drag.

## Issue 20 Inventory Notes

- `PortfolioSummaryView` now owns drag/drop stock sale presentation: a persistent red `$` sale drop zone, temporary visual hiding of the dragged owned stock card, pointer-following owned-stock detail panel, sale-zone `+N` gain display, pointer-position drop testing, and original `StockSlots` index sale routing.
- `ProjectShell` creates the portfolio sale drop zone and owned stock drag detail panel while keeping legacy owned-stock Sell Button children hidden for scene compatibility.
- `StockSaleAction` continues to be the public rule service for sale execution; tests now pin sale by original stock slot index when displayed portfolio cards are compressed around empty slots.

## Stock Overhaul Issue 01 Notes

- Starter 런 static data now seeds stock cards instead of asset-class cards. `AssetCardData` keeps the existing type name for compatibility, but its public data now exposes `CardDomain.Stock`, base value/dividend, authored foil value/dividend, and min/max deck copy counts.
- Investment philosophy resources are the canonical resource surface: Reading, Meditation, and Patience replace Research, Credit, and Commodity for display and new tests. The old enum/property names remain as compatibility aliases for older purchase and UI wiring code.
- `ResourceLedger` now caps investment philosophy at 5 per type without a total cap, discarding only per-type overflow. Cash and Deal stay outside investment philosophy caps, and Deal no longer has a holding cap.

## Stock Overhaul Issue 02 Notes

- `AssetCardData` now distinguishes stock market cards from consumable resource market cards with `CardDomain.ConsumableResource`, plus provided resource type and amount fields.
- `PurchasePayment` branches confirmed market purchases by card domain: stock cards still become owned assets, while consumable resource cards are paid for with cash only, removed from the market/card pool, grant funding cash or investment philosophy immediately, and consume a 영업일 without entering the portfolio.
- `ProjectShell` and `MainGameShellBootstrap` no longer create, wire, or refresh the 중앙 은행/GainLiquidity play path; legacy 중앙 은행 and GainLiquidity UI objects are removed during shell ensure.
- `MarketTapeView` renders consumable resource cards without a display name, dividend, or 운용가치, showing their cash cost, 희귀도, and provided resource instead.

## Stock Overhaul Issue 03 Notes

- `RunStaticDataSet` now seeds both stock cards and consumable resource deck cards so the market can continue after non-returning stocks are exhausted.
- `MarketConfigData` now carries the stock deck draw weight, defaulting to the PRD's 75% stock / 25% consumable resource split.
- `MarketDeck` is the public market supply rule service: one draw request chooses the stock or consumable resource deck by weight, falls back to the opposite deck when needed, recycles removed consumable resource cards as available draws, never recycles removed stock cards, and throws an explicit exhaustion exception if neither deck can supply a card.
- `MarketTape` now asks `MarketDeck` for each new card instead of directly selecting from a single card list. Issue 04 replaces the old 3-zone movement with ordered 1x8 slot movement.

## Stock Overhaul Issue 04 Notes

- `MarketTapeState` now exposes an ordered 1x8 `Slots` list. `MarketTapeSlotState` stores the card in a slot plus whether that slot is reservation-locked.
- `MarketConfigData` now carries `MarketTapeSlots`, with MVP defaults set to 8; the old 3-zone counts remain as compatibility fields while older callers are migrated.
- `MarketTape.Advance`, `Refresh`, `PullFromEmptySlot`, and `PullAllEmptySlots` are the public 1x8 tape rules: non-reserved cards move left, reserved slots stay fixed, refresh replaces only non-reserved slots, old non-reserved visible cards are removed before schedule refresh so consumables can recycle, and empty slots are pulled from the leftmost gap.
- `ReservationAction` now locks the selected stock in its current market slot instead of replacing it from an upcoming column.
- `PurchasePayment` now routes market and reserved purchases through slot pull behavior after the purchased card leaves the tape. Foil-removal pull integration remains for the later foil issue.
- `BusinessDayFlow` advances the market tape at the start of a normal next business day and refreshes the tape at new-quarter start without an extra progress step.
- `ProjectShell` and `MarketTapeView` render the market as one row of 8 current-market slot buttons; reserved slots are marked on the card.

## Stock Overhaul Issue 05 Notes

- Reservations now live on `MarketTapeSlotState.IsReserved`; `ReservationState` remains only as a compatibility/capacity shell and no longer stores newly reserved cards.
- `ReservationAction` locks the selected stock in its existing market slot, grants one Deal without a holding cap, increases redemption pressure, checks failure immediately, and consumes the business day without advancing the market tape.
- Reserved stock purchases are opened and confirmed as market-slot purchases. Buying the reserved stock clears that locked slot and uses market tape pull behavior to fill the gap.
- `CardDetailState` hides Reserve for reserved cards, consumable resource cards, previews, and extra-buy purchases. The separate Reservation Panel UI is removed because the market card slot itself carries reservation state.

## Stock Overhaul Issue 06 Notes

- `OwnedAssetState` now exposes the 8-stock-slot portfolio limit through `MaxStockSlots`, `OpenStockSlots`, `IsPortfolioFull`, and `CanAcceptStockPurchase`; it leaves a third-copy stock purchase path open for the later foil-combination rule.
- `PurchasePayment` now checks stock portfolio capacity before professional resource slots, cash, or business-day consumption, returns "주식 매도가 필요합니다" for blocked new stock buys, and keeps consumable resource card purchases outside the portfolio cap.
- Market and reserved purchases now call the market tape pull API directly after clearing the purchased slot, so purchase replenishment follows 1x8 slot pull behavior rather than column-advance wording.

## Stock Overhaul Issue 07 Notes

- `RunBootstrapper` creates four runtime instances for each stock definition: the original plus three duplicates. Runtime instances share the same stock `AssetCardData.Id` but have distinct `AssetCardRuntimeData.RuntimeId` values.
- `AssetCardRuntimeData` carries `RuntimeId`, `IsFoil`, and effective management value/income accessors so market state can track physical card instances while portfolio scoring can use authored foil values.
- `OwnedAssetState` exposes `StockSlots` for ordered portfolio slots with preserved empty holes, while `OwnedCards` remains the occupied-card view used by scoring and summaries.
- `PurchasePayment` merges the third owned copy of a stock into one foil owned card in the earliest matching owned slot, leaves the other matching portfolio slots empty, allows the purchase even when the 8-slot portfolio is full, removes remaining same-stock available/reserved runtime cards, clears same-stock market and reservation slots, and pulls empty market slots afterward.

## Stock Overhaul Issue 07a Notes

- `PortfolioSummaryView` now renders the portfolio as an `Owned Stock Card 1~N` compressed card row from `OwnedAssetState.StockSlots` instead of a text list from `OwnedCards`; it walks stock slots left-to-right, skips empty holes, and occupied cards show stock name, rarity, effective value, dividend, and foil state.
- `ProjectShell` creates eight named owned-stock-card containers under the portfolio panel, each with a card button, text child, and hidden sell button, while keeping the legacy owned-cards text object empty for compatibility with older scene objects and tests.

## Stock Overhaul Issue 08 Notes

- `StockSaleAction` is the public rule service for selling an owned stock from a portfolio slot without consuming a 영업일.
- Stock sale rewards use the current quarter inflation modifier: normal stocks pay base 1 plus inflation, foil stocks pay base 3 plus inflation.
- Sold runtime stock cards are marked `Removed` and the sold portfolio slot remains empty, so sold stocks do not return to market supply.
- Stock sale rewards flow through the same revenue counters as dividends and quarter-end settlement income, while consumable resource cash remains funding cash.
- `PortfolioSummaryView` separated stock-card hover from sale confirmation for the issue 08 path; issue 20 superseded that hover Sell Button interaction with drag/drop sale while keeping the legacy child buttons hidden for scene compatibility.

## Stock Overhaul Issue 09 Notes

- `AssetCardData` now exposes `CanBePurchasedWithExtraBuy`: stocks are always eligible, while consumable resource cards must opt in through card data.
- `ExtraBuyAction` now owns extra-buy candidate validation so reserved stocks and non-opt-in consumable resource cards cannot enter an extra-buy purchase.
- `PurchasePayment` preserves the existing extra-buy lifecycle: the granting purchase keeps the current 영업일 open, using the right consumes the 영업일, and nested extra-buy grants during that purchase are ignored.
- Extra-buy purchases use the same 1x8 market-tape pull path as normal market purchases, including allowed consumable resource reward/removal.

## Stock Overhaul Issue 10 Notes

- `MarketAreaFlow` no longer enters `CardDetail` for market-card selection; the market area remains in `Market` while `CardDetailState` temporarily carries the selected purchase/reservation working data.
- `ExtraBuyAction.BeginPurchase`, `PurchasePayment`, and `ReservationAction` now operate from the single market state instead of requiring `MarketAreaState.CardDetail`.
- `MarketTapeView` now owns the hover-only enlarged market card presentation. It shows the same formatted card information as the market button and hides it on pointer exit without changing run state.
- `ProjectShell` creates the hover card panel under the market area and keeps the legacy card-detail panel hidden in normal play.
- `CardDetailView` still refreshes hidden child controls for compatibility with the current purchase/reservation tests, but the visible card-detail screen is no longer part of the new play path.

## Stock Overhaul Issue 15 Notes

- `PurchasePayment` now validates and pays market purchases automatically from final cash plus mastery-discounted investment philosophy costs. Deal is no longer a purchase-payment resource, and manual payment-slot filling is compatibility-only.
- `PurchasePaymentResult` now carries `PurchaseFailureKind` plus the failed market-card runtime id so the shell can suppress system messages for cost shortage while still shaking the attempted card.
- `MarketCardFailureFeedback` is a small card-local presentation component used by PlayMode tests and the market tape view to record purchase-failure shake requests.
- `CardDetailView` keeps the legacy card-detail objects available for compatibility, but Payment Pot, manual slot buttons, and final cash controls are hidden in the new single-market play path.

## Stock Overhaul Issue 16 Notes

- `MarketCardFormatter` centralizes the compact market-card detail text shared by market buttons, hover cards, and purchase confirmation.
- `PurchaseConfirmationView` renders the blocking purchase confirmation modal with shared card detail text, discounted/insufficient cost display, a long bottom `확인` button, and a top-right `돌아가기` button.
- `MainGameShellBootstrap` now treats a valid stock-card short click as purchase-confirmation intent, while a valid consumable-resource short click immediately confirms purchase without the modal. Invalid purchase attempts skip the modal and apply existing card-local failure feedback, and modal confirmation reuses `PurchasePayment.ConfirmPurchase` for confirm-time revalidation.
- While the modal is open, shell-level background commands such as market card selection, schedule/resource/dev controls, reservation, and stock sale return without changing run state.

## v3 Issue 01 Notes

- `StockTagCatalog` owns the five allowed v3 stock tags: Technology, Consumer, Energy, Financials, and Industrials.
- Starter stock data now assigns exactly one allowed v3 tag to every stock. Tags are presentation and later scoring/mission classification only; they do not add stock-specific effect text.
- `RunCalendar` and the default quarter rows now use 8 business days for playable FY1/FY2 quarters and 10 business days for playable FY3 quarters while keeping FY1/FY2 4Q vacation routing and FY3 4Q final-settlement routing.

## Shell And Editor Setup

| Type | File | Purpose |
| --- | --- | --- |
| `ProjectShellRoots` | `Runtime/ProjectShell.cs` | Small return value for the ensured Game Root and UI Canvas. |
| `ProjectShell` | `Runtime/ProjectShell.cs` | Central scene/UI factory for MVP shell objects, names, paths, Canvas settings, placeholder panels, status HUD, market tape UI, hover card presentation, portfolio slot board UI, legacy Payment Pot controls, and temporary controls; applies default layout only to UI objects it creates in the current bootstrap pass, keeps Market Tape zone panels under `Market Area Market Panel`, and removes legacy Reservation Panel plus 중앙 은행/GainLiquidity UI from the new play path. |
| `ProjectShell.PlaceholderPanel` | `Runtime/ProjectShell.cs` | Private helper return value for placeholder panel GameObject/Text pairs. |
| `BootstrapSceneLoader` | `Runtime/BootstrapSceneLoader.cs` | Play-mode component that moves from Bootstrap scene to MainGame scene. |
| `AssetManagerProjectSetup` | `Editor/AssetManagerProjectSetup.cs` | Unity editor menu commands for creating/verifying scenes, build scene entries, static data asset, and required shell objects. |

## Run Data Shapes

These classes are mostly serialized configuration or immutable runtime snapshots.

| Type | File | Purpose |
| --- | --- | --- |
| `RunStaticDataSet` | `Runtime/RunStaticDataSet.cs` | ScriptableObject container for seed stock cards, 분기 data, quarter inflation lookup, final ratings, market/resource/redemption configs, and default run data. |
| `StockTagCatalog` | `Runtime/StockTagCatalog.cs` | Public catalog and validation surface for the five allowed v3 stock tags and the exactly-one-tag stock rule. |
| `ProfessionalResourceCost` | `Runtime/RunModels.cs` | One 전문 자원 cost requirement for a 자산 카드. |
| `TagData` | `Runtime/RunModels.cs` | Serialized 태그 identity, display name, and tag type. |
| `AssetCardData` | `Runtime/RunModels.cs` | Static market card definition: stock costs, 희귀도, value/dividend, authored foil value/dividend, min/max deck copy counts, tags, optional extra-buy grant, and extra-buy purchase eligibility, or consumable resource provided-resource data. The type name and `ManagementValue` accessor remain from the MVP asset-card implementation for compatibility. |
| `QuarterData` | `Runtime/RunModels.cs` | Static 분기 row used by bootstrap, 분기 마감 target lookup, and table-driven inflation cash-cost modifiers. `RunCalendar` still owns playable schedule routing. |
| `FinalRatingData` | `Runtime/RunModels.cs` | Final rating threshold row for later 최종 정산, keyed by minimum final value with the old management-value field/accessor kept for compatibility. |
| `RedemptionPressureLevelData` | `Runtime/RunModels.cs` | 월세 밀림 단계 row for later final comments; type name remains for compatibility. |
| `FinalManagementCommentData` | `Runtime/RunModels.cs` | 운용 코멘트 row keyed by final rating and 환매 압력 단계. |
| `MarketConfigData` | `Runtime/RunModels.cs` | Market tape slot count for the 1x8 tape, legacy 3-zone slot counts kept for compatibility, and the stock deck draw weight. MVP default is 8 slots with 75% stock draw weight. |
| `ResourceConfigData` | `Runtime/RunModels.cs` | Starting 현금, active investment philosophy type cap, legacy total-cap field, and reservation capacity compatibility value. |
| `RedemptionPressureConfigData` | `Runtime/RunModels.cs` | Starting and maximum 월세 밀림; type name remains for compatibility. |

## Runtime State Shapes

| Type | File | Purpose |
| --- | --- | --- |
| `ResourceState` | `Runtime/RunModels.cs` | Current 현금, 독서, 명상, 인내, 딜, plus investment philosophy total lookup. Old Research/Credit/Commodity property names remain as aliases. |
| `InvestmentPhilosophyMasteryState` | `Runtime/RunModels.cs` | Run-scoped 독서/명상/인내 mastery values used to discount purchase philosophy costs; values are capped per type by `ResourceLedger`. |
| `RunCalendarState` | `Runtime/RunModels.cs` | Current 회계년도, 분기, and remaining 영업일. |
| `RunPerformanceState` | `Runtime/RunModels.cs` | Current 분기, 회계년도, and total 수익 counters, tracked 조달 현금, and completed 분기 수익 records for 4Q 휴가 summaries; Revenue is the canonical public surface and `EarnedCash` remains as compatibility aliases. |
| `QuarterPerformanceRecord` | `Runtime/RunModels.cs` | Completed 회계년도/분기 수익 row recorded at 분기 마감 for later 회계년도 summary display, with Revenue as the canonical accessor. |
| `AssetCardRuntimeData` | `Runtime/RunModels.cs` | Runtime wrapper for one physical market card instance and whether it is available, reserved, owned, or removed; owned cards can carry purchase source, acquired order, foil state, and effective value/income derived from base or foil card data. |
| `MarketTapeSlotState` | `Runtime/RunModels.cs` | One 1x8 market tape slot: optional visible market card plus whether the slot is reservation-locked. |
| `MarketTapeState` | `Runtime/RunModels.cs` | Current ordered 1x8 market tape slots, with `CurrentMarketCards` projecting visible slot cards for older callers during the migration. |
| `ReservationState` | `Runtime/RunModels.cs` | Reservation capacity compatibility shell. Newly reserved stocks are tracked on market tape slots rather than in this separate collection. |
| `OwnedAssetState` | `Runtime/RunModels.cs` | Current 보유 자산 list plus Owned-only 보유 자산 수, 현재 가치, 영업일 시작 운용 수익 totals using foil-aware runtime values, and the 8-stock-slot portfolio capacity helpers. |
| `BusinessDayState` | `Runtime/RunModels.cs` | Current phase and 시장 영역 state. |
| `LiquidityActionState` | `Runtime/RunModels.cs` | Current 자원 확보 selected basic resources and whether the first resource has committed the action. |
| `QuarterEndResult` | `Runtime/RunModels.cs` | Snapshot of a completed 분기 마감: 정산 수익, 분기 수익, 분기 목표, 목표 달성률, and 월세 밀림 impact. Legacy property names remain as aliases. |
| `CardDetailDisplayData` | `Runtime/CardDetailState.cs` | Snapshot of selected 자산 카드 fields shown in 카드 상세보기, exposing value as the canonical score field. |
| `PaymentSlotState` | `Runtime/CardDetailState.cs` | One 비용 슬롯 in 카드 상세보기: required 전문 자원 and optional placed 전문 자원 or 딜. |
| `PurchaseCostToken` | `Runtime/CardDetailState.cs` | Display/validation token for one purchase cost amount, including whether the current run lacks that amount. |
| `PurchaseCostBreakdown` | `Runtime/CardDetailState.cs` | Display-ready purchase cost summary with original philosophy costs, mastery-discounted philosophy costs, cash token, and plain text formatting. |
| `PurchasePaymentState` | `Runtime/CardDetailState.cs` | Pending 자산 매수 payment in 카드 상세보기: card id, base cash cost, mastery-discounted 비용 슬롯 list, current quarter inflation modifier, and final cash cost. |
| `CardDetailState` | `Runtime/CardDetailState.cs` | Transient 카드 상세보기 state: selected card, 매수 출처, display data, pending payment, extra-buy flag, preview flag, and Buy/Reserve visibility conditions. |
| `RedemptionPressureState` | `Runtime/RunModels.cs` | Current and maximum 월세 밀림, with rent-arrears aliases over the compatibility pressure property names. |
| `RunSessionState` | `Runtime/RunModels.cs` | Top-level 런 snapshot passed through rules and UI, including resources, investment philosophy mastery, transient 카드 상세보기, 자원 확보 state, latest QuarterEndResult, rent arrears state, and failure reason. Most transitions create a new instance. |

| `DealRewardState` | `Runtime/RunModels.cs` | Run-scoped one-time Deal reward flags for portfolio occupied stock-slot thresholds 3/5/8 and first foil completion. |

## Enums

| Type | Purpose |
| --- | --- |
| `ResourceType` | Cash, investment philosophy resources, and 딜 identifiers. Reading/Meditation/Patience are canonical; Research/Credit/Commodity remain aliases. |
| `CardDomain` | Market card domain: Stock for portfolio cards, ConsumableResource for immediate cash or investment-philosophy supply cards. |
| `AssetRarity` | 자산 카드 희귀도. |
| `TagType` | Current tag grouping categories. |
| `PurchaseSource` | Whether 자산 매수 came from market tape or reservation. |
| `AssetCardRuntimeState` | Available, reserved, owned, or removed card state. |
| `MarketAreaState` | Legacy area enum. New stock-overhaul play keeps the market path in `Market`; `CardDetail` and `GainLiquidity` remain only for compatibility with older rule/UI code. |
| `MarketTapeZone` | 매도 임박, 현재 시장, or 예비 시장. |
| `RunState` | Not started, playing, failed, or completed 런 state. |
| `BusinessDayPhase` | Awaiting action, resolving action, 분기 마감, 4Q 휴가, or 최종 정산. |
| `PurchaseFailureKind` | Purchase attempt failure category: no failure, cost shortage with suppressed system message, or non-cost failure with the existing message. |

## Run Rules

| Type | File | Purpose |
| --- | --- | --- |
| `RunBootstrapper` | `Runtime/RunBootstrapper.cs` | Creates a new playable `RunSessionState` from static data and immediately performs initial 시장 테이프 갱신. |
| `RunCalendarQuarter` | `Runtime/RunCalendar.cs` | One playable calendar row with 회계년도, 분기, and 영업일 count. |
| `RunCalendarDefinition` | `Runtime/RunCalendar.cs` | Query object for playable quarters, 4Q 휴가, and total playable 영업일. |
| `RunCalendar` | `Runtime/RunCalendar.cs` | Factory for the playable calendar: 1/2회계년도 1Q-3Q have 8 영업일; 3회계년도 1Q-4Q have 10 영업일. |
| `BusinessDayFlow` | `Runtime/BusinessDayFlow.cs` | Advances the 영업일 loop, applies 보유 자산 영업일 시작 운용 수익 through ResourceLedger, settles the last 영업일 into 분기 마감, blocks schedule progress after 런 실패, routes 4Q 휴가, starts next 회계년도, and reaches 최종 정산. |
| `MarketAreaFlow` | `Runtime/MarketAreaFlow.cs` | Public rule service for selecting/clearing a market card purchase working model while keeping the visible market area in the single `Market` state, plus 다음 영업일 gating. |
| `ResourceLedger` | `Runtime/ResourceLedger.cs` | Public rule service for adding 조달 현금, 수익 cash through AddRevenue, per-type-capped investment philosophy resources, per-type-capped investment philosophy mastery, and uncapped 딜. |
| `ResourceLedgerResult` | `Runtime/ResourceLedger.cs` | Return data for capped 자원 operations, including gained amount, discarded amount, and short feedback message. |
| `DealRewardAction` | `Runtime/DealRewardAction.cs` | Public rule service for run-level Deal rewards from occupied portfolio stock-slot thresholds and first foil creation. |
| `DealRewardActionResult` | `Runtime/DealRewardAction.cs` | Return data for Deal reward evaluation, including the updated run and number of Deals granted. |
| `DealMasteryAction` | `Runtime/DealMasteryAction.cs` | Public rule service for dropping one Deal on an investment philosophy lane to consume the Deal and add one mastery, with max-mastery failure feedback. |
| `DealMasteryActionResult` | `Runtime/DealMasteryAction.cs` | Return data for Deal-to-mastery attempts, including success and short feedback message. |
| `StockSaleAction` | `Runtime/StockSaleAction.cs` | Public rule service for selling owned stock slots, leaving an empty portfolio slot, removing the sold runtime stock from the run, and adding sale cash as 수익 without consuming a 영업일. |
| `StockSaleActionResult` | `Runtime/StockSaleAction.cs` | Return data for stock sale attempts, including the updated run, success flag, and short feedback message. |
| `QuarterSettlement` | `Runtime/QuarterSettlement.cs` | Public rule service for 분기 마감 정산, 정산 수익 application, 목표 달성률, 월세 밀림 increase, and QuarterEndResult creation. |
| `QuarterSettlementResult` | `Runtime/QuarterSettlement.cs` | Return data for 분기 마감, including the updated run, stored QuarterEndResult fields, and stock-overhaul revenue/rent-arrears aliases. |
| `FiscalYearSummary` | `Runtime/FiscalYearSummary.cs` | Public rule service for 4Q 휴가 summary data: 현재 가치, 올해 수익, completed 분기별 수익, 보유 주식 수, and 월세 밀림. |
| `FiscalYearSummaryResult` | `Runtime/FiscalYearSummary.cs` | Return data displayed by the 4Q 휴가 panel, with stock-overhaul value/revenue/rent-arrears aliases over compatibility names. |
| `FinalSettlement` | `Runtime/FinalSettlement.cs` | Public rule service for 최종 정산: computes 최종 가치 from 보유 주식 only, selects the highest reachable 최종 평가, and picks a 최종 코멘트 by 월세 밀림 단계. |
| `FinalSettlementResult` | `Runtime/FinalSettlement.cs` | Return data displayed by the 최종 정산 panel, with final value, owned stock count, rent arrears, and final comment aliases over compatibility names. |
| `RentArrears` | `Runtime/RedemptionPressure.cs` | Public rule service for adding 월세 밀림 and immediately converting the run to 파산 at the configured max. |
| `RentArrearsResult` | `Runtime/RedemptionPressure.cs` | Return data for 월세 밀림 changes, including the updated run, increase amount, and bankruptcy flag. |
| `RedemptionPressure` | `Runtime/RedemptionPressure.cs` | Compatibility wrapper over RentArrears for older pressure-named callers. |
| `RedemptionPressureResult` | `Runtime/RedemptionPressure.cs` | Compatibility return data for pressure-named callers. |
| `LiquidityAction` | `Runtime/LiquidityAction.cs` | Public rule service for 자원 확보 entry, close eligibility, selected resource sequence validation, funding-cash/investment-philosophy gain, per-type capacity checks, auto-ending, and 영업일 consumption. |
| `LiquidityActionResult` | `Runtime/LiquidityAction.cs` | Return data for 자원 확보 selections, including the updated run and short feedback message. |
| `ExtraBuyAction` | `Runtime/ExtraBuyAction.cs` | Public rule service for granting, awaiting, validating, entering, returning from, and clearing extra-buy purchase state; candidates are available market stocks or card-data-opt-in consumable resource cards, never reserved stocks. |
| `PurchaseCostCalculator` | `Runtime/CardDetailState.cs` | Public helper for calculating immutable-source purchase costs with run mastery discounts, zero-floor philosophy costs, display formatting, and insufficient-token marking inputs. |
| `PurchasePayment` | `Runtime/PurchasePayment.cs` | Public rule service for 카드 상세보기 mastery-discounted compatibility slot creation, automatic final-cash and investment-philosophy payment validation/consumption, Deal exclusion from purchase payment, portfolio-cap validation, stock ownership transition, consumable resource card reward/removal, market-slot pull after market or reserved-slot purchase, and 영업일 consumption. |
| `PurchasePaymentResult` | `Runtime/PurchasePayment.cs` | Return data for 결제 and 자산 매수 operations, including success, short feedback message, failure category, and failed market-card runtime id for card-local feedback. |
| `ReservationAction` | `Runtime/ReservationAction.cs` | Public rule service for stock-only single market-slot reservation and unreservation, including automatic previous reservation release, no 딜 reward, no 월세 밀림 increase, and no 영업일 consumption. |
| `ReservationActionResult` | `Runtime/ReservationAction.cs` | Return data for 예약 operations, including success and short feedback message. |

## Market Rules

| Type | File | Purpose |
| --- | --- | --- |
| `MarketDeck` | `Runtime/MarketDeck.cs` | Public rule service for one market supply draw from separated stock and consumable resource decks, including weighted deck choice, fallback, consumable recycling, non-returning removed stocks, and explicit exhaustion. |
| `MarketTape` | `Runtime/MarketTape.cs` | Pure rule service for 1x8 시장 테이프 갱신, 영업일 시작 진행, 빈칸 당김, multiple-gap pull order, reservation slot locking, duplicate prevention, and removed-card marking; delegates each new card supply request to `MarketDeck`. |

Important distinction:

- `Refresh` replaces non-reserved 1x8 slots and keeps reservation-locked slots fixed.
- `Advance` removes the leftmost non-reserved card, moves the remaining non-reserved cards left across non-reserved slots, and supplies new cards on the right.
- `PullFromEmptySlot` compresses cards to the right of one empty slot leftward while reserved slots stay fixed, then supplies one card to the rightmost non-reserved gap.
- `PullAllEmptySlots` repeats that pull from the leftmost remaining empty slot.
- `MarketDeck` owns whether a new card comes from the stock deck or consumable resource deck; `MarketTape` only supplies exclusion context such as visible, owned, reserved, and non-returning stock cards.

## UI And Presentation

| Type | File | Purpose |
| --- | --- | --- |
| `MainGameShellBootstrap` | `Runtime/MainGameShellBootstrap.cs` | Runtime orchestrator: owns `CurrentRun`, wires buttons plus market/reserved card selection, market-card release intents, card-local reservation toggles, and Deal HUD drops to rule services while keeping the visible market state active, opens and guards the purchase confirmation modal for valid stock-card click purchase intent, routes valid consumable-resource clicks and portfolio-area releases to immediate purchase, treats non-portfolio drag releases as no-ops, refreshes visible UI, and leaves the old GainLiquidity entry disconnected from the new play flow. |
| `RunStatusFormatter` | `Runtime/RunStatusFormatter.cs` | Formats the top HUD time/progress/rent-arrears text from `RunSessionState` without player resource counts. |
| `RunStatusHud` | `Runtime/RunStatusHud.cs` | MonoBehaviour wrapper that displays formatted 런 status. |
| `ResourceHud` | `Runtime/ResourceHud.cs` | Displays the bottom chip tray: cash as `<value>$`, investment philosophy holdings as large integers, optional small mastery `+N`, philosophy chip stacks, Deal chip stack, manual Sprite slots, current short resource message, runtime chip stack instances anchored to the configured base images, and Deal hover/drag/drop presentation for mastery conversion. |
| `PortfolioSummaryView` | `Runtime/PortfolioSummaryView.cs` | Displays the 포트폴리오 summary plus an `OwnedAssetState.StockSlots`-derived compressed owned-stock-card row; empty slots are skipped, occupied cards show stock name, rarity, effective value, dividend, and foil state, and owned stocks sell through a drag/drop flow using a persistent red `$` sale zone and original `StockSlots` index. |
| `RunProgressControls` | `Runtime/RunProgressControls.cs` | Shows/hides 다음 영업일, 계속, 분기 마감, 4Q 휴가, 파산, and 최종 정산 UI; displays current 분기 수익, 현재 가치, 보유 주식, 월세 밀림, final value, and final comment summaries. |
| `MarketTapeView` | `Runtime/MarketTapeView.cs` | Renders the pointer-draggable 1x8 market tape, classifies click vs drag by pointer movement threshold, positions card-number-based hover enlargement, creates stock-only card-local 예약/해제 buttons, lowers reserved cards for interaction, and records card-local purchase-failure shake requests. |
| `MarketCardFormatter` | `Runtime/MarketCardFormatter.cs` | Shared formatter for compact market-card detail text used by market buttons, hover card presentation, and purchase confirmation. |
| `LiquidityActionView` | `Runtime/LiquidityActionView.cs` | Legacy GainLiquidity view for 중앙 은행 resource-object choices; no longer created or wired by the new play flow. |
| `CardDetailView` | `Runtime/CardDetailView.cs` | Legacy card-detail/payment control view. The panel, Payment Pot, manual slot buttons, and final cash controls stay hidden in the new single-market play path, while selected-card data remains available for compatibility until card-local action controls replace them. |
| `MarketCardFailureFeedback` | `Runtime/MarketTapeView.cs` | Card-local presentation marker that records purchase-failure shake requests for the attempted market card. |
| `PurchaseConfirmationView` | `Runtime/PurchaseConfirmationView.cs` | Blocking modal surface for normal market-card purchase confirmation, showing shared card detail text plus final discounted cost state and exposing `확인`/`돌아가기` buttons to the shell. |
| `MarketTapeDevControls` | `Runtime/MarketTapeDevControls.cs` | Temporary Market-state-only development buttons for 시장 테이프 진행 and 시장 테이프 갱신. |
| `ResourceDevControls` | `Runtime/ResourceDevControls.cs` | Temporary Market-state-only development buttons for adding 조달 현금, 운용 수익, 전문 자원, and 딜 through `ResourceLedger`. |

## Verification Map

| Area | Tests |
| --- | --- |
| Shell/editor setup | `ProjectShellSetupTests`, `MainGameShellBootstrapTests` |
| 런 bootstrap | `RunBootstrapperTests`, `MainGameShellBootstrapTests` |
| Calendar and 영업일 loop | `RunCalendarTests`, `BusinessDayFlowTests`, `MainGameShellBootstrapTests` |
| 시장 테이프 rules | `MarketTapeTests`, `BusinessDayFlowTests`, `MainGameShellBootstrapTests` |
| 시장 영역 and 카드 상세보기 | `MarketAreaFlowTests`, `MainGameShellBootstrapTests` |
| 자원 원장 and 보유 자원 UI | `ResourceLedgerTests`, `MainGameShellBootstrapTests` |
| 딜 rewards and mastery drag | `DealRewardActionTests`, `DealMasteryActionTests`, `PurchasePaymentTests`, `ReservationActionTests`, `MainGameShellBootstrapTests` |
| 자산 매수 and 비용 슬롯 결제 | `PurchasePaymentTests`, `MainGameShellBootstrapTests` |
| 보유 자산 income and 포트폴리오 UI | `OwnedAssetStateTests`, `BusinessDayFlowTests`, `PurchasePaymentTests`, `MainGameShellBootstrapTests` |
| 소모형 자원 카드 and retired GainLiquidity path | `PurchasePaymentTests`, `MainGameShellBootstrapTests`, `MvpSmokeScenarioTests` |
| 예약 action, card-local 예약 UI, and 예약 카드 매수 | `ReservationActionTests`, `PurchasePaymentTests`, `BusinessDayFlowTests`, `MarketTapeTests`, `MainGameShellBootstrapTests` |
| 분기 마감, 4Q 휴가, 최종 정산, and 월세 밀림 실패 | `QuarterSettlementTests`, `FiscalYearSummaryTests`, `FinalSettlementTests`, `BusinessDayFlowTests`, `ResourceLedgerTests`, `ReservationActionTests`, `MainGameShellBootstrapTests` |
| 주식 매도 and 수익 tracking | `StockSaleActionTests`, `MainGameShellBootstrapTests`, `BusinessDayFlowTests`, `QuarterSettlementTests`, `ResourceLedgerTests`, `PurchasePaymentTests` |

## Notes For Next Cleanup

- `RunCalendar` still owns playable schedule routing, while `RunStaticDataSet.Quarters` now supplies MVP target rows for every playable 분기. Keep those responsibilities separate unless a future data unification issue explicitly changes it.
