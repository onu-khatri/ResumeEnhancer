# PRD Writing Guide

Use this guide when the source material needs to be shaped into a clear product
decision document.

## Purpose

A PRD aligns product, design, and engineering on the problem, intended outcome,
scope, and decisions needed before delivery. It answers:

- What problem are we solving and for whom?
- What behavior or capability is required?
- What is deliberately excluded?
- How will the team recognize a successful outcome?

## Writing Principles

- Start with the documented user or business pain, then explain the proposed
  capability.
- Use plain, consistent product language. Keep implementation details in a
  technical design unless they constrain product scope or delivery.
- Make requirements observable and reviewable. Use identifiers when a PRD has
  multiple requirements.
- Prefer source-backed facts. Label missing information as an assumption or
  open question instead of using template examples as evidence.
- Keep the document proportionate: a narrow story should not become a long
  feature charter.

## Section Intent

| Section | What it should establish |
| --- | --- |
| Executive summary | The problem, proposed direction, and intended result. |
| Problem and opportunity | Who is affected, the pain, and why it matters. |
| Scope | Clear in-scope, out-of-scope, and future boundaries. |
| Requirements | Specific behavior, priorities, and acceptance expectations. |
| Success signals | Metrics where available, or an explicit measurement plan/gap. |
| Risks and dependencies | Constraints, unresolved decisions, and mitigation. |
| Traceability | The BR and story-pack evidence that supports the document. |

## Requirements

Write one behavior per requirement where possible. For example:

```markdown
### REQ-001: [Capability]

[User-visible or business behavior].

Acceptance expectations:
- [Observable condition]
- [Observable condition]
```

Use `Must`, `Should`, and `Could` priorities only when the source establishes a
priority or the PRD needs an explicit scope tradeoff.

## Evidence And Metrics

Include metrics, baselines, targets, quotes, and launch dates only when cited
sources provide them. If the desired outcome is understood but measurement is
not, state what needs validation instead of inventing a number.

## Maintenance

Treat a PRD as a living decision artifact. On update, reconcile it against the
current sources, keep valid rationale, remove contradictions, refresh dates,
and do not turn it into an edit log.
