---
name: code-refactoring-tech-debt
description: Reduce technical debt in ResumeEnhancer through scoped refactoring that preserves behavior while improving structure, duplication, and maintainability. Use when Codex needs to clean up existing code, prepare for new features, or address architectural friction.
---

# Code Refactoring Tech Debt

Use this skill to make the codebase cheaper to change without disguising a redesign as a refactor.

## Use this skill when

- a code area is slowing feature work through duplication, unclear ownership, or brittle structure
- a cleanup should happen before new behavior is added
- you want a safer, reviewable refactor plan

## Do not use this skill when

- the proposed change is really a redesign
- there is no reliable verification path for preserved behavior

## Refactoring workflow

1. Name the debt precisely: duplication, mixed responsibilities, unstable tests, poor boundaries, or query sprawl.
2. Establish a behavior baseline using tests, fixtures, or observable current behavior.
3. Refactor in small passes that keep the diff understandable.
4. Stop when the code becomes easier to change; do not continue polishing for its own sake.
5. Record remaining debt separately if it does not belong in the same change.

## Review lenses

- preserved behavior
- reduced cognitive load
- smaller blast radius for future changes
- fewer hidden dependencies
- improved testability

## ResumeEnhancer focus

- bloated handlers or repositories
- duplicated frontend form and mapping logic
- persistence concerns leaking upward
- cross-layer naming drift that weakens story traceability

## Output requirements

- current debt statement
- proposed refactor boundary
- verification approach
- residual debt left intentionally untouched

## Definition of Done

- Behavior is preserved and proven by the existing test suite.
- Backend: `dotnet build application\ResumeEnhancerApp.slnx` and `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore` pass.
- Frontend: `npm run check` and `npm run build` pass in the client.
- The diff stays small and reviewable; unrelated debt is recorded separately.
