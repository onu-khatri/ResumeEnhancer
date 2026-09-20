---
title: Resilience Policy Ownership And Failure Semantics
status: accepted
date: 2026-09-19
---

# Resilience Policy Ownership And Failure Semantics

## Context and constraints

ResumeEnhancer is a net10 modular monolith with shared caching and persistence infrastructure, an Auth lifecycle, and a durable Auth outbox. Redis, MemCache, database, and downstream provider outages are expected operational conditions. Limiter exhaustion remains fail-open by the approved Auth design, while audit failure is tolerated by the protected transition and retained for durable retry. Focused tests must not require live providers. The existing approved lifecycle/limiter schema is reused; this decision does not authorize a migration.

## Decision drivers

Security, correctness, idempotency, explicit module ownership, bounded resource use, redacted observability, deterministic testability, and migration safety drive this decision.

## Ownership and package matrix

| Package | Version | Owner | Use |
| --- | --- | --- | --- |
| `Microsoft.Extensions.Resilience` | `10.1.0` | `Core.CommonLibrary` | Provider-neutral named pipelines and the package-neutral execution wrapper |
| `Microsoft.Extensions.Http.Resilience` | Not added | Future concrete HTTP owner | No concrete safe/idempotent outbound HTTP client seam exists |

Core CommonLibrary owns the resilience package, profile binding/validation, named pipeline construction, and the package-neutral execution wrapper. Caching owns provider selection and provider-native atomic adapters. Persistence owns the serializable database limiter transaction. Auth SL owns audit orchestration and consumes the wrapper for durable audit delivery. The host binds the `Resilience` section once through Core. No package is added to DM or AM, and no speculative HTTP client is created.

## Policy rules

`LimiterProvider`, `DbCache`, `AuditOutbox`, and `SafeOutboundHttp` are bound from `Resilience:Profiles`, validated against bounded retry, delay, jitter, timeout, circuit, and concurrency values, and selected explicitly by use case. Pipelines compose concurrency, timeout, bounded classified retry with jitter, and circuit breaking. Limiter provider calls preserve atomic provider operations and fail open with a redacted degradation signal. DB/cache transient or conflict handling is bounded and state-safe. Audit persistence failure does not fail the protected transition; it queues a redacted durable retry or emits safe exhaustion telemetry. HTTP policy is reserved for safe or explicitly idempotent operations and uses timeout, bounded retry, circuit breaking, and jitter.

The wrapper never retries cancellation. `NonIdempotentMutation` executes once even when a profile has retries configured; login/session creation, refresh rotation, logout/revocation, password change/reset, verification completion, registration, and challenge consumption therefore remain outside automatic retry. The MemCache `add`/`incr` expiry-race loop is provider-protocol handling, not a second resilience policy.

## Non-idempotent Auth rule

There is no broad automatic retry for login session creation, refresh rotation, logout/revocation, password change/reset, verification completion, registration, or challenge consumption. Application-level retry requires an idempotency key or state predicate and an explicit owner decision.

## Observability and redaction

Allowed operational fields are correlation ID, operation/provider category, attempt count, elapsed-time bucket, outcome, circuit state, and degraded/fail-open/audit-exhausted classification. Passwords, tokens, cookies, CSRF values, raw headers, connection strings, SQL, exception text, and unredacted user data are forbidden in logs, audits, telemetry, and public responses.

## Testing and rollout

Tests use deterministic fake pipelines and provider adapters; live providers are optional additive evidence, not a requirement. Production startup rejects invalid profiles. Package restore/build and focused unit/integration tests verify profile binding, environment-key overrides, bounded behavior, fail-open limiter behavior, audit tolerance, and no-retry Auth mutations.

## Migration and rollback

The approved schema is reused. Any new table, column, index, lease field, or backfill is a stop-and-replan boundary. Rollback disables a policy or provider through configuration while retaining durable rows; it does not re-enable process-local production throttling or delete retry evidence.

## Alternatives and consequences

Direct Polly-only policy, package-everywhere, latest-version pinning, and broad mutation retries were rejected. The chosen approach keeps ownership explicit and dependency scope small, but requires disciplined named-profile configuration and deterministic fakes. The absence of an HTTP package means a future concrete client must add a separately reviewed host seam and package reference.

## Follow-up and related artifacts

This ADR supports OpenSpec change `gh-36-authentication-authorization-hardening`, tasks 5.1–5.4, the approved plan `batch-revised-core-resilience-5-1-5-4.md`, ADR-001, ADR-002, and the applicable API/application, EF Core, persistence, and caching KnowledgeBase topics. No new schema, migration, seed data, or lease field is introduced by this decision.
