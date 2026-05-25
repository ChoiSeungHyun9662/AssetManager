---
name: assetmanager-doc-sync
description: Synchronize Asset Manager project documents after design or implementation changes across `plan/`, `.scratch/`, `CONTEXT.md`, `docs/agents/class-inventory.md`, and `docs/agents/project-memory.md`. Use when the user asks to update docs, sync PRD/plan/context, reflect an implementation in documentation, or clean up terminology drift.
---

# Asset Manager Doc Sync

Update the minimum set of documents needed to make the current project state understandable to the next agent.

## Authority Order

When documents conflict, prefer:

1. The user's latest explicit decision.
2. The newest accepted design-decision file.
3. The newest versioned PRD, such as `.scratch/v3/`.
4. Current implemented behavior when the task is post-implementation sync.
5. Older `plan/` or v1/v2 PRDs as prior art only.

Do not silently merge incompatible rules. Name the conflict and update the stale document or leave a clear note.

## Read First

- `CONTEXT.md`
- `docs/agents/domain.md`
- `docs/agents/project-memory.md`
- `docs/agents/class-inventory.md`
- The relevant PRD, issue, feedback, and plan files

Use `rg` to find old terms and stale behavior. Avoid broad rewrites that are not directly tied to the requested sync.

## What To Update

- `CONTEXT.md`: domain terms, relationships, resolved ambiguities, and stable vocabulary.
- `docs/agents/class-inventory.md`: implemented production classes, state shapes, enums, responsibilities, and verification map.
- `docs/agents/project-memory.md`: durable tooling/workflow facts only.
- `.scratch/<feature>/PRD.md`: accepted product behavior.
- `.scratch/<feature>/issues/*.md`: status, acceptance criteria, implementation notes, progress logs, and remaining risks.
- `plan/*.md`: stable design documents that should match the active PRD direction.

If a class, struct, enum, serialized data shape, or major responsibility changed under `Asset Manager/Assets/_AssetManager/Scripts`, update class inventory in the same turn.

## Editing Rules

- Keep changes surgical. Do not reformat entire documents.
- Preserve raw feedback files as raw evidence; summarize decisions elsewhere.
- Prefer appending a dated decision section or replacing a stale paragraph over mixing old and new rules in one section.
- Use the project's canonical vocabulary from `CONTEXT.md`.
- If old implementation aliases remain in code, document the domain meaning first and mention compatibility names second.

## Verification

After edits:

- Re-read changed sections.
- Run `rg` for the old term or stale behavior that motivated the sync.
- Check `git diff -- <changed files>` to ensure unrelated docs were not rewritten.
- Documentation-only sync does not require Unity tests unless the task also changed code or serialized Unity assets.

Finish by listing which docs were synced, which source of truth won, and any conflicts left unresolved.
