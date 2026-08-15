---
name: architect-review
description: Review ResumeEnhancer changes from an architecture perspective, focusing on layering, module boundaries, dependency direction, scalability, integration risk, and long-term maintainability. Use when Codex needs a design-level review beyond line-by-line correctness.
---

# Architect Review

Use this skill for design-level review when code correctness alone is not enough.

## Use this skill when

- the main question is whether a solution fits the repository architecture
- a change touches multiple layers, shared contracts, or long-lived abstractions
- you need to identify future maintenance cost before merge

## Do not use this skill when

- the task is only to find local bugs in a small change
- there is no architecture-sensitive behavior involved
- the request is purely implementation, not review

## Review workflow

1. Read the story, requirement, or stated design goal.
2. Inspect the actual diff and enough surrounding code to understand the call path.
3. Check dependency direction across frontend, Web, AM, SL, PL, and DM.
4. Evaluate whether responsibilities are leaking across transport, domain, and persistence concerns.
5. Separate immediate design defects from watch items that do not block the change.

## Review lenses

- module boundaries and ownership
- dependency direction and composition root discipline
- contract stability and duplication
- persistence leakage into higher layers
- scale, testability, and future refactor cost

## ResumeEnhancer focus

- Minimal API endpoint boundaries
- validator, handler, repository, and mapper placement
- frontend feature boundaries and API-client ownership
- cross-layer naming consistency and traceability to user stories

## Output requirements

- findings first
- severity and impact
- what should change now
- what should be documented or monitored later