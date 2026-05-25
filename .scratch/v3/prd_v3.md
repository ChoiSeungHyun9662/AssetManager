# v3 재미 강화 PRD

Status: ready-for-agent

Triage: ready-for-agent

이 PRD는 v1 `.scratch/mvp`, v2 `.scratch/stock-rules-overhaul` 이후의 v3 개선 범위를 정의한다. v3는 전체 게임을 새로 정의하는 문서가 아니라, v2 주식 규칙 개편 위에 `태그`, `미션`, `Mr.Market 제안`을 얹어 재미의 방향을 강화하는 델타 PRD다. 이 문서와 v1/v2 문서가 충돌하면 v3 규칙을 우선한다.

## Problem Statement

현재 MVP/v2 규칙은 주식 카드 수집, 시장 테이프, 예약, 포트폴리오, 호일, 분기 평가라는 기본 골격을 갖추고 있다. 그러나 플레이어 관점에서 "이번 판에 어떤 빌드를 노릴지", "왜 지금 이 카드를 사야 하는지", "고점이 터졌다는 감각이 어디서 오는지"가 아직 약하다.

카드마다 고유 효과를 붙이면 각 카드의 텍스트와 예외가 늘어나 게임이 지나치게 복잡해진다. 따라서 v3는 주식 카드 자체 효과를 추가하지 않고, 카드 위에 얹히는 분류와 시장 제안만으로 선택의 의미를 만든다.

핵심 문제는 다음과 같다.

- 주식 카드가 개별 숫자만 가진 상태라, 플레이어가 장기 빌드를 읽고 선택하기 어렵다.
- 1x8 시장 테이프는 단순 명료하지만, 슬롯 흐름에 따른 매수 타이밍의 재미가 충분히 살아나지 않는다.
- 현재 플레이에서는 집중 투자로 고점을 보는 "뽕맛"이 약하다.
- 자원 카드와 주식 카드가 같은 시장에서 경쟁하므로, 영업일 수와 템포 보상이 v2보다 넉넉하게 재조정되어야 한다.
- "시장 상황"을 별도 시스템으로 추가하면 UI와 규칙이 조잡해질 수 있으므로, 시장 상황의 감각을 더 작은 규칙으로 표현해야 한다.
- 매도 타이밍도 강화해야 하지만, 아직 설계가 끝나지 않았고 매수 타이밍 설계 이후 별도 논의가 필요하다.

## Solution

v3는 재미 강화를 세 축으로 해결한다.

첫째, 모든 주식 카드에 산업/테마 태그를 정확히 1개 부여한다. 태그는 카드 효과가 아니라 미션과 포트폴리오 방향성을 만들기 위한 분류다. 태그는 기술(Technology), 소비재(Consumer), 에너지(Energy), 금융(Financials), 공업(Industrials) 5종이다.

둘째, 런 시작 시 미션 후보 3개를 공개한다. 미션은 플레이어에게 이번 판의 투자 철학을 제안한다. 플레이어는 한 런에 하나의 미션만 확정할 수 있고, 최초로 클리어 조건을 만족한 미션이 즉시 확정된다. 확정된 미션은 이후 매 분기 말 현재 포트폴리오를 기준으로 미션 수익을 정산한다. 미션 수익은 현금이 아니지만, 분기 평가 목표와 월세 밀림 판정에는 항상 포함된다.

셋째, 별도의 "시장 상황" 시스템을 만들지 않고 Mr.Market 제안이 곧 시장 상황이 되게 한다. 매 분기 3개의 공개 제안이 1x8 시장 테이프의 특정 슬롯에 배치된다. 제안은 주식 카드에만 적용되며, 해당 슬롯에 있는 주식은 일반 매수가 아니라 반드시 제안 조건으로만 매수된다. 제안은 단순 할인 보너스가 아니라 "이 주식을 어떤 성격으로 사게 되는가"를 바꾸는 장치다.

