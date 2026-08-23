---
name: architect-review
description: Review modular and distributed architecture-sensitive changes for structural risk, quality attributes, and evidence-backed corrective direction. Use when design-level review is needed beyond local correctness.
---

# Architect Review

Use this skill for evidence-led design review when code correctness alone is not enough.

## Use this skill when

- a diff or design changes ownership, dependency direction, contracts, composition, or long-lived abstractions
- the main question is architecture fit, structural risk, or future maintenance cost
- a review needs to separate blocking design defects from non-blocking watch items
- the change may affect reliability, availability, scalability, performance, security, observability, deployability, or cost efficiency

## Do not use this skill when

- the task is only a local correctness review
- there is no architecture-sensitive behavior
- the request is implementation rather than review

## Knowledge Routing

1. Read `KnowledgeBase/INDEX.md`.
2. Read `architecture-review.knowledge.md` before producing findings.
3. Select review mode: modular-monolith by default; distributed only for remote boundaries, asynchronous messaging, independent deployment, eventual consistency, resilience, or distributed observability.
4. In distributed mode, read `distributed-architecture-review.knowledge.md`.
5. Read `dotnet-modular-architecture.knowledge.md` only for applicable architecture lenses.
6. For ResumeEnhancer facts, read `resumeenhancer-architecture-routing.knowledge.md`, then the authority it identifies.

## Specialist Gates

- Load `$domain-driven-design` for business vocabulary, bounded-context, aggregate, lifecycle, or invariant risk.
- Load `$backend-security-coder` for trust boundaries, authentication/authorization, sensitive data, secrets, abuse, privacy, or export risk.
- Load `$performance-optimization` for observed or visible latency, throughput, query, cache, resource, or capacity risk.
- Load `$deep-research` before recommending a concrete distributed platform or pattern not supported by repository evidence.
- Recommend `$architecture-decision-records` when a durable choice changes boundaries, integration, persistence, deployment, or test expectations.

## Review Gate

Do not label a concern architectural without evidence of a boundary, dependency, operational, or evolution consequence. Mark uninspected behavior as unverified.

Do not enter distributed mode merely because a change is large or future-facing. Do not present a technology preference as a requirement, violation, or solution without quality-attribute evidence and constraints.

## Output Requirements

- findings first
- Architecture Impact: High, Medium, or Low, with affected quality attributes and rationale
- severity, evidence, impact, and smallest corrective direction
- blocking defects separate from significant risks and watch items
- required specialist skill, research, or ADR action and why
- residual risks and verification gaps stated explicitly
