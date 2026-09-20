# auth/authentication-authorization-hardening Specification

## Purpose

Provides a fail-closed authentication and authorization boundary with secure
login, account recovery, guest access, and security controls for private
ResumeEnhancer operations.

## Requirements

### Requirement: Authentication issues and validates secure access tokens

The system MUST authenticate login credentials without account enumeration,
create a refresh-session family only after successful validation, and issue an
access JWT that validates configured issuer, audience, signature, algorithm,
subject, session identifier, `nbf`, `iat`, `exp`, and bounded clock skew.
Access tokens MUST expire after the configured 30-minute lifetime.

#### Scenario: Valid login
- **WHEN** an active, verified, non-disabled, non-locked account submits valid normalized credentials
- **THEN** the system creates a refresh-session family and returns a safe authentication response containing a valid access JWT

#### Scenario: Invalid or unavailable account
- **WHEN** credentials are invalid or the account is unknown, unverified, disabled, deleted, or locked
- **THEN** the system returns an enumeration-safe authentication failure and creates no session

#### Scenario: Invalid token
- **WHEN** a bearer token is malformed, expired, unsecured, algorithm-invalid, incorrectly signed, outside its time bounds, or has invalid issuer, audience, subject, or session claims
- **THEN** the system rejects it and does not establish an authenticated principal

### Requirement: Protected endpoints fail closed and guest access is explicit

The system MUST require authentication for endpoints without explicit
anonymous metadata. Guest routes MUST opt in with `.AllowAnonymous()`.
Guest-authorized endpoint metadata MUST declare the required access profile and
allowed role, and authorization middleware MUST allow the request only when the
guest access profile contains the required role. Authenticated authorization
MUST enforce applicable role, capability, policy, entitlement, and
resource-ownership requirements.

#### Scenario: Anonymous guest route
- **WHEN** an unauthenticated request targets an endpoint explicitly marked `.AllowAnonymous()` and its guest metadata is satisfied
- **THEN** the request is allowed

#### Scenario: Anonymous private route
- **WHEN** an unauthenticated request targets an endpoint without explicit anonymous metadata
- **THEN** the system returns `401`

#### Scenario: Guest role is not allowed
- **WHEN** an anonymous request targets a guest-authorized endpoint whose required role is not part of the configured guest access profile
- **THEN** the system returns `403` and does not execute the handler

#### Scenario: Authenticated user lacks authorization
- **WHEN** an authenticated principal lacks the required policy, role, capability, entitlement, or ownership
- **THEN** the system returns `403`

### Requirement: Trusted principal identity controls protected behavior

The system MUST derive user and audit actors from the authenticated principal
and MUST ignore or reject client-controlled identity headers as authoritative
identity sources.

#### Scenario: Forged identity header
- **WHEN** a request supplies `X-User-Id` or `X-Audit-UserId` that differs from the authenticated subject
- **THEN** the system uses the authenticated subject or rejects the request and never impersonates the supplied identity

### Requirement: Account lifecycle and password recovery are secure

The system MUST enforce active, verified, non-disabled, and non-locked account
state during login, refresh, and protected access. It MUST lock an account after
the configured failed-attempt threshold for the configured lockout window and
apply the configured progressive delay thereafter. The recommended defaults are
five failed attempts and a 15-minute lockout window. Password change and reset
MUST preserve the 12-character baseline, reject either of the previous two
password hashes, store password history and challenges only as protected hashes,
use single-use expiring challenges, revoke applicable refresh-session families,
and return enumeration-safe recovery responses. Password reset challenges MUST
expire after 30 minutes and email-verification challenges after 24 hours. Each
persisted challenge MUST reference an Auth-owned challenge-purpose setup row
through a foreign key; the purpose code is the stable code on that setup row
and MUST NOT be persisted as a free-form string on the challenge. Lockout and
progressive-delay policy values MUST be bound from application configuration and
validated at startup.

#### Scenario: Lockout threshold
- **WHEN** an account reaches the configured failed-attempt threshold
- **THEN** the system locks the account for the configured lockout window and applies the configured progressive delay to subsequent attempts

#### Scenario: Password reuse
- **WHEN** a password change or reset matches the current password or either of the previous two password hashes
- **THEN** the system rejects the change and preserves the existing credential state

#### Scenario: Single-use reset challenge
- **WHEN** a valid reset challenge is consumed
- **THEN** the system changes the password transactionally, marks the challenge unusable, revokes applicable refresh sessions, and audits the event

#### Scenario: Setup-backed challenge purpose
- **WHEN** a reset or email-verification challenge is persisted
- **THEN** it references the active Auth challenge-purpose setup row by foreign key, and an unknown or inactive purpose cannot be persisted or consumed

#### Scenario: Unknown recovery account
- **WHEN** a forgot-password request targets an unknown email
- **THEN** the response is equivalent to a valid-account response and reveals no account existence

### Requirement: Browser token transport and abuse controls are safe

The system MUST return access JWTs for `Authorization` bearer transport and
use a `HttpOnly`, `Secure`, `SameSite` refresh-token cookie. Cookie-authenticated
mutations MUST use CSRF protection. CORS and CSRF MUST use an
environment-specific explicit trusted-origin allowlist with no wildcard.
Sensitive authentication operations MUST use distributed rate limits and
redacted ProblemDetails responses.

#### Scenario: Secure refresh transport
- **WHEN** registration, login, or refresh establishes a browser session
- **THEN** the access token uses the approved bearer contract and the refresh token is issued only through the secure cookie contract

#### Scenario: Cross-site mutation
- **WHEN** a cookie-authenticated mutation lacks a valid CSRF proof or originates outside the configured trusted origins
- **THEN** the system rejects it without executing the mutation

#### Scenario: Throttled sensitive operation
- **WHEN** login, registration, refresh, verification, password recovery, password change, or relevant logout exceeds its distributed limit
- **THEN** the system returns a safe throttling response and records a redacted security event

### Requirement: Signing-key rotation is protected and available across instances

The system MUST store application-owned RSA signing keys in protected
configuration or secret storage, use `RS256` for new access JWTs, rotate keys
every configurable 180 days, and accept previously active keys only during a
configurable 48-hour overlap. Key material MUST NOT be logged, committed, or
returned.

#### Scenario: Active-key issuance
- **WHEN** a new access token is issued
- **THEN** it uses the active RSA key and key identifier

#### Scenario: Rotation overlap
- **WHEN** a token was signed by the immediately previous approved key during the 48-hour overlap
- **THEN** the token validates if all other checks pass; after overlap it is rejected

### Requirement: Security failures are redacted and auditable

The system MUST return standardized redacted ProblemDetails or
ValidationProblemDetails and MUST NOT expose passwords, tokens, OTPs, secrets,
inner exceptions, SQL details, or production stack traces. It MUST emit
structured redacted audit events for login failures, denials, replay, reset,
verification, lockout, disablement, and throttling.

#### Scenario: Authentication failure response
- **WHEN** a protected authentication operation fails
- **THEN** the response contains a stable safe error contract without credential or infrastructure details

#### Scenario: Security audit event
- **WHEN** a security-sensitive event occurs
- **THEN** an audit event records the event type and safe context without credential material