v3의 고점은 미션 중심으로 둔다. Mr.Market 제안은 좋은 매수 타이밍과 매입 품질을 만들고, 호일은 그 매입 이력을 압축한다. 집중 투자 미션과 성격 변환형 제안, 호일 합성이 맞물릴 때 큰 고점이 발생해야 한다.

영업일 수는 v2보다 늘린다. 1/2 회계년도는 플레이 가능한 분기당 8영업일, 3 회계년도는 플레이 가능한 분기당 10영업일을 사용한다. 이는 미션 추적, 시장 제안 타이밍 판단, 자원 카드 구매, 추가 매수권 체인을 감당하기 위한 조정이다.

## User Stories

1. As a player, I want every stock to have exactly one industry/theme tag, so that I can read the portfolio as a strategic build rather than isolated cards.
2. As a player, I want the five tags to be Technology, Consumer, Energy, Financials, and Industrials, so that the tag space is small enough to understand quickly.
3. As a player, I want tags to avoid adding card-specific effects, so that the game stays readable.
4. As a player, I want three mission candidates to appear at run start, so that I immediately see possible investment theses for this run.
5. As a player, I want all three mission candidates to remain visible until one is confirmed, so that I can compare directions while the market develops.
6. As a player, I want each mission slot to have one mulligan, so that I can reject a bad initial thesis without rerolling the whole run.
7. As a player, I want to manually discard a mission slot after its mulligan is spent, so that I can avoid accidentally confirming a mission I no longer want.
8. As a player, I want discarded mission slots to stay empty, so that mission management has commitment and does not become endless rerolling.
9. As a player, I want the first mission whose clear condition is satisfied to become the confirmed mission immediately, so that mission choice has tension.
10. As a player, I want the remaining mission candidates to be automatically discarded after one mission is confirmed, so that the run has one clear thesis.
11. As a player, I want simultaneous mission clears to resolve from the leftmost slot, so that edge cases are deterministic.
12. As a player, I want mission clear conditions and settlement formulas to be separate, so that confirming a mission and earning from it can support different decisions.
13. As a player, I want a confirmed mission to settle every quarter end based on the current portfolio, so that the mission keeps shaping the whole run.
14. As a player, I want a mission confirmed during a quarter to settle at that same quarter end, so that early planning is rewarded immediately.
15. As a player, I want mission revenue to be cumulative evaluation value, not cash, so that it helps performance without funding purchases.
16. As a player, I want quarter-end UI to show mission revenue as a separate line, so that I understand why I met or missed the quarter target.
17. As a player, I want mission revenue to count toward rent arrears checks, so that the mission is strategically important.
18. As a player, I want simple early missions to produce small recurring revenue, so that an early-clear strategy is valid.
19. As a player, I want hard late missions to produce large later revenue, so that greed and delayed payoff are valid.
20. As a player, I want one-tag concentration missions, so that focused investing can chase high scores.
21. As a player, I want some two-tag missions, so that diversified but constrained builds are also viable.
22. As a player, I want foil-related missions, so that completing a set can become a major scoring plan.
23. As a player, I want high-value missions to care about final card value, so that Mr.Market value changes can create strong synergies.
24. As a player, I want mission formulas to sometimes count target-tag cards and sometimes all cards, so that mission design can vary without adding card text.
25. As a player, I want mission difficulty display to be readable but not a hidden rule input, so that I can trust the visible condition and formula.
26. As a designer, I want mission names to sound like investment theses, so that the build direction has flavor without adding mechanics.
27. As a designer, I want a first mission pool of about 25 cards, so that the system is broad enough for variety but small enough to balance.
28. As a designer, I want the same tag to appear in multiple mission candidates in one run, so that runs can naturally lean into a theme.
29. As a player, I want no separate market-situation system, so that the game does not gain another UI layer.
30. As a player, I want Mr.Market offers to be the market situation, so that market mood is expressed directly on buy opportunities.
31. As a player, I want three Mr.Market offers at the start of each quarter, so that each quarter has visible tactical hooks.
32. As a player, I want offers to be placed on slots rather than attached to cards, so that the 1x8 market tape creates timing decisions.
33. As a player, I want one offer in slots 1-2, one in slots 3-5, and one in slots 6-8, so that each market zone has a different role.
34. As a player, I want offers to be public information, so that I can plan around upcoming movement.
35. As a player, I want offers to be universal rather than tag-specific, so that the game suggests opportunities without forcing a tag choice.
36. As a player, I want offers to apply only to stock cards, so that consumable resource cards stay simple.
37. As a player, I want an offer under a consumable resource card to remain inactive and unconsumed, so that the next stock may still use it.
38. As a player, I want a stock on an offer slot to be purchasable only through that offer, so that the offer meaningfully changes the decision.
39. As a player, I want failed purchase attempts to do nothing, so that I am not punished for checking affordability.
40. As a player, I want unused offers to disappear at quarter end, so that market timing matters.
41. As a player, I want offer slots to pass opportunities to later cards if unused, so that card flow and slot position matter.
42. As a player, I want payment reduction offers in the left slots, so that late-stage cards can become easier to grab before they leave.
43. As a player, I want character transformation offers in the center slots, so that the middle market creates the most interesting buy-quality decisions.
44. As a player, I want tempo offers in the right slots, so that new arrivals can create explosive turns.
45. As a player, I want payment reduction offers to be simple -1 discounts, so that the left market stays easy to parse.
46. As a player, I want character transformation offers to permanently change value or dividend, so that Mr.Market can create unique ownership histories.
47. As a player, I want tempo offers to grant extra buy rights or investment philosophy bonuses, so that a single good turn can accelerate my plan.
48. As a player, I want Mr.Market effects to feel like character conversion rather than free bonuses, so that every offer has a tradeoff or tempo identity.
49. As a player, I want final value and final dividend to never go below 0, so that negative card states do not create awkward edge cases.
50. As a player, I want foil cards to inherit the summed permanent Mr.Market traces from their ingredients, so that great purchases compress into great foils.
51. As a player, I want Mr.Market permanent value and dividend changes to affect mission settlement, so that buy timing can feed the scoring engine.
52. As a player, I want sale price to ignore Mr.Market value and dividend deltas, so that selling remains a separate timing problem.
53. As a player, I want reserved cards to stay in their market slot, so that reservation preserves the card's slot context.
54. As a player, I want reservation to preserve only the card and not the offer, so that each quarter's offer refresh still matters.
55. As a player, I want a reserved card on an offer slot to use the current quarter's offer, so that reserving a card can hold it in a favorable slot for this quarter.
56. As a player, I want a reserved card to receive the next quarter's new offer if its slot gets one, so that reservation and quarter refresh interact naturally.
57. As a player, I want only one reservation at a time, so that reservation is a focused tool rather than a second hand.
58. As a player, I want reservation to be free, so that it acts as slot control rather than another full action tax.
59. As a player, I want stock opportunities and consumable resource cards in the same 1x8 market, so that resource acquisition competes with investment timing.
60. As a player, I want no separate investment philosophy market tape, so that the UI stays simple.
61. As a player, I want 1/2 fiscal years to have 8 business days per playable quarter, so that I have enough time for planning and mission play.
62. As a player, I want the third fiscal year to have 10 business days per playable quarter, so that the final stretch supports bigger setups.
63. As a player, I want extra buy rights to allow another stock or consumable resource purchase in the same business day, so that tempo offers can create spikes.
64. As a player, I want extra buy rights to work on both market stocks and reserved stocks, so that reservation can participate in tempo turns.
65. As a player, I want reservation to preserve extra buy rights, so that I can lock a card without ending a tempo line.
66. As a player, I want pressing next business day to discard unused extra buy rights, so that tempo remains immediate.
67. As a player, I want extra-buy purchases to still delay dividends until the next business day, so that income timing stays consistent.
68. As a player, I want tempo chains to be allowed, so that high-roll market turns can happen.
69. As a player, I want no hard chain cap, so that balance is handled by offer frequency and values rather than arbitrary interruption.
70. As a player, I want card UI to show final calculated values, tag, and offer badge, so that I can decide from the card face.
71. As a player, I want hover or quote modal to show original and final values, so that I can inspect changes without reading a log of reasons.
72. As a player, I want purchase confirmation to show offer terms and final values, so that I know what I am committing to.
73. As a designer, I want Mr.Market offer pools to be data-driven, so that balance can change without changing code.
74. As a designer, I want later fiscal years to have a higher probability of stronger offers, so that the run escalates.
75. As a designer, I want v3 to leave sell timing as the next design topic, so that purchase timing can be finished before adding another timing system.

