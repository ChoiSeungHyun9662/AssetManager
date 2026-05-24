# 20260520 피드백 반영 Amendment PRD

Status: ready-for-agent

Parent PRD: `.scratch/stock-rules-overhaul/PRD.md`

Source decisions: `.scratch/stock-rules-overhaul/feedback/20260520_design-decisions.md`

이 문서는 기존 주식 규칙 대개편 PRD를 대체하지 않는다. 이미 구현된 현재 버전을 기준으로 2026-05-20 피드백을 반영하기 위한 수정 전용 PRD다. 기존 PRD, 이슈, 구현과 충돌하는 경우 이 문서의 결정은 이 피드백 반영 범위 안에서 우선한다.

## Problem Statement

현재 구현은 주식 중심 시장과 포트폴리오, 1x8 시장 테이프, 카드 호버 확대, 포트폴리오 주식 표시, 매수/예약/매도 규칙의 큰 줄기를 이미 갖추고 있다. 그러나 최신 플레이 피드백 기준으로 핵심 조작과 성장 구조가 아직 번거롭거나 서로 어긋난다.

가장 큰 문제는 매수, 예약, 딜, 투자 철학, 매도가 서로 다른 과거 규칙의 잔재를 끌고 있다는 점이다. 매수는 여전히 비용 슬롯과 딜 배치 전제를 일부 남기고, 딜은 결제 보조 자원이면서 동시에 새 성장 자원으로 재정의되어야 한다. 예약은 기존처럼 영업일을 소비하고 딜을 주는 행동이 아니라, 시장 카드 1장을 편하게 고정하고 해제하는 토글형 조작이어야 한다. 포트폴리오 매도도 호버 버튼보다 드래그 앤 드롭이 더 직관적이다.

플레이어 입장에서는 다음이 명확해야 한다.

- 시장 카드를 잡고 놓는 위치에 따라 매수, 즉시 매수, 예약, 예약 해제가 예측 가능해야 한다.
- 매수 비용은 플레이어가 직접 칩을 끼우지 않아도 자동으로 계산되고 지불되어야 한다.
- 딜은 결제 대체물이 아니라 투자 철학 마스터리로 바꾸는 성장 자원이어야 한다.
- 투자 철학은 총합 제한 없이 각 철학별 한도 안에서 성장해야 한다.
- 비용 부족은 말보다 카드와 비용 표시가 알려줘야 한다.
- 보유 주식 매도는 포트폴리오 카드 자체를 매도 실행 영역으로 끌어 놓는 방식이어야 한다.

## Solution

이번 피드백 반영은 새 게임 모드를 만드는 것이 아니라, 현재 주식 대개편 버전의 조작과 자원 규칙을 교체한다.

시장 카드는 클릭하거나 포인터로 잡아 위아래로 움직일 수 있다. 짧은 클릭은 포인터 이동량 임계값을 넘지 않은 `down -> release`로 판정한다. 주식 카드를 짧게 클릭하면 자산 매수 확인 모달을 열고, 소모형 자원 카드를 짧게 클릭하면 확인 모달 없이 즉시 자산 매수를 시도한다. 카드 릴리즈 위치가 포트폴리오 영역이면 카드 타입과 무관하게 확인 팝업 없이 즉시 자산 매수를 시도한다. 드래그 후 포트폴리오 영역이 아닌 곳에 놓으면 아무 액션도 일어나지 않고 카드가 이전 위치로 복귀한다. 모달은 시장 카드 호버 확대와 같은 카드 상세 표시를 포함하고, 하단의 긴 확인 버튼과 우측 상단 돌아가기 버튼만 제공한다.

자산 매수는 비용 슬롯과 Payment Pot을 사용하지 않는다. 매수 시 현재 현금, 투자 철학 보유량, 투자 철학 마스터리를 기준으로 할인 후 비용을 계산하고 자동 지불한다. 비용 부족이면 시스템 메시지를 띄우지 않고 카드 떨림만 발생한다. 비용 외 실패는 카드 떨림과 기존 메시지를 함께 보여준다.

