---
name: production-code-reviewer
description: Perform production-grade code review for ResumeEnhancer with a defect-first mindset across architecture, correctness, security, tests, and maintainability. Use when Codex needs to review diffs, pull requests, or uncommitted changes before merge.
---

# Production Code Reviewer

Use this skill to produce high-signal review findings that protect correctness and delivery quality without devolving into style nitpicks.

## Use this skill when

- reviewing pull requests, staged changes, or local diffs
- auditing risk before merge
- preparing or teaching repository-specific review standards

## Do not use this skill when

- there are no implementation changes to inspect
- the task is only to write code, not review it
- the user needs architecture advice without a concrete diff

## Review workflow

1. Read `AGENTS.md`, the user story, and any touched requirements.
2. Inspect the real diff and enough surrounding code to understand the full execution path.
3. Check architecture fit, correctness, validation, mapping, persistence behavior, and tests.
4. Look for security, performance, and rollout risks in proportion to the change.
5. Report findings by severity with concrete impact and narrow file references.

## Review lenses

- correctness and regression risk
- module boundary discipline
- validator, mapper, and contract consistency
- query shape, transaction safety, and error handling
- user-facing loading, error, and permission states
- tests that prove the changed behavior

## Read these references when needed

- `references/implementation-playbook.md` for detailed code-review patterns
- `references/review-playbook.md` for the ResumeEnhancer review sequence
- `references/review-checklist.md` for systematic checklist coverage
- `references/ai-review-playbook.md` for AI-assisted review and triage patterns

## Output requirements

- findings first
- severity-ordered issues
- concise risk summary
- verification and testing gaps

## Severity model

- **Blocking** — must fix before merge: correctness, security, or regression risk.
- **Important** — should fix; discuss if the author disagrees (architecture drift, missing tests, contract inconsistency).
- **Minor / nit** — nice to have, non-blocking (naming, redundant code, style).
- **Question** — intent unclear; ask instead of asserting.

Distinguish confirmed defects from watch items; never present speculative warnings as confirmed bugs.

## Verification

- Run the smallest meaningful tests for the touched area before reporting: `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore` for backend, and `npm run check` for frontend.
- Only claim a defect after confirming it against the real code; otherwise mark it as a question or watch item.