## Implementation Decisions

- Treat v3 as a delta over v2 stock rules, not a replacement for the whole project.
- Do not add card-specific stock effects in v3.
- Add exactly one tag to each stock card.
- Use these five tags: Technology, Consumer, Energy, Financials, Industrials.
- Tags are used by missions, scoring formulas, and portfolio direction only.
- Do not create a separate "market situation" system. Mr.Market offers are the market situation.
- Keep consumable resource cards in the single 1x8 market tape.
- Do not create a separate investment philosophy market tape.
- Use 8 business days per playable quarter in fiscal years 1 and 2.
- Use 10 business days per playable quarter in fiscal year 3.
- Rebalance quarter evaluation targets and dividend/cash-flow pacing after business day count changes.
- Use the corrected v2 Deal meaning: Deal is not a purchase-payment wildcard in v3 planning. Deal converts into investment philosophy mastery. Do not restore the old Deal-as-payment rule from earlier v2 text.

Mission rules:

- Generate three mission candidates immediately at run start.
- Mission candidates are generated independently of the current market.
- Keep all three mission candidates visible until one mission is confirmed.
- Allow each mission candidate slot to be mulliganed once at any time before confirmation.
- After a slot's mulligan is spent, allow that slot to be manually discarded.
- A manually discarded mission slot remains empty and is not refilled.
- A run can confirm exactly one mission.
- The first mission candidate whose clear condition is satisfied is immediately confirmed.
- If multiple candidates satisfy their clear condition simultaneously, the leftmost candidate wins.
- On confirmation, automatically discard all other mission candidate slots.
- After confirmation, show only the confirmed mission.
- Separate mission clear condition from mission settlement formula.
- Clear condition only determines initial confirmation.
- Settlement formula runs at every quarter end after confirmation.
- A mission confirmed during a quarter is eligible for that same quarter-end settlement.
- Settlement uses the current portfolio at settlement time.
- Settlement can target cards with selected tags or all owned cards, depending on mission data.
- Settlement can use card count, foil count, tag thresholds, value totals, or designer-authored formula variants.
- Mission revenue is stored separately from cash and cash flow.
- Mission revenue is displayed to the player as part of final/evaluation value, with quarter-end UI showing `미션 수익 +N`.
- Mission revenue always counts toward the rent-arrears check.
- Rent-arrears check uses `현금 흐름 + 미션 수익`.
- 현금 흐름 is `배당 + 매도 현금`.
- 미션 수익 is the quarter-end value generated by the confirmed mission.
- 분기 평가 목표 is the quarter-end evaluation target required to avoid rent arrears.
- Mission clear checks and settlement formulas include Mr.Market permanent value changes where relevant.
- Mission names should use investment-thesis language rather than generic quest language.

