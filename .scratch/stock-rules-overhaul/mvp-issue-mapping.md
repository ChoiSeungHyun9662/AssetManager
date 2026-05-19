# MVP 이슈 매핑: 주식 규칙 대개편

Status: done

기준 문서:

- `.scratch/stock-rules-overhaul/PRD.md`
- `plan/22_stock_rules_overhaul.md`
- 기존 이슈: `.scratch/mvp/issues/`

이 문서는 기존 MVP 이슈를 주식 규칙 대개편 기준으로 분류한다. 기존 MVP 이슈 파일은 수정하지 않는다.

## 분류 기준

| 분류 | 의미 |
|---|---|
| 이미 완료 | 새 개편에서도 거의 그대로 유지 가능한 기반 작업 |
| 수정 필요 | 구현 또는 의도는 재사용 가능하지만 새 규칙에 맞춰 변경해야 하는 작업 |
| 폐기됨 | 이슈의 핵심 전제가 새 PRD와 충돌하므로 그대로 구현하거나 유지하면 안 되는 작업 |
| 새로 필요 | 기존 MVP 이슈에 없거나 충분히 다뤄지지 않은 새 개편 전용 작업 |

## 기존 MVP 이슈 매핑

| MVP 이슈 | 기존 상태 | 분류 | 판단 |
|---|---|---|---|
| `00-unity-project-creation-and-empty-playable-shell.md` | done | 이미 완료 | Unity 프로젝트, Bootstrap Scene, Main Game Scene, 빈 UI 루트는 규칙 개편과 무관한 기반이다. 그대로 유지한다. |
| `01-unity-mvp-run-shell-and-data-bootstrap.md` | done | 수정 필요 | 런 셸은 재사용 가능하지만 테스트 데이터와 부트스트랩은 주식/소모형 자원 카드, 독서/명상/인내, 월세 밀림, 2덱 구조에 맞게 교체해야 한다. |
| `02-calendar-business-day-and-schedule-loop.md` | done | 수정 필요 | 회계연도/분기/영업일 구조는 유지된다. 다만 영업일 시작 시 시장 테이프 진행, 분기 첫 영업일 refresh-only, 주식 매도 비소비 액션, 소모형 자원 카드 매수 소비 액션을 반영해야 한다. |
| `03-market-tape-display-draw-advance-and-refresh.md` | done | 수정 필요 | 시장 테이프 구현 의도는 유지되지만 3구역 구조가 1x8로 바뀐다. 진행/갱신/당김, 예약 슬롯 건너뛰기, 2덱 드로우, 75/25 fallback 규칙으로 재작성해야 한다. |
| `04-market-area-state-and-card-detail-view.md` | done | 폐기됨 | Market/CardDetail/GainLiquidity 상태 전환과 카드 상세보기 화면은 제거된다. 새 규칙은 단일 시장 상태와 카드 호버 확대를 사용한다. |
| `05-resource-ledger-and-resource-hud.md` | implemented-needs-editor-check, done으로 간주 | 수정 필요 | 사용자가 직접 확인해 기존 MVP 이슈는 완료로 간주한다. 자원 원장과 HUD 기반은 재사용 가능하며, 새 delta 이슈에서 리서치/신용/원자재를 독서/명상/인내로 바꾸고, 투자 철학 총 10/각 5 한도, 초과 획득 폐기, 수익/조달 현금 분리를 덧입힌다. |
| `06-market-card-purchase-and-minimal-chip-payment.md` | done | 수정 필요 | 결제 검증과 자원 소비 흐름은 재사용 가능하다. 주식 카드 매수, 소모형 자원 카드 매수, 딜 적용 범위, 포트폴리오 8칸 제한, 호일 예외, 시장 테이프 당김을 반영해야 한다. |
| `07-owned-assets-income-and-performance-tracking.md` | done | 수정 필요 | 보유 카드 수익 추적 기반은 재사용 가능하다. 자산/운용 수익을 주식/배당금/수익으로 바꾸고, 주식 매도 수익 포함, 소모형 자원 카드 현금 제외를 반영해야 한다. |
| `08-liquidity-action-view-and-resource-selection.md` | done | 폐기됨 | 중앙 은행, GainLiquidity 상태, 자원 확보 화면은 제거된다. 이 역할은 시장의 소모형 자원 카드 구매로 대체한다. |
| `09-market-card-reservation-deal-and-redemption-pressure.md` | done | 수정 필요 | 예약, 딜, 압력 증가라는 축은 유지된다. 다만 예약 구역 이동이 아니라 시장 슬롯 잠금으로 바꾸고, 환매 압력을 월세 밀림으로 바꾸며, 예약 직후 시장 테이프 진행이 아니라 영업일 종료 후 다음 영업일 시작 진행으로 맞춰야 한다. |
| `10-reserved-card-persistence-and-purchase.md` | done | 수정 필요 | 예약 카드 유지와 예약 카드 매수 개념은 유지된다. 별도 예약 카드 상태가 아니라 시장 슬롯의 예약 상태로 관리하고, 매수 시 예약 해제, 슬롯 비움, 시장 테이프 당김을 적용해야 한다. |
| `11a-inflation-cost-modifier-and-purchase-display.md` | done | 수정 필요 | 딜 할인 뒤 인플레이션 적용 규칙은 유지된다. 카드 상세보기 의존 UI를 제거하고, 주식 매수에만 딜을 허용하며, 매도 보상에도 인플레이션을 사용하도록 범위를 확장해야 한다. |
| `11-quarter-settlement-and-redemption-failure.md` | done | 수정 필요 | 분기 마감 구조는 유지된다. 운용 수익/환매 압력/런 실패 용어를 수익/월세 밀림/파산으로 바꾸고, 수익 구성에 배당금, 주식 매도 수익, 분기 정산 수익을 반영해야 한다. |
| `12-vacation-and-final-settlement-screens.md` | done | 수정 필요 | 4Q 휴가와 최종 정산 흐름은 유지된다. 최종 운용가치를 최종 가치로 바꾸고, 보유 주식 수, 월세 밀림, 최종 가치 기준 평가, 월세 밀림 단계별 코멘트를 반영해야 한다. |
| `13-extra-buy-action-support.md` | done | 수정 필요 | 추가 매수권 구조는 현재 계획 문서에 남아 있다. 주식/소모형 자원 카드 범위, 예약 불가, 시장 테이프 당김, 새 액션 목록과 충돌하지 않게 재확인해야 한다. |
| `14-mvp-play-mode-qa-and-regression-checks.md` | done | 수정 필요 | 기존 QA 기반은 유지하되 새 PRD 기준의 회귀 검증으로 다시 짜야 한다. 1x8 시장, 호버 확대, 소모형 자원 카드, 포트폴리오 8칸, 호일, 매도, 월세 밀림을 추가해야 한다. |
| `15-resource-object-assets-and-gain-liquidity-ui.md` | done | 수정 필요 | Bottom Chip Tray와 자원 오브젝트 방향은 재사용 가능하다. GainLiquidity 화면은 폐기하고, 아이콘/명칭을 독서/명상/인내/딜로 교체하며, 소모형 자원 카드 UI와 연결해야 한다. |
| `16-market-asset-card-icon-language.md` | done | 수정 필요 | 카드 정보의 아이콘 중심 문법은 유지 가능하다. 3구역 시장, 자산 카드, 운용 가치/운용 수익/태그 중심 표시는 주식 카드와 소모형 자원 카드 표시 규칙으로 다시 작성해야 한다. |
| `17-payment-pot-background-and-cost-slot-visuals.md` | ready-for-agent | 폐기됨 | 카드 상세보기의 Payment Pot 전제가 새 PRD와 충돌한다. 비용 슬롯/딜 시각화는 필요하지만, 별도 카드 상세보기 화면 안의 Payment Pot으로 구현하면 안 된다. |
| `18-card-detail-action-board-and-preview-state.md` | ready-for-agent | 폐기됨 | 카드 상세보기 액션 보드, 예비 시장 미리보기, Market Peek은 모두 새 규칙과 충돌한다. 새 규칙은 단일 시장 상태와 호버 확대를 사용한다. |
| `19-right-context-reservation-and-portfolio-board.md` | ready-for-agent | 수정 필요 | 포트폴리오 보드 방향은 유효하지만 예약 구역 3칸은 제거된다. 읽기 전용 1x8 포트폴리오 카드 보드는 `07a-readonly-portfolio-card-board.md`로 분리하고, 시장 슬롯 예약 상태와 수익 요약은 관련 후속 이슈에서 다시 써야 한다. |
| `20-quarter-result-board-in-shared-frame.md` | ready-for-agent | 수정 필요 | 공통 프레임 안의 분기 결과판 방향은 유지 가능하다. 운용 수익/환매 압력을 수익/월세 밀림으로 바꾸고 새 수익 구성과 파산 판정을 반영해야 한다. |
| `21-ui-gameplay-spec-integration-qa.md` | ready-for-human | 수정 필요 | 통합 QA 목적은 유지된다. 다만 검수 대상이 카드 상세보기, Payment Pot, 3구역 시장, GainLiquidity가 아니라 새 주식 규칙 흐름으로 바뀐다. |

