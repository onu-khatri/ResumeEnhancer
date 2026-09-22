---
kind: implementation-plan
status: Proposed
scope: batch
change: gh-36-authentication-authorization-hardening
tasks:
  - "[x] 2.1 Add persistence models/configuration for lockout state, password history, reset challenges, verification challenges, and required session/key metadata; verify EF model constraints, protected-hash storage, indexes, and unit persistence tests."
  - "[x] 2.3 Implement transactional password history, challenge consumption, session-family revocation, and five-failure/15-minute lockout with progressive delay; verify concurrency, replay, expiry, and previous-two-password tests."
  - "[x] 3.3 Preserve refresh rotation/replay detection while moving browser refresh transport to `HttpOnly`, `Secure`, `SameSite` cookies and access transport to `Authorization`; verify registration, login, refresh, logout, revocation, and concurrent-use integration tests."
  - "[x] 4.1 Configure authentication before authorization and a fail-closed fallback policy in WebSolution.Server; verify protected endpoints return `401`, authorized endpoints establish `HttpContext.User`, and existing approved anonymous contracts remain reachable."
  - "[x] 4.3 Replace authoritative `X-User-Id` and `X-Audit-UserId` reads with principal-derived identity and enforce roles, capabilities, policies, entitlements, and resource ownership; verify forged-header, `401`, `403`, and ownership integration tests."
  - "[x] 5.1 Implement login, password change, forgot/reset, verification completion, and authenticated-me routes with enumeration-safe behavior and account-state enforcement; verify active, unverified, disabled, deleted, locked, unknown, and invalid-credential scenarios."
  - "[x] 6.1 Add focused AuthModule unit tests for token validation, key rotation, password history, lockout, challenge expiry/reuse, redaction, metadata, and identity derivation; verify `dotnet test test\\ResumeEnhancer.Tests\\ResumeEnhancer.Tests.Unit.csproj --no-restore` passes for the affected tests."
  - "[ ] 6.2 Add Auth, WebSolution, guest-access, ownership, cookie/CSRF, refresh replay, rate-limit, and account-state integration coverage; verify `dotnet test test\\IntegrationTest\\ResumeEnhancer.Tests.Integration.csproj --no-restore` passes for the affected tests. Rawls identified missing account-state transition, verification-challenge creation, and reset-outbox delivery coverage."
  - "[ ] 6.3 Run `dotnet build application\\ResumeEnhancerApp.slnx`, OpenSpec validation, and acceptance-criteria review; verify existing registration/bootstrap/refresh/logout/verification regression tests remain green and no frontend files changed. Acceptance review is blocked by three implementation defects reported by Rawls."
created_at: 2026-09-20
plan_version: 2
---

# Proposed corrective implementation plan: Rawls authentication-state and challenge-delivery blockers

## Plan metadata and gate state

- Exact OpenSpec change: `gh-36-authentication-authorization-hardening`.
- Exact covered task identities are reproduced verbatim in the frontmatter. The current canonical task artifact marks 6.2 and 6.3 `[ ]`, with Rawls’ missing-coverage and blocked-acceptance notes; this plan does not alter those checkboxes. The plan is a corrective evidence/implementation batch for residual review blockers inside their approved behavior, not a new product capability.
- Plan scope: one serialized backend batch covering current account/session-state enforcement, verification-challenge delivery, reset-challenge outbox secrecy, and focused regression coverage. The three behavior fixes share Auth SL/PL/Web contracts and test fixtures; splitting them would create inconsistent challenge/session semantics.
- Current phase: OpenSpec artifacts and a substantial staged implementation exist in the canonical worktree, but the requested review blockers are not proven closed. This plan is Proposed and is not an implementation authorization.
- Canonical worktree: `D:/RND/ResumeEnhancer/.worktrees/gh-36-authentication-authorization-hardening`.
- Expected branch: `openspec/gh-36-authentication-authorization-hardening`.
- Intended implementation owner after approval: exactly one `backend-implementer`, with a read-only security/code review lane after implementation. Shared contracts, persistence seams, composition, and test-host changes remain serialized under that owner.
- Planner boundary: this turn writes only this Proposed plan. It does not edit production code, tests, migrations, configuration, OpenSpec artifacts, task checkboxes, commits, branches, or hosted state.

## Goal and success criteria

Close the three Rawls review blockers without changing the approved Auth endpoint purposes or introducing a frontend scope:

1. A refresh request and every protected bearer request re-evaluate current durable Auth and Profiling state. Verified, disabled/deactivated, deleted, locked, expired, and revoked cases cannot continue through a previously issued access or refresh session. A current account-state dependency failure fails closed with a safe response.
2. Registration and verification resend create a fresh purpose-backed email-verification challenge, persist only its hash, and deliver the one-time material through the existing outbox/side-effect seam. The raw challenge is never persisted in an Auth entity, outbox payload, audit record, log, response, or test fixture assertion.
3. Password-reset delivery remains enumeration-safe and outbox-backed. The database stores only the challenge hash; the outbox stores only a protected/encrypted delivery envelope or an approved equivalent that can be unprotected only by the delivery boundary. Raw challenge material is not present in durable JSON or diagnostics.
4. Unit and HTTP integration regression coverage proves success, invalid/expired/reused/no-op, state denial, persistence, mapping, outbox delivery, dependency failure, and existing registration/session compatibility paths.
5. The `System.Security.Cryptography.Xml`/NU1903 warning is traced to its actual direct or transitive package owner. If it is outside this change’s owned dependency graph, the plan records a bounded follow-up owner/command rather than opportunistically changing package versions. Wildcard trusted-origin behavior remains a watch item and is not expanded unless current evidence proves violation of the approved requirement.

Completion is evidence-based: focused tests pass with exact totals, the relevant Auth/Web projects build or environment blockers are separately reported, response/outbox/database inspections show no raw challenge, and the OpenSpec change validates without checkbox edits by this planner.

## Current-state evidence

### OpenSpec, story, and requirement evidence

