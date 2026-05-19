# MVP 구현 PRD

Status: ready-for-agent

이 PRD는 기획 문서 세트를 MVP 구현 관점으로 압축한 문서이다. 목표는 모든 연출과 밸런스를 완성하는 것이 아니라, 자산 운용 로그라이트의 한 런이 처음부터 실패 또는 최종 정산까지 규칙대로 진행되는 상태를 만드는 것이다.

> 2026-05-19 implementation note: 주식 규칙 대개편 기준이 이 문서의 옛 카드 상세보기/예약 구역/자원 확보 UI 규칙을 대체한다. 현재 예약은 별도 `Reservation Panel` 없이 시장 슬롯 주식 잠금으로 표시하고, 보유 주식 매도 버튼은 `Owned Stock Card 1~8 Card Button` 호버 중에만 표시한다. Card Button과 Sell Button 사이로 커서를 이동하는 동안에는 표시를 유지하고, 둘 다 벗어나면 숨기며, Card Button 클릭은 표시 조건이 아니다.

## Problem Statement

플레이어는 제한된 영업일 안에서 자산을 매수하고, 카드를 예약하고, 자원을 확보하며, 분기 목표 운용 수익을 달성해야 한다. 현재 기획은 시스템별 세부 문서로 충분히 풀려 있지만, 구현을 시작하려면 어떤 범위를 1차 MVP로 묶을지, 어떤 모듈을 우선 분리할지, 어떤 규칙을 테스트로 고정할지 한 문서로 정리되어야 한다.

MVP에서 해결해야 하는 핵심 문제는 다음이다.

- 영업일, 분기, 회계년도가 정해진 구조대로 진행되어야 한다.
- 시장 테이프가 표시되고, 진행과 갱신과 슬롯 보충이 서로 다르게 동작해야 한다.
- 플레이어가 시장 카드와 예약 상태의 시장 슬롯을 검토하고 매수할 수 있어야 한다. 주식 규칙 대개편 이후 별도 카드 상세보기 화면과 `Reservation Panel`은 사용하지 않는다.
- 플레이어가 시장 카드를 예약해 딜을 얻고 환매 압력을 감수할 수 있어야 한다.
- 인플레이션이 매수 현금 비용에 반영되어, 분기별 비용 압박이 플레이 판단에 들어가야 한다.
- 플레이어가 자원 확보로 현금과 전문 자원을 얻되, 운용 수익과 조달 현금이 섞이지 않아야 한다.
- 분기 마감에서 운용 수익, 분기 목표, 환매 압력을 판정해야 한다.
- 환매 압력 10 이상 도달 시 즉시 런 실패가 발생해야 한다.
- 3회계년도 4Q까지 실패 없이 완료하면 최종 정산을 표시해야 한다.

## Solution

1차 MVP는 "처음부터 끝까지 플레이 가능한 규칙 중심 런"을 구현한다.

플레이어는 새 런을 시작하면 1회계년도 1Q부터 플레이한다. 각 플레이 분기에서는 제한된 영업일 동안 시장 카드 매수, 시장 카드 예약, 자원 확보, 다음 영업일을 선택한다. 마지막 영업일이 끝나면 분기 마감으로 들어가 분기 운용 수익과 목표 달성률을 판정한다. 목표 미달 또는 예약으로 환매 압력이 누적되고, 환매 압력이 10 이상이면 즉시 실패한다. 1·2회계년도 3Q 이후에는 4Q 휴가 요약 화면을 거쳐 다음 회계년도로 넘어가며, 3회계년도 4Q 이후에는 최종 정산으로 이동한다.

MVP는 다음 시스템을 포함한다.

- 시간 구조: 3개 회계년도, 10개 플레이 분기, 총 44영업일
- 영업일 흐름: 영업일 시작 운용 수익, 플레이어 입력, 행동 확정, 영업일 종료
- 시장 영역 3상태: Market, CardDetail, GainLiquidity
- 시장 테이프: 매도 임박, 현재 시장, 예비 시장
- 자원 구조: 현금, 리서치, 신용, 원자재, 딜
- 자산 카드: 운용가치, 운용 수익, 비용, 태그, 희귀도
- 카드 상세보기와 매수 결제
- 인플레이션: 현재 값, 현금 비용 적용, 비용 표시
- 칩 조작 최소 UX: 클릭 배치, 회수, 딜 배치, 비용 슬롯 표시
- 예약 시스템과 딜
- 자원 확보
- 추가 매수권
- 분기 마감과 환매 압력
- 4Q 휴가
- 최종 정산과 실패 화면
- 데이터 테이블과 런타임 상태