## 분류별 요약

### 이미 완료

- `00-unity-project-creation-and-empty-playable-shell.md`

### 수정 필요

- `01-unity-mvp-run-shell-and-data-bootstrap.md`
- `02-calendar-business-day-and-schedule-loop.md`
- `03-market-tape-display-draw-advance-and-refresh.md`
- `05-resource-ledger-and-resource-hud.md`
- `06-market-card-purchase-and-minimal-chip-payment.md`
- `07-owned-assets-income-and-performance-tracking.md`
- `09-market-card-reservation-deal-and-redemption-pressure.md`
- `10-reserved-card-persistence-and-purchase.md`
- `11a-inflation-cost-modifier-and-purchase-display.md`
- `11-quarter-settlement-and-redemption-failure.md`
- `12-vacation-and-final-settlement-screens.md`
- `13-extra-buy-action-support.md`
- `14-mvp-play-mode-qa-and-regression-checks.md`
- `15-resource-object-assets-and-gain-liquidity-ui.md`
- `16-market-asset-card-icon-language.md`
- `19-right-context-reservation-and-portfolio-board.md`
- `20-quarter-result-board-in-shared-frame.md`
- `21-ui-gameplay-spec-integration-qa.md`

### 폐기됨

- `04-market-area-state-and-card-detail-view.md`
- `08-liquidity-action-view-and-resource-selection.md`
- `17-payment-pot-background-and-cost-slot-visuals.md`
- `18-card-detail-action-board-and-preview-state.md`

