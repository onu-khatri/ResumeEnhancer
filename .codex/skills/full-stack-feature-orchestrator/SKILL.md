---
name: full-stack-feature-orchestrator
description: Coordinate full-stack feature delivery in ResumeEnhancer across requirements, frontend, API, service layer, persistence, tests, and PR packaging. Use when Codex needs to drive an end-to-end feature from story to review-ready implementation.
---

# Full Stack Feature Orchestrator

Use this skill when a feature spans multiple layers and success depends on coordinated delivery rather than isolated edits.

## Use this skill when

- a story touches frontend, backend, persistence, and tests
- there are dependencies or rollout concerns across layers
- the user wants a coherent path from story to review-ready implementation

## Do not use this skill when

- the task is small and isolated to one layer
- orchestration would add more process than value

## Workflow

1. Start from requirements, acceptance criteria, and current behavior.
2. Identify impacted layers: frontend, API, validation, handlers, persistence, tests, and PR packaging.
3. Surface shared-contract, migration, or concurrency risks before parallel work starts.
4. Decide which steps can be delegated and which must stay coordinated.
5. End with a verification, rollout, and review plan.

## Review lenses

- story traceability
- contract consistency across layers
- migration and deployment risk
- test coverage at the right boundaries
- reviewer clarity and PR packaging

## ResumeEnhancer focus

- frontend feature changes coordinated with Web and SL contracts
- persistence changes flagged early
- branch and worktree safety during multi-story delivery

## Output requirements

- phase plan
- impacted layers
- risks and dependencies
- verification and rollout notes

## Definition of Done

- Backend builds and tests pass: `dotnet build application\ResumeEnhancerApp.slnx` and the relevant test project from `test/`.
- Frontend checks pass: `npm run check` and `npm run build` in the client.
- Cross-layer contracts are consistent and any migration impact is documented.
- The change is packaged for review with story traceability and verification notes.