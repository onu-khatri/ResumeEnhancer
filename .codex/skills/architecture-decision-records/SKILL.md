---
name: architecture-decision-records
description: Write clear architecture decision records for ResumeEnhancer that capture context, options, tradeoffs, status, and consequences in project-specific language. Use when Codex needs to document a meaningful technical decision or proposed architecture change.
---

# Architecture Decision Records

Use this skill to create durable ADRs that another engineer or agent can understand months later without this conversation.

## Use this skill when

- a technical decision changes module boundaries, integration style, persistence, deployment, or testing expectations
- multiple options were considered and the tradeoff should remain visible
- a design should be documented before or alongside implementation

## Do not use this skill when

- the change is a minor implementation detail
- the note belongs in a code comment, commit message, or PR description instead
- there is no actual decision to record

## ADR workflow

1. Capture the business and technical context that forced the decision.
2. Name the decision drivers explicitly: maintainability, delivery speed, security, performance, cost, or team familiarity.
3. Document the realistic options, not strawmen.
4. Record the chosen path, why it won, and what it costs.
5. State consequences, follow-up actions, and whether the ADR is proposed, accepted, rejected, deprecated, or superseded.

## Recommended structure

- title
- status
- date or version context
- problem and constraints
- decision drivers
- considered options
- decision
- consequences
- follow-up actions
- related stories, requirements, or ADRs

## ResumeEnhancer focus

- host versus module composition rules
- Web, AM, SL, PL, and DM ownership boundaries
- frontend and backend contract coordination
- migration, seeding, and testing implications

## Quality bar

- be specific enough that future contributors can repeat the reasoning
- document both upside and cost
- avoid abstract pattern language when the repository has concrete terms