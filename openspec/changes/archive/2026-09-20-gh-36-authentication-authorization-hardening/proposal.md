## Why

Issue #36 needs to close the current authentication boundary gaps: login and password lifecycle operations are absent, the application does not establish a framework-validated JWT principal before authorization, and endpoint access is not fail-closed. This is required to protect private resume, profile, billing, administrative, and mutation operations while preserving explicit guest access.

## What Changes

- Add login, password change, password recovery, verification completion, and authenticated-subject flows without recreating or redesigning existing registration, refresh, logout, verification-resend, or bootstrap contracts.
- Replace custom access-token issuance with standards-compliant `RS256` JWTs created and validated with `Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler`.
- Use application-owned RSA key storage and scheduled 180-day rotation with configurable 48-hour previous-key validation overlap.
- Configure a 30-minute access-token lifetime, 30-second clock skew, configured issuer/audience, and complete token-claim validation.
- Establish authentication before authorization and apply a fail-closed authorization boundary.
- Preserve explicit `.AllowAnonymous()` guest routes; add endpoint metadata for guest access-profile and role requirements, enforced by authorization middleware.
- Derive subject and audit identity from authenticated principals, not client-controlled headers.
- Add account-state enforcement, five-failure/15-minute lockout with progressive delay, previous-two password history, hashed single-use challenges, safe errors, auditing, and distributed abuse controls.
- Keep access JWTs in `Authorization`; use secure `HttpOnly`, `Secure`, `SameSite` refresh cookies with CSRF protection for cookie-authenticated mutations.
- Configure environment-specific trusted origins with no wildcard.

## Capabilities

### New Capabilities

- `auth/authentication-authorization-hardening`: Login, JWT validation, fail-closed guest/private authorization, account lifecycle security, password recovery, abuse controls, and redacted security errors.

### Modified Capabilities

- `auth/account-registration-session-bootstrap`: Preserve existing registration/bootstrap/session contracts while aligning shared bearer-session behavior with JWT access tokens, secure refresh-cookie transport, revocation, and regression guarantees.

## Impact

- AuthModule AM, DM, SL, PL, and Web layers; WebSolution server authentication/authorization composition; ProfilingModule integration for access profiles and roles; shared exception/error and rate-limit infrastructure.
- New or changed database records and migrations for account lockout, password history, reset/verification challenges, and session/key metadata as required by the approved design.
- Existing Auth and registration integration tests plus new unit/integration security coverage.
- No frontend implementation and no semantic redesign of the existing protected Auth endpoints.
- Issue: https://github.com/onu-khatri/ResumeEnhancer/issues/36
- Source artifact: `.tmp/artifacts/issues/36.md`
