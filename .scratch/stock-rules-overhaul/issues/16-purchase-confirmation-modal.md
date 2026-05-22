# 16. 매수 확인 모달

Status: done

## Parent

- `.scratch/stock-rules-overhaul/PRD_20260520_feedback_overhaul.md`

## What to build

일반 시장 카드 매수 시도에 사용하는 확인 모달을 만든다. 이 모달은 카드 상세보기 액션 화면의 복귀가 아니라, 자산 매수 의도를 확인하는 최소 표면이다. 모달은 시장 카드 호버 확대와 같은 카드 상세 표시를 포함하고, 할인 비용 표시와 부족 비용 붉은 표시도 같은 규칙을 따른다.

모달 하단에는 긴 `확인` 버튼을 두고, 우측 상단에는 `돌아가기` 버튼을 둔다. 모달이 열려 있는 동안 배경 시장 카드, 포트폴리오, 딜 드래그 조작은 막는다. `확인`을 누를 때 매수 가능성을 다시 검증하고, 실패하면 모달을 닫은 뒤 동일한 실패 피드백을 적용한다. `돌아가기`는 아무 상태도 바꾸지 않고 카드 위치를 이전 상태로 돌린다.

## Acceptance criteria

- [x] 일반 매수 시도에서 현재 매수 가능한 시장 카드는 확인 모달을 연다.
- [x] 비용 부족이나 비용 외 실패 상태인 카드는 모달을 열지 않고 자동 매수 실패 피드백 규칙을 따른다.
- [x] 모달은 시장 호버 확대와 같은 카드 상세 정보를 보여준다.
- [x] 모달 비용 표시는 마스터리 할인과 부족 비용 붉은 표시 규칙을 공유한다.
- [x] 모달 하단의 긴 `확인` 버튼이 매수 확정 조작이다.
- [x] 모달 우측 상단의 `돌아가기` 버튼이 모달을 닫고 상태 변경 없이 이전 카드 위치를 복구한다.
- [x] 모달이 열린 동안 배경 시장 카드, 포트폴리오, 딜 드래그 조작은 실행되지 않는다.
- [x] `확인`을 누를 때 매수 가능성을 다시 검증한다.
- [x] `확인` 재검증 실패 시 모달을 닫고 비용 부족/비용 외 실패 피드백 규칙을 적용한다.
- [x] 관련 PlayMode 테스트가 모달 열기, 확인 성공, 확인 재검증 실패, 돌아가기 취소, 배경 입력 차단을 검증한다.

## Completion notes

- RED/GREEN: added PlayMode coverage for affordable market-card modal opening, modal confirm purchase, modal `돌아가기` cancellation with no run mutation, confirm-time revalidation failure, modal background input blocking, and discounted/insufficient modal cost display.
- Implementation: `MainGameShellBootstrap` now routes normal market-card clicks through purchase validation: valid cards open the modal, invalid cards immediately apply the existing purchase-failure feedback. The modal confirm button reuses `PurchasePayment.ConfirmPurchase`, so confirmation revalidates against current run state.
- UI: `PurchaseConfirmationView` renders a blocking modal with shared market-card detail formatting, a long bottom `확인` button, and a top-right `돌아가기` button. `MarketCardFormatter` centralizes the market hover/modal card summary text.
- Compatibility: legacy reservation-oriented PlayMode tests now enter card-detail working state directly when testing reservation compatibility, because normal market-card click is now purchase-confirmation intent.
- Verification: Unity EditMode `AssetManager.Tests.EditMode` passed 113/113. Unity PlayMode `AssetManager.Tests.PlayMode` passed 46/46.
- Unity manual check: not run. Automated PlayMode covered modal opening, button wiring, failure feedback, cancellation, and background command blocking.

## Blocked by

- `.scratch/stock-rules-overhaul/issues/14-investment-philosophy-mastery-and-discounted-costs.md`
- `.scratch/stock-rules-overhaul/issues/15-automatic-purchase-payment-and-failure-feedback.md`
