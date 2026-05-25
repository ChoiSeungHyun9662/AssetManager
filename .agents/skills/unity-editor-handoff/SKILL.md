---
name: unity-editor-handoff
description: Produce concrete Unity Editor setup, asset-wiring, scene-inspection, and manual QA guidance for Asset Manager after code or UI changes. Use when code changes need Unity Editor work, the user asks what to check manually, assets must be assigned, scene hierarchy must be adjusted, or final reporting needs Editor follow-up.
---

# Unity Editor Handoff

Use this when code changes are not enough for the user to see or validate the feature in Unity.

## Inspect Before Advising

Read the relevant code and scene/setup docs first:

- `docs/agents/project-memory.md`
- `docs/agents/class-inventory.md`
- The changed MonoBehaviours, ScriptableObjects, and tests
- Relevant `.scratch/**/issues/*.md` acceptance criteria
- Relevant QA docs such as `.scratch/mvp/play-mode-qa-checklist.md` or `.scratch/mvp/UI_INTEGRATION_QA.md`

If scene or asset state can be inspected from text YAML, inspect it. If Unity serialization or binary assets prevent certainty, say what is inferred from code and what must be checked in the Editor.

## Handoff Content

Give the user exact Editor steps:

- Scene path to open.
- Hierarchy object names to select.
- Component names to inspect.
- Field names to assign.
- Expected object names created by runtime shell code.
- Example values or asset names.
- What can be skipped because code has a fallback.
- What must be saved in the scene or asset.

Avoid vague instructions such as "wire the UI". Name the button, panel, sprite, ScriptableObject, or serialized field.

## Distinguish Work Types

- **Automatic**: code or `ProjectShell` creates it at runtime; the user only verifies it.
- **Manual required**: Unity scene, asset reference, sprite assignment, prefab, import setting, or ScriptableObject data must be set by the user.
- **Optional polish**: improves presentation but does not block the feature.
- **Blocked/unknown**: cannot be confirmed without opening Unity.

For UI layout work, explain whether the user should edit RectTransforms in the scene or whether code currently owns the layout. If code owns it and the user wants manual control, recommend the smallest follow-up change to preserve Editor-authored layout.

## Manual QA

Write checks as observable Play Mode flows:

1. Starting state.
2. User action.
3. Expected visual result.
4. Expected state/rule result.
5. Failure symptom to watch for.

Include negative checks when relevant, such as "drop outside the target does nothing" or "failed purchase spends no resources".

## Final Report Add-on

When used after implementation, include:

- What code now handles automatically.
- What the user still needs to do in Unity Editor.
- Manual checks run by Codex, or explicitly "not run" with reason.
- A short troubleshooting list for the most likely missing references, hierarchy names, or stale scene objects.