Initial mission pool:

- Use a first mission pool of 25 mission cards.
- Include 5 one-tag fast-entry missions.
- Include 5 one-tag concentration missions.
- Include 5 one-tag foil missions.
- Include 5 one-tag high-value missions.
- Include 5 curated two-tag stable/diversified missions.
- Allow the same tag to appear across multiple candidates in the same run.
- Difficulty display may be auto-derived initially, but designers can manually override it.
- Difficulty is display-only and does not mechanically change conditions or rewards.

Mission formula templates:

- Fast-entry mission: clear when target tag count is 3 or more; settle as target-tag card count times `+N`; low reward; early recurring accumulation.
- Concentration mission: clear when target tag count is 5 or more; settle as target-tag card count times `+N`; medium or higher reward; supports focused high scores.
- Foil mission: clear when at least one target-tag foil exists; settle with target-tag card bonus plus additional foil bonus; high reward; supports set completion.
- High-value mission: clear when a target-tag card reaches a designer-authored value threshold; settle using a portion of target-tag total value; synergizes with Mr.Market value changes.
- Two-tag stable mission: clear when each target tag has at least 2 cards; settle as both target-tag card counts times `+N`; low-to-mid reward; supports controlled diversification.

Mr.Market offer generation:

- At every quarter start, refresh the market tape first.
- Then generate three Mr.Market offers.
- Place exactly one payment reduction offer in slots 1-2.
- Place exactly one character transformation offer in slots 3-5.
- Place exactly one tempo offer in slots 6-8.
- Offers are public information.
- Offers are slot-bound, not card-bound.
- Offers apply only to stock cards.
- Offers do not require any tag or mission condition.
- A consumable resource card on an offer slot shows the offer as inactive.
- An inactive offer under a consumable resource card is not consumed.
- If a stock later occupies that slot before the offer expires, the stock may use that offer.
- A stock on an offer slot cannot be bought normally.
- A stock on an offer slot must be bought with that offer's terms.
- An offer is consumed only after a successful purchase.
- Failed purchase attempts spend no costs, consume no offer, do not consume a business day, and do not change extra-buy state.
- Unused offers are discarded at quarter end.
- The same offer formula can appear again in later quarters.
- Each quarter draws one offer from each role group.
- Later fiscal years should have a higher probability of stronger offers.
- Balance offer strength through pool weights, slot placement, and values rather than tag restrictions.