- `openspec/changes/gh-36-authentication-authorization-hardening/proposal.md` requires standards-compliant bearer authentication, durable/revocable lifecycle state, hashed single-use challenges, secure refresh transport, and no frontend implementation.
- `design.md` assigns authentication/session/cookie/request-identity ownership to AuthModule, user/access-profile state to ProfilingModule, durable lifecycle state to Auth persistence, setup-purpose identity to `AuthChallengePurpose`, and safe failure contracts to the application boundary.
- `specs/auth/authentication-authorization-hardening/spec.md` requires active/verified/non-disabled/non-locked account enforcement during login, refresh, and protected access; password-reset challenges expire after 30 minutes; email-verification challenges expire after 24 hours; persisted challenges reference an Auth-owned purpose row; and challenge material is stored only as protected hashes.
- `specs/auth/account-registration-session-bootstrap/spec.md` requires existing registration/bootstrap/session purpose and refresh replay behavior to remain stable.
- `tasks.md` records the earlier broad implementation tasks as checked while the current canonical task artifact leaves 6.2 and 6.3 open because Rawls identified missing integration coverage and blocked acceptance review. This plan preserves exact task identity and scope while requiring corrective evidence; it does not claim that earlier checked boxes prove the requested blockers are closed.
- `.tmp/artifacts/issues/36.md` and `User-Stories/3.3 authentication-authorization-hardening.US.md` establish the backend-only boundary, account-state enforcement during refresh/protected access, and regression expectations for registration, refresh, logout, verification, and recovery.

### Current implementation evidence in the canonical worktree

1. `application/Modules/AuthModule/AuthModuleSL/Services/RegistrationService.cs`
   - `RefreshAsync` finds a refresh session, checks expiry/revocation/rotation, rotates it, and issues a new access token, but does not load the current `IUserLookupService` state, `AuthenticationIdentity` verification/lock state, or a durable session-validity decision before rotation/issuance.
   - `RegisterAsync` and `ResendVerificationAsync` enqueue `verification-email` messages containing `{ UserId, email }` but do not create an `AuthChallenge` or include a delivery token/envelope.
   - The existing refresh path and cookie/body transport must be preserved; the correction belongs behind the existing service and repository seams.
2. `application/Modules/AuthModule/AuthModuleSL/Services/AuthLifecycleService.cs`
   - Login checks Profiling `IsDeactivated`/`IsDeleted`, `EmailVerified`, and `LockedUntilUtc`.
   - Forgot-password creates a hashed `AuthChallenge`, but serializes the raw `challenge` into `password-reset-email` outbox JSON. This directly violates the no-raw-material requirement.
   - Verify/reset consume by purpose code and hash, but current delivery cannot produce a matching raw verification challenge for registration/resend and reset delivery leaks the raw value into durable outbox state.
3. `application/Modules/AuthModule/AuthModuleWeb/Authentication/AuthTokenAuthenticationHandler.cs`
   - The bearer handler calls `ITokenService.ValidateAccessTokenAsync` and immediately establishes a principal. It does not re-check current Auth identity state, refresh-session family/session revocation, Profiling disabled/deleted state, email verification, or lockout before accepting the bearer request.
4. `application/WebSolution/ModulesComposition/Authorization/EndpointAuthorizationMiddleware.cs`
   - Protected metadata checks Profiling authorization snapshots for deactivation/deletion and permissions, but it does not itself prove Auth identity/session state. A valid JWT can therefore reach this seam unless the authentication handler or a dedicated current-state service closes that gap.
5. `application/Modules/AuthModule/AuthModuleSL/Abstractions/IAuthRepository.cs` and `AuthModulePL/Repositories/AuthRepository.cs`
   - Existing seams cover identity/session lookup, purpose resolution, hash-only challenge persistence/consumption, session-family revocation, and outbox claim/ack/failure transitions.
   - They do not expose one atomic, intention-revealing current-session/account-state decision for both refresh and bearer authentication. The implementation must extend this seam rather than query `AppDbContext` from Web or cross into Profiling persistence.
6. `application/Modules/AuthModule/AuthModuleDM/Entities/AuthChallenge.cs`, `AuthChallengePurpose.cs`, and `AuthModulePL/Seeding/AuthModuleSeeder.cs`
   - `AuthChallenge` stores `TokenHash` and `AuthChallengePurposeId`, with no `PurposeCode` or raw-token property. This is the correct durable model to preserve.
7. `application/Modules/AuthModule/AuthModuleWeb/Outbox/AuthOutboxDispatcher.cs`
   - `AuthSideEffectPayload` currently contains only `UserId` and `Email`; the dispatcher passes those fields to `IAuthSideEffectHandler`. It has no challenge delivery envelope/protection step, so adding challenge delivery requires an explicit payload contract and unprotect boundary.
8. `application/Modules/AuthModule/AuthModulePL/SecurityServices.cs`, `IAuthProtectedKeyMaterialStore`, `AuthSecurityOptions`, and `DataProtection` composition
   - The repository already uses ASP.NET Core Data Protection for protected RSA signing-key material, configured through `DataProtectionKeyRingPath`, application name, and purpose. This is the approved existing protection boundary candidate for an outbox challenge envelope, but a separate purpose string and delivery-only unprotect location are required; RSA signing-key purpose/material APIs must not be reused by accident.
9. Existing tests:
   - `AuthDurableStateTests` prove generic hash-only challenge persistence, purpose FK, expiry, single use, and transaction rollback.
   - `AuthRegistrationServiceTests` prove registration/resend/outbox existence and refresh rotation/replay, but they do not assert that registration/resend creates a verification challenge or that outbox payloads contain only protected delivery material.
   - `AuthHttpIntegrationTests` prove cookie attributes, refresh replay, forged-header rejection, and CSRF/origin cases, but do not change account state after token issuance and then exercise refresh/protected bearer access.
   - No current test proves that raw reset/verification challenge material is absent from persisted `AuthOutboxMessage.PayloadJson`, audit rows, logs, or the delivered handler input.
10. Dependency evidence:
   - Auth SL directly references `Microsoft.IdentityModel.JsonWebTokens` and `Microsoft.IdentityModel.Tokens`; no direct `System.Security.Cryptography.Xml` package reference appears in the inspected Auth projects or repository project files. NU1903 ownership is therefore unconfirmed and must be assessed with transitive package inspection before any remediation.

## Development-entry readiness reasoning

Gate: **BLOCKED for implementation; PASS for corrective planning**.

Outcome: close current-account-state and challenge-delivery security blockers in the existing auth hardening behavior.

Shape: **backend**, crossing Auth SL/PL/Web, Profiling SL integration, host authentication composition, shared Data Protection configuration, outbox delivery, and unit/integration tests.

Primary workflow: `$workflow-planning` for this Proposed plan, followed by the repository’s approved OpenSpec workflow after explicit user approval.

