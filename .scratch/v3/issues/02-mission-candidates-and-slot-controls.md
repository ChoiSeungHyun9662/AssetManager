# 02. 미션 후보 3장과 슬롯 조작

Status: done

Type: AFK

## Parent

- `.scratch/v3/prd_v3.md`

## User stories covered

- 4, 5, 6, 7, 8, 25, 26, 27, 28

## What to build

런 시작 시 미션 후보 3장을 생성하고, 플레이어가 후보를 비교, 멀리건, 폐기할 수 있는 첫 미션 핸드를 만든다. 이 슬라이스는 아직 미션 확정이나 분기말 정산을 완성하지 않는다. 대신 후보 슬롯 상태, 1회 멀리건, 멀리건 이후 수동 폐기, 빈 슬롯 유지, 미션 표시 정보가 끝까지 검증 가능해야 한다.

초기 미션 풀은 v3 PRD의 25장 구조를 따른다. 각 미션은 투자 철학형 이름, 대상 태그, 클리어 조건 설명, 정산 공식 설명, 표시용 난이도를 가진다. 난이도는 시스템 판정 값이 아니라 표시용 정보로만 취급한다.

## Acceptance criteria

- [x] 새 런을 시작하면 미션 후보 슬롯 3개가 생성된다.
- [x] 미션 후보는 현재 시장 카드와 무관하게 생성된다.
- [x] 후보 3장은 미션 확정 전까지 항상 확인 가능하다.
- [x] 각 슬롯은 확정 전 한 번만 멀리건할 수 있다.
- [x] 멀리건한 슬롯은 새 미션 후보로 교체된다.
- [x] 이미 멀리건한 슬롯은 수동 폐기할 수 있다.
- [x] 수동 폐기된 슬롯은 빈 슬롯으로 남고 다시 보충되지 않는다.
- [x] 미션 후보는 투자 철학형 이름, 대상 태그, 클리어 조건, 정산 공식, 표시용 난이도를 보여준다.
- [x] 초기 미션 풀은 빠른 진입형 5장, 집중형 5장, 호일형 5장, 고가치형 5장, 2태그 안정형 5장으로 구성된다.
- [x] 같은 태그가 한 런의 여러 후보에 등장할 수 있다.
- [x] 난이도는 표시만 바꾸고 클리어 조건이나 보상 수치에 영향을 주지 않는다.
- [x] 관련 EditMode 테스트가 후보 생성, 멀리건 제한, 폐기, 빈 슬롯 유지를 검증한다.
- [x] PlayMode 또는 UI-facing 테스트가 후보 3장 표시와 슬롯 버튼 상태를 검증한다.

## Blocked by

- `01-v3-stock-tags-and-calendar-baseline.md`

## Implementation notes

- Added a 25-card mission pool to `RunStaticDataSet` with 5 missions each for fast-entry, concentration, foil, high-value, and two-tag stable templates.
- Added `MissionRunState` with exactly three candidate slots at run start.
- Added `MissionCandidateAction` for one mulligan per slot and post-mulligan manual discard to a persistent empty slot.
- Added `MissionCandidateView` and shell wiring so the three candidates, display metadata, mulligan buttons, and discard buttons are visible in PlayMode.
- Mission confirmation, clear-condition evaluation, and quarter-end mission settlement remain out of scope for later v3 issues.

## Test evidence

- RED: `powershell -ExecutionPolicy Bypass -File scripts\Run-UnityBatchmode.ps1 -Mode EditMode` failed on missing `MissionDefinitionData` / `MissionTemplate` symbols after the first behavior tests were added.
- GREEN: `powershell -ExecutionPolicy Bypass -File scripts\Run-UnityBatchmode.ps1 -Mode EditMode` passed 129/129 on 2026-05-25, results `editmode-20260525-221350-results.xml`.
- GREEN: `powershell -ExecutionPolicy Bypass -File scripts\Run-UnityBatchmode.ps1 -Mode PlayMode` passed 61/61 on 2026-05-25, results `playmode-20260525-221418-results.xml`.

## Remaining risk

- No manual Unity Editor scene inspection was run in this turn.
- The UI uses generated shell layout only; final art/layout polish is not part of this issue.
