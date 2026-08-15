---
name: plan-writing
description: Write clear, phased implementation plans for ResumeEnhancer changes, connecting story intent to code, tests, and verification. Use when a task is large enough to need a plan before coding.
---

# Plan Writing

Use this skill to turn a ResumeEnhancer user story or change request into a reviewable implementation plan.

## Use this skill when

- a change spans multiple layers or files and benefits from a plan before coding
- you need to sequence frontend, backend, persistence, and test work
- the plan must be durable enough to resume after a context gap

## Do not use this skill when

- the task is a single, obvious edit
- a plan would add process without reducing risk

## Plan structure

1. **Goal** — one or two sentences on the outcome and the story/requirement it serves.
2. **Current behavior** — the relevant request flow (`endpoint -> validator -> handler -> mapper -> repository`).
3. **Impacted layers** — frontend features, `<ModuleName>ModuleWeb`, `<ModuleName>ModuleAM`, `<ModuleName>ModuleSL`, `<ModuleName>ModulePL`, `<ModuleName>ModuleDM`, tests.
4. **Phases** — ordered, reviewable steps with the files each step touches.
5. **Risks and dependencies** — contract changes, migrations, shared files, or rollout concerns.
6. **Verification** — the exact commands and tests that prove completion.
7. **Rollout** — migration/seeding notes and any manual steps.

## ResumeEnhancer rules

- Start from the user story and any linked business requirement.
- Keep contract changes additive unless a breaking change is explicitly required.
- Call out migration and seed-data impact before implementation.
- Keep the plan proportional to the change; do not inflate small tasks.

## Definition of Done

- The plan is concrete enough that another agent could execute it.
- Verification commands are explicit (`dotnet build`, `dotnet test`, `npm run check`).
- Risks and rollout steps are stated up front.
