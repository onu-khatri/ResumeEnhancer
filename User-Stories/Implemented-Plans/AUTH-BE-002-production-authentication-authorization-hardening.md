---
kind: implemented-plan-archive
status: Archived
userStory: AUTH-BE-002
userStoryTitle: Production Authentication And Authorization Hardening
userStoryReference: User-Stories/3.3 authentication-authorization-hardening.US.md
githubIssue: 36
githubIssueTitle: Production Authentication And Authorization Hardening
githubRepository: onu-khatri/ResumeEnhancer
parentIssue: AUTH-BE-001
relatedIssues: []
branch: openspec/gh-36-authentication-authorization-hardening
worktree: .worktrees/gh-36-authentication-authorization-hardening
pullRequest: null
commits: []
mergeCommit: null
release: null
change: gh-36-authentication-authorization-hardening
planCreatedAt: 2026-09-15
implementationCompletedAt: 2026-09-20
archivedAt: 2026-09-20
planner: implementation-planner
reviewers:
  - user
  - implementation-plan reviewers recorded in source plans
relatedADRs:
  - KnowledgeBase/ADRs/ADR-006-resilience-policy-ownership-and-failure-semantics.md
knowledgeReferences:
  - KnowledgeBase/INDEX.md
sourcePlans:
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/2.3-auth-durable-state-provider-evidence.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/5.5-auth-timeprovider-migration.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-account-lifecycle-abuse-errors-csrf-5-1-5-4.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-auth-acceptance-blocker-follow-up.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-auth-contracts-profiling-ports.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-authentication-authorization-boundary-4-1-4-4.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-carson-corrective-3-1-3-3.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-corrective-remaining-5-1-5-4-parfit.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-durable-authentication-state-corrective-hypatia.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-durable-authentication-state.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-final-corrective-5-1-5-4-carver.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-final-corrective-5-1-5-4-peirce.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-final-corrective-5-1-5-4-ramanujan.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-final-gap-closure-3-1-3-3.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-final-revised-3-1-3-3.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-harvey-corrective-jwt-browser-session.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-helmholtz-corrective-2-1-2-3.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-huygens-corrective-jwt-browser-session.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-jwt-browser-session-security.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-rawls-corrective-auth-state-challenges.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-review-remediation-1-1-1-3.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-revised-account-lifecycle-abuse-errors-csrf-5-1-5-4.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-revised-core-resilience-5-1-5-4.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-revised-resilience-corrective-5-1-5-4.md
  - .tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/batch-verification-regression-6-1-6-3.md
---

# AUTH-BE-002 — Consolidated implementation plan history

## Scope and source-plan inventory

This archive consolidates the 25 implementation-plan artifacts created in the
canonical worktree for OpenSpec change
`gh-36-authentication-authorization-hardening`. The source files remain in
`.tmp/ImplementationPlans/gh-36-authentication-authorization-hardening/` and
are ordered by their declared `created_at`, then `plan_version`, then path.
They include initial plans, corrective plans, reviewer-driven revisions,
approval records, and verification planning.

The source plans are the detailed evidence for each batch. This document is
the permanent lifecycle summary and does not replace those source artifacts.

## Planning timeline

- **2026-09-15 — Initial durable-state and contract plans:** plans for Auth
  contracts, Profiling integration ports, durable lockout/challenge/session
  state, setup-data purpose references, and migration/schema constraints were
  created and approved.
- **2026-09-15 to 2026-09-16 — Corrective 2.x and 3.x planning:** reviewer
  findings drove revisions for concurrency, challenge consumption, key
  ownership, rotation overlap, invalidation, refresh transport, and durable
  validation. The revised batches were approved before implementation.
- **2026-09-18 — 4.x boundary planning:** authentication ordering, fail-closed
  authorization, guest access-profile/role metadata, principal-derived
  identity, and endpoint inventory were approved as one serialized boundary.
- **2026-09-19 — 5.1–5.4 corrective planning:** lifecycle behavior, distributed
  limiting and db-cache fallback, redacted errors, auditing, trusted origins,
  CSRF, resilience ownership, retry configuration, and no-new-schema limits
  were reconciled through successive approved plans.
- **2026-09-20 — 5.5 and 6.x planning:** Auth clock migration to
  `System.TimeProvider`/`FakeTimeProvider` and the final verification/regression
  batch were approved. Final implementation evidence was recorded in the
  worktree OpenSpec tasks.

