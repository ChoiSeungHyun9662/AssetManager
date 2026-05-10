# 02. 영업일·분기·회계년도 진행 루프

Status: done

## Parent

.scratch/mvp/PRD.md

## What to build

새 런이 실제 시간 구조를 따라 진행되게 한다. 플레이어는 다음 영업일 버튼을 눌러 영업일을 소비하고, 마지막 영업일이 끝나면 분기 마감 placeholder로 이동하며, 1·2회계년도 3Q 이후에는 4Q 휴가 placeholder, 3회계년도 4Q 이후에는 최종 정산 placeholder로 이동해야 한다.

## Code work

- RunCalendar module을 구현해 회계년도, 분기, 플레이 가능 여부, 휴가 분기 여부, 분기당 영업일 수를 판정한다.
- BusinessDayFlow module을 구현해 영업일 시작, 영업일 종료, 남은 영업일 감소, 다음 일정 라우팅을 처리한다.
- 다음 영업일 행동을 첫 번째 end-to-end 액션으로 연결한다.
- 1·2회계년도는 1Q~3Q만 플레이하고 4Q는 휴가로 라우팅한다.
- 3회계년도는 1Q~4Q를 플레이한다.
- 전체 플레이 분기 10개, 전체 플레이 영업일 44일 구조를 고정한다.
- 분기 마감, 휴가, 최종 정산은 이후 slice에서 실제 화면으로 교체할 수 있도록 명확한 상태 또는 이벤트로 노출한다.

## Unity Editor work

- 다음 영업일 버튼을 상단 또는 주요 입력 영역에 배치한다.
- 회계년도, 분기, 남은 영업일 텍스트가 버튼 클릭 후 즉시 갱신되게 연결한다.
- 분기 마감 placeholder, 4Q 휴가 placeholder, 최종 정산 placeholder를 임시 Panel로 배치한다.
- 각 placeholder에는 현재 도달 지점을 확인할 수 있는 최소 텍스트를 표시한다.

## Verification

- Play Mode에서 다음 영업일을 4번 누르면 1회계년도 1Q의 남은 영업일이 0이 되고 분기 마감 placeholder로 이동한다.
- 1회계년도 3Q 마감 이후 4Q 휴가 placeholder로 이동한다.
- 휴가 계속 입력 이후 2회계년도 1Q가 시작된다.
- 3회계년도 4Q 마감 이후 최종 정산 placeholder로 이동한다.

## Acceptance criteria

- [x] 1회계년도와 2회계년도 플레이 분기는 각 4영업일이다.
- [x] 3회계년도 플레이 분기는 각 5영업일이다.
- [x] 1·2회계년도 4Q는 플레이하지 않고 휴가로 간다.
- [x] 3회계년도 4Q는 플레이한다.
- [x] 다음 영업일 버튼은 영업일을 정확히 1만 소비한다.
- [x] 마지막 영업일 종료 후 다음 영업일 운용 수익 흐름으로 가지 않고 분기 마감 상태로 간다.
- [x] 전체 구조가 10개 플레이 분기, 44영업일로 검증된다.

## Blocked by

- .scratch/mvp/issues/01-unity-mvp-run-shell-and-data-bootstrap.md

## User stories covered

2, 3, 4, 5, 12, 53, 54

## Comments

2026-05-10 TDD progress:

- RED/GREEN: added `BusinessDayFlowTests.AdvanceToNextBusinessDayConsumesExactlyOneBusinessDay`; implemented `BusinessDayFlow.AdvanceToNextBusinessDay`.
- RED/GREEN: added last 영업일 coverage; `AdvanceToNextBusinessDay` now enters `BusinessDayPhase.QuarterSettlement` at 0 remaining 영업일.
- RED/GREEN: added `RunCalendarTests` for 10 playable quarters, 44 total 영업일, 1·2회계년도 4Q vacation, and 3회계년도 4Q playable rules; implemented `RunCalendar`.
- RED/GREEN: added quarter settlement routing to next playable 분기, 4Q 휴가, 휴가 계속, and 3회계년도 4Q final settlement.
- RED/GREEN: added a guard so `AdvanceToNextBusinessDay` does not consume below 0 after 분기 마감.
- RED/GREEN: added PlayMode coverage for the 다음 영업일 button updating `RunSessionState` and top HUD, and for 분기 마감 placeholder display.
- GREEN: added runtime-created 다음 영업일 button, 계속 button, 분기 마감/4Q 휴가/최종 정산 placeholder panels, and connected them through `MainGameShellBootstrap`.
- Verification: escalated Unity EditMode batchmode passed, 13/13 tests.
- Verification: escalated Unity PlayMode batchmode passed, 4/4 tests.
- Verification: `rg "AUM|aum|턴|스테이지|Turn|Stage" Asset Manager/Assets/_AssetManager` returned no matches.

Manual Unity checklist:

- Open `Assets/_AssetManager/Scenes/MainGame.unity`.
- Enter Play Mode and confirm the top status area starts at `1회계년도 1Q`, `남은 4영업일`.
- Click `다음 영업일` 4 times and confirm the 분기 마감 placeholder shows `1회계년도 1Q`.
- Click `계속` through 1회계년도 3Q 마감 and confirm the 4Q 휴가 placeholder appears.
- Click `계속` from 4Q 휴가 and confirm `2회계년도 1Q`, `남은 4영업일`.
- Continue through 3회계년도 4Q and confirm the 최종 정산 placeholder appears.
