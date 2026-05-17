# 09. 추가 매수권 새 규칙 정합화

Status: ready-for-agent

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

기존 추가 매수권 지원을 새 카드 타입과 시장 규칙에 맞춘다. 추가 매수권은 다음 영업일로 이월되지 않고 중첩되지 않으며, 추가 매수권 상태에서는 예약할 수 없다. 추가 매수 전후 시장 빈칸은 시장 테이프 당김으로 처리한다.

소모형 자원 카드가 추가 매수권으로 구매 가능한지는 카드 효과 설계에서 제한 가능해야 한다. 어떤 경우에도 추가 매수권은 예약권으로 동작하지 않는다.

## Existing implementation conflicts

- `ExtraBuyAction`은 카드 상세보기와 시장/예약 카드 구매 흐름을 전제로 한다.
- 기존 추가 매수권은 예약 카드 구매와 시장 카드 구매를 모두 대상으로 삼을 수 있다.
- `BusinessDayState`의 추가 매수 대기 상태는 Market/CardDetail 상태 전환과 연결되어 있다.
- 기존 보충은 3구역 시장 column advance와 연결되어 있다.

## Refactor approach

- 추가 매수권은 단일 시장 상태 안의 선택 상태로 유지하고, 카드 상세보기 진입을 요구하지 않게 한다.
- 예약은 추가 매수권 사용 대상에서 제외한다.
- 추가 매수권으로 가능한 카드 타입은 주식과 허용된 소모형 자원 카드로 제한한다.
- 추가 매수 후 시장 빈칸은 1x8 시장 테이프 당김으로 처리한다.

## Acceptance criteria

- [ ] 추가 매수권은 다음 영업일로 이월되지 않는다.
- [ ] 추가 매수권은 중첩되지 않는다.
- [ ] 추가 매수권 상태에서 주식 예약은 불가능하다.
- [ ] 추가 매수권 사용 후에는 즉시 영업일을 종료한다.
- [ ] 추가 매수로 다시 추가 매수권이 발생하더라도 해당 효과는 무시된다.
- [ ] 추가 매수 전후 시장 빈칸은 시장 테이프 당김으로 처리한다.
- [ ] 추가 매수 후보는 주식 카드와, 허용된 경우 소모형 자원 카드까지 포함할 수 있다.
- [ ] 추가 매수권을 사용하지 않고 다음 영업일로 넘기면 권리는 폐기된다.

## Blocked by

- `02-consumable-resource-cards-replace-central-bank.md`
- `04-market-tape-1x8-progress-refresh-pull.md`
- `06-stock-purchase-payment-and-portfolio-cap.md`
