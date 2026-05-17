# 11. 월세 밀림과 파산 흐름 전환

Status: ready-for-agent

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

환매 압력과 런 실패 용어/흐름을 월세 밀림과 파산으로 전환한다. 월세 밀림은 예약 시 +1 증가하고, 분기 목표 수익 미달 시 달성률에 따라 +1/+2/+3 증가한다. 월세 밀림이 10에 도달하면 즉시 파산으로 게임 오버된다.

이 이슈는 기존 분기 마감 구조와 예약 흐름을 재사용하되, 사용자-facing 언어와 실패 판정 의미를 새 PRD에 맞춘다.

## Existing implementation conflicts

- `RedemptionPressure`, `RedemptionPressureState`, `RedemptionPressureConfigData`가 실패 압력 도메인을 대표한다.
- `ReservationAction`과 `QuarterSettlement`는 환매 압력 증가를 호출한다.
- `RunState.Failed`, failure reason, `RunStatusFormatter`, `RunProgressControls`는 기존 실패 문구와 환매 압력 표시를 사용한다.
- 최종 코멘트 데이터도 환매 압력 단계에 묶여 있다.

## Refactor approach

- 내부 구현은 한 번에 대규모 rename하지 않아도 되지만, 새 public 도메인과 사용자-facing 텍스트는 월세 밀림/파산으로 통일한다.
- 기존 threshold 10과 증가량 계산 구조는 재사용하되, 입력 이벤트를 예약과 분기 실패로 명확히 제한한다.
- `RedemptionPressure` 계열은 점진적으로 `RentArrears` 의미로 감싸거나 이름을 바꿀 수 있게 이슈 안에서 리팩터링 단계를 명시한다.
- 파산 발생 시 기존 failure routing을 재사용하되, 휴가/최종 정산으로 진행하지 않는 조건을 다시 검증한다.

## Acceptance criteria

- [ ] 사용자-facing 실패 압력 용어는 월세 밀림으로 표시된다.
- [ ] 월세 밀림 한도는 10이다.
- [ ] 주식 예약 성공 시 월세 밀림이 1 증가한다.
- [ ] 분기 수익이 목표 이상이면 월세 밀림 증가가 없다.
- [ ] 분기 수익이 목표 미만이면 달성률에 따라 월세 밀림이 +1/+2/+3 증가한다.
- [ ] 월세 밀림이 10 이상이 되는 즉시 파산으로 게임 오버된다.
- [ ] 파산이 발생하면 다음 영업일, 다음 분기, 휴가, 최종 정산으로 진행하지 않는다.
- [ ] 기존 환매 압력/대규모 환매/런 실패 텍스트는 새 흐름에서 노출되지 않는다.

## Blocked by

- `05-market-slot-reservation.md`
- `08-stock-sale-and-revenue-tracking.md`
