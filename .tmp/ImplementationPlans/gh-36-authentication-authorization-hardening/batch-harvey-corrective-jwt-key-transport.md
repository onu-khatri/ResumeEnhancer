---
kind: implementation-plan
status: Proposed
scope: batch
change: gh-36-authentication-authorization-hardening
tasks:
  - "[ ] 3.1 Replace custom access-token text/HMAC issuance with JsonWebTokenHandler and RS256 JWT issuance/validation using configured issuer, audience, subject, session ID, 30-minute lifetime, and 30-second clock skew; verify valid, malformed, expired, unsecured, wrong-algorithm, wrong-key, and invalid-claim tests."
  - "[ ] 3.2 Implement protected application-owned RSA key loading, key identifiers, configurable 180-day rotation, 48-hour previous-key overlap, and emergency invalidation behavior; verify key material is absent from logs/responses and rotation overlap tests pass."
  - "[ ] 3.3 Preserve refresh rotation/replay detection while moving browser refresh transport to HttpOnly, Secure, SameSite cookies and access transport to Authorization; verify registration, login, refresh, logout, revocation, and concurrent-use integration tests."
created_at: 2026-09-16
plan_version: 1
---

# Corrective implementation plan: JWT, durable signing keys, and refresh transport

## Task identity, scope, phase, and owner

This is one dependency-linked corrective batch for OpenSpec change `gh-36-authentication-authorization-hardening`, covering the exact unchecked task text for 3.1, 3.2, and 3.3 above. The current phase is implementation planning after the staged task-1/2 baseline and the latest Harvey review findings supplied by the user. The intended implementation owner is one `backend-implementer` in the canonical worktree `D:\RND\ResumeEnhancer\.worktrees\gh-36-authentication-authorization-hardening`, after this plan is explicitly approved. The owner must serialize shared Auth SL/PL/Web, options, composition, transport, and migration decisions.

## Goal and success criteria

Replace the current partially hardened token path with standards-compliant RS256 JWT issuance and validation backed by protected, application-owned, durable key lifecycle state; preserve refresh rotation/replay semantics while making browser refresh cookie-based and access bearer-based.

Success requires:

- only RS256 JWTs from `JsonWebTokenHandler` are issued and accepted, with issuer, audience, canonical positive invariant-culture `sub`, `sid`, `iat`, `nbf`, `exp`, `kid`, and configured 30-minute lifetime/30-second skew;
- one validated `AuthSecurityOptions` registration feeds token, key, cookie, CSRF, origin, and startup behavior;
- private RSA material is loaded only through a protected application-owned provider/reference, is never durable in EF, logged, returned, or committed;
- rotation is actually configurable and scheduled at 180 days, with a previous key accepted only from `RetiredAtUtc` through a configurable 48-hour overlap, and durable invalidation is authoritative across fresh service instances;
- sync and async validation share one authoritative validation policy rather than allowing sync fallback around durable invalidation or `RetiredAtUtc`;
- browser refresh uses secure HttpOnly/Secure/SameSite cookies, access uses `Authorization`, cookie mutations require trusted-origin and CSRF proof, and the legacy body-only path has an explicit compatibility decision;
- registration, login, refresh, logout, revocation, replay, concurrent-use, and existing endpoint purposes remain compatible; no frontend files change.

## Current-state evidence

