---
name: architecture-review
description: Review modular and distributed architecture-sensitive changes for structural risk, quality attributes, and evidence-backed corrective direction. Use when design-level review is needed beyond local correctness.
---

# Architecture Review

Use this skill to review an existing design or diff. It is a finding-producing skill, not an implementation owner.

## Use this skill when

- a change affects ownership, dependency direction, contracts, composition, integration, persistence responsibility, deployment, or long-lived abstractions
- the review must assess structural risk or quality attributes beyond local correctness
- the change may affect reliability, availability, scalability, performance, security, observability, deployability, or cost

## Do not use this skill when

- the task is only a local correctness, style, or test review
- no architecture-sensitive behavior or boundary is involved
- the request is to implement the change; hand findings to the owning implementation workflow

## Owned references

- Read [architecture review guide](references/architecture-review-guide.md) for the generic review procedure and lenses.
- Read [distributed review guide](references/distributed-review-guide.md) only when the review-mode gate below selects distributed review.
- Read [capability expansion](references/capability-expansion.md) when changing this skill's scope, specialist routing, or review-mode contract.
- Read [skill-system maintenance](references/skill-system-maintenance.md) when changing skill authority, source synchronization, or routing policy.
- Read [ResumeEnhancer architecture routing](references/resumeenhancer-architecture-routing.md) only when repository-specific ownership or authority selection is material.

## Workflow

1. Establish the intended behavior from the request, story, change artifact, requirement, or existing authority. Record missing expectations as review limits.
2. Read the generic review guide and inspect the actual diff, affected symbols, dependency path, registrations, runtime path, tests, and operational configuration.
3. Select modular review by default. Select distributed review only for a remote boundary, asynchronous messaging, independent deployment, eventual consistency, distributed transaction, resilience policy, or distributed observability concern.
4. Discover applicable project authority dynamically. Use `KnowledgeBase/INDEX.md` when present as a retrieval map, search its linked topics and the applicable ADR directory/registry by decision area, and verify authority status. Never require a particular knowledge filename or ADR number.
5. Apply only the lenses supported by evidence: ownership, dependency/composition, integration/data, evolution/duplication, operational fitness, and decision traceability.
6. Route a specialist only when its trigger applies; specialists provide constraints or findings and do not duplicate this review.
7. Report findings first, then impact, evidence, smallest corrective direction, residual risk, and verification gaps.

## Specialist boundaries

## References

- Read [capability expansion](references/capability-expansion.md) when reviewing the modular-versus-distributed review boundary or specialist routing contract.
- Read [skill-system maintenance](references/skill-system-maintenance.md) when maintaining backend or architecture skill authority, source synchronization, or routing policy.

## Specialist Gates

- `$architecture-domain-modeling`: business language, bounded contexts, aggregates, lifecycles, and invariants.
- `$backend-dotnet-architecture`: proposed .NET ownership, dependency, composition, or integration design; it does not own independent review findings.
- `$backend-security`: trust boundaries, authorization, sensitive data, secrets, abuse, privacy, and export risk.
- `$quality-performance`: observed or visible latency, throughput, query, cache, resource, or capacity risk.
- `$research-deep`: material evidence gaps or concrete platform/pattern recommendations not supported by repository evidence.
- `$architecture-adr`: recording a durable decision after the decision is understood; it does not replace review evidence.

## Review Gate

## Review gates

- Do not label a concern architectural without evidence of a boundary, dependency, operational, or evolution consequence.
- Do not select distributed review because a change is large or future-facing.
- Do not turn a technology preference into a requirement without quality-attribute evidence and constraints.
- Do not copy project facts from knowledge or ADRs into this skill; cite the applicable authority and report its status.
- Do not implement, edit the reviewed feature, or silently resolve an authority conflict. Return the conflict and the smallest clarification or decision needed.

## Output contract

- `Architecture Impact`: High, Medium, or Low, with affected quality attributes and rationale.
- Findings first, ordered by severity: blocking defect, significant risk, watch item, or unverified concern.
- For each material finding: severity, evidence, consequence, smallest corrective direction, and authority used.
- Specialist, research, or ADR handoff only when its trigger and reason are explicit.
- Residual risks and verification gaps, including checks available versus checks actually run.