Mr.Market offer roles:

- Payment reduction offers affect only the payment moment and have no permanent trace.
- Character transformation offers may change purchase cost and may add permanent value/dividend deltas.
- Tempo offers may grant extra buy rights or specific investment philosophy bonuses.
- Tempo offers do not change cost and do not create permanent value/dividend deltas.
- Permanent offer traces can only store value delta and dividend delta.
- Final card value cannot be below 0.
- Final card dividend cannot be below 0.
- When three cards merge into a foil, sum the three ingredient cards' permanent value/dividend traces into the foil.
- Apply dividend floor after summing traces on the final foil.
- Mr.Market permanent value/dividend traces affect final value display and mission settlement.
- Sale price does not reflect Mr.Market value/dividend traces.

Payment reduction offer pool:

| ID | Formula |
|---|---|
| P01 | 현금 비용 -1 |
| P02 | 독서 비용 -1 |
| P03 | 명상 비용 -1 |
| P04 | 인내 비용 -1 |

- Payment reductions cannot reduce a cost below 0.
- The four payment reduction offers are intentionally reused.

Character transformation offer pool draft:

| ID | Formula |
|---|---|
| S01 | 현금 비용 +0 / 가치 +1 / 배당 -1 |
| S02 | 현금 비용 +1 / 가치 +2 / 배당 -1 |
| S03 | 현금 비용 +1 / 가치 +3 / 배당 -2 |
| S04 | 현금 비용 +2 / 가치 +4 / 배당 -2 |
| S05 | 현금 비용 +2 / 가치 +5 / 배당 -3 |
| S06 | 현금 비용 +0 / 가치 -1 / 배당 +1 |
| S07 | 현금 비용 +1 / 가치 -1 / 배당 +2 |
| S08 | 현금 비용 +1 / 가치 -2 / 배당 +2 |
| S09 | 현금 비용 +2 / 가치 -2 / 배당 +3 |
| S10 | 현금 비용 +2 / 가치 -3 / 배당 +3 |
| S11 | 현금 비용 +1 / 가치 +2 / 배당 +0 |
| S12 | 현금 비용 +2 / 가치 +3 / 배당 +0 |
| S13 | 현금 비용 +2 / 가치 +4 / 배당 +0 |
| S14 | 현금 비용 +3 / 가치 +5 / 배당 +0 |
| S15 | 현금 비용 +3 / 가치 +6 / 배당 +0 |
| S16 | 현금 비용 +1 / 가치 +1 / 배당 +1 |
| S17 | 현금 비용 +2 / 가치 +2 / 배당 +1 |
| S18 | 현금 비용 +2 / 가치 +1 / 배당 +2 |
| S19 | 현금 비용 +3 / 가치 +2 / 배당 +2 |
| S20 | 현금 비용 +3 / 가치 +3 / 배당 +2 |

