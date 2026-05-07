# 11. 분기 마감과 환매 압력 실패

Status: ready-for-agent

## Parent

.scratch/mvp/PRD.md

## What to build

분기 마지막 영업일 종료 후 분기 마감 화면으로 들어가고, 분기 마감 정산 수익을 운용 수익에 반영한 뒤 목표 달성률과 환매 압력 증가량을 계산한다. 환매 압력이 10 이상이면 즉시 실패 화면으로 이동하고, 다음 분기나 휴가나 최종 정산으로 진행하지 않아야 한다.

## Code work

- QuarterSettlement module을 구현한다.
- QuarterEndResult를 생성한다.
- 분기 마감 정산 수익은 목표 달성률 계산 전에 운용 수익으로 반영한다.
- 분기 운용 수익은 자산 인컴과 분기 마감 정산 수익을 포함하고 유동성 확보 현금은 제외한다.
- 목표 달성률은 분기 운용 수익 / 분기 목표로 계산한다.
- 달성률 100% 이상은 환매 압력 +0, 75% 이상 100% 미만은 +1, 50% 이상 75% 미만은 +2, 50% 미만은 +3으로 처리한다.
- RedemptionPressure module의 압력 추가 함수는 증가 직후 실패 여부를 반환하거나 RunState.Failed를 설정한다.
- 환매 압력 실패 사유는 "대규모 환매 발생"으로 표시한다.
- 실패 시 이후 영업일, 다음 분기, 4Q 휴가, 최종 정산 진행을 모두 중단한다.

## Unity Editor work

- 분기 마감 Panel을 만든다.
- 분기 운용 수익, 분기 목표, 목표 달성률, 환매 압력 증가량, 현재 환매 압력을 표시한다.
- 다음 진행 버튼을 분기 마감 Panel에 배치한다.
- 실패 화면 Panel을 만들고 실패 사유, 도달 회계년도/분기, 현재 운용가치, 총 운용 수익, 보유 자산 수, 환매 압력을 표시한다.
- 분기 마감에서 실패하면 분기 마감 UI 대신 실패 화면으로 전환되게 연결한다.

## Verification

- Play Mode에서 마지막 영업일을 종료하면 분기 마감 Panel이 열린다.
- 유동성 확보로 얻은 현금만 있는 경우 분기 운용 수익에 포함되지 않는다.
- 자산 인컴과 분기 정산 수익은 분기 운용 수익에 포함된다.
- 분기 목표 미달 달성률별로 환매 압력 증가량이 올바르게 계산된다.
- 환매 압력이 10 이상이 되면 실패 화면으로 이동하고 다음 일정으로 진행하지 않는다.

## Acceptance criteria

- [ ] 분기 마감은 마지막 영업일 종료 후 발생한다.
- [ ] 마지막 영업일 종료 후 다음 영업일 인컴은 발생하지 않는다.
- [ ] 정산 수익은 목표 달성률 계산 전에 운용 수익으로 반영된다.
- [ ] 유동성 확보 현금은 분기 운용 수익에 포함하지 않는다.
- [ ] 목표 달성률별 환매 압력 증가량이 적용된다.
- [ ] 환매 압력 증가 후 즉시 한도 검사를 한다.
- [ ] 환매 압력 10 이상이면 RunState.Failed가 되고 실패 화면으로 이동한다.
- [ ] 실패 시 다음 영업일, 다음 분기, 휴가, 최종 정산으로 가지 않는다.

## Blocked by

- .scratch/mvp/issues/02-calendar-business-day-and-schedule-loop.md
- .scratch/mvp/issues/07-owned-assets-income-and-performance-tracking.md
- .scratch/mvp/issues/09-market-card-reservation-deal-and-redemption-pressure.md

## User stories covered

49, 50, 51, 52

## Comments