Specialist route after approval: `$backend-dotnet-patterns`, `$backend-feature-development`, `$backend-ef-core` only if schema/model changes are proven necessary, `$backend-security`/`$security-management`, `$backend-dotnet-architecture` for the Auth/Profiling seam, and `$quality-production-code-review` as read-only review. `$research-deep` was used as a local evidence method for the cross-cutting defect boundary; no external source was needed.

Affected ownership: Auth AM/DM/SL/PL/Web; `ModulesComposition` authentication/authorization; Profiling SL integration snapshot; shared Data Protection registration/options; Auth outbox; Auth unit and HTTP integration tests. No frontend ownership.

Confirmed decisions: preserve endpoint purposes and existing browser/body compatibility tests; keep purpose-backed setup rows; persist challenge hashes only; use one current-state decision seam for refresh and bearer; keep wildcard trusted origins outside scope unless evidence proves a requirement violation.

Blocking decisions before implementation: exact challenge-delivery envelope contract, whether the existing Data Protection key ring is approved for delivery protection in all environments, the canonical current-state failure mapping (`401` versus `403` by boundary), and the NU1903 remediation owner if the warning is transitive and actionable. These are listed for approval below; they are not guessed here.

Verification: focused Auth unit/integration tests, package vulnerability/dependency inspection, project builds, full relevant test commands, OpenSpec validation, and a scoped diff/evidence review.

Next action: obtain explicit approval for this exact Proposed plan and its unresolved decisions, then hand it to one backend implementation owner. No implementation starts from this plan while it remains Proposed.

## Scope, exclusions, assumptions, dependencies, and risks

### Included

- One current-state authorization/read seam that can be called from refresh and bearer authentication without exposing Profiling persistence to Auth PL/SL.
- Refresh denial before rotation/issuance for unverified, disabled/deactivated, deleted, locked, missing, expired, or revoked current state; bearer denial before protected endpoint execution for the same applicable state and invalid/revoked session.
- Registration and resend challenge generation, hash persistence, purpose resolution by stable code, and outbox delivery payload changes.
- Password-reset outbox secrecy using a dedicated protected delivery envelope at the existing Data Protection boundary, or an explicitly approved repository-native equivalent if evidence shows Data Protection cannot satisfy the deployment contract.
- Focused unit/integration regression coverage and bounded NU1903 assessment/follow-up.

### Excluded

- Frontend changes, new login/recovery product behavior, social login, SSO, MFA, provider integration, email-template redesign, or changes to public endpoint purposes.
- Wildcard trusted-origin remediation unless current repository/spec evidence demonstrates that the existing wildcard behavior violates the approved requirement. It remains a watch item with a validation command.
- New migrations or schema changes unless implementation inspection proves the current entities/indexes cannot support the state/challenge behavior. Any necessary migration is a replanning trigger because this plan is intended to use the existing model.
- OpenSpec task-checkbox edits, task status changes, plan approval metadata, commits, pushes, or hosted issue/PR updates in the planning turn.

### Assumptions to validate

- A bearer token’s `sid` maps to the current `RefreshSession.SessionKey`/session family and can be checked efficiently through a narrow Auth repository/service method. If the current model cannot support this without a schema change, stop for replanning.
- “Locked” is the Auth identity `LockedUntilUtc > now`; “disabled/deactivated” and “deleted” are Profiling state snapshot values; “verified” is `AuthenticationIdentity.EmailVerified`.
- Current account-state failure responses remain enumeration-safe. Refresh and bearer may both return `401` at the authentication boundary; protected authorization failures remain `403` where the established middleware contract requires it. The exact split requires approval and test evidence.
- The outbox delivery handler is the only component allowed to unprotect challenge delivery material. Logging side effects and persistence inspection must see only safe identifiers or protected ciphertext.
- The existing Data Protection key ring is durable and shared across application instances in the target deployment. If it is not, the plan must stop rather than silently use process-local protection.

### Dependencies

1. Existing task-1/2 contracts, durable identity/session/challenge/purpose entities, and current Auth/Profiling integration ports.
2. Existing task-3 JWT/cookie transport and task-4 principal/authorization composition; this plan must not create a second authentication scheme.
3. Existing `AuthOutboxDispatcher` claim/lease/ack/failure lifecycle and `IAuthSideEffectHandler` seam.
4. Shared Data Protection configuration and test fixture key-ring setup.
5. Test utilities that can mutate identity/profile/session state after token issuance and capture persisted outbox payloads without exposing secrets.

### Risks and mitigations

- **Stale bearer acceptance:** a JWT can remain cryptographically valid after account/session revocation. Mitigate with a single current-state check in the handler/service and integration tests that mutate state after issuance.
- **Cross-module coupling:** Auth might reach into Profiling persistence. Mitigate with an additive narrow `IUserLookupService`/authorization snapshot contract and keep all relational queries in the owning module.
- **Challenge replay or mismatch:** registration/resend could issue multiple active challenges or mismatch delivery and hash. Mitigate with a generated raw value held only in memory, hash persisted in the same transaction as the outbox envelope, purpose FK validation, and end-to-end consume tests.
- **Outbox ciphertext misuse:** encrypting a raw challenge but allowing generic logs/handlers to inspect it could still leak material. Mitigate with a typed protected envelope, a delivery-only unprotect interface, redacted serialization/logging, and negative assertions over payload/log/audit text.
- **Data Protection key-ring outage:** delivery may be impossible if protection/unprotection fails. Mitigate with safe retry/dependency-failure behavior and no raw fallback.
- **Race between state read and session rotation:** account disable/revoke could race refresh. Mitigate with an atomic repository operation or a documented consistency boundary; do not claim a plain pre-read is sufficient for concurrent security transitions.
- **NU1903 scope creep:** package updates can alter cryptography behavior or broad dependency graphs. Mitigate with package-graph evidence first and a bounded follow-up if not directly owned.

## Ordered implementation steps

### 1. Reconfirm contracts and ownership before edits

