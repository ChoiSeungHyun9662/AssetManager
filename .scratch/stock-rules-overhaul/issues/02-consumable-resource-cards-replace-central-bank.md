# 02. 소모형 자원 카드로 중앙 은행 대체

Status: done

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

- [x] 시장 카드 타입으로 주식 카드와 소모형 자원 카드를 구분할 수 있다.
- [x] 소모형 자원 카드는 이미지, 등급, 제공 자원, 코스트를 표시하고 이름은 표시하지 않는다.
- [x] 현금 획득 소모형 카드는 구매 시 현금을 지급한다.
- [x] 투자 철학 획득 소모형 카드는 구매 시 독서/명상/인내 중 지정 자원을 지급한다.
- [x] 소모형 자원 카드는 현금 비용만 요구하고, 투자 철학 비용과 딜을 사용하지 않는다.
- [x] 소모형 자원 카드 구매는 영업일을 소비한다.
- [x] 소모형 자원 카드로 얻은 현금은 수익에 포함되지 않는다.
- [x] 중앙 은행 액션과 GainLiquidity 진입 경로는 새 플레이 흐름에서 사용되지 않는다.

## Completion notes

- `AssetCardData`에 `CardDomain.ConsumableResource`, 제공 자원 타입, 제공 수량을 추가해 시장 카드가 주식과 소모형 자원 카드로 갈라질 수 있게 했다.
- `PurchasePayment`는 주식 카드는 기존처럼 보유 자산으로 전환하고, 소모형 자원 카드는 현금 비용만 결제한 뒤 즉시 조달 현금 또는 투자 철학을 지급하고 제거한다.
- 소모형 현금 카드는 `ResourceLedger.AddFundingCash` 경로를 사용해 분기/회계년도/총 운용 수익에 포함되지 않는다.
- `ProjectShell`과 `MainGameShellBootstrap`에서 중앙 은행/GainLiquidity UI 생성과 연결을 새 플레이 흐름에서 끊고, 기존 씬에 남은 레거시 오브젝트는 부트스트랩 중 제거한다.
- `MarketTapeView`는 소모형 자원 카드를 이름 없이 코스트, 희귀도, 제공 자원 중심으로 렌더링한다.
- `docs/agents/class-inventory.md`를 새 카드 타입과 구매/표시 책임에 맞게 갱신했다.

## Verification

- RED/GREEN 1: 소모형 현금 카드 매수는 처음에 카드 타입/제공 자원 필드가 없어 EditMode 컴파일 RED가 났고, 이후 보유 자산 없이 조달 현금을 지급하고 영업일을 소비하도록 통과시켰다.
- RED/GREEN 2: 소모형 투자 철학 카드가 작성된 전문 비용 슬롯을 만들던 RED를 확인하고, 소모형 카드는 cash-only 결제로 통과시켰다.
- RED/GREEN 3: 중앙 은행/GainLiquidity PlayMode 테스트를 새 PRD 기대치로 교체하고, 새 플레이 흐름에서 해당 UI와 진입이 사라지도록 통과시켰다.
- RED/GREEN 4: 소모형 자원 카드가 시장에서 이름/배당/운용가치를 표시하던 RED를 확인하고, 코스트/희귀도/제공 자원 표시로 통과시켰다.
- 최종 Unity EditMode `AssetManager.Tests.EditMode`: 73/73 passed.
- 최종 Unity PlayMode `AssetManager.Tests.PlayMode`: 33/33 passed.
- Unity 수동 확인은 별도로 실행하지 않았다. PlayMode가 시장 표시, 버튼 연결, 중앙 은행 제거, 스모크 흐름을 검증했다.

## Remaining risks

- 소모형 자원 카드의 별도 덱, 75/25 시장 드로우, 재활용 규칙은 후속 `03-split-market-decks-and-draw-rules.md` 범위로 남아 있다.
- `LiquidityAction`, `LiquidityActionState`, `LiquidityActionView` 타입 자체는 호환/후속 정리 대상으로 남아 있지만, 새 플레이 UI 경로에서는 생성/연결되지 않는다.

## Blocked by

None - can start immediately
