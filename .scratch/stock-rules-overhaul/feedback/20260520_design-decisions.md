# 20260520 Feedback Design Decisions

Source feedback: `.scratch/stock-rules-overhaul/feedback/20260520_feedback.md`

Status: in progress
Last updated: 2026-05-22

This document records decisions confirmed during the grill-me pass. It is the implementation-facing interpretation of the feedback, not a replacement for the original feedback file.

## Deal, Payment, And Mastery

- Deal is removed from direct purchase payment.
- Deal only affects purchases indirectly after the player converts a Deal chip into investment philosophy mastery.
- Deal holding cap is removed.
- Investment philosophy holdings are limited per type only: Reading 0-5, Meditation 0-5, Patience 0-5.
- The old total investment philosophy cap of 10 is removed; the player can hold up to 15 total philosophy resources.
- Investment philosophy mastery is also limited per type only: Reading 0-5, Meditation 0-5, Patience 0-5.
- There is no total mastery cap.
- There is no cash mastery.
- Card source cost data stays unchanged. Mastery discounts are applied only when displaying costs, validating purchase affordability, and paying for a purchase.
- Discounted philosophy costs cannot go below 0.
- If a philosophy cost is discounted to 0, that philosophy resource is not consumed when buying the card.
- Cost display keeps cash on the left only. Example with no discount: `50$, R1, M1`.
- Cost display with a discount shows the full original cost on the left and the discounted philosophy costs only on the right. Example: `50$, R1, M1 -> R0, M1`.
- Insufficient philosophy or cash costs are shown in red selectively on market card cost displays.
- The philosophy HUD shows the holding amount as a large integer.
- The philosophy HUD shows mastery as a smaller `+N` beside the holding amount only when mastery is greater than 0.
- Dropping a Deal on a philosophy whose mastery is already 5 fails, does not consume the Deal, restores the dragged chip, and shows a short failure message.
- Philosophy resource gains still discard only the amount above that philosophy type's cap of 5 and show the existing cap feedback message.
- Philosophy resource gains no longer discard anything because of total philosophy count.

## Deal Drag And Rewards

- Deal chips are dragged directly from the resource HUD Deal stack.
- Deal cannot be dragged when the player has 0 Deal.
- A successful Deal drop consumes exactly 1 Deal.
- Reading, Meditation, and Patience HUD lanes are the Deal drop targets.
- Dropping a Deal on a philosophy HUD lane gives that philosophy +1 mastery.
- Deal drag uses one pointer-following guide panel plus one Deal chip image.
- Deal drop success is decided by pointer position, not panel overlap.
- If the pointer is not inside a philosophy HUD lane on drop, the drag is cancelled and the Deal is not consumed.
- Deal is no longer granted by reservation.
- Deal is granted when the occupied portfolio stock-slot count first reaches 3, 5, and 8.
- Occupied stock-slot count is based on how many portfolio slots contain stock cards. Two cards with the same `AssetCardData.Id` count as two if they occupy two slots.
- The 3/5/8 stock-slot rewards are run-level first-time rewards. They are not revoked or granted again after sale or foil merge reduces the count.
- Deal is granted once when the run creates its first foil stock card.
- The first-foil Deal reward is run-level and can happen only once.
- Deal rewards are evaluated after purchase, foil merge, and portfolio slot cleanup finish.
- If a stock-slot threshold reward and the first-foil reward happen from the same purchase, both Deal rewards are granted.

## Market Purchase