딜은 매수 결제에서 빠진다. 플레이어는 보유 딜을 리소스 HUD에서 독서, 명상, 인내 HUD lane으로 드래그해 해당 투자 철학 마스터리를 1 올린다. 딜 칩을 호버하거나 드래그하면 안내 패널을 표시하고, 드래그 중에는 원래 딜 칩 하나를 숨긴 채 딜 이미지와 안내 패널을 포인터에 붙여 움직인다. 안내 패널의 우측 하단 모서리는 포인터 위치에 맞춘다. 마스터리는 해당 철학의 시장 카드 비용을 표시, 검증, 지불 시점에만 할인한다. 카드 원본 비용은 바뀌지 않는다.

투자 철학 보유량과 투자 철학 마스터리는 각각 독서, 명상, 인내별 0-5 범위만 가진다. 기존 투자 철학 총합 10 제한과 딜 보유 한도는 제거한다. 철학 HUD는 보유량을 큰 정수로 표시하고, 마스터리가 1 이상일 때만 작은 `+N`을 붙인다.

예약은 유지하되 성격과 입력 방식을 바꾼다. 예약 가능 카드는 주식 카드 1장뿐이고, 예약은 영업일을 소비하지 않으며 딜을 지급하지 않는다. 주식 카드를 호버하면 원래 시장 카드 아래에 자물쇠 예약 버튼을 표시하고, 이 버튼을 클릭하면 예약한다. 예약된 카드는 카드의 아래 면이 예약 버튼 아래 면 위치까지 내려간 상태로 고정된다. 예약된 카드를 호버하면 내려간 카드 위에 예약 해제 버튼을 표시하고, 이 버튼을 클릭하면 즉시 원래 시장 줄로 복귀한다. 카드 또는 예약/해제 버튼 위에 포인터가 있으면 버튼은 계속 표시된다. 버튼 클릭은 카드 클릭 매수로 전파되지 않는다. 새 카드를 예약하면 기존 예약은 자동 해제된다. 예약된 카드는 시장 진행과 갱신에서 남아 있고, 내려간 시각 위치가 클릭, 드래그, 호버 기준이 된다.

보유 주식 매도는 드래그 앤 드롭으로 바꾼다. 매도 실행 영역은 항상 보이는 붉은 배경의 `$` 영역이다. 보유 주식을 드래그하면 원래 포트폴리오 카드는 숨겨지고, 시장 호버 패널과 같은 카드 상세 패널이 포인터를 따라다닌다. 포인터를 매도 실행 영역 안에서 놓으면 매도되고, 밖에서 놓으면 취소된다.

딜 획득은 예약 보상이 아니라 런 중 업적 보상으로 바뀐다. 포트폴리오에 존재하는 주식 카드 수가 처음 3, 5, 8에 도달할 때마다 딜을 1개 지급한다. 같은 주식 ID라도 두 슬롯을 차지하면 두 장으로 센다. 런 최초 호일 완성도 딜을 1개 지급한다. 같은 자산 매수에서 슬롯 수 보상과 최초 호일 보상이 동시에 발생하면 둘 다 지급한다.

## User Stories