- Character transformation values are balance draft data.
- Designers can revise the formulas and weights directly.

Tempo offer pool draft:

| ID | Formula |
|---|---|
| T01 | 추가 매수권 +1 |
| T02 | 추가 매수권 +1 / 독서 +1 |
| T03 | 추가 매수권 +1 / 명상 +1 |
| T04 | 추가 매수권 +1 / 인내 +1 |
| T05 | 추가 매수권 +1 / 독서 +1 / 명상 +1 |
| T06 | 독서 +1 |
| T07 | 명상 +1 |
| T08 | 인내 +1 |
| T09 | 독서 +2 |
| T10 | 명상 +2 |
| T11 | 인내 +2 |
| T12 | 독서 +1 / 명상 +1 |
| T13 | 독서 +1 / 인내 +1 |
| T14 | 명상 +1 / 인내 +1 |
| T15 | 독서 +2 / 명상 +1 |
| T16 | 독서 +2 / 인내 +1 |
| T17 | 명상 +2 / 독서 +1 |
| T18 | 명상 +2 / 인내 +1 |
| T19 | 인내 +2 / 독서 +1 |
| T20 | 인내 +2 / 명상 +1 |

- T02-T05 are intentionally strong and remain in the pool.
- Control strong tempo offers through weights and placement rather than system-level chain caps.
- Investment philosophy bonuses discard overflow according to normal investment philosophy cap rules.

Reservation and Mr.Market:

- Reservation is a free action.
- Only one card can be reserved at a time.
- Reservation preserves the card, not the offer.
- Reserved cards remain in their market slot.
- Reserved cards do not move during market tape progress.
- If a reserved stock is on an offer slot, it can be bought with that slot's current-quarter offer.
- When the next quarter begins, the reserved card stays in the slot, the old offer is discarded, and the new quarter's offer placement applies.
- A reserved stock on an offer slot effectively occupies that offer while it remains there.

Extra buy:

- Tempo offers can grant extra buy rights.
- Extra buy rights can be spent on market stock purchases.
- Extra buy rights can be spent on reserved stock purchases.
- Extra buy rights can be spent on consumable resource card purchases.
- Buying a stock with an extra buy right consumes the right.
- Buying a consumable resource card with an extra buy right consumes the right.
- Reservation is allowed during extra buy and preserves the extra buy right.
- Pressing next business day discards the extra buy right.
- Extra buy rights do not carry into the next business day.
- Stocks purchased through extra buy do not pay dividends until the next business day.
- Tempo chains are allowed.
- If an extra-buy purchase consumes a tempo offer that grants another extra buy right, the new right can continue the chain.
- The system does not automatically end a tempo chain except through player action.
- Multiple offers may be consumed in one business day.
- Cards pulled into the market during the same day can immediately become extra-buy candidates.

Display decisions:

- Stock cards show core numbers, tag, offer badge, and all calculated final values.
- Consumable resource cards on offer slots show the offer as inactive.
- Hover or quote modal shows original values and final values.
- Do not show a detailed reason list for why a value changed.
- Purchase confirmation modal for offered stocks shows the offer formula and final calculated values.
- Offer display should be formula-centered, not flavor-name-centered.

Recommended modules:

