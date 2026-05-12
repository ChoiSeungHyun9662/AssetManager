# Class Inventory

Living map of implemented production classes for Asset Manager. Keep this as a quick orientation document, not full API documentation.

Last reviewed: 2026-05-12
Covered implementation slices: issues 00-10

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
- **05 - 자원 원장 and 보유 자원 UI**: adds ResourceLedger as the public 자원 rule service, separates 조달 현금 from 운용 수익 counters, applies 전문 자원 and 딜 한도, and displays 보유 자원 with short cap messages.
- **06 - 자산 매수 and 비용 슬롯 결제**: adds PurchasePayment as the public 자산 매수 rule service, creates 비용 슬롯 from 전문 자원 costs, supports 전문 자원/딜 tentative placement and recovery, confirms 시장 카드 purchases, advances only the purchased 시장 테이프 column, and consumes a 영업일.
- **07 - 보유 자산 income and 포트폴리오 summary**: connects OwnedAssetState calculations, AcquiredOrder on owned cards, business-day-start 운용 수익 through ResourceLedger, and a portfolio summary UI for 보유 자산 수, 현재 운용가치, and 분기 운용 수익.
- **08 - 자원 확보 action**: adds LiquidityAction as the public 자원 확보 rule service, opens GainLiquidity from the 중앙 은행, applies 조달 현금 and 전문 자원 choices, completes on two matching or three different basic resources, blocks 딜 and professional-resource cap overflow, and connects the GainLiquidity UI.
- **09 - 예약 action**: adds ReservationAction as the public 예약 rule service, moves 시장 카드 into the 예약 구역, grants 딜, increases 환매 압력, advances only the reserved market-tape column, and connects the 예약 구역 UI.
- **10 - 예약 카드 유지 and 매수**: keeps 예약 카드 across calendar and 시장 테이프 transitions, opens 예약 카드 상세보기 from the 예약 구역, buys reserved cards through PurchasePayment with `Reserved` source, clears only the purchased reservation slot, and leaves 시장 테이프 unchanged.

Current runtime flow:

1. `BootstrapSceneLoader` loads `MainGame`.
2. `MainGameShellBootstrap.Awake` ensures the scene shell through `ProjectShell`.
3. `RunBootstrapper.CreateNewRun` builds `RunSessionState` from `RunStaticDataSet`.
4. `MarketTape.Refresh` fills the first market tape.
5. UI components render the current `RunSessionState`.
6. Buttons call `BusinessDayFlow`, `MarketAreaFlow`, `PurchasePayment`, `ReservationAction`, `LiquidityAction`, `MarketTape`, or `ResourceLedger`, then all UI is refreshed from the new state.

## Shell And Editor Setup

| Type | File | Purpose |
| --- | --- | --- |
| `ProjectShellRoots` | `Runtime/ProjectShell.cs` | Small return value for the ensured Game Root and UI Canvas. |
| `ProjectShell` | `Runtime/ProjectShell.cs` | Central scene/UI factory for MVP shell objects, names, paths, Canvas settings, placeholder panels, status HUD, market tape UI, and temporary controls. |
| `ProjectShell.PlaceholderPanel` | `Runtime/ProjectShell.cs` | Private helper return value for placeholder panel GameObject/Text pairs. |
| `BootstrapSceneLoader` | `Runtime/BootstrapSceneLoader.cs` | Play-mode component that moves from Bootstrap scene to MainGame scene. |
| `AssetManagerProjectSetup` | `Editor/AssetManagerProjectSetup.cs` | Unity editor menu commands for creating/verifying scenes, build scene entries, static data asset, and required shell objects. |

## Run Data Shapes

These classes are mostly serialized configuration or immutable runtime snapshots.

| Type | File | Purpose |
| --- | --- | --- |
| `RunStaticDataSet` | `Runtime/RunStaticDataSet.cs` | ScriptableObject container for seed 자산 카드, 분기 data, final ratings, market/resource/redemption configs, and MVP default data. |
| `ProfessionalResourceCost` | `Runtime/RunModels.cs` | One 전문 자원 cost requirement for a 자산 카드. |
| `TagData` | `Runtime/RunModels.cs` | Serialized 태그 identity, display name, and tag type. |
| `AssetCardData` | `Runtime/RunModels.cs` | Static 자산 카드 definition: costs, 희귀도, 운용가치, income field, and tags. |
| `QuarterData` | `Runtime/RunModels.cs` | Static 분기 row used by bootstrap data. The full MVP playable schedule currently lives in `RunCalendar`. |
| `FinalRatingData` | `Runtime/RunModels.cs` | Final rating threshold row for later 최종 정산. |
| `RedemptionPressureLevelData` | `Runtime/RunModels.cs` | 환매 압력 단계 row for later final comments. |
| `FinalManagementCommentData` | `Runtime/RunModels.cs` | 운용 코멘트 row keyed by final rating and 환매 압력 단계. |
| `MarketConfigData` | `Runtime/RunModels.cs` | Slot counts for 매도 임박, 현재 시장, and 예비 시장. MVP default is 3/3/3. |
| `ResourceConfigData` | `Runtime/RunModels.cs` | Starting 현금, 전문 자원 한도, and 딜 한도. |
| `RedemptionPressureConfigData` | `Runtime/RunModels.cs` | Starting and maximum 환매 압력. |