1. As a player, I want to drag a market card freely before releasing it, so that buying and reserving feel like direct card manipulation.
2. As a player, I want dropping a market card on the portfolio area to immediately attempt purchase, so that a clear "put this into my portfolio" gesture skips extra confirmation.
3. As a player, I want short-clicking a stock card to open a purchase confirmation modal, so that stock purchases still have explicit confirmation.
4. As a player, I want the purchase confirmation modal to show the same card detail information as the market hover panel, so that I can confirm exactly what I am buying.
5. As a player, I want the confirmation modal to have a long bottom confirmation button, so that the primary action is obvious.
6. As a player, I want the confirmation modal to close through a top-right 돌아가기 button, so that cancelling is visually clear.
7. As a player, I want background interactions blocked while the confirmation modal is open, so that the confirmed card and current state cannot drift.
8. As a player, I want the purchase to be revalidated when I press 확인, so that stale confirmation state cannot buy an invalid card.
9. As a player, I want cancelling the purchase modal to return the market card to its previous position, so that no state changes happen when I back out.
10. As a player, I want purchase payment to happen automatically, so that I no longer need to place chips into Payment Pot slots.
11. As a player, I want consumable resource cards to short-click purchase immediately without a confirmation modal, so that immediate resource cards remain fast to use.
12. As a player, I want purchase cost shortage to shake the market card without showing a system message, so that failure is fast and non-intrusive.
13. As a player, I want non-cost purchase failures to shake the card and show the existing message, so that constraints like portfolio capacity are still explained.
14. As a player, I want costs I cannot afford to be shown in red on the market card, so that I can understand shortage before clicking.
15. As a player, I want mastery discounts reflected in market card, hover, and modal cost displays, so that the whole UI tells the same cost story.
16. As a player, I want discounted costs to show original cost then discounted philosophy cost, so that I can see what mastery changed.
17. As a player, I want a market card purchase success to move the card into the correct result flow without shake feedback, so that shake means failure.
18. As a player, I want market card hover enlargement to hide during drag, so that it does not interfere with placement.
19. As a player, I want market cards 1-6 to enlarge to the right and cards 7-8 to enlarge to the left, so that hover panels stay predictable.
20. As a player, I want hover offset to use the card's current visual position, so that reserved cards enlarge from their lowered position.
21. As a player, I want stock cards to show a lock button below the card while hovered, so that reservation has a clear dedicated control.
22. As a player, I want reservation and unreservation to apply immediately without a modal, so that reservation remains lightweight.
23. As a player, I want exactly one card reservable at a time, so that the reservation rule is easy to reason about.
24. As a player, I want reserving a new card to automatically release the previous reserved card, so that changing my mind is quick.
25. As a player, I want reservation to no longer consume a business day, so that it is a convenience tool rather than a major action.
26. As a player, I want reservation to no longer grant Deal, so that Deal rewards come from portfolio achievements.
27. As a player, I want reserved cards to remain in the market across market movement and refresh, so that reservation reliably protects one opportunity.
28. As a player, I want reserved cards to be buyable with the same purchase rules as normal market cards, so that reservation does not create a separate purchase flow.
29. As a player, I want reserved stock cards to show an unlock button above the lowered card while hovered, so that reservation release is clear.
30. As a player, I want drag release outside the portfolio area to do nothing, so that dragging a market card only buys when I deliberately drop it on the portfolio.
31. As a player, I want the hover card action area to show whether clicking will confirm purchase or immediately gain the card, so that stock and consumable resource card click behavior is predictable.
32. As a player, I want Deal removed from direct purchase payment, so that it has one clear purpose.
33. As a player, I want to drag a Deal from the resource HUD onto Reading, Meditation, or Patience, so that I can convert Deal into mastery.
34. As a player, I want Deal hover or drag to show the guide panel, and Deal drag to show a pointer-following Deal image with the guide panel's bottom-right corner at the pointer, so that I understand the drop target action.
35. As a player, I want dropping Deal outside philosophy lanes to cancel without spending it, so that failed drag attempts are harmless.
36. As a player, I want dropping Deal on a mastery-5 philosophy to fail without spending it, so that capped mastery cannot waste a Deal.
37. As a player, I want Deal to have no holding cap, so that achievement rewards are not discarded.
38. As a player, I want investment philosophy holdings to be capped per type only, so that I can hold up to 5 Reading, 5 Meditation, and 5 Patience.
39. As a player, I want the old total investment philosophy cap removed, so that one philosophy does not block another.
40. As a player, I want investment philosophy mastery capped per type at 5, so that discounts have a clear maximum.
41. As a player, I want no cash mastery, so that mastery only applies to investment philosophy costs.
42. As a player, I want mastery to discount display and payment without mutating card source costs, so that market cards stay consistent as my mastery changes.
43. As a player, I want a philosophy cost discounted to zero to consume zero of that philosophy, so that mastery has full effect.
44. As a player, I want the philosophy HUD to show holdings as large integers, so that the available resource count is immediately readable.
45. As a player, I want mastery shown as a smaller `+N` only when above zero, so that mastery is visible without clutter.
46. As a player, I want to earn Deal when my occupied portfolio stock-slot count first reaches 3, 5, and 8, so that portfolio growth creates rewards.
47. As a player, I want duplicate stocks in different slots to count as multiple cards for these rewards, so that the reward follows portfolio occupancy.
48. As a player, I want these threshold Deal rewards to happen only once per run, so that selling and rebuying cannot farm them.
49. As a player, I want the first foil stock in a run to grant Deal once, so that the collection milestone feels rewarded.
50. As a player, I want threshold and first-foil rewards to both pay out if they occur from the same purchase, so that simultaneous achievements are not lost.
51. As a player, I want owned stock hover to stop revealing a sell button, so that selling has one clear interaction model.
52. As a player, I want the sale execution area always visible as a red `$` area, so that I know where to sell stocks.
53. As a player, I want dragging an owned stock to hide the original portfolio card, so that I do not see duplicate cards.
54. As a player, I want an owned-stock detail panel to follow my pointer while dragging, so that I can inspect what I am selling.
55. As a player, I want the sale area to show `+N` while dragging a stock, so that the sale payout is clear before drop.
56. As a player, I want dropping an owned stock on the sale area to sell it, so that sale is a direct drag-and-drop action.
57. As a player, I want dropping an owned stock outside the sale area to cancel and restore it, so that accidental drags are reversible.
58. As a developer, I want these changes captured in a focused amendment PRD, so that follow-up issues can reference one current source of truth.

