---
name: assetmanager-feedback-to-issues
description: Convert Asset Manager playtest or design feedback into implementation-ready PRD deltas, design decisions, issue updates, and TDD-ready issue slices. Use when the user points to feedback files, says feedback should become issues, asks to concretize feedback, or wants follow-up implementation tickets under `.scratch/`.
---

# Asset Manager Feedback To Issues

Turn raw feedback into small, non-overlapping implementation work. Preserve feedback as evidence, but write issues from confirmed behavior decisions.

## Inputs

Read the smallest relevant set:

- The feedback file or user-provided feedback text.
- The current feature PRD under `.scratch/<feature>/`.
- Existing design-decision files, if present.
- Existing issue files that the feedback references or supersedes.
- `CONTEXT.md` for project terms.

If feedback references a completed or in-progress issue, update that issue or create a narrowly named follow-up. Do not duplicate an existing issue with a different title.

## Decision Pass

Before writing issues, separate:

- Raw observation: what the user saw or felt.
- Desired behavior: what should happen instead.
- Confirmed decision: what the user has explicitly accepted.
- Open question: what cannot be inferred without risk.
- Conflict: what old PRD, issue, test, or implementation expectation is now stale.

Ask only for decisions that change implementation direction. Do not ask about already-fixed game rules unless there is a concrete functional conflict.

## Issue Writing

Each issue should be independently grabbable and TDD-ready:

- Include scope and non-goals.
- Include existing implementation conflicts and a refactor approach.
- Define behavior-level acceptance criteria.
- Name the likely EditMode and PlayMode coverage.
- Mention doc updates required after implementation, especially issue log and class inventory.
- Keep UI/Unity Editor work explicit when code alone cannot prove the result.

Prefer small follow-up issues over one broad "apply all feedback" issue unless the feedback is purely documentation.

## Document Updates

Use these targets as appropriate:

- `.scratch/<feature>/feedback/*.md`: keep raw feedback intact.
- `.scratch/<feature>/feedback/*design-decisions.md`: record the implementation-facing interpretation.
- `.scratch/<feature>/PRD.md`: add the accepted delta, not a transcript.
- `.scratch/<feature>/mvp-issue-mapping.md`: update when v1/v2/v3 scope changes or an old issue is superseded.
- `.scratch/<feature>/issues/*.md`: create or update the actionable work.

When the user says the new scope belongs to a new version, create a new `.scratch/<version>/` directory and keep older PRDs as prior art.

## Output

Finish with:

- What feedback became which issue(s).
- Which old behavior or issue is superseded.
- What remains unresolved.
- A short TDD prompt the user can reuse if implementation is not started in the same turn.