1차 MVP의 완료 기준은 다음이다.

- 새 런을 시작할 수 있다.
- 1회계년도 1Q부터 3회계년도 4Q까지 진행할 수 있다.
- 영업일이 행동 확정 시점에만 소비된다.
- 자산을 매수하고 보유 자산의 영업일 시작 현금을 받을 수 있다.
- 인플레이션이 매수 최종 현금 비용에 적용된다.
- 자원 확보로 자원을 얻을 수 있다.
- 시장 카드를 예약하고, 딜과 환매 압력을 처리할 수 있다.
- 예약된 주식을 시장 슬롯에 유지하고 나중에 매수할 수 있다.
- 보유 주식 Card Button 호버로 Sell Button을 표시하고, Sell Button 클릭으로 주식을 매도할 수 있다.
- 분기 마감에서 목표 달성률과 환매 압력을 계산한다.
- 환매 압력 10 이상이면 최종 정산이 아니라 실패 화면으로 간다.
- 실패하지 않고 3회계년도 4Q를 마치면 최종 정산이 표시된다.

## User Stories

1. As a player, I want to start a new run, so that I can begin managing a portfolio from 1회계년도 1Q.
2. As a player, I want to see the current 회계년도, 분기, and remaining 영업일, so that I understand my time pressure.
3. As a player, I want each playable 분기 to have a fixed number of 영업일, so that I can plan actions around a known limit.
4. As a player, I want 1·2회계년도 to skip 4Q as 휴가, so that the run has a clear annual rhythm.
5. As a player, I want 3회계년도 4Q to be playable, so that the final year has a complete closing push.
6. As a player, I want each 영업일 to start by applying owned asset income, so that earlier purchases feel like an engine.
7. As a player, I want card viewing to be free, so that I can inspect market choices without spending a 영업일.
8. As a player, I want closing card detail to be free, so that backing out of an uncommitted decision is safe.
9. As a player, I want purchase confirmation to consume a 영업일, so that buying assets has a real opportunity cost.
10. As a player, I want reservation confirmation to consume a 영업일, so that reserving strong cards has a cost.
11. As a player, I want liquidity to consume a 영업일 only after I gain the first resource, so that opening the liquidity view is reversible.
12. As a player, I want to skip action with the next 영업일 button, so that waiting for income is a valid choice.
13. As a player, I want the market area to show only one of Market, CardDetail, or GainLiquidity, so that I never face overlapping states.
14. As a player, I want the next 영업일 button active only in Market state, so that I cannot accidentally advance while resolving another action.
15. As a player, I want a market tape with 매도 임박, 현재 시장, and 예비 시장 zones, so that I can anticipate card flow.
16. As a player, I want market card purchase to refill only the purchased slot, so that buying does not unexpectedly move the whole market.
17. As a player, I want market card reservation to refill the reserved slot and then advance the market tape, so that reservation changes market tempo.
18. As a player, I want fiscal year start to refresh the market tape, so that each new year feels like a new market.
19. As a player, I want next quarter within the same fiscal year to advance the market tape, so that time passing changes opportunities.
20. As a player, I want reserved cards to survive market tape advance and refresh, so that reservation feels reliable.
21. As a player, I want owned, reserved, removed, and currently visible cards not to duplicate in new market slots, so that card state remains credible.
22. As a player, I want asset cards to show cash cost, professional resource cost, management value, income, tags, and rarity, so that I can compare choices.
23. As a player, I want only owned assets to count toward management value, so that final scoring is tied to completed purchases.
24. As a player, I want reserved cards not to generate income, so that reservation is not confused with ownership.
25. As a player, I want reserved cards not to count in quarter settlement, so that settlement uses only owned assets.
26. As a player, I want chip slots for professional resource costs, so that asset buying feels like a board-game transaction.
27. As a player, I want placed chips to remain unspent until purchase confirmation, so that I can adjust payment before committing.
28. As a player, I want closing card detail to return placed chips, so that canceling a purchase never costs resources.
29. As a player, I want deal chips to replace any professional resource slot, so that deals function as flexible resources.
30. As a player, I want each used deal chip to reduce base cash cost by 1, so that deal timing matters.
31. As a player, I want deal chips consumed only on purchase confirmation, so that tentative placement is reversible.
32. As a player, I want to reserve a market stock in its current market slot, so that I can save a valuable future purchase without moving it to a separate reservation panel.
33. As a player, I want reservation to give deal +1, so that reservation has an immediate tactical upside.
34. As a player, I want reservation to increase redemption pressure +1, so that reservation has a risk cost.
35. As a player, I want reservation to remain possible even when deal is already 3/3, so that the card-saving effect is not blocked by deal cap.
36. As a player, I want extra deal above 3 to be discarded with feedback, so that the cap is understandable.
37. As a player, I want reservation disabled when three market stocks are already reserved, so that impossible reservation does not consume a 영업일.
38. As a player, I want to buy reserved cards later, so that reservation can become an owned asset.
39. As a player, I want reserved stock purchase to release its market-slot reservation and fill the empty market slot through market tape pull.
40. As a player, I want liquidity to offer cash, research, credit, and commodity, so that I can prepare for future purchases.
41. As a player, I want liquidity to complete with two of the same resource or three different resources, so that resource gathering has a simple rule.
42. As a player, I want professional resources capped at a combined 10, so that resource hoarding is limited.
43. As a player, I want cash and deal excluded from the professional resource cap, so that each resource type keeps a distinct role.
44. As a player, I want liquidity cash to be funding cash, not earned cash, so that quarter goals cannot be cleared by simply gathering cash.
45. As a player, I want asset income and quarter settlement cash to count as earned cash, so that operational success is measured correctly.
46. As a player, I want extra buy actions to allow one more asset purchase in the same 영업일, so that some cards can create tempo spikes.
47. As a player, I want extra buy actions limited to asset purchase only, so that I cannot use them for reservation or liquidity.
48. As a player, I want extra buy actions not to stack or carry into the next 영업일, so that the rule remains readable.
49. As a player, I want quarter end to calculate settlement income before goal achievement, so that all quarter performance is included.
50. As a player, I want failed quarter goals to increase redemption pressure by achievement band, so that partial success matters.
51. As a player, I want redemption pressure checked immediately after it increases, so that failure timing is consistent.
52. As a player, I want a failure screen when redemption pressure reaches 10, so that I clearly understand why the run ended.
53. As a player, I want 4Q vacation screens to summarize the year without rewards or penalties, so that annual transition is clean.
54. As a player, I want final settlement to show final management value, grade, total earned cash, owned asset count, redemption pressure, and comment, so that I understand my run result.
55. As a player, I want final grade based only on final management value, so that redemption pressure affects commentary but not grade.
56. As a designer, I want card costs, income, management value, tags, quarter goals, ratings, market slots, and comments to be table-driven, so that balance can be tuned without rewriting rules.
57. As a developer, I want runtime state separated from static data, so that rules can be tested and later saved cleanly.
58. As an implementation agent, I want deep modules with narrow interfaces, so that each rule cluster can be tested in isolation before UI integration.
59. As a player, I want current inflation reflected in final purchase cash cost, so that expensive market periods change my buying decisions.
60. As a designer, I want MVP inflation values to be table-driven, so that the pressure curve can be tuned without changing purchase rules.

