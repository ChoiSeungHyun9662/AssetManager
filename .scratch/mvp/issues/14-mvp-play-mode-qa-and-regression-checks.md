# 14. MVP Play Mode QA와 회귀 검증

Status: done

## Parent

.scratch/mvp/PRD.md

## What to build

MVP 성공 기준이 Unity 엔진 안에서 모두 구현되었는지 확인할 수 있는 검증 세트를 만든다. 자동화 가능한 규칙은 테스트로 고정하고, 에디터에서 직접 눌러봐야 하는 흐름은 Play Mode QA 체크리스트로 만든다. 이 slice의 목표는 "코드가 있다"가 아니라 "Unity에서 PRD의 핵심 경로가 실제로 된다"를 검증하는 것이다.

## Code work

- RunCalendar, ResourceLedger, MarketTape, PurchasePayment, LiquidityAction, RedemptionPressure, QuarterSettlement, FinalSettlement의 핵심 순수 규칙 테스트를 만든다.
- MarketAreaState와 BusinessDayFlow의 상태 전환 테스트를 만든다.
- 기본 진행, 자산 매수, 자원 확보, 예약, 환매 압력 실패, 최종 정산 smoke scenario를 자동 또는 반자동 테스트로 고정한다.
- 테스트 데이터는 밸런스가 아니라 규칙 검증에 필요한 최소 fixture로 둔다.
- 실패 시 어떤 규칙이 깨졌는지 알 수 있는 테스트 이름을 사용한다.

## Unity Editor work

- QA용 Scene 또는 QA 모드를 준비한다.
- 테스트 데이터 세트를 Inspector에서 쉽게 교체하거나 확인할 수 있게 한다.
- Play Mode QA 체크리스트 문서를 만든다.
- 각 시나리오별 시작 상태를 빠르게 만들 수 있는 개발용 버튼이나 디버그 메뉴를 제공한다.
- 최종 검증 시 Scene, Prefab, ScriptableObject, UI 연결 누락이 없는지 확인한다.

## Verification

- 자동 테스트 또는 반자동 테스트로 핵심 deep module 규칙이 검증된다.
- Play Mode에서 기본 진행 smoke scenario가 성공한다.
- Play Mode에서 시장 카드 매수 smoke scenario가 성공한다.
- Play Mode에서 자원 확보 smoke scenario가 성공한다.
- Play Mode에서 예약과 환매 압력 smoke scenario가 성공한다.
- Play Mode에서 환매 압력 10 실패 smoke scenario가 성공한다.
- Play Mode에서 3회계년도 4Q 이후 최종 정산 smoke scenario가 성공한다.

## Acceptance criteria

- [x] PRD의 MVP smoke scenario 6개가 Unity에서 검증 가능하다.
- [x] 핵심 규칙 모듈의 자동 테스트 또는 명시적 회귀 테스트가 있다.
- [x] Play Mode QA 체크리스트가 이슈 또는 문서로 남는다.
- [x] 모든 주요 UI 입력은 Scene에서 실제 버튼/카드/중앙 은행 조작으로 확인된다.
- [x] 완료 기준은 코드 존재가 아니라 Unity 실행 결과로 판정한다.
- [x] 누락된 에디터 연결이나 데이터 연결이 있으면 QA에서 잡을 수 있다.
- [x] 최종 상태에서 새 런부터 실패 또는 최종 정산까지 한 흐름이 플레이 가능하다.

## Blocked by

- .scratch/mvp/issues/12-vacation-and-final-settlement-screens.md
- .scratch/mvp/issues/13-extra-buy-action-support.md

## User stories covered

1, 2, 3, 6, 9, 10, 11, 15, 16, 32, 40, 49, 51, 52, 53, 54, 55, 58

## Comments

- 2026-05-14: Implemented issue 14 with TDD. Added `MvpSmokeScenarioTests` PlayMode coverage for the six MVP smoke scenarios: basic progression, market purchase/income, liquidity, reservation, redemption-pressure failure, and final settlement. Added `.scratch/mvp/play-mode-qa-checklist.md` for manual Play Mode QA. Verified Unity PlayMode 26/26 passed and EditMode 66/66 passed via `scripts/Run-UnityBatchmode.ps1`.
