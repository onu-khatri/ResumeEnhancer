---
title: Architecture Review
intent: Help an agent perform an evidence-led, finding-first architecture review beyond local code correctness.
scope: Reusable architecture-review method for boundary, dependency, integration, evolution, and operability risk. Excludes project layer names and persistence-specific rules.
audience: Autonomous Codex implementation agents, architecture planners, and reviewers
last_reviewed: 2026-08-23
---

# Architecture Review

## When To Use This Knowledge

Read this after `KnowledgeBase/INDEX.md` when a diff or design changes module ownership, dependency direction, cross-component contracts, composition, persistence responsibility, scalability posture, or long-lived abstractions. Do not use it instead of a local bug review or implementation workflow.

## Review Mode Selection

- Use **modular-monolith mode** by default for ownership, dependency, composition, contract, persistence, and evolution review.
- Use **distributed mode** only when a change creates a remote boundary, asynchronous communication, independently deployed component, eventual-consistency model, resilience requirement, or distributed-observability concern. Retrieve `distributed-architecture-review.knowledge.md` before applying that mode.
- Do not select distributed mode because a design is merely large, future-facing, or composed of multiple local modules.

## Review Workflow

1. Establish the claimed intent from the requirement, story, ADR, or change description. State missing expected behavior as a review limitation.
2. Trace the changed path through entry, orchestration, domain policy, persistence/integration, composition, and visible result. Read surrounding code and registrations, not only the diff.
3. Apply relevant ownership, dependency, composition, integration, evolution, and operability lenses.
4. Tie each concern to a concrete failure or maintenance consequence. A preference without observable risk is not a finding.
5. Classify findings: blocking defects violate a required boundary or risk incorrect behavior; significant risks need resolution before dependent work; watch items require documentation or monitoring but do not block the change.
6. Report findings first with evidence, impact, and the smallest corrective direction. Keep unverified concerns separate from confirmed defects.
7. State residual risks and verification gaps honestly.

## Review Procedure

1. Establish intended behavior and mark missing expectations as review limits.
2. Trace the changed path through entry, orchestration, domain policy, persistence or integration, composition, and observable outcome.
3. Test relevant ownership, dependency, integration, evolution, and operability lenses against concrete consequences.
4. Classify confirmed findings as blocking, significant risk, or watch item; keep unverified concerns separate.
5. Report evidence, impact, smallest correction, residual risk, and verification gap.

## Review Lenses

- **Boundary and ownership:** Responsibilities remain with the owning layer or component; private details do not leak through public contracts.
- **Dependency and composition:** Reference direction supports substitution, testability, and independently controlled registration.
- **Integration and data:** Interaction choice is proportionate, explicit, and considers consistency, failure, and ownership.
- **Evolution and duplication:** Policy is centralized in the right place and the extension path does not depend on parallel abstractions.
- **Operational fitness:** Failure behavior, authorization, cancellation, configuration, observability, rollout, and test seams are considered when relevant.
- **Decision traceability:** Durable choices follow an existing authority or are identified as needing an ADR.

## Architecture Impact Assessment

Rate the structural consequence as `High`, `Medium`, or `Low`. Name the affected quality attributes and evidence; the rating never replaces findings.

- **High:** integrity, data consistency, trust, availability, or a durable boundary can fail or become costly to reverse.
- **Medium:** maintainability, operability, performance, testability, scalability, or deployability has a meaningful but contained consequence.
- **Low:** structural reach is limited and no material quality-attribute regression is evidenced.

Assess only attributes relevant to the change: maintainability, testability, reliability, availability, scalability, performance, security, observability, deployability, and cost efficiency.

## Specialist Review Gates

- Use `$domain-driven-design` for business-language, context, aggregate, lifecycle, or invariant concerns.
- Use `$backend-security-coder` for trust, authorization, sensitive-data, secret, abuse, privacy, or export concerns.
- Use `$performance-optimization` when latency, throughput, query, cache, resource, or capacity risk is observed or visible.
- Use `$deep-research` before concrete distributed platform or pattern recommendations unsupported by repository evidence.
- Recommend `$architecture-decision-records` for durable boundary, integration, persistence, deployment, or test-expectation decisions.

## Finding-Quality Example

**Good finding:** it identifies evidence, consequence, and the smallest correction.

```text
Blocking: Create<ModuleName>Handler depends on IOtherModuleRepository.
Impact: application orchestration depends on another module's persistence detail.
Direction: replace it with the owning module's narrow lookup or snapshot contract.
Evidence: constructor dependency and the cross-module contract boundary.
```

**Bad finding:** it reports a preference without architectural consequence.

```text
This repository name is not clean. Rename it.
```

## Boundaries

- Do not report a style preference as an architecture defect without a concrete dependency, ownership, operational, or evolution consequence.
- Do not duplicate a production code review. Route local correctness, security, and test defects to the discipline that owns them unless they create an architectural consequence.
- Do not infer ownership from a class name or folder. Trace dependencies, registrations, and the runtime path.
- Do not recommend abstraction, modularization, or distribution without identifying present pressure and cost.
- Do not label uninspected behavior compliant. Mark it unverified and identify the smallest evidence needed.

## Verification Evidence

- Cite the inspected requirement or ADR, changed symbols, dependency path, and relevant registration or test seam for each material finding.
- Identify whether focused unit, integration, architecture, or composition tests prove the risky boundary; distinguish available tests from tests actually run.
- Recheck governing project authority whenever a conclusion depends on local architecture rather than generic reasoning.
- Assign a specific residual risk to a follow-up ADR, monitoring action, or test only when it has a clear owner.

## Project Adaptation Boundary

This topic is generic. Retrieve `resumeenhancer-architecture-routing.knowledge.md` for ResumeEnhancer-specific authority and do not copy its module, helper, or persistence rules here.

When a finding depends on module ownership or cross-module behavior, check ADR-001 or ADR-002 through routing and report their status. Do not infer an undocumented exception.

## Discover Locally Only When

Inspect the actual diff, dependency graph, registrations, affected tests, and authority status. Do not recite generic review lenses without evidence.
