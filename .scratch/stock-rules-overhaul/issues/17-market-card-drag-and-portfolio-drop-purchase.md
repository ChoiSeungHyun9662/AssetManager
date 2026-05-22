# 17. 시장 카드 드래그와 포트폴리오 드롭 즉시 매수

Status: verification-blocked

## Parent

- `.scratch/stock-rules-overhaul/PRD_20260520_feedback_overhaul.md`

## What to build

시장 카드 입력을 포인터 기반 드래그/릴리즈 조작으로 바꾼다. 플레이어는 시장 카드를 누른 뒤 위아래로 자유롭게 움직일 수 있고, 드래그 중에는 원본 카드가 포인터를 따라 움직인다. 드래그가 시작되면 시장 카드 호버 확대는 숨긴다.

릴리즈 지점이 포트폴리오 영역 안이면 확인 모달 없이 즉시 자산 매수를 시도한다. 이 포트폴리오 드롭 즉시 매수는 이후 예약 임계값보다 우선한다. 즉시 매수가 실패하면 자동 매수 실패 피드백 규칙을 그대로 따른다. 릴리즈 지점이 포트폴리오 영역이 아니고 예약 임계값을 넘지 않은 경우에는 매수 확인 모달 흐름으로 넘어간다.

짧은 클릭은 포인터를 거의 움직이지 않은 `down -> release`로 취급한다. 따라서 시장 카드 클릭은 포트폴리오 영역 밖에서 예약 임계값을 넘지 않은 릴리즈와 동일하게 매수 확인 모달 흐름으로 넘어간다.

시장 카드 호버 확대 위치도 이 이슈에서 확정한다. Market Tape Current Market Card Button 1-6은 각 카드 기준 `PosX +300px, PosY 0px` 위치에 호버 확대 카드를 표시하고, Button 7-8은 각 카드 기준 `PosX -300px, PosY 0px` 위치에 표시한다. 드래그 중에는 호버 확대를 숨기고, 드래그 종료 후 다시 호버하면 이 위치 규칙을 따른다.

## Acceptance criteria

- [x] 시장 카드는 포인터 down 이후 릴리즈 전까지 위아래로 드래그할 수 있다.
- [x] 시장 카드 드래그 중 원본 카드가 포인터 이동을 따라 움직인다.
- [x] 시장 카드 드래그 시작 시 해당 카드의 호버 확대 패널은 숨겨진다.
- [ ] Market Tape Current Market Card Button 1-6은 각 카드 기준 `PosX +300px, PosY 0px`에 호버 확대 카드를 표시한다.
- [ ] Market Tape Current Market Card Button 7-8은 각 카드 기준 `PosX -300px, PosY 0px`에 호버 확대 카드를 표시한다.
- [ ] 시장 카드 드래그 종료 후 다시 호버하면 카드 번호별 호버 확대 위치 규칙을 따른다.
- [x] 포트폴리오 영역 안에서 시장 카드를 놓으면 확인 모달 없이 즉시 매수를 시도한다.
- [x] 포트폴리오 영역 릴리즈는 예약 임계값보다 우선한다.
- [x] 포트폴리오 드롭 즉시 매수 성공 시 카드가 시장에서 빠지고 기존 슬롯 보충 규칙이 실행된다.
- [x] 포트폴리오 드롭 즉시 매수 실패 시 자동 매수 실패 피드백 규칙을 적용하고 카드를 이전 위치로 복구한다.
- [x] 시장 카드의 짧은 클릭은 매수 확인 모달 흐름으로 넘어간다.
- [x] 포트폴리오 영역 밖에서 예약 임계값을 넘지 않고 놓으면 매수 확인 모달 흐름으로 넘어간다.
- [x] 모달 취소나 매수 실패 후 일반 카드는 원래 시장 줄 위치로 돌아간다.
- [x] 관련 PlayMode 테스트가 드래그 중 호버 숨김, 짧은 클릭 모달 진입, 포트폴리오 드롭 즉시 매수, 실패 복구, 일반 릴리즈 모달 진입을 검증한다.

## Completion notes

- Assumptions: issue 18 owns downward reservation/unreservation behavior; this issue only gives portfolio-area release priority and preserves the non-portfolio purchase-modal path when no reservation action is taken.
- RED/GREEN: added PlayMode coverage for drag hiding hover and moving the original card, portfolio-area drop immediate purchase without modal, non-portfolio drag release opening the purchase confirmation modal, and failed portfolio drop restoring the card with existing failure feedback.
- Implementation: `MarketTapeView` now tracks pointer down/drag/up for current-market cards, moves the original card during drag, hides hover while dragging, restores the card visual on release, and reports release position to `MainGameShellBootstrap`.
- Purchase routing: `MainGameShellBootstrap` sends portfolio-area releases directly through existing purchase validation/payment and failure feedback, while non-portfolio releases reuse the existing confirmation-modal path.
- Verification: Unity PlayMode `AssetManager.Tests.PlayMode` passed 50/50.
- Unity manual check: not run. Automated PlayMode covered the required pointer interaction and modal/purchase/failure wiring.

## Post-completion correction

- 2026-05-23 feedback review found that Feedback 1's explicit hover-position requirement was only present in final QA, not in a concrete implementation issue.
- This issue is reopened to own the market hover enlargement placement rule for current-market buttons 1-8.
- Existing completion notes above still describe the previously completed drag/purchase work; the new unchecked acceptance criteria describe the remaining hover-position work.

## 2026-05-23 continuation notes

- RED intent added: PlayMode tests now specify that current-market card 1 hover appears at the card position plus `PosX +300px, PosY 0px`, card 8 hover appears at `PosX -300px, PosY 0px`, and re-hover after a completed drag uses the restored slot position rule.
- GREEN implementation added: `MarketTapeView` passes the current-market slot index into hover handling and positions the hover panel from the hovered card's current `RectTransform.position` using `+300px` for slots 1-6 and `-300px` for slots 7-8.
- Verification blocked: Unity PlayMode execution could not be run because Codex escalation was rejected by the current usage limit. `git diff --check` passed for the touched files. Keep the three new acceptance criteria unchecked until `AssetManager.Tests.PlayMode` runs successfully.

## Blocked by

- `.scratch/stock-rules-overhaul/issues/15-automatic-purchase-payment-and-failure-feedback.md`
- `.scratch/stock-rules-overhaul/issues/16-purchase-confirmation-modal.md`
