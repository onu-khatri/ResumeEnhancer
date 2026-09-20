## Context

The current application targets .NET 10 and has AuthModule registration/session behavior plus ProfilingModule-owned access profiles and roles. The repository evidence shows no production `.AllowAnonymous()` route usage, no configured JWT authentication middleware, and existing identity-header-based authorization paths. Issue #36 depends on AUTH-BE-001 (#33) and must preserve its existing endpoint contracts.

## Goals / Non-Goals

**Goals:**

- Establish one framework-integrated authentication boundary before authorization.
- Keep guest access explicit and data-driven through endpoint metadata and the guest access profile.
- Preserve module ownership: AuthModule owns authentication/session behavior; ProfilingModule owns users, roles, and access profiles.
- Add secure password/account lifecycle state, JWT key rotation, browser token transport, abuse controls, and redacted security observability.
- Keep the implementation testable across unit, integration, and multi-instance session behavior.

**Non-Goals:**

- Frontend implementation, social login, SSO, external identity providers, or MFA.
- Recreating or semantically redesigning existing registration, refresh, logout, verification-resend, or bootstrap endpoints.
- Moving user, role, or access-profile ownership into AuthModule.

## Decisions

1. **Use framework bearer authentication with `JsonWebTokenHandler`.** The access-token contract is JWT and the implementation will use `Microsoft.IdentityModel.JsonWebTokens`; custom HMAC text tokens are not extended. This gives standard claim validation and key identifiers while preserving the existing application boundary.

2. **Use application-managed rotating RSA keys.** New tokens use `RS256`; protected appsettings/secret storage supplies the active key, and the previous key remains valid only for the configured 48-hour overlap during the 180-day rotation cadence. A managed external KMS remains a future deployment option but is not part of this issue.

3. **Make authorization fail closed with endpoint metadata.** Endpoints explicitly opt into anonymous access with `.AllowAnonymous()`. A custom endpoint metadata attribute carries guest access-profile and allowed-role requirements. Middleware/policy evaluation checks this metadata against the guest profile and then delegates authenticated authorization to principal claims and resource checks. This keeps route declarations close to the endpoint while preventing client headers from becoming identity.

4. **Use bearer access plus cookie refresh transport.** Access JWTs travel in `Authorization`; refresh tokens use secure `HttpOnly`, `Secure`, `SameSite` cookies. Cookie-authenticated mutations require CSRF proof and explicit environment-configured trusted origins. This limits JavaScript token exposure while making browser cross-site mutation checks explicit.

5. **Keep lifecycle state durable and revocable.** Lockout, password history, reset challenges, verification challenges, and refresh-session families are persisted through AuthModule persistence and revoked transactionally where required. ProfilingModule remains the source for access profiles and roles through a narrow integration contract.

6. **Persist challenge purposes as Auth setup data.** `AuthChallenge` references an Auth-owned `AuthChallengePurpose` setup entity by foreign key. Stable purpose codes such as `password-reset` and `email-verification` live on seeded setup rows; the challenge table does not duplicate them as a free-form string. Application contracts may accept a purpose code at the boundary, but the Auth service/repository must resolve and validate it against an active setup row before persistence or consumption.

7. **Configure login lockout policy through appsettings.** `ProgressiveLoginDelayPolicy` and the lockout window are constructed from one validated Auth security options section. The recommended defaults are five failures, a 15-minute lockout window, and the existing progressive delay schedule; deployments may override the threshold, window, and delay time spans through environment-specific appsettings without code changes. Invalid, zero, negative, or overflowed values fail startup/options validation.

6. **Use safe, stable failure contracts.** Authentication failures are enumeration-safe and all sensitive failures use redacted ProblemDetails. Security audit events contain event classification and safe context but no credential or token material.

## Risks / Trade-offs

- **[Risk]** Application-owned signing keys can be exposed if deployment secret handling is weak → require protected configuration/secret storage, startup validation, no logging, and rotation tests.
- **[Risk]** A 48-hour key overlap permits a recently compromised previous key to validate → keep overlap bounded, support emergency revocation, and audit key use.
- **[Risk]** Guest metadata can accidentally broaden anonymous access → require explicit `.AllowAnonymous()`, validate metadata at startup/tests, and default all unspecified endpoints to authenticated access.
- **[Risk]** Cookie refresh transport introduces CSRF complexity → enforce SameSite/Secure/HttpOnly settings, trusted-origin checks, anti-CSRF proof, and integration tests for cross-site mutations.
- **[Risk]** Distributed throttling and lockout state can diverge across instances if the provider is unavailable → fail safely, expose degradation through redacted operational telemetry, and cover multi-instance/concurrency behavior.
- **[Risk]** Existing clients may depend on the custom token shape → preserve endpoint purposes and provide the approved JWT authentication response contract with explicit regression coverage.

## Migration Plan

1. Add schema and persistence changes for lifecycle state and session/key metadata while preserving AUTH-BE-001 data and endpoint contracts.
2. Introduce token validation and issuance behind the existing AuthModule/WebSolution composition boundary.
3. Add explicit guest metadata to approved routes and protect unspecified routes through the fallback policy.
4. Enable durable lockout, challenge, password-history, refresh revocation, CSRF, origin, and distributed-limit behavior.
5. Run regression and security integration tests, then deploy with configured issuer, audience, origins, key material, and rotation settings.
6. Roll back by disabling the new deployment while retaining additive schema data; do not delete session or challenge records during rollback. Revoke compromised key/session families through the operational controls.

## Open Questions

- The concrete per-environment origin values and secret-provider binding names can be supplied during deployment configuration without changing the approved behavior or design.
