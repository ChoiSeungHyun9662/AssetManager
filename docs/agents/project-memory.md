# Project Memory

Recurring facts that should carry across agent sessions. Keep this file short: add only information that is stable, likely to recur, and changes how future work should be run.

## Unity Test Runner

Unity project root:

```text
C:\Users\lusia\CodexWorkspaces\AssetManager\Asset Manager
```

Editor version currently used by the project:

```text
6000.4.5f1
```

Do not run Unity Editor from the default Codex sandbox context. In this environment, Unity can exit before writing its log with unknown software exception `0x80000003`, even for a minimal command such as:

```text
Unity.exe -batchmode -quit
```

Run Unity batchmode commands outside the sandbox / escalated execution context.

EditMode tests:

```text
Unity.exe -batchmode -projectPath "<project>" -runTests -testPlatform EditMode -assemblyNames AssetManager.Tests.EditMode -runSynchronously -testResults "<results.xml>" -logFile "<run.log>"
```

PlayMode tests:

```text
Unity.exe -batchmode -projectPath "<project>" -runTests -testPlatform PlayMode -assemblyNames AssetManager.Tests.PlayMode -testResults "<results.xml>" -logFile "<run.log>"
```

Do not pass `-runSynchronously` to PlayMode test runs. In this project/environment it hung in the PlayMode prebuild path. Use `-runSynchronously` for EditMode only.

When wrapping Unity with PowerShell `Start-Process`, pass the Unity arguments as one quoted string. `-ArgumentList` arrays can split the `Asset Manager` project path on the space before Unity receives it.

Known verification from 2026-05-10:

- Escalated EditMode batchmode run: 20/20 passed.
- Escalated PlayMode batchmode run without `-runSynchronously`: 6/6 passed.

## Updating This File

Use this workflow at the end of any diagnosis, test-run failure, build failure, editor automation issue, dependency setup issue, or repeated manual workaround:

1. Decide whether the finding should be promoted.
2. If yes, update this file in the same turn as the fix or workaround.
3. Keep only the durable rule, the command shape, and the latest known verification.
4. Leave detailed logs, timestamps, failed attempts, and one-off history in the relevant issue comment.
5. If the finding supersedes an older rule here, replace the older rule instead of appending a contradictory note.

Promote a finding here when it meets all of these:

- It is not a one-off issue comment.
- It affects how future agents should run commands, tests, builds, tools, or local workflows.
- It is likely to save time or avoid repeated false diagnosis in later sessions.

Do not promote a finding when any of these are true:

- It is specific to one failed run and has no expected recurrence.
- It is already captured by an issue's acceptance criteria or implementation notes.
- It is speculative and has not been verified.
- It would make this file a chronological log instead of an operating memory.

When in doubt, add a short issue comment first. Promote only after the same problem recurs or the workaround becomes part of the normal workflow.