- Owner: backend implementation owner; read-only architecture/security review beforehand.
- Action: inspect current `IAuthRepository`, `IAuthSecurityStateService`, `ITokenService`, `IUserLookupService`, `IProfilingAuthorizationService`, `AuthBrowserTransport`, outbox handler, and test fixture composition against this plan.
- Modify: no production file in the planning phase; implementation target is the existing seams only.
- Contract impact: define one intention-revealing current-state result, for example `AuthSessionAccessDecision` containing account/session validity and a safe failure category. It must not expose password hashes, challenge material, or Profiling persistence types.
- Completion: owner can identify one path for refresh and bearer state evaluation, one challenge creation path per purpose, one outbox protection boundary, and one test fixture mutation seam. Any need for a migration, new module boundary, or public endpoint redesign stops for replanning.

### 2. Close current account/session state at refresh and bearer boundaries

- Owning files/seams: `AuthModuleSL/Abstractions/IAuthRepository.cs`, `AuthModulePL/Repositories/AuthRepository.cs`, a focused Auth SL state/session service or existing `AuthSecurityStateService`, `RegistrationService.RefreshAsync`, and `AuthTokenAuthenticationHandler.HandleAuthenticateAsync`.
- Add/modify: add/extend a narrow Auth state decision method; modify refresh to perform the decision before rotation and token issuance; modify bearer authentication to validate the token and then perform the same current-state decision before creating the `AuthenticationTicket`.
- Data flow: JWT `sub`/`sid` -> parse canonical user/session identifiers -> Auth repository checks session not revoked/rotated/expired and identity lock/verification state -> Profiling integration checks current deactivated/deleted state -> return principal or safe denial. Refresh uses the presented session -> same account/profile check -> atomic rotate only after the state is accepted.
- Required cases: verified active; unverified; disabled/deactivated; deleted; locked; unknown identity; expired/revoked/rotated session; missing session for bearer `sid`; repository/Profiling dependency failure. All denials are safe and do not reveal which state was found.
- Concurrency requirement: prefer one repository operation that checks the session predicate and performs rotation conditionally, or document and test the chosen transaction/isolation boundary. A non-atomic state pre-read must not be described as revocation protection.
- Contract impact: preserve existing refresh replay code and cookie/body transport; only add current-state enforcement. Preserve `HttpContext.User` principal claims for accepted requests.
- Completion: after a token/session is issued, mutating each state in a fresh context prevents refresh and protected bearer access; state restoration permits access where appropriate; concurrent revoke/refresh has one safe winner and no new token after revocation.

Illustrative shape only:

```text
validate bearer signature and required claims
  -> current Auth session/account decision(sub, sid, now)
  -> current Profiling state decision(sub, now)
  -> if not allowed: fail authentication / safe 401
  -> else create HttpContext.User

refresh presented session
  -> current state decision
  -> conditional rotate where session is still valid and state is active
  -> issue replacement access token only after committed rotation
```

### 3. Create and deliver purpose-backed verification challenges

- Owning files/seams: `RegistrationService.RegisterAsync`, `RegistrationService.ResendVerificationAsync`, `AuthLifecycleService` challenge helper or a new Auth SL challenge factory, `IAuthRepository.AddChallengeAsync`/purpose lookup, `AuthOutboxDispatcher`, `IAuthSideEffectHandler`, and related DM/PL tests.
- Add/modify: introduce a shared in-memory challenge generator and a purpose-code-to-purpose-row lookup; create `AuthChallenge` with SHA-256 hash, `email-verification` FK, 24-hour expiry, issue time, and safe metadata; add a protected delivery envelope to the outbox contract; modify registration/resend to persist challenge and outbox message in the same transaction.
- Data flow: generate raw random challenge -> hash immediately for `AuthChallenge.TokenHash` -> protect a delivery envelope containing only the minimum recipient/user/purpose/raw challenge data using the approved dedicated Data Protection purpose -> persist challenge hash and protected envelope atomically -> dispatcher unprotects only at delivery handler boundary -> handler sends/delivers without logging raw material.
- Registration behavior: preserve existing registration idempotency and welcome/verification side-effect semantics; a replayed idempotency result must not create an additional challenge/outbox pair unless current idempotency rules explicitly require it.
- Resend behavior: preserve enumeration-safe unknown/verified no-op response; eligible unverified account creates a fresh challenge and outbox message. Define whether prior unconsumed verification challenges are invalidated/superseded at this seam; this is an approval decision, not an assumption.
- Completion: the verify endpoint accepts the delivered raw challenge, hashes it, resolves the purpose by stable code, consumes it once before expiry, marks identity verified transactionally, and rejects replay/expiry/incorrect-purpose.

Illustrative shape only:

```text
raw = random()
hash = SHA256(raw)
purposeId = resolveActivePurposeId("email-verification")
challenge = AuthChallenge(identity, purposeId, hash, now, now + 24h)
envelope = protector("AuthModule.ChallengeDelivery.v1").Protect({ userId, email, purpose, raw })
persist(challenge, outbox(type, protectedEnvelope)) in one transaction
```

### 4. Remove raw password-reset challenge material from outbox persistence

- Owning files/seams: `AuthLifecycleService.ForgotPasswordAsync`, `AuthOutboxMessage` payload contract, `AuthOutboxDispatcher`, `IAuthSideEffectHandler`, `SecurityServices`/Data Protection registration, and outbox tests.
- Modify: keep `AuthChallenge.TokenHash` as the only database challenge representation; replace raw `challenge` in `PayloadJson` with the dedicated protected delivery envelope. Do not use the RSA signing-key purpose or serialize an unprotected fallback.
- Protection boundary: use the repository’s approved shared Data Protection key-ring/application-name boundary with a distinct challenge-delivery purpose and bounded version. If the existing protection store is RSA-only or unavailable to the outbox process, stop and request the user’s decision on a repository-approved alternative; do not invent a second encryption system.
- Dispatcher behavior: deserialize a safe envelope DTO, unprotect at the delivery boundary, validate purpose/user/email binding and envelope version, call the handler, then acknowledge. Invalid/unprotectable envelopes follow the existing safe retry/exhaustion path and never include raw or exception details in `LastError`, audit metadata, or logs.
- Completion: persisted outbox payload does not contain the raw challenge, its direct reversible plaintext encoding, or a field named `challenge` containing the raw value; the handler receives the expected delivery content only after successful unprotection; dependency failure leaves the message retryable without leaking material.

### 5. Add focused regression coverage for all three defects