- OpenSpec `tasks.md` marks 3.1–3.3 unchecked and defines the required JWT, key, rotation, overlap, cookie, and regression behavior. `proposal.md`, `design.md`, and the delta spec require application-owned protected RSA keys, RS256, fail-safe loading, secure cookie transport, and preservation of existing endpoint purposes.
- `AuthSecurityOptions.cs:18-40` already exposes issuer, audience, 30-minute lifetime, 30-second skew, 180-day rotation, 48-hour overlap, active/previous key IDs/references, PEM fields, retirement, invalidated IDs, cookie flags, and trusted origins. `AuthSecurityOptionsValidator.Validate` validates shape but currently permits configuration to remain split between PEM material, references, and durable metadata; this is the options consolidation seam.
- `AuthSecurityServices.cs:38-175`, `AuthTokenService`, issues RS256 through `JsonWebTokenHandler`, but constructor-time PEM loading owns secret access, `CreateAccessToken` checks only configured invalidated IDs, synchronous `ValidateAccessToken` can run without repository metadata, async validation creates a scope and catches all exceptions, and `InvalidateKey` is empty. `GetValidationKeys`/`IsKeyCurrentlyAccepted` mix options and optional durable metadata and do not constitute one durable provider or actual rotation operation.
- `AuthSigningKeyMetadata.cs:6-17` contains non-secret `KeyIdentifier`, `ProtectedReference`, `ActivatedAtUtc`, `RetiredAtUtc`, and `InvalidatedAtUtc`. It is a candidate durable metadata model, not proof of an active-key pointer, atomic rotation, or fresh-instance authority.
- `AuthRepository.cs` contains the existing Auth persistence adapter and signing-key metadata/invalidation seams. It must remain the only EF/AppDbContext adapter; no private key material or second context may be introduced.
- `AuthBrowserTransport.cs:9-54` classifies `LegacyBody`, `BrowserCookie`, and `Ambiguous`, sets refresh/CSRF cookies, and compares CSRF values, but its current proof path does not visibly enforce the configured trusted-origin allowlist. `AuthMinimalApis.cs` reads cookie or JSON refresh material and returns ad hoc 400/403 JSON in transport branches; this is the exact compatibility/error-mapping seam.
- `AuthMinimalApis.cs` and `RegistrationService.cs:107-201,247+` show existing registration/session creation and refresh flows. They must retain hashed refresh-at-rest, rotation, family revocation, replay, expiry, and concurrent-use semantics while adapting response transport.
- `AuthInfrastructureTests.cs:68-245` already covers issuance, invalid token forms, canonical subjects, a 47/48/49-hour overlap boundary, configured invalidation, and async durable overlap. It references `AuthSecurityOptions` without `using ResumeEnhancer.AuthModule.SL.Options;`, a current test compilation defect that must be corrected in the same test ownership lane.
- `AuthBrowserTransportTests.cs:11-84` covers cookie flags, CSRF token equality, bearer-only no-cookie behavior, and transport classification, but does not prove trusted-origin rejection in the helper’s actual implementation or endpoint-level body/cookie compatibility.
- The canonical worktree status shows all task-1/2 and earlier task-3 files staged as user work. Existing Proposed/Approved plans under the same directory are preserved and are not replaced by this plan.

## Scope, exclusions, assumptions, dependencies, and risks

### Included

- Auth SL token/key ports and implementation, Auth PL durable metadata/provider operations, existing migration only if the reviewed model truly lacks required lifecycle state, one options/configuration registration, Web bearer integration, refresh-cookie/CSRF/origin transport, and focused unit/integration tests.
- Corrective behavior explicitly identified by Harvey: durable application-owned provider, actual 180-day rotation, 48-hour overlap, authoritative sync/async validation, durable invalidation and `RetiredAtUtc`, startup usability validation, legacy refresh compatibility, duplicate exception handling, and the `AuthSecurityOptions` test compile fix.

### Excluded

- Task 4 authorization metadata/fallback, task 5 lifecycle/rate-limit/error-audit expansion, task 6 checkbox or final acceptance updates, frontend work, endpoint redesign, HMAC or dual-algorithm compatibility, external KMS/vendor selection, private key columns in EF, generated `.agents` workflows, OpenSpec task checkbox edits, commits, pushes, and PR operations.

### Dependencies and assumptions

- Approved task-1/2 changes and their migrations/tests are the baseline; the implementation owner must re-read their approved plans before editing.
- `AuthSigningKeyMetadata` remains non-secret durable state. Secret material is available through the repository-supported protected configuration/secret binding; the exact deployment binding name is not assumed here.
- Existing callers may include non-browser body-only refresh clients. This is a research/decision point, not silently inferred authorization to remove the body contract.
- A provider instance must be safe for concurrent request validation and fresh application instances. If this requires a new store, external provider, second DbContext, or breaking contract, stop for replanning and approval.

### Risks

