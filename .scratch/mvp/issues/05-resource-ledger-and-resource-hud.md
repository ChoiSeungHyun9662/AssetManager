# 05. 자원 원장과 보유 자원 UI

Status: ready-for-agent

## Parent

.scratch/mvp/PRD.md

## What to build

현금, 리서치, 신용, 원자재, 딜을 관리하는 ResourceLedger를 만들고 화면에 보유 자원을 표시한다. 이 slice는 이후 매수, 자원 확보, 예약이 같은 자원 원장을 사용하도록 하는 기반이다. 특히 운용 수익과 조달 현금은 기록 방식이 달라야 한다.

## Code work

- ResourceLedger module을 구현한다.
- 현금 증가는 조달 현금과 운용 수익을 구분하는 인터페이스로 제공한다.
- 운용 수익은 CurrentCash, CurrentQuarterEarnedCash, CurrentFiscalYearEarnedCash, TotalEarnedCash에 반영한다.
- 조달 현금은 CurrentCash에만 반영한다.
- 전문 자원 합계는 리서치 + 신용 + 원자재 <= 10으로 제한한다.
- 전문 자원 초과 시 기존 보유 자원은 건드리지 않고 신규 획득분 중 초과분만 폐기한다.
- 딜은 최대 3으로 제한하고 초과분 폐기 결과를 반환하거나 메시지로 노출한다.
- 현금과 딜은 전문 자원 한도에 포함하지 않는다.

## Unity Editor work

- 보유 자원 영역을 만든다.
- 현금은 숫자로 표시한다.
- 리서치, 신용, 원자재, 딜은 숫자 또는 임시 칩 UI로 표시한다.
- 전문 자원 합계와 딜 최대치를 확인할 수 있는 개발용 표시 또는 툴팁을 추가한다.
- 짧은 메시지 영역을 만들고 자원 한도 관련 메시지를 표시할 수 있게 연결한다.

## Verification

- Play Mode에서 개발용 입력 또는 테스트 버튼으로 조달 현금을 얻으면 현금만 증가하고 운용 수익은 증가하지 않는다.
- 운용 수익을 얻으면 현금과 분기, 회계년도, 총 운용 수익이 함께 증가한다.
- 전문 자원 합계가 10을 넘지 않는다.
- 딜이 3을 넘지 않고 초과 시 메시지를 표시한다.

## Acceptance criteria

- [ ] 현금, 리서치, 신용, 원자재, 딜이 런타임 상태로 관리된다.
- [ ] 조달 현금은 운용 수익에 포함하지 않는 경로로 추가된다.
- [ ] 보유 자산의 영업일 시작 현금과 분기 마감 정산 수익을 위한 운용 수익 추가 경로가 있다.
- [ ] 전문 자원 합계 한도 10이 적용된다.
- [ ] 딜 한도 3이 적용된다.
- [ ] 전문 자원 한도 초과는 신규 획득분에서만 폐기된다.
- [ ] 자원 상태가 Unity 화면에 표시된다.

## Blocked by

- .scratch/mvp/issues/01-unity-mvp-run-shell-and-data-bootstrap.md

## User stories covered

40, 42, 43, 44, 45, 56, 57, 58

## Comments