- Owning files: `test/ResumeEnhancer.Tests/Modules/AuthModule/AuthDurableStateTests.cs`, `AuthRegistrationServiceTests.cs`, `AuthInfrastructureTests.cs`, `AuthBrowserTransportTests.cs`/`PrincipalIdentityTests.cs` as applicable, `test/IntegrationTest/Modules/AuthModule/AuthHttpIntegrationTests.cs`, and test-host support only where a new dependency seam is required.
- Add unit coverage for:
  - current-state decision success and each state denial, missing/expired/revoked session, and dependency failure;
  - challenge generation uniqueness, purpose FK, 24-hour verification expiry, 30-minute reset expiry, hash-only persistence, and single-use consume;
  - registration/resend outbox payload protection and no raw material in serialized JSON;
  - reset outbox protection/unprotection, wrong purpose/user, malformed envelope, key-ring failure, retry, and safe exhaustion;
  - no token/session creation on refresh denial and no principal on bearer state denial.
- Add HTTP integration coverage for:
  - issue a valid token/session, then mutate each account/session state and assert refresh/protected bearer rejection;
  - active verified state succeeds and principal-derived identity remains authoritative despite forged `X-User-Id`/`X-Audit-UserId` headers;
  - registration and resend create a persisted verification challenge and deliverable outbox envelope; verify challenge completion and replay/expiry behavior through the actual endpoint seam;
  - forgot password returns the same safe public response for known/unknown email while the known account has hash-only challenge persistence and a protected outbox payload;
  - dependency failures map to safe `401`/`503`/retry outcomes according to the approved boundary without exception text.
- Completion: each executable behavior path below has at least one focused test at the narrowest meaningful layer and one integration test when HTTP, composition, persistence, or transaction semantics are material.

### 6. Assess NU1903 and keep trusted-origin scope bounded

- Owner: implementation owner performs evidence collection; dependency owner/security reviewer decides remediation priority.
- Action: run direct/transitive package inspection for the Auth SL, Web, migration, unit, and integration projects; identify which package introduces `System.Security.Cryptography.Xml`, the resolved vulnerable version, and whether it is runtime-reachable from this change.
- If direct and in-scope: plan the smallest supported package upgrade/removal with a focused restore/build/security regression check. Do not alter cryptographic behavior without review.
- If transitive and not owned by the Auth change: record an explicit bounded follow-up (package owner, affected project, version/upgrade constraint, and validation command) and leave production dependency files unchanged in this batch.
- Trusted origins: run a targeted scan of current options/config and existing tests for `*`, but do not include a remediation step unless evidence proves the wildcard violates the accepted OpenSpec requirement in the current branch. If it does, stop for scope approval because that would add a separate trust-boundary correction.
- Completion: plan records command output, package provenance, severity/scope interpretation, disposition, and owner; no unbounded dependency upgrade or origin-policy redesign is hidden in the corrective batch.

## Per-file implementation details

| File or seam | Action | Layer/responsibility and flow | Contract/config/import impact | Completion condition |
|---|---|---|---|---|
| `AuthModuleSL/Abstractions/IAuthRepository.cs` | Modify | Auth persistence port for current session/account decision and atomic refresh transition | Add narrow records/results only; no EF or Profiling types | Both refresh and bearer use the same intention-revealing port |
| `AuthModulePL/Repositories/AuthRepository.cs` | Modify | Query/transaction ownership for session, identity, challenge, purpose, outbox state | Preserve shared UoW and existing indexes; migration only if proven necessary | State denial/rotation and challenge/outbox writes are transactionally evidenced |
| `AuthModuleSL/Services/RegistrationService.cs` | Modify | Registration, resend, refresh orchestration | Preserve endpoint/idempotency/legacy transport contracts | Verification challenge is persisted and delivered; refresh checks state before issuance |
| `AuthModuleSL/Services/AuthLifecycleService.cs` | Modify | Login/reset/verify orchestration and common challenge creation | Preserve generic recovery/verification response behavior | Reset outbox contains protected envelope; verify/reset consume matching hashes |
| `AuthModuleSL/Services/AuthSecurityStateService.cs` or focused sibling | Modify/add | Shared current-state policy and safe decision mapping | Must be injectable in Web auth handler and SL refresh flow | No duplicate account-state rules across login/refresh/bearer |
| `AuthModuleWeb/Authentication/AuthTokenAuthenticationHandler.cs` | Modify | Framework bearer authentication boundary | Keep scheme/order and principal claim contract | Invalid current state produces no authenticated ticket |
| `AuthModuleWeb/Outbox/AuthOutboxDispatcher.cs` | Modify | Delivery-only unprotect, validation, retry/ack | Protected DTO and handler interface evolve together | Raw material never appears in persisted payload/error/log paths |
| `AuthModulePL/SecurityServices.cs` and Data Protection composition | Modify only if required | Existing protected material boundary, distinct challenge purpose | No reuse of signing-key purpose; shared key-ring settings remain explicit | Fresh application instance can unprotect the envelope or dependency failure is safe |
| `AuthModuleDM/Entities/AuthChallenge.cs` and `AuthChallengePurpose.cs` | No schema change expected; inspect only | Existing hash-only purpose-FK model | Do not add raw token or free-form purpose code | Existing model tests continue to prove forbidden properties |
| `AuthModuleWeb/MiniApis/AuthMinimalApis.cs` and `AuthBrowserTransport.cs` | Inspect/modify only if result mapping or delivery endpoint seam requires | HTTP response/cookie behavior | No endpoint redesign; preserve current cookie/body semantics | State/challenge failures map to approved safe responses |
| `ProfilingModelSL/Integrations/IUserLookupService.cs` / authorization snapshot | Modify only if current state lacks required field | Narrow cross-module user state contract | No direct Profiling PL reference from Auth | Deactivated/deleted state is current and testable |
| `test/.../AuthDurableStateTests.cs` | Modify | Deterministic persistence/state/challenge unit evidence | No production behavior | Hash-only, expiry, reuse, state, transaction, and protection cases pass |
| `test/.../AuthRegistrationServiceTests.cs` | Modify | Service/outbox orchestration regression | Existing test doubles extended for protected envelope | Registration/resend/reset payload and refresh-state regressions pass |
| `test/.../AuthInfrastructureTests.cs` and related auth unit tests | Modify | Handler, composition, principal, and dependency-failure evidence | Keep project boundary focused | Bearer handler never accepts stale state and no raw secret is logged/returned |
| `test/IntegrationTest/.../AuthHttpIntegrationTests.cs` and test support | Modify | Real HTTP/composition/persistence proof | Add state mutation and outbox inspection helpers only | All HTTP state/challenge paths and existing session regressions pass |
| project files/configuration | No planned change; conditional only | NU1903 assessment and Data Protection binding evidence | Any package/config change requires explicit approval/replan | No speculative dependency or environment change is introduced |