- Stale local key state can accept invalidated or expired previous keys; mitigation is a single provider result used by both sync and async paths, with durable state queried or refreshed before acceptance.
- Rotation races can produce two active keys or partial retirement; mitigation is an atomic durable operation with explicit concurrency/conflict results and fresh-instance tests.
- Protected secret configuration may be unavailable at startup; fail startup before request processing, without logging the reference value or material.
- Cookie migration can break non-browser consumers or create token confusion; use a documented body/cookie matrix and reject ambiguous requests.
- Existing ad hoc exception/result branches can leak or duplicate handling; centralize safe mapping and test one response per failure.

## Disjoint ownership and dependency order

One serialized backend owner is required because options, provider, token validation, response transport, and composition share contracts. The ownership is disjoint by responsibility:

1. Auth SL owns options policy, narrow token/key provider contracts, token issuance/validation orchestration, and safe result classification.
2. Auth PL owns durable metadata queries, atomic rotation/invalidation, EF configuration, and conditional migration artifacts; it does not own JWT policy or private material.
3. Auth Web/host composition owns bearer handler registration, startup options validation, cookie/CSRF/origin enforcement, endpoint transport adaptation, and no direct persistence.
4. Auth unit/HTTP tests own the executable proof; they may fix the missing options namespace import but must not weaken assertions or delete task-1/2 coverage.

Order is 3.2 provider/state contract and options validation → 3.1 token issuance/validation and bearer composition → 3.3 transport adaptation and integration verification. No parallel edits are permitted across shared options, provider, migration, or endpoint response contracts.

## Ordered implementation steps

### 1. Rebaseline exact seams and compatibility evidence

- **Owner/files:** implementation owner; inspect `AuthSecurityOptions`, `AuthTokenService`, `AuthSecurityStateService`, `IAuthRepository`, `AuthRepository`, `AuthSigningKeyMetadata`, `AuthMinimalApis`, `AuthBrowserTransport`, `RegistrationService`, all refresh request consumers/tests, and existing approved plans.
- **Action:** inventory every token/key/refresh construction and validation call, determine whether `CreatedAtUtc`/rotation metadata and active-key identity already exist, and identify body-only clients/tests. Do not edit during this inventory.
- **Completion:** a current symbol/call-site list is attached to the implementation report; any missing schema, external secret binding, or breaking consumer is escalated before edits.

### 2. Consolidate and startup-validate security options

- **Owner/files:** modify `AuthModuleSL/Options/AuthSecurityOptions.cs`; `AuthModuleSL/Composition/DependencyInjection.cs`, `AuthModulePL/Composition/DependencyInjection.cs`, `AuthModuleWeb/DependencyInjection.cs`, `ModulesComposition/DependencyInjection.cs`, `WebSolution.Server/Program.cs`, and `appsettings.json` only where registration/reference defaults require it.
- **Behavior:** bind `Auth:Security` once; validate issuer/audience, exact/default 30-minute lifetime and 30-second skew bounds, active/previous IDs and references, 180-day rotation and 48-hour overlap, cookie invariants, trusted origins, invalidation shape, and protected reference usability. Startup validation must verify that an active key reference resolves to an RSA private key suitable for RS256, without logging material. Runtime consumers receive the same validated object/provider; no manual `IConfiguration` reconstruction remains.
- **Contract impact:** internal DI/configuration only; existing endpoint JSON remains unchanged. Invalid configuration is a startup failure, not a random-token fallback.
- **Completion:** options tests prove valid defaults and every invalid/empty/zero/negative/overflow/unusable-key case; host startup tests prove failure before serving requests and no secret output.

### 3. Establish the application-owned durable signing-key provider

