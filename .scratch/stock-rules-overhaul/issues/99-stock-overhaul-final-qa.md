# 99. 주식 개편 최종 QA와 폐기 UI 회귀 방지

Status: ready-for-human

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/PRD_20260520_feedback_overhaul.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

주식 규칙 대개편과 20260520 피드백 반영분이 Unity Play Mode에서 하나의 플레이 흐름으로 이어지는지 최종 검수한다. 이 이슈는 구현 이슈가 아니라 최종 QA 이슈이며, 번호를 `99`로 둬 후속 구현 이슈가 중간 번호에 추가될 수 있게 한다.

자동 테스트가 잡을 수 있는 규칙 회귀는 EditMode/PlayMode 테스트로 확인하고, 화면 읽힘과 포인터 기반 조작감은 사람이 수동 확인한다. 특히 폐기된 카드 상세보기 액션 화면, GainLiquidity 화면, 별도 예약 구역, Payment Pot, 보유 주식 hover Sell Button이 새 플레이 경로에 남아 있지 않은지 확인한다.

## QA Approach

- 구현 이슈 `01-12`, `14-20`의 완료 기준을 통합 흐름으로 다시 훑는다.
- EditMode는 순수 규칙 회귀를 확인한다.
- PlayMode는 Unity shell, UI wiring, pointer interaction, modal, drag/drop, hover presentation을 확인한다.
- 수동 QA는 자동 테스트가 확인하기 어려운 위치감, 드래그 감각, 카드 떨림, 모달 차단감, 비용 부족 색상 읽힘을 확인한다.
- 결과는 이슈 코멘트에 자동 테스트 명령/결과, 수동 QA 체크 결과, 남은 리스크로 기록한다.

## Acceptance criteria

- [ ] 새 런에서 주식 카드와 소모형 자원 카드가 1x8 시장에 표시된다.
- [ ] 시장 테이프 진행, 갱신, 당김이 예약 카드를 고려해 동작한다.
- [ ] 시장 카드 1-6의 호버 확대는 카드 기준 `+300px, 0px`, 7-8은 `-300px, 0px`에 표시된다.
- [ ] 예약된 시장 카드는 내려간 시각 위치를 기준으로 클릭, 드래그, 호버 확대가 동작한다.
- [ ] 투자 철학 보유량은 독서/명상/인내 각각 5까지만 제한되고 총합 10 제한은 적용되지 않는다.
- [ ] 투자 철학 마스터리는 독서/명상/인내 각각 0-5로 제한된다.
- [ ] 철학 HUD는 보유량을 큰 정수로 표시하고, 마스터리 1 이상만 작은 `+N`으로 표시한다.
- [ ] 딜 보유 한도는 적용되지 않는다.
- [ ] 딜은 자산 매수 결제에 직접 쓰이지 않고, 딜 드래그로 투자 철학 마스터리를 올리는 데만 쓰인다.
- [ ] 시장 카드, 호버 확대, 매수 확인 모달의 비용 표시는 마스터리 할인 전/후와 부족 비용 붉은 표시를 같은 규칙으로 보여준다.
- [ ] 주식 카드 짧은 클릭은 매수 가능할 때 확인 모달을 열고, 모달은 카드 상세 표시, 하단 긴 `확인`, 우측 상단 `돌아가기`를 제공한다.
- [ ] 소모형 자원 카드 짧은 클릭은 확인 모달 없이 즉시 매수를 시도한다.
- [ ] 매수 확인 모달이 열린 동안 배경 시장 카드, 포트폴리오, 딜 드래그 조작은 막힌다.
- [ ] 모달 `확인`은 매수 가능성을 다시 검증하고, `돌아가기`는 상태 변경 없이 카드 위치를 복구한다.
- [ ] 포트폴리오 영역에 시장 카드를 드롭하면 확인 모달 없이 즉시 매수를 시도한다.
- [ ] 시장 카드를 드래그 후 포트폴리오 영역 밖에 놓으면 아무 액션도 일어나지 않고 카드가 이전 위치로 복귀한다.
- [ ] 포트폴리오 영역 밖 드래그 릴리즈는 매수 모달, 즉시 매수, 메시지, 카드 떨림을 발생시키지 않는다.
- [ ] 비용 부족 매수 실패는 시스템 메시지 없이 카드 떨림만 발생한다.
- [ ] 비용 외 매수 실패는 카드 떨림과 기존 실패 메시지를 함께 보여준다.
- [ ] 매수 성공은 카드 떨림을 발생시키지 않고 정상 구매 결과로 이어진다.
- [ ] 일반 주식 카드 호버 시 카드 아래에 자물쇠 예약 버튼이 표시되고, 버튼 클릭은 카드 1장만 즉시 예약하며 새 예약은 기존 예약을 자동 해제한다.
- [ ] 소모형 자원 카드 호버 시 예약 버튼은 표시되지 않는다.
- [ ] 예약된 주식 카드 호버 시 내려간 카드 위에 예약 해제 버튼이 표시되고, 버튼 클릭은 즉시 예약 해제한다.
- [ ] 카드에서 예약/해제 버튼으로 포인터를 이동해도 버튼은 계속 표시된다.
- [ ] 예약/해제 버튼 클릭은 카드 클릭 매수로 전파되지 않는다.
- [ ] 예약된 카드는 카드 아래 면이 예약 버튼 아래 면 위치까지 내려간 상태로 표시된다.
- [ ] 예약과 예약 해제는 영업일, 딜, 월세 밀림을 변경하지 않는다.
- [ ] 예약 카드는 시장 진행/갱신에서 남아 있고, 같은 매수 규칙으로 구매할 수 있다.
- [ ] 예약 해제된 카드는 예약 해제 버튼 위치를 역산 기준으로 쓰지 않고 정상 시장 줄로 되돌아간다.
- [ ] 포트폴리오 점유 주식 슬롯 수가 런에서 처음 3/5/8에 도달할 때 각각 딜 1개를 지급한다.
- [ ] 포트폴리오 점유 주식 슬롯 수 보상은 같은 주식 ID라도 점유 슬롯 수 기준으로 계산하고, 이미 받은 보상은 재지급하지 않는다.
- [ ] 런 최초 호일 완성은 딜 1개를 지급하고, 슬롯 수 보상과 동시에 발생하면 둘 다 지급한다.
- [ ] 딜을 독서/명상/인내 HUD lane에 드롭하면 딜 1개를 소모하고 해당 마스터리를 1 올린다.
- [ ] 딜 칩 호버/드래그 시 안내 패널이 표시되고, 드래그 중 안내 패널의 우측 하단 모서리는 포인터 위치에 맞춰진다.
- [ ] 딜을 lane 밖이나 마스터리 5인 lane에 드롭하면 딜을 소모하지 않고 숨겼던 딜 칩을 다시 표시한다.
- [ ] 보유 주식 hover는 Sell Button을 노출하지 않는다.
- [ ] 항상 보이는 붉은 `$` 매도 실행 영역이 있다.
- [ ] 보유 주식 드래그 중 원래 포트폴리오 카드는 숨겨지고, 카드 상세 패널은 포인터를 따라다니며, 매도 영역은 `+N`을 표시한다.
- [ ] 포인터가 매도 실행 영역 안에서 드롭되면 원래 `StockSlots` 인덱스 기준으로 매도된다.
- [ ] 포인터가 매도 실행 영역 밖에서 드롭되면 매도는 취소되고 원래 포트폴리오 카드가 복구된다.
- [ ] 폐기된 카드 상세보기 액션 화면, Payment Pot, GainLiquidity 화면, 별도 예약 구역/예약 카드 목록 UI가 새 플레이 경로에 노출되지 않는다.
- [ ] 월세 밀림 10 도달 시 런 실패로 이동하고, 런 실패가 없으면 최종 정산까지 도달할 수 있다.
- [ ] 최종 정산은 최종 운용가치, 총 운용 수익, 보유 주식 수, 월세 밀림, 운용 코멘트를 현재 기준으로 표시한다.
- [ ] `scripts/Run-UnityBatchmode.ps1 -Mode EditMode` 결과가 이슈 코멘트에 기록된다.
- [ ] `scripts/Run-UnityBatchmode.ps1 -Mode PlayMode` 결과가 이슈 코멘트에 기록된다.
- [ ] 수동 QA 체크 결과와 남은 리스크가 이슈 코멘트에 기록된다.

