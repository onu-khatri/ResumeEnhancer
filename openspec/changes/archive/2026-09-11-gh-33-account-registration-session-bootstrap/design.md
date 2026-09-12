## Context

The repository is a modular .NET monolith with explicit module composition and shared persistence infrastructure. `ADR-001` currently describes an `IdentityModule` boundary, while the approved story decision selects a dedicated `AuthModule`; the proposal therefore includes an ADR amendment. Profiling remains the source of truth for user, role, claim, access-profile, and preference data.

## Goals / Non-Goals

**Goals:**

- Define a dedicated authentication boundary while keeping profile ownership explicit.
- Make registration atomic for core records and safe under retries and concurrency.
- Make bearer-token sessions work across multiple containers through shared database state and signing configuration.
- Make consent, bootstrap continuation, security events, and asynchronous email failure testable and recoverable.

**Non-Goals:**

- Social login, MFA enrollment, enterprise provisioning, or billing checkout.
- Browser-local token persistence.
- Making email delivery part of the account transaction.

## Decisions

1. **Module boundary:** Create `AuthModule` for authentication, registration orchestration, token/session runtime, and auth HTTP concerns. Integrate with `ProfilingModule` through intention-revealing application contracts rather than importing profile persistence into auth internals. This follows the approved product decision; amend `ADR-001` to replace the prior IdentityModule naming while retaining its ownership split.

2. **Session model:** Use short-lived bearer access tokens plus refresh tokens. Store only hashed refresh-token/session material and lifecycle state in the shared database, including subject, expiry, rotation/revocation state, and audit metadata. Keep signing keys and validation configuration shared across instances through deployment configuration or a secret provider.

3. **Transaction and side effects:** Commit user, identity, consent, Profiling-owned preference and entitlement state, and initial refresh-session state in one application-owned transaction. The coordinating application workflow uses narrow module integration contracts; no module reaches another module's repository or `DbContext`. Publish verification, welcome, and analytics work through retryable asynchronous jobs after commit; side-effect failure produces a degraded state rather than rollback.

4. **Consent model:** Represent Terms, Privacy, and Marketing as separate consent records with server-resolved version identities. Do not trust client-supplied document versions or store full legal document text in account events.

5. **Bootstrap contract:** Return one versioned response shape sufficient for frontend routing and partial-success rendering. Route calculation is allowlisted and falls back safely when selected template or source context is unavailable.

6. **Error and abuse boundaries:** Map validation, uniqueness, throttling, operational degradation, and delayed verification to stable codes. Apply rate limits before expensive password hashing and audit security-relevant outcomes without secrets.

## Risks / Trade-offs

- **[Risk]** Refresh-token rotation and replay handling are security-sensitive. → **Mitigation:** hash stored tokens, rotate on use, revoke the token family on replay, add security-focused tests, and require `$backend-security-coder` review.
- **[Risk]** Shared signing-key configuration can drift between containers. → **Mitigation:** define deployment configuration requirements, key rotation overlap, and startup validation in the implementation design.
- **[Risk]** Cross-module account creation can create coupling. → **Mitigation:** keep orchestration in the application layer and expose small Profiling contracts; review composition with `$architect-review`.
- **[Risk]** Queue failure can leave verification delayed. → **Mitigation:** durable retry/outbox-compatible job handling, explicit response state, and resend endpoint.
- **[Risk]** Existing repository conventions may not yet provide all auth persistence primitives. → **Mitigation:** perform a baseline architecture/persistence inventory before coding and keep schema changes in one coordinating lane.

## Migration Plan

1. Amend `ADR-001` and document the approved AuthModule/ProfilingModule boundary.
2. Add schema and configuration with backward-compatible startup behavior where possible.
3. Deploy persistence and queue consumers before enabling registration traffic.
4. Enable registration after focused unit, integration, concurrency, and security checks pass.
5. Roll back by disabling registration and retaining committed account/session data for controlled recovery; do not silently invalidate active sessions without an explicit security decision.

## Implementation Decisions

- Access tokens live for 15 minutes; refresh sessions live for 30 days and rotate on every successful refresh. A reused or rotated refresh token revokes its entire family. There is no grace window because the shared database is authoritative.
- Signing uses HMAC-SHA256 with `Auth:SigningKey`, validated at startup by the Auth composition extension. Deployments must provide the same key to every instance and rotate by overlapping configuration during rollout.
- Side effects use an application-owned durable outbox table. Registration commits the outbox rows with the account transaction; a hosted dispatcher can retry pending verification and welcome messages. The resend endpoint creates a new idempotent outbox request.
- Retry safety uses an optional client idempotency key plus the normalized-email unique constraint. A matching key returns the stored result; a different key for a claimed email returns `AUTH_EMAIL_IN_USE`.
- Entitlements are owned by ProfilingModule. `UserEntitlement` is one subscription/access-profile grant and references `User`, `BillingSubscription`, and `AccessProfile`; the source `BillingPlan` is obtained through the subscription and is not duplicated. Multiple active subscriptions create multiple entitlement rows, while effective capabilities are the union of each profile's seeded roles. The application resolver consumes a BillingModule subscription snapshot and a ProfilingModule access-shape snapshot; it does not query either module's persistence directly.
- Stable capability identifiers are persisted on the ProfilingModule `Role` setup entity and populated by deterministic, idempotent seeds; the resolver reads the seeded role capability values through access-profile role relations rather than maintaining a service-owned capability list. `UserPreference` is also a ProfilingModule entity.
- `AuthenticationIdentity.User` remains a permitted Auth-DM to Profiling-DM business relationship under ADR-002 Rule 1. The stricter integration rule applies to Auth SL/PL query and orchestration code: those layers receive primitive snapshots and never inject another module's repository or `AppDbContext`.
- Registration stages the Profiling user, Billing starter account/subscription, Profiling preference, and Profiling entitlement on the shared unit of work; the Auth repository owns the application transaction commit for the complete synchronous baseline.