- Tag catalog: owns the five stock tags and stock-to-tag assignment validation.
- Mission definition: owns mission clear condition data, settlement formula data, difficulty display, and mission name metadata.
- Mission run state: owns candidate slots, mulligan state, manual discard state, confirmation, and confirmed mission tracking.
- Mission resolver: evaluates clear conditions and deterministic leftmost confirmation.
- Mission settlement: calculates quarter-end mission revenue from the current portfolio.
- Mr.Market offer catalog: owns role groups, formula definitions, fiscal-year weights, and designer-tunable offer data.
- Mr.Market quarter state: owns current-quarter offer placement, offer expiration, and offer consumption.
- Offer application: calculates payment-time modifiers, permanent traces, final value, and final dividend.
- Market tape integration: applies offer-slot behavior, inactive resource-card offers, reserved-slot behavior, and offer persistence within a quarter.
- Purchase transaction: enforces offer-only purchase on offered stocks and no-op failed attempts.
- Foil trace aggregation: carries permanent value/dividend traces from ingredient cards to foil cards.
- Extra buy state: owns grant, consume, carry prevention, reservation interaction, and chain behavior.
- Quarter evaluation: combines cash flow and mission revenue for quarter target/rent-arrears checks.
- Card presentation: renders tag, offer badge, original/final values, and confirmation modal values.

Suggested implementation order:

1. Add stock tag data and presentation.
2. Add mission definitions, candidate slots, mulligan/discard, and confirmation.
3. Add mission quarter-end settlement and rent-arrears integration.
4. Add Mr.Market offer data and quarter placement.
5. Add offer application to stock purchase validation and payment calculation.
6. Add permanent value/dividend traces and foil aggregation.
7. Add tempo offer and extra-buy integration.
8. Add UI presentation for offer badges, original/final values, and confirmation modal.
9. Rebalance business days, quarter targets, and offer weights.
10. Defer sell timing design to the next v3 discussion.

## Testing Decisions

Good tests should assert externally visible rules and state transitions, not private helper structure. v3 has several deep rule modules that should be tested as pure rule modules before UI wiring.

Test the tag catalog:

- Every stock has exactly one tag.
- Only the five allowed tags are accepted.
- Tag assignment does not add card-specific effects.

Test mission candidate state:

- Run start creates three mission candidates.
- Candidate slots remain visible until confirmation.
- Each slot can mulligan once.
- A spent slot can be manually discarded.
- Discarded slots are not refilled.
- Confirmation discards all other slots.
- Leftmost candidate wins simultaneous clears.
- No second mission can be confirmed after the first confirmation.

Test mission clear and settlement:

- Clear condition and settlement formula are evaluated separately.
- A mission confirmed during a quarter settles at that same quarter end.
- Settlement uses the current portfolio at quarter end.
- Settlement can target target-tag cards.
- Settlement can target all owned cards when configured.
- Fast-entry, concentration, foil, high-value, and two-tag templates each produce the expected revenue.
- Mission revenue is 0 when the settlement formula yields no matching portfolio value.
- Mission revenue is not added to cash.
- Mission revenue is included in quarter target/rent-arrears checks.
- Mr.Market permanent value changes affect mission settlement where relevant.

Test business-day count:

- Fiscal years 1 and 2 use 8 business days per playable quarter.
- Fiscal year 3 uses 10 business days per playable quarter.
- Quarter routing/vacation behavior from v2 remains intact unless explicitly changed by later work.

Test Mr.Market quarter generation:

- Quarter start refreshes the market tape before generating offers.
- Exactly one offer appears in slots 1-2.
- Exactly one offer appears in slots 3-5.
- Exactly one offer appears in slots 6-8.
- The three offers are drawn from different role groups.
- Offers are public state.
- Offers expire at quarter end.
- Later fiscal-year weights can produce stronger offers at higher probability.

Test offer-slot behavior:

- Offers apply only to stock cards.
- Consumable resource cards show inactive offers.
- Consumable resource cards do not consume inactive offers.
- A stock entering an unused offer slot can use that offer.
- A stock on an offer slot cannot be purchased normally.
- A stock on an offer slot can only be purchased through the offer.
- Successful purchase consumes the offer.
- Failed purchase consumes no cost, no offer, no business day, and no extra buy right.

