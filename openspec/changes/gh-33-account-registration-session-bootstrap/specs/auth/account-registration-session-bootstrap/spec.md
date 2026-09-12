## Purpose

Provide secure registration and correct subscription-driven access-profile assignment.

## ADDED Requirements

### Requirement: Registration creates a valid access-profile baseline

Registration MUST create the user, subscription-linked `UserAccessProfile`, authentication identity, consent, preferences, and session state atomically where synchronous registration owns those operations.

#### Scenario: Registration creates an access-profile baseline
- **WHEN** registration succeeds with a valid starter plan
- **THEN** the user receives one valid subscription-linked access-profile assignment

### Requirement: Billing plans select access profiles

Every billing plan MUST reference one active Profiling access profile. A subscription MUST reference its user and plan and MUST NOT own an access-profile relationship.

#### Scenario: Subscription derives access from its plan
- **WHEN** a user subscribes to a valid billing plan
- **THEN** Profiling assigns the plan's access profile to that user

### Requirement: Setup values use seeded identities

Access-profile sources MUST be seeded Profiling setup entities and user assignments MUST store the source ID. Billing currency, billing interval, billing-account status, and subscription status MUST be seeded Billing setup entities and plans, accounts, or subscriptions MUST store their IDs rather than arbitrary strings. Billing MUST send the stable plan code to Profiling when an access-profile assignment or revision is requested.

#### Scenario: Setup identity is persisted
- **WHEN** a plan or user access-profile assignment is created
- **THEN** the persisted record references valid seeded setup IDs

### Requirement: Administrators may opt into plan access-profile cascade

An administrator-only operation MUST change a plan's access profile and accept an explicit cascade option. Without cascade, existing assignments remain unchanged. With cascade, all active subscriptions for that plan MUST be revised for their users.

#### Scenario: Administrator enables cascade
- **WHEN** an authorized administrator changes a plan with cascade enabled
- **THEN** all active subscriptions for that plan receive a durable access-profile revision request

#### Scenario: Administrator disables cascade
- **WHEN** an authorized administrator changes a plan with cascade disabled
- **THEN** existing user assignments remain unchanged

### Requirement: Cascade revision is durable and idempotent

Plan revisions MUST be durably recorded, retryable, auditable, and idempotent. Reprocessing MUST not create duplicate active assignments or alter users of other plans.

#### Scenario: Revision is retried
- **WHEN** the same revision is delivered more than once
- **THEN** the result contains no duplicate active assignment and no unrelated user is changed

### Requirement: Expired or disabled assignments grant no capabilities

Capability resolution MUST ignore disabled assignments and assignments whose validity has expired.

#### Scenario: Expired assignment is resolved
- **WHEN** a user's access-profile assignment is disabled or past its validity date
- **THEN** its capabilities are excluded from the effective access set