## Requirements and originally planned approach

The story required backend authentication and authorization hardening while
preserving existing Auth endpoint purposes and excluding frontend work. The
planned solution covered:

- explicit Auth contracts and module-safe Profiling integration ports;
- durable lockout, password-history, challenge, refresh-session, and signing
  key state with EF configuration and migration evidence;
- RS256 JWT issuance/validation through `JsonWebTokenHandler`, configured
  issuer/audience/claims/lifetime/skew, protected application-owned RSA key
  material, rotation overlap, and invalidation;
- secure bearer access-token and refresh-cookie transport with CSRF and
  trusted-origin controls;
- fail-closed WebSolution authorization, explicit anonymous boundaries,
  guest access-profile/role evaluation, and principal-derived identity;
- account lifecycle operations, enumeration-safe errors, distributed abuse
  limits with configurable resilience, structured redacted auditing, and
  `TimeProvider`-controlled time behavior;
- focused unit/integration regression coverage and backend-scoped validation.

## Review feedback and revisions

The plans were not blindly concatenated. Repeated task scopes were carried
forward once, while material corrections were retained: setup-table foreign
key semantics, atomic challenge and lockout transitions, durable key-provider
authority, actual rotation scheduling and overlap, sync/async validation
consistency, cookie/body compatibility, centralized audit ownership,
provider-neutral limiter contracts, db-cache fallback, configurable retry and
jitter profiles, trusted-origin matching, CSRF rejection, injected time,
single-migration boundaries, and backend-only client-SDK exclusion.

Each corrective plan records its own reviewer/approver and reason. The final
source plan for a task supersedes earlier drafts only for implementation
direction; earlier drafts remain part of this historical record.

## Per-plan decision and evidence ledger

The following ledger preserves the context that a filename-only inventory
would lose. `Superseded` means the plan remains valid historical evidence but
is no longer the final implementation direction for that scope.

### 2026-09-15 — contracts, durable state, and acceptance evidence

1. **`batch-auth-contracts-profiling-ports.md` — Implemented.** Covered 1.1
   and 1.3. Established typed Auth contracts, normalization and redaction
   validation, immutable Profiling security snapshots, Guest profile/role
   resolution, and the dependency direction from Auth SL to Profiling SL
   integrations without direct Profiling persistence coupling.
2. **`batch-review-remediation-1-1-1-3.md` — Implemented.** Corrected the
   missing/obsolete Guest behavior, real SQLite projection evidence, validator
   registration/error matrices, oversized input cases, and the task-1.2
   evidence-only boundary. It intentionally did not authorize unrelated
   package, migration, story, or workflow changes.
3. **`batch-durable-authentication-state.md` — Superseded.** Introduced the
   durable Auth model direction: setup-table FK for `AuthChallengePurpose`,
   appsettings-backed lockout/progressive-delay policy, shared persistence
   ownership, and a dedicated migration. Later review plans refined its
   concurrency and timestamp details.
4. **`batch-durable-authentication-state-corrective-hypatia.md` — Implemented.**
   Added expiry-window reset, atomic failed-login transitions, active-purpose
   conditional challenge consumption, rollback proof, and evidence-based
   `RefreshSession.CreatedAtUtc` backfill compatibility.
5. **`batch-helmholtz-corrective-2-1-2-3.md` — Implemented.** Closed
   provider-specific challenge ordering, deactivation-versus-consumption,
   rollback, service-level timestamp compatibility, and staged-worktree scope
   reconciliation without changing the approved persistence boundary.
6. **`batch-auth-acceptance-blocker-follow-up.md` — Implemented.** Bounded
   SQLite evidence honestly, documented what it does not prove about SQL Server
   locking/isolation, preserved conditional repository predicates, and removed
   only the explicitly identified EOF artifacts.
7. **`2.3-auth-durable-state-provider-evidence.md` — Implemented.** Added the
   final task-2.3 evidence note distinguishing deterministic SQLite commit-order
   proof from unperformed SQL Server lock/deadlock claims.

### 2026-09-15 to 2026-09-18 — JWT, keys, and browser transport

8. **`batch-jwt-browser-session-security.md` — Superseded.** Established the
   original 3.1–3.3 direction: `JsonWebTokenHandler`, RS256, issuer/audience,
   canonical subject/session claims, 30-minute lifetime, 30-second skew,
   protected RSA material, 180-day rotation, 48-hour overlap, secure refresh
   cookies, CSRF, replay detection, and preserved endpoint purposes.