## Implementation Decisions

- Use the project vocabulary consistently: 영업일, 분기, 회계년도, 운용가치, 운용 수익, 조달 현금, 환매 압력, 시장 테이프, 예약 카드, 보유 자산.
- Do not use AUM in user-facing text, data field names, or domain terminology. Use ManagementValue / 운용가치.
- Do not use 턴 or 스테이지 in user-facing text. Use 영업일 and 분기.
- Implement the first MVP with reservation and deal chips included. They are central to the current gameplay. If scope must be cut again, reservation, deal, extra buy, and advanced chip UX are the candidates for a second MVP, but the preferred MVP includes reservation and deal.
- Include MVP inflation as a simple table-driven integer cash-cost modifier, not a full economic event system.
- Apply inflation only to cash cost, after cost modifiers and deal discounts. Reserved cards use the inflation value at purchase time, not reservation time.
- Treat additional buy as part of MVP rules, but make its priority dependent on whether the first MVP card pool actually contains a GrantExtraBuyAction-style effect.
- Keep UI layout and art exactness out of the core MVP. The MVP defines which controls exist, when they are visible/enabled, and what they do.
- Store balance-sensitive values in data tables: card costs, professional costs, management value, income, tags, rarity, quarter targets, inflation values, rating thresholds, redemption pressure levels, final comments, and market slot count per zone.
- Separate static data from runtime state. Static data defines cards, tags, quarters, inflation config, rating bands, comments, resource config, market config, and redemption pressure config. Runtime state tracks current resources, calendar, current inflation, performance, market tape, market-slot reservations, owned assets, card runtime state, business day state, and redemption pressure.