## 새로 필요

기존 MVP 이슈만으로는 충분히 커버되지 않는 새 개편 전용 작업이다. 후속 `to-issues`를 진행한다면 아래 항목을 delta issue 후보로 삼는다.

1. 주식 전용 데이터/용어 전환
   - 자산을 주식으로 교체한다.
   - 운용 가치/운용 수익을 가치/배당금/수익으로 분리한다.
   - 최종 평가와 표시 텍스트를 새 용어로 맞춘다.

2. 투자 철학 자원 전환과 보유 한도
   - 리서치/신용/원자재를 독서/명상/인내로 교체한다.
   - 투자 철학 총 10, 각 종류별 5 한도를 적용한다.
   - 초과 획득 폐기 메시지를 추가한다.

3. 소모형 자원 카드
   - 중앙 은행과 GainLiquidity를 대체한다.
   - 현금 획득 카드와 투자 철학 획득 카드를 시장 카드로 구현한다.
   - 구매 시 즉시 효과를 발동하고 카드가 사라지게 한다.
   - 소모형 자원 카드 현금은 수익에 포함하지 않는다.

4. 주식 덱과 소모형 자원 덱 분리
   - 75% 주식 덱, 25% 소모형 자원 덱 선택 규칙을 구현한다.
   - 선택 덱 고갈 시 반대 덱 fallback을 허용한다.
   - 소모형 자원 카드 재순환과 셔플을 구현한다.
   - 제거/매도/호일 완성 주식은 덱에 되돌리지 않는다.

5. 1x8 시장 테이프와 시장 테이프 당김
   - 기존 3구역 시장을 1x8 시장으로 바꾼다.
   - 진행, 갱신, 당김을 새 규칙으로 구현한다.
   - 모든 빈칸 처리를 시장 테이프 당김으로 통일한다.
   - 예약 슬롯 건너뛰기와 복수 빈칸 순차 당김을 검증한다.

6. 시장 슬롯 예약
   - 예약 구역을 제거한다.
   - 시장 슬롯의 주식 잠금으로 예약을 관리한다.
   - 예약된 주식은 진행/갱신/당김의 영향을 받지 않는다.
   - 예약 보상 딜, 월세 밀림 +1, 예약 최대 3을 적용한다.

7. 포트폴리오 8칸과 호일 합성
   - 최대 8칸 포트폴리오 슬롯을 구현한다.
   - 8칸 초과 매수를 차단한다.
   - 동일 주식 3장 매수 시 즉시 호일 주식으로 합친다.
   - 호일 완성 후 시장/덱/예약 슬롯의 같은 종목 주식을 제거한다.

7a. 읽기 전용 1x8 포트폴리오 카드 보드
   - 기존 포트폴리오 텍스트 보유 목록을 1x8 카드 보드로 대체한다.
   - `OwnedAssetState.StockSlots`의 순서와 빈 슬롯을 그대로 표시한다.
   - 빈 슬롯은 텍스트 없는 빈 카드 프레임으로 보여준다.
   - 보유 주식 슬롯은 이름, 등급, 현재 가치, 배당금, 호일 여부를 표시한다.
   - 구매/예약/결제/매도 조작은 넣지 않고, 매도 조작은 주식 매도 이슈로 넘긴다.

