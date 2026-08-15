---
name: pr-creator
description: Create high-quality pull requests for ResumeEnhancer with branch safety, story traceability, reviewer context, and clear validation notes. Use when Codex needs to prepare a branch, stage work, summarize code changes, link user stories or business requirements, and produce a reviewer-ready PR flow.
---

# PR Creator

Use this skill to finish implementation work safely and package it for review with the right branch, story references, architectural context, and verification notes.

## Workflow

1. Read `AGENTS.md`, the relevant user story, and any touched business requirements.
2. Check the current branch. If it is `main`, create a short `codex/` branch name that includes the feature intent and a timestamp.
3. Review the diff before staging so the PR summary reflects the real change.
4. Group the PR around one coherent scope. If the diff mixes unrelated work, separate it before creating the PR.
5. Summarize the change using project language: frontend, web boundary, service layer, persistence, tests, and story traceability.
6. Include user story IDs and requirement references when they exist.
7. Record what was verified and what remains unverified.

## ResumeEnhancer Rules

- Mention module boundaries explicitly when they matter.
- Call out changes to `<ModuleName>ModuleWeb`, `<ModuleName>ModuleSL`, `<ModuleName>ModulePL`, and frontend `features/resume` separately when touched.
- Mention schema or migration impact whenever persistence changes.
- Mention validation, mapping, and test coverage when relevant.

## Verification

- State which verification commands were run, not just that code changed.
- Backend: `dotnet build application\ResumeEnhancerApp.slnx` and the relevant `dotnet test` project from `test/`.
- Frontend: `npm run check` and `npm run build` in the client.
- Separate verified behavior from untested or risky areas in the PR description.