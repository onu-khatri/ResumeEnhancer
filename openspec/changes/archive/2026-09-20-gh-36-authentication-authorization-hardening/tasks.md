## 1. Contracts and dependencies

- [x] 1.1 Add login, password-change, forgot/reset, verification-completion, authenticated-subject, and safe-authentication response contracts in AuthModule AM; verify request validation and response tests cover normalization, redaction, and stable error codes.
- [x] 1.2 Add `Microsoft.IdentityModel.JsonWebTokens` and required bearer/authentication dependencies to the owning projects; verify package restore and the solution build resolve the approved .NET 10 API surface. Targeted AuthModule PL restore/build passed; full solution baseline remains blocked by the existing JavaScript SDK/NuGet.Config environment failure.
- [x] 1.3 Extend the AuthModule integration ports for ProfilingModule user state, guest access profile/role resolution, and account status without referencing ProfilingModule persistence directly; verify project dependency direction and contract tests.

## 2. Durable authentication state

- [x] 2.1 Add persistence models/configuration for lockout state, password history, reset challenges, verification challenges, and required session/key metadata; verify EF model constraints, protected-hash storage, indexes, and unit persistence tests.
- [x] 2.2 Create and validate the EF migration from the current migration project; verify migration generation/build and schema snapshot consistency without altering existing registration/session data semantics.
- [x] 2.3 Implement transactional password history, challenge consumption, session-family revocation, and five-failure/15-minute lockout with progressive delay; verify concurrency, replay, expiry, and previous-two-password tests.

## 3. JWT and browser session security

- [x] 3.1 Replace custom access-token text/HMAC issuance with `JsonWebTokenHandler` and `RS256` JWT issuance/validation using configured issuer, audience, subject, session ID, 30-minute lifetime, and 30-second clock skew; verify valid, malformed, expired, unsecured, wrong-algorithm, wrong-key, and invalid-claim tests.
- [x] 3.2 Implement protected application-owned RSA key loading, key identifiers, configurable 180-day rotation, 48-hour previous-key overlap, and emergency invalidation behavior; verify key material is absent from logs/responses and rotation overlap tests pass.
- [x] 3.3 Preserve refresh rotation/replay detection while moving browser refresh transport to `HttpOnly`, `Secure`, `SameSite` cookies and access transport to `Authorization`; verify registration, login, refresh, logout, revocation, and concurrent-use integration tests.

## 4. Authentication and authorization boundary

- [x] 4.1 Configure authentication before authorization and a fail-closed fallback policy in WebSolution.Server; verify protected endpoints return `401`, authorized endpoints establish `HttpContext.User`, and existing approved anonymous contracts remain reachable.
- [x] 4.2 Add the custom guest access-profile/role endpoint metadata and middleware/policy evaluation for `.AllowAnonymous()` routes; verify the guest role allow/deny matrix and startup/endpoint metadata checks.
- [x] 4.3 Replace authoritative `X-User-Id` and `X-Audit-UserId` reads with principal-derived identity and enforce roles, capabilities, policies, entitlements, and resource ownership; verify forged-header, `401`, `403`, and ownership integration tests.
- [x] 4.4 Add explicit anonymous/protected metadata to the existing Auth, bootstrap, discovery, resume, profile, billing, administration, and mutation routes while preserving endpoint purpose; verify route inventory and anonymous/private endpoint tests.

## 5. Account lifecycle, abuse, and error handling

- [x] 5.1 Implement login, password change, forgot/reset, verification completion, and authenticated-me routes with enumeration-safe behavior and account-state enforcement; verify active, unverified, disabled, deleted, locked, unknown, and invalid-credential scenarios.
- [x] 5.2 Replace process-local registration-only throttling with distributed limits for login, registration, refresh, verification, password recovery/change, and relevant logout; verify safe `429` responses, progressive delay, and multi-instance behavior.
- [x] 5.3 Implement redacted ProblemDetails/ValidationProblemDetails and structured security auditing for failures, denials, replay, reset, verification, lockout, disablement, and throttling; verify no secrets, tokens, passwords, inner exceptions, SQL details, or stack traces appear.
- [x] 5.4 Configure explicit environment-specific trusted origins and CSRF protection for cookie-authenticated mutations; verify no wildcard origin is accepted and cross-site mutation tests fail safely.
- [x] 5.5 Migrate obsolete Auth clock APIs to `System.TimeProvider` and use `Microsoft.Extensions.Time.Testing.FakeTimeProvider` for deterministic Auth time-sensitive tests; preserve UTC semantics and existing contracts.

## 6. Verification and regression

- [x] 6.1 Add focused AuthModule unit tests for token validation, key rotation, password history, lockout, challenge expiry/reuse, redaction, metadata, and identity derivation; verify `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore` passes for the affected tests.
- [x] 6.2 Add Auth, WebSolution, guest-access, ownership, cookie/CSRF, refresh replay, rate-limit, and account-state integration coverage; verify `dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore` passes for the affected tests. Final evidence: production Auth integration 39/39 passed; full integration 58/58 passed with the client project excluded; real RS256, current-state, challenge delivery, replay/expiry, CSRF, CORS, and raw-secret coverage completed.
- [x] 6.3 Run the backend-scoped build and test verification, OpenSpec validation, and acceptance-criteria review; verify existing registration/bootstrap/refresh/logout/verification regression tests remain green and no frontend files changed. Backend acceptance evidence: backend server build with `-p:SkipClientProjectReference=true` passed with 0 warnings and 0 errors; unit tests 382/382 passed; full backend integration tests 58/58 passed; production Auth integration 39/39 passed; OpenSpec validation 2/2 passed; no frontend files changed. The unrelated client-project `Microsoft.VisualStudio.JavaScript.Sdk`/NuGet.Config limitation is excluded from this backend-only task.