Proposed major modules:

- RunCalendar module: owns 회계년도, 분기, 영업일 counts, playable/vacation quarter decisions, next schedule routing, and 44-day total progression.
- BusinessDayFlow module: owns 영업일 start, income timing, action commitment, remaining-day decrement, end-of-day routing, and extra buy decision state.
- MarketAreaState module: owns the single active market area state: Market, CardDetail, or GainLiquidity, plus input gating such as next 영업일 availability.
- MarketTape module: owns market tape zones, advance, refresh, slot refill, duplicate prevention, and interaction with purchase/reservation.
- AssetCard module: owns static card data, runtime card state, ownership transition, management value calculation, income application, tags, and owned asset ordering.
- ResourceLedger module: owns cash, professional resources, deal count, earned cash vs funding cash, professional resource cap, deal cap, and resource add/spend operations.
- PurchasePayment module: owns payment slots, chip placement state, deal substitution, final cash cost calculation, validation, and transaction-like purchase confirmation.
- Inflation module: owns current inflation value lookup, quarter-start update, cash-cost modifier, and display-ready cost breakdown.
- Reservation module: owns market-slot reservation capacity, reserve action validation, market-slot lock state, deal gain, redemption pressure increase, and reserved stock purchase cleanup. It does not own a separate Reservation Panel.
- LiquidityAction module: owns liquidity entry, close conditions, selected resource sequence, valid next choices, completion, auto-end, and funding-cash handling.
- ExtraBuyAction module: owns extra buy grant, non-stacking, no carryover, input restrictions, second-purchase resolution, and GrantExtraBuyAction suppression during extra buy.
- QuarterSettlement module: owns settlement income, earned cash finalization, target achievement, redemption pressure increase amount, quarter result data, and next schedule handoff.
- RedemptionPressure module: owns pressure addition, max threshold, immediate failure, failure reason, and pressure level for final comments.
- FiscalYearSummary module: owns 4Q vacation summary data, no-reward/no-penalty vacation behavior, annual earned cash summary, and next fiscal year start.
- FinalSettlement module: owns final management value, final grade selection, total earned cash, owned asset count, redemption pressure level, management comment, and final summary.
- FeedbackMessage module: owns short feedback messages such as resource cap reached, discarded deal, max reserved stocks reached, invalid chip placement, and insufficient purchase conditions.

Suggested deep modules to test first:

- RunCalendar: a small interface can answer the current period, business days for a quarter, vacation detection, and next schedule.
- MarketTape: a small interface can advance, refresh, refill, and draw without duplicate card states.
- ResourceLedger: a small interface can add funding cash, add earned cash, add capped resources, add capped deal, and spend confirmed payment.
- PurchasePayment: a small interface can create payment slots, place/remove chips, calculate final cash cost, and validate/commit a purchase.
- Inflation: a small interface can return the current cash-cost modifier and apply it after deal discount.
- LiquidityAction: a small interface can evaluate valid choices and completion from a selected resource sequence.
- RedemptionPressure: a small interface can add pressure and return whether run failure is triggered.
- QuarterSettlement: a small interface can calculate achievement rate, pressure increase, and quarter result.
- FinalSettlement: a small interface can calculate final value, select grade, select pressure level, and select comment.

