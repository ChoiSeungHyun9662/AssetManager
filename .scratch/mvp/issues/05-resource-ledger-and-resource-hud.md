# 05. 자원 원장과 보유 자원 UI

Status: implemented-needs-editor-check

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

- [x] 현금, 리서치, 신용, 원자재, 딜이 런타임 상태로 관리된다.
- [x] 조달 현금은 운용 수익에 포함하지 않는 경로로 추가된다.
- [x] 보유 자산의 영업일 시작 현금과 분기 마감 정산 수익을 위한 운용 수익 추가 경로가 있다.
- [x] 전문 자원 합계 한도 10이 적용된다.
- [x] 딜 한도 3이 적용된다.
- [x] 전문 자원 한도 초과는 신규 획득분에서만 폐기된다.
- [x] 자원 상태가 Unity 화면에 표시된다.

## Blocked by

- .scratch/mvp/issues/01-unity-mvp-run-shell-and-data-bootstrap.md

## User stories covered

40, 42, 43, 44, 45, 56, 57, 58

## Comments

2026-05-11 TDD progress:

- RED: added `ResourceLedgerTests.FundingCashOnlyIncreasesCashAndEarnedCashIncreasesPerformanceCounters`; Unity EditMode batchmode reached compilation and failed because `ResourceLedger` did not exist yet.
- GREEN: added `ResourceLedger.AddFundingCash` and `ResourceLedger.AddEarnedCash`; `RunPerformanceState` now exposes `CurrentQuarterEarnedCash`, `CurrentFiscalYearEarnedCash`, and `TotalEarnedCash`.
- RED/GREEN: added professional resource cap coverage; `AddProfessionalResource` caps 리서치 + 신용 + 원자재 at 10, preserves existing holdings, discards only newly gained overflow, and returns a short message.
- RED/GREEN: added 딜 cap coverage; `AddDeal` caps 딜 at 3, excludes 딜 from 전문 자원 한도, and returns `딜 최대 보유: 추가 딜 폐기` when needed.
- RED/GREEN UI: added PlayMode coverage for development resource buttons and 보유 자원 HUD; added runtime-created `ResourceHud` and `ResourceDevControls` with buttons for 조달 현금, 운용 수익, 리서치, 신용, 원자재, and 딜.
- Verification: escalated Unity EditMode batchmode was started once with `Start-Process` and quoted single-string Unity arguments. It timed out after 10 minutes; no automatic Unity retry was attempted.
- Verification: Unity log captured the intended RED compile failure before implementation: `ResourceLedger` did not exist.
- Verification: manual static compile using Unity's Roslyn response files passed for `AssetManager.Runtime`, `AssetManager.Tests.EditMode`, and `AssetManager.Tests.PlayMode` after adding the new runtime files to the compile check and replacing the runtime reference with the static-check build.
- Batchmode result files: no XML test result was produced because the Unity run timed out before normal completion.

Manual Unity checklist:

- Open `Assets/_AssetManager/Scenes/MainGame.unity`.
- Enter Play Mode and confirm the 보유 자원 HUD shows `현금`, `리서치`, `신용`, `원자재`, `전문 자원 0/10`, and `딜 0/3`.
- Click `조달 현금 +1` and confirm 현금 increases while 분기/회계년도/총 운용 수익 counters are unchanged in the runtime inspector or debugger.
- Click `운용 수익 +1` and confirm 현금, 현재 분기 운용 수익, 현재 회계년도 운용 수익, and 총 운용 수익 all increase by 1.
- Click `리서치 +1` 11 times and confirm 리서치 stops at 10, 전문 자원 stays 10/10, and the message shows `자원칩 최대 보유: 리서치 +1 폐기`.
- Click `딜 +1` 4 times and confirm 딜 stops at 3/3 and the message shows `딜 최대 보유: 추가 딜 폐기`.
- Confirm the resource development buttons are visible only while the run is playing in the Market state.