8. 주식 매도
   - 주식 매도는 영업일을 소비하지 않는다.
   - 일반 주식은 현금 1 x 인플레이션, 호일 주식은 현금 3 x 인플레이션을 지급한다.
   - 매도 수익은 수익에 포함한다.
   - 매도된 주식은 이번 게임에서 제거한다.

9. 카드 호버 확대와 단일 시장 상태
   - 카드 상세보기 화면을 제거한다.
   - 호버 시 동일 정보의 확대 카드를 표시한다.
   - 시장 영역은 단일 상태로 유지한다.
   - 주식 카드와 소모형 자원 카드의 표시 정보를 분리한다.

10. 월세 밀림과 파산 전환
    - 환매 압력을 월세 밀림으로 교체한다.
    - 월세 밀림 10 도달 시 파산으로 게임 오버 처리한다.
    - 예약과 분기 실패의 월세 밀림 증가를 연결한다.

11. 최종 정산 갱신
    - 최종 가치는 보유 주식 가치 합계로 계산한다.
    - 총 수익은 배당금, 주식 매도 수익, 분기 정산 수익을 포함한다.
    - 월세 밀림 단계와 최종 등급에 따른 코멘트를 표시한다.

12. 주식 개편 전용 QA
    - 기존 MVP QA를 새 PRD 기준으로 재작성한다.
    - 1x8 시장, 당김, 예약, 소모형 자원 카드, 읽기 전용 1x8 포트폴리오 보드, 호일, 매도, 월세 밀림, 파산, 최종 정산을 검증한다.

## 권장 후속 작업

1. 기존 `done` MVP 이슈는 그대로 둔다.
2. 기존 open MVP 이슈 중 낡은 규칙을 구현하게 만드는 이슈는 `superseded` 상태와 새 delta issue 참조로 정리한다.
3. 새 구현은 `.scratch/stock-rules-overhaul/issues/`의 delta issue를 기준으로 진행한다.
4. "수정 필요"로 분류된 기존 구현은 delta issue 안에서 재사용 가능한 기반으로 취급한다.
5. "폐기됨"으로 분류된 기존 구현이 코드에 남아 있다면 제거 또는 비활성화 작업을 관련 delta issue에서 처리한다.

## 발행 결과

승인된 delta issue는 `.scratch/stock-rules-overhaul/issues/` 아래에 발행했다.

| Delta issue | Type | 기존 구현과의 관계 |
|---|---|---|
| `01-stock-data-and-investment-philosophy-resources.md` | AFK | 기존 런 데이터와 자원 원장 기반을 새 용어/한도로 전환 |
| `02-consumable-resource-cards-replace-central-bank.md` | AFK | 중앙 은행과 GainLiquidity를 시장 카드 구매로 대체 |
| `03-split-market-decks-and-draw-rules.md` | AFK | 기존 시장 드로우를 2덱 75/25 공급으로 대체 |
| `04-market-tape-1x8-progress-refresh-pull.md` | AFK | 기존 3구역 시장 테이프를 1x8 진행/갱신/당김으로 대체 |
| `05-market-slot-reservation.md` | AFK | 기존 예약 구역을 시장 슬롯 예약으로 대체 |
| `06-stock-purchase-payment-and-portfolio-cap.md` | AFK | 기존 매수/결제 기반에 주식 매수, 딜 범위, 8칸 제한 적용 |
| `07-foil-merge-and-stock-removal.md` | AFK | 기존 보유 카드 흐름에 호일 합성과 종목 제거 추가 |
| `07a-readonly-portfolio-card-board.md` | AFK | 기존 텍스트 보유 목록을 `OwnedAssetState.StockSlots` 기반 읽기 전용 1x8 포트폴리오 카드 보드로 대체 |
| `08-stock-sale-and-revenue-tracking.md` | AFK | 기존 수익 추적에 주식 매도와 새 수익 정의 추가 |
| `09-extra-buy-action-overhaul.md` | AFK | 기존 추가 매수권을 새 시장/카드 타입에 맞춤 |
| `10-hover-card-and-single-market-state.md` | AFK | 카드 상세보기/GainLiquidity 제거와 호버 확대 도입 |
| `11-rent-arrears-and-bankruptcy.md` | AFK | 환매 압력/런 실패를 월세 밀림/파산으로 전환 |
| `12-quarter-vacation-final-settlement-overhaul.md` | AFK | 분기/휴가/최종 정산을 새 수익/가치 기준으로 갱신 |
| `13-stock-overhaul-integration-qa.md` | HITL | 새 주식 개편 흐름 전체 QA |

기존 MVP open issue 중 `17`, `18`, `19`, `20`, `21`은 `superseded`로 표시하고 관련 delta issue를 참조하도록 정리했다.
기존 `implemented-needs-editor-check` 상태였던 `05`는 사용자 확인에 따라 완료로 간주하고 수정하지 않았다.