- **Owner/files:** add/modify `AuthModuleSL/Abstractions/IAuthSigningKeyProvider.cs` and result types; modify `AuthModulePL/Repositories/AuthRepository.cs` and its `IAuthRepository` extension only for non-secret lifecycle operations; modify `AuthModuleSL/Services/AuthSecurityServices.cs` or add the provider/token service at the correct SL seam; reuse `AuthSigningKeyMetadata.cs`, `AuthEntityConfigurations.cs`, and existing migration/snapshot only if necessary.
- **Behavior:** the provider resolves protected material by durable `KeyIdentifier`/`ProtectedReference`, exposes one active key and eligible previous keys, evaluates `RetiredAtUtc` plus overlap and `InvalidatedAtUtc`, and performs atomic due rotation. Rotation at `now >= active.ActivatedAtUtc + KeyRotationPeriod` creates/loads a new protected key reference, makes it active, retires the former active at the same durable instant, and preserves exactly one active key. Emergency invalidation is persisted and immediately authoritative for issuance and validation. Provider/repository/configuration failures return a safe unavailable result or fail startup; they never fall back to process-local/random/HMAC keys.
- **Persistence impact:** metadata only; use existing `AppDbContext`/`IUnitOfWork` and concurrency conventions. Generate an additive migration only if active/previous lifecycle data cannot be represented; inspect all operations and do not apply it here.
- **Completion:** fresh provider instances see invalidation and retirement; concurrent rotation produces one winner and a safe loser; secret scan finds no private material in source, migration, logs, or responses.

Illustrative provider shape (not production code):

```text
GetValidationSnapshot(now, ct)
  -> durable active + previous metadata
  -> protected-store resolve by reference
  -> reject invalidated active/previous; accept previous only [RetiredAtUtc, RetiredAtUtc + overlap)

EnsureRotation(now, ct)
  -> compare durable active activation time to configured period
  -> transactional conditional update / concurrency result
  -> new active + old active RetiredAtUtc, never two active winners
```

### 4. Unify sync/async RS256 issuance and validation

- **Owner/files:** modify `AuthTokenService`/`IAuthSecurityServices` or the selected SL token seam; modify `AuthTokenAuthenticationHandler.cs` and Web registration as needed.
- **Behavior:** use `JsonWebTokenHandler` with `RsaSecurityKey`, `SecurityAlgorithms.RsaSha256`, `kid`, configured issuer/audience, `sub`, `sid`, `iat`, `nbf`, `exp`, required signed tokens, valid algorithms restricted to RS256, and configured lifetime/skew. Validate canonical positive invariant-culture decimal `sub`, a valid `sid`, all required claims, key acceptance, durable invalidation, and `RetiredAtUtc` in one policy. Synchronous framework callbacks must not silently bypass durable state; use a coherent provider snapshot/cache contract that is authoritative and bounded, or change the integration seam to an async-capable path. Async and sync must map the same malformed/expired/wrong-key/provider-failure outcomes to no principal/safe failure. Remove duplicate broad exception handling where lower layers already classify expected token errors; preserve cancellation and map unexpected failures once.
- **Contract impact:** additive/implementation-level JWT change; preserve endpoint purposes and safe `AuthTokens` response shape unless a separately approved compatibility change is required.
- **Completion:** valid, malformed, expired, not-yet-valid, unsecured, wrong-algorithm, wrong-key, issuer/audience, missing claim, invalid `sid`, noncanonical subject, invalidated key, overlap boundary, provider unavailable, sync, and async tests agree.

Illustrative validation shape (not production code):

```text
token -> read kid only for lookup -> provider snapshot -> JsonWebTokenHandler RS256 validation
      -> required claims + canonical sub/sid -> principal
      -> any expected invalid/provider-unavailable outcome -> no principal
```

### 5. Preserve refresh semantics and make browser transport explicit

- **Owner/files:** modify `AuthBrowserTransport.cs`, `AuthMinimalApis.cs`, `AuthRequests.cs`/`AuthResponses.cs` only additively if needed, and the existing registration/refresh/logout service call sites.
- **Behavior:** access JWT is returned/used through `Authorization`; browser refresh is set only in `HttpOnly`, `Secure`, `SameSite=Lax`, `__Host-`, `Path=/`, no-domain cookie. CSRF cookie/header values must match in fixed time, and cookie mutations must require an explicit configured trusted `Origin` (or the approved same-origin policy) before mediator/session mutation. Define and test: cookie-only browser, body-only legacy, both-present ambiguous, neither, malformed body, invalid cookie, and untrusted/missing origin. The legacy body-only path is retained only if current consumer evidence confirms it; it must not be emitted to browser responses or treated as a browser cookie substitute. Both-present requests should be rejected unless the user approves a different precedence.
- **Semantics preserved:** hashed refresh storage, rotation, family revocation, reuse detection, expiry, logout idempotency, concurrent-use behavior, and existing register/refresh/logout purposes. Cookie clearing occurs safely on invalid/revoked browser refresh/logout without returning token material.
- **Completion:** endpoint integration tests prove cookie flags, bearer access, CSRF/origin rejection before mutation, explicit body compatibility, ambiguous rejection, refresh rotation/replay, concurrent use, revocation, and registration/login/logout regressions.