Implementation order:

1. Data model, enums, runtime state, and table shape.
2. ResourceLedger and earned-cash/funding-cash separation.
3. RunCalendar and BusinessDayFlow.
4. MarketTape state and card draw/refill behavior.
5. AssetCard static/runtime data and owned asset behavior.
6. MarketAreaState and CardDetail state.
7. PurchasePayment, inflation modifier, and minimal chip placement.
8. LiquidityAction.
9. Reservation.
10. Deal chip behavior.
11. ExtraBuyAction if supported by MVP card effects.
12. QuarterSettlement.
13. RedemptionPressure and failure screen.
14. FiscalYearSummary / 4Q vacation.
15. FinalSettlement.
16. UI feedback and message polish.

## Testing Decisions

Good tests for this MVP should check external behavior and state transitions, not internal implementation details. Prefer asserting final state, emitted result data, and visible action availability over asserting private helper calls. The most valuable tests are pure rule tests around deep modules, followed by integration tests that exercise a full 영업일, 분기, and run path.

There is no existing source or test suite in this repository yet; the first implementation should establish the test style with isolated rule tests for calendar, market tape, resource ledger, payment, liquidity, reservation, quarter settlement, redemption pressure, and final settlement.

Core rules to test:

- Calendar: 1회계년도 and 2회계년도 have 1Q~3Q playable and 4Q vacation.
- Calendar: 3회계년도 has 1Q~4Q playable.
- Calendar: 1·2회계년도 playable quarters have 4영업일 each.
- Calendar: 3회계년도 playable quarters have 5영업일 each.
- Calendar: total playable quarters are 10 and total playable 영업일 are 44.
- Calendar: 1·2회계년도 3Q 마감 leads to 4Q vacation, then next 회계년도 1Q.
- Calendar: 3회계년도 4Q 마감 leads to final settlement if no failure occurred.
- Business day: card click and card detail close do not consume 영업일.
- Business day: purchase confirmation consumes exactly one 영업일 unless extra buy keeps the day open.
- Business day: reservation click consumes exactly one 영업일 when reservation is valid.
- Business day: opening liquidity does not consume 영업일.
- Business day: first liquidity resource click commits the action and prevents closing.
- Business day: next 영업일 is enabled only in Market state.
- Business day: last 영업일 ending enters quarter settlement without starting another income step.
- Market area: Market, CardDetail, and GainLiquidity are mutually exclusive.
- Market tape: all zones use the same slot count per zone. MVP default is 3 cards each for 매도 임박, 현재 시장, and 예비 시장.
- Market tape: advance removes every card in the 매도 임박 zone, moves every 현재 시장 card to 매도 임박, moves every 예비 시장 card to 현재 시장, and refills every 예비 시장 slot.
- Market tape: refresh removes current market tape cards and rebuilds all zones.
- Market tape: fiscal year start refreshes, same-fiscal-year next quarter advances.
- Market tape: 4Q vacation performs no advance or refresh.
- Market tape: market card purchase refills only the purchased slot and does not advance.
- Market tape: reserved stock purchase releases the market-slot reservation, empties that market slot, and fills it through market tape pull.
- Market tape: market card reservation locks the selected stock in place and does not immediately advance the tape.
- Market tape: owned, reserved, removed, and already visible cards are excluded from new draws.
- Asset card: only owned assets contribute to management value.
- Asset card: reserved and market cards do not contribute to management value.
- Asset card: owned asset income occurs at the next 영업일 start, not immediately on purchase.
- Resource ledger: research + credit + commodity cannot exceed 10.
- Resource ledger: cash and deal are excluded from professional resource cap.
- Resource ledger: professional resource cap overflow discards only newly gained overflow, not existing holdings.
- Resource ledger: deal cannot exceed 3.
- Resource ledger: reservation is still allowed at deal 3/3, and only extra deal is discarded.
- Resource ledger: liquidity cash increases current cash but not earned cash.
- Resource ledger: asset income and quarter settlement cash increase current cash and earned cash records.
- Purchase: validation failure spends no resources, changes no card state, refills no market slot, and does not release a market-slot reservation.
- Purchase: placed chips do not spend resources before confirmation.
- Purchase: closing card detail returns all placed chips and consumes no 영업일.
- Purchase: deal can fill any professional resource slot.
- Purchase: each used deal reduces base cash cost by 1, with minimum cash cost 0.
- Purchase: deal discount is applied before the current inflation modifier.
- Purchase: inflation applies only to cash cost, after cost modifiers and deal discount.
- Purchase: MVP default inflation +0 leaves final cash cost unchanged.
- Purchase: reserved card purchase uses purchase-time inflation.
- Purchase: final cash cost is clamped at 0 after inflation.
- Purchase: market purchase records source as MarketTape and moves the card to owned assets.
- Purchase: reserved purchase records source as Reserved and moves the card to owned assets.
- Reservation: reservation is only available for market cards.
- Reservation: reservation button is hidden for reserved cards.
- Reservation: three existing reserved market stocks disable reservation and consume no 영업일.
- Reservation: reservation action has no confirmation state.
- Reservation: if chips are placed and reservation is valid, reservation auto-recovers chips and consumes no resources.
- Reservation: reserved cards survive 영업일, 분기, 회계년도, market advance, and market refresh.
- Reservation: reserved cards do not generate income or participate in settlement.
- Liquidity: two of the same basic resource completes liquidity.
- Liquidity: three different basic resources completes liquidity.
- Liquidity: invalid mixed sequences cannot be selected.
- Liquidity: at professional resource 10/10, only cash is selectable.
- Liquidity: at professional resource 9/10, one professional resource can be selected, then professional buttons are disabled.
- Liquidity: if no valid next choice remains before normal completion, liquidity auto-ends and shows the resource cap message.
- Liquidity: deal cannot be gained through liquidity.
- Extra buy: extra buy allows market and reserved card purchase only.
- Extra buy: reservation and liquidity are disabled during extra buy.
- Extra buy: extra buy does not stack.
- Extra buy: extra buy does not carry to the next 영업일.
- Extra buy: using extra buy ends the 영업일 immediately.
- Extra buy: GrantExtraBuyAction triggered by an extra buy purchase is ignored.
- Quarter settlement: settlement income is applied before target achievement is calculated.
- Quarter settlement: funding cash from liquidity is excluded from quarter earned cash.
- Quarter settlement: success at 100% or higher adds no redemption pressure and gives no extra reward.
- Quarter settlement: achievement 75% to less than 100% adds redemption pressure +1.
- Quarter settlement: achievement 50% to less than 75% adds redemption pressure +2.
- Quarter settlement: achievement less than 50% adds redemption pressure +3.
- Redemption pressure: reservation adds +1 and immediately checks failure.
- Redemption pressure: quarter failure pressure increase immediately checks failure.
- Redemption pressure: pressure 0 through 9 allows the run to continue.
- Redemption pressure: pressure 10 or higher sets run failed and prevents next day, next quarter, vacation, or final settlement.
- Vacation: 4Q vacation shows current management value, fiscal-year earned cash, quarter earned cash list, owned asset count, and redemption pressure.
- Vacation: 4Q vacation grants no reward, applies no penalty, and does no market tape processing.
- Vacation: vacation continue starts the next fiscal year and refreshes market tape.
- Final settlement: final settlement requires 3회계년도 4Q completion and redemption pressure below 10.
- Final settlement: final management value sums owned asset management value only.
- Final settlement: cash and reserved cards are excluded from final management value.
- Final settlement: final grade is selected by highest threshold not exceeding final management value.
- Final settlement: redemption pressure does not lower final grade.
- Final settlement: management comment is selected by final grade and redemption pressure level.
- Failure screen: redemption pressure failure displays "대규모 환매 발생" and does not display the normal final grade.

