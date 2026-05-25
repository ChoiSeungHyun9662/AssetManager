---
name: grill-me
description: Interview the user relentlessly about a plan or design until reaching shared understanding, resolving each branch of the decision tree. Use when user wants to stress-test a plan, get grilled on their design, asks for Socratic questioning, wants concise decision options, or mentions "grill me".
---

# Grill Me

Interrogate the plan until the important decisions are explicit. The goal is not to ask many questions; it is to remove ambiguity that would otherwise cause bad implementation or weak design.

## Default Mode

Ask one question at a time. For each question:

- Explain the decision pressure in one or two sentences.
- Offer two or three concrete candidates.
- Recommend one candidate and say why.
- After the user answers, record the decision briefly and move to the next unresolved branch.

If a question can be answered by exploring the codebase or docs, explore instead of asking.

Do not ask about rules that are already fixed unless there is a functional conflict. Skip questions where every answer means "failed action does nothing" unless failure handling is the actual design topic.

## Batch Decision Mode

Use this when the user asks for efficient grilling, ten questions, candidate choices, or a compressed decision pass.

Before the questions, provide a compact context packet:

- Current understanding: what the plan is trying to achieve.
- Relevant fixed decisions: rules that should not be re-litigated.
- Design pressure: what could go wrong if this branch is underspecified.
- Implementation pressure: which systems or docs will be affected.
- Assumptions: what you are assuming from prior context.

Then ask up to ten questions in this exact shape:

```text
Q1. <question>
Background: <why this decision matters, concise>
Candidate 1: <option>
Candidate 2: <option>
Candidate 3: <optional option>
Recommendation: Candidate N - <short reason>
```

Keep candidates mutually exclusive when possible. If a question needs more than three candidates, split it into a later question.

## After Answers

When the user answers a batch, produce:

- Confirmed decisions, mapped to Q numbers.
- Any answer that conflicts with previous docs or implemented behavior.
- Remaining decision branches and an estimate of how many questions remain.
- The next batch or the next single highest-leverage question.

When the design is ready for implementation, summarize it as decisions, risks, implementation units, and verification criteria.