## Implementation Decisions

- Treat this as an amendment over the implemented stock-overhaul version, not a rewrite of the whole PRD.
- Keep source card cost data immutable. Apply mastery discounts only when presenting, validating, and paying purchase costs.
- Introduce run-scoped investment philosophy mastery for Reading, Meditation, and Patience.
- Enforce mastery caps per philosophy type at 5.
- Enforce investment philosophy holding caps per type at 5.
- Remove the old total investment philosophy holding cap.
- Remove the Deal holding cap.
- Remove Deal from direct purchase payment and old payment-slot substitution.
- Keep no cash mastery.
- Add a reusable final-cost calculation that can serve market card display, hover display, confirmation modal display, purchase validation, and payment.
- Make cost display capable of marking only insufficient cost tokens in red.
- Keep discounted philosophy costs floored at zero.
- Remove Payment Pot and manual chip placement from the new play path.
- Keep compatibility types only where they reduce migration risk; do not expose them in the normal play UI.
- Make market card input distinguish short-click from drag by pointer movement threshold.
- Make short-clicked stock cards open a confirmation modal only after current validation says the purchase is possible.
- Make short-clicked consumable resource cards attempt immediate purchase without a confirmation modal.
- Make market card drag release inside the portfolio area attempt immediate purchase.
- Make market card drag release outside the portfolio area perform no action and restore the previous card position.
- Revalidate purchase on modal confirmation.
- Make card shake a purchase-failure presentation event, not a success event.
- Split validation failure categories into cost shortage and non-cost failure so messages can be suppressed only for cost shortage.
- Make reservation state allow exactly one reserved stock card.
- Keep reservation as market-slot state, with lowered visual position as the interaction position.
- Make reservation and unreservation non-consuming actions with no Deal payout.
- Replace drag-threshold reservation with stock-card hover lock/unlock buttons.
- Show the reserve lock button below a normal stock card while the card or button is hovered.
- Show the unreserve button above a lowered reserved stock card while the card or button is hovered.
- Prevent lock/unlock button clicks from propagating to market-card purchase input.
- Lower a reserved card until its bottom edge reaches the reserve button's bottom edge; unreserve returns it to the normal market row.
- Automatically release the previous reservation when reserving a new card.
- Keep reserved cards fixed across market movement and refresh.
- Make reserved cards use the same purchase flow as normal market cards.
- Calculate market hover panel position by current card visual position and slot number.
- Reuse one card-detail presentation model for market hover, purchase modal, and owned-stock drag detail where possible.
- Make owned-stock sale pointer-position based.
- Keep sale based on the original stock-slot index of the dragged portfolio card.
- Remove owned-stock hover sell-button behavior from the new play path.
- Add Deal-to-mastery drag handling from the resource HUD Deal stack.
- Make Reading, Meditation, and Patience HUD lanes the Deal drop targets.
- Make Deal drop and sale drop use pointer position rather than visual panel overlap.
- Add run-level reward state for first reached occupied stock-slot thresholds 3, 5, and 8.
- Count occupied stock slots, not unique stock IDs, for the 3/5/8 Deal rewards.
- Add run-level reward state for first foil creation.
- Evaluate Deal rewards after purchase, foil merge, and portfolio slot cleanup.
- Allow multiple Deal rewards from the same purchase when multiple reward conditions are newly met.
- Update domain documentation later if this amendment becomes the accepted direction, because the current glossary still describes old Deal and reservation behavior.

