# 주식 규칙 대개편 PRD

Status: ready-for-agent

이 PRD는 주식 규칙 대개편 기획을 구현 가능한 제품 요구사항으로 정리한 문서다. 목적은 기존 MVP의 구현 상태를 전제로, 게임의 정체성을 "주식만 포트폴리오에 담는 카드 엔진 빌딩 게임"으로 수렴시키는 것이다.

## Problem Statement

현재 게임은 자산군, 전문 자원, 중앙 은행, 카드 상세보기, 예약 구역, 3구역 시장 테이프 등 여러 개념이 함께 존재해 플레이어 판타지가 흐려지고 구현 표면도 넓다.

플레이어 입장에서는 "무엇을 사서 포트폴리오를 키우는 게임인지"가 즉시 선명해야 한다. 채권, 원자재, 부동산 같은 넓은 자산군보다는 주식 하나로 카드 수집과 엔진 성장의 감각을 집중시키는 편이 더 이해하기 쉽다.

또한 기존 시장 구조는 3x3 구역과 상세보기/유동성 확보 상태를 오가며 카드 흐름이 복잡해진다. MVP가 이미 상당히 개발된 상태이므로, 새 규칙은 기존 구현을 전면 폐기하는 방식이 아니라 핵심 플레이 루프를 유지하면서 용어, 카드 타입, 시장 테이프, 예약, 포트폴리오 제한, 수익/파산 규칙을 정리하는 방향이어야 한다.

이번 개편이 해결해야 하는 핵심 문제는 다음과 같다.

- 포트폴리오의 대상이 주식 하나로 명확해야 한다.
- 기존 자산/전문 자원/환매 압력 계열 용어를 새 테마에 맞게 교체해야 한다.
- 중앙 은행과 자원 확보 상태를 제거하고, 자원 획득을 시장 카드 구매로 통합해야 한다.
- 시장 테이프를 1x8 구조로 단순화하면서도 예약과 카드 흐름의 긴장감을 유지해야 한다.
- 포트폴리오에 8칸 제한을 두어 매수, 매도, 호일 완성 선택이 의미 있어야 한다.
- 동일 주식 3장을 모으면 호일 주식으로 합쳐지는 수집 목표가 있어야 한다.
- 월세 밀림과 파산 규칙이 코믹하지만 명확한 실패 압력으로 작동해야 한다.
- 이미 개발된 MVP 이슈와 충돌하지 않도록 새 구현 작업은 변경분 중심으로 추적 가능해야 한다.

## Solution

게임을 "주식 전용 포트폴리오 빌딩"으로 재정의한다. 플레이어는 1x8 시장에서 주식 카드와 소모형 자원 카드를 구매하고, 보유 주식의 배당금과 주식 매도 수익으로 분기 목표 수익을 달성한다. 목표를 놓치거나 주식을 예약하면 월세 밀림이 증가하며, 월세 밀림이 10에 도달하면 파산으로 게임 오버된다.

핵심 변경은 다음과 같다.

