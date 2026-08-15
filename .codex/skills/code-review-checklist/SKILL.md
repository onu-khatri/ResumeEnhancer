---
name: code-review-checklist
description: Provide a systematic, project-specific checklist for reviewing ResumeEnhancer changes across functionality, security, performance, maintainability, and tests. Use when conducting a review or establishing review standards.
---

# Code Review Checklist

Use this skill to make ResumeEnhancer reviews systematic rather than intuitive. Pair with `production-code-reviewer` for the review workflow and severity model.

## Use this skill when

- reviewing a pull request or local diff
- conducting a code audit
- establishing or teaching review standards for this repository

## Do not use this skill when

- there are no code changes to inspect
- you need architecture-only advice without a diff (use `architect-review`)

## Review sequence

### 1. Context

- [ ] Confirm the user story / requirement being implemented.
- [ ] Confirm scope: is unrelated work mixed into the diff?
- [ ] Confirm changed layers: frontend features, `<ModuleName>ModuleWeb`, `<ModuleName>ModuleAM`, `<ModuleName>ModuleSL`, `<ModuleName>ModulePL`, `<ModuleName>ModuleDM`, tests.

### 2. Correctness

- [ ] Acceptance criteria are actually satisfied.
- [ ] Null, empty, and error states are handled.
- [ ] Validators, handlers, and mappers stay consistent with each other.
- [ ] Response shapes and UI expectations remain aligned.

### 3. Security

- [ ] Ownership/authorization checks filter by `userId` where needed.
- [ ] No secrets or sensitive user data are exposed or logged.
- [ ] Inputs are validated (FluentValidation) and safely rendered/persisted.
- [ ] Error mapping (`ApiEndpointExecutor`) returns 403/404/400 without leaking internals.

### 4. Performance

- [ ] Repository queries are `AsNoTracking`, `AsSplitQuery`, paged, and deterministically ordered.
- [ ] No N+1 queries or redundant fetching.
- [ ] Frontend avoids unnecessary re-renders and heavy client loops.

### 5. Maintainability

- [ ] Responsibilities stay in their owning layer/project.
- [ ] Names and abstractions are clear; no speculative complexity.
- [ ] No avoidable coupling or duplication.

### 6. Tests

- [ ] Tests exist at the right boundary (unit vs integration).
- [ ] Tests prove the changed behavior, not implementation trivia.
- [ ] No obvious gaps in error, permission, or migration coverage.

## Definition of Done for a review

- Findings are severity-ordered (blocking / important / minor / question).
- Each finding has a concrete file reference and one-line rationale.
- Confirmed defects are separated from watch items.