## Testing Decisions

- Rule tests should focus on public behavior and state transitions, not helper method shape.
- Resource tests should cover per-type philosophy cap, removal of total philosophy cap, Deal cap removal, and mastery cap.
- Cost tests should cover discount application, zero floor, unchanged source costs, insufficient token marking inputs, and actual resource consumption.
- Purchase tests should cover automatic payment for stock and consumable resource cards, cost shortage failure without state changes, non-cost failure without state changes, stock click modal purchase, consumable click immediate purchase, portfolio immediate purchase, and modal confirm revalidation.
- Purchase flow tests should cover modal cancel restoring normal and reserved card positions without market, reservation, resource, or portfolio changes.
- Reservation rule tests should cover one-stock-card capacity, automatic previous reservation release, no business-day consumption, no Deal grant, persistence across market movement/refresh, and purchase clearing reservation.
- Market input PlayMode tests should cover movement-threshold click/drag split, stock short-click modal, consumable short-click immediate purchase, portfolio drop immediate purchase, drag release outside portfolio doing nothing, and drag hiding hover enlargement.
- Reservation PlayMode tests should cover stock-only hover lock button, reserved-card hover unlock button, hover persistence while moving from card to button, button click propagation blocking, lowered reserved card position, and unreserve return to normal row.
- Hover presentation tests should cover slots 1-6 using `+300px`, slots 7-8 using `-300px`, and reserved lowered positions as the offset base.
- Deal reward tests should cover first occupied-slot thresholds 3/5/8, duplicate stock cards counting as multiple occupied slots, no repeat after sale or foil reduction, first-foil reward, and simultaneous reward payout.
- Deal drag UI tests should cover 0 Deal disabled state, guide panel follow, successful philosophy drop, outside drop cancel, and mastery-5 failure without Deal consumption.
- Resource HUD tests should cover large holding count display, hidden `+0` mastery, and visible small `+N` mastery.
- Stock sale rule/UI tests should cover hover sell-button removal, sale area default `$`, drag `+N` display, pointer-inside sale success, pointer-outside cancel, original stock-slot index sale, and original card restoration.
- Final verification should include relevant EditMode rule suites and PlayMode UI suites. Unity batchmode should use the existing project wrapper outside the default sandbox.

## Out of Scope

- Rewriting the whole stock-overhaul PRD.
- Reworking quarter settlement, rent arrears, final settlement, or market deck rules beyond the direct impact of these feedback decisions.
- Adding tutorial copy, onboarding flows, or animation polish beyond required card shake and drag-follow presentations.
- Adding separate reservation zones, reservation panels, hover-panel reservation buttons, or drag-threshold reservation.
- Keeping manual Payment Pot interaction in the normal play path.
- Treating Deal as both mastery currency and purchase payment currency.
- Making mastery permanent across runs.
- Adding cash mastery.
- Adding total caps for investment philosophy holdings or mastery.
- Changing stock sale payout math unless a later issue explicitly reopens it.
- Updating all domain glossary wording in this PRD step.

## Further Notes

- `PRD_v2.md` exists in the feature directory, but it conflicts with the latest grill-me decisions around reservation, Deal rewards, and purchase confirmation. This amendment should be used as the current source for follow-up issue slicing.
- Existing implementation already has several compatible foundations: 1x8 market tape, market hover enlargement, portfolio stock slots, stock sale rule service, and compatibility card-detail state. Follow-up issues should use those foundations rather than re-opening completed overhaul work.
- The old glossary still defines Deal and reservation using pre-feedback behavior. Treat that as known drift for this amendment; update the docs once the issue breakdown is accepted.