- 포트폴리오에는 주식만 들어간다.
- "전문 자원"은 "투자 철학"으로 바뀌며, 종류는 독서/명상/인내다.
- 투자 철학은 총 10개, 각 종류별 5개까지 보유할 수 있다.
- 딜은 유지하며, 예약 보상으로 지급된다.
- 딜은 주식 매수 비용 슬롯 대체와 현금 할인에만 사용된다.
- 중앙 은행은 제거한다.
- 현금 및 투자 철학 획득은 시장에 등장하는 소모형 자원 카드 구매로 처리한다.
- 시장은 1x8 테이프이며, 오른쪽에서 카드가 들어오고 왼쪽으로 이동해 사라진다.
- 시장 테이프는 새 영업일 시작 시 진행되고, 새 분기 시작 시 갱신된다.
- 분기 첫 영업일에는 갱신만 하고 진행하지 않는다.
- 빈칸은 항상 시장 테이프 당김으로 메운다.
- 예약은 별도 구역이 아니라 시장 슬롯의 주식 잠금이다.
- 예약된 주식은 진행, 당김, 갱신의 영향을 받지 않는다.
- 최대 3개의 주식을 동시에 예약할 수 있다.
- 포트폴리오는 최대 8칸이다.
- 동일 주식 3장을 보유하면 즉시 호일 주식 1장으로 합쳐진다.
- 호일 완성 후 시장과 덱에 남은 같은 종목 주식은 모두 이번 게임에서 제거된다.
- 주식 매도는 영업일을 소비하지 않으며, 일반 주식은 현금 1 x 인플레이션, 호일 주식은 현금 3 x 인플레이션을 지급한다.
- 주식 매도 수익은 분기 목표 판정에 쓰이는 수익에 포함한다.
- 카드 상세보기 화면은 제거하고, 카드 호버 확대만 제공한다.
- 최종 평가는 보유 주식의 최종 가치 기준으로 결정한다.

이 PRD는 새 구현 이슈를 바로 생성하지 않는다. 후속 `to-issues` 작업을 할 경우, 기존 MVP 이슈와 대조해 이미 완료된 영역은 제외하고 변경분 중심의 이슈로 쪼개야 한다.

## User Stories

