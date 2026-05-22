# 12. 분기, 휴가, 최종 정산 새 기준 반영

Status: done

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

분기 마감, 4Q 휴가, 최종 정산 화면을 새 수익/가치/월세 밀림 기준으로 갱신한다. 분기 마감은 배당금, 주식 매도 수익, 분기 정산 수익을 합친 분기 수익으로 목표 달성률을 계산한다. 4Q 휴가는 현재 가치, 올해 수익, 보유 주식 수, 월세 밀림을 요약한다. 최종 정산은 보유 주식의 최종 가치로 등급을 정하고, 등급과 월세 밀림 단계로 코멘트를 선택한다.

기존 화면 흐름은 재사용하되 운용 가치/운용 수익/환매 압력 표현은 새 용어와 규칙으로 교체한다.

## Existing implementation conflicts

- `QuarterSettlement`는 운용 수익과 환매 압력 기준으로 분기 결과를 만든다.
- `FiscalYearSummary`는 현재 운용 가치, 회계연도 운용 수익, 보유 자산 수를 표시한다.
- `FinalSettlement`는 최종 운용 가치와 환매 압력 단계로 결과를 만든다.
- `RunProgressControls`는 분기 마감, 휴가, 실패, 최종 정산 UI 문구를 기존 용어로 표시한다.

## Refactor approach

- 분기 결과의 계산 구조는 유지하되, 입력 수익 카운터를 새 revenue 정의로 바꾼다.
- 휴가 요약은 보유 주식 가치, 올해 수익, 보유 주식 수, 월세 밀림으로 재구성한다.
- 최종 정산은 보유 주식 최종 가치 기준으로 등급을 선택하고, 월세 밀림 단계와 등급 조합으로 코멘트를 고른다.
- 기존 화면 라우팅은 유지하되, 파산 상태에서는 휴가/최종 정산으로 진입하지 않는 것을 회귀 테스트로 고정한다.

## Acceptance criteria

- [x] 분기 마감은 현재 분기 수익과 분기 목표를 비교해 달성률을 표시한다.
- [x] 분기 수익에는 배당금, 주식 매도 수익, 분기 정산 수익이 포함된다.
- [x] 분기 마감에서 월세 밀림 증가량과 현재 월세 밀림을 표시한다.
- [x] 1, 2회계연도 4Q 휴가는 보상/패널티 없이 현재 가치, 올해 수익, 보유 주식 수, 월세 밀림을 표시한다.
- [x] 3회계연도 4Q 마감 후 파산이 아니면 최종 정산으로 이동한다.
- [x] 최종 가치는 보유 주식 가치의 합계로 계산한다.
- [x] 최종 평가는 최종 가치 기준으로 결정한다.
- [x] 최종 코멘트는 최종 평가 등급과 월세 밀림 단계 조합으로 선택한다.

## Implementation notes

- Added stock-overhaul aliases to quarter, vacation, and final-settlement result objects while preserving old property names for compatibility.
- Updated `RunProgressControls` summaries to show current quarter revenue, current value, owned stock count, rent arrears, final value, and final comment terminology.
- Added EditMode regressions for failed quarter settlement not entering 4Q vacation or final settlement.
- Follow-up architecture cleanup: `RentArrears` is now the canonical public rule surface for 월세 밀림 and 파산, `Revenue` is now the canonical public accounting surface, and `Value`/`CurrentValue`/`FinalValue` are now the canonical public value surface. `RedemptionPressure`, `EarnedCash`, and `ManagementValue` names remain as compatibility aliases/deferred serialized naming.
- Verified with `scripts/Run-UnityBatchmode.ps1 -Mode EditMode -AssemblyNames AssetManager.Tests.EditMode` and `scripts/Run-UnityBatchmode.ps1 -Mode PlayMode -AssemblyNames AssetManager.Tests.PlayMode`.

## Blocked by

- `08-stock-sale-and-revenue-tracking.md`
- `11-rent-arrears-and-bankruptcy.md`