9. **`batch-huygens-corrective-jwt-browser-session.md` — Superseded.** Added
   one validated `AuthSecurityOptions` source, durable invalidation across
   fresh instances, explicit legacy-body/browser-cookie classification, and
   safe failure behavior for key/configuration dependencies.
10. **`batch-harvey-corrective-jwt-browser-session.md` — Superseded.** Added
    application-owned protected key-provider requirements, actual rotation
    scheduling, startup usability validation, authoritative sync/async
    validation, and transport compatibility evidence.
11. **`batch-final-gap-closure-3-1-3-3.md` — Superseded.** Bounded the
    remaining 3.x gap closure to migration safety, active/previous-key
    invariants, refresh transport, and no endpoint redesign.
12. **`batch-final-revised-3-1-3-3.md` — Superseded.** Refined the final
    3.x implementation ownership and evidence boundary, including protected
    key references, durable lifecycle state, and compatibility decisions.
13. **`batch-carson-corrective-3-1-3-3.md` — Implemented.** Closed migration
    transition safety, active-key predicate consistency, production fallback
    removal, RSA disposal, provider-backed rotation/invalidation tests, and
    hosted transport edge cases.

### 2026-09-18 — authentication and authorization boundary

14. **`batch-authentication-authorization-boundary-4-1-4-4.md` — Implemented.**
    Established authentication-before-authorization, fail-closed fallback,
    explicit `.AllowAnonymous()` plus guest metadata, principal-derived user
    and audit identity, protected ownership/policy routes, and the anonymous
    route inventory. Missing Guest profile/role or dependency failure fails
    closed.

### 2026-09-19 — lifecycle, limiting, errors, CSRF, and resilience

15. **`batch-account-lifecycle-abuse-errors-csrf-5-1-5-4.md` — Superseded.**
    Captured the initial approved decisions for account lifecycle, provider-
    neutral atomic limiting, db-cache fallback, fail-open limiter outages,
    redacted errors, audit tolerance, and environment-specific trusted origins.
16. **`batch-revised-account-lifecycle-abuse-errors-csrf-5-1-5-4.md` —
    Superseded.** Added explicit `User.IsDeleted`, database limiter schema and
    rollback expectations, provider adapters, audit persistence, and the
    no-new-schema stop rule.
17. **`batch-corrective-remaining-5-1-5-4-parfit.md` — Superseded.** Closed
    provider selection/startup rejection, TTL/concurrency behavior, durable
    audit retry, stable correlation-linked ProblemDetails, and denial-event
    coverage.
18. **`batch-revised-resilience-corrective-5-1-5-4.md` — Superseded.** Defined
    bounded retry/timeout/circuit/jitter profiles, idempotency exclusions,
    fail-open/fail-tolerated semantics, and configurable values without hard-
    coding provider behavior.
19. **`batch-revised-core-resilience-5-1-5-4.md` — Superseded.** Moved the
    reusable resilience wrapper and policy ownership to Core, fixed the package
    target to `Microsoft.Extensions.Resilience` 10.1.0, excluded unsupported
    HTTP resilience, and required limiter retry-loop replacement.
20. **`batch-final-corrective-5-1-5-4-ramanujan.md` — Superseded.** Added
    outbox acknowledgement/idempotency, shared limiter coverage, lifecycle
    enforcement, centralized redacted errors, degradation auditing, and the
    strict migration stop rule.
21. **`batch-final-corrective-5-1-5-4-peirce.md` — Superseded.** Reconciled
    centralized audit ownership, correlated denials, existing-fields-only
    deduplication, and injected `TimeProvider` retry-after calculations.
22. **`batch-final-corrective-5-1-5-4-carver.md` — Implemented.** Finalized
    correlation linkage, concurrency-safe existing-field deduplication,
    centralized side-effect auditing, injected retry timing, and the no-new-
    schema boundary for the delivered 5.x behavior.

### 2026-09-20 — time and verification

23. **`5.5-auth-timeprovider-migration.md` — Implemented.** Replaced obsolete
    Auth clock APIs with one registered `System.TimeProvider`, used the official
    `FakeTimeProvider` test seam, preserved UTC contracts, avoided sleeps, and
    explicitly excluded inherited response-shape failures from task 5.5.
