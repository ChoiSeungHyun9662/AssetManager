# 00. Unity 프로젝트 생성과 빈 실행 셸

Status: done

## Parent

.scratch/mvp/PRD.md

## What to build

MVP 구현을 시작할 수 있도록 Unity 프로젝트를 생성하고, 이후 모든 issue가 붙을 수 있는 빈 실행 셸을 만든다. 이 slice의 성공 기준은 게임 규칙 구현이 아니라 Unity Editor에서 프로젝트를 열고 Play Mode를 눌렀을 때 Bootstrap Scene에서 Main Game Scene으로 진입하며, 빈 UI 루트와 테스트 준비 상태가 확인되는 것이다.

프로젝트는 현재 기획/이슈 레포 루트 아래의 `Asset Manager` 폴더를 Unity 프로젝트 루트로 사용한다. 레포 루트는 문서와 작업 관리 공간으로 유지하고, Unity 생성물은 `Asset Manager/Assets`, `Asset Manager/Packages`, `Asset Manager/ProjectSettings` 아래로 모은다.

`Asset Manager` 폴더는 사용자가 생성한 실제 Unity 프로젝트로 확정한다. 별도 `Game` 프로젝트는 만들지 않는다.

## Code work

- Unity 프로젝트 루트는 레포 하위의 `Asset Manager` 폴더로 고정한다.
- MVP 코드와 에디터 자산을 담을 루트 네임스페이스와 폴더 구조를 준비한다.
- Domain, Application, Presentation, UI, Tests 영역이 이후 issue에서 자연스럽게 분리될 수 있도록 최소 폴더와 assembly definition 구성을 만든다.
- Bootstrap Scene에서 Main Game Scene으로 넘어가는 최소 실행 흐름을 만든다.
- Main Game Scene에 빈 Game Root와 UI Root를 두고, 이후 HUD, Market, Detail, Modal, Settlement 화면이 붙을 자리를 만든다.
- EditMode 테스트와 PlayMode 테스트가 각각 하나 이상 실행될 수 있는 테스트 골격을 만든다.
- Unity가 생성하는 캐시성 폴더와 사용자별 설정이 git에 들어가지 않도록 ignore 규칙을 준비한다.

## Unity Editor work

- Unity Hub에서 사용자가 생성한 `Asset Manager` 프로젝트를 연다.
- 템플릿은 Universal 3D 또는 URP 3D 계열을 사용한다.
- 현재 생성된 Unity editor version `6000.4.5f1`을 기준으로 진행한다. 버전 변경이 필요하면 이 issue에서 먼저 결정한다.
- Version Control은 Visible Meta Files로 설정한다.
- Asset Serialization은 Force Text로 설정한다.
- TextMeshPro, Input System, Unity Test Framework가 프로젝트에서 사용 가능한지 확인한다.
- Bootstrap Scene과 Main Game Scene을 만들고 Build Profiles 또는 Scenes In Build에 등록한다.
- Canvas Scaler 기준 해상도는 MVP 기본값으로 1920x1080을 사용한다.
- 첫 Play Mode 진입 시 콘솔 오류가 없도록 Scene 참조와 기본 오브젝트 연결을 확인한다.

## Verification

- Unity Hub에서 `Asset Manager` 프로젝트를 열 수 있다.
- `Asset Manager/Assets`, `Asset Manager/Packages`, `Asset Manager/ProjectSettings`가 존재한다.
- Play Mode에서 Bootstrap Scene을 실행하면 Main Game Scene으로 진입한다.
- Main Game Scene에는 빈 Game Root와 UI Root가 존재한다.
- 화면에는 최소한의 빈 MVP 셸 또는 준비 상태 텍스트가 표시된다.
- Console에 error나 missing reference가 없다.
- EditMode 테스트 골격이 실행되어 통과한다.
- PlayMode 테스트 골격이 실행되어 통과한다.
- git status에서 Unity 캐시성 폴더가 추적 대상으로 보이지 않는다.

## Acceptance criteria

- [x] Unity 프로젝트가 레포 하위의 `Asset Manager` 폴더로 확정되어 있다.
- [x] 프로젝트를 Unity Editor에서 열 수 있다.
- [x] Bootstrap Scene과 Main Game Scene이 존재하고 Scene 목록에 등록되어 있다.
- [x] Play Mode에서 Bootstrap Scene이 Main Game Scene으로 진입한다.
- [x] Main Game Scene에는 이후 MVP UI를 붙일 루트 오브젝트가 있다.
- [x] EditMode와 PlayMode 테스트 골격이 각각 실행 가능하다.
- [x] Unity 메타 파일은 보존되고, 캐시성 생성물은 git 추적 대상에서 제외된다.
- [x] 이 slice만 완료되어도 이후 01번 데이터 부트스트랩 issue를 시작할 수 있다.

## Blocked by

None - can start immediately

## User stories covered

56, 57, 58

## Comments

This is a prerequisite setup issue added before the original MVP implementation sequence.

Decision: use the existing `Asset Manager` folder as the Unity project root. It currently contains `Assets`, `Packages`, `ProjectSettings`, and Unity editor version `6000.4.5f1`.

TDD result:

- Added EditMode setup tests for creating/registering Bootstrap and MainGame scenes and verifying MainGame roots.
- Added a PlayMode test for Game Root, UI Root, 1920x1080 Canvas Scaler, and the visible ready status text.
- Added runtime shell code under `Asset Manager/Assets/_AssetManager/Scripts/Runtime`.
- Added editor setup commands at `Asset Manager > Setup > Ensure Project Shell` and `Asset Manager > Setup > Verify Project Shell`.
- Ran `VerifyProjectShell` in Unity batchmode successfully.
- Ran Unity Test Runner in batchmode without `-quit`: EditMode 2/2 passed, PlayMode 1/1 passed.
