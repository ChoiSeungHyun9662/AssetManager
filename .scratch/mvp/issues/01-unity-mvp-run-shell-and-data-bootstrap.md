# 01. Unity MVP 런 셸과 데이터 부트스트랩

Status: done

## Parent

.scratch/mvp/PRD.md

## What to build

Unity 에디터에서 MVP 런을 시작할 수 있는 최소 Scene과 데이터 부트스트랩을 만든다. 이 slice의 목표는 아직 게임 규칙을 모두 구현하는 것이 아니라, 이후 slice들이 붙을 수 있는 실제 Unity 실행 표면을 만드는 것이다. Play Mode에서 새 런을 시작하면 테스트용 카드, 분기 데이터, 평가 데이터, 자원 설정이 로드되고 상단 상태 영역에 1회계년도 1Q의 초기 상태가 표시되어야 한다.

## Code work

- 정적 데이터와 런타임 상태를 분리한 기본 모델을 만든다.
- ResourceType, AssetRarity, TagType, PurchaseSource, AssetCardRuntimeState, MarketAreaState, MarketTapeZone, RunState, BusinessDayPhase에 해당하는 enum을 준비한다.
- AssetCardData, ProfessionalResourceCost, TagData, QuarterData, FinalRatingData, RedemptionPressureLevelData, FinalManagementCommentData, MarketConfigData, ResourceConfigData, RedemptionPressureConfigData의 데이터 shape를 준비한다.
- ResourceState, RunCalendarState, RunPerformanceState, AssetCardRuntimeData, MarketTapeState, ReservationState, OwnedAssetState, BusinessDayState, RedemptionPressureState의 런타임 shape를 준비한다.
- 테스트용 정적 데이터 세트를 로드해 새 RunState를 만들 수 있는 bootstrap 흐름을 만든다.
- 운용가치 명칭은 ManagementValue로 통일하고 AUM 명칭은 쓰지 않는다.

## Unity Editor work

- MVP 개발용 Scene을 만든다.
- Scene에 런 부트스트랩 오브젝트를 배치하고 필요한 데이터 참조를 Inspector에서 연결한다.
- 상단 상태 영역 Canvas를 배치해 회계년도, 분기, 남은 영업일, 현금, 전문 자원, 딜, 환매 압력을 표시할 공간을 만든다.
- 테스트용 ScriptableObject 또는 Unity에서 편집 가능한 데이터 자산을 만든다.
- 최소 카드 데이터, 분기 데이터, 최종 평가 데이터, 환매 압력 설정 데이터를 에디터에서 확인 가능하게 둔다.

## Verification

- Play Mode에서 Scene을 실행하면 새 런이 자동 또는 버튼 입력으로 시작된다.
- 화면에 1회계년도 1Q, 남은 4영업일, 환매 압력 0/10이 표시된다.
- 테스트용 카드 데이터와 분기 데이터가 null 없이 로드된다.
- Inspector에서 연결 누락이 있으면 명확히 확인할 수 있다.

## Acceptance criteria

- [x] Play Mode에서 새 런을 시작할 수 있다.
- [x] 새 런은 1회계년도 1Q, 4영업일, RunState.Playing으로 초기화된다.
- [x] 기본 자원과 환매 압력 값이 상단 상태 영역에 표시된다.
- [x] 최소 1개 이상의 테스트용 AssetCardData가 로드된다.
- [x] 최소 QuarterData, FinalRatingData, ResourceConfigData, RedemptionPressureConfigData가 에디터 데이터로 존재한다.
- [x] 코드와 데이터에서 AUM 명칭을 사용하지 않는다.
- [x] 이 slice만 완료되어도 Unity Scene을 열고 실행 상태를 눈으로 확인할 수 있다.

## Blocked by

.scratch/mvp/issues/00-unity-project-creation-and-empty-playable-shell.md

## User stories covered

1, 2, 56, 57, 58

## Comments

2026-05-10 TDD progress:

- RED: added `RunBootstrapperTests.CreateNewRunStartsAtFirstFiscalYearFirstQuarter` for 새 런 초기 상태.
- GREEN: added runtime enums, static data shapes, runtime state shapes, `RunStaticDataSet`, and `RunBootstrapper.CreateNewRun`.
- RED: added PlayMode coverage for `MainGameShellBootstrap` starting a run and showing initial status text.
- GREEN: added run status HUD formatting and connected `MainGameShellBootstrap` to create/show the initial run.
- RED: added EditMode coverage for `EnsureProjectShell` creating MVP run data and connecting it to Main Game Shell.
- GREEN: updated editor setup to create `Assets/_AssetManager/Data/MvpRunStaticData.asset` and connect it to `MainGameShellBootstrap`.
- Verification: Unity batchmode/test runner could not complete because Unity Editor crashed before running tests with unknown software exception `0x80000003`.
- Verification fallback: Unity-bundled C# compiler checks passed for Runtime, Editor, EditMode tests, and PlayMode tests.
- Verification fallback: `rg "AUM|aum" Asset Manager/Assets/_AssetManager` returned no matches.

Manual Unity checklist after the batchmode crash is resolved:

- Open the `Asset Manager` Unity project.
- Run `Asset Manager > Setup > Ensure Project Shell`.
- Run `Asset Manager > Setup > Verify Project Shell`.
- Run Unity Test Runner EditMode and PlayMode tests.
- Open `Assets/_AssetManager/Scenes/MainGame.unity`, enter Play Mode, and confirm the top status area shows `1회계년도 1Q`, `남은 4영업일`, `현금 3`, `딜 0/3`, and `환매 압력 0/10`.

2026-05-10 manual Unity verification:

- User completed the manual Unity checklist.
- Marked all acceptance criteria complete and closed the issue as `done`.

2026-05-10 Unity batchmode diagnosis:

- Root cause: `0x80000003` reproduced when Unity was launched from the default Codex sandbox context, even with only `-batchmode -quit` and no project-specific test code.
- Fix/workaround: run Unity batchmode commands outside the sandbox/escalated execution context.
- EditMode batchmode verification passed outside the sandbox: 4/4 tests passed.
- PlayMode batchmode verification passed outside the sandbox: 2/2 tests passed.
- Note: do not pass `-runSynchronously` to PlayMode test runs; it hung in the PlayMode prebuild path. Use `-runSynchronously` for EditMode only.