24. **`batch-rawls-corrective-auth-state-challenges.md` — Implemented.** Closed
    current-state re-evaluation, challenge-delivery secrecy, outbox protection,
    dependency-failure behavior, trusted-origin evidence, and package-warning
    ownership without claiming an unrelated dependency fix.
25. **`batch-verification-regression-6-1-6-3.md` — Implemented.** Defined the
    serialized unit/integration verification owner, exact test totals,
    clean-build versus `--no-build` evidence distinction, OpenSpec validation,
    acceptance review, and no-frontend-change check.

## Final decision register

| Decision | Introduced/refined by | Delivered evidence |
| --- | --- | --- |
| Auth challenge purpose is a setup-table FK; semantic identity is stable `Code` | durable-state plans and Hypatia/Rawls corrections | `AuthChallengePurpose`, EF configuration, migration, persistence tests |
| Lockout threshold/window/progressive delay are appsettings-configurable | durable-state plans | `AuthSecurityOptions`, lockout services, time-controlled tests |
| RS256 only through `JsonWebTokenHandler`; issuer/audience/subject/session/time claims are validated | JWT baseline and Carson corrections | Auth security services, bearer handler, token tests |
| RSA private material is application-owned/protected; rotation is 180 days with 48-hour overlap | Huygens/Harvey/Carson plans | key provider/state service, migration, rotation/invalidation tests |
| Browser access is bearer-based; refresh is secure cookie transport with CSRF/origin checks | JWT and 5.x plans | browser transport, Minimal APIs, HTTP integration tests |
| Anonymous access is explicit and guest role/profile-aware; fallback is fail closed | 4.x boundary plan | endpoint metadata/middleware, startup validation, guest matrix tests |
| Identity and audit actor derive from the principal, never client headers | 4.x boundary plan | principal extensions, endpoint and ownership tests |
| Limiting is provider-neutral, atomic, configurable, and has db-cache fallback | 5.x lifecycle/resilience plans | Core contracts, Redis/MemCache/DB stores, limiter migration/tests |
| Limiter outage is fail open but observable; tolerated audit failure does not undo business state | Parfit/Ramanujan/Peirce/Carver plans | audit signal/outbox paths and failure tests |
| Resilience wrapper belongs in Core and uses version 10.1.0 | Chandrasekhar/Core resilience plan | Core wrapper/options and consumer project references |
| Auth time uses `TimeProvider`; tests use `FakeTimeProvider` | Kant/5.5 plan | composition registration and deterministic tests |
| One consolidated migration per implementation commit | migration corrections and repository policy | `20260920151219_ConsolidatedAuthenticationAuthorizationHardening` |

## Implementation evidence map

- **Contracts and identity:** AuthModule AM/SL contracts, Profiling integration
  ports, `PrincipalIdentityExtensions`, and contract/identity tests.
- **Durable Auth state:** Auth DM entities, Auth PL configurations/repository,
  `AppDbContext`, setup data, and the consolidated migration/designer/snapshot.
- **JWT and session security:** `AuthSecurityServices`, key provider/state
  services, `AuthTokenAuthenticationHandler`, browser transport, Minimal APIs,
  and Auth infrastructure/durable-state tests.
- **Authorization boundary:** WebSolution authorization metadata, middleware,
  startup validators, endpoint composition, and guest/ownership integration
  coverage.
- **Lifecycle and abuse controls:** Registration/Auth handlers and services,
  profiling authorization adapter, limiter stores, db-cache persistence,
  resilience Core wrapper, audit recorder/signal, and account-state tests.
- **Verification:** Auth unit tests, WebLibrary tests, Auth HTTP integration,
  full backend integration fixtures, and the task 6.3 acceptance evidence.

The source plans remain the detailed design evidence; this map identifies the
code/test ownership seams so a future agent can recover context quickly.

## Approval record

The source plans contain explicit approval metadata and user approvals for the
covered batches. No approval is inferred from OpenSpec validation. The final
implementation was carried out in the canonical branch worktree after those
plans were approved.

## Implementation changes

The completed worktree includes Auth contracts, durable persistence and one
consolidated authentication/authorization migration, Core-owned resilience
wrappers, db-cache limiter state, Auth lifecycle and security services,
RS256/key lifecycle and refresh-cookie transport, WebSolution authorization
middleware/metadata, redacted errors/auditing, trusted-origin/CSRF behavior,
TimeProvider migration, and focused unit/integration test coverage.