### 6. Correct test compilation and execute focused verification

- **Owner/files:** modify `test/ResumeEnhancer.Tests/Modules/AuthModule/AuthInfrastructureTests.cs` to import `ResumeEnhancer.AuthModule.SL.Options` (or use an equivalent fully qualified reference); extend the existing Auth infrastructure/browser tests and `AuthHttpIntegrationTests.cs`/`AuthApiTestData.cs` without removing prior coverage.
- **Behavior:** add injectable clock/provider fixtures for exact 180-day/48-hour boundaries and fresh-provider invalidation; keep test secrets generated in memory and never asserted in logs/responses.
- **Completion:** test project compiles past the current `AuthSecurityOptions` symbol issue; focused tests provide exact totals and failures; environment/build limitations are reported separately.

## Per-file implementation details

| File/seam | Action | Owning layer and responsibility | Control/data flow and impact | Completion |
|---|---|---|---|---|
| `AuthSecurityOptions.cs` | Modify | SL options policy | One bound/validated object feeds provider, token, transport, and startup | Invalid config and usable-key startup tests pass |
| `IAuthSigningKeyProvider.cs` (new) | Add | SL abstraction | Token service asks for snapshot/rotation/invalidation without EF/config types | Narrow result contract is cancellation-aware and failure-safe |
| `AuthSecurityServices.cs` / token seam | Modify | SL | Credentials/session flow calls provider; sync/async validation shares policy | RS256 and durable state behavior proven |
| `AuthSecurityStateService.cs` | Modify only if it owns shared options/provider hookup | SL | No duplicate key or exception policy | No unrelated durable-state regression |
| `AuthSigningKeyMetadata.cs` and `AuthEntityConfigurations.cs` | Modify only if required | DM/PL model | Non-secret lifecycle/reference fields and concurrency/index rules | Model remains additive and migration-reviewed |
| `IAuthRepository.cs` / `AuthRepository.cs` | Modify | SL port / PL adapter | Durable metadata reads and atomic rotation/invalidation through existing UoW | No raw EF/private key leakage; fresh instance proof |
| Migration/snapshot | Conditional add/modify | Infrastructure migration | Schema supports metadata only | Generated operations reviewed; never applied by this plan |
| `AuthModuleSL/Composition`, `AuthModulePL/Composition`, `AuthModuleWeb/DependencyInjection`, `ModulesComposition`, `Program.cs` | Modify | Composition/Web host | One DI graph; authentication is registered before authorization | Startup/composition tests pass |
| `AuthTokenAuthenticationHandler.cs` | Modify | Web authentication boundary | `Authorization` bearer invokes the same token/provider policy | Principal/no-principal mapping is consistent |
| `AuthBrowserTransport.cs` | Modify | Web transport | Cookie/CSRF/origin classification and secure cookie flags | Matrix tests pass |
| `AuthMinimalApis.cs` | Modify | Web HTTP | Transport proof completes before mediator; safe one-time error mapping | No duplicate exception/response handling or mutation on failed proof |
| `RegistrationService.cs` and existing Auth services | Modify only at transport/session seams | SL orchestration | Existing session lifecycle remains source of refresh semantics | Regression tests pass |
| `AuthInfrastructureTests.cs`, `AuthBrowserTransportTests.cs`, `AuthHttpIntegrationTests.cs`, `AuthApiTestData.cs` | Modify | Unit/integration verification | Covers every behavior path and compilation fix | Exact totals and limitations recorded |
| `appsettings.json` | Modify references/defaults only | Host configuration | No private key material committed | Secret/wildcard scan clean |

## Coverage scenarios

