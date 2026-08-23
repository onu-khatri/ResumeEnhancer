---
title: Distributed Architecture Review
intent: Help an agent review a real distributed-system boundary without treating distributed technologies as default architecture.
scope: Reusable distributed-boundary, consistency, resilience, observability, rollout, and operability review guidance. Excludes provider and platform selection without evidence.
audience: Autonomous Codex architecture reviewers and implementation agents
last_reviewed: 2026-08-23
---

# Distributed Architecture Review

## When To Use This Knowledge

Read this only when a reviewed change creates or materially changes a remote boundary, asynchronous message flow, independently deployed component, eventual-consistency model, distributed transaction, resilience policy, or distributed-observability requirement. Do not use it for ordinary modular-monolith collaboration.

## Review Procedure

1. State the boundary and why local composition is insufficient.
2. Define synchronous or asynchronous interaction, availability and latency expectations, and failure ownership.
3. Define consistency model, idempotency, ordering, duplication, timeout, retry, compensation, and recovery behavior.
4. Assess data ownership, trust boundaries, observability, rollout/rollback, operational cost, and verification strategy.
5. Require repository evidence or `$deep-research` before naming a broker, service mesh, cloud, container, or provider technology.

## High-Risk Anti-Patterns

- Splitting a component without independent ownership, deployment, scale, or failure-isolation pressure.
- Using remote calls as a substitute for an explicit data relationship or ownership decision.
- Adding retries without idempotency, timeout, duplicate handling, and a terminal failure policy.
- Publishing events without consumer contracts, failure handling, ordering assumptions, or observability.
- Accepting eventual consistency without a user-visible recovery state or compensation path.
- Presenting a platform preference as architecture rationale.

## Verification Evidence

- Failure and recovery tests, contract compatibility, timeout/retry behavior, idempotency and duplicate handling, cross-boundary traceability, and rollout/rollback evidence when applicable.
- Quality-attribute evidence for availability, reliability, latency, throughput, scalability, security, observability, deployability, and cost where material.

## Boundary

This topic evaluates patterns, not vendor products. A concrete platform recommendation requires evidence from the target system or `$deep-research`.