No frontend files were changed for this backend story. The unrelated client
project `Microsoft.VisualStudio.JavaScript.Sdk`/NuGet.Config limitation is
explicitly excluded from backend acceptance evidence.

## Originally planned vs actually implemented

The implementation follows the approved final direction. The important
implementation discoveries and deviations were handled by corrective plans:

| Area | Originally planned | Implemented result | Reason/evidence |
| --- | --- | --- | --- |
| Resilience | Consumer-specific resilience use | Reusable Core-owned wrapper with configured profiles and provider-specific limiter adapters | Shared behavior and retry-loop coverage required one ownership boundary. |
| Limiting | Distributed provider abstraction | Redis/MemCache paths plus database-backed db-cache fallback, with explicit outage behavior | Deployment environments may not provide Redis; durable fallback preserves behavior. |
| Auth time | Existing clock abstraction | `System.TimeProvider` in production and `FakeTimeProvider` in tests | Deterministic time-sensitive lifecycle and security tests. |
| Migration | Multiple schema-related changes during iteration | One consolidated migration for the implementation commit | Repository migration policy and review findings required one migration boundary. |
| Client build | Full solution build | Backend-scoped build/test evidence with client SDK limitation recorded | Frontend was out of scope and the unrelated SDK/NuGet environment issue was isolated. |

No unrecorded material deviation is claimed. Any future change to the approved
behavior requires a new plan or corrective plan rather than silently rewriting
this history.

## Final implemented plan

1. Preserve module boundaries and existing endpoint purposes while adding the
   missing Auth contracts and Profiling projections.
2. Persist and transact account security state, challenge purposes, session
   families, and key metadata through the existing persistence composition.
3. Issue and validate RS256 JWTs with configured claims and secure application-
   owned key lifecycle behavior; keep private material out of persistence,
   logs, responses, and source control.
4. Use bearer access tokens and secure refresh cookies, rejecting unsafe
   cookie-authenticated mutations without trusted-origin and CSRF proof.
5. Enforce authentication before authorization, fail closed by default, map
   anonymous guest access through endpoint metadata, and derive identity from
   the authenticated principal.
6. Implement account lifecycle, lockout, password history, challenges,
   distributed limiting, redacted errors, structured audit signals, and
   configurable resilience with deterministic time.
7. Validate the result with focused unit tests, backend integration tests,
   Auth production integration tests, backend build evidence, OpenSpec strict
   validation, and acceptance-criteria review.

## Validation and review evidence

- OpenSpec tasks: 1.1 through 6.3 checked in the canonical worktree.
- Backend server build with `-p:SkipClientProjectReference=true`: 0 warnings,
  0 errors.
- Unit tests: 382/382 passed.
- Full backend integration tests: 58/58 passed.
- Production Auth integration tests: 39/39 passed.
- OpenSpec strict validation: `Change 'gh-36-authentication-authorization-hardening' is valid`.
- Acceptance evidence includes RS256, current-state, challenge delivery,
  replay/expiry, CSRF, CORS, raw-secret, ownership, rate-limit, account-state,
  and regression coverage.
- No frontend files were changed for this backend-only story.

These are worktree/local evidence. No hosted PR, CI run, merge commit, or
release is claimed because no such identifiers were present during archival.

## Traceability

`AUTH-BE-002` → GitHub issue `onu-khatri/ResumeEnhancer#36` → OpenSpec change
`gh-36-authentication-authorization-hardening` → branch/worktree
`openspec/gh-36-authentication-authorization-hardening` /
`.worktrees/gh-36-authentication-authorization-hardening` → source plans →
implementation and tests → this archive.

Relevant architectural guidance includes
`KnowledgeBase/ADRs/ADR-006-resilience-policy-ownership-and-failure-semantics.md`
and the routed `KnowledgeBase/INDEX.md` authorities. Commit, PR, CI, merge,
and release links remain unset until verified from Git/hosted evidence.

## Residual risks and follow-up

- Hosted PR/CI and merge state still require separate delivery verification.
- The story source status remains a delivery-tracking concern and must only be
  updated by the delivery closeout workflow after its corresponding evidence.
- Temporary source plans are intentionally retained for auditability.

## Archive result

The permanent consolidated plan archive is valid and readable. All 25 source
plans are accounted for, no source plan was deleted or moved, and this archive
is the historical record of the implementation lifecycle.
