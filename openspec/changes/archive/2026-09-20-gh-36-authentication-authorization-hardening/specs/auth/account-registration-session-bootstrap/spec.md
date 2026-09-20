## MODIFIED Requirements

### Requirement: Shared bearer sessions support multi-instance operation
The system MUST issue standards-compliant JWT access tokens and secure refresh tokens using shared signing configuration. Access tokens MUST use the configured issuer and audience, `RS256`, a 30-minute lifetime, 30-second clock skew, and complete validation of signature, algorithm, subject, session identifier, `nbf`, `iat`, and `exp`. Refresh-session state MUST be database-backed, revocable, expiry-aware, rotation-safe, and safe to use across multiple application container instances. Browser refresh tokens MUST use a `HttpOnly`, `Secure`, `SameSite` cookie, and sensitive tokens MUST NOT be persisted in browser local storage or logs.

#### Scenario: Registration issues a usable session
- **WHEN** account creation commits successfully
- **THEN** the response returns the approved session state, issues a valid JWT access token, and establishes the secure refresh-token transport that can be validated by any configured application instance

#### Scenario: Refresh session is revoked or expired
- **WHEN** a refresh request presents an expired, revoked, rotated, replayed, or unknown session
- **THEN** the system rejects it with a stable authentication error, does not issue new credentials, and records a safe security event

#### Scenario: Existing registration contract remains stable
- **WHEN** a client uses the existing registration or bootstrap flow
- **THEN** the endpoint purpose and response compatibility remain intact while the shared session credentials use the approved JWT and secure refresh transport