## Coverage scenarios

| Path | Required scenario and expected evidence |
|---|---|
| Current-state success | Active, verified, non-disabled, non-deleted, non-locked account with unrevoked session refreshes and accesses a protected endpoint; new token/principal maps to the same subject/session. |
| Verification failure | Unverified account cannot refresh or access protected bearer endpoint; response is safe and no replacement session/token is created. |
| Disabled/deleted failure | Profiling state changed after issuance to deactivated or deleted; both refresh and bearer access fail at the approved boundary; no protected handler runs. |
| Locked failure | Auth identity locked after issuance; refresh and bearer fail until the approved unlock boundary; lock state does not leak. |
| Revoked/rotated/expired/not-found | Refresh replay, revoked family, rotated session, expired session, missing session, and bearer `sid` mismatch produce safe authentication failure and no token issuance. |
| Dependency failure | Auth repository, Profiling lookup, or Data Protection failure returns the approved safe result/retry state, preserves transaction/outbox consistency, and emits only redacted operational evidence. |
| Verification challenge success | Registration/resend creates a purpose-backed 24-hour challenge; the delivered raw material hashes to the persisted value and verifies once. |
| Verification no-op/conflict | Unknown or already verified email returns the existing enumeration-safe no-op; resend does not disclose account existence; repeated/competing active challenges follow the approved supersession rule. |
| Verification invalid/expired/reused | Wrong value, wrong purpose, expired value, and second use do not mark verified and do not disclose internal state. |
| Reset success | Known account recovery creates a 30-minute hash-only challenge and protected outbox envelope; delivered value resets password transactionally and revokes applicable sessions. |
| Reset unknown/no-op | Unknown email receives the same public response and no identifiable challenge/outbox record. |
| Reset envelope failure | Malformed, wrong-purpose, wrong-user, expired protected envelope, unavailable key ring, and unprotect failure are retryable/safely exhausted with no raw material in `LastError`, audit, or logs. |
| Raw-secret negative proof | Persisted `AuthChallenge`, `AuthOutboxMessage.PayloadJson`, audit rows, responses, and captured logs contain no raw challenge or password; only hash/ciphertext/safe IDs appear. |
| Refresh transport regression | Existing cookie-only, legacy body-only, mixed transport, CSRF/origin, logout, replay, registration, and cookie-attribute tests remain green. |
| Authorization regression | Forged identity headers remain ignored; valid principal and current state reach the existing roles/capabilities/entitlements/ownership checks; guest routes remain unchanged. |
| Empty/no-op | Unknown logout/resend/recovery and absent/empty bearer/refresh inputs follow established safe no-op/error contracts without persistence mutation. |
| NU1903 | Package provenance is identified; in-scope remediation is bounded and verified, or an explicit follow-up owner/disposition is recorded. |
| Wildcard origin watch | Current configuration/tests are scanned; no scope expansion occurs unless a direct requirement violation is demonstrated and approved. |

## Validation commands, expected evidence, and recovery

Commands are for the approved implementation handoff; this planning turn does not claim their results.

1. Worktree/ownership baseline:

   ```powershell
   git -C D:\RND\ResumeEnhancer\.worktrees\gh-36-authentication-authorization-hardening status --short --branch
   git -C D:\RND\ResumeEnhancer\.worktrees\gh-36-authentication-authorization-hardening diff --name-only
   ```

   Expected: branch identity matches; prior user changes are preserved; implementation owner records any overlap before editing.

2. Dependency/security assessment:

   ```powershell
   dotnet list application\Modules\AuthModule\AuthModuleSL\ResumeEnhancer.AuthModule.SL.csproj package --include-transitive --vulnerable
   dotnet list application\WebSolution\WebSolution.Server\ResumeEnhancer.WebSolution.Server.csproj package --include-transitive --vulnerable
   rg -n "System.Security.Cryptography.Xml|NU1903|TrustedOrigins|[\"']\*[\"']" application test
   ```

   Expected: exact NU1903 package provenance and current wildcard evidence; no remediation is inferred from a warning without ownership.

3. Focused builds:

   ```powershell
   dotnet build test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore --nologo
   dotnet build test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore --nologo
   ```

   Expected: exact build evidence or an explicit environment blocker. Existing JavaScript SDK/NuGet.Config blockers must remain distinct from Auth defects.

4. Focused unit tests:

   ```powershell
   dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-build --filter "FullyQualifiedName~AuthModule" --logger "console;verbosity=normal"
   ```

   Expected: exact passed/failed/skipped totals covering state, challenge, protection, principal, redaction, and persistence paths. `--no-build` is artifact execution evidence only.

5. Focused integration tests:

   ```powershell
   dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-build --filter "FullyQualifiedName~AuthModule" --logger "console;verbosity=normal"
   ```

   Expected: exact HTTP totals for post-issuance state mutation, refresh/bearer denial, registration/resend/reset delivery, replay/expiry, outbox secrecy, CSRF/origin, and existing session regressions.

6. Broader relevant verification:

   ```powershell
   dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore
   dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore
   dotnet build application\ResumeEnhancerApp.slnx
   openspec validate --changes gh-36-authentication-authorization-hardening
   ```

   Expected: report clean rebuild separately from existing-artifact test execution; OpenSpec remains valid; no task checkbox is changed by this plan.

### Rollback/recovery

- Preserve the existing worktree and prior staged changes; use only path-scoped, approved recovery after implementation review. Never reset the worktree or delete challenge/session data as a debugging shortcut.
- If current-state enforcement cannot be made atomic enough for the approved security boundary, stop before migration or contract changes and return for replanning.
- If Data Protection cannot be shared across instances or safely isolated by purpose, stop and request the user’s choice of an approved delivery-protection boundary; never fall back to plaintext outbox material.
- If a schema change is required, leave the Proposed plan unchanged and create a revised plan with migration ownership, compatibility, rollback, and SQL-provider evidence.
- If NU1903 requires a package upgrade outside Auth ownership, record the bounded follow-up instead of bundling a speculative upgrade.

## OpenSpec task completion evidence

The implementation owner must not edit the task checkboxes as part of this plan handoff. The coordinator may reconcile the covered task evidence only after:

- current account/session state is enforced and proven in refresh and protected bearer integration tests;
- verification and reset challenges are purpose-backed, hash-only in persistence, and delivered through a protected outbox envelope with negative raw-secret assertions;
- existing registration/bootstrap/refresh/logout/resend/CSRF/authorization behavior remains green;
- focused unit and integration totals are exact and any clean-build/environment limitation is separately recorded;
- NU1903 provenance/disposition and wildcard-origin watch evidence are recorded;
- `openspec validate --changes gh-36-authentication-authorization-hardening` passes;
- scoped diff review proves no frontend, unrelated worktree, generated workflow, migration, configuration, or task-checkbox mutation beyond approved scope.

## Unresolved decisions and exact user approval required

Approval is required for this exact Proposed corrective batch and every covered task identity. Before implementation, the user must explicitly decide:

1. **Current-state failure mapping:** Should stale/disabled/deleted/locked/unverified bearer and refresh requests all fail as safe `401` authentication failures, or should any authenticated-but-no-longer-authorized case remain a `403` at the existing authorization boundary? The plan recommends `401` before principal/session acceptance and `403` only for post-auth role/capability/ownership denial, but this is a user approval decision.
2. **Session check authority:** Approve using the `sid` claim and durable Auth session record as the authoritative bearer-session revocation check, with the same state decision reused by refresh; otherwise specify the approved alternate authority.
3. **Challenge delivery protection:** Approve a dedicated ASP.NET Core Data Protection purpose such as `AuthModule.ChallengeDelivery.v1`, shared through the existing persisted key ring and unprotected only at the outbox delivery boundary. If the raw challenge must instead be delivered by an external provider contract, specify that contract; plaintext outbox storage is not an option.
4. **Verification resend supersession:** Should a resend invalidate prior unconsumed verification challenges for that identity/purpose, or may multiple unexpired challenges remain valid? The plan recommends superseding prior unconsumed challenges to reduce replay surface, but does not assume this without approval.
5. **NU1903 disposition:** If package inspection finds a transitive warning outside Auth ownership, approve recording a bounded follow-up rather than changing package versions in this corrective batch. If direct/in-scope, approve the smallest compatible remediation after evidence is attached.
6. **Wildcard-origin watch:** Approve keeping wildcard trusted-origin behavior separate unless current repository evidence proves violation of the accepted requirement; if the user wants it included regardless, it requires a revised scope and separate task mapping.

No approval is requested on the user’s behalf by this plan; the status remains Proposed until the user explicitly approves the exact change and covered tasks.

## Visible evidence ledger

| Source | Date/version | Observation | Interpretation | Confidence | Limitation | Effect on plan |
|---|---|---|---|---|---|---|
| `AGENTS.md` | Current main checkout, inspected 2026-09-20 | Auth Web/AM/SL/DM/PL ownership, host composition boundary, approval-gated OpenSpec implementation, preserve unrelated work | One serialized backend owner and no production edits by planner | High | Repository policy, not runtime proof | Sets ownership and planning boundary |
| `KnowledgeBase/INDEX.md` | Last reviewed 2026-08-23 | Routes API, EF/persistence, modular architecture, and applicable ADR authorities | Minimum authority set selected | High | Index is routing, not behavior evidence | Governs which topics/ADRs were read |
| `dotnet-backend-api-delivery.knowledge.md` and `resumeenhancer-api-application-delivery.knowledge.md` | Last reviewed 2026-08-23 | Boundary validation/application invariants/test layers and Web-AM-SL-PL composition | Current-state checks belong in reusable SL/PL seams; HTTP integration proves composition | High | General guidance | Shapes per-file ownership and tests |
| `dotnet-ef-core-persistence.knowledge.md` and `persistence-project.knowledge.md` | Last reviewed 2026-08-15/23 | Durable state, transactions, indexes, migrations, shared UoW, module PL ownership | Existing model should be reused; migration is conditional | High | Does not prove provider concurrency in this branch | Limits persistence scope and recovery |
| ADR-001, ADR-002 | Accepted 2026-08-16 | Auth owns authentication/session/cookies/request identity; Profiling owns users/access profiles; cross-module use narrow SL contracts | State lookup must not use Profiling repositories directly from Auth | High | ADR does not prescribe exact result codes | Defines integration seam |
| ADR-003, ADR-004, ADR-005 | Accepted 2026-08-16 | Setup values use entities/FKs, stable codes, and owning-module setup access | Challenge purpose must remain FK/code-backed; no magic IDs/free-form persisted purpose | High | Existing Auth seeder implementation is still current-code evidence | Preserves purpose model |
| OpenSpec proposal/design/specs/tasks | Current canonical worktree artifacts, rechecked 2026-09-20 | Requires active-state enforcement, 24h/30m challenges, hash-only storage, secure sessions, regression coverage; 6.2 and 6.3 are `[ ]` with Rawls’ missing-coverage and blocked-acceptance notes | Requested blockers are within accepted behavior, not scope expansion; open 6.2/6.3 accurately reflect incomplete acceptance evidence | High | Task status is coordinator-owned and can change independently of this plan | Uses exact current identities without editing tasks |
| Issue/story artifacts `.tmp/artifacts/issues/36.md`, `User-Stories/3.3...` | Current worktree, inspected 2026-09-20 | Backend-only auth hardening, current state during refresh/protected access, compatibility constraints | No frontend or endpoint redesign | High | Review feedback is not stored as a Rawls file | Defines exclusions and compatibility |
| `RegistrationService.cs` | Current canonical worktree, inspected 2026-09-20 | Refresh checks session lifecycle only; registration/resend enqueue verification mail without challenge creation | Confirms state and verification-delivery blockers | High | Worktree contains in-progress staged edits | Targets steps 2 and 3 |
| `AuthLifecycleService.cs` | Current canonical worktree, inspected 2026-09-20 | Reset stores hash but places raw challenge in outbox JSON; verify consumes purpose/hash | Confirms reset secrecy defect and existing consume seam | High | Delivery handler behavior is logging-only in current code | Targets step 4 and negative tests |
| `AuthTokenAuthenticationHandler.cs` | Current canonical worktree, inspected 2026-09-20 | Validates JWT and immediately creates ticket; no current account/session check | Confirms stale bearer defect | High | Does not by itself show full middleware ordering | Targets step 2 |
| `EndpointAuthorizationMiddleware.cs` | Current canonical worktree, inspected 2026-09-20 | Checks Profiling authorization snapshot but only after authenticated principal exists | Existing authorization is not a substitute for Auth session state | High | Protected metadata coverage varies by endpoint | Keeps fix at authentication/current-state boundary |
| `AuthRepository.cs`, `IAuthRepository.cs`, Auth DM/PL entities | Current canonical worktree, inspected 2026-09-20 | Existing session, purpose, hash-only challenge, outbox, Data Protection/key-material seams | Correct extension points exist without immediate schema redesign | High | Atomic refresh/state operation still needs implementation proof | Favors additive ports and conditional migration |
| `AuthOutboxDispatcher.cs` and `SecurityServices.cs` | Current canonical worktree, inspected 2026-09-20 | Outbox payload is user/email only; dispatcher has no protected challenge envelope; Data Protection protects RSA material under configured purpose | Delivery contract must be added and purpose-isolated | High | Current handler is a logging stub, not an external email provider | Targets protected envelope and dependency-failure tests |
| Auth unit/integration test files | Current canonical worktree, inspected 2026-09-20 | Generic challenge/session tests exist; post-issuance state and raw outbox negative tests do not | Focused regression coverage is missing exactly at the three blockers | High | Test totals were not rerun in this planning turn | Defines coverage and validation |
| Project/package scan | Current canonical worktree, inspected 2026-09-20 | Direct Auth package references are IdentityModel; no direct `System.Security.Cryptography.Xml` reference found | NU1903 provenance is unresolved and must be assessed, not guessed | Medium-high | No package vulnerability command was run in planning turn | Adds bounded assessment step |
| User request | 2026-09-20 | Explicit Rawls blockers, canonical worktree, planning-only boundary, wildcard watch instruction | Direct scope and approval gate | High | No standalone Rawls cited-file artifact exists locally | Controls exact plan outcome |