Test offer formulas:

- Payment reduction offers reduce only the matching cost.
- Payment reduction floors at 0.
- Character transformation cost changes affect purchase validation.
- Character transformation value/dividend deltas are stored as permanent traces.
- Final value and final dividend floor at 0.
- Tempo offers grant extra buy rights or investment philosophy bonuses.
- Investment philosophy bonuses discard overflow under existing cap rules.
- Sale price ignores Mr.Market value/dividend traces.

Test reservation and offers:

- Reservation preserves the card and not the offer.
- Reserved cards stay in their slot.
- Reserved cards do not move with tape progress.
- A reserved card uses the current offer on its slot.
- Quarter transition discards old offer while preserving the reserved card.
- A reserved card can receive a new quarter offer if its slot gets one.
- Only one reservation can exist at a time.
- Reservation is free.

Test foil trace aggregation:

- Foil merge sums all three ingredient value traces.
- Foil merge sums all three ingredient dividend traces.
- Dividend floor applies after the final summed trace.
- Foil final value and dividend participate in mission settlement.

Test extra buy:

- Tempo offer can grant extra buy right.
- Extra buy can buy market stock.
- Extra buy can buy reserved stock.
- Extra buy can buy consumable resource card.
- Buying stock or consumable resource card consumes the extra buy right.
- Reservation during extra buy preserves the extra buy right.
- Next business day discards unused extra buy right.
- Extra buy does not carry to the next business day.
- Extra-buy stock purchase does not pay dividend until the next business day.
- Extra buy can chain through another tempo offer.
- Multiple offers can be consumed in one business day.

Test display behavior:

- Stock card face shows core numbers, tag, offer badge, and final calculated values.
- Hover or quote modal shows original and final values.
- Hover or quote modal does not show a reason-by-reason change log.
- Purchase confirmation for offered stocks shows offer formula and final calculated values.
- Consumable resource cards show inactive offer status without implying the offer was consumed.

Prior art:

- Reuse or adapt v2 tests for market tape movement, reservation slot locking, purchase validation, resource caps, portfolio/foil resolution, quarter settlement, and UI button gating.
- Add new pure tests for mission resolver, mission settlement, offer catalog, offer application, and extra-buy state before PlayMode/UI tests.
- Final verification should include relevant EditMode rule suites and PlayMode/UI checks for mission display, offer badges, offered-stock purchase confirmation, reservation/offer interaction, and quarter-end settlement display.

## Out of Scope

- Implementing this PRD's issues in the PRD step.
- Generating v3 implementation issues in this PRD step.
- Rewriting v1/v2 PRDs.
- Adding card-specific stock effects.
- Adding a separate market-situation system.
- Adding a separate investment philosophy market tape.
- Making Mr.Market offers tag-specific.
- Making Mr.Market offers mission-specific.
- Designing the sell-timing system.
- Changing sale price to include Mr.Market permanent value/dividend traces.
- Adding card-by-card revenue analytics.
- Adding tutorial, animation polish, or final art beyond what is needed to understand the rules.
- Rebalancing all final numbers as part of the PRD writing step.

## Further Notes

- v3 implementation should prioritize `태그/미션`, then `Mr.Market 제안`, then `매도 타이밍`.
- The next design discussion should focus on sell timing. It should use a different logic from market-tape buy timing.
- Payment reduction offer count was intentionally reduced to four reusable formulas.
- Character transformation and tempo pools are draft balance data intended for designer review.
- Mr.Market offers should feel like Mr.Market presenting good or bad prices and personalities for the same asset, not like generic bonus tiles.
- The 1x8 market tape remains important because it keeps UX simple. v3 should deepen that tape with slot offers rather than add parallel markets.
- If older docs still say Deal substitutes purchase philosophy costs, treat that as superseded by the later decision that Deal is investment philosophy mastery.
