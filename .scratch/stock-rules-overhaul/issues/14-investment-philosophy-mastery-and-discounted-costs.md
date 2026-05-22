# 14. 투자 철학 마스터리와 할인 비용 표면

Status: done

## Parent

- `.scratch/stock-rules-overhaul/PRD_20260520_feedback_overhaul.md`

## What to build

투자 철학 보유량과 투자 철학 마스터리를 새 피드백 기준으로 정리한다. 독서, 명상, 인내 보유량은 각 5까지만 제한하고 기존 총합 10 제한은 제거한다. 딜 보유 한도도 제거한다.

런 안에 독서, 명상, 인내 마스터리를 0-5 범위로 저장하고, 시장 카드 비용을 표시하거나 매수 가능성을 검증하거나 실제로 지불할 때 원본 카드 비용에 마스터리 할인을 적용한다. 원본 카드 비용 데이터는 바꾸지 않는다. 할인 후 투자 철학 비용은 0보다 작아질 수 없고, 0이 된 비용은 지불 시 소모되지 않는다.

비용 표시 표면은 할인 전 전체 비용을 먼저 보여주고, 할인 발생 시 오른쪽에는 할인 후 투자 철학 비용만 보여준다. 비용 부족 토큰은 선택적으로 붉게 표시할 수 있어야 한다. 철학 HUD는 보유량을 큰 정수로 표시하고, 마스터리가 1 이상일 때만 작은 `+N`을 붙인다.

## Acceptance criteria

- [x] 독서, 명상, 인내 보유량은 각각 5까지만 보유할 수 있고 총합 10 제한은 더 이상 적용되지 않는다.
- [x] 딜 보유량은 기존 딜 한도로 잘리지 않는다.
- [x] 독서, 명상, 인내 마스터리는 런 상태에 저장되며 각각 0-5 범위로 제한된다.
- [x] 마스터리 할인은 카드 원본 비용을 변경하지 않고 표시, 검증, 지불 시점에만 적용된다.
- [x] 할인 후 투자 철학 비용은 0 미만이 되지 않고, 0이 된 비용은 지불 시 소모되지 않는다.
- [x] 할인 없음 비용은 `50$, R1, M1`처럼 표시할 수 있다.
- [x] 할인 있음 비용은 `50$, R1, M1 -> R0, M1`처럼 원본 전체 비용과 할인 후 투자 철학 비용을 표시할 수 있다.
- [x] 현재 보유량 기준으로 부족한 비용 토큰만 붉게 표시할 수 있다.
- [x] 철학 HUD는 보유량을 큰 정수로 표시하고, 마스터리 0은 숨기며, 마스터리 1 이상은 작은 `+N`으로 표시한다.
- [x] 관련 EditMode 테스트가 투자 철학 cap 제거, 딜 cap 제거, 마스터리 cap, 할인 계산, 원본 비용 불변, 부족 비용 토큰 판정을 검증한다.
- [x] 관련 PlayMode 또는 UI 테스트가 철학 HUD 보유량/마스터리 표시를 검증한다.

## Agent notes

- RED/GREEN intent: added behavior tests for per-type-only philosophy caps, uncapped Deal, mastery cap, cost display formatting, insufficient cost token marking, discounted purchase consumption without mutating source card costs, and HUD holding/mastery display.
- Implementation: added run-scoped `InvestmentPhilosophyMasteryState`, `ResourceLedger.AddInvestmentPhilosophyMastery`, reusable `PurchaseCostCalculator`, mastery-discounted payment slot creation, and HUD text formatting for holdings plus mastery.
- Compatibility cleanup: updated liquidity selection to ignore the former total philosophy cap and updated legacy tests that expected Deal overflow.
- Verification run: `dotnet build "Asset Manager\AssetManager.Tests.EditMode.csproj" --no-restore`, `dotnet build "Asset Manager\AssetManager.Tests.PlayMode.csproj" --no-restore`, and `git diff --check` passed.
- Unity batchmode: not run. The required escalated Unity test command was rejected by the app usage/approval limit, so final EditMode/PlayMode execution remains a residual risk.

## Blocked by

None - can start immediately
