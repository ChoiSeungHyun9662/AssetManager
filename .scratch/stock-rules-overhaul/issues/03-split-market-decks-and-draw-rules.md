# 03. 주식 덱과 소모형 자원 덱 공급 규칙

Status: ready-for-agent

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

시장 카드 공급을 주식 덱과 소모형 자원 덱으로 분리한다. 새 시장 카드 1장을 뽑을 때마다 75% 확률로 주식 덱, 25% 확률로 소모형 자원 덱을 선택한다. 선택한 덱에서 뽑을 카드가 없으면 반대 덱으로 fallback한다.

소모형 자원 카드는 재순환 가능한 풀을 통해 덱에 되돌아올 수 있지만, 제거된 주식, 매도된 주식, 호일 완성으로 제거된 주식은 주식 덱으로 돌아오지 않는다.

## Existing implementation conflicts

- 현재 시장 공급은 `RunStaticDataSet.AssetCards`와 `MarketTape`의 단일 자산 카드 풀을 전제로 한다.
- `MarketTape`의 중복 방지와 removed 처리도 주식/소모형 자원 덱 분리를 알지 못한다.
- `MarketConfigData`는 3구역 슬롯 수 중심이며, draw weight나 재순환 풀을 표현하지 않는다.
- 기존 테스트는 visible/owned/reserved/removed 자산 중복 방지를 단일 카드 풀 기준으로 검증한다.

## Refactor approach

- 시장 공급 책임을 "새 카드 1장 요청" 단위의 별도 규칙으로 분리해 `MarketTape`가 덱 세부를 직접 알지 않게 한다.
- 주식 덱은 제거 상태를 영구 반영하고, 소모형 자원 덱은 재순환 가능한 풀을 별도로 가진다.
- 75/25 선택, fallback, 재순환 셔플을 순수 규칙으로 테스트한다.
- 기존 duplicate/removed 개념은 주식 덱 쪽으로 이관하고, 소모형 자원 카드는 재사용 가능 여부에 따라 별도 처리한다.

## Acceptance criteria

- [ ] 시장 공급은 주식 덱과 소모형 자원 덱을 별도로 가진다.
- [ ] 새 카드 공급 시 75%/25%의 soft draw 규칙으로 대상 덱을 선택한다.
- [ ] 선택한 덱이 비어 있으면 반대 덱에서 뽑는다.
- [ ] 소모형 자원 덱이 고갈되면 재순환 가능한 소모형 자원 카드 풀을 되돌리고 셔플한다.
- [ ] 제거/매도/호일 완성으로 빠진 주식은 주식 덱으로 되돌아오지 않는다.
- [ ] 양쪽 덱 모두 공급할 카드가 없는 경우 시장 슬롯을 조용히 비워두지 않고 예외 상태를 표시할 수 있다.
- [ ] 덱/풀에 카드가 들어간 경우 해당 덱/풀 셔플 규칙이 적용된다.

## Blocked by

- `01-stock-data-and-investment-philosophy-resources.md`
- `02-consumable-resource-cards-replace-central-bank.md`
