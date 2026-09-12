## Why

`AUTH-BE-001` needs a secure, retry-safe registration workflow that creates a usable account and shared session across multiple application instances. The current story has approved product decisions but no implementation contract for registration, consent evidence, token sessions, bootstrap continuation, or recoverable side effects.

## What Changes

- Introduce a dedicated `AuthModule` for registration and authenticated runtime concerns.
- Integrate account and access-profile creation with `ProfilingModule` through explicit contracts.
- Add email/password registration with normalization, validation, throttling, consent capture, default entitlement assignment, and audit events.
- Add bearer access/refresh-token sessions with database-backed refresh-session state for multi-container continuity.
- Add source-aware bootstrap response data and recoverable verification-email side effects.
- Add stable validation, conflict, throttling, degradation, and delayed-verification error/state contracts.
- Amend or supersede `ADR-001` to record the approved `AuthModule` boundary while preserving `ProfilingModule` ownership of user and access-profile data.

## Capabilities

### New Capabilities

- `auth/account-registration-session-bootstrap`: Registration, consent evidence, shared token sessions, entitlements, bootstrap routing, and recoverable onboarding side effects.

### Modified Capabilities

- None.

## Impact

- New AuthModule domain, application, persistence, and HTTP boundaries.
- ProfilingModule integration for users, roles, claims, access profiles, and preferences.
- Shared database schema, migrations, transaction boundaries, session signing configuration, and background side effects.
- Registration and bootstrap API contracts consumed by the React client.
- Unit, integration, security, concurrency, and failure-recovery tests.
