---
name: tdd
description: Test-driven development with red-green-refactor loop. Use when user wants to build features or fix bugs using TDD, mentions "red-green-refactor", wants integration tests, or asks for test-first development.
---

# Test-Driven Development

## Philosophy

**Core principle**: Tests should verify behavior through public interfaces, not implementation details. Code can change entirely; tests shouldn't.

**Good tests** are integration-style: they exercise real code paths through public APIs. They describe _what_ the system does, not _how_ it does it. A good test reads like a specification - "user can checkout with valid cart" tells you exactly what capability exists. These tests survive refactors because they don't care about internal structure.

**Bad tests** are coupled to implementation. They mock internal collaborators, test private methods, or verify through external means (like querying a database directly instead of using the interface). The warning sign: your test breaks when you refactor, but behavior hasn't changed. If you rename an internal function and tests fail, those tests were testing implementation, not behavior.

See [tests.md](tests.md) for examples and [mocking.md](mocking.md) for mocking guidelines.

## Transparency Contract

TDD work must be understandable to a project owner who may not be comfortable reviewing implementation details. Do not rely on "trust me, the code changed" as the user's review surface.

For every non-trivial TDD task, expose the work in behavior-first terms:

- State assumptions before coding, including what is intentionally out of scope.
- Name the user-visible or system-visible behaviors being protected by tests.
- For each RED/GREEN cycle, summarize the failing behavior, the minimal change made, and the passing check.
- When Unity manual testing is relevant, describe the exact scene, flow, or interaction tested.
- Call out any unverified behavior, residual risk, or test gap instead of implying complete coverage.

The user should be able to review the behavior list, automated checks, Unity manual checks, and remaining risks without needing to read the code.

## Anti-Pattern: Horizontal Slices

**DO NOT write all tests first, then all implementation.** This is "horizontal slicing" - treating RED as "write all tests" and GREEN as "write all code."

This produces **crap tests**:

- Tests written in bulk test _imagined_ behavior, not _actual_ behavior
- You end up testing the _shape_ of things (data structures, function signatures) rather than user-facing behavior
- Tests become insensitive to real changes - they pass when behavior breaks, fail when behavior is fine
- You outrun your headlights, committing to test structure before understanding the implementation

**Correct approach**: Vertical slices via tracer bullets. One test → one implementation → repeat. Each test responds to what you learned from the previous cycle. Because you just wrote the code, you know exactly what behavior matters and how to verify it.

```
WRONG (horizontal):
  RED:   test1, test2, test3, test4, test5
  GREEN: impl1, impl2, impl3, impl4, impl5

RIGHT (vertical):
  RED→GREEN: test1→impl1
  RED→GREEN: test2→impl2
  RED→GREEN: test3→impl3
  ...
```

## Workflow

### 1. Planning

When exploring the codebase, use the project's domain glossary so that test names and interface vocabulary match the project's language, and respect ADRs in the area you're touching.

Before writing any code:

- [ ] Confirm with user what interface changes are needed
- [ ] Confirm with user which behaviors to test (prioritize)
- [ ] State assumptions, non-goals, and the manual Unity verification target if applicable
- [ ] Identify opportunities for [deep modules](deep-modules.md) (small interface, deep implementation)
- [ ] Design interfaces for [testability](interface-design.md)
- [ ] List the behaviors to test (not implementation steps)
- [ ] Define the final "verification receipt": tests run, Unity checks performed, and known gaps
- [ ] Get user approval on the plan

Ask: "What should the public interface look like? Which behaviors are most important to test?"

**You can't test everything.** Confirm with the user exactly which behaviors matter most. Focus testing effort on critical paths and complex logic, not every possible edge case.

### 2. Tracer Bullet

Write ONE test that confirms ONE thing about the system:

```
RED:   Write test for first behavior → test fails
GREEN: Write minimal code to pass → test passes
```

This is your tracer bullet - proves the path works end-to-end.

### 3. Incremental Loop

For each remaining behavior:

```
RED:   Write next test → fails
GREEN: Minimal code to pass → passes
```

Rules:

- One test at a time
- Only enough code to pass current test
- Don't anticipate future tests
- Keep tests focused on observable behavior
- Keep the user-facing progress log behavior-based: RED behavior, GREEN change, verification result

### 3a. Unity Verification Economy

Unity test launches are expensive. Preserve TDD intent, but choose the cheapest trustworthy verification signal for the current slice.

For pure rule modules:

- It is acceptable to write 2-3 closely related behavior tests before running EditMode, when they exercise the same public rule surface and do not force speculative implementation.
- Still keep the implementation incremental: make the smallest rule change that satisfies the current grouped behaviors, then run EditMode and report the group as one RED/GREEN slice.
- Do not group unrelated domains, UI wiring, or state-machine transitions just to reduce test runs.

For compiler RED:

- If the expected RED is a missing type, missing method, wrong signature, or assembly reference issue, use the fastest available compile/static signal instead of a full Unity test run when possible.
- A compiler/static RED is enough only for proving compile failure; behavior still needs EditMode or PlayMode verification once implemented.

For Unity-visible behavior:

- Keep PlayMode tests for buttons, panels, scene objects, click wiring, visible state, and gating.
- Run the relevant final EditMode and PlayMode suites before marking the task complete, unless blocked; report any skipped suite and remaining risk.

### 4. Refactor

After all tests pass, look for [refactor candidates](refactoring.md):

- [ ] Extract duplication
- [ ] Deepen modules (move complexity behind simple interfaces)
- [ ] Apply SOLID principles where natural
- [ ] Consider what new code reveals about existing code
- [ ] Run tests after each refactor step

**Never refactor while RED.** Get to GREEN first.

## Checklist Per Cycle

```
[ ] Test describes behavior, not implementation
[ ] Test uses public interface only
[ ] Test would survive internal refactor
[ ] Code is minimal for this test
[ ] No speculative features added
[ ] User-facing progress explains behavior and verification, not private implementation trivia
```

## Completion Report

End each TDD task with a compact verification receipt:

- Behavior changed: what the system now does from a user's or caller's perspective.
- Automated tests: which EditMode, PlayMode, or other test suites ran and whether they passed.
- Unity manual checks: which scene/flow/interactions were exercised, or "not run" with a reason.
- Files touched: only the relevant files and why they changed.
- Remaining risk: anything not covered, flaky, environment-dependent, or deferred.

This report is part of the workflow, not optional polish.