## Runtime State Shapes

| Type | File | Purpose |
| --- | --- | --- |
| `ResourceState` | `Runtime/RunModels.cs` | Current 현금, 리서치, 신용, 원자재, 딜, plus 전문 자원 total lookup. |
| `RunCalendarState` | `Runtime/RunModels.cs` | Current 회계년도, 분기, and remaining 영업일. |
| `RunPerformanceState` | `Runtime/RunModels.cs` | Current 분기, 회계년도, and total 운용 수익 counters. |
| `AssetCardRuntimeData` | `Runtime/RunModels.cs` | Runtime wrapper for one 자산 카드 and whether it is available, reserved, owned, or removed; owned cards can carry 매수 출처 and AcquiredOrder. |
| `MarketTapeState` | `Runtime/RunModels.cs` | Current visible market tape cards by zone. |
| `ReservationState` | `Runtime/RunModels.cs` | 예약 구역 capacity and reserved cards. |
| `OwnedAssetState` | `Runtime/RunModels.cs` | Current 보유 자산 list plus Owned-only 보유 자산 수, 현재 운용가치, and 영업일 시작 운용 수익 totals. |
| `BusinessDayState` | `Runtime/RunModels.cs` | Current phase and 시장 영역 state. |
| `LiquidityActionState` | `Runtime/RunModels.cs` | Current 자원 확보 selected basic resources and whether the first resource has committed the action. |
| `CardDetailDisplayData` | `Runtime/CardDetailState.cs` | Snapshot of selected 자산 카드 fields shown in 카드 상세보기. |
| `PaymentSlotState` | `Runtime/CardDetailState.cs` | One 비용 슬롯 in 카드 상세보기: required 전문 자원 and optional placed 전문 자원 or 딜. |
| `PurchasePaymentState` | `Runtime/CardDetailState.cs` | Pending 자산 매수 payment in 카드 상세보기: card id, base cash cost, 비용 슬롯 list, and 딜-discounted final cash cost. |
| `CardDetailState` | `Runtime/CardDetailState.cs` | Transient 카드 상세보기 state: selected card, 매수 출처, display data, pending payment, extra-buy flag, and Reserve visibility condition. |
| `RedemptionPressureState` | `Runtime/RunModels.cs` | Current and maximum 환매 압력. |
| `RunSessionState` | `Runtime/RunModels.cs` | Top-level 런 snapshot passed through rules and UI, including transient 카드 상세보기 and 자원 확보 state. Most transitions create a new instance. |

## Enums

| Type | Purpose |
| --- | --- |
| `ResourceType` | Cash, professional resources, and 딜 identifiers. |
| `AssetRarity` | 자산 카드 희귀도. |
| `TagType` | Current tag grouping categories. |
| `PurchaseSource` | Whether 자산 매수 came from market tape or reservation. |
| `AssetCardRuntimeState` | Available, reserved, owned, or removed card state. |
| `MarketAreaState` | 시장, 카드 상세보기, or 자원 확보 area state. |
| `MarketTapeZone` | 매도 임박, 현재 시장, or 예비 시장. |
| `RunState` | Not started, playing, failed, or completed 런 state. |
| `BusinessDayPhase` | Awaiting action, resolving action, 분기 마감, 4Q 휴가, or 최종 정산. |

## Run Rules

| Type | File | Purpose |
| --- | --- | --- |
| `RunBootstrapper` | `Runtime/RunBootstrapper.cs` | Creates a new playable `RunSessionState` from static data and immediately performs initial 시장 테이프 갱신. |
| `RunCalendarQuarter` | `Runtime/RunCalendar.cs` | One playable calendar row with 회계년도, 분기, and 영업일 count. |
| `RunCalendarDefinition` | `Runtime/RunCalendar.cs` | Query object for playable quarters, 4Q 휴가, and total playable 영업일. |
| `RunCalendar` | `Runtime/RunCalendar.cs` | Factory for the MVP calendar: 1/2회계년도 1Q-3Q have 4 영업일; 3회계년도 1Q-4Q have 5 영업일. |
| `BusinessDayFlow` | `Runtime/BusinessDayFlow.cs` | Advances the 영업일 loop, applies 보유 자산 영업일 시작 운용 수익 through ResourceLedger, enters 분기 마감, routes 4Q 휴가, starts next 회계년도, and reaches 최종 정산. |
| `MarketAreaFlow` | `Runtime/MarketAreaFlow.cs` | Public rule service for entering/closing 카드 상세보기 and gating 다음 영업일 to Market state only. |
| `ResourceLedger` | `Runtime/ResourceLedger.cs` | Public rule service for adding 조달 현금, 운용 수익, capped 전문 자원, and capped 딜. |
| `ResourceLedgerResult` | `Runtime/ResourceLedger.cs` | Return data for capped 자원 operations, including gained amount, discarded amount, and short feedback message. |
| `LiquidityAction` | `Runtime/LiquidityAction.cs` | Public rule service for 자원 확보 entry, close eligibility, selected resource sequence validation, funding-cash/professional-resource gain, auto-ending, and 영업일 consumption. |
| `LiquidityActionResult` | `Runtime/LiquidityAction.cs` | Return data for 자원 확보 selections, including the updated run and short feedback message. |
| `PurchasePayment` | `Runtime/PurchasePayment.cs` | Public rule service for 카드 상세보기 비용 슬롯 creation, tentative chip placement/recovery, 자산 매수 validation, 시장/예약 카드 ownership transition, purchased-column advance for 시장 카드 only, reservation cleanup for 예약 카드, and 영업일 consumption. |
| `PurchasePaymentResult` | `Runtime/PurchasePayment.cs` | Return data for 결제 and 자산 매수 operations, including success and short feedback message. |
| `ReservationAction` | `Runtime/ReservationAction.cs` | Public rule service for 예약 validation, 예약 카드 transition, 예약 구역 capacity, 딜 reward, 환매 압력 increase/failure check, reserved-column market-tape advance, and 영업일 consumption. |
| `ReservationActionResult` | `Runtime/ReservationAction.cs` | Return data for 예약 operations, including success and short feedback message. |