## Blocked by

- `.scratch/stock-rules-overhaul/issues/01-stock-data-and-investment-philosophy-resources.md`
- `.scratch/stock-rules-overhaul/issues/02-consumable-resource-cards-replace-central-bank.md`
- `.scratch/stock-rules-overhaul/issues/03-split-market-decks-and-draw-rules.md`
- `.scratch/stock-rules-overhaul/issues/04-market-tape-1x8-progress-refresh-pull.md`
- `.scratch/stock-rules-overhaul/issues/05-market-slot-reservation.md`
- `.scratch/stock-rules-overhaul/issues/06-stock-purchase-payment-and-portfolio-cap.md`
- `.scratch/stock-rules-overhaul/issues/07-foil-merge-and-stock-removal.md`
- `.scratch/stock-rules-overhaul/issues/07a-readonly-portfolio-card-board.md`
- `.scratch/stock-rules-overhaul/issues/08-stock-sale-and-revenue-tracking.md`
- `.scratch/stock-rules-overhaul/issues/09-extra-buy-action-overhaul.md`
- `.scratch/stock-rules-overhaul/issues/10-hover-card-and-single-market-state.md`
- `.scratch/stock-rules-overhaul/issues/11-rent-arrears-and-bankruptcy.md`
- `.scratch/stock-rules-overhaul/issues/12-quarter-vacation-final-settlement-overhaul.md`
- `.scratch/stock-rules-overhaul/issues/14-investment-philosophy-mastery-and-discounted-costs.md`
- `.scratch/stock-rules-overhaul/issues/15-automatic-purchase-payment-and-failure-feedback.md`
- `.scratch/stock-rules-overhaul/issues/16-purchase-confirmation-modal.md`
- `.scratch/stock-rules-overhaul/issues/17-market-card-drag-and-portfolio-drop-purchase.md`
- `.scratch/stock-rules-overhaul/issues/18-single-card-lock-reservation-toggle.md`
- `.scratch/stock-rules-overhaul/issues/19-deal-rewards-and-mastery-drag.md`
- `.scratch/stock-rules-overhaul/issues/20-owned-stock-drag-sale.md`
