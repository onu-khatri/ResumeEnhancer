---
name: ai-code-review
description: Combine automated analysis with AI-assisted reasoning to review ResumeEnhancer changes, triage large diffs, and focus human attention on security, contracts, and architecture. Use when reviewing changes with AI assistance or setting up review automation.
---

# AI Code Review

Use this skill to widen review coverage with AI without replacing engineering judgment. It complements `production-code-reviewer` and `code-review-checklist`.

## Use this skill when

- triaging a large or high-risk diff before deep review
- augmenting a human review with edge-case and architectural reasoning
- deciding what an AI review should focus on for this repository

## Do not use this skill when

- the change is small enough to review directly
- you only need a final approval decision without additional analysis

## Workflow

1. Classify the change by size, language, and risk (feature, bug fix, refactor, or breaking change).
2. Run automated signals first: `dotnet build`, `dotnet test`, `npm run check`, plus any configured linters/analyzers.
3. Give the AI the real diff, surrounding context, and the acceptance criteria.
4. Ask it to reason about edge cases, architectural fit, and missing tests.
5. Verify every AI finding against the actual code before reporting it.

## ResumeEnhancer review focus

- contract drift between frontend and backend (AM requests/responses vs UI models)
- validator or mapper mismatches
- boundary leakage across `<ModuleName>ModuleWeb`, `<ModuleName>ModuleSL`, and `<ModuleName>ModulePL`
- ownership/authorization gaps on resume data (IDOR)
- missing tests around permissions, error handling, and migrations

## Good AI review prompt shape

Include:
- the actual diff or changed files
- surrounding context needed to understand the call path
- the story intent or acceptance criteria
- explicit focus areas (security, performance, compatibility)

## Rules

- Verify every finding before surfacing it as a defect.
- Never present speculative warnings as confirmed bugs.
- Prefer precise, reproducible findings with file references over long generic commentary.

## Definition of Done

- Automated checks (build/tests) have run and their results are stated.
- AI findings are each verified against the code and classified by severity.
- The final output separates confirmed defects from questions and watch items.
