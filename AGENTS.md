# AGENTS.md

Behavioral guidelines to reduce common LLM coding mistakes. Merge with project-specific instructions as needed.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

### TDD transparency

When using TDD, make the work reviewable without requiring the user to read implementation details:
- State assumptions, non-goals, and behavior-level success criteria before coding.
- Report each RED/GREEN/REFACTOR cycle in terms of behavior tested, minimal change made, and verification result.
- For Unity work, include the manual scene/flow/interaction checked, or say that manual testing was not run and why.
- Finish with a compact verification receipt: behavior changed, tests run, Unity manual checks, files touched, and remaining risks.

### Unity TDD verification economy

Unity batchmode is comparatively expensive. Keep TDD behavior-first, but avoid spending Unity startup time on checks that a cheaper signal can cover.

- For pure rule modules, group 2-3 closely related behavior tests before running EditMode, as long as each test still describes a distinct public behavior and the implementation stays minimal.
- For compiler RED checks, prefer a fast compile/static signal when available instead of launching a full Unity test run just to prove a missing type or method.
- Keep PlayMode/UI wiring tests for Unity-visible behavior such as buttons, panels, scene shell objects, and gating.
- Always run the relevant final EditMode and PlayMode suites before calling the issue done, or clearly report any skipped verification and residual risk.

---

**These guidelines are working if:** fewer unnecessary changes in diffs, fewer rewrites due to overcomplication, and clarifying questions come before implementation rather than after mistakes.

---

Skills are organized into bucket folders under skills/:

engineering/ — daily code work
productivity/ — daily non-code workflow tools
misc/ — kept around but rarely used
personal/ — tied to my own setup, not promoted
in-progress/ — drafts not yet ready to ship
deprecated/ — no longer used
Every skill in engineering/, productivity/, or misc/ must have a reference in the top-level README.md and an entry in .claude-plugin/plugin.json. Skills in personal/, in-progress/, and deprecated/ must not appear in either.

Each skill entry in the top-level README.md must link the skill name to its SKILL.md.

Each bucket folder has a README.md that lists every skill in the bucket with a one-line description, with the skill name linked to its SKILL.md.

## Agent skills

### Issue tracker

Issues and PRDs are tracked as local markdown files under `.scratch/`. See `docs/agents/issue-tracker.md`.

### Triage labels

This repo uses the default mattpocock/skills triage label vocabulary. See `docs/agents/triage-labels.md`.

### Domain docs

This repo uses a single-context domain docs layout. See `docs/agents/domain.md`.

### Unity batchmode

Use `scripts/Run-UnityBatchmode.ps1` for Unity batchmode runs. In Codex, run it outside the default sandbox / with escalated execution; the default sandbox can terminate Unity before it writes a log with unknown software exception `0x80000003`.

The Unity project path contains a space (`Asset Manager`). If manually wrapping Unity with PowerShell `Start-Process`, pass the Unity arguments as one quoted string, not an `-ArgumentList` array. Use `-runSynchronously` for EditMode only; do not pass it to PlayMode.

### Class inventory

Implemented production classes are tracked in `docs/agents/class-inventory.md`. When adding, removing, renaming, moving, or substantially changing the responsibility of a class, struct, enum, or serialized data shape under `Asset Manager/Assets/_AssetManager/Scripts`, update that document in the same turn.
