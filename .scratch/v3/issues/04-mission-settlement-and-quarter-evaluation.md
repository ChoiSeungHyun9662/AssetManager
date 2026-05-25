# 04. 미션 수익과 분기 평가 통합

Status: done

Type: AFK

## Parent

- `.scratch/v3/prd_v3.md`

## User stories covered

- 13, 14, 15, 16, 17, 18, 19, 24, 51

## What to build

확정된 미션을 매 분기말 현재 포트폴리오 기준으로 정산하고, 그 결과를 현금과 분리된 `미션 수익`으로 기록한다. 미션 수익은 구매 자금이 아니지만, 분기 평가 목표와 월세 밀림 판정에는 항상 포함된다.

이 슬라이스는 분기말 결과 표시까지 포함한다. 플레이어는 `현금 흐름`, `미션 수익`, 그리고 이 둘의 합으로 월세 밀림 판정이 이루어진다는 사실을 확인할 수 있어야 한다.

## Acceptance criteria

- [x] 확정 미션은 매 분기말 현재 포트폴리오를 기준으로 정산된다.
- [x] 미션이 확정된 같은 분기의 분기말에도 미션 수익이 정산된다.
- [x] 미션 정산 공식은 대상 태그 카드 수를 사용할 수 있다.
- [x] 미션 정산 공식은 대상 태그 호일 수를 사용할 수 있다.
- [x] 미션 정산 공식은 대상 태그 총 가치를 사용할 수 있다.
- [x] 미션 정산 공식은 모든 보유 카드를 대상으로 사용할 수 있다.
- [x] 정산 공식이 현재 포트폴리오에서 수익을 만들지 못하면 해당 분기의 미션 수익은 0이다.
- [x] 미션 수익은 현금 잔액에 더해지지 않는다.
- [x] 미션 수익은 내부적으로 현금 흐름과 분리되어 저장된다.
- [x] 분기말 UI는 `미션 수익 +N`을 별도 라인으로 보여준다.
- [x] 월세 밀림 판정은 `현금 흐름 + 미션 수익`을 사용한다.
- [x] 현금 흐름은 배당과 매도 현금을 포함한다.
- [x] 미션 수익은 최종/평가 가치 표시에는 섞어서 반영된다.
- [x] Mr.Market 영구 가치 변화가 존재하는 경우 미션 정산에 반영될 수 있는 계산 경로가 준비된다.
- [x] 관련 EditMode 테스트가 정산 공식, 현금 미증가, 월세 밀림 판정 통합을 검증한다.
- [x] PlayMode 또는 UI-facing 테스트가 분기말 미션 수익 표시를 검증한다.

## Blocked by

- `03-mission-clear-and-confirmation-flow.md`

## Implementation notes

- Added `MissionSettlement` as the public pure rule service for confirmed mission quarter-end revenue.
- `MissionSettlement` evaluates fast-entry/concentration/two-tag formulas from target-tag owned card count, foil formulas from target-tag owned card count plus target-tag foil count, and high-value formulas from target-tag effective value totals.
- Empty target-tag formulas intentionally target all owned cards, giving the mission data path a way to score all 보유 주식 without new code.
- `QuarterSettlement` now treats 현금 흐름 as the existing quarter revenue counters only, adds the quarter's mission revenue to separate mission counters, and uses `현금 흐름 + 미션 수익` for 목표 달성률 and 월세 밀림.
- Mission revenue does not call `ResourceLedger.AddRevenue`, so it does not increase 현금 or 총 수익.
- `FinalSettlement` and `FiscalYearSummary` include accumulated mission revenue in displayed/evaluated value, while still showing cash revenue separately.
- `RunProgressControls` displays quarter-end `현금 흐름`, `미션 수익 +N`, and the combined `판정 합계`.

## Test evidence

- RED: Added `MissionSettlementTests`, quarter integration assertions, final settlement mission-value assertion, and quarter-end UI mission line assertions. Initial EditMode compile failed on missing `MissionSettlement`, mission revenue state, quarter result fields, and final settlement mission total.
- GREEN: `MissionSettlementTests` cover target-tag card count, target-tag foil count, target-tag total value, all-owned-card targeting, and zero revenue.
- GREEN: `QuarterSettlementTests` cover mission revenue not increasing cash, separate mission revenue storage, and rent-arrears integration from cash flow plus mission revenue.
- GREEN: `FinalSettlementTests.CreateFinalSettlementIncludesTotalMissionRevenueInFinalValue`.
- GREEN: `MainGameShellBootstrapShowsQuarterSettlementMissionRevenueLine`.
- `powershell -ExecutionPolicy Bypass -File scripts/Run-UnityBatchmode.ps1 -Mode EditMode`: passed, 142/142.
- `powershell -ExecutionPolicy Bypass -File scripts/Run-UnityBatchmode.ps1 -Mode PlayMode`: passed, 63/63.

## Remaining risk

- Mr.Market permanent value traces are not implemented yet; this slice prepares the path by using `AssetCardRuntimeData.Value` for mission value settlement, so future effective-value deltas can feed settlement through that existing runtime value surface.