Suggested test layers:

- Pure unit tests for RunCalendar, ResourceLedger, MarketTape, PurchasePayment, LiquidityAction, RedemptionPressure, QuarterSettlement, and FinalSettlement.
- State-machine tests for MarketAreaState and BusinessDayFlow.
- Scenario tests for basic progression, purchase, liquidity, reservation, redemption failure, vacation transition, and final settlement.
- Minimal UI-facing tests for button visibility/enabled state where it encodes rules: next 영업일, reserve button, liquidity close, resource buttons, and extra buy restrictions.

MVP smoke scenarios:

1. 새 런 시작 → 1회계년도 1Q 시작 → 다음 영업일로 4일 진행 → 분기 마감 진입.
2. 시장 카드 클릭 → 카드 상세보기 → 칩 배치 → 매수 확정 → 보유 자산 추가 → 다음 영업일 시작 → 운용 수익 발생.
3. 중앙 은행 클릭 → 자원 확보 → 현금 선택 → 현금 선택 → 현금 +2 → 영업일 종료.
4. 시장 주식 예약 → 시장 슬롯에서 예약 상태 표시 → 딜 +1 → 환매 압력 +1 → 시장 테이프 즉시 진행 없음 → 영업일 종료.
5. 보유 주식 Card Button 호버 → Sell Button 표시 → Sell Button 클릭 → 주식 매도 → 현금/운용 수익 증가 → 영업일 소비 없음.
6. 환매 압력 9 → 시장 카드 예약 → 환매 압력 10 → 실패 화면.
7. 3회계년도 4Q 완료 → 환매 압력 10 미만 → 최종 정산 → 최종 운용가치 / 평가 / 총 운용 수익 표시.
8. 기본 현금 비용 5 → 딜 2개 사용 → 인플레이션 +1 → 최종 현금 비용 4 → 매수 확정 시 현금 4 차감.