## Market Rules

| Type | File | Purpose |
| --- | --- | --- |
| `MarketTape` | `Runtime/MarketTape.cs` | Pure rule service for 시장 테이프 갱신, 시장 테이프 진행, 슬롯 보충, single-column advance after purchase/reservation, duplicate prevention, and removed-card marking. |

Important distinction:

- `Refresh` rebuilds the whole 시장 테이프 and marks previously visible available cards as removed.
- `Advance` removes 매도 임박 cards, moves 현재 시장 to 매도 임박, moves 예비 시장 to 현재 시장, then refills 예비 시장.
- `RefillSlot` fills only one zone up to its configured slot count.
- `AdvanceSlotAt` advances only one vertical market column after purchase/reservation: the selected cell is filled by the card behind it, and the 예비 시장 cell receives a new card.

## UI And Presentation

| Type | File | Purpose |
| --- | --- | --- |
| `MainGameShellBootstrap` | `Runtime/MainGameShellBootstrap.cs` | Runtime orchestrator: owns `CurrentRun`, wires buttons plus market/reserved card clicks to rule services, and refreshes all visible UI including GainLiquidity. |
| `RunStatusFormatter` | `Runtime/RunStatusFormatter.cs` | Formats the top HUD text from `RunSessionState`. |
| `RunStatusHud` | `Runtime/RunStatusHud.cs` | MonoBehaviour wrapper that displays formatted 런 status. |
| `ResourceHud` | `Runtime/ResourceHud.cs` | Displays 보유 자원, 전문 자원 total/cap, 딜 total/cap, and the current short resource message. |
| `PortfolioSummaryView` | `Runtime/PortfolioSummaryView.cs` | Displays the 포트폴리오 summary: 보유 자산 수, 현재 운용가치, 이번 분기 운용 수익, and a short ordered 보유 자산 list. |
| `RunProgressControls` | `Runtime/RunProgressControls.cs` | Shows/hides 다음 영업일, 계속, 분기 마감, 4Q 휴가, 런 실패, and 최종 정산 placeholder UI; 다음 영업일 uses MarketArea gating. |
| `MarketTapeView` | `Runtime/MarketTapeView.cs` | Renders market tape zone names and clickable visible market card summary buttons. |
| `ReservationView` | `Runtime/ReservationView.cs` | Renders the 예약 구역 count and three reserved-card summary slots in the 시장 area, with clickable occupied slots for 예약 카드 상세보기. |
| `LiquidityActionView` | `Runtime/LiquidityActionView.cs` | Shows 중앙 은행 entry, GainLiquidity resource buttons, selected resource text, close gating, and cap feedback through `LiquidityAction`. |
| `CardDetailView` | `Runtime/CardDetailView.cs` | Shows the 카드 상세보기 replacement panel, selected card display data, 비용 슬롯 state, final cash cost, chip placement/recovery buttons, buy availability, and 예약 button visibility/availability. |
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
| 자산 매수 and 비용 슬롯 결제 | `PurchasePaymentTests`, `MainGameShellBootstrapTests` |
| 보유 자산 income and 포트폴리오 UI | `OwnedAssetStateTests`, `BusinessDayFlowTests`, `PurchasePaymentTests`, `MainGameShellBootstrapTests` |
| 자원 확보 action and GainLiquidity UI | `LiquidityActionTests`, `MainGameShellBootstrapTests` |
| 예약 action, 예약 구역 UI, and 예약 카드 매수 | `ReservationActionTests`, `PurchasePaymentTests`, `BusinessDayFlowTests`, `MarketTapeTests`, `MainGameShellBootstrapTests` |

## Notes For Next Cleanup

- `MarketTapeView` currently displays the label `인컴`; the glossary prefers 운용 수익 language. Do not spread the older term into new user-facing UI.
- `RunStaticDataSet` has minimal `QuarterData` seed data while `RunCalendar` owns the full MVP schedule. If future work needs quarter goals for all playable quarters, reconcile those two sources deliberately.
