# auth/account-registration-session-bootstrap Specification

## Purpose
Provides secure, retry-safe account registration and first-session bootstrap so a new user can begin work consistently across application instances.

## Requirements

### Requirement: Registration validates and normalizes identity input

The system MUST accept the registration identity, password, consent choices, and supported source context. It MUST trim supported names, normalize email before conflict checks and persistence, enforce password and required-consent rules, reject unsupported source values, and apply throttling by identifier and request origin.

#### Scenario: Valid registration input
- **WHEN** a request contains valid identity, password, required consent, and supported source data
- **THEN** the system accepts the normalized values for registration processing

#### Scenario: Invalid or throttled request
- **WHEN** the request has malformed data, missing required consent, an unsupported source, a weak password, or exceeds abuse thresholds
- **THEN** the system returns a stable field-addressable validation or throttling code and creates no account data

### Requirement: Registration creates an atomic account and entitlement baseline

The system MUST create the account, authentication identity, separate consent records, preferences, default plan/entitlements, and initial session state atomically for the synchronous registration workflow. Normalized email uniqueness MUST be enforced under concurrent requests.

#### Scenario: Account creation succeeds
- **WHEN** all transactional registration operations are valid
- **THEN** the system commits one account with its identity, consent evidence, preferences, default entitlement baseline, and session state

#### Scenario: Duplicate or concurrent registration
- **WHEN** the normalized email is already claimed or two equivalent requests race
- **THEN** the system returns a stable conflict or consistent idempotent result and does not create duplicate account, consent, entitlement, or session records

### Requirement: Consent evidence is independently auditable

The system MUST store Terms, Privacy, and Marketing consent as separate records. Legal consent versions MUST be resolved by the server, and each record MUST preserve consent type, state, version, timestamp, applicable request context, and audit linkage without storing raw legal text.

#### Scenario: Legal consent accepted and marketing declined
- **WHEN** a user accepts required legal documents and declines optional marketing
- **THEN** the system stores independently queryable records for each consent type with the exact server-resolved versions and states

### Requirement: Shared bearer sessions support multi-instance operation

The system MUST issue bearer access and refresh tokens using shared signing configuration. Refresh-session state MUST be database-backed, revocable, expiry-aware, and safe to use across multiple application container instances. Sensitive tokens MUST NOT be persisted in browser local storage or logs.

#### Scenario: Registration issues a usable session
- **WHEN** account creation commits successfully
- **THEN** the system returns the approved session state and issues access/refresh credentials that can be validated by any configured application instance

#### Scenario: Refresh session is revoked or expired
- **WHEN** a refresh request presents an expired, revoked, rotated, or unknown session
- **THEN** the system rejects it with a stable authentication error and does not issue new credentials

### Requirement: Bootstrap preserves source-aware continuation

The system MUST return a compact bootstrap contract containing session state, email-verification state, default plan code, workspace route, source context, selected template when applicable, next action, entitlement summary, and recovery/degradation information. It MUST use a safe default route when continuation calculation degrades.

#### Scenario: Source-aware bootstrap succeeds
- **WHEN** registration originates from a supported homepage, pricing, or template context
- **THEN** the response contains the originating context and the corresponding approved continuation route or next action

#### Scenario: Bootstrap continuation degrades
- **WHEN** the selected continuation is unavailable or cannot be calculated
- **THEN** the system returns a usable account/session response with a safe default workspace route and recoverable degradation state

### Requirement: Non-critical verification side effects are recoverable

The system MUST queue verification and welcome side effects asynchronously. Queue failure MUST NOT roll back a committed account or session, MUST be logged without secrets or full legal text, and MUST leave a resend path and machine-readable delayed-verification state.

#### Scenario: Verification queue is unavailable
- **WHEN** account and session persistence succeeds but verification enqueue fails
- **THEN** the account remains usable, the response identifies delayed verification, the failure is retryable, and resend remains available

### Requirement: Entitlements resolve through subscription-owned access profiles

The system MUST resolve effective bootstrap entitlements through the BillingModule subscription, the ProfilingModule access profile attached to that subscription, and the roles attached to that access profile. `UserEntitlement` and `UserPreference` MUST be owned and persisted by the ProfilingModule. Each `UserEntitlement` MUST represent one subscription/access-profile grant and reference its `User`, granting `BillingSubscription`, and `AccessProfile` by foreign key. The source `BillingPlan` MUST be reached through the granting subscription rather than duplicated on the entitlement. A user MAY have multiple entitlement rows for multiple active subscriptions, and effective capabilities MUST be the union of the attached access-profile roles. Each seeded ProfilingModule `Role` MUST persist a stable capability identifier in the role setup data and the resolver MUST read it from the database rather than maintain a duplicate capability constant list. Identifiers MUST include `TemplateView`, `ResumePublish`, `PdfExport`, `ResumeCreate`, and `PremiumTemplates`. AuthModule MUST consume narrow ProfilingModule and BillingModule integration contracts and MUST NOT access another module's repository, `DbContext`, or persistence entity directly. Unknown or unavailable subscriptions/profiles MUST resolve to the safe starter profile; HTTP handlers MUST NOT contain subscription-specific checks.

#### Scenario: Subscription resolves capabilities
- **WHEN** a registered user has an active subscription with an attached access profile
- **THEN** bootstrap and authorization receive the effective capabilities from that profile's roles

#### Scenario: Unknown subscription falls back safely
- **WHEN** subscription or access-profile resolution is unavailable or unknown
- **THEN** the user receives the safe starter capability set and a recoverable fallback state without failing registration

### Requirement: Security events and errors are observable without secret leakage

The system MUST audit account creation, consent acceptance, session creation, conflicts, throttling, and relevant recovery events. It MUST distinguish validation, conflict, throttling, operational degradation, and delayed-side-effect outcomes without logging plaintext passwords, bearer tokens, or full legal text.

#### Scenario: Suspicious attempts exceed limits
- **WHEN** repeated registration attempts exceed configured thresholds
- **THEN** the system throttles the request and records an auditable security event without sensitive request contents
