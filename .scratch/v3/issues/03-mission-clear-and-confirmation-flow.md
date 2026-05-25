# 03. 미션 클리어 판정과 최초 확정

Status: done

Type: AFK

## Parent

- `.scratch/v3/prd_v3.md`

## User stories covered

- 9, 10, 11, 12, 20, 21, 22, 23, 24

## What to build

미션 후보가 현재 포트폴리오를 기준으로 클리어 조건을 만족하는지 평가하고, 최초로 만족한 미션을 런의 확정 미션으로 고정한다. 클리어 조건은 정산 공식과 분리되어야 하며, 이 슬라이스에서는 확정까지의 흐름을 완성한다.

여러 후보가 같은 순간 조건을 만족하면 왼쪽 슬롯의 미션이 확정된다. 하나가 확정되면 나머지 후보 슬롯은 자동 폐기되고, 이후에는 새 미션을 확정할 수 없다.

## Acceptance criteria

- [x] 미션 클리어 조건과 정산 공식이 별도 데이터/상태로 관리된다.
- [x] 대상 태그 3장 이상 조건을 평가할 수 있다.
- [x] 대상 태그 5장 이상 조건을 평가할 수 있다.
- [x] 대상 태그 호일 1장 이상 조건을 평가할 수 있다.
- [x] 대상 태그의 고가치 카드 기준 조건을 평가할 수 있다.
- [x] 두 대상 태그를 각각 2장 이상 보유하는 조건을 평가할 수 있다.
- [x] 클리어 조건은 현재 보유 포트폴리오만 사용하며 예약 카드나 시장 카드를 포함하지 않는다.
- [x] 클리어 조건을 처음 만족한 후보는 즉시 확정 미션이 된다.
- [x] 여러 후보가 동시에 만족하면 왼쪽 슬롯 후보가 확정된다.
- [x] 미션 확정 즉시 나머지 후보 슬롯은 자동 폐기된다.
- [x] 확정 후에는 후보 UI 대신 확정 미션 1장만 표시된다.
- [x] 확정 후에는 다른 미션을 새로 확정할 수 없다.
- [x] 관련 EditMode 테스트가 각 클리어 조건, 동시 만족 우선순위, 후보 자동 폐기를 검증한다.
- [x] PlayMode 또는 UI-facing 테스트가 확정 전 후보 표시와 확정 후 단일 미션 표시를 검증한다.

## Blocked by

- `02-mission-candidates-and-slot-controls.md`

## Implementation notes

- Added `MissionConfirmationAction` as the public rule service for clear-condition evaluation and first confirmation.
- `MissionRunState` now carries `ConfirmedMission` separately from candidate slots. The winning slot remains visible in state; non-winning slots become empty.
- `PurchasePayment` evaluates mission confirmation after successful stock ownership changes and before business-day consumption.
- `MissionCandidateView` renders the normal three candidates before confirmation and a single confirmed mission after confirmation.

## Test evidence

- RED/GREEN: Added `MissionConfirmationActionTests` for fast-entry, concentration, foil, high-value, two-tag stable, owned-only evaluation, simultaneous leftmost priority, auto-discard, and no second confirmation.
- GREEN: Added `PurchasePaymentTests.ConfirmPurchaseImmediatelyConfirmsFirstClearedMission`.
- GREEN: Added `MainGameShellBootstrapShowsOnlyConfirmedMissionAfterConfirmation`.
- `powershell -ExecutionPolicy Bypass -File scripts/Run-UnityBatchmode.ps1 -Mode EditMode`: passed, 136/136.
- `powershell -ExecutionPolicy Bypass -File scripts/Run-UnityBatchmode.ps1 -Mode PlayMode`: passed, 62/62.

## Remaining risk

- Mission settlement and quarter-end mission revenue integration are intentionally deferred to the next issue slice.