| Path | Required evidence |
|---|---|
| Issuance success | Active protected RSA key emits RS256 JWT with `kid`, issuer, audience, canonical `sub`, `sid`, `iat`, `nbf`, `exp`, 30-minute expiry. |
| Startup/config failure | Missing reference, unusable/non-RSA key, invalid issuer/audience/lifetime/skew/rotation/overlap/cookie/origin/invalidation config prevents startup safely. |
| Validation success | Active key and all required claims establish expected principal in sync handler and async service paths. |
| Invalid token | Malformed, expired, not-yet-valid, unsecured, wrong algorithm/key, invalid issuer/audience, missing claims, invalid `sid`, noncanonical/nonpositive/overflow `sub` produce no principal. |
| Durable invalidation | Current and fresh provider instances reject invalidated active/previous keys; issuance cannot use invalidated active state. |
| Rotation success/no-op | Before 180 days no rotation; at/after 180 days one durable transition occurs; repeated calls are no-op after winner. |
| Rotation conflict/dependency failure | Concurrent rotation has one winner; provider/store failure does not issue a token or create split-brain metadata. |
| Previous overlap | Previous key passes strictly before 48-hour end and fails at/after end, with `RetiredAtUtc` authoritative. |
| Empty/not-found | Unknown key/reference or no usable active key fails safely; no process-local fallback. |
| Duplicate exception handling | Expected token/provider failures map once to no principal/safe response; unexpected failure is redacted once; cancellation propagates. |
| Compilation | `AuthInfrastructureTests` resolves `AuthSecurityOptions` and test project builds. |
| Browser registration/login/refresh | Access uses bearer response contract; browser refresh appears only in secure cookie; CSRF cookie is readable only as required for proof. |
| Legacy refresh | Body-only compatibility is retained or explicitly rejected according to inspected caller evidence; no silent breaking change. |
| Ambiguous transport | Cookie plus body is rejected (planned default) before mediator/session mutation. |
| CSRF/origin failure | Missing/mismatched proof, absent/untrusted origin, and cross-site mutation return safe 403/400 before state changes. |
| Replay/concurrency/revocation | Existing old-token replay, family revocation, expiry, logout, and concurrent-use protections remain green. |
| Redaction | No password, refresh/access token, key material, reference secret, exception/inner exception, SQL, or stack data in responses/logs. |
| Integration/regression | Existing register/bootstrap/refresh/logout/verification and task-1/2 tests remain green; no frontend files changed. |

## Validation commands and expected evidence

Run only after explicit approval and implementation; this plan itself runs no production tests.

1. `git -C D:\RND\ResumeEnhancer\.worktrees\gh-36-authentication-authorization-hardening status --short` and scoped diff: only the approved Auth/composition/migration/test/config paths are changed; prior user changes remain.
2. `rg -n "AuthSecurityOptions|JsonWebTokenHandler|RsaSha256|IAuthSigningKeyProvider|RetiredAtUtc|InvalidatedAtUtc|RefreshCookie|TrustedOrigins|X-CSRF" application test`: one options/provider path, no duplicate local key authority.
3. `dotnet build application\Modules\AuthModule\AuthModulePL\ResumeEnhancer.AuthModule.PL.csproj --no-restore` and WebSolution server equivalent: compile evidence; report JavaScript SDK/NuGet blockers separately.
4. `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore --filter "FullyQualifiedName~Modules.AuthModule" --logger "console;verbosity=normal"`: exact totals for token/key/options/provider/transport and the compile fix.
5. `dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore --filter "FullyQualifiedName~Modules.AuthModule" --logger "console;verbosity=normal"`: exact HTTP totals for registration/login/refresh/logout/replay/cookie/CSRF/concurrency. Distinguish existing-artifact `--no-build` execution from clean rebuild.
6. Full commands from `AGENTS.md` where feasible, with known malformed client `.esproj`, JavaScript SDK, NuGet ACL, or restore blockers reported rather than hidden.
7. If model changed, inspect generated migration/snapshot for additive non-secret operations; do not apply migration.
8. `openspec validate --changes gh-36-authentication-authorization-hardening` or repository-supported strict equivalent, plus `git diff --check` and a scoped secret/log/response review. OpenSpec validation is artifact evidence, not runtime proof.

## Rollback/recovery and OpenSpec completion evidence