1. As a player, I want the portfolio to contain only stocks, so that the game fantasy is immediately clear.
2. As a player, I want every stock card to show image, name, grade, value, cost, and dividend, so that I can compare market choices quickly.
3. As a player, I want consumable resource cards to show image, grade, provided resource, and cost, so that I can understand their immediate effect without treating them like portfolio cards.
4. As a player, I want consumable resource cards to have no display name, so that they read as tactical supply cards rather than collectable stocks.
5. As a player, I want Reading, Meditation, and Patience chips to replace the old professional resources, so that the resource fantasy feels like investment philosophy.
6. As a player, I want each investment philosophy type capped at 5, so that one resource cannot grow without limit.
7. As a player, I want total investment philosophy capped at 10, so that resource management stays tight.
8. As a player, I want excess investment philosophy gains to discard only the overflow, so that I keep existing resources while understanding the cap.
9. As a player, I want cash and deal to remain outside the investment philosophy cap, so that funding and philosophy have distinct roles.
10. As a player, I want deal to remain available, so that reservation still gives a useful tactical reward.
11. As a player, I want deal to substitute stock purchase philosophy slots and reduce base cash cost, so that it helps me buy hard-to-afford stocks.
12. As a player, I want deal to be unusable on consumable resource cards, so that resource cards remain simple cash purchases.
13. As a player, I want the central bank action removed, so that resource acquisition happens through the same market I am already watching.
14. As a player, I want cash gain cards to appear in the market, so that I can recover funding through visible opportunities.
15. As a player, I want investment philosophy gain cards to appear in the market, so that I can plan purchases around resource supply.
16. As a player, I want consumable resource cards to disappear after purchase, so that their role is immediate and tactical.
17. As a player, I want buying a consumable resource card to consume a business day, so that taking resources has the same time pressure as buying a stock.
18. As a player, I want the market to be a single 1x8 tape, so that card flow is easier to read.
19. As a player, I want cards to enter from the right and move left over time, so that opportunities feel temporary.
20. As a player, I want the market tape to progress at the start of a new business day, so that time passing visibly changes the market.
21. As a player, I want the market tape to refresh at the start of each quarter, so that each quarter opens with a new market context.
22. As a player, I want the first business day of a quarter to refresh without also progressing, so that quarter start behavior is predictable.
23. As a player, I want empty market slots to be filled by market tape pull, so that market gaps never linger awkwardly.
24. As a player, I want multiple empty market slots to be pulled from the leftmost empty slot in order, so that refill behavior is deterministic.
25. As a player, I want reserved stocks to remain fixed during tape progress, pull, and refresh, so that reservation feels reliable.
26. As a player, I want non-reserved cards to skip over reserved stocks when moving left, so that locked cards do not break market flow.
27. As a player, I want the market to draw new cards from separate stock and consumable resource decks, so that late-game consumable density does not drift unpredictably from a shared deck.
28. As a player, I want each new market draw to choose stock deck with 75% probability and consumable resource deck with 25% probability, so that the market mostly remains about stocks.
29. As a player, I want the opposite deck to be used if the selected deck is empty, so that the market can keep functioning.
30. As a player, I want consumable resource cards to recycle when appropriate, so that resource supply can continue after the deck is used.
31. As a player, I want removed, sold, and foil-removed stocks to never return to the stock deck, so that stock scarcity matters.
32. As a player, I want an explicit exception state if both decks cannot provide a card, so that the market never silently leaves a slot empty.
33. As a player, I want reservation to lock a stock in its current market slot, so that I can protect a future buy without moving it to a separate area.
34. As a player, I want only stock cards to be reservable, so that consumable resource cards remain immediate market choices.
35. As a player, I want to reserve up to 3 stocks, so that I can plan without freezing the entire market.
36. As a player, I want reserving a stock to give me 1 deal, so that reservation has a positive tactical payoff.
37. As a player, I want reserving a stock to add 1 월세 밀림, so that locking opportunities carries comic risk.
38. As a player, I want reservation to consume a business day, so that protecting a card has a real cost.
39. As a player, I want buying a reserved stock to release its reservation and pull the market, so that the locked slot returns to normal flow.
40. As a player, I want foil completion to remove matching reserved stocks too, so that the game respects the "this stock is complete" state everywhere.
41. As a player, I want a maximum of 8 portfolio slots, so that I must manage space.
42. As a player, I want two copies of the same stock to occupy two slots, so that collecting toward foil still creates pressure.
43. As a player, I want buying a ninth non-foil stock to be blocked with a sale-needed message, so that I understand why the purchase failed.
44. As a player, I want blocked purchases to spend no cost and no business day, so that invalid attempts do not punish me.
45. As a player, I want buying the third copy of a stock to be allowed even when the portfolio is full, so that foil completion is never blocked by the temporary third card.
46. As a player, I want three matching stocks to immediately merge into one foil stock, so that the collection payoff is instant.
47. As a player, I want the earliest acquired matching stock slot to become the foil, so that merge placement is deterministic.
48. As a player, I want the two other merged slots to remain empty, so that portfolio holes reflect the merge result.
49. As a player, I want new stocks to enter the leftmost empty portfolio slot, so that filling behavior is predictable.
50. As a player, I want foil stocks to keep the same grade but have designer-authored value and dividend, so that foil power can be balanced intentionally.
51. As a player, I want foil completion to remove the same stock from market and deck, so that completed stocks feel truly retired from the run.
52. As a player, I want stock deck copies per stock to be data-driven by min/max values, so that designers can tune stock frequency.
53. As a player, I want stock buying to consume a business day, so that growing the portfolio advances time.
54. As a player, I want stock sale to be available multiple times per day, so that I can free space and cover revenue gaps.
55. As a player, I want stock sale to not consume a business day, so that selling is a tactical correction rather than a full turn.
56. As a player, I want selling a normal stock to grant cash equal to 1 times inflation, so that sale has a small but reliable payoff.
57. As a player, I want selling a foil stock to grant cash equal to 3 times inflation, so that foil sale feels meaningfully larger.
58. As a player, I want stock sale revenue to count toward revenue, so that selling can help me barely meet a quarterly target.
59. As a player, I want sold stocks to be removed from the run, so that selling has a lasting opportunity cost.
60. As a player, I want owned stocks to pay dividends at business day start, so that my portfolio feels like an income engine.
61. As a player, I want dividends to count as revenue, so that portfolio growth supports quarterly goals.
62. As a player, I want revenue to include dividends, stock sale revenue, and quarter-end settlement revenue, so that the quarter target reflects investment performance.
63. As a player, I want cash gained from consumable resource cards to be funding cash, not revenue, so that resource cards do not directly solve quarterly goals.
64. As a player, I want 월세 밀림 to replace the old pressure term, so that failure pressure feels comic and immediately understandable.
65. As a player, I want 월세 밀림 to reach bankruptcy at 10, so that the loss condition is clear.
66. As a player, I want reservation and quarter failure to increase 월세 밀림, so that both tactical greed and poor performance create risk.
67. As a player, I want bankruptcy to trigger immediately when 월세 밀림 fills, so that the failure state is decisive.
68. As a player, I want card details to appear on hover as a larger version of the same card, so that I can inspect information without changing screen state.
69. As a player, I want the market area to have one normal state, so that buying, reserving, and hovering feel direct.
70. As a player, I want the final settlement to score final value from owned stocks only, so that the end result rewards the portfolio I built.
71. As a player, I want final comments to consider both rating and 월세 밀림, so that the game recognizes the style of my run.
72. As a designer, I want stock value, dividend, foil value, foil dividend, cost, and deck copy counts to be data-driven, so that balance changes do not require rule rewrites.
73. As a designer, I want consumable resource card costs, rewards, rarity, and recycle behavior to be data-driven, so that market supply can be tuned.
74. As a designer, I want market draw weights to be configurable, so that the 75/25 ratio can be tuned if playtests require it.
75. As a developer, I want the overhaul tracked as a separate PRD from the original MVP, so that already-completed MVP work can be mapped rather than duplicated.

