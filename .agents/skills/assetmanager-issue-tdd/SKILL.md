---
name: assetmanager-issue-tdd
description: Asset Manager issue implementation workflow that wraps TDD with repo-specific Unity verification, issue-document updates, class-inventory updates, and stale-test conflict handling. Use when working on an Asset Manager `.scratch/**/issues/*.md` implementation issue, especially when the user invokes TDD, asks to implement an issue, or references EditMode/PlayMode verification.
---

# Asset Manager Issue TDD

Use this as the project-specific wrapper around the generic `tdd` skill. Keep the actual change narrow: one issue, one behavior slice at a time.

## Start

1. Read `AGENTS.md`, `CONTEXT.md`, `docs/agents/project-memory.md`, `docs/agents/class-inventory.md`, the target issue, and the related PRD or design-decision file.
2. Identify blockers, dependencies, `Existing implementation conflicts`, and `Refactor approach` from the issue before editing.
3. State assumptions, non-goals, behavior-level success criteria, and the cheapest relevant verification path.
4. If the issue conflicts with older MVP expectations, prefer the newest accepted PRD/design decision. Do not preserve stale tests blindly.

## RED/GREEN Loop

- Write tests against public behavior, not private helper structure.
- For pure rule modules, group two or three tightly related behavior tests before paying the Unity startup cost.
- Use EditMode first for rules, state transitions, cost calculation, calendar, market tape, portfolio, mission, offer, and settlement behavior.
- Use PlayMode for Unity-visible behavior: buttons, scene shell objects, panels, drag/drop, hover, modal gating, layout wiring, and screen transitions.
- Avoid global rename or broad replacement as the first move. Change one visible behavior, make it pass, then continue.
- When legacy names remain for compatibility, describe the domain term first and mention the implementation alias only if needed.

## Unity Verification

Use the canonical wrapper from `scripts/Run-UnityBatchmode.ps1`.

Run Unity outside the default sandbox / with escalated execution. The project has a recurring local failure where default sandbox batchmode can terminate before writing a useful log with unknown software exception `0x80000003`.

Commands:

```powershell
scripts/Run-UnityBatchmode.ps1 -Mode EditMode
scripts/Run-UnityBatchmode.ps1 -Mode PlayMode
scripts/Run-UnityBatchmode.ps1 -Mode QuitOnly
```

Rules:

- EditMode uses `-runSynchronously`; PlayMode must not.
- The Unity project path contains a space: `Asset Manager`. If wrapping Unity manually with `Start-Process`, pass Unity arguments as one quoted string, not an `-ArgumentList` array.
- Read the latest log and result XML under `.scratch/test-results/` after a run.
- If Unity verification is blocked, run the cheaper static/code checks available, stop retrying the same failing Unity command, and report the skipped verification plus residual risk.
- Before calling an issue done, run the relevant final EditMode and PlayMode suites or explicitly report why a suite was skipped.

## Documentation

Update only documents touched by the behavior:

- Target issue: status, progress log, test evidence, and remaining risk.
- `docs/agents/class-inventory.md`: whenever a production class, struct, enum, serialized state shape, or major responsibility under `Asset Manager/Assets/_AssetManager/Scripts` changes.
- `docs/agents/project-memory.md`: only for durable recurring tooling/workflow facts, not one-off logs.
- PRD/plan docs: only when the implementation exposes a spec conflict or the user asked for design sync.

## Finish

Report a compact verification receipt:

- Behavior changed.
- RED/GREEN cycles completed.
- Tests run, including exact EditMode/PlayMode result.
- Unity manual checks run or not run.
- Files touched.
- Issue/class-inventory/project-memory updates.
- Remaining risks.