Rollback is deployment-level: disable the new bearer/cookie behavior while retaining additive durable metadata and existing session rows; do not reset, stash, delete, apply destructive migrations, restore HMAC acceptance, or create process-local keys. For compromise, invalidate the durable key and revoke affected session families. If protected-store access, provider concurrency, or legacy consumer evidence changes the boundary, stop and return for approval.

Task 3.1 completion evidence is RS256 issuance/validation output and all positive/negative claim, algorithm, key, time, canonical-subject, sync/async, and safe-failure tests. Task 3.2 completion evidence is startup usable-key validation, protected loading, durable active/previous metadata, exact 180/48 boundaries, fresh-instance invalidation, rotation conflict proof, and no-secret scan/migration result. Task 3.3 completion evidence is HTTP registration/login/refresh/logout evidence for bearer access, cookie flags, explicit body/cookie classification, CSRF/trusted-origin rejection, rotation/replay/revocation/concurrent use, and compatibility. The implementation owner/coordinator, not this planner, updates OpenSpec checkboxes after those results.

## Unresolved decisions and exact approval required

1. **Protected binding:** approve use of the repository-supported protected configuration/secret-store mechanism without selecting a vendor/KMS or inventing a binding name. Any external provider or EF-stored private material requires replanning.
2. **Rotation transaction owner:** approve Auth PL repository/UoW as the durable atomic owner. A second context, distributed lock, or cross-module transaction requires replanning.
3. **Sync validation seam:** approve a provider snapshot/authority contract that prevents sync validation from bypassing durable invalidation/retirement. Remote blocking or stale local-only fallback requires replanning.
4. **Legacy body transport:** approve retaining body-only refresh only where current non-browser caller evidence confirms it, with browser responses cookie-only; approve rejecting both cookie+body as ambiguous. Removing body compatibility or preserving refresh tokens in browser JSON requires a user decision.

Exact approval requested: approve this Proposed plan for `gh-36-authentication-authorization-hardening`, tasks 3.1, 3.2, and 3.3, as one serialized backend batch in the supplied canonical worktree, under the four decision rules above.

## Assumptions, unknowns, conflicts, rejected alternatives, and deferred decisions

- **Confirmed:** task identity/text, OpenSpec scope, module ownership, endpoint-purpose preservation, RS256, 30-minute lifetime, 30-second skew, 180-day rotation, 48-hour overlap, protected application-owned keys, and no frontend work.
- **User-supplied:** Harvey corrective findings and required issue list on 2026-09-16; no separate Harvey report file was found in the inspected worktree.
- **Inferred:** tasks 3.1–3.3 must be one batch because the token provider, options, response transport, and bearer/cookie composition share seams.
- **Unknown:** exact protected secret binding, whether current metadata supports an atomic active pointer/rotation lease, and whether external non-browser refresh callers still exist. These are implementation-gate checks, not assumptions to code around.
- **Conflict:** current code has PEM/options authority plus optional durable metadata and an empty sync invalidation method; this plan resolves authority toward one validated provider path and durable lifecycle state.
- **Rejected:** external KMS selection, private keys in EF/source/logs/responses, random/process-local fallback, indefinite overlap, local-only invalidation, HMAC/dual-algorithm acceptance, silent breaking removal of legacy body refresh, endpoint redesign, frontend changes, parallel shared-contract ownership, and generated workflow edits.
- **Deferred:** task 4 authorization boundary, task 5 broader account/abuse/error/audit controls, task 6 final acceptance sweep, and concrete per-environment origin/reference names.

## Complete visible evidence ledger