## Implementation Decisions

- Treat this overhaul as a delta over the existing MVP, not a fresh rewrite by default.
- Use "stock" as the only portfolio card domain. Non-stock asset classes are out of the game model for MVP.
- Use "investment philosophy" as the grouping term for Reading, Meditation, and Patience.
- Keep cash and deal as separate resources.
- Keep deal behavior: one deal replaces one investment philosophy slot and reduces base cash cost by 1 before inflation.
- Keep deal max at 3. If reservation would grant deal at cap, the reservation succeeds and extra deal is discarded.
- Enforce investment philosophy caps at both total and per-type levels.
- Replace central bank resource acquisition with market-bought consumable resource cards.
- Model market cards as two types: stock and consumable resource.
- Model market supply with two decks: stock deck and consumable resource deck.
- Draw each new market card using a soft 75% stock / 25% consumable resource selection.
- Allow fallback to the opposite deck if the selected deck has no drawable card.
- Recycle only eligible consumable resource cards back into the consumable resource deck.
- Do not return removed, sold, or foil-completed stocks to the stock deck.
- Preserve an explicit "no drawable card" exception state for impossible MVP exhaustion cases.
- Replace the 3-zone market with a single 1x8 market tape.
- Make market tape progress a business-day-start behavior.
- Make market tape refresh a quarter-start behavior.
- Make quarter first business day refresh-only, with no additional tape progress.
- Apply market tape pull to all empty-slot cases.
- Keep reserved stocks fixed during progress, pull, and refresh.
- Store reservation as state on the market slot rather than as a separate reservation area.
- Restrict reservation to stock cards.
- Reserve action grants deal, increases 월세 밀림, checks bankruptcy, and consumes a business day.
- Buying a reserved stock releases the reservation and fills the resulting empty slot via market tape pull.
- Use a portfolio slot model with maximum 8 occupied stock slots.
- Block non-foil stock purchases when the portfolio is full.
- Allow a purchase that immediately completes a foil even when the portfolio is full.
- Merge three owned copies of the same stock into one foil stock immediately.
- Use the earliest acquired matching stock slot as the foil result slot.
- Leave the two consumed portfolio slots empty after foil completion.
- Remove all same-stock cards from market and stock deck after foil completion.
- Use data-authored foil value and foil dividend rather than multipliers.
- Allow stock sale multiple times per day without consuming a business day.
- Include stock sale revenue in revenue tracking.
- Remove sold stocks from the run permanently.
- Remove card detail screen state and central bank/resource acquisition screen state.
- Use hover enlargement for card inspection while keeping card information identical at market and hover scales.
- Define revenue as the cumulative value used for quarter targets: dividends, stock sale revenue, and quarter-end settlement revenue.
- Define dividend as the owned-stock business-day-start cash payout.
- Treat consumable cash gain as funding cash, not revenue.
- Rename failure pressure to 월세 밀림 and bankruptcy to 파산.
- Use 월세 밀림 10 as the bankruptcy threshold.
- Score final rating by final value from owned stocks only.
- Use final rating and 월세 밀림 level together to select final run comment.
- Recommended deep modules for implementation are market tape movement, market deck draw/recycle, portfolio/foil resolution, payment/deal validation, revenue tracking, rent arrears bankruptcy, and card presentation state.

