# 13. 주식 개편 통합 QA와 폐기 UI 회귀 방지

Status: ready-for-human

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

주식 규칙 대개편이 Unity Play Mode에서 하나의 플레이 흐름으로 이어지는지 검수한다. 자동 테스트가 잡을 수 있는 규칙 회귀는 관련 테스트로 확인하고, 화면의 읽힘, 카드 호버, 시장 테이프 이동, 예약/포트폴리오/월세 밀림/파산/최종 정산의 조작감은 사람이 확인한다.

이 이슈는 기존 MVP QA를 새 PRD 기준으로 대체한다. 특히 폐기된 카드 상세보기 화면, GainLiquidity 화면, 예약 구역, 3구역 시장이 새 플레이 경로에 남아 있지 않은지 확인한다.

## Existing implementation conflicts

- 기존 PlayMode 테스트는 3구역 시장, 카드 상세보기, Payment Pot, GainLiquidity, 예약 구역이 존재한다고 검증한다.
- `ProjectShell`은 낡은 UI 오브젝트를 자동 생성하거나 보존할 수 있다.
- 여러 rule 테스트는 Asset/ProfessionalResource/RedemptionPressure 이름과 old market zones를 기준으로 한다.
- 기존 QA 이슈는 old UI_GAMEPLAY_SPEC 흐름을 기준으로 한다.

## Refactor approach

- 낡은 UI 존재를 기대하는 테스트는 새 단일 시장/호버/1x8 테이프 기대값으로 교체한다.
- 폐기된 화면이 새 플레이 경로에서 열리지 않는 것을 명시적으로 검증한다.
- EditMode는 순수 규칙 회귀를, PlayMode는 실제 Unity 상호작용과 화면 연결을 확인하는 식으로 나눈다.
- 기존 QA 체크리스트는 새 delta issue 01-12의 완료 기준을 순서대로 훑는 통합 체크리스트로 재작성한다.

## Acceptance criteria

- [ ] 새 런에서 주식/소모형 자원 카드가 1x8 시장에 표시된다.
- [ ] 시장 테이프 진행, 갱신, 당김이 예약 슬롯을 고려해 동작한다.
- [ ] 주식 예약, 예약 주식 구매, 딜 지급, 월세 밀림 증가가 연결되어 보인다.
- [ ] 주식 매수, 소모형 자원 카드 매수, 주식 매도, 다음 영업일의 영업일 소비 여부가 새 규칙과 맞다.
- [ ] 포트폴리오 8칸 제한과 호일 합성 흐름이 플레이에서 확인된다.
- [ ] 카드 호버 확대가 동작하고, 카드 상세보기 화면으로 전환되지 않는다.
- [ ] 중앙 은행/GainLiquidity 화면이 새 플레이 경로에 노출되지 않는다.
- [ ] 월세 밀림 10 도달 시 파산으로 이동하고, 파산하지 않으면 최종 정산까지 도달할 수 있다.
- [ ] 최종 정산은 최종 가치, 총 수익, 보유 주식 수, 월세 밀림, 코멘트를 새 기준으로 표시한다.
- [ ] 관련 자동 테스트와 수동 QA 체크 결과가 이슈 코멘트에 기록된다.

## Blocked by

- `02-consumable-resource-cards-replace-central-bank.md`
- `03-split-market-decks-and-draw-rules.md`
- `04-market-tape-1x8-progress-refresh-pull.md`
- `05-market-slot-reservation.md`
- `06-stock-purchase-payment-and-portfolio-cap.md`
- `07-foil-merge-and-stock-removal.md`
- `08-stock-sale-and-revenue-tracking.md`
- `09-extra-buy-action-overhaul.md`
- `10-hover-card-and-single-market-state.md`
- `11-rent-arrears-and-bankruptcy.md`
- `12-quarter-vacation-final-settlement-overhaul.md`