- One-button purchase applies to every market card, including stock cards and consumable resource cards.
- Existing Payment Pot and manual payment-slot UI are removed from the new play path.
- Purchase payment automatically consumes the discounted final cash and philosophy costs.
- Market card input starts with pointer down and can be dragged freely up or down.
- On release, if the pointer is inside the portfolio area, immediate purchase is attempted first.
- Portfolio-area release has priority over reservation threshold logic.
- If release is not inside the portfolio area and the reservation threshold is crossed, the action is reservation or reservation release.
- If release is not inside the portfolio area and the reservation threshold is not crossed, the action is a purchase attempt.
- A normal purchase attempt opens a confirmation modal only if the purchase is currently possible.
- If the purchase fails because of insufficient cost, no system message is shown and the market card shakes.
- If the purchase fails for a non-cost reason, the market card shakes and the existing system message is shown.
- Purchase success does not shake the card.
- Portfolio-area immediate purchase skips the confirmation modal but uses the same validation and failure feedback rules.
- Consumable resource cards use the same failure feedback rules as stock cards.
- The confirmation modal is an intent-confirmation surface, not a restored card-detail action screen.
- The confirmation modal includes the same card detail display used by the market hover panel.
- The confirmation modal reflects mastery discount display and insufficient-cost red text using the same cost display rules as market cards.
- The modal has a long bottom `확인` button.
- The modal has a top-right `돌아가기` button that closes the modal.
- While the modal is open, background market, portfolio, and Deal drag interactions are blocked.
- Pressing `확인` validates purchase affordability and non-cost constraints again.
- If validation fails at confirm time, the modal closes and the same market-card failure feedback is applied.
- During market card drag, the original card moves with the pointer.
- On purchase success, the market card leaves the market and normal market slot refill rules run.
- On purchase failure or modal cancel, the market card returns to its previous visual position.
- Cancelling the purchase confirmation modal with `돌아가기` does not change market, reservation, resource, or portfolio state.
- After modal cancel, a normal card returns to the normal market row and a reserved card returns to its lowered reserved position.
- Market card hover enlargement is hidden during market card drag.

## Reservation

- Reservation stays in the game.
- Reservation capacity is 1 card.
- Reservation does not consume a business day.
- Reservation does not grant Deal.
- The player can reserve and unreserve freely.
- Reservation and unreservation apply immediately without a confirmation modal.
- A market card is reserved by dragging it downward past a vertical threshold and releasing outside the portfolio area.
- Reserved cards are visually fixed slightly below their normal market slot position.
- A reserved card is unreserved by dragging it upward past the vertical threshold back toward the normal market row.
- There is no separate reservation drop zone.
- If a different card is reserved while one card is already reserved, the old reservation is automatically released and the new card becomes the only reserved card.
- Reserved cards remain in the market across next-business-day movement and market refresh.
- Reserved cards can still be bought through the same purchase rules as normal market cards.
- A short click/release on a reserved card opens the same purchase confirmation modal as a normal market card.
- Releasing a reserved card inside the portfolio area attempts immediate purchase, just like a normal market card.
- Buying a reserved card clears its reservation and runs the normal purchase refill behavior.
- The reserved card's lowered visual position is its interaction position for click, drag, and hover.

## Market Hover Enlargement

- Market card hover enlargement direction is based on card number.
- Market Tape Current Market Card Button 1-6 show the hover card at card position plus `PosX 300px, PosY 0px`.
- Market Tape Current Market Card Button 7-8 show the hover card at card position plus `PosX -300px, PosY 0px`.
- Reservation lowering does not change the direction rule; the offset is still based on the card number.
- If a reserved card is lowered, the hover offset is calculated from the lowered card position.
- No extra screen-boundary correction is part of the current decision.

## Stock Sale

- The stock sale execution area is always visible.
- The sale area has a red background and a `$` icon in its normal state.
- During owned-stock drag, the sale area changes to show the cash gain as `+N`.
- Owned stock cards no longer reveal a sell button on hover.
- Selling is done only by dragging and dropping an owned stock card onto the sale area.
- Hover enlargement for owned stock cards is not part of this scope.
- During owned-stock drag, the original portfolio card button is temporarily hidden.
- During owned-stock drag, a card detail panel matching the market hover panel follows the pointer.
- The owned-stock drag detail panel uses pointer-relative positioning: the panel's bottom-left corner is at the mouse pointer.
- Owned-stock drag detail excludes purchase cost because the card is already owned.
- Sale drop success is decided by pointer position.
- If the pointer is inside the sale area on drop, the stock is sold.
- If the pointer is outside the sale area on drop, the drag is cancelled, the panel closes, and the original portfolio card is shown again.
- Sale uses the original `StockSlots` index for the dragged owned stock.