## Testing Decisions

- Tests should validate externally observable behavior rather than internal helper details.
- Market tape tests should cover progress, refresh, pull, reserved-slot skipping, multiple empty slots, and rightmost refill behavior.
- Market deck tests should cover 75/25 weighted selection at the decision boundary, fallback behavior, consumable recycle, and non-returning stock removal.
- Reservation tests should cover stock-only reservation, max 3 reservation cap, deal grant at cap, 월세 밀림 increase, bankruptcy check, and reserved stock purchase release.
- Portfolio tests should cover 8-slot blocking, full-portfolio foil exception, merge placement, empty slot preservation, leftmost empty insertion, and same-stock removal from market/deck.
- Payment tests should cover cash cost, investment philosophy cost, deal substitution, deal cash discount before inflation, and consumable resource card payment without deal.
- Resource tests should cover total investment philosophy cap 10, per-type cap 5, overflow discard, and cash/deal exclusion from philosophy caps.
- Revenue tests should cover dividend revenue, stock sale revenue, quarter-end settlement revenue, and exclusion of consumable resource cash from revenue.
- Rent arrears tests should cover reservation increase, quarter-failure increase tiers, threshold 10 bankruptcy, and immediate game-over behavior.
- Card presentation tests should cover stock card fields, consumable resource card fields, no consumable display name, and hover enlargement without changing card information.
- Flow tests should cover business-day-consuming actions, non-consuming sale action, quarter-start refresh-only behavior, and final settlement routing.
- Existing MVP tests can be adapted where they already cover purchase payment, market tape display, resource HUD, and main game shell behavior.
- Final verification should include the relevant EditMode suites for rule modules and PlayMode/UI checks for card hover, market interaction, portfolio capacity, reservation, and bankruptcy/final settlement routing.

## Out of Scope

- Creating implementation issues in this PRD step.
- Automatically closing, rewriting, or reclassifying existing MVP issues.
- Adding non-stock asset classes such as bonds, commodities, or real estate.
- Reintroducing the central bank action or resource acquisition screen.
- Reintroducing the card detail screen state.
- Reintroducing a separate reservation zone.
- Guaranteeing an exact 6:2 market composition at all times.
- Treating consumable resource card cash as revenue.
- Making foil stocks separate cards in the stock deck.
- Returning sold or foil-removed stocks to the stock deck.
- Advanced reporting such as card-by-card revenue contribution, fiscal-year detail drilldowns, or full run analytics.
- Advanced animation, tutorial, or visual polish beyond what is required to make the new rules understandable.

## Further Notes

- Source planning basis: `plan/22_stock_rules_overhaul.md`.
- This PRD is intended to live separately from the existing MVP PRD because the MVP issue set is already substantially implemented.
- Follow-up issue generation should first compare existing MVP issues against this PRD and create only delta issues for work that is new, changed, or invalidated by the overhaul.
- Existing filenames in `plan/` may still contain old terms for compatibility, but user-facing terminology should follow this PRD.
- If future implementation discovers a conflict between older MVP documents and this PRD, the stock rules overhaul should take priority for this feature.