| Source | Date/version | Observation | Interpretation | Confidence | Limitation | Effect on plan |
|---|---|---|---|---|---|---|
| User request and Harvey findings | 2026-09-16 | Requires durable provider, real rotation/overlap, authoritative sync/async validation, startup usability, legacy compatibility, duplicate exceptions, and test compile correction | Current corrective acceptance input | High | Review text is supplied in context, not stored locally | Exact corrective steps and approval decisions |
| `AGENTS.md` | current canonical worktree | Auth Web/AM/SL/PL ownership; ModulesComposition host boundary; no implementation before approved plan | Repository authority | High | Does not choose secret binding | Disjoint ownership and gates |
| `KnowledgeBase/INDEX.md` | last reviewed 2026-08-23 | Routes API, EF, persistence, modular architecture, and ADR authorities | Retrieval authority | High | Index is routing only | Selected evidence set |
| API/application knowledge | 2026-08-23 | Additive contracts, boundary validation, safe error distinction, unit/integration boundary | API authority | High | Generic framework guidance | Compatibility and test strategy |
| EF/persistence knowledge | 2026-08-23 | Explicit concurrency/transaction/migration review; relational proof for provider semantics | Persistence authority | High | Generic; key provider is project-specific | Atomic rotation and conditional migration |
| persistence-project knowledge | 2026-08-15 | Shared AppDbContext/UoW; module PL owns repository/schema behavior; no module secrets in shared persistence | Project persistence authority | High | Does not define signing provider | Reuse existing context and PL ownership |
| modular architecture knowledge + ADR-001/002 | 2026-08-23 / accepted 2026-08-16 | Narrow contracts, composition boundary, cross-module ownership | Architecture authority | High | No cookie specifics | No Profiling persistence coupling; one Auth lane |
| OpenSpec `proposal.md`, `design.md`, `tasks.md`, delta spec | current canonical worktree | Exact task and acceptance behavior; protected keys, RS256, 180/48, secure cookies, compatibility | Behavioral authority | High | Binding names remain open | Scope, exclusions, evidence |
| `AuthSecurityOptions.cs` | current worktree | Security fields and validator exist, including 180/48 values and cookie/origin fields | Concrete options seam | High | Usability validation not demonstrated | Consolidate and startup-validate |
| `AuthSecurityServices.cs` / `AuthTokenService` | current worktree | RS256 handler exists but local PEM, optional durable metadata, empty sync invalidation, broad async catch remain | Concrete corrective gap | High | Runtime results not rerun in this planning pass | Provider and unified validation steps |
| `AuthSigningKeyMetadata.cs`, repository, migration | current worktree | Non-secret lifecycle columns exist; active/rotation authority not proven | Reuse candidate, not acceptance proof | Medium-high | Schema/concurrency sufficiency unknown | Conditional migration and fresh-instance tests |
| `AuthBrowserTransport.cs`, `AuthMinimalApis.cs`, registration service | current worktree | Cookie helper/classification exists; body/cookie and proof/error path remains incomplete/ambiguous | Transport corrective seam | High | Consumer inventory pending | Explicit compatibility matrix and pre-mutation checks |
| Auth unit/browser/HTTP tests | current worktree | Partial JWT, overlap, invalidation, cookie, CSRF, and classification tests; missing options namespace import | Existing verification seam plus compile defect | High | Full current totals not run | Extend focused tests and report exact totals |
| Existing plans in `.tmp/ImplementationPlans/...` | current worktree | Prior plans establish task-1/2 baseline and earlier corrective concerns; must be preserved | Historical/local planning evidence | Medium-high | Some are approved and may reflect earlier review names | Avoid overlap and preserve changes |
| `workflow-planning`, `openspec-repository-policy`, `workflow-development-entry` | current repository | Evidence-first planning, mandatory approval, canonical worktree, implementation hard gate | Workflow authority | High | No runtime proof | Proposed-only artifact and handoff gate |

## Self-review record

Independent self-review completed 2026-09-16. Every requested Harvey finding maps to a concrete step, file/symbol seam, scenario, and validation command. Ownership is serialized where options/provider/contracts/migration/composition overlap and disjoint by layer elsewhere. The plan preserves endpoint purposes and task-1/2 work, makes secret storage and sync authority explicit, covers success/validation/not-found/conflict/empty/dependency-failure/result mapping/persistence/integration paths, identifies the current test compile defect, and forbids production edits before approval. Remaining limitations are the unavailable local Harvey report, unknown deployment binding, unverified current consumer inventory, and unexecuted runtime totals; each is visible and has an implementation-gate action.

## Approval status and next safe action

Status remains **Proposed**. The next safe action is for the user to review and explicitly approve this exact plan and all three exact OpenSpec task identities. Until approval metadata is added by the authorized workflow, no implementation agent may edit code, tests, migration, configuration, or OpenSpec checkboxes.