## Out of Scope

- 최종 밸런스 수치 확정.
- 전체 카드 목록 완성.
- 최종 UI 배치 좌표, 색상, 애니메이션 완성도.
- 고급 칩 물리감, 고급 카드 연출, 고급 사운드.
- 튜토리얼 전체 시나리오.
- 시장 뉴스 시스템.
- 시장 뉴스나 이벤트로 변하는 동적 인플레이션.
- 복잡한 이벤트 시스템.
- 휴가 보너스 또는 휴가 패널티.
- 카드 매각 기능.
- 회계년도별/분기별 상세 리포트 화면.
- 카드별 상세 기여도 리포트.
- 실패 원인 상세 분석 화면.
- 라이브 밸런싱 정책.
- 세이브/로드 정책. 단, 런타임 상태 구조는 이후 세이브/로드가 가능하도록 분리한다.

## Further Notes

- PRD 작성 기준: 기존 plan 문서 세트의 MVP 범위와 시스템별 규칙을 종합했다.
- 구현 기준: Unity 클라이언트 구현을 염두에 두되, 이 PRD는 구체적인 파일 구조나 UI 좌표를 강제하지 않는다.
- 핵심 리스크: 운용 수익과 조달 현금이 섞이면 분기 목표와 최종 통계가 무너진다. ResourceLedger와 RunPerformanceState 성격을 초기에 확실히 분리해야 한다.
- 핵심 리스크: 인플레이션 적용 순서가 흔들리면 딜 가치와 매수 가능 판정이 달라진다. PurchasePayment는 비용 수정 효과, 딜 할인, 인플레이션, 최종 0 클램프 순서를 한 경로로 계산해야 한다.
- 핵심 리스크: 시장 테이프 진행, 갱신, 슬롯 보충이 섞이면 예약과 매수의 의사결정 비용이 흐려진다. MarketTape module을 독립적으로 먼저 테스트해야 한다.
- 핵심 리스크: 예약된 시장 슬롯과 보유 자산이 섞이면 운용가치, 운용 수익, 정산, 최종 평가가 모두 오염된다. 카드 런타임 상태 전환을 명확하게 제한해야 한다.
- 핵심 리스크: 환매 압력 한도 검사가 늦으면 실패해야 할 런이 다음 영업일, 휴가, 또는 최종 정산으로 진행될 수 있다. RedemptionPressure module은 pressure add와 failure check를 한 인터페이스로 묶는 것이 좋다.
- 초기 구현은 화려함보다 규칙 정확성을 우선한다. 화면은 플레이 가능한 최소 표시와 버튼 상태를 갖추고, 연출과 사운드는 이후 단계에서 보강한다.