## Assumptions, unknowns, conflicts, rejected alternatives, and deferred decisions

- **Confirmed:** the existing Auth challenge entity is hash-only and purpose-FK-backed; the plan does not add raw-token persistence.
- **Confirmed:** current code lacks end-to-end verification challenge creation/delivery and leaks raw reset challenge material into outbox JSON.
- **Confirmed:** bearer JWT validation alone does not enforce current account/session state.
- **Inferred:** the existing Data Protection key ring is the approved protection boundary for protected delivery material because it already protects Auth signing-key material; user approval is still required for the new purpose and consumer boundary.
- **Unknown:** whether the final delivery handler is expected to send raw challenge content directly or pass a protected envelope to an external notification service. The plan keeps the delivery boundary explicit and requires approval.
- **Unknown:** whether current SQL/provider transaction semantics can atomically combine account-state checking with refresh rotation. Implementation must prove this or stop for replanning.
- **Conflict resolved:** the canonical task artifact now leaves 6.2 and 6.3 unchecked because Rawls identified missing integration coverage and blocked acceptance review. The plan mirrors that current status and does not edit the artifact or claim completion.
- **Rejected alternative:** store verification/reset raw challenges in outbox JSON for convenience. This violates the stated security boundary.
- **Rejected alternative:** rely only on JWT claims or only on Profiling authorization middleware for current state. Neither proves durable session revocation plus Auth identity state at both refresh and bearer boundaries.
- **Rejected alternative:** add a second encryption/key-management stack without evidence that the existing Data Protection boundary is unsuitable. This would expand architecture and deployment risk.
- **Rejected alternative:** include wildcard trusted-origin remediation solely because it is security-adjacent. The user explicitly made it a separate watch item.
- **Deferred:** email provider/template design, frontend behavior, external KMS/HSM, broad package upgrades, and any new migration not proven necessary.

## Recorded self-review

Self-review re-run as an independent pass on 2026-09-20 after reconciling the coordinator’s task correction:

- Traceability checked: each requested defect maps to exact current symbols, covered task identities, ordered implementation steps, scenarios, and validation commands.
- Task identity/status checked: frontmatter now reproduces canonical 6.2 and 6.3 text as `[ ]`, including Rawls’ coverage and acceptance-blocker notes; no OpenSpec task file was edited.
- Ownership checked: Auth owns authentication/session/challenge orchestration; Profiling remains the state source; PL owns queries/transactions; Web owns HTTP authentication/outbox composition; tests prove integration.
- Dependency order checked: current-state contract precedes refresh/bearer changes; challenge factory/protection precedes registration/resend/reset delivery; tests follow both; NU1903 follows package provenance rather than speculative upgrade.
- Security checked: raw challenge material is explicitly prohibited from persistence/logging/audit/response; Data Protection purpose isolation and no-plaintext fallback are explicit; bearer/refresh current-state checks and race handling are required.
- Contract checked: existing endpoint purposes, legacy body-only refresh compatibility, cookie transport, idempotency, safe recovery responses, and task checkbox immutability are preserved.
- Coverage checked: success, validation/authorization failure, not-found/conflict, empty/no-op, dependency failure, result mapping, persistence, replay, expiry, concurrency, and integration paths are present.
- NU1903 checked: no direct package reference was found; the plan correctly treats provenance/disposition as unresolved rather than claiming a remediation.
- Wildcard scope checked: it is a watch item only, matching the user’s instruction.
- Staleness check corrected: all wording claiming that the live covered tasks were checked was replaced with the current 6.2/6.3 open-status explanation; no corrective scope or unresolved decision was broadened.
- Corrections made during review: added explicit current-state race requirement, delivery-only unprotect boundary, resend supersession decision, negative raw-secret assertions, package-owner follow-up, and exact OpenSpec completion evidence.
- Remaining limitation: no standalone Rawls review artifact or literal cited-file list exists in the worktree. The plan therefore records the user-provided blocker list plus direct current-code evidence and does not invent review quotations or external findings.

## Approval status and next safe action

- Gate status: **Proposed; approval gate pending**.
- Required approval: explicit approval of this exact plan, all eight covered OpenSpec task identities, the four material behavior decisions (current-state result mapping, `sid` session authority, Data Protection challenge envelope, resend supersession), and the bounded NU1903/wildcard dispositions above.
- Next safe action: user reviews and either explicitly approves this plan or requests revisions. After approval, only the approved workflow may mark this plan `Approved` with approval metadata and hand it to exactly one backend implementation owner. This planner must not begin implementation.
