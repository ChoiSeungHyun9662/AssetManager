# 02. 소모형 자원 카드로 중앙 은행 대체

Status: ready-for-agent

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

중앙 은행과 자원 확보 화면을 시장에 등장하는 소모형 자원 카드로 대체한다. 플레이어는 시장에서 현금 획득 카드 또는 투자 철학 획득 카드를 구매해 즉시 자원을 얻는다. 이 카드는 포트폴리오에 들어가지 않고, 구매 후 효과를 발동하고 사라진다.

기존 GainLiquidity 화면은 새 흐름에서 사용하지 않는다. 이 이슈는 자원 획득을 기존 시장 구매 흐름과 연결하는 첫 세로 조각이다.

## Existing implementation conflicts

- `LiquidityAction`, `LiquidityActionState`, `LiquidityActionView`는 중앙 은행/GainLiquidity 화면에서 자원을 직접 선택하는 구조다.
- `ProjectShell`과 `MainGameShellBootstrap`는 중앙 은행 또는 GainLiquidity UI를 생성하고 연결한다.
- `PurchasePayment`는 시장 카드 구매 결과가 보유 자산 전환이라고 가정한다.
- 기존 리소스 개발 버튼과 QA는 자원 확보 화면이 존재한다고 기대한다.

## Refactor approach

- 자원 획득은 별도 액션 화면이 아니라 시장 카드 구매 결과로 이동시킨다.
- 시장 카드 런타임에 주식 카드와 소모형 자원 카드 타입을 분리하고, 구매 확정 시 카드 타입에 따라 보유 주식 추가 또는 즉시 자원 지급으로 분기한다.
- 중앙 은행/GainLiquidity UI는 새 플레이 경로에서 끊고, 관련 테스트는 소모형 자원 카드 구매 테스트로 대체한다.
- 소모형 자원 카드가 지급한 현금은 `ResourceLedger`의 funding cash 경로를 사용해 수익 카운터와 분리한다.

## Acceptance criteria

- [ ] 시장 카드 타입으로 주식 카드와 소모형 자원 카드를 구분할 수 있다.
- [ ] 소모형 자원 카드는 이미지, 등급, 제공 자원, 코스트를 표시하고 이름은 표시하지 않는다.
- [ ] 현금 획득 소모형 카드는 구매 시 현금을 지급한다.
- [ ] 투자 철학 획득 소모형 카드는 구매 시 독서/명상/인내 중 지정 자원을 지급한다.
- [ ] 소모형 자원 카드는 현금 비용만 요구하고, 투자 철학 비용과 딜을 사용하지 않는다.
- [ ] 소모형 자원 카드 구매는 영업일을 소비한다.
- [ ] 소모형 자원 카드로 얻은 현금은 수익에 포함되지 않는다.
- [ ] 중앙 은행 액션과 GainLiquidity 진입 경로는 새 플레이 흐름에서 사용되지 않는다.

## Blocked by

- `01-stock-data-and-investment-philosophy-resources.md`
