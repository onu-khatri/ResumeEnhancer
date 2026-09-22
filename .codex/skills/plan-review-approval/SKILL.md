---
name: plan-review-approval
description: Review plans created by workflow-planning and automatically approve only plans that meet explicit evidence, completeness, and risk criteria.
metadata:
  author: ResumeEnhancer
  version: "1.0"
---

# Plan Review and Approval

Review a plan produced by `$workflow-planning` and make one controlled
decision: approve, return for revision, escalate, or reject. This skill does
not implement the plan.

## Required context

Read the plan, source request, current-state evidence, applicable repository
instructions, and only the authorities needed for the plan's decisions. Do not
bulk-load unrelated knowledge or treat assumptions as facts.

## Review criteria

Record evidence, confidence, gaps, and actionable findings for:

- requirements coverage and measurable completion conditions;
- architecture, ownership, boundaries, and project conventions;
- security, privacy, abuse resistance, and failure handling;
- testing and validation, including negative, integration, and regression paths;
- knowledge and authority alignment;
- dependencies, sequencing, and external prerequisites;
- breaking changes, compatibility, rollout, rollback, and migration impact;
- missing steps, decisions, risks, ownership, or handoff evidence.

## Automatic approval

Approve automatically only when all conditions hold:

1. The plan has a clear outcome, scope, owner, dependencies, decisions,
   validation, recovery, and one next safe action.
2. Every review category is supported by current evidence and no material gap
   or contradiction remains.
3. Confidence is high and there is no unresolved architectural ambiguity,
   conflicting requirement or authority, high-risk security/privacy/data or
   irreversible migration concern, or externally breaking change.
4. Plan identity, location, ownership, and current approval state are valid.

When all criteria pass, update only the plan approval metadata with
`status: Approved`, `approved_by: automated-plan-review-approval`,
`approved_at`, and concise notes referencing the completed review. Do not alter
the plan body or scope during approval.

## Routing decisions

- `RevisionRequired`: return concrete findings to the planner and keep the plan
  unapproved.
- `Escalate`: require a human decision for architectural ambiguity, conflicting
  requirements, high-risk changes, or insufficient confidence.
- `Rejected`: use only for invalid, duplicated, prohibited, or incoherent work.

Automatic approval does not bypass implementation ownership,
development-entry readiness, code review, verification, or delivery gates. It
authorizes only the plan transition.

## Report contract

Return the plan path, category results, evidence and authorities, confidence,
decision (`Approved`, `RevisionRequired`, `Escalate`, or `Rejected`), whether
approval metadata changed, findings, context expansions, and exactly one next
safe action. Never claim implementation, test, merge, or delivery completion
from plan approval.
